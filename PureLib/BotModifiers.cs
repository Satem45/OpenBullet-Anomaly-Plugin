using RuriLib;
using PluginFramework;
using System;
using System.Collections.Generic;
using System.Text;
using PluginFramework.Attributes;
using RuriLib.Interfaces;
using RuriLib.Runner;

namespace Anomaly
{
    public class BotModifiers : IPlugin
    {
        public string Name => "Bot Modifiers";

        [TextMulti("Explanation")]
        public string[] How_To_Use { get; set; } = new string[] { "These Changes Alter how the Bots Work inside of OpenBullet.", "To Enable a feature Tick the box" };

        [Checkbox("Disable Wordlist Requirement")]
        public bool RequireWordlist { get; set; } = false;
        [Button("Set Changes")]
        public void Execute(IApplication app)
        {
            if (RequireWordlist)
            {
                
            }
        }
    }
}
