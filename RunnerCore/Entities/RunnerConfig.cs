using System;
using System.Collections.Generic;

namespace RunnerCore.Entities
{
    /// <summary>
    /// Полная конфигурация скриптраннера
    /// </summary>
    public class RunnerConfig
    {
        // Приветственное сообщение
        public string HelloMessage { get; set; } = string.Empty;
        // Скриптовые вставки
        public Dictionary<string, IReadOnlyList<string>> ScriptInserts { get; set; } = new Dictionary<string, IReadOnlyList<string>>();
        // Среды
        public Dictionary<string, RunnerEnvironment> Environments { get; set; } = new Dictionary<string, RunnerEnvironment>();
        // Набор групп и скриптов
        public IReadOnlyList<ScriptInfo> Content { get; set; } = Array.Empty<ScriptInfo>();
    }

    /// <summary>
    /// Конфигурация среды для скриптов
    /// </summary>
    public class RunnerEnvironment
    {
        // Строки, добавляемые перед скриптом
        public IReadOnlyList<string> BeforeLines { get; set; }
        // Строки, добавляемые после скрипта
        public IReadOnlyList<string> AfterLines { get; set; }
    }
}
