using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts;

public record ApplicationUserEmail : DefaultString<ApplicationUserEmailCanNotBeEmpty, ApplicationUserEmailCanNotBeTooLong>
{
    public ApplicationUserEmail(string value) : base(value) { }
    protected ApplicationUserEmail() { }
    public static ApplicationUserEmail Empty => new();
}
