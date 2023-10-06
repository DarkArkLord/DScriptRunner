using System.Collections.Generic;

namespace RunnerCore.Entities
{
    /// <summary>
    /// Конфигурация скрипта
    /// </summary>
    public class ScriptLines : ScriptInfo
    {
        public override bool IsScript => true;

        public ScriptLines(string title, IReadOnlyList<string> lines) : base(title)
        {
            ScriptLines = lines;
        }
    }
}
