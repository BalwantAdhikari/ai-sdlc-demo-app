using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class ApprovalServiceTests
{
    private static Invoice InvoiceWith(string id, InvoiceStatus status, decimal amount = 100m) =>
        new() { Id = id, CustomerName = "Test School", Amount = amount, Status = status };

    private static ApprovalService ServiceFor(params Invoice[] invoices) =>
        new(new FakeInvoiceRepository(invoices));

    [Fact]
    public void Approve_ManagerCanApproveUpTo50kInclusive()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 50000m));

        var result = service.Approve("INV-1", "manager");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("manager", result.DecidedBy);
    }

    [Fact]
    public void Approve_DirectorCanApproveUpTo500kInclusive()
    {
        var service = ServiceFor(InvoiceWith("INV-2", InvoiceStatus.PendingApproval, 500000m));

        var result = service.Approve("INV-2", "director");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("director", result.DecidedBy);
    }

    [Fact]
    public void Approve_ManagerRejectsAmountOver50k()
    {
        var service = ServiceFor(InvoiceWith("INV-3", InvoiceStatus.PendingApproval, 50001m));

        Assert.Throws<InvalidOperationException>(() => service.Approve("INV-3", "manager"));
    }

    [Fact]
    public void Approve_DirectorRejectsAmountOver500k()
    {
        var service = ServiceFor(InvoiceWith("INV-4", InvoiceStatus.PendingApproval, 500001m));

        Assert.Throws<InvalidOperationException>(() => service.Approve("INV-4", "director"));
    }

    [Fact]
    public void Submit_MovesDraftToPendingApproval()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.Draft);

        var result = ServiceFor(invoice).Submit("INV-1");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
    }

    [Fact]
    public void Submit_RejectsInvoiceThatIsNotADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Approved));

        Assert.Throws<InvalidOperationException>(() => service.Submit("INV-1"));
    }

    [Fact]
    public void Approve_SetsStatusApproverAndTimestamp()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 25000m));

        var result = service.Approve("INV-1", "manager");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("manager", result.DecidedBy);
        Assert.NotNull(result.DecidedAt);
    }

    [Fact]
    public void Approve_RejectsInvoiceThatIsStillADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Draft));

        Assert.Throws<InvalidOperationException>(() => service.Approve("INV-1", "demo.approver"));
    }

    [Fact]
    public void Approve_ThrowsWhenInvoiceDoesNotExist()
    {
        var service = ServiceFor();

        Assert.Throws<KeyNotFoundException>(() => service.Approve("INV-404", "demo.approver"));
    }

    [Fact]
    public void Reject_RecordsReasonAndApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        var result = service.Reject("INV-1", "demo.approver", "Duplicate invoice");

        Assert.Equal(InvoiceStatus.Rejected, result.Status);
        Assert.Equal("Duplicate invoice", result.DecisionNote);
    }

    [Fact]
    public void Reject_RequiresAReason()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.approver", "  "));
    }
}
