using Mapster;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Persistence.Documents;

namespace MonoMarket.Tests.Infrastructure.Persistence;

/// <summary>
/// Pruebas del mapeo de InvoiceDocument (persistencia) a Invoice (dominio) con Mapster.
/// </summary>
public class InvoiceDocumentMappingTests
{
    /// <summary>
    /// Verifica que Adapt copie todos los campos del documento a la entidad de dominio.
    /// </summary>
    [Fact]
    public void Adapt_CopiesAllFields()
    {
        DateTime issueDate = new(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc);
        DateTime lastReminderSentAt = new(2025, 3, 16, 10, 30, 0, DateTimeKind.Utc);
        InvoiceDocument document = new()
        {
            Id = "100000000000000000000001",
            InvoiceNumber = "FAC-001-2025",
            ClientIdentification = "900123456-7",
            Amount = 1500000m,
            IssueDate = issueDate,
            Status = 2,
            LastReminderSentAt = lastReminderSentAt,
            ReminderCount = 1,
            CreatedAt = issueDate,
            UpdatedAt = lastReminderSentAt,
        };

        Invoice invoice = document.Adapt<Invoice>();

        Assert.Equal(document.Id, invoice.Id);
        Assert.Equal(document.InvoiceNumber, invoice.InvoiceNumber);
        Assert.Equal(document.ClientIdentification, invoice.ClientIdentification);
        Assert.Equal(document.Amount, invoice.Amount);
        Assert.Equal(document.IssueDate, invoice.IssueDate);
        Assert.Equal(document.Status, invoice.Status);
        Assert.Equal(document.LastReminderSentAt, invoice.LastReminderSentAt);
        Assert.Equal(document.ReminderCount, invoice.ReminderCount);
        Assert.Equal(document.CreatedAt, invoice.CreatedAt);
        Assert.Equal(document.UpdatedAt, invoice.UpdatedAt);
    }
}
