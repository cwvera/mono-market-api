// Datos de prueba para depurar el job de escaneo de recordatorios (InvoiceReminderScanJob).
// Las fechas se calculan relativas al momento en que corre el contenedor (no son fijas),
// para que los umbrales de 30/60/90 días sigan siendo válidos sin importar cuándo se levante.
function daysAgo(days) {
  return new Date(Date.now() - days * 24 * 60 * 60 * 1000);
}

db.Clients.insertMany([
  {
    _id: ObjectId("000000000000000000000010"),
    identification: "800111222-3",
    name: "Comercial Beta Ltda",
    email: "monomarket-pruebas-beta@yopmail.com",
    phone: "3009876543",
    createdAt: daysAgo(120),
    updatedAt: daysAgo(120)
  },
  {
    _id: ObjectId("000000000000000000000011"),
    identification: "800222333-4",
    name: "Distribuidora Gamma SA",
    email: "monomarket-pruebas-gamma@yopmail.com",
    phone: "3018765432",
    createdAt: daysAgo(120),
    updatedAt: daysAgo(120)
  },
  {
    _id: ObjectId("000000000000000000000012"),
    identification: "800333444-5",
    name: "Inversiones Delta SAS",
    email: "monomarket-pruebas-delta@yopmail.com",
    phone: "3027654321",
    createdAt: daysAgo(120),
    updatedAt: daysAgo(120)
  }
]);

db.Invoices.insertMany([
  // 3 días: todavía no cumple el umbral de 30 días de Pendiente -> el job la trae pero no debe avisar nada.
  {
    _id: ObjectId("100000000000000000000010"),
    invoiceNumber: "FAC-100-2026",
    clientIdentification: "800111222-3",
    amount: NumberDecimal("500000"),
    issueDate: daysAgo(3),
    status: -1, // Pending
    lastReminderSentAt: null,
    reminderCount: 0,
    createdAt: daysAgo(3),
    updatedAt: daysAgo(3)
  },
  // 30 días: cumple el umbral de Pendiente -> PrimerRecordatorio (transición silenciosa, sin plantilla).
  {
    _id: ObjectId("100000000000000000000011"),
    invoiceNumber: "FAC-101-2026",
    clientIdentification: "800111222-3",
    amount: NumberDecimal("750000"),
    issueDate: daysAgo(30),
    status: -1, // Pending
    lastReminderSentAt: null,
    reminderCount: 0,
    createdAt: daysAgo(30),
    updatedAt: daysAgo(30)
  },
  // 60 días: cumple el umbral de PrimerRecordatorio -> SegundoRecordatorio (plantilla SecondReminder).
  {
    _id: ObjectId("100000000000000000000012"),
    invoiceNumber: "FAC-102-2026",
    clientIdentification: "800222333-4",
    amount: NumberDecimal("1200000"),
    issueDate: daysAgo(60),
    status: 1, // FirstReminder
    lastReminderSentAt: null,
    reminderCount: 0,
    createdAt: daysAgo(60),
    updatedAt: daysAgo(60)
  },
  // 90 días: cumple el umbral de SegundoRecordatorio -> Deactivated (plantilla Deactivation).
  {
    _id: ObjectId("100000000000000000000013"),
    invoiceNumber: "FAC-103-2026",
    clientIdentification: "800333444-5",
    amount: NumberDecimal("2000000"),
    issueDate: daysAgo(90),
    status: 2, // SecondReminder
    lastReminderSentAt: null,
    reminderCount: 0,
    createdAt: daysAgo(90),
    updatedAt: daysAgo(90)
  },
  // Pagada: no tiene etapa en el factory, el job no debe hacer nada con ella sin importar la fecha.
  {
    _id: ObjectId("100000000000000000000014"),
    invoiceNumber: "FAC-104-2026",
    clientIdentification: "800333444-5",
    amount: NumberDecimal("300000"),
    issueDate: daysAgo(150),
    status: 0, // Paid
    lastReminderSentAt: null,
    reminderCount: 0,
    createdAt: daysAgo(150),
    updatedAt: daysAgo(150)
  }
]);
