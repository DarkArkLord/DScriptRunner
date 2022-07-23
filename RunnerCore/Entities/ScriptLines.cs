using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class ScriptLines : ScriptInfo
    {
        public override bool IsScript => true;
        public string Environment { get; private set; }

        public ScriptLines(string title, string env, IReadOnlyList<string> lines) : base(title)
        {
            Environment = env;
            ScriptLines = lines;
        }
    }
}
