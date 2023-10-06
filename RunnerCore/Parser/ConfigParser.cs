using RunnerCore.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RunnerCore.Parser
{
    public static class ConfigParser
    {
        /// <summary>
        /// Считывание xml-документа из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Считанный xml-документ</returns>
        public static XDocument ReadXml(string path)
        {
            var file = File.ReadAllText(path);
            var xDocument = XDocument.Parse(file);
            return xDocument;
        }

        /// <summary>
        /// Парсинг конфигурации из xml-документа
        /// </summary>
        /// <param name="xml">xml-документ</param>
        /// <returns>Конфигурация раннера</returns>
        /// <exception cref="Exception">Ошибка при парсинге</exception>
        public static RunnerConfig ParseXml(XDocument xml)
        {
            var config = xml.Element(ConfigNodes.Main);
            // Если нет корневой секции - нечего парсить
            if (config is null)
            {
                throw new Exception($"Секция {ConfigNodes.Main} не обнаружена");
            }

            var result = new RunnerConfig();

            // Установки приветственного сообщения
            var helloMessage = config.Element(ConfigNodes.Hello)?.Value ?? string.Empty;
            result.HelloMessage = helloMessage.Trim();

            ParseEnvironmentsList(config, result);
            ParseScriptInserts(config, result);

            // Если нет секции скриптов, значит нет и скриптов
            var scriptSection = config.Element(ConfigNodes.Scripts);
            if (scriptSection is null)
            {
                throw new Exception($"Секция {ConfigNodes.Scripts} не обнаружена");
            }

            result.Content = ParseScriptSection(scriptSection, result);

            return result;
        }

        // Парсинг списка сред
        private static void ParseEnvironmentsList(XElement xml, RunnerConfig config)
        {
            var environmentsSection = xml.Element(ConfigNodes.Environments);
            // Нет секции - нет сред
            if (environmentsSection is null) return;

            foreach (var element in environmentsSection.Elements(ConfigNodes.Environment))
            {
                ParseEnvironment(element, config);
            }
        }

        // Парсинг среды
        private static void ParseEnvironment(XElement xml, RunnerConfig config)
        {
            var name = GetNodeName(xml);
            if (config.Environments.ContainsKey(name))
            {
                throw new Exception($"Среда {name} уже определена");
            }

            var beforeLines = ParseEnvironmentSection(xml, ConfigNodes.EnvironmentBefore, name);
            var afterLines = ParseEnvironmentSection(xml, ConfigNodes.EnvironmentAfter, name);

            if (beforeLines.Count + afterLines.Count < 1)
            {
                throw new Exception($"Среда {name} пуста. Добавьте дочернюю секцию {ConfigNodes.EnvironmentBefore} или {ConfigNodes.EnvironmentAfter}");
            }

            var env = new RunnerEnvironment
            {
                BeforeLines = beforeLines,
                AfterLines = afterLines,
            };

            config.Environments.Add(name, env);
        }

        // Парсинг секции среды (перед или после)
        private static IReadOnlyList<string> ParseEnvironmentSection(XElement xml, string section, string envName)
        {
            var sectionNode = xml.Element(section);
            var sectionText = sectionNode?.Value ?? string.Empty;
            var sectionLines = ParseCode(sectionText);
            if (sectionNode != null && sectionLines.Count < 1)
            {
                throw new Exception($"В среде {envName} определена пустая секция {section}");
            }
            return sectionLines;
        }

        // Парсинг скриптовых вставок
        private static void ParseScriptInserts(XElement xml, RunnerConfig config)
        {
            var scriptElementsSection = xml.Element(ConfigNodes.ScriptInserts);
            // Нет секции - нет вставок
            if (scriptElementsSection is null) return;

            foreach (var element in scriptElementsSection.Elements(ConfigNodes.ScriptInsert))
            {
                ParseScriptInsert(element, config);
            }
        }

        // Парсинг скриптовой вставки
        private static void ParseScriptInsert(XElement xml, RunnerConfig config)
        {
            var name = GetNodeName(xml);
            if (config.ScriptInserts.ContainsKey(name))
            {
                throw new Exception($"Скриптовая вставка {name} уже определена");
            }

            var scriptText = xml.Value ?? string.Empty;
            var scriptLines = ParseCode(scriptText);
            if (scriptLines.Count < 1)
            {
                throw new Exception($"Скриптовая вставка {name} пуста");
            }

            config.ScriptInserts.Add(name, scriptLines);
        }

        // Парсинг секции скриптов
        private static IReadOnlyList<ScriptInfo> ParseScriptSection(XElement xml, RunnerConfig config, RunnerEnvironment environment = null)
        {
            var result = new List<ScriptInfo>();

            foreach (var node in xml.Elements())
            {
                ParseScriptSectionElement(node, result, config, environment);
            }

            return result;
        }

        // Парсинг элемента секции скриптов
        private static void ParseScriptSectionElement(XElement xml, List<ScriptInfo> result, RunnerConfig config, RunnerEnvironment environment = null)
        {
            var isNodeVisible = xml.Attribute(ConfigNodes.AttributeHidden) is null;
            if (!isNodeVisible) return;

            if (xml.Name == ConfigNodes.ScriptsGroup)
            {
                var nodeName = GetNodeName(xml);
                var groupEnvironment = MergeEnvironments(xml, environment, config);
                var nodeContent = ParseScriptSection(xml, config, groupEnvironment);
                var scriptGroup = new ScriptsGroup(nodeName, nodeContent);
                result.Add(scriptGroup);
            }
            else if (xml.Name == ConfigNodes.ScriptsScript)
            {
                var scriptInfo = ParseScript(xml, environment, config);
                result.Add(scriptInfo);
            }
            else
            {
                throw new Exception($"Обнаруженна секция с неизвестным тегом {xml.Name}");
            }
        }

        // Объединение нескольких сред
        private static RunnerEnvironment MergeEnvironments(XElement xml, RunnerEnvironment current, RunnerConfig config)
        {
            var name = xml.Attribute(ConfigNodes.AttributeEnvironment)?.Value;
            if (name is null) return current;

            if (!config.Environments.ContainsKey(name))
            {
                throw new Exception($"Среда {name} не найдена");
            }

            var env = config.Environments[name];

            if (current is null) return env;

            var result = new RunnerEnvironment
            {
                BeforeLines = current.BeforeLines
                    .Concat(env.BeforeLines)
                    .ToArray(),
                AfterLines = env.AfterLines
                    .Concat(current.AfterLines)
                    .ToArray(),
            };

            return result;
        }

        // Парсинг скрипта
        private static ScriptLines ParseScript(XElement xml, RunnerEnvironment environment, RunnerConfig config)
        {
            var scriptName = GetNodeName(xml);

            var scriptLines = new List<string>();
            // Если в секции скрипта есть вложенные элементы - парсим скрипт как набор вставок
            // Иначе - как полнотекстовый скрипт
            if (xml.HasElements)
            {
                foreach (var innerNode in xml.Elements())
                {
                    var sectionLines = ParseScriptInnerNode(innerNode, scriptName, config);
                    var sectionEnvironment = MergeEnvironments(innerNode, null, config);
                    if (sectionEnvironment != null)
                    {
                        sectionLines = sectionEnvironment.BeforeLines
                            .Concat(sectionLines)
                            .Concat(sectionEnvironment.AfterLines)
                            .ToArray();
                    }
                    scriptLines.AddRange(sectionLines);
                }
            }
            else
            {
                var sectionLines = ParseCode(xml.Value);
                scriptLines.AddRange(sectionLines);
            }

            if (scriptLines.Count < 1)
            {
                throw new Exception($"В скрипте {scriptName} не содержится исполняемого кода. Добавьте код или дочернюю секцию {ConfigNodes.ScriptText} или {ConfigNodes.ScriptInsert}.");
            }

            // Объединение сред
            var scriptEnvironment = MergeEnvironments(xml, environment, config);
            if (scriptEnvironment != null)
            {
                scriptLines.InsertRange(0, scriptEnvironment.BeforeLines);
                scriptLines.AddRange(scriptEnvironment.AfterLines);
            }

            var scriptInfo = new ScriptLines(scriptName, scriptLines);
            return scriptInfo;
        }

        // Парсинг внутренних секций скрипта
        private static IReadOnlyList<string> ParseScriptInnerNode(XElement xml, string scriptName, RunnerConfig config)
        {
            if (xml.Name == ConfigNodes.ScriptText)
            {
                var nodeText = xml.Value;
                var nodeLines = ParseCode(nodeText);
                if (nodeLines.Count < 1)
                {
                    throw new Exception($"В скрипте {scriptName} есть пустая дочерняя секция {ConfigNodes.ScriptText}");
                }

                return nodeLines;
            }
            else if (xml.Name == ConfigNodes.ScriptInsert)
            {
                var insertName = GetNodeName(xml);
                if (!config.ScriptInserts.ContainsKey(insertName))
                {
                    throw new Exception($"Скриптовая вставка {insertName} не найдена");
                }
                var nodeLines = config.ScriptInserts[insertName];
                return nodeLines;
            }
            else
            {
                throw new Exception($"Обнаруженна секция с неизвестным тегом {xml.Name}");
            }
        }

        // Считывание имени xml-секции
        private static string GetNodeName(XElement xml)
        {
            var nodeName = xml.Attribute(ConfigNodes.AttributeName)?.Value;
            if (nodeName is null)
            {
                throw new Exception($"В секции {xml.Name} не обнаружен аттрибут {ConfigNodes.AttributeName}");
            }
            return nodeName;
        }

        // Разбиение кода на список строк
        private static IReadOnlyList<string> ParseCode(string code)
        {
            var lines = code?.Split('\n')
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrEmpty(value) && value.Length > 0)
                .ToArray() ?? Array.Empty<string>();
            return lines;
        }
    }
}
