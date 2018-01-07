using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SavingFor.Domain.Model;
using SavingFor.Services;
using Should;
using SpecsFor;

namespace SavingFor.AndroidTests.WhenUpdatingTotal
{
    class Given_update_rate_is_low : SpecsFor<TotalUpdater>
    {
        private int counter;

        protected override void InitializeClassUnderTest()
        {
            SUT = new TotalUpdater(Onupdate);
        }

        protected override void Given()
        {
            Given<GoalAmountAdded>();
            base.Given();
        }

        protected override void When()
        {
            SUT.Start();
        }

        [Test]
        public async void Then_update_should_only_be_fired_once_in_given_timespan()
        {
            await Task.Delay(50);
            SUT.Stop();
            counter.ShouldBeInRange(1,1);
        }

        private void Onupdate(decimal @decimal)
        {
            counter++;
        }
    }

    class GoalAmountAdded : IContext<TotalUpdater>
    {
        public void Initialize(ISpecs<TotalUpdater> state)
        {
            state.SUT.Set(new [] {new GoalAmountRequired(0.001m,DateTime.Now.AddHours(-24))});
        }
    }
}
