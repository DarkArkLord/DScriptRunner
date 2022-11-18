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
        public static XDocument ReadXml(string path)
        {
            var file = File.ReadAllText(path);
            var xDocument = XDocument.Parse(file);
            return xDocument;
        }

        public static RunnerConfig ParseXml(XDocument xml)
        {
            var config = xml.Element(ConfigNodes.Main);
            if (config is null)
            {
                throw new Exception($"Секция {ConfigNodes.Main} не обнаружена");
            }

            var result = new RunnerConfig();

            var helloMessage = config.Element(ConfigNodes.Hello)?.Value ?? string.Empty;
            result.HelloMessage = helloMessage.Trim();

            ParseEnvironmentsList(config, result);
            ParseScriptElements(config, result);

            var scriptSection = config.Element(ConfigNodes.Scripts);
            if (scriptSection is null)
            {
                throw new Exception($"Секция {ConfigNodes.Scripts} не обнаружена");
            }

            result.Content = ParseScriptSection(scriptSection, result);

            return result;
        }

        private static void ParseEnvironmentsList(XElement xml, RunnerConfig config)
        {
            var environmentsSection = xml.Element(ConfigNodes.Environments);
            if (environmentsSection is null)
            {
                return;
            }

            foreach (var element in environmentsSection.Elements(ConfigNodes.Environment))
            {
                var name = GetNodeName(element);
                if (config.Environments.ContainsKey(name))
                {
                    throw new Exception($"Среда {name} уже определена");
                }

                var beforeNode = element.Element(ConfigNodes.EnvironmentBefore);
                var beforeText = beforeNode?.Value ?? string.Empty;
                var beforeLines = ParseCode(beforeText);
                if (beforeNode != null && beforeLines.Count < 1)
                {
                    throw new Exception($"В среде {name} определена пустая секция {ConfigNodes.EnvironmentBefore}");
                }

                var afterNode = element.Element(ConfigNodes.EnvironmentAfter);
                var afterText = afterNode?.Value ?? string.Empty;
                var afterLines = ParseCode(afterText);
                if (afterNode != null && afterLines.Count < 1)
                {
                    throw new Exception($"В среде {name} определена пустая секция {ConfigNodes.EnvironmentAfter}");
                }

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
        }

        private static void ParseScriptElements(XElement xml, RunnerConfig config)
        {
            var scriptElementsSection = xml.Element(ConfigNodes.ScriptElements);
            if (scriptElementsSection is null)
            {
                return;
            }

            foreach (var element in scriptElementsSection.Elements(ConfigNodes.ScriptElement))
            {
                var name = GetNodeName(element);
                if (config.ScriptElements.ContainsKey(name))
                {
                    throw new Exception($"Скриптовая вставка {name} уже определена");
                }

                var scriptText = element.Value ?? string.Empty;
                var scriptLines = ParseCode(scriptText);
                if (scriptLines.Count < 1)
                {
                    throw new Exception($"Скриптовая вставка {name} пуста");
                }

                config.ScriptElements.Add(name, scriptLines);
            }
        }

        private static IReadOnlyList<ScriptInfo> ParseScriptSection(XElement xml, RunnerConfig config, RunnerEnvironment environment = null)
        {
            var result = new List<ScriptInfo>();
            foreach (var node in xml.Elements())
            {
                var isNodeVisible = node.Attribute(ConfigNodes.AttributeHidden) is null;
                if (!isNodeVisible)
                {
                    continue;
                }

                if (node.Name == ConfigNodes.ScriptsGroup)
                {
                    var nodeName = GetNodeName(node);
                    var groupEnvironment = MergeEnvironments(node, environment, config);
                    var nodeContent = ParseScriptSection(node, config, groupEnvironment);
                    var scriptGroup = new ScriptsGroup(nodeName, nodeContent);
                    result.Add(scriptGroup);
                }
                else if (node.Name == ConfigNodes.ScriptsScript)
                {
                    var scriptInfo = ParseScript(node, environment, config);
                    result.Add(scriptInfo);
                }
                else
                {
                    throw new Exception($"Обнаруженна секция с неизвестным тегом {node.Name}");
                }
            }

            return result;
        }

        private static RunnerEnvironment MergeEnvironments(XElement xml, RunnerEnvironment current, RunnerConfig config)
        {
            var name = xml.Attribute(ConfigNodes.AttributeEnvironment)?.Value;
            if (name is null)
            {
                return current;
            }

            if (!config.Environments.ContainsKey(name))
            {
                throw new Exception($"Среда {name} не найдена");
            }

            var env = config.Environments[name];

            if (current is null)
            {
                return env;
            }

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

        private static ScriptLines ParseScript(XElement xml, RunnerEnvironment environment, RunnerConfig config)
        {
            var scriptName = GetNodeName(xml);

            var scriptLines = new List<string>();
            foreach (var innerNode in xml.Elements())
            {
                var sectionLines = ParseScriptInnerNodes(innerNode, scriptName, config);
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

            if (scriptLines.Count < 1)
            {
                throw new Exception($"В скрипте {scriptName} не содержится исполняемого кода. Добавьте дочернюю секцию {ConfigNodes.ScriptText} или {ConfigNodes.ScriptElement}");
            }

            var scriptEnvironment = MergeEnvironments(xml, environment, config);
            if (scriptEnvironment != null)
            {
                scriptLines.InsertRange(0, scriptEnvironment.BeforeLines);
                scriptLines.AddRange(scriptEnvironment.AfterLines);
            }

            var scriptInfo = new ScriptLines(scriptName, scriptLines);
            return scriptInfo;
        }

        private static IReadOnlyList<string> ParseScriptInnerNodes(XElement xml, string scriptName, RunnerConfig config)
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
            else if (xml.Name == ConfigNodes.ScriptElement)
            {
                var insertName = GetNodeName(xml);
                if (!config.ScriptElements.ContainsKey(insertName))
                {
                    throw new Exception($"Скриптовая вставка {insertName} не найдена");
                }
                var nodeLines = config.ScriptElements[insertName];
                return nodeLines;
            }
            else
            {
                throw new Exception($"Обнаруженна секция с неизвестным тегом {xml.Name}");
            }
        }

        private static string GetNodeName(XElement xml)
        {
            var nodeName = xml.Attribute(ConfigNodes.AttributeName)?.Value;
            if (nodeName is null)
            {
                throw new Exception($"В секции {xml.Name} не обнаружен аттрибут {ConfigNodes.AttributeName}");
            }
            return nodeName;
        }

        private static IReadOnlyList<string> ParseCode(string code)
        {
            var lines = code.Split('\n')
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrEmpty(value) && value.Length > 0)
                .ToArray();
            return lines;
        }
    }
}
