namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;

public record Id<T>
{
    private readonly int value;
    public int Value => value;

    public static Id<T> Empty { get; } = new Id<T>();

    private Id() { }

    private Id(int value) => this.value = value;

    public static Id<T> From(int value) => new(value);
}

