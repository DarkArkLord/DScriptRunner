using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class RunnerConfig
    {
        public string HelloMessage { get; set; } = string.Empty;
        public Dictionary<string, (IReadOnlyList<string> BeforeLines, IReadOnlyList<string> AfterLines)> Environments { get; set; } = new Dictionary<string, (IReadOnlyList<string> BeforeLines, IReadOnlyList<string> AfterLines)>();
        public IReadOnlyList<ScriptInfo> Content { get; set; } = Array.Empty<ScriptInfo>();
    }
}
