using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SavingFor.Domain.Model;
using SavingFor.Services;
using Should;
using SpecsFor;

namespace SavingFor.AndroidTests.WhenUpdatingTotal
{
    public class Given_update_has_started : SpecsFor<TotalUpdater>
    {
        private int counter;
        private int secondUpdate;

        protected override void InitializeClassUnderTest()
        {
            SUT = new TotalUpdater(OnUpdate);
        }

        protected override void Given()
        {
            Given<GoalAmountRequiredAdded>();
            Given<UpdateHasStarted>();
            base.Given();
        }

        private void OnUpdate(decimal @decimal)
        {
            if (counter++ == 0)
                return;
            secondUpdate++;
            
        }

        [Test]
        public async void Then_total_amount_should_be_updated_continually()
        {
            await Task.Delay(80);
            SUT.Stop();
            secondUpdate.ShouldBeGreaterThan(0);
        }
    }

    class GoalAmountRequiredAdded : IContext<TotalUpdater>
    {
        public void Initialize(ISpecs<TotalUpdater> state)
        {
            state.SUT.Set(new [] {new GoalAmountRequired(10,DateTime.Now.AddHours(-24))});
        }
    }
    class UpdateHasStarted : IContext<TotalUpdater>
    {
        public void Initialize(ISpecs<TotalUpdater> state)
        {
            state.SUT.Start();
        }
    }
}
