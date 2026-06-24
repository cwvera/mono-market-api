using Moq;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Application.Invoices.Queries.GetByNumber;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.Invoices.Queries.GetByNumber;

/// <summary>
/// Pruebas del handler de GetByNumberQuery.
/// </summary>
public class GetByNumberQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _repositoryMock = new();

    /// <summary>
    /// Verifica que el handler devuelva el DTO mapeado cuando la factura existe.
    /// </summary>
    [Fact]
    public async Task Handle_ExistingInvoice_ReturnsDto()
    {
        Invoice invoice = new() { InvoiceNumber = "FAC-001-2025", ClientIdentification = "900123456-7" };
        _repositoryMock
            .Setup(r => r.GetByNumberAsync("FAC-001-2025", It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        GetByNumberQueryHandler handler = new GetByNumberQueryHandler(_repositoryMock.Object);

        InvoiceDto? result = await handler.Handle(new GetByNumberQuery("FAC-001-2025"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("FAC-001-2025", result!.InvoiceNumber);
    }

    /// <summary>
    /// Verifica que el handler devuelva null cuando la factura no existe.
    /// </summary>
    [Fact]
    public async Task Handle_NonExistingInvoice_ReturnsNull()
    {
        _repositoryMock
            .Setup(r => r.GetByNumberAsync("NO-EXISTE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        GetByNumberQueryHandler handler = new GetByNumberQueryHandler(_repositoryMock.Object);

        InvoiceDto? result = await handler.Handle(new GetByNumberQuery("NO-EXISTE"), CancellationToken.None);

        Assert.Null(result);
    }
}
