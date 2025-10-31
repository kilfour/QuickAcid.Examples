using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.Tests;

public class CreateCoachUnitTests
{
    private static Coach CreateValidCoach()
        => Coach.Create(TheCanonical.AdminActor(), TheCanonical.CoachName, TheCanonical.CoachEmail);

    [Fact]
    public void RegisterCoach_WithValidData_ShouldSucceed()
    {
        var coach = CreateValidCoach();
        Assert.Equal(TheCanonical.CoachName, coach.Name.Value);
        Assert.Equal(TheCanonical.CoachEmail, coach.Email.Value);
        Assert.Empty(coach.Skills);
    }

    [Fact]
    public void RegisterCoach_WithValidData_does_not_assign_id()
        => Assert.Equal(default, CreateValidCoach().Id.Value);

    [Fact]
    public void RegisterCoach_WithEmptyName_ShouldThrow()
        => Assert.Throws<CoachNameCanNotBeEmpty>(
            () => Coach.Create(TheCanonical.AdminActor(), string.Empty, TheCanonical.CoachEmail));

    [Fact]
    public void RegisterCoach_WithLongName_ShouldThrow()
        => Assert.Throws<CoachNameCanNotBeTooLong>(
            () => Coach.Create(TheCanonical.AdminActor(), new string('-', 101), TheCanonical.CoachEmail));

    [Fact]
    public void RegisterCoach_WithInvalidActor_ShouldThrow()
        => Assert.Throws<CoachNameCanNotBeTooLong>(
            () => Coach.Create(TheCanonical.AdminActor(), new string('-', 101), TheCanonical.CoachEmail));

    [Fact]
    public void RegisterCoach_WithEmptyEmail_ShouldThrow()
        => Assert.Throws<CoachEmailCanNotBeEmpty>(
            () => Coach.Create(TheCanonical.AdminActor(), TheCanonical.CoachName, string.Empty));

    [Fact]
    public void RegisterCoach_WithLongEmail_ShouldThrow()
        => Assert.Throws<CoachEmailCanNotBeTooLong>(
            () => Coach.Create(TheCanonical.AdminActor(), TheCanonical.CoachName, new string('-', 101)));


    [Fact]
    public void RegisterCoach_With_unauthenticated_ShouldThrow()
        => Assert.Throws<UnauthorizedAccessException>(
            () => Coach.Create(TheCanonical.EmptyActor, TheCanonical.CoachName, new string('-', 101)));

    [Fact]
    public void RegisterCoach_With_non_admin_ShouldThrow()
        => Assert.Throws<UnauthorizedAccessException>(
            () => Coach.Create(TheCanonical.CoachActor(), TheCanonical.CoachName, new string('-', 101)));
}
