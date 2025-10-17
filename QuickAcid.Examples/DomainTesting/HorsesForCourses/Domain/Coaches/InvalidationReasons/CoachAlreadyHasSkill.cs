using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;

public class CoachAlreadyHasSkill(string skill) : DomainException(skill) { }