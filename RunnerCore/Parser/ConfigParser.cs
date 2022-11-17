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

            ParseScriptElements(config, result);

            var xmlScriptSection = config.Element(ConfigNodes.Scripts);
            if (xmlScriptSection is null)
            {
                throw new Exception($"Секция {ConfigNodes.Scripts} не обнаружена");
            }

            var scriptSection = ParseScriptSection(xmlScriptSection, result);
            result.Content = scriptSection;

            return result;
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

        private static IReadOnlyList<ScriptInfo> ParseScriptSection(XElement xml, RunnerConfig config)
        {
            var result = new List<ScriptInfo>();
            var childs = xml.Elements();
            foreach (var node in childs)
            {
                var isNodeVisible = node.Attribute(ConfigNodes.AttributeHidden) is null;
                if (!isNodeVisible)
                {
                    continue;
                }

                if (node.Name == ConfigNodes.ScriptsGroup)
                {
                    var nodeName = GetNodeName(node);
                    var nodeContent = ParseScriptSection(node, config);
                    var scriptGroup = new ScriptsGroup(nodeName, nodeContent);
                    result.Add(scriptGroup);
                }
                else if (node.Name == ConfigNodes.ScriptsScript)
                {
                    var nodeName = GetNodeName(node);

                    var scriptLines = new List<string>();
                    foreach (var innerNode in node.Elements())
                    {
                        if (innerNode.Name == ConfigNodes.ScriptText)
                        {
                            var nodeText = innerNode.Value;
                            var nodeLines = ParseCode(nodeText);
                            if (nodeLines.Count < 1)
                            {
                                throw new Exception($"В скрипте {nodeName} есть пустая дочерняя секция {ConfigNodes.ScriptText}");
                            }

                            scriptLines.AddRange(nodeLines);
                        }
                        else if (innerNode.Name == ConfigNodes.ScriptElement)
                        {
                            var insertName = GetNodeName(innerNode);
                            if (!config.ScriptElements.ContainsKey(insertName))
                            {
                                throw new Exception($"Скриптовая вставка {insertName} не найдена");
                            }
                            var nodeLines = config.ScriptElements[insertName];
                            scriptLines.AddRange(nodeLines);
                        }
                        else
                        {
                            throw new Exception($"Обнаруженна секция с неизвестным тегом {innerNode.Name}");
                        }
                    }

                    if (scriptLines.Count < 1)
                    {
                        throw new Exception($"В скрипте {nodeName} не содержится исполняемого кода. Добавьте дочернюю секцию {ConfigNodes.ScriptText} или {ConfigNodes.ScriptElement}");
                    }

                    var scriptInfo = new ScriptLines(nodeName, scriptLines);
                    result.Add(scriptInfo);
                }
                else
                {
                    throw new Exception($"Обнаруженна секция с неизвестным тегом {node.Name}");
                }
            }

            return result;
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
