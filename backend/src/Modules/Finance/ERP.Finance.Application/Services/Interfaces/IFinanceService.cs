using ERP.Finance.Application.DTOs;
using ERP.Shared;

namespace ERP.Finance.Application.Services.Interfaces;

public interface IFinanceService
{
    // Invoice
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<InvoiceDto>> SearchInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken = default);
    Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Guid> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> SetInvoicePaidAsync(Guid id, CancellationToken cancellationToken = default);

    // Payment
    Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Guid> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default);

    // Account
    Task<IEnumerable<AccountDto>> GetAllAccountsAsync(CancellationToken cancellationToken = default);
    Task<AccountDto?> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
}