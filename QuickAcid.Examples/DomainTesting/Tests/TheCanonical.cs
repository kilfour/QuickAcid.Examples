using System.Security.Claims;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses.TimeSlots;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Skills;
using WibblyWobbly;

namespace QuickAcid.Examples.DomainTesting.Tests;

public static class TheCanonical
{
    public static Actor AdminActor()
        => new Actor().Declare(ClaimTypes.Name, CoachName)
            .Declare(ClaimTypes.Role, HorsesForCourses.Domain.Accounts.ApplicationUser.AdminRole);
    public static Actor CoachActor()
        => new Actor().Declare(ClaimTypes.Name, CoachName)
            .Declare(ClaimTypes.Role, HorsesForCourses.Domain.Accounts.ApplicationUser.CoachRole);
    public static Actor AuthenticatedActor()
        => new Actor().Declare(ClaimTypes.Name, CoachName);
    public static Actor EmptyActor => new();

    public static readonly int BadId = -1;

    public static readonly int CoachId = 99;
    public const string CoachName = "The Coach";
    public const string CoachEmail = "coach@coaching.mcCoach";
    public static Coach Coach()
        => HorsesForCourses.Domain.Coaches.Coach.Create(AdminActor(), CoachName, CoachEmail);

    // public static PagedResult<CoachSummary> CoachSummaryList()
    //     => new([new CoachSummary(CoachId, CoachName, CoachEmail, 0)], 1, 1, 25);

    // public static CoachDetail CoachDetail()
    //     => new() { Id = CoachId, Name = CoachName, Email = CoachEmail };

    public readonly static List<string> Skills = ["one", "two"];
    public readonly static HashSet<Skill> HardSkills = [Skill.From("one"), Skill.From("two")];
    public readonly static List<Skill> HardSkillsList = [Skill.From("one"), Skill.From("two")];

    public static readonly int CourseId = 999;
    public const string CourseName = "The Course";
    public static readonly DateOnly CourseStart = 1.January(2025);
    public static readonly DateOnly CourseEnd = 31.January(2025);

    public static Course Course()
        => HorsesForCourses.Domain.Courses.Course.Create(AdminActor(), CourseName, CourseStart, CourseEnd);

    // public static IEnumerable<TimeSlotRequest> TimeSlotsRequestFullDayMonday()
    //     => [new(CourseDay.Monday, 9, 17)];
    public static IEnumerable<(CourseDay, int, int)> TimeSlotsFullDayMonday()
        => [(CourseDay.Monday, 9, 17)];
    // public static IEnumerable<TimeSlotViewModel> TimeSlotsFullDayMondayViewModel()
    //     => [new TimeSlotViewModel(CourseDay.Monday, 9, 17)];
    // public static IEnumerable<TimeSlot> TimeSlotsFullDayMondayExpected()
    //     => [TimeSlot.From(CourseDay.Monday, 9, 17)];
    // public static IEnumerable<TimeSlotInfo> TimeSlotsFullDayMondayInfo()
    //     => [new TimeSlotInfo(CourseDay.Monday, 9, 17)];

    // public static IEnumerable<TimeSlotViewModel> TimeSlotsWithDuplicateViewModel()
    //     => [new TimeSlotViewModel(CourseDay.Monday, 9, 17), new TimeSlotViewModel(CourseDay.Monday, 9, 17)];

    // public static PagedResult<CourseSummary> CourseSummaryList()
    //     => new([new CourseSummary(CourseId, CourseName, CourseStart, CourseEnd, false, false)], 1, 1, 25);

    // public static CourseDetail CourseDetail()
    //     => new() { Id = CourseId, Name = CourseName, Start = CourseStart, End = CourseEnd };

    // public static ApplicationUser ApplicationUser()
    //    => HorsesForCourses.Domain.Accounts.ApplicationUser.Create(
    //         CoachName, CoachEmail, Password, Password, "");
    // public static ApplicationUser ApplicationUser(string role)
    //    => HorsesForCourses.Domain.Accounts.ApplicationUser.Create(
    //         CoachName, CoachEmail, Password, Password, role);

    // public const string Password = "pass123";
}