db.createCollection("Clients", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["identification", "name", "email", "createdAt", "updatedAt"],
      properties: {
        identification: { bsonType: "string" },
        name: { bsonType: "string" },
        email: { bsonType: "string" },
        phone: { bsonType: "string" },
        createdAt: { bsonType: "date" },
        updatedAt: { bsonType: "date" }
      }
    }
  }
});
db.Clients.createIndex({ identification: 1 }, { unique: true });
db.Clients.createIndex({ email: 1 }, { unique: true });

db.createCollection("Invoices", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["invoiceNumber", "clientIdentification", "amount", "issueDate", "status", "reminderCount", "createdAt", "updatedAt"],
      properties: {
        invoiceNumber: { bsonType: "string" },
        clientIdentification: { bsonType: "string" },
        amount: { bsonType: "decimal" },
        issueDate: { bsonType: "date" },
        status: { bsonType: "int" },
        lastReminderSentAt: { bsonType: ["date", "null"] },
        reminderCount: { bsonType: "int" },
        createdAt: { bsonType: "date" },
        updatedAt: { bsonType: "date" }
      }
    }
  }
});
db.Invoices.createIndex({ invoiceNumber: 1 }, { unique: true });
db.Invoices.createIndex({ clientIdentification: 1 });
db.Invoices.createIndex({ status: 1 });
db.Invoices.createIndex({ clientIdentification: 1, status: 1 });

db.createCollection("SendMails", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["invoiceNumber", "toEmail", "toName", "templateType", "subject", "status", "totalAttempts", "data", "createdAt", "updatedAt"],
      properties: {
        invoiceNumber: { bsonType: "string" },
        toEmail: { bsonType: "string" },
        toName: { bsonType: "string" },
        templateType: { bsonType: "int" },
        subject: { bsonType: "string" },
        status: { bsonType: "int" },
        totalAttempts: { bsonType: "int" },
        lastAttemptAt: { bsonType: ["date", "null"] },
        data: { bsonType: "string" },
        createdAt: { bsonType: "date" },
        updatedAt: { bsonType: "date" }
      }
    }
  }
});
db.SendMails.createIndex({ invoiceNumber: 1 });
db.SendMails.createIndex({ toEmail: 1 });
db.SendMails.createIndex({ status: 1 });
db.SendMails.createIndex({ createdAt: -1 });

db.createCollection("SendMailsLog", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["sendMailId", "attemptNumber", "status", "createdAt"],
      properties: {
        sendMailId: { bsonType: "objectId" },
        attemptNumber: { bsonType: "int" },
        status: { bsonType: "int" },
        errorMessage: { bsonType: ["string", "null"] },
        durationMs: { bsonType: "int" },
        sentAt: { bsonType: "date" },
        createdAt: { bsonType: "date" }
      }
    }
  }
});
db.SendMailsLog.createIndex({ sendMailId: 1 });
db.SendMailsLog.createIndex({ sendMailId: 1, attemptNumber: 1 });
