namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;

public interface IDomainEntity { }

public abstract class DomainEntity<T> : IDomainEntity
{
    public Id<T> Id { get; } = Id<T>.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is not DomainEntity<T> other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
