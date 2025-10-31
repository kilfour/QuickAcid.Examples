using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Accounts;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches;
using QuickAcid.Examples.DomainTesting.HorsesForCourses.Domain.Coaches.InvalidationReasons;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Examples.DomainTesting.Tests;

public class CreateCoachAcidTests
{
    public record CreateCoach : Act
    {
        public QAcidScript<Acid> Test =
            from name in Script.Input<Name>().With(Fuzzr.String(1, 100))
            from email in Script.Input<Email>().With(Fuzzr.String(1, 100))
            from coach in Script.Act<CreateCoach>().With(
                () => Coach.Create(TheCanonical.AdminActor(), name, email))
            from nameIsSet in Script.Spec<NameIsSet>(
                () => coach?.Name?.Value == name)
            from emailIsSet in Script.Spec<EmailIsSet>(
                () => coach?.Email?.Value == email)
            from _BR1 in CreateCoachThrows<WithCoachActor, UnauthorizedAccessException>(
                TheCanonical.CoachActor(), name, email)
            from _BR2 in CreateCoachThrows<WithEmptyActor, UnauthorizedAccessException>(
                TheCanonical.EmptyActor, name, email)
            from _BR3 in CreateCoachThrows<NameIsEmpty, CoachNameCanNotBeEmpty>(
                TheCanonical.AdminActor(), "", email)
            from _BR4 in CreateCoachThrows<NameIsTooLong, CoachNameCanNotBeTooLong>(
                TheCanonical.AdminActor(), new string('x', 101), email)
            from _BR5 in CreateCoachThrows<EmailIsEmpty, CoachEmailCanNotBeEmpty>(
                TheCanonical.AdminActor(), name, "")
            from _BR6 in CreateCoachThrows<EmailIsTooLong, CoachEmailCanNotBeTooLong>(
                TheCanonical.AdminActor(), name, new string('x', 101))
            select Acid.Test;

        private static QAcidScript<Acid> CreateCoachThrows<TLabel, TEx>(
            Actor actor, string name, string email)
                where TLabel : Act
                where TEx : Exception
                =>
                    from res in Script.ActCarefully<TLabel>(() => Coach.Create(actor, name, email))
                    from s in $"Throws {typeof(TEx).Name}".Spec(res.ThrewExactly<TEx>)
                    select Acid.Test;

        public record Name : Input;
        public record Email : Input;
        public record NameIsSet : Spec;
        public record EmailIsSet : Spec;
        public record WithCoachActor : Act;
        public record WithEmptyActor : Act;
        public record NameIsEmpty : Act;
        public record NameIsTooLong : Act;
        public record EmailIsEmpty : Act;
        public record EmailIsTooLong : Act;
    }

    [Fact]
    public void Test()
    {
        var script =
            from name in Script.Input<CreateCoach.Name>().With(Fuzzr.String(1, 100))
            from email in Script.Input<CreateCoach.Email>().With(Fuzzr.String(1, 100))
            from coach in Script.Act<CreateCoach>().With(
                () => Coach.Create(TheCanonical.AdminActor(), name, email))
            from nameIsSet in Script.Spec<CreateCoach.NameIsSet>(
                () => coach?.Name?.Value == name)
            from emailIsSet in Script.Spec<CreateCoach.EmailIsSet>(
                () => coach?.Email?.Value == email)
            from _BR1 in CreateCoachThrows<CreateCoach.WithCoachActor, UnauthorizedAccessException>(
                TheCanonical.CoachActor(), name, email)
            from _BR2 in CreateCoachThrows<CreateCoach.WithEmptyActor, UnauthorizedAccessException>(
                TheCanonical.EmptyActor, name, email)
            from _BR3 in CreateCoachThrows<CreateCoach.NameIsEmpty, CoachNameCanNotBeEmpty>(
                TheCanonical.AdminActor(), "", email)
            from _BR4 in CreateCoachThrows<CreateCoach.NameIsTooLong, CoachNameCanNotBeTooLong>(
                TheCanonical.AdminActor(), new string('x', 101), email)
            from _BR5 in CreateCoachThrows<CreateCoach.EmailIsEmpty, CoachEmailCanNotBeEmpty>(
                TheCanonical.AdminActor(), name, "")
            from _BR6 in CreateCoachThrows<CreateCoach.EmailIsTooLong, UnauthorizedAccessException>( //CoachEmailCanNotBeTooLong
                TheCanonical.AdminActor(), name, new string('x', 101))
            select Acid.Test;
        QState.Run(script)
            .With(100.Runs())
            .AndOneExecutionPerRun();
    }

    private static QAcidScript<Acid> CreateCoachThrows<TLabel, TEx>(
        Actor actor, string name, string email)
            where TLabel : Act
            where TEx : Exception
            =>
                from res in Script.ActCarefully<TLabel>(() => Coach.Create(actor, name, email))
                from s in $"Throws {typeof(TEx).Name}".Spec(res.ThrewExactly<TEx>)
                select Acid.Test;
}
