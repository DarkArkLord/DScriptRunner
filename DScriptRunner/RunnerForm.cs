using Microsoft.Toolkit.Uwp.Notifications;
using RunnerCore;
using RunnerCore.Entities;
using RunnerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace DScriptRunner
{
    /// <summary>
    /// Основная форма приложения (значек в трее)
    /// </summary>
    class RunnerForm : ApplicationContext
    {
        private readonly NotifyIcon appIcon;
        private RunnerConfig appConfig;

        public RunnerForm()
        {
            appIcon = new NotifyIcon
            {
                Icon = AppResources.AppIcon,
                Visible = true,
            };
            LoadMenu();
        }

        private void LoadMenu()
        {
            var contextMenu = new ContextMenuStrip();

            // Считывание конфигураии раннера (возможны ошибки)
            try
            {
                var xml = ConfigParser.ReadXml(AppResources.ConfigFileName);
                appConfig = ConfigParser.ParseXml(xml);
                ConfigureMenu(contextMenu.Items, appConfig.Content);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, AppResources.LoadErrorCaption);
            }

            // Подготовка основных элементов управления приложением
            ConfigureCommonMenuItems(contextMenu);
            appIcon.ContextMenuStrip = contextMenu;

            // Вывод приветственного сообщения
            SayHello();
        }

        private void ConfigureMenu(ToolStripItemCollection items, IReadOnlyList<ScriptInfo> config)
        {
            foreach (var info in config)
            {
                // Если это скрипт - то он должен исполняться при нажатии
                // Иначе это группа, которая должна содержать набор скриптов 
                if (info.IsScript)
                {
                    var item = items.Add(info.Title);
                    var executor = new ScriptExecutor(info as ScriptLines);
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
            // Открытие файла конфигурации
            var configItem = menu.Items.Add(AppResources.ButtonConfig);
            configItem.Click += (object sender, EventArgs e) => OpenConfigFile();

            // Перезагрузка меню
            var refreshItem = menu.Items.Add(AppResources.ButtonRefresh);
            refreshItem.Click += (object sender, EventArgs e) => LoadMenu();

            // Информация о программе
            var infoItem = menu.Items.Add(AppResources.ButtonInfo);
            infoItem.Click += (object sender, EventArgs e) => Info();

            // Выход
            var exitItem = menu.Items.Add(AppResources.ButtonExit);
            exitItem.Click += (object sender, EventArgs e) => Exit();
        }

        private void OpenConfigFile()
        {
            var path = $"/select, \"{Environment.CurrentDirectory}\\{AppResources.ConfigFileName}\"";
            Process.Start("explorer.exe", path);
        }

        private void Info()
        {
            var text = "О программе DScriptRunner\n" +
                       "Разработчик: Алексей Петров aka DarkNessLord\n" +
                       "https://github.com/ShadowOfFallenLord/DScriptRunner \n" +
                       "2022 год \n" +
                       "Версия 1.6.2";
            MessageBox.Show(text, "О программе");
        }

        private void Exit()
        {
            appIcon.Visible = false;
            Application.Exit();
        }

        private void SayHello()
        {
            if (appConfig is null || string.IsNullOrEmpty(appConfig.HelloMessage)) return;

            var builder = new ToastContentBuilder();
            builder.AddText(appConfig.HelloMessage).Show();
        }
    }
}
