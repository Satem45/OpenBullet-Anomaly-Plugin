using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.Interfaces;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Anomaly
{
    public class AnomalyExtras : IPlugin
    {
        public string Name => "Anomaly Extras";

        [Button("Convert .Anom to .Loli")]
        public void Convert(IApplication app)
        {
            try
            {
                string[] Files = Directory.GetFiles(@".\Configs");
                foreach (string CurFile in Files)
                {
                    if (CurFile.Contains(".anom"))
                    {
                        string NewName = Path.ChangeExtension(CurFile, ".loli");
                        File.Move(CurFile, NewName);
                    }
                }
                app.Logger.Log($"Renamed Config Extensions", LogLevel.Info, false);
            }
            catch (Exception ex) { app.Logger.Log($"An Error Occured While trying to rename files: {ex}", LogLevel.Error, true); }
        }

        [Button("Scrape HTTP Proxies")]
        public async System.Threading.Tasks.Task ScrapeHTTPAsync(IApplication app)
        {
            try
            {
                HttpClient Http = new HttpClient();
                Http.Timeout = TimeSpan.FromSeconds(15);
                var response = await Http.GetAsync(Anomaly.Globals.HttpProxyUrl);
                var content = await response.Content.ReadAsStringAsync();
                List<CProxy> list = new List<CProxy>();
                string[] Proxies = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in Proxies)
                {
                    list.Add(new CProxy
                    {
                        Proxy = line,
                        Type = Extreme.Net.ProxyType.Http
                    });
                }
                app.ProxyManager.AddRange(list);
                app.Logger.Log(string.Format("{0} proxies have been added to your proxy manager!", list.Count), 0, true, 0);
            }
            catch (Exception ex)
            { app.Logger.Log($"Error Adding proxies ex:{ex}", LogLevel.Error, false); }
        }

        [Button("Scrape Socks4 Proxies")]
        public async System.Threading.Tasks.Task ScrapeSocks4Proxies(IApplication app)
        {
            try
            {
                HttpClient Http = new HttpClient();
                Http.Timeout = TimeSpan.FromSeconds(15);
                var response = await Http.GetAsync(Anomaly.Globals.Socks4ProxyUrl);
                var content = await response.Content.ReadAsStringAsync();
                List<CProxy> list = new List<CProxy>();
                string[] Proxies = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in Proxies)
                {
                    list.Add(new CProxy
                    {
                        Proxy = line,
                        Type = Extreme.Net.ProxyType.Socks4
                    });
                }
                app.ProxyManager.AddRange(list);
                app.Logger.Log(string.Format("{0} proxies have been added to your proxy manager!", list.Count), 0, true, 0);
            }
            catch (Exception ex)
            { app.Logger.Log($"Error Adding proxies ex:{ex}", LogLevel.Error, false); }
        }

        [Button("Scrape Socks5 Proxies")]
        public async System.Threading.Tasks.Task ScrapeSocks5Proxies(IApplication app)
        {
            try
            {
                HttpClient Http = new HttpClient();
                Http.Timeout = TimeSpan.FromSeconds(15);
                var response = await Http.GetAsync(Anomaly.Globals.Socks5ProxyUrl);
                var content = await response.Content.ReadAsStringAsync();
                List<CProxy> list = new List<CProxy>();
                string[] Proxies = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in Proxies)
                {
                    list.Add(new CProxy
                    {
                        Proxy = line,
                        Type = Extreme.Net.ProxyType.Socks5
                    });
                }
                app.ProxyManager.AddRange(list);
                app.Logger.Log(string.Format("{0} proxies have been added to your proxy manager!", list.Count), 0, true, 0);
            }
            catch (Exception ex)
            { app.Logger.Log($"Error Adding proxies ex:{ex}", LogLevel.Error, false); }
        }

        [Button("Patch Environment File (Requires Restart)")]
        public void PatchEnvironment(IApplication app)
        {
            try
            {
                var client = new System.Net.WebClient();
                client.DownloadFile(Anomaly.Globals.EnvironmentURL, Anomaly.Globals.EnvironmentPath);
                app.Logger.Log("Installed new Environment.ini, Please Restart.", LogLevel.Info, true);
            }
            catch (Exception ex) {app.Logger.Log($"Failed Installing new Environment.ini ex:{ex}", LogLevel.Error, true); }
        }
    }
}