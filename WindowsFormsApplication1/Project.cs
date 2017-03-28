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
                        string Tmp, lower;

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
                            string firstWord = line.Trim();
                            firstWord.Substring(0, firstWord.IndexOf(" "));
                            Functions.Add(new Function(firstWord.Substring(0, firstWord.IndexOf(" "))));
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
                writer.Write("\r\n");
                writer.Write(func.Name);
            }
            writer.Write("\r\n");
            writer.Close();
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
