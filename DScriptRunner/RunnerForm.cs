using ParserCore;
using ParserCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace DScriptRunner
{
    class RunnerForm : ApplicationContext
    {
        private readonly NotifyIcon appIcon;
        private readonly RunnerConfig appConfig;

        public RunnerForm()
        {
            var xml = ConfigParser.ReadXml(RunnerResources.ConfigFileName);
            appConfig = ConfigParser.ParseXml(xml);
            var menuContainer = PrepareMenu(appConfig);
            appIcon = new NotifyIcon
            {
                Icon = RunnerResources.AppIcon,
                Visible = true,
                ContextMenuStrip = menuContainer,
            };
        }

        private ContextMenuStrip PrepareMenu(RunnerConfig config)
        {
            var contextMenu = new ContextMenuStrip();
            ConfigureMenu(contextMenu.Items, config.Content);
            ConfigureCommonMenuItems(contextMenu);
            return contextMenu;
        }

        private void ConfigureMenu(ToolStripItemCollection items, IReadOnlyList<ScriptInfo> config)
        {
            foreach (var info in config)
            {
                if (info.IsScript)
                {
                    var item = items.Add(info.Title);
                    var executor = new ScriptExecutor(info as ScriptLines, appConfig);
                    item.Click += executor.ExecuteAsTask;
                }
                else
                {
                    var item = new ToolStripMenuItem(info.Title);
                    ConfigureMenu(item.DropDownItems, info.Content);
                    items.Add(item);
                }
            }
        }

        private void ConfigureCommonMenuItems(ContextMenuStrip menu)
        {
            var configItem = menu.Items.Add("Config");
            configItem.Click += OpenConfigFile;

            var exitItem = menu.Items.Add("Exit");
            exitItem.Click += Exit;
        }

        private void OpenConfigFile(object sender, EventArgs e)
        {
            var path = $"/select, \"{Environment.CurrentDirectory}\\{RunnerResources.ConfigFileName}\"";
            Process.Start("explorer.exe", path);
        }

        private void Exit(object sender, EventArgs e)
        {
            appIcon.Visible = false;
            Application.Exit();
        }
    }
}
