using Moq;
using ERP.Finance.Application.DTOs;
using ERP.Finance.Application.Repositories;
using ERP.Finance.Application.Services;
using ERP.Finance.Domain.Entities;
using ERP.Purchasing.Application.Repositories;
using ERP.Shared;
using ERP.Shared.Exceptions;
using Xunit;

namespace ERP.Tests.Finance;

public class FinanceServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock = new();
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new();
    private readonly Mock<IAccountRepository> _accountRepoMock = new();
    private readonly Mock<ISupplierRepository> _supplierRepoMock = new();
    private readonly FinanceService _service;

    public FinanceServiceTests()
    {
        _service = new FinanceService(
            _uowMock.Object,
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
            _accountRepoMock.Object,
            _supplierRepoMock.Object);
    }

    // ─── Invoice ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllInvoicesAsync_WhenInvoicesExist_ReturnsMappedDtos()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", CustomerId = Guid.NewGuid(),
                    TotalAmount = 1000, AmountDue = 1000, Status = InvoiceStatus.Issued },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", SupplierId = Guid.NewGuid(),
                    TotalAmount = 2000, AmountDue = 2000, Status = InvoiceStatus.Issued },
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(invoices);

        // Act
        var result = await _service.GetAllInvoicesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.InvoiceNumber == "INV-001");
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenInvoiceExists_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = id,
            InvoiceNumber = "INV-001",
            CustomerId = Guid.NewGuid(),
            TotalAmount = 500,
            AmountDue = 500,
            Status = InvoiceStatus.Issued
        };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(invoice);

        // Act
        var result = await _service.GetInvoiceByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INV-001", result.InvoiceNumber);
        Assert.Equal(500, result.TotalAmount);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                        .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _service.GetInvoiceByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidData_ReturnsNewId()
    {
        // Arrange
        var dto = new CreateInvoiceDto
        {
            InvoiceNumber = "INV-003",
            CustomerId = Guid.NewGuid(),
            TotalAmount = 1500,
            AmountDue = 1500,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _service.CreateInvoiceAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _invoiceRepoMock.Verify(r => r.AddAsync(It.IsAny<Invoice>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithoutSupplierAndCustomer_ThrowsBadRequest()
    {
        // Arrange
        var dto = new CreateInvoiceDto
        {
            InvoiceNumber = "INV-ERR",
            SupplierId = null,
            CustomerId = null,
            TotalAmount = 1000,
            AmountDue = 1000
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateInvoiceAsync(dto));
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithZeroTotalAmount_ThrowsBadRequest()
    {
        // Arrange
        var dto = new CreateInvoiceDto
        {
            CustomerId = Guid.NewGuid(),
            TotalAmount = 0,
            AmountDue = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateInvoiceAsync(dto));
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenAmountDueExceedsTotalAmount_ThrowsBadRequest()
    {
        // Arrange
        var dto = new CreateInvoiceDto
        {
            CustomerId = Guid.NewGuid(),
            TotalAmount = 500,
            AmountDue = 999   // เกิน TotalAmount
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateInvoiceAsync(dto));
    }

    [Fact]
    public async Task UpdateInvoiceAsync_WhenInvoiceIsPaid_ThrowsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var paidInvoice = new Invoice { Id = id, Status = InvoiceStatus.Paid, AmountDue = 0 };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(paidInvoice);

        var dto = new UpdateInvoiceDto { TotalAmount = 999 };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateInvoiceAsync(id, dto));
    }

    [Fact]
    public async Task SetInvoicePaidAsync_WhenInvoiceExists_UpdatesStatusAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();
        var invoice = new Invoice { Id = id, Status = InvoiceStatus.Issued };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(invoice);

        // Act
        var result = await _service.SetInvoicePaidAsync(id);

        // Assert
        Assert.True(result);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task SetInvoicePaidAsync_WhenInvoiceNotFound_ReturnsFalse()
    {
        // Arrange
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                        .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _service.SetInvoicePaidAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    // ─── Payment ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePaymentAsync_WithValidData_CreatesPaymentAndUpdatesInvoice()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var invoice = new Invoice
        {
            Id = invoiceId,
            AmountDue = 1000,
            Status = InvoiceStatus.Issued
        };
        var account = new Account { Id = accountId, Balance = 5000 };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, default)).ReturnsAsync(invoice);
        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, default)).ReturnsAsync(account);

        var dto = new CreatePaymentDto
        {
            InvoiceId = invoiceId,
            AccountId = accountId,
            AmountPaid = 500,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = "BankTransfer"
        };

        // Act
        var result = await _service.CreatePaymentAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.Equal(500, invoice.AmountDue);   // ลดลงจาก 1000
        Assert.Equal(4500, account.Balance);    // ลดลงจาก 5000
        _paymentRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), default), Times.Once);
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenAmountExceedsDue_ThrowsBadRequest()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, AmountDue = 200, Status = InvoiceStatus.Issued };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, default)).ReturnsAsync(invoice);

        var dto = new CreatePaymentDto
        {
            InvoiceId = invoiceId,
            AccountId = Guid.NewGuid(),
            AmountPaid = 999   // เกิน AmountDue
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePaymentAsync(dto));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenAccountBalanceInsufficient_ThrowsBadRequest()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, default))
            .ReturnsAsync(new Invoice { Id = invoiceId, AmountDue = 1000 });
        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, default))
            .ReturnsAsync(new Account { Id = accountId, Balance = 10 }); // เงินไม่พอ

        var dto = new CreatePaymentDto
        {
            InvoiceId = invoiceId,
            AccountId = accountId,
            AmountPaid = 500
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePaymentAsync(dto));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenFullyPaid_SetsInvoiceStatusToPaid()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var invoice = new Invoice { Id = invoiceId, AmountDue = 500, Status = InvoiceStatus.Issued };
        var account = new Account { Id = accountId, Balance = 9999 };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, default)).ReturnsAsync(invoice);
        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, default)).ReturnsAsync(account);

        var dto = new CreatePaymentDto
        {
            InvoiceId = invoiceId,
            AccountId = accountId,
            AmountPaid = 500,  // จ่ายครบ
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = "Cash"
        };

        // Act
        await _service.CreatePaymentAsync(dto);

        // Assert
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    // ─── Account ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAccountAsync_WithValidData_ReturnsNewId()
    {
        // Arrange
        var dto = new CreateAccountDto
        {
            AccountName = "Main Account",
            AccountCode = "ACC-001",
            Balance = 10000
        };

        // Act
        var result = await _service.CreateAccountAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _accountRepoMock.Verify(r => r.AddAsync(It.IsAny<Account>(), default), Times.Once);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ReturnsAllAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), AccountName = "Cash", AccountCode = "ACC-001", Balance = 5000 },
            new() { Id = Guid.NewGuid(), AccountName = "Bank", AccountCode = "ACC-002", Balance = 20000 },
        };
        _accountRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(accounts);

        // Act
        var result = await _service.GetAllAccountsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}
