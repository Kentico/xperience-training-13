using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;

namespace Business
{
    public class DevelopmentOptionsService<TOptions> : IOptionsService<TOptions>
        where TOptions : class, new()
    {
        protected IOptionsSnapshot<TOptions> OptionsSnapshot { get; }

        public DevelopmentOptionsService(IOptionsSnapshot<TOptions> optionsSnapshot)
        {
            OptionsSnapshot = optionsSnapshot;
        }

        public TOptions Options => OptionsSnapshot.Value;
    }

    public class ProductionOptionsService<TOptions> : IOptionsService<TOptions>
        where TOptions : class, new()
    {
        protected IOptionsMonitor<TOptions> OptionsMonitor { get; }

        public ProductionOptionsService(IOptionsMonitor<TOptions> optionsMonitor)
        {
            OptionsMonitor = optionsMonitor;
        }

        public TOptions Options => OptionsMonitor.CurrentValue;
    }
}
