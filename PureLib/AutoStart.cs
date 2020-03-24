using PluginFramework;
using RuriLib;
using RuriLib.Interfaces;
using System;
using System.ComponentModel;
using System.IO;

namespace Anomaly
{
    public class AutoStart : IPlugin
    {
        public string Name
        {
            get
            {
                return "AutoStart";
            }
        }

        private BackgroundWorker BWWorker = new BackgroundWorker();

        private IApplication app;

        private void Work(object sender, DoWorkEventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter($@".\Plugins\AnomalySettings.json");
                sw.WriteLine("Test");
                sw.Close();
            }
            catch (Exception ex) { app.Logger.Log($"Something Fucked Up: {ex}", LogLevel.Error, false); }
        }

        public AutoStart(IApplication app)
        {
            this.app = app;
            this.BWWorker.DoWork += this.Work;
            this.BWWorker.RunWorkerAsync();
            app.Logger.Log("Anomaly Plugin Initialized.", LogLevel.Info, false);
        }
    }
}