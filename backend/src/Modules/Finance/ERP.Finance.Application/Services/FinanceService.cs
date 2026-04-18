using ERP.Finance.Application.DTOs;
using ERP.Finance.Application.Repositories;
using ERP.Purchasing.Application.Repositories;
using ERP.Finance.Application.Services.Interfaces;
using ERP.Finance.Domain.Entities;
using ERP.Shared;
using ERP.Shared.Exceptions;

namespace ERP.Finance.Application.Services;

public class FinanceService : IFinanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ISupplierRepository _supplierRepository;
    public FinanceService(
        IUnitOfWork unitOfWork,
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IAccountRepository accountRepository
        , ISupplierRepository supplierRepository)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _accountRepository = accountRepository;
        _supplierRepository = supplierRepository;
    }

    // Invoice
    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        return invoices.Select(MapToInvoiceDto);
    }

    public async Task<PagedResult<InvoiceDto>> SearchInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var result = await _invoiceRepository.SearchInvoicesAsync(filter, cancellationToken);
        return new PagedResult<InvoiceDto>
        {
            Items = result.Items.Select(MapToInvoiceDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        return invoice == null ? null : MapToInvoiceDto(invoice);
    }

    public async Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByInvoiceNumberAsync(invoiceNumber, cancellationToken);
        return invoice == null ? null : MapToInvoiceDto(invoice);
    }

    public async Task<Guid> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validation: ต้องระบุอย่างใดอย่างหนึ่ง (Supplier หรือ Customer)
        if (dto.SupplierId == null && dto.CustomerId == null)
        {
            throw new BadRequestException("Invoice must be linked to either a Supplier or a Customer.");
        }

        // 2. ป้องกันยอดรวมและยอดค้างชำระเป็น 0 หรือติดลบ
        if (dto.TotalAmount <= 0)
        {
            throw new BadRequestException("Invoice total amount must be greater than zero.");
        }

        if (dto.AmountDue <= 0)
        {
            throw new BadRequestException("Invoice amount due must be greater than zero.");
        }

        if (dto.AmountDue > dto.TotalAmount)
        {
            throw new BadRequestException("ยอดค้างชำระต้องไม่เกินยอดรวม");
        }

        var invoice = new Invoice
        {
            InvoiceNumber = dto.InvoiceNumber,
            SupplierId = dto.SupplierId,
            CustomerId = dto.CustomerId,
            PurchaseOrderId = dto.PurchaseOrderId,
            TotalAmount = dto.TotalAmount,
            AmountDue = dto.AmountDue,
            InvoiceDate = dto.InvoiceDate.ToUniversalTime(),
            DueDate = dto.DueDate.ToUniversalTime(),
            Status = InvoiceStatus.Issued,
            Description = $"ใบแจ้งหนี้อัตโนมัติจาก PO: {dto.PurchaseOrderId}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // 4. บันทึกลง Database
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }

    public async Task<bool> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice == null) return false;

        if (invoice.Status == InvoiceStatus.Paid)
        {
            throw new BadRequestException("ไม่สามารถแก้ไข Invoice ที่ชำระเงินเสร็จสิ้นแล้วได้");
        }
        if (dto.TotalAmount.HasValue)
        {
            if (dto.TotalAmount.Value < invoice.AmountDue)
            {
                throw new BadRequestException("ยอดรวมต้องไม่น้อยกว่ายอดค้างชำระ");
            }
            if (dto.TotalAmount.Value > 0)
            {
                invoice.TotalAmount = dto.TotalAmount.Value;
            }
        }
        if (dto.AmountDue.HasValue && dto.AmountDue.Value > 0)
        {
            invoice.AmountDue = dto.AmountDue.Value;
        }
        if (dto.DueDate != default)
        {
            invoice.DueDate = dto.DueDate;
        }
        if (dto.Description != null)
        {
            invoice.Description = dto.Description;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            if (Enum.TryParse<InvoiceStatus>(dto.Status, true, out var status))
            {
                invoice.Status = status;
            }
            else
            {
                throw new BadRequestException($"สถานะ '{dto.Status}' ไม่ถูกต้อง");
            }
        }

        invoice.LastModifiedAt = DateTime.UtcNow;
        invoice.LastModifiedBy = "System";

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<bool> SetInvoicePaidAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice == null) return false;

        invoice.Status = InvoiceStatus.Paid;
        invoice.LastModifiedAt = DateTime.UtcNow;

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // Payment
    public async Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken);
        return payments.Select(MapToPaymentDto);
    }

    public async Task<Guid> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.AmountPaid <= 0)
        {
            throw new BadRequestException("จำนวนเงินที่ชำระต้องมากกว่า 0");
        }

        var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId, cancellationToken);
        if (invoice == null) throw new NotFoundException("Invoice not found");

        if (dto.AmountPaid > invoice.AmountDue)
        {
            throw new BadRequestException("จำนวนเงินที่ชำระเกินยอดค้างชำระ");
        }

        var account = await _accountRepository.GetByIdAsync(dto.AccountId, cancellationToken);
        if (account == null) throw new NotFoundException("Account not found");

        if (account.Balance < dto.AmountPaid)
        {
            throw new BadRequestException("ยอดเงินในบัญชีไม่เพียงพอ");
        }

        var payment = new Payment
        {
            InvoiceId = dto.InvoiceId,
            AccountId = dto.AccountId,
            AmountPaid = dto.AmountPaid,
            PaymentDate = dto.PaymentDate.ToUniversalTime(),
            PaymentMethod = Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var method) ? method : PaymentMethod.Others,
            ReferenceNumber = dto.ReferenceNumber,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);

        invoice.AmountDue -= dto.AmountPaid;
        invoice.AmountDue = Math.Max(0, invoice.AmountDue);
        if (invoice.AmountDue <= 0)
        {
            invoice.Status = InvoiceStatus.Paid;
        }

        _invoiceRepository.Update(invoice);

        account.Balance -= dto.AmountPaid;
        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return payment.Id;
    }

    // Account
    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        return accounts.Select(MapToAccountDto);
    }

    public async Task<AccountDto?> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
        return account == null ? null : MapToAccountDto(account);
    }

    public async Task<Guid> CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            AccountName = dto.AccountName,
            AccountCode = dto.AccountCode,
            Balance = dto.Balance,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }

    private static InvoiceDto MapToInvoiceDto(Invoice invoice) => new()
    {
        Id = invoice.Id,
        InvoiceNumber = invoice.InvoiceNumber,
        CustomerId = invoice.CustomerId ?? Guid.Empty,
        SupplierId = invoice.SupplierId ?? Guid.Empty,
        AccountId = invoice.AccountId,
        PurchaseOrderId = invoice.PurchaseOrderId,
        TotalAmount = invoice.TotalAmount,
        Description = invoice.Description,
        AmountDue = invoice.AmountDue,
        InvoiceDate = invoice.InvoiceDate.ToUniversalTime(),
        DueDate = invoice.DueDate.ToUniversalTime(),
        Status = invoice.Status.ToString()
    };

    private static PaymentDto MapToPaymentDto(Payment payment) => new()
    {
        Id = payment.Id,
        InvoiceId = payment.InvoiceId,
        AccountId = payment.AccountId,
        AmountPaid = payment.AmountPaid,
        PaymentDate = payment.PaymentDate.ToUniversalTime(),
        PaymentMethod = payment.PaymentMethod.ToString(),
        ReferenceNumber = payment.ReferenceNumber
    };

    private static AccountDto MapToAccountDto(Account account) => new()
    {
        Id = account.Id,
        AccountName = account.AccountName,
        AccountCode = account.AccountCode,
        Balance = account.Balance
    };
}
