using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    /// <summary>
    /// Базовый класс конфигурации скрипта
    /// </summary>
    public abstract class ScriptInfo
    {
        // Показывает, скрипт это или группа
        public abstract bool IsScript { get; }
        // Название
        public string Title { get; protected set; }
        // Вложенные конфигурации для группы
        public IReadOnlyList<ScriptInfo> Content { get; protected set; } = Array.Empty<ScriptInfo>();
        // Строки скрипта
        public IReadOnlyList<string> ScriptLines { get; protected set; } = Array.Empty<string>();

        protected ScriptInfo(string title)
        {
            Title = title;
        }
    }
}
