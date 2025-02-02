using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Shuttle.Esb.Throttle.Tests;

[TestFixture]
public class ThrottleSectionFixture
{
    protected ThrottleOptions GetOptions()
    {
        var result = new ThrottleOptions();

        result.DurationToSleepOnAbort.Clear();

        new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\appsettings.json")).Build()
            .GetRequiredSection($"{ThrottleOptions.SectionName}").Bind(result);

        return result;
    }

    [Test]
    public void Should_be_able_to_load_the_configuration()
    {
        var options = GetOptions();

        Assert.That(options, Is.Not.Null);
        Assert.That(options.CpuUsagePercentage, Is.EqualTo(35));
        Assert.That(options.AbortCycleCount, Is.EqualTo(7));
        Assert.That(options.DurationToSleepOnAbort[0], Is.EqualTo(TimeSpan.FromSeconds(2)));
        Assert.That(options.DurationToSleepOnAbort[1], Is.EqualTo(TimeSpan.FromSeconds(2)));
        Assert.That(options.DurationToSleepOnAbort[2], Is.EqualTo(TimeSpan.FromSeconds(3)));
    }
}