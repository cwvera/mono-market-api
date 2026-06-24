using MonoMarket.Application.InvoiceReminders.EmailTemplates;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Tests.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Pruebas de la factory de plantillas: debe devolver la plantilla correcta por tipo, o lanzar si no existe.
/// </summary>
public class InvoiceReminderEmailTemplateFactoryTests
{
    private readonly InvoiceReminderEmailTemplateFactory _factory = new(
    [
        new SecondReminderEmailTemplate(),
        new DeactivationEmailTemplate(),
    ]);

    /// <summary>
    /// Verifica que cada tipo registrado devuelva la plantilla correcta.
    /// </summary>
    [Theory]
    [InlineData(SendMailTemplateType.SecondReminder, typeof(SecondReminderEmailTemplate))]
    [InlineData(SendMailTemplateType.Deactivation, typeof(DeactivationEmailTemplate))]
    public void GetTemplate_WithRegisteredType_ReturnsCorrectTemplate(SendMailTemplateType templateType, Type expectedTemplateType)
    {
        IInvoiceReminderEmailTemplate template = _factory.GetTemplate(templateType);

        Assert.IsType(expectedTemplateType, template);
    }

    /// <summary>
    /// Verifica que un tipo sin plantilla registrada lance una excepción clara.
    /// </summary>
    [Fact]
    public void GetTemplate_WithUnregisteredType_Throws()
    {
        InvoiceReminderEmailTemplateFactory emptyFactory = new([]);

        Assert.Throws<InvalidOperationException>(() => emptyFactory.GetTemplate(SendMailTemplateType.SecondReminder));
    }
}
