using System.Collections.Generic;

namespace RunnerCore.Entities
{
    public class ScriptsGroup : ScriptInfo
    {
        public override bool IsScript => false;

        public ScriptsGroup(string title, IReadOnlyList<ScriptInfo> content) : base(title)
        {
            Content = content;
        }
    }
}
