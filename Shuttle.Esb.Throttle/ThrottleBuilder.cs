using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Throttle
{
    public class ThrottleBuilder
    {
        private ThrottleOptions _throttleOptions = new ThrottleOptions();
        public IServiceCollection Services { get; }

        public ThrottleBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public ThrottleOptions Options
        {
            get => _throttleOptions;
            set => _throttleOptions = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}