using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Shuttle.Esb.Throttle.Tests
{
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

			Assert.IsNotNull(options);
			Assert.AreEqual(35, options.CpuUsagePercentage);
			Assert.AreEqual(7, options.AbortCycleCount);
			Assert.AreEqual(TimeSpan.FromSeconds(2), options.DurationToSleepOnAbort[0]);
			Assert.AreEqual(TimeSpan.FromSeconds(2), options.DurationToSleepOnAbort[1]);
			Assert.AreEqual(TimeSpan.FromSeconds(3), options.DurationToSleepOnAbort[2]);
		}
	}
}