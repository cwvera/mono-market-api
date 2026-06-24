db.Clients.insertOne({
  _id: ObjectId("000000000000000000000001"),
  identification: "900123456-7",
  name: "Empresa Alpha SAS",
  email: "contacto@alpha.com",
  phone: "3001234567",
  createdAt: ISODate("2025-01-01T00:00:00Z"),
  updatedAt: ISODate("2025-01-01T00:00:00Z")
});

db.Invoices.insertOne({
  _id: ObjectId("100000000000000000000001"),
  invoiceNumber: "FAC-001-2025",
  clientIdentification: "900123456-7",
  amount: NumberDecimal("1500000"),
  issueDate: ISODate("2025-02-15T00:00:00Z"),
  status: 1,
  lastReminderSentAt: null,
  reminderCount: 0,
  createdAt: ISODate("2025-02-15T00:00:00Z"),
  updatedAt: ISODate("2025-02-15T00:00:00Z")
});

db.SendMails.insertOne({
  _id: ObjectId("300000000000000000000001"),
  invoiceNumber: "FAC-001-2025",
  toEmail: "contacto@alpha.com",
  toName: "Empresa Alpha SAS",
  templateType: 0,
  subject: "Segundo Recordatorio de Pago",
  status: 1,
  totalAttempts: 1,
  lastAttemptAt: ISODate("2025-03-16T10:30:00Z"),
  data: "Factura FAC-001-2025 | FirstReminder -> SecondReminder",
  createdAt: ISODate("2025-03-16T10:00:00Z"),
  updatedAt: ISODate("2025-03-16T10:30:00Z")
});

db.SendMailsLog.insertOne({
  _id: ObjectId("400000000000000000000001"),
  sendMailId: ObjectId("300000000000000000000001"),
  attemptNumber: 1,
  status: 0,
  errorMessage: null,
  durationMs: 1250,
  sentAt: ISODate("2025-03-16T10:30:00Z"),
  createdAt: ISODate("2025-03-16T10:30:00Z")
});
