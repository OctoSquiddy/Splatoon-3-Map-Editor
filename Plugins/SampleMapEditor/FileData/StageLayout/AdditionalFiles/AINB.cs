using System;
using System.Collections.Generic;
using Wheatley.io;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Toolbox.Core;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace SampleMapEditor.Ainb
{
    public class AINB
    {
        // Get the base path relative to the executable location
        private static string GetBasePath()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = Path.GetDirectoryName(exePath);
            return exeDir;
        }

        private static string PathToInput => Path.Combine(GetBasePath(), "Lib", "AINB", "in");
        private static string PathToOutput => Path.Combine(GetBasePath(), "Lib", "AINB", "out");
        private static string PathToAINBDir => Path.Combine(GetBasePath(), "Lib", "AINB");

        [JsonProperty]
        public LogicInfo Info {  get; set; }

        [JsonProperty("Commands")]
        public List<LogicCommand> Commands { get; set; }

        [JsonProperty("Nodes")]
        public List<LogicNode> Nodes { get; set; }

        [JsonProperty("File Hashes")]
        public LogicFileHashes FileHashes { get; set; }

        public AINB()
        {
            
        }

        public static AINB LoadFromAINBData(List<byte> data)
        {
            string jsonData = ainb2json(data);

            return JsonConvert.DeserializeObject<AINB>(jsonData);
        }

        public string getJSONData()
        {
            // Ensure Commands is never null (Python requires it)
            if (Commands == null)
                Commands = new List<LogicCommand>();
            if (Nodes == null)
                Nodes = new List<LogicNode>();

            string jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);

            // Remove empty arrays EXCEPT "Commands" and "Nodes" which are required by Python
            // Note: Lookahead (?!Commands|Nodes) checks the property name, not "Commands"" with extra quote
            var removeEmptyArray = new Regex("\\s*\"(?!Commands|Nodes)[^\"]+\":\\s*\\[\\]\\,?");
            var removeEmptyClass = new Regex("\"[^\"]*\":\\s*\\{\\s*\\},?");
            var removeComma = new Regex(",(?=\\s*})");

            jsonData = removeEmptyArray.Replace(jsonData, "");
            jsonData = removeComma.Replace(jsonData, "");

            jsonData = removeEmptyClass.Replace(jsonData, "");
            jsonData = removeComma.Replace(jsonData, "");

            return jsonData;
        }

        public static string ainb2json(List<byte> ainbData)
        {
            // Ensure the AINB directory exists
            if (!Directory.Exists(PathToAINBDir))
            {
                throw new DirectoryNotFoundException($"AINB library not found at: {PathToAINBDir}");
            }

            File.WriteAllBytes(PathToInput, ainbData.ToArray());

            string scriptPath = Path.Combine(PathToAINBDir, "ainb2json.py");
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"AINB conversion script not found: {scriptPath}");
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "python";
            process.StartInfo.WorkingDirectory = PathToAINBDir;
            process.StartInfo.Arguments = $"\"{scriptPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start Python. Make sure Python is installed and in PATH. Error: {ex.Message}");
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"AINB conversion error (exit code {process.ExitCode}): {error}");
                Console.WriteLine($"Output: {output}");
            }

            if (!File.Exists(PathToOutput))
            {
                string errorMsg = $"AINB conversion failed. Output file not found: {PathToOutput}\n";
                errorMsg += $"Exit code: {process.ExitCode}\n";
                if (!string.IsNullOrEmpty(error)) errorMsg += $"Error: {error}\n";
                if (!string.IsNullOrEmpty(output)) errorMsg += $"Output: {output}";
                throw new Exception(errorMsg);
            }

            string res = File.ReadAllText(PathToOutput);
            ClearFiles();

            return res;
        }

        public static List<byte> json2ainb(string jsonData)
        {
            // Ensure the AINB directory exists
            if (!Directory.Exists(PathToAINBDir))
            {
                throw new DirectoryNotFoundException($"AINB library not found at: {PathToAINBDir}");
            }

            File.WriteAllText(PathToInput, jsonData);

            string scriptPath = Path.Combine(PathToAINBDir, "json2ainb.py");
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"AINB conversion script not found: {scriptPath}");
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "python";
            process.StartInfo.WorkingDirectory = PathToAINBDir;
            process.StartInfo.Arguments = $"\"{scriptPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start Python. Make sure Python is installed and in PATH. Error: {ex.Message}");
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"AINB conversion error (exit code {process.ExitCode}): {error}");
                Console.WriteLine($"Output: {output}");
            }

            if (!File.Exists(PathToOutput))
            {
                string errorMsg = $"AINB conversion failed. Output file not found: {PathToOutput}\n";
                errorMsg += $"Exit code: {process.ExitCode}\n";
                if (!string.IsNullOrEmpty(error)) errorMsg += $"Error: {error}\n";
                if (!string.IsNullOrEmpty(output)) errorMsg += $"Output: {output}";
                throw new Exception(errorMsg);
            }

            List<byte> res = File.ReadAllBytes(PathToOutput).ToList();
            ClearFiles();

            return res;
        }

        private static void ClearFiles()
        {
            if (File.Exists(PathToInput))
            {
                File.Delete(PathToInput);
            }

            if (File.Exists(PathToOutput))
            {
                File.Delete(PathToOutput);
            }
        }

        public class LogicInfo
        {
            [JsonProperty("Magic")]
            public string Magic { get; set; }

            [JsonProperty("Version")]
            public string Version { get; set; }

            [JsonProperty("Filename")]
            public string Filename { get; set; }

            [JsonProperty("File Category")]
            public string FileCategory { get; set; }

            public LogicInfo()
            {
                Magic = "";
                Version = "";
                Filename = "";
                FileCategory = "";
            }
        }

        public class LogicNode
        {
            [JsonProperty("Node Type")]
            public string NodeType { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Flags")]
            public List<string> Flags { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("GUID")]
            public string GUID { get; set; }

            [JsonProperty("Precondition Nodes")]
            public List<int> PreconditionNodes { get; set; }

            [JsonProperty("Internal Parameters")]
            public InternalParameter InternalParameters { get; set; }

            [JsonProperty("Input Parameters")]
            public InputParameters InputParameters { get; set; }

            [JsonProperty("Output Parameters")]
            public OutputParameters OutputParameters { get; set; }

            [JsonProperty("Linked Nodes")]
            public LinkedNodes LinkedNodes { get; set; }

            public LogicNode()
            {
                NodeType = "";
                NodeIndex = 0;
                Flags = new List<string>();
                Name = "";
                GUID = "";
                PreconditionNodes = new List<int>();
                InternalParameters = new InternalParameter();
                InputParameters = new InputParameters();
                OutputParameters = new OutputParameters();
                LinkedNodes = new LinkedNodes();
            }
        }

        public class LogicFileHashes
        {
            [JsonProperty("Unknown File Hash")]
            public string UnknownFileHash { get; set; }

            public LogicFileHashes()
            {
                UnknownFileHash = "";
            }
        }

        /// <summary>
        /// Command entry - connects AI Group references to nodes.
        /// The Name field contains the AI Group reference (e.g., "ItemCardKey_06cf").
        /// </summary>
        public class LogicCommand
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("GUID")]
            public string GUID { get; set; }

            [JsonProperty("Left Node Index")]
            public int LeftNodeIndex { get; set; }

            [JsonProperty("Right Node Index")]
            public int RightNodeIndex { get; set; }

            public LogicCommand()
            {
                Name = "";
                GUID = "";
                LeftNodeIndex = -1;
                RightNodeIndex = -1;
            }
        }

        public class InternalStringParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public string Value { get; set; }

            public InternalStringParameter()
            {
                Name = "";
                Value = "";
            }
        }

        public class InternalBoolParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public bool Value { get; set; }

            public InternalBoolParameter()
            {
                Name = "";
                Value = false;
            }
        }

        public class InternalIntParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public int Value { get; set; }

            public InternalIntParameter()
            {
                Name = "";
                Value = 0;
            }
        }

        public class InternalFloatParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public float Value { get; set; }

            public InternalFloatParameter()
            {
                Name = "";
                Value = 0.0f;
            }
        }

        public class InternalParameter
        {
            [JsonProperty("int")]
            public List<InternalIntParameter> Int { get; set; }

            [JsonProperty("bool")]
            public List<InternalBoolParameter> Bool { get; set; }

            [JsonProperty("float")]
            public List<InternalFloatParameter> Float { get; set; }

            [JsonProperty("string")]
            public List<InternalStringParameter> String { get; set; }

            public InternalParameter()
            {
                Bool = new List<InternalBoolParameter>();
                String = new List<InternalStringParameter>();
                Int = new List<InternalIntParameter>();
                Float = new List<InternalFloatParameter>();
            }
        }
        
        public class UserDefinedParameterSource
        {
            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            public UserDefinedParameterSource()
            {
                NodeIndex = 0;
                ParameterIndex = 0;
            }
        }

        public class UserDefinedParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Class")]
            public string Class { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            [JsonProperty("Value")]
            public int Value { get; set; }

            [JsonProperty("Sources")]
            public List<UserDefinedParameterSource> Sources { get; set; }

            public UserDefinedParameter()
            {
                Name = "";
                Class = "";
                NodeIndex = 0;
                ParameterIndex = 0;
                Value = 0;
                Sources = new List<UserDefinedParameterSource>();
            }
        }

        public class InputIntParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            [JsonProperty("Value")]
            public int Value { get; set; }

            public InputIntParameter()
            {
                Name = "";
                NodeIndex = 0;
                ParameterIndex = 0;
                Value = 0;
            }
        }

        public class InputBoolParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            [JsonProperty("Value")]
            public bool Value { get; set; }

            public InputBoolParameter()
            {
                Name = "";
                NodeIndex = 0;
                ParameterIndex = 0;
                Value = true;
            }
        }

        public class InputFloatParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            [JsonProperty("Value")]
            public float Value { get; set; }

            public InputFloatParameter()
            {
                Name = "";
                NodeIndex = 0;
                ParameterIndex = 0;
                Value = 0.0f;
            }
        }

        public class InputStringParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            [JsonProperty("Value")]
            public string Value { get; set; }

            public InputStringParameter()
            {
                Name = "";
                NodeIndex = 0;
                ParameterIndex = 0;
                Value = "";
            }
        }

        public class InputParameterSource
        {
            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter Index")]
            public int ParameterIndex { get; set; }

            public InputParameterSource()
            {
                NodeIndex = 0;
                ParameterIndex = 0;
            }
        }

        public class InputParameters
        {
            [JsonProperty("int")]
            public List<InputIntParameter> Int { get; set; }

            [JsonProperty("bool")]
            public List<InputBoolParameter> Bool { get; set; }

            [JsonProperty("float")]
            public List<InputFloatParameter> Float { get; set; }

            [JsonProperty("string")]
            public List<InputStringParameter> String { get; set; }

            [JsonProperty("userdefined")]
            public List<UserDefinedParameter> UserDefined { get; set; }

            [JsonProperty("Sources")]
            public List<InputParameterSource> Sources { get; set; }

            public InputParameters()
            {
                Int = new List<InputIntParameter>();
                Bool = new List<InputBoolParameter>();
                Float = new List<InputFloatParameter>();
                String = new List<InputStringParameter>();
                UserDefined = new List<UserDefinedParameter>();
                Sources = new List<InputParameterSource>();
            }
        }

        public class OutputIntParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            public OutputIntParameter()
            {
                Name = "";
            }
        }

        public class OutputBoolParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            public OutputBoolParameter()
            {
                Name = "";
            }
        }

        public class OutputFloatParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            public OutputFloatParameter()
            {
                Name = "";
            }
        }

        public class OutputStringParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            public OutputStringParameter()
            {
                Name = "";
            }
        }

        public class OutputUserDefinedParameter
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Class")]
            public string Class { get; set; }

            public OutputUserDefinedParameter()
            {
                Name = "";
                Class = "";
            }
        }

        public class OutputParameters
        {
            [JsonProperty("int")]
            public List<OutputIntParameter> Int { get; set; }

            [JsonProperty("bool")]
            public List<OutputBoolParameter> Bool { get; set; }

            [JsonProperty("float")]
            public List<OutputFloatParameter> Float { get; set; }

            [JsonProperty("string")]
            public List<OutputStringParameter> String { get; set; }

            [JsonProperty("userdefined")]
            public List<OutputUserDefinedParameter> UserDefined { get; set; }

            public OutputParameters()
            {
                Int = new List<OutputIntParameter>();
                Bool = new List<OutputBoolParameter>();
                Float = new List<OutputFloatParameter>();
                String = new List<OutputStringParameter>();
                UserDefined = new List<OutputUserDefinedParameter>();
            }
        }

        public class LinkedNode
        {
            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter")]
            public string Parameter { get; set; }

            public LinkedNode()
            {
                NodeIndex = 0;
                Parameter = "";
            }
        }

        public class IntLinkedNode
        {
            [JsonProperty("Node Index")]
            public int NodeIndex { get; set; }

            [JsonProperty("Parameter")]
            public string Parameter { get; set; }

            public IntLinkedNode()
            {
                NodeIndex = 0;
                Parameter = "";
            }
        }

        public class LinkedNodes
        {
            [JsonProperty("Bool/Float Input Link and Output Link")]
            public List<LinkedNode> BoolFloatInputLinkAndOutputLink { get; set; }

            [JsonProperty("Int Input Link")]
            public List<IntLinkedNode> IntInputLink { get; set; }

            public LinkedNodes()
            {
                BoolFloatInputLinkAndOutputLink = new List<LinkedNode>();
                IntInputLink = new List<IntLinkedNode>();
            }
        }
    }
}
