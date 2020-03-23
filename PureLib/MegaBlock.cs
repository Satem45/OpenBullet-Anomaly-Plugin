using CG.Web.MegaApiClient;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;
using System;
using System.Collections.Generic;

namespace Anomaly
{
    public class BlockMegaNZ : BlockBase, IBlockPlugin
    {
        public string Name
        {
            get
            {
                return "MEGANZ";
            }
        }

        public string Color
        {
            get
            {
                return "Red";
            }
        }

        public bool LightForeground
        {
            get
            {
                return false;
            }
        }

        [Text("Email", "The mega.nz account's email")]
        public string Email { get; set; } = "<USER>";

        [Text("Password", "The mega.nz account's password")]
        public string Password { get; set; } = "<PASS>";

        [Numeric("Max Files Captured", "The maximum number of files to enumerate from the repository", minimum = 0, maximum = 5000)]
        public int MaxFilesCaptured { get; set; } = 20;

        [Text("Variable Name", "The output variable name")]
        public string VariableName { get; set; } = "";

        [Checkbox("Is Capture", "Should the output variable be marked as capture?")]
        public bool IsCapture { get; set; } = true;

        public BlockMegaNZ()
        {
            base.Label = this.Name;
        }

        public override BlockBase FromLS(string line)
        {
            string text = line.Trim();
            bool flag = text.StartsWith("#");
            if (flag)
            {
                base.Label = LineParser.ParseLabel(ref text);
            }
            this.Email = LineParser.ParseLiteral(ref text, "Email", false, null);
            this.Password = LineParser.ParseLiteral(ref text, "Password", false, null);
            this.MaxFilesCaptured = LineParser.ParseInt(ref text, "MaxFilesCaptured");
            bool flag2 = LineParser.ParseToken(ref text, TokenType.Arrow, false, true) == "";
            BlockBase result;
            if (flag2)
            {
                result = this;
            }
            else
            {
                try
                {
                    string text2 = LineParser.ParseToken(ref text, TokenType.Parameter, true, true);
                    bool flag3 = text2.ToUpper() == "VAR" || text2.ToUpper() == "CAP";
                    if (flag3)
                    {
                        this.IsCapture = (text2.ToUpper() == "CAP");
                    }
                }
                catch
                {
                    throw new ArgumentException("Invalid or missing variable type");
                }
                try
                {
                    this.VariableName = LineParser.ParseToken(ref text, TokenType.Literal, true, true);
                }
                catch
                {
                    throw new ArgumentException("Variable name not specified");
                }
                result = this;
            }
            return result;
        }

        public override string ToLS(bool indent = true)
        {
            BlockWriter blockWriter = new BlockWriter(base.GetType(), indent, base.Disabled).Label(base.Label).Token(this.Name, "").Literal(this.Email, "").Literal(this.Password, "").Integer(this.MaxFilesCaptured, "");
            bool flag = !blockWriter.CheckDefault(this.VariableName, "VariableName");
            if (flag)
            {
                blockWriter.Arrow().Token(this.IsCapture ? "CAP" : "VAR", "").Literal(this.VariableName, "");
            }
            return blockWriter.ToString();
        }

        public override void Process(BotData data)
        {
            base.Process(data);
            string email = BlockBase.ReplaceValues(this.Email, data);
            string password = BlockBase.ReplaceValues(this.Password, data);
            MegaApiClient megaApiClient = new MegaApiClient();
            try
            {
                megaApiClient.Login(email, password);
                bool flag = !megaApiClient.IsLoggedIn;
                if (flag)
                {
                    throw new Exception();
                }
                IEnumerator<INode> enumerator = megaApiClient.GetNodes().GetEnumerator();
                List<string> list = new List<string>();
                int num = 0;
                while (enumerator.MoveNext() && num < this.MaxFilesCaptured)
                {
                    bool flag2 = enumerator.Current.Name != null;
                    if (flag2)
                    {
                        list.Add(enumerator.Current.Name);
                        num++;
                    }
                }
                IAccountInformation accountInformation = megaApiClient.GetAccountInformation();
                BlockBase.InsertVariable(data, this.IsCapture, accountInformation.UsedQuota.ToString(), "USEDQUOTA", "", "", false, false);
                BlockBase.InsertVariable(data, this.IsCapture, accountInformation.TotalQuota.ToString(), "TOTALQUOTA", "", "", false, false);
                BlockBase.InsertVariable(data, this.IsCapture, list, this.VariableName, "", "", false, false);
                data.Status = RuriLib.BotStatus.SUCCESS;
            }
            catch (Exception ex)
            {
                data.Log(ex.Message);
                data.Status = RuriLib.BotStatus.FAIL;
            }
        }
    }
}