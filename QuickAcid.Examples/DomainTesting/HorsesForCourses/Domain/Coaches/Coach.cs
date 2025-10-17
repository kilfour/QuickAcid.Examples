using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Courses;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Skills;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.ValidationHelpers;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;

public class Coach : DomainEntity<Coach>
{
    public CoachName Name { get; init; } = CoachName.Empty;
    public CoachEmail Email { get; init; } = CoachEmail.Empty;

    public IReadOnlyCollection<Skill> Skills => skills.ToList().AsReadOnly();
    private readonly HashSet<Skill> skills = [];

    public IReadOnlyCollection<Course> AssignedCourses => assignedCourses.AsReadOnly();
    private readonly List<Course> assignedCourses = [];

    private Coach() { /*** EFC Was Here ****/ }
    protected Coach(string name, string email)
    {
        Name = new CoachName(name);
        Email = new CoachEmail(email);
    }

    public static Coach Create(Actor actor, string name, string email)
    {
        OnlyActorsWithAdminRoleCanCreateCoach();
        return new(name, email);
        void OnlyActorsWithAdminRoleCanCreateCoach()
            => actor.CanCreateCoach();
    }

    public virtual void UpdateSkills(Actor actor, IEnumerable<string> newSkills)
    {
        OnlyAdminsAndActorsWhoRegisteredAsCoachCanEdit();
        NotAllowedWhenThereAreDuplicateSkills();
        OverwriteSkills();
        // ------------------------------------------------------------------------------------------------
        // --
        void OnlyAdminsAndActorsWhoRegisteredAsCoachCanEdit()
            => actor.CanEditCoach(Email.Value);
        bool NotAllowedWhenThereAreDuplicateSkills()
            => newSkills.NoDuplicatesAllowed(a => new CoachAlreadyHasSkill(string.Join(",", a)));
        void OverwriteSkills()
        {
            skills.Clear();
            newSkills.Select(Skill.From)
                .ToList()
                .ForEach(a => skills.Add(a));
        }
        // ------------------------------------------------------------------------------------------------
    }

    public bool IsSuitableFor(Course course)
        => course.RequiredSkills.All(Skills.Contains);

    public bool IsAvailableFor(Course course)
        => CheckIf.ImAvailable(this).For(course);

    public void AssignCourse(Course course)
        => assignedCourses.Add(course);
}
