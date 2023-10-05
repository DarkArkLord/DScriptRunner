namespace RunnerCore.Parser
{
    // Список узлов конфигурации скрипта
    public static class ConfigNodes
    {
        // Корневой узел
        public const string Main = "Config";

        // Тест приветствия
        public const string Hello = "Hello";

        // Настройка сред
        public const string Environments = "Environments";
        // Настройка среды
        public const string Environment = "Environment";
        // Содержимоге среды перед скриптом
        public const string EnvironmentBefore = "Before";
        // Содержимоге среды после скрипта
        public const string EnvironmentAfter = "After";

        // Скриптовые вставки
        public const string ScriptInserts = "ScriptInserts";
        // Содержимое скриптовой вставки
        public const string ScriptInsert = "ScriptInsert";

        // Список скриптов
        public const string Scripts = "Scripts";
        // Группа скриптов
        public const string ScriptsGroup = "Group";
        // Скрипт
        public const string ScriptsScript = "Script";
        // Текстовый скрипт
        public const string ScriptText = "ScriptText";

        // Атрибуты
        // Имя
        public const string AttributeName = "name";
        // Используемая среда
        public const string AttributeEnvironment = "env";
        // Сокрытияе узла
        public const string AttributeHidden = "hidden";
    }
}
