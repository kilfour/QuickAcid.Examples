using QuickAcid.Examples.DomainTesting.HorsesForCourses.Abstractions;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts.InvalidationReasons;

namespace QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts;

public record ApplicationUserName : DefaultString<ApplicationUserNameCanNotBeEmpty, ApplicationUserNameCanNotBeTooLong>
{
    public ApplicationUserName(string value) : base(value) { }
    protected ApplicationUserName() { }
    public static ApplicationUserName Empty => new();
}
