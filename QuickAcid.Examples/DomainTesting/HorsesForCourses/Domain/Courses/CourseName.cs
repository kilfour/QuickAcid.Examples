using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses;

public record CourseName : DefaultString<CourseNameCanNotBeEmpty, CourseNameCanNotBeTooLong>
{
    public CourseName(string value) : base(value) { }
    protected CourseName() { }
    public static CourseName Empty => new();
}
