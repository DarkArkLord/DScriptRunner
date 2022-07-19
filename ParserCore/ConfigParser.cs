using ParserCore.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ParserCore
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

            ParseGlobal(config, result);

            var xmlScriptSection = config.Element(ConfigNodes.Scripts);
            if (xmlScriptSection is null)
            {
                throw new Exception($"Секция {ConfigNodes.Scripts} не обнаружена");
            }

            var scriptSection = ParseScriptSection(xmlScriptSection);
            result.Content = scriptSection;

            return result;
        }

        private static void ParseGlobal(XElement xml, RunnerConfig config)
        {
            var global = xml.Element(ConfigNodes.Global);
            if (global is null)
            {
                return;
            }

            var helloMessage = global.Element(ConfigNodes.GlobalHello)?.Value ?? string.Empty;
            config.HelloMessage = helloMessage;

            var beforeText = global.Element(ConfigNodes.GlobalBefore)?.Value ?? string.Empty;
            var beforeLines = ParseCode(beforeText);
            config.BeforeScriptLines = beforeLines;

            var afterText = global.Element(ConfigNodes.GlobalAfter)?.Value ?? string.Empty;
            var afterLines = ParseCode(afterText);
            config.AfterScriptLines = afterLines;
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
                    var nodeText = node.Value;
                    var nodeLines = ParseCode(nodeText);
                    var scriptInfo = new ScriptLines(nodeName, nodeLines);
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
                .ToArray();
            return lines;
        }
    }
}
