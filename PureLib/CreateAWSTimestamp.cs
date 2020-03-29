using DotNetApiGatewayIam;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;
using System;
using System.Net.Http;
using System.Net;

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

        [Text("Secret AccessKey", "AWS Secret key / Token")]
        public string SecretAccessKey { get; set; } = "";

        [Text("API Endpoint", "API Endpoint")]
        public string AWSUrl { get; set; } = "";

        [Text("AWS Endpoint Stages", "API Endpoint Stages")]
        public string AWSStages { get; set; } = "";

        [Text("RegionName", "API Region")]
        public string RegionName { get; set; } = "";

        [Text("JsonData", "Json Post data")]
        public string JsonData { get; set; } = "";

        [Text("SessionToken", "Session Token")]
        public string SessionToken { get; set; } = "";

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
                SecretAccessKey = LineParser.ParseLiteral(ref input, "SecretAccessKey");
                AWSUrl = LineParser.ParseLiteral(ref input, "AWSURL");
                RegionName = LineParser.ParseLiteral(ref input, "AWS Region Name");
                JsonData = LineParser.ParseLiteral(ref input, "AWS Json Data");
                AWSStages = LineParser.ParseLiteral(ref input, "AWS API Stages");
                SessionToken = LineParser.ParseLiteral(ref input, "AWS Session token");
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
                .Literal(SecretAccessKey)
                .Literal(AWSUrl)
                .Literal(RegionName)
                .Literal(JsonData)
                .Literal(AWSStages)
                .Literal(SessionToken);
            if (!writer.CheckDefault(VariableName, nameof(VariableName)))
            {
                writer
                     .Arrow() // Write the -> arrow.
                     .Token(IsCapture ? "CAP" : "VAR") // Write CAP or VAR depending on IsCapture.
                     .Literal(VariableName); // Write the Variable Name as a literal.
            }
            return writer.ToString();
        }


        public override async void Process(BotData data)
        {
            // Magick Stuff goin on
            try
            {
                var request = new DotNetApiGatewayIam.AwsApiGatewayRequest()
                {
                    RegionName = RegionName,
                    Host = AWSUrl,
                    AccessKey = AccessKey,
                    SecretKey = SecretAccessKey,
                    AbsolutePath = AWSStages,
                    JsonData = JsonData,
                    SessionToken = SessionToken,
                    RequestMethod = HttpMethod.Post
                };
                var apiRequest = new ApiRequest(request);
                var response = await apiRequest.GetResponseStringAsync();
                var result = response.ToString();
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