namespace QuickAcid.Examples.EFC.Invoicing.Domain;

public record Money(decimal Value, string Currency)
{
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch.");
        return new Money(a.Value + b.Value, a.Currency);
    }

    public static Money operator *(Money m, int factor) =>
        new Money(m.Value * factor, m.Currency);

    public static Money Empty => new(0, string.Empty);
}
