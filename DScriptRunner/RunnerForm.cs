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
        private RunnerConfig appConfig;

        public RunnerForm()
        {
            appIcon = new NotifyIcon
            {
                Icon = RunnerResources.AppIcon,
                Visible = true,
            };
            LoadMenu();
        }

        private void LoadMenu()
        {
            var xml = ConfigParser.ReadXml(RunnerResources.ConfigFileName);
            appConfig = ConfigParser.ParseXml(xml);
            var menuContainer = PrepareContextMenu(appConfig);
            appIcon.ContextMenuStrip = menuContainer;
        }

        private ContextMenuStrip PrepareContextMenu(RunnerConfig config)
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
            configItem.Click += (object sender, EventArgs e) => OpenConfigFile();

            var refreshItem = menu.Items.Add("Refresh");
            refreshItem.Click += (object sender, EventArgs e) => LoadMenu();

            var exitItem = menu.Items.Add("Exit");
            exitItem.Click += (object sender, EventArgs e) => Exit();
        }

        private void OpenConfigFile()
        {
            var path = $"/select, \"{Environment.CurrentDirectory}\\{RunnerResources.ConfigFileName}\"";
            Process.Start("explorer.exe", path);
        }

        private void Exit()
        {
            appIcon.Visible = false;
            Application.Exit();
        }
    }
}
