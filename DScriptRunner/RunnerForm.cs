using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DScriptRunner
{
    class RunnerForm : ApplicationContext
    {
        private readonly NotifyIcon appIcon;

        public RunnerForm()
        {
            var menuContainer = PrepareMenu();
            appIcon = new NotifyIcon()
            {
                Icon = RunnerResources.AppIcon,
                Visible = true,
                ContextMenuStrip = menuContainer,
            };
        }

        private ContextMenuStrip PrepareMenu()
        {
            var menuItem1 = new ToolStripMenuItem("Temp1");
            var item = menuItem1.DropDownItems.Add("Config");
            item.Click += OpenConfigFile;

            var menuItem2 = new ToolStripMenuItem("Temp2");
            menuItem1.DropDownItems.Add(menuItem2);

            item = menuItem2.DropDownItems.Add("Exit");
            item.Click += Exit;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(menuItem1); 
            return contextMenu;
        }

        private void Exit(object sender, EventArgs e)
        {
            appIcon.Visible = false;
            Application.Exit();
        }

        private void OpenConfigFile(object sender, EventArgs e)
        {
            var path = $"/select, \"{Environment.CurrentDirectory}\\{RunnerResources.ConfigFileName}\"";
            Process.Start("explorer.exe", path);
        }
    }
}
