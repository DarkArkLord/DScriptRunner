using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class ScriptLines : ScriptInfo
    {
        public override bool IsScript => true;

        public ScriptLines(string title, IReadOnlyList<string> lines) : base(title)
        {
            ScriptLines = lines;
        }
    }
}
