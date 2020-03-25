using Newtonsoft.Json;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.Interfaces;
using System;
using System.Net.Http;

namespace Anomaly
{
    public class Updater : IPlugin
    {
        public string Name => "Anomaly Updater";
        public string UpdateStatus { get; set; } = "...";

        [Button("Check For Update")]
        public async System.Threading.Tasks.Task CheckUpdate(IApplication app)
        {
            string VersionURL = Anomaly.Globals.VersionURL;
            string ChangelogURL = Anomaly.Globals.ChangelogURL;
            string LocalVersion = Anomaly.Globals.VersionNumber;
            HttpClient client = new HttpClient();
            using (HttpResponseMessage response = await client.GetAsync(VersionURL))
            {
                try
                {
                    var LatestVersion = await response.Content.ReadAsStringAsync();
                    if (LatestVersion.Trim() == LocalVersion)
                    {
                        app.Logger.Log($"Anomaly Plugin is Up To Date. v{LocalVersion}", LogLevel.Info, true);
                    }
                    else
                    {
                        app.Logger.Log($"Anomaly is Not Up To Date. LV: {LatestVersion}", LogLevel.Error, true);
                    }
                }
                catch (Exception ex) { app.Logger.Log($"Error in Update Check. Ex:{ex}", LogLevel.Error, true); }
            }
            using (HttpResponseMessage response = await client.GetAsync(ChangelogURL))
            {
                var Changelog = await response.Content.ReadAsStringAsync();
                ChangeLog = Changelog;
                app.Logger.Log(ChangeLog, LogLevel.Info, true);
            }
        }

        //This class grabs the Json response Anomaly needs from Github. This is the Version Number it saves.
        public partial class GHApi
        {
            [JsonProperty("tag_name")]
            public string TagName { get; set; }
        }

        [Button("Install Update")]
        public async System.Threading.Tasks.Task Update(IApplication app)
        {
            try
            {
                //The Http Request
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0");
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                var response = await client.GetAsync(Anomaly.Globals.LatestAPIVersion);
                var content = await response.Content.ReadAsStringAsync();
                //The Json Parse
                GHApi ParseAPI = JsonConvert.DeserializeObject<GHApi>(content);
                //This will get the latest Release DLL From Github
                var client2 = new System.Net.WebClient();
                //This will save with the extension
                client2.DownloadFile(Anomaly.Globals.LatestDownload, $@".\Plugins\Anomaly-{ParseAPI.TagName.Trim()}.zip");
                app.Logger.Log("Downloaded Update. Please Unzip and Restart OB.", LogLevel.Info, true);
            }
            catch (Exception ex) { app.Logger.Log($"Error Collecting ID Or Saving. ex:{ex}", LogLevel.Error, true); }
        }

        [Text("Changelog")]
        public string ChangeLog { get; set; } = "";
    }
}