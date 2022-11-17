using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class RunnerConfig
    {
        public string HelloMessage { get; set; } = string.Empty;
        public Dictionary<string, IReadOnlyList<string>> ScriptElements { get; set; } = new Dictionary<string, IReadOnlyList<string>>();
        public IReadOnlyList<ScriptInfo> Content { get; set; } = Array.Empty<ScriptInfo>();
    }
}
