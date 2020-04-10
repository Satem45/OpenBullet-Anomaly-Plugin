using DotNetApiGatewayIam;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;
using System;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Anomaly
{
    public class AWSTimeStamp : BlockBase, IBlockPlugin
    {
        public string Name => "AWSToken";
        public string Color => "Aqua";
        public bool LightForeground => false;

        [Text("Variable Name", "The output variable name")]
        public string VariableName { get; set; } = "";

        [Text("AccessKey", "AWS Access Key")]
        public string AccessKey { get; set; } = "";

        [Text("AWSService", "API Endpoint Stages")]
        public string AWSService { get; set; } = "";

        [Text("RegionName", "API Region")]
        public string RegionName { get; set; } = "";

        [Text("AWSDateStamp", "Session Token")]
        public string AWSDateStamp { get; set; } = "";

        [Checkbox("Is Capture", "Should the output variable be marked as capture?")]
        public bool IsCapture { get; set; } = false;

        public AWSTimeStamp()
        {
            Label = Name;
        }

        public override BlockBase FromLS(string line)
        {
            var input = line.Trim();
            if (input.StartsWith("#")) // If the input actually has a label
                Label = LineParser.ParseLabel(ref input); // Parse the label and remove it from the original string
                VariableName = LineParser.ParseLiteral(ref input, "Variable Name");
                AccessKey = LineParser.ParseLiteral(ref input, "AcessKey");              
                RegionName = LineParser.ParseLiteral(ref input, "AWS Region Name");
                AWSService = LineParser.ParseLiteral(ref input, "AWS API Stages");
                AWSDateStamp = LineParser.ParseLiteral(ref input, "AWS Session token");
            if (LineParser.ParseToken(ref input, TokenType.Arrow, false) == "")
                return this;
            try
            {
                var varType = LineParser.ParseToken(ref input, TokenType.Parameter, true);
                if (varType.ToUpper() == "VAR" || varType.ToUpper() == "CAP")
                    IsCapture = varType.ToUpper() == "CAP";
            }
            catch { throw new ArgumentException("Invalid or missing variable type"); }
            try { VariableName = LineParser.ParseToken(ref input, TokenType.Literal, true); }
            catch { throw new ArgumentException("Variable name not specified"); }
            return this;
        }

        public override string ToLS(bool indent = true)
        {
            var writer = new BlockWriter(GetType(), indent, Disabled)
                .Label(Label) // Write the label. If the label is the default one, nothing is written.
                .Token(Name) // Write the block name. This cannot be changed.
                .Literal(VariableName)
                .Literal(AccessKey)
                .Literal(RegionName)
                .Literal(AWSService)
                .Literal(AWSDateStamp);
            if (!writer.CheckDefault(VariableName, nameof(VariableName)))
            {
                writer
                     .Arrow() // Write the -> arrow.
                     .Token(IsCapture ? "CAP" : "VAR") // Write CAP or VAR depending on IsCapture.
                     .Literal(VariableName); // Write the Variable Name as a literal.
            }
            return writer.ToString();
        }

        static byte[] HmacSHA256(String data, byte[] key)
        {
            String algorithm = "HmacSHA256";
            KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        static byte[] getSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes(("AWS4" + key).ToCharArray());
            byte[] kDate = HmacSHA256(dateStamp, kSecret);
            byte[] kRegion = HmacSHA256(regionName, kDate);
            byte[] kService = HmacSHA256(serviceName, kRegion);
            byte[] kSigning = HmacSHA256("aws4_request", kService);

            var result = kSigning;
            return kSigning;
        }

        public override void Process(BotData data)
        {
            // Magick Stuff goin on
            try
            {
                var result = Encoding.UTF8.GetString(getSignatureKey(AccessKey, AWSDateStamp, RegionName, AWSService));
                InsertVariable(data, IsCapture, result, VariableName, "", "", false, false);
                data.Status = RuriLib.BotStatus.SUCCESS;
                data.Log($"Generated AWS Token with result {result}");
            }
            catch (Exception ex)
            {
                data.Status = RuriLib.BotStatus.ERROR;
                data.Log($"Error Generating AWS tokens Ex:{ex}");
            }
        }
    }
}