using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;

public record CoachName : DefaultString<CoachNameCanNotBeEmpty, CoachNameCanNotBeTooLong>
{
    public CoachName(string value) : base(value) { }
    protected CoachName() { }
    public static CoachName Empty => new();
}
