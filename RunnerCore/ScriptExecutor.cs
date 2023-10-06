using RunnerCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RunnerCore
{
    /// <summary>
    /// Исполнитель скриптов
    /// </summary>
    public class ScriptExecutor
    {
        // Список используемых файлов по индексам
        private static readonly List<bool> usedFiles = new List<bool>();

        // Текущий исполняемый скрипт
        private readonly ScriptLines currentScript;

        public ScriptExecutor(ScriptLines currentScript)
        {
            this.currentScript = currentScript;
        }

        public void ExecuteAsTask(object sender, EventArgs e)
        {
            Task.Run(() => Execute(sender, e));
        }

        public void Execute(object sender, EventArgs e)
        {
            // Определение имени для создания файла со скриптом
            var fileIndex = GetFileIndex();
            var fileName = GetFileName(fileIndex);

            // Создание файла со скриптом
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllLines(path, currentScript.ScriptLines);

            // Запуск скрипта из созданного файла
            var process = Process.Start("powershell", path);
            process.WaitForExit();

            // Удаление файла после использования
            File.Delete(path);
            FreeFileIndex(fileIndex);
        }

        // Поиска неиспользуемого файла для записи скрипта 
        private int GetFileIndex()
        {
            lock (usedFiles)
            {
                for (int i = 0; i < usedFiles.Count; i++)
                {
                    if (!usedFiles[i])
                    {
                        usedFiles[i] = true;
                        return i;
                    }
                }

                var newIndex = usedFiles.Count;
                usedFiles.Add(true);
                return newIndex;
            }
        }

        private string GetFileName(int index)
        {
            return $"temp{index}.ps1";
        }

        private void FreeFileIndex(int index)
        {
            lock (usedFiles)
            {
                usedFiles[index] = false;
            }
        }
    }
}
