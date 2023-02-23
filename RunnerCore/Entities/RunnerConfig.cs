using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class RunnerConfig
    {
        public string HelloMessage { get; set; } = string.Empty;
        public Dictionary<string, IReadOnlyList<string>> ScriptInserts { get; set; } = new Dictionary<string, IReadOnlyList<string>>();
        public Dictionary<string, RunnerEnvironment> Environments { get; set; } = new Dictionary<string, RunnerEnvironment>();
        public IReadOnlyList<ScriptInfo> Content { get; set; } = Array.Empty<ScriptInfo>();
    }

    public class RunnerEnvironment
    {
        public IReadOnlyList<string> BeforeLines { get; set; }
        public IReadOnlyList<string> AfterLines { get; set; }
    }
}
