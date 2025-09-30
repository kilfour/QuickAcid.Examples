namespace QuickAcid.Examples.EFC.Invoicing.Domain;

public class InvoiceLine
{
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Empty;

    private InvoiceLine() { }

    public InvoiceLine(string description, int quantity, Money unitPrice)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Money Total() => UnitPrice * Quantity;
}
