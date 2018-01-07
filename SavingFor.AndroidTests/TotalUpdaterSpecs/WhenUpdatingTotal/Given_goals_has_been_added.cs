using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SavingFor.Domain.Model;
using SavingFor.Services;
using Should;
using SpecsFor;

namespace SavingFor.AndroidTests.WhenUpdatingTotal
{
    public class Given_goals_has_been_added : SpecsFor<TotalUpdater>
    {
        private decimal requiredAmount;

        protected override void InitializeClassUnderTest()
        {
            SUT = new TotalUpdater(OnUpdate);
        }

        protected override void Given()
        {
            Given<GoalsHasBeenAdded>();
            base.Given();
        }

        protected override void When()
        {
            SUT.Start();
        }

        [Test]
        public async void Then_required_total_should_be_updated()
        {
            await Task.Delay(149);
            SUT.Stop();
            requiredAmount.ShouldBeInRange(5760,5761);
        }

        private void OnUpdate(decimal number)
        {
            requiredAmount = number;
        }
    }

    class GoalsHasBeenAdded : IContext<TotalUpdater>
    {
        public void Initialize(ISpecs<TotalUpdater> state)
        {
            state.SUT.Set(new List<GoalAmountRequired>
            {
                new GoalAmountRequired(4,DateTime.Now.AddHours(-24))
            });
        }
    }
}
