using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.Interfaces;
using RuriLib.Models;
using RuriLib.ViewModels;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace Anomaly
{
    public class Updater : IPlugin
    {
        public string Name => "Anomaly Updater";
        public string UpdateStatus { get; set; } = "...";
        private static readonly HttpClient client = new HttpClient();
        [Button("Check For Update")]
        public async System.Threading.Tasks.Task ExecuteAsync(IApplication app)
        {
            string VersionURL = "https://raw.githubusercontent.com/PurityWasHere/OpenBullet-Anomaly-Plugin/master/VersionNumber.txt";
            string LocalVersion = "0.01";
            var response = await client.GetAsync(VersionURL);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Trim().Contains(LocalVersion))
            {
                app.Logger.Log($"Anomaly Plugin is Up To Date Version Number {LocalVersion}", LogLevel.Info, true);
            }
            else if (!responseString.Trim().Contains(LocalVersion))
            {
                app.Logger.Log($"Anomaly Plugin is not up to date. Newest Release is {responseString.Trim()}", LogLevel.Info, true);
            }
            else
            {
                app.Logger.Log("Error Checking For Update.", LogLevel.Info, true);
            }
        }
    }
}