using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    /// <summary>
    /// Базовый класс конфигурации скрипта
    /// </summary>
    public abstract class ScriptInfo
    {
        public abstract bool IsScript { get; }
        public string Title { get; protected set; }
        public IReadOnlyList<ScriptInfo> Content { get; protected set; } = Array.Empty<ScriptInfo>();
        public IReadOnlyList<string> ScriptLines { get; protected set; } = Array.Empty<string>();

        protected ScriptInfo(string title)
        {
            Title = title;
        }
    }
}
