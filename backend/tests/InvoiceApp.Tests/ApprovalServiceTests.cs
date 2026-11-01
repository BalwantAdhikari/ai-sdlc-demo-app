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
    public void Approve_LargeInvoiceRequiresManagerThenDirector()
    {
        var service = ServiceFor(InvoiceWith("INV-5", InvoiceStatus.PendingApproval, 600000m));

        var firstDecision = service.Approve("INV-5", "manager");
        var secondDecision = service.Approve("INV-5", "director");

        Assert.Equal(InvoiceStatus.Approved, secondDecision.Status);
        Assert.Equal("director", secondDecision.DecidedBy);
        Assert.Equal(InvoiceStatus.PendingApproval, firstDecision.Status);
        Assert.Equal("manager", firstDecision.DecidedBy);
        Assert.Equal("manager", firstDecision.FirstApprover);
        Assert.Equal("manager", secondDecision.FirstApprover);
        Assert.Equal("director", secondDecision.SecondApprover);
        Assert.NotNull(firstDecision.DecidedAt);
    }

    [Fact]
    public void Approve_LargeInvoiceRejectsDuplicateApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-6", InvoiceStatus.PendingApproval, 600000m));

        service.Approve("INV-6", "manager");

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-6", "manager"));
        Assert.Contains("requires a director as the second approver", exception.Message);
    }

    [Fact]
    public void Approve_LargeInvoiceRejectsInvalidFirstApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-7", InvoiceStatus.PendingApproval, 600000m));

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-7", "director"));
        Assert.Contains("requires a manager as the first approver", exception.Message);
    }

    [Fact]
    public void Approve_LargeInvoiceRejectsNonDirectorSecondApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-8", InvoiceStatus.PendingApproval, 600000m));

        service.Approve("INV-8", "manager");

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-8", "employee"));
        Assert.Contains("requires a director as the second approver", exception.Message);
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

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-1", "demo.approver"));
        Assert.Contains("only pending invoices can be decided", exception.Message);
    }

    [Fact]
    public void Approve_RejectsBlankApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-blank", InvoiceStatus.PendingApproval, 25000m));

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-blank", "   "));
        Assert.Contains("Approver is required", exception.Message);
    }

    [Fact]
    public void Approve_RejectsNullApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-null-approver", InvoiceStatus.PendingApproval, 25000m));

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-null-approver", null!));
        Assert.Contains("Approver is required", exception.Message);
    }

    [Fact]
    public void Approve_LargeInvoiceRejectsBlankApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-large-blank", InvoiceStatus.PendingApproval, 600000m));

        var exception = Assert.Throws<InvalidOperationException>(() => service.Approve("INV-large-blank", "   "));
        Assert.Contains("Approver is required", exception.Message);
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
        Assert.Equal("demo.approver", result.DecidedBy);
        Assert.NotNull(result.DecidedAt);
    }

    [Fact]
    public void Reject_RequiresAReason()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        var exception = Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.approver", "  "));
        Assert.Equal("reason", exception.ParamName);
        Assert.Contains("A reason is required", exception.Message);
    }
}
