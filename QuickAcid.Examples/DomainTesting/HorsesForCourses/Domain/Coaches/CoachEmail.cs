using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;

public record CoachEmail : DefaultString<CoachEmailCanNotBeEmpty, CoachEmailCanNotBeTooLong>
{
    public CoachEmail(string value) : base(value) { }
    protected CoachEmail() { }
    public static CoachEmail Empty => new();
}
