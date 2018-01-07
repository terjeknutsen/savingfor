using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SavingFor.Services;
using SpecsFor;

namespace SavingFor.AndroidTests.TotalUpdaterSpecs.WhenUpdatingTotal
{
    public class Given_progress_per_second_is_zero : SpecsFor<TotalUpdater>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new TotalUpdater((a) => {});
        }

        [Test]
        public void Then_exception_should_not_be_thrown()
        {
            Assert.DoesNotThrow((() => SUT.Start()));
        }
    }
}
