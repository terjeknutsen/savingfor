using System;
using NUnit.Framework;
using SavingFor.Domain.Model;
using SavingFor.Services;
using Should;
using SpecsFor;

namespace SavingFor.AndroidTests.TotalUpdaterSpecs.WhenSettingRequiredAmount
{
    public class Given_required_amounts_already_set : SpecsFor<TotalUpdater>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new TotalUpdater((a) => {});
        }

        protected override void Given()
        {
            SUT.Set(new [] {new GoalAmountRequired(0.01m, DateTime.Now.AddDays(-10)), });
            var invocation = SUT.Delay;
            base.Given();
        }

        protected override void When()
        {
            SUT.Set(new [] {new GoalAmountRequired(incomePerMinute: 10, startDateTime: DateTime.Now.AddDays(-10)) });
        }

        [Test]
        public void Then_delay_should_be_calculated_on_basis_of_last_goal_amounts()
        {
            SUT.Delay.ShouldBeLessThan(100);
        }
    }
}
