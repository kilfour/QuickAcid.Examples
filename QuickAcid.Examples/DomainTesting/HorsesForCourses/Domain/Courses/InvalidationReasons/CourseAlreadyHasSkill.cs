using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses.InvalidationReasons;

public class CourseAlreadyHasSkill(string skill) : DomainException(skill) { }
