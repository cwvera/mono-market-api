using Moq;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.SendMails.Queries.GetByInvoiceNumber;

/// <summary>
/// Pruebas del handler de GetByInvoiceNumberQuery.
/// </summary>
public class GetByInvoiceNumberQueryHandlerTests
{
    private readonly Mock<ISendMailRepository> _repositoryMock = new();

    /// <summary>
    /// Verifica que el handler mapee a DTO todos los correos devueltos por el repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsMappedDtoList()
    {
        List<SendMail> sendMails =
        [
            new SendMail { InvoiceNumber = "FAC-001-2025", ToEmail = "contacto@alpha.com" },
            new SendMail { InvoiceNumber = "FAC-001-2025", ToEmail = "otro@alpha.com" },
        ];
        _repositoryMock
            .Setup(r => r.GetByInvoiceNumberAsync("FAC-001-2025", It.IsAny<CancellationToken>()))
            .ReturnsAsync(sendMails);
        GetByInvoiceNumberQueryHandler handler = new(_repositoryMock.Object);

        List<SendMailDto> result = await handler.Handle(new GetByInvoiceNumberQuery("FAC-001-2025"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.ToEmail == "contacto@alpha.com");
        Assert.Contains(result, dto => dto.ToEmail == "otro@alpha.com");
    }

    /// <summary>
    /// Verifica que el handler reenvíe el número de factura al repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_ForwardsInvoiceNumberToRepository()
    {
        _repositoryMock
            .Setup(r => r.GetByInvoiceNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        GetByInvoiceNumberQueryHandler handler = new(_repositoryMock.Object);

        await handler.Handle(new GetByInvoiceNumberQuery("FAC-001-2025"), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByInvoiceNumberAsync("FAC-001-2025", It.IsAny<CancellationToken>()), Times.Once);
    }
}
