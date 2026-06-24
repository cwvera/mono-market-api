namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Arma y envía el correo correspondiente a un aviso de recordatorio, si tiene plantilla asignada.
/// </summary>
public interface IInvoiceReminderEmailDispatcher
{
    /// <summary>
    /// Si el aviso tiene plantilla, crea la cabecera en SendMails, arma el contenido y lo envía.
    /// Si la transición es silenciosa (sin plantilla), no hace nada.
    /// Devuelve true si la transición puede persistirse (no había plantilla, o el correo se envió con
    /// éxito); false si había plantilla pero el envío falló tras agotar los reintentos.
    /// </summary>
    Task<bool> DispatchAsync(InvoiceReminderNotice notice, CancellationToken cancellationToken);
}
