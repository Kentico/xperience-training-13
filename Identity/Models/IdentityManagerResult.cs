using System;
using System.Collections.Generic;

namespace Identity.Models
{
    public class IdentityManagerResult<TResultState>
        where TResultState : Enum
    {
        public bool Success { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public TResultState ResultState { get; set; } = default!;
    }

    public class IdentityManagerResult<TResultState, TData> : IdentityManagerResult<TResultState>
        where TResultState : Enum
    {
        public TData Data { get; set; } = default!;
    }
}