using System;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Esb.Throttle.Tests;

[TestFixture]
public class ThrottlePolicyFixture
{
    [Test]
    public void Should_be_able_to_determine_whether_to_abort()
    {
        var policy = new ThrottlePolicy(Options.Create(new ThrottleOptions()));

        Assert.That(policy.ShouldAbort(), Is.False);

        var aborted = false;
        var timeout = DateTimeOffset.Now.AddSeconds(2);

        policy = new(Options.Create(new ThrottleOptions { CpuUsagePercentage = 1 }));

        while (!aborted && DateTimeOffset.Now < timeout)
        {
            aborted = policy.ShouldAbort();
        }

        Assert.That(aborted, Is.True);
    }
}