using System;
using System.Collections.Generic;

namespace ParserCore.Entities
{
    public class RunnerConfig
    {
        public string HelloMessage { get; set; } = string.Empty;
        public IReadOnlyList<string> BeforeScriptLines { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> AfterScriptLines { get; set; } = Array.Empty<string>();
        public IReadOnlyList<ScriptInfo> Content { get; set; } = Array.Empty<ScriptInfo>();
    }
}
