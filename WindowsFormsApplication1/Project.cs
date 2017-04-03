using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SharpSvn;

namespace WindowsFormsApplication1
{
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(Function))]
    public class Project
    {
        public string Path { get; set; }
        public long SVNVersion { get; set; }
        public List<string> FilesList { get; set; }
        public List<Function> Functions { get; set; }

        public Project()
        {
            FilesList = new List<string>();
            Functions = new List<Function>();
        }

        public Project(string path)
        {
            Path = path;
            FilesList = new List<string>();
            Functions = new List<Function>();
        }

        public void ParseProject()
        {
            GetUserFiles();
            GetFunctionList();
            GetSVNRevision();
        }
        private void GetUserFiles()
        {
            List<string> folders = new List<string>(Directory.EnumerateDirectories(Path));

            foreach (string line in folders)
            {
                if ((line.Contains(Constants.SVN)) ||
                    (line.Contains(Constants.BINOUTPUT)) ||
                    (line.Contains(Constants.Objects)) ||
                    (line.Contains(Constants.Listings)) ||
                    (line.Contains(Constants.RTE)))
                {

                }
                else
                {
                    string Tmp1;
                 
                    Tmp1 = line.Replace(Path, "");
                    Tmp1 = Tmp1.Replace(@"\", "");

                    string[] dirs = Directory.GetFiles(line, "*.c");    
                    foreach (string dir in dirs)
                    {
                        string Tmp;

                        Tmp = dir.Replace(line, "");
                        Tmp = Tmp.Replace(@"\", "");

                        FilesList.Add(Tmp);
                    }
                }
            }
        }

        private void GetFunctionList()
        {
            string MapFilePath;
            string[] lines;

            var allFiles = Directory.GetFiles(Path, "*.map", SearchOption.AllDirectories);
            MapFilePath = allFiles[0];

            lines = System.IO.File.ReadAllLines(MapFilePath);

            foreach (string line in lines)
            {
                if (line.Contains("Thumb Code")) //&& (line.Contains("(i.")))
                {
                    string temp;
                    string shortFileName;

                    foreach (string filesname in FilesList)
                    {
                        shortFileName = filesname.Remove(filesname.Length - 2).ToLower();
                        // ads1118.o(i.
                        temp = "\\b" + shortFileName + ".o" + "\\b";
                        // temp = "(i." + filesname;
                        if (Regex.IsMatch(line, temp, RegexOptions.IgnoreCase))
                        {
                            string FunctionFilePath = Directory.GetFiles(Path, filesname, SearchOption.AllDirectories).First();

                            using (StreamReader sr = new StreamReader(FunctionFilePath))
                            {
                                string firstWord = line.Trim();
                                firstWord.Substring(0, firstWord.IndexOf(" "));
                                string functionName = firstWord.Substring(0, firstWord.IndexOf(" "));

                                string fileContents = sr.ReadToEnd();

                                Regex rx = new Regex(String.Format("[a-zA-Z0-9]+ {0}\\s*[(]", functionName));

                                Match match = rx.Match(fileContents);
                                int functionIndex = match.Index;
                                fileContents = fileContents.Substring(functionIndex);
                                functionIndex = fileContents.IndexOf('{');
                                fileContents = fileContents.Substring(0, functionIndex);
                                functionIndex = fileContents.LastIndexOf(')');
                                // Getting function initialization with format : *return type* *function name* (*parameters*) 
                                string functionInitialization = fileContents.Substring(0, functionIndex);
                                functionIndex = functionInitialization.IndexOf(" ");

                                string functioReturnType = functionInitialization.Substring(0, functionIndex);
                                functionInitialization = functionInitialization.Substring(functionIndex);

                                functionIndex = functionInitialization.IndexOf("(");
                                functionInitialization = functionInitialization.Substring(functionIndex + 1);

                                string[] parameters = functionInitialization.Split(',');
                                List<FuncParameter> funcParameters = new List<FuncParameter>();
                                foreach (string parameterPair in parameters)
                                {
                                    string[] parametersTokens = parameterPair.Split(' ');
                                    if (parametersTokens.Length == 1)
                                        funcParameters.Add(new FuncParameter() { Name = "", Type = "void" });
                                    else
                                    {
                                        string parameterType = parametersTokens[0];
                                        for (var i = 1; i < parametersTokens.Length - 1; i++)
                                        {
                                            parameterType += " " + parametersTokens[i];
                                        }
                                        funcParameters.Add(new FuncParameter() { Name = parametersTokens[parametersTokens.Length - 1], Type = parameterType });
                                    }
                                }
                                Functions.Add(new Function(functionName, FunctionFilePath.Replace(Path, ""), funcParameters));
                            }
                        }
                    }
                }
            }
        }
        private void GetSVNRevision()
        {
            using (SvnClient client = new SvnClient())
            {
                SvnInfoEventArgs info;
                SVNVersion =  client.GetInfo(SvnPathTarget.FromString(Path), out info) ? info.Revision : 0;
            }
        }
            
        public void GenerateReport(string ReportPath)
        {
            FileStream file1 = new FileStream(ReportPath, FileMode.Create);
            StreamWriter writer = new StreamWriter(file1);

            writer.Write(String.Format("Project Path: {0}\r\nTortoiseSVN Version: {1}\r\n", Path,SVNVersion.ToString()));

            writer.Write("***************************User Files List*********************************");
            foreach (var file in FilesList)
            {
                writer.Write("\r\n");
                writer.Write(file);
            }
            writer.Write("\r\n");
            writer.Write("***************************Functions List*********************************");
            foreach (var func in Functions)
            {
                writer.Write(String.Format("\r\n Name: {0} \r\n Parameters:", func.Name));
                foreach (var param in func.Parameters)
                {
                    writer.Write(String.Format("\r\n{0} {1}", param.Type, param.Name));
                }
            }
            writer.Write("\r\n");
            writer.Close();
        }


        public void CopyFunctionsToSeparetedFiles (string ReportFolder)
        {
            /// 1. create separeted folder
            /// 2. create file .c file with name of function
            /// 3. copy function to file
            /// 4. close file
            string FunctionReportPath;
            string NewFileName;
            byte OpenBrace = 0;
            byte CloseBrace = 0;
            bool startFunction = false;
            string[] lines;
           

            FunctionReportPath = string.Concat(ReportFolder, "\\Functions");
            Directory.CreateDirectory(FunctionReportPath);


            foreach (var func in Functions)
            {
                // 1. create file "function Name.c"
                 List<string> FunctionLines = new List<string>(); 
                NewFileName = string.Concat(string.Concat(string.Concat(FunctionReportPath, "\\"), func.Name), ".c");
                //
                ////FileStream NewFile = new FileStream(NewFileName, FileMode.Create);
                ////StreamWriter writer = new StreamWriter(NewFile);
                // func.SourcePath

                lines = System.IO.File.ReadAllLines(Path + func.SourcePath);
                foreach (string line in lines)
                {
                    if ((!line.Contains(":") && (line.Contains(func.Name))) || (startFunction == true))
                    {
                        ////add cycle for copying function
                        if (line.Contains("{"))
                            OpenBrace++;
                        if (line.Contains("}"))
                            CloseBrace++;
                        if ((OpenBrace > 0) && (CloseBrace > 0) && (OpenBrace == CloseBrace))
                        {
                            FunctionLines.Add(line);
                            startFunction = false;
                            OpenBrace = CloseBrace = 0;
                            break;
                        }
                        else
                        {
                            FunctionLines.Add(line);
                            startFunction = true;
                        }
                    }
                }
                System.IO.File.WriteAllLines(NewFileName, FunctionLines);
            }

        }

    }

    static class ProjectXMLSerializer
    {
        public static void SaveProjectBD(Project projObj, string bdPath)
        {
            XmlSerializer serializer =
                    new XmlSerializer(typeof(Project));
            TextWriter writer = new StreamWriter(bdPath);

            serializer.Serialize(writer, projObj);
            writer.Close();
        }

        public static Project ReadProjectDB(string bdPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Project));
            TextReader reader = new StreamReader(bdPath);
            object obj = deserializer.Deserialize(reader);
            return (Project)obj;
        }
    }
}
