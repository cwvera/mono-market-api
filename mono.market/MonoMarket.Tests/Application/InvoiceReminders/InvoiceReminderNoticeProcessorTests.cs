using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MonoMarket.Application.Clients.Repositories;
using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.InvoiceReminders.StageHandlers;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.InvoiceReminders;

/// <summary>
/// Pruebas del procesador de avisos: decide si llamar al cliente y construir el aviso, según la factory.
/// </summary>
public class InvoiceReminderNoticeProcessorTests
{
    private readonly Mock<IInvoiceReminderStageHandlerFactory> _factoryMock = new();
    private readonly Mock<IClientRepository> _clientRepositoryMock = new();
    private readonly Mock<IInvoiceReminderEmailDispatcher> _emailDispatcherMock = new();
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock = new();

    private InvoiceReminderNoticeProcessor CreateProcessor() => new(
        _factoryMock.Object,
        _clientRepositoryMock.Object,
        _emailDispatcherMock.Object,
        _invoiceRepositoryMock.Object,
        NullLogger<InvoiceReminderNoticeProcessor>.Instance);

    /// <summary>
    /// Verifica que, si el estado de la factura no tiene handler, no se consulte al cliente.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_NoHandlerForStatus_DoesNotQueryClient()
    {
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns((IInvoiceReminderStageHandler?)null);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(new Invoice { Status = (int)InvoiceStatus.Paid }, CancellationToken.None);

        _clientRepositoryMock.Verify(r => r.GetByIdentificationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifica que, si la factura todavía no cumple el plazo de la etapa, no se consulte al cliente.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ThresholdNotMet_DoesNotQueryClient()
    {
        Mock<IInvoiceReminderStageHandler> handlerMock = new();
        handlerMock.Setup(h => h.IsDue(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns(handlerMock.Object);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(new Invoice { Status = (int)InvoiceStatus.FirstReminder }, CancellationToken.None);

        _clientRepositoryMock.Verify(r => r.GetByIdentificationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifica que, si cumple el plazo y el cliente existe, se construya el aviso (sin lanzar excepciones).
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ThresholdMetAndClientExists_BuildsNotice()
    {
        Invoice invoice = new() { InvoiceNumber = "FAC-001-2025", ClientIdentification = "900123456-7", Status = (int)InvoiceStatus.FirstReminder };
        Client client = new() { Name = "Empresa Alpha SAS", Email = "contacto@alpha.com" };
        Mock<IInvoiceReminderStageHandler> handlerMock = new();
        handlerMock.Setup(h => h.IsDue(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(true);
        handlerMock
            .Setup(h => h.BuildNotice(It.IsAny<InvoiceReminderCandidate>()))
            .Returns((InvoiceReminderCandidate candidate) => new InvoiceReminderNotice { Candidate = candidate });
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns(handlerMock.Object);
        _clientRepositoryMock
            .Setup(r => r.GetByIdentificationAsync(invoice.ClientIdentification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(invoice, CancellationToken.None);

        handlerMock.Verify(
            h => h.BuildNotice(It.Is<InvoiceReminderCandidate>(c =>
                c.InvoiceNumber == invoice.InvoiceNumber && c.ClientName == client.Name && c.ClientEmail == client.Email)),
            Times.Once);
        _emailDispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<InvoiceReminderNotice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifica que, si el despacho del correo puede avanzar (sin plantilla, o envío exitoso),
    /// se persista el siguiente estado de la factura.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_WhenDispatchCanAdvance_UpdatesInvoiceStatus()
    {
        Invoice invoice = new() { Id = "invoice-1", ClientIdentification = "900123456-7", Status = (int)InvoiceStatus.FirstReminder };
        Client client = new() { Name = "Empresa Alpha SAS", Email = "contacto@alpha.com" };
        Mock<IInvoiceReminderStageHandler> handlerMock = new();
        handlerMock.Setup(h => h.IsDue(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(true);
        handlerMock
            .Setup(h => h.BuildNotice(It.IsAny<InvoiceReminderCandidate>()))
            .Returns(new InvoiceReminderNotice { NextStatus = InvoiceStatus.SecondReminder });
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns(handlerMock.Object);
        _clientRepositoryMock
            .Setup(r => r.GetByIdentificationAsync(invoice.ClientIdentification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _emailDispatcherMock
            .Setup(d => d.DispatchAsync(It.IsAny<InvoiceReminderNotice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(invoice, CancellationToken.None);

        _invoiceRepositoryMock.Verify(
            r => r.UpdateStatusAsync(invoice.Id, (int)InvoiceStatus.SecondReminder, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, si el correo no se pudo enviar (reintentos agotados), no se persista el avance
    /// de estado, para que la próxima corrida vuelva a intentarlo.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_WhenDispatchFails_DoesNotUpdateInvoiceStatus()
    {
        Invoice invoice = new() { Id = "invoice-1", ClientIdentification = "900123456-7", Status = (int)InvoiceStatus.FirstReminder };
        Client client = new() { Name = "Empresa Alpha SAS", Email = "contacto@alpha.com" };
        Mock<IInvoiceReminderStageHandler> handlerMock = new();
        handlerMock.Setup(h => h.IsDue(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(true);
        handlerMock
            .Setup(h => h.BuildNotice(It.IsAny<InvoiceReminderCandidate>()))
            .Returns(new InvoiceReminderNotice { NextStatus = InvoiceStatus.SecondReminder });
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns(handlerMock.Object);
        _clientRepositoryMock
            .Setup(r => r.GetByIdentificationAsync(invoice.ClientIdentification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _emailDispatcherMock
            .Setup(d => d.DispatchAsync(It.IsAny<InvoiceReminderNotice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(invoice, CancellationToken.None);

        _invoiceRepositoryMock.Verify(
            r => r.UpdateStatusAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, si cumple el plazo pero el cliente no existe, no se intente construir el aviso.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ThresholdMetButClientMissing_DoesNotBuildNotice()
    {
        Invoice invoice = new() { ClientIdentification = "NO-EXISTE", Status = (int)InvoiceStatus.FirstReminder };
        Mock<IInvoiceReminderStageHandler> handlerMock = new();
        handlerMock.Setup(h => h.IsDue(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(true);
        _factoryMock.Setup(f => f.GetHandler(It.IsAny<int>())).Returns(handlerMock.Object);
        _clientRepositoryMock
            .Setup(r => r.GetByIdentificationAsync(invoice.ClientIdentification, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);
        InvoiceReminderNoticeProcessor processor = CreateProcessor();

        await processor.ProcessAsync(invoice, CancellationToken.None);

        handlerMock.Verify(h => h.BuildNotice(It.IsAny<InvoiceReminderCandidate>()), Times.Never);
    }
}
