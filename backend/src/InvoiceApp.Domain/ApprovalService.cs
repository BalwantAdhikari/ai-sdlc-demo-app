namespace InvoiceApp.Domain;

/// <summary>
/// Moves an invoice through its lifecycle: Draft -> PendingApproval -> Approved or Rejected.
/// One invoice at a time, decided as a whole by a single approver.
/// </summary>
public class ApprovalService
{
    private const decimal ManagerApprovalLimit = 50_000m;
    private const decimal DirectorApprovalLimit = 500_000m;

    private readonly IInvoiceRepository _repository;

    public ApprovalService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public Invoice Submit(string invoiceId)
    {
        var invoice = Find(invoiceId);

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Invoice {invoiceId} is {invoice.Status}; only drafts can be submitted.");
        }

        invoice.Status = InvoiceStatus.PendingApproval;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Approve(string invoiceId, string approver)
    {
        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        if (invoice.Amount > DirectorApprovalLimit)
        {
            var isFirstLargeApproval = string.IsNullOrWhiteSpace(invoice.FirstApprover);

            ValidateLargeInvoiceApproval(invoice, approver);
            _repository.Update(invoice);

            if (isFirstLargeApproval)
            {
                return CloneInvoice(invoice);
            }

            return invoice;
        }

        ValidateSingleApproverLimit(invoice, approver);

        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Reject(string invoiceId, string approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A reason is required to reject an invoice.", nameof(reason));
        }

        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        invoice.Status = InvoiceStatus.Rejected;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.DecisionNote = reason;
        _repository.Update(invoice);
        return invoice;
    }

    private Invoice Find(string invoiceId) =>
        _repository.GetById(invoiceId)
        ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");

    private static void EnsurePending(Invoice invoice)
    {
        if (invoice.Status != InvoiceStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                $"Invoice {invoice.Id} is {invoice.Status}; only pending invoices can be decided.");
        }
    }

    private static void ValidateSingleApproverLimit(Invoice invoice, string approver)
    {
        if (string.IsNullOrWhiteSpace(approver))
        {
            throw new InvalidOperationException("Approver is required to approve an invoice.");
        }

        var normalizedApprover = approver.Trim();

        if (invoice.Amount <= ManagerApprovalLimit && normalizedApprover.Equals("manager", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (invoice.Amount > ManagerApprovalLimit && invoice.Amount <= DirectorApprovalLimit && normalizedApprover.Equals("director", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new InvalidOperationException(
            $"Approver '{approver}' cannot approve invoice '{invoice.Id}' for amount {invoice.Amount:C} based on the current role limits.");
    }

    private static void ValidateLargeInvoiceApproval(Invoice invoice, string approver)
    {
        if (string.IsNullOrWhiteSpace(approver))
        {
            throw new InvalidOperationException("Approver is required to approve an invoice.");
        }

        var normalizedApprover = approver.Trim();
        var firstApprover = invoice.FirstApprover;

        if (string.IsNullOrWhiteSpace(firstApprover))
        {
            if (!normalizedApprover.Equals("manager", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Invoice '{invoice.Id}' requires a manager as the first approver before a director can approve it.");
            }

            invoice.FirstApprover = normalizedApprover;
            invoice.DecidedBy = normalizedApprover;
            invoice.DecidedAt = DateTime.UtcNow;
            invoice.Status = InvoiceStatus.PendingApproval;
            return;
        }

        if (!normalizedApprover.Equals("director", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(firstApprover, normalizedApprover, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Invoice '{invoice.Id}' requires a director as the second approver for amounts above {DirectorApprovalLimit:C}.");
        }

        if (!string.Equals(firstApprover, "manager", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Invoice '{invoice.Id}' requires a manager as the first approver before a director can approve it.");
        }

        invoice.SecondApprover = normalizedApprover;
        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = normalizedApprover;
        invoice.DecidedAt = DateTime.UtcNow;
    }

    private static Invoice CloneInvoice(Invoice invoice) => new()
    {
        Id = invoice.Id,
        CustomerName = invoice.CustomerName,
        Amount = invoice.Amount,
        Status = invoice.Status,
        CreatedAt = invoice.CreatedAt,
        DecidedBy = invoice.DecidedBy,
        FirstApprover = invoice.FirstApprover,
        SecondApprover = invoice.SecondApprover,
        DecidedAt = invoice.DecidedAt,
        DecisionNote = invoice.DecisionNote
    };
}
