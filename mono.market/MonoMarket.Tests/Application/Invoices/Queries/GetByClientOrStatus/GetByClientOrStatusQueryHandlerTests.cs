using Moq;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.Invoices.Queries.GetByClientOrStatus;

/// <summary>
/// Pruebas del handler de GetByClientOrStatusQuery.
/// </summary>
public class GetByClientOrStatusQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _repositoryMock = new();

    /// <summary>
    /// Verifica que el handler mapee a DTO todas las facturas devueltas por el repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsMappedDtoList()
    {
        List<Invoice> invoices =
        [
            new Invoice { InvoiceNumber = "FAC-001-2025", ClientIdentification = "900123456-7" },
            new Invoice { InvoiceNumber = "FAC-002-2025", ClientIdentification = "900123456-7" },
        ];
        _repositoryMock
            .Setup(r => r.GetByClientOrStatusAsync("900123456-7", (int)InvoiceStatus.SecondReminder, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);
        GetByClientOrStatusQueryHandler handler = new(_repositoryMock.Object);

        List<InvoiceDto> result = await handler.Handle(new GetByClientOrStatusQuery("900123456-7", InvoiceStatus.SecondReminder), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.InvoiceNumber == "FAC-001-2025");
        Assert.Contains(result, dto => dto.InvoiceNumber == "FAC-002-2025");
    }

    /// <summary>
    /// Verifica que el handler reenvíe los filtros de cliente y estado al repositorio, convirtiendo el enum a int.
    /// </summary>
    [Fact]
    public async Task Handle_ForwardsFiltersToRepository()
    {
        _repositoryMock
            .Setup(r => r.GetByClientOrStatusAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        GetByClientOrStatusQueryHandler handler = new(_repositoryMock.Object);

        await handler.Handle(new GetByClientOrStatusQuery("900123456-7", InvoiceStatus.SecondReminder), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByClientOrStatusAsync("900123456-7", (int)InvoiceStatus.SecondReminder, It.IsAny<CancellationToken>()), Times.Once);
    }
}
