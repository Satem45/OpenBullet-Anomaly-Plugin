using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenBulletPlugin
{
    public class Challenge : BlockBase, IBlockPlugin
    {
        public string Name => "OAuthChallenge";
        public string Color => "Blue";
        public bool LightForeground => false;
        [Text("Variable Name", "The output variable name")]
        public string VariableName { get; set; } = "";

        [Text("Verifier", "The Result from Verifier Gen")]
        public string VerInput { get; set; } = "";

        [Checkbox("Is Capture", "Should the output variable be marked as capture?")]
        public bool IsCapture { get; set; } = false;
        public Challenge()
        {
            Label = Name;
        }
        public override BlockBase FromLS(string line)
        {
            var input = line.Trim();
            if (input.StartsWith("#")) // If the input actually has a label
                Label = LineParser.ParseLabel(ref input); // Parse the label and remove it from the original string
            VerInput = LineParser.ParseLiteral(ref input, "First Number");

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
                .Literal(VerInput);
            if (!writer.CheckDefault(VariableName, nameof(VariableName)))
            {
                writer
                     .Arrow() // Write the -> arrow.
                     .Token(IsCapture ? "CAP" : "VAR") // Write CAP or VAR depending on IsCapture.
                     .Literal(VariableName); // Write the Variable Name as a literal.
            }
            return writer.ToString();
        }
        public override void Process(BotData data)
        {
            var Input = (ReplaceValues(VerInput, data));
            byte[] bytes = Encoding.UTF8.GetBytes(Input);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = Input;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(hashString);
            string encodedStr = System.Convert.ToBase64String(plainTextBytes);
            var result = encodedStr;
            InsertVariable(data, IsCapture, result, VariableName, "", "", false, false);
            data.Log($"Generated OAuth Challenge with result {result}");
        }
    }
}