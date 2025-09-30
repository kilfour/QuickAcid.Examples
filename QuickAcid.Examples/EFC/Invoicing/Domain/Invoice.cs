namespace QuickAcid.Examples.EFC.Invoicing.Domain;

public class Invoice
{
    public Guid Id { get; private set; }
    public Customer? Customer { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }

    private readonly List<InvoiceLine> lines = new();
    public IReadOnlyCollection<InvoiceLine> Lines => lines.AsReadOnly();

    private Invoice() { }

    public Invoice(Customer customer, DateTime issueDate, DateTime dueDate)
    {
        Id = Guid.NewGuid();
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        IssueDate = issueDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Draft;
    }

    public void AddLine(string description, int quantity, Money unitPrice)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify invoice after it is sent.");

        lines.Add(new InvoiceLine(description, quantity, unitPrice));
    }

    public Money Total() =>
        lines.Aggregate(new Money(0, "EUR"), (sum, line) => sum + line.Total());

    public void MarkAsSent() => Status = InvoiceStatus.Sent;
    public void MarkAsPaid() => Status = InvoiceStatus.Paid;
    public void MarkAsOverdue() => Status = InvoiceStatus.Overdue;
}
