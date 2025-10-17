using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain;

public class UnavailableFor(Id<Coach> CoachId, Id<Course> CourseId) : DomainEntity<UnavailableFor>
{
    public Id<Coach> CoachId { get; } = CoachId;
    public Id<Course> CourseId { get; } = CourseId;
}
