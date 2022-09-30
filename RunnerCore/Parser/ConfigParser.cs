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

            ParseEnvironments(config, result);

            var xmlScriptSection = config.Element(ConfigNodes.Scripts);
            if (xmlScriptSection is null)
            {
                throw new Exception($"Секция {ConfigNodes.Scripts} не обнаружена");
            }

            var scriptSection = ParseScriptSection(xmlScriptSection);
            result.Content = scriptSection;

            return result;
        }

        private static void ParseEnvironments(XElement xml, RunnerConfig config)
        {
            var environmentSection = xml.Element(ConfigNodes.Environments);
            if (environmentSection is null)
            {
                return;
            }

            foreach (var environment in environmentSection.Elements(ConfigNodes.Environment))
            {
                var name = environment.Attribute(ConfigNodes.AttributeName)?.Value;
                if (name is null)
                {
                    throw new Exception($"В секции {environment.Name} не обнаружен аттрибут {ConfigNodes.AttributeName}");
                }
                if (config.Environments.ContainsKey(name))
                {
                    throw new Exception($"Среда {name} уже определена");
                }

                var beforeText = environment.Element(ConfigNodes.EnvironmentBefore)?.Value ?? string.Empty;
                var beforeLines = ParseCode(beforeText);

                var afterText = environment.Element(ConfigNodes.EnvironmentAfter)?.Value ?? string.Empty;
                var afterLines = ParseCode(afterText);

                var envConf = (beforeLines, afterLines);
                config.Environments.Add(name, envConf);
            }
        }

        private static IReadOnlyList<ScriptInfo> ParseScriptSection(XElement xml)
        {
            var result = new List<ScriptInfo>();
            var childs = xml.Elements();
            foreach (var node in childs)
            {
                if (node.Name == ConfigNodes.ScriptsScript)
                {
                    var nodeNameAttr = node.Attribute(ConfigNodes.AttributeName);
                    if (nodeNameAttr is null)
                    {
                        throw new Exception($"В секции {node.Name} не обнаружен аттрибут {ConfigNodes.AttributeName}");
                    }

                    var nodeName = nodeNameAttr.Value;
                    var nodeEnv = node.Attribute(ConfigNodes.AttributeEnvironment)?.Value ?? string.Empty;
                    // Exception для сред, которые не были определены?
                    var nodeText = node.Value;
                    var nodeLines = ParseCode(nodeText);
                    var scriptInfo = new ScriptLines(nodeName, nodeEnv, nodeLines);
                    result.Add(scriptInfo);
                }
                else if (node.Name == ConfigNodes.ScriptsGroup)
                {
                    var nodeNameAttr = node.Attribute(ConfigNodes.AttributeName);
                    if (nodeNameAttr is null)
                    {
                        throw new Exception($"В секции {node.Name} не обнаружен аттрибут {ConfigNodes.AttributeName}");
                    }

                    var nodeName = nodeNameAttr.Value;
                    var nodeContent = ParseScriptSection(node);
                    var scriptGroup = new ScriptsGroup(nodeName, nodeContent);
                    result.Add(scriptGroup);
                }
                else
                {
                    throw new Exception($"Обнаруженна секция с неизвестным тегом {node.Name}");
                }
            }

            return result;
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
