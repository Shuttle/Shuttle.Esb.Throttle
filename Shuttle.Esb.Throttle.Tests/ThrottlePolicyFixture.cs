using System;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Esb.Throttle.Tests
{
    [TestFixture]
    public class ThrottlePolicyFixture
    {
        [Test]
        public void Should_be_able_to_determine_whether_to_abort()
        {
            var policy = new ThrottlePolicy(Options.Create(new ThrottleOptions()));

            Assert.IsFalse(policy.ShouldAbort());

            var aborted = false;
            var timeout = DateTime.Now.AddSeconds(2);

            policy = new ThrottlePolicy(Options.Create(new ThrottleOptions { CpuUsagePercentage = 1 }));

            while (!aborted && DateTime.Now < timeout)
            {
                aborted = policy.ShouldAbort();
            }

            Assert.IsTrue(aborted);
        }
    }
}