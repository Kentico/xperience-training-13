using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    public interface IOptionsService<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; }
    }
}
