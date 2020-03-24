using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.Interfaces;
using System;
using System.IO;

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
                app.Logger.Log($"renamed Config Extensions", LogLevel.Info, false);
            }
            catch (Exception ex) { app.Logger.Log($"An Error Occured While trying to rename files: {ex}", LogLevel.Error, true); }
        }
    }
}