using RunnerCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RunnerCore
{
    public class ScriptExecutor
    {
        private readonly ScriptLines currentScript;
        private readonly RunnerConfig config;

        public ScriptExecutor(ScriptLines currentScript, RunnerConfig config)
        {
            this.currentScript = currentScript;
            this.config = config;
        }

        public void ExecuteAsTask(object sender, EventArgs e)
        {
            Task.Run(() => Execute(sender, e));
        }

        public void Execute(object sender, EventArgs e)
        {
            var script = ConcatScript();
            File.WriteAllLines(RunnerResources.TempScriptFileName, script);
            var path = Path.Combine(Directory.GetCurrentDirectory(), RunnerResources.TempScriptFileName);
            var process = Process.Start("powershell", path);
            process.WaitForExit();
            File.Delete(RunnerResources.TempScriptFileName);
        }

        private IReadOnlyList<string> ConcatScript()
        {
            var script = new List<string>();
            if (config.BeforeScriptLines != null)
            {
                script.AddRange(config.BeforeScriptLines);
            }
            if (currentScript.ScriptLines != null)
            {
                script.AddRange(currentScript.ScriptLines);
            }
            if (config.AfterScriptLines != null)
            {
                script.AddRange(config.AfterScriptLines);
            }
            return script;
        }
    }
}
