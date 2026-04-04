namespace ERP.Finance.Domain.Entities;

public enum InvoiceStatus
{
    Draft,
    Issued,
    Paid,
    Overdue,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    BankTransfer,
    Cheque,
    Others
}
