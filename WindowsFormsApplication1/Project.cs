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

                        temp = "\\b" + shortFileName + ".o" + "\\b";

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

                                string functionReturnType = functionInitialization.Substring(0, functionIndex);
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
                                Functions.Add(new Function(functionName, FunctionFilePath.Replace(Path, ""), funcParameters, functionReturnType));
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
                SVNVersion = client.GetInfo(SvnPathTarget.FromString(Path), out info) ? info.Revision : 0;
            }
        }

        public void GenerateReport(string ReportPath)
        {
            FileStream file1 = new FileStream(ReportPath, FileMode.Create);
            StreamWriter writer = new StreamWriter(file1);

            writer.Write(String.Format("Project Path: {0}\r\nTortoiseSVN Version: {1}\r\n", Path, SVNVersion.ToString()));

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


        public void CopyFunctionsToSeparetedFiles(string ReportFolder)
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
                int ElementsNumber = 1;
                byte ReturnFlag = 0;
                List<int> IfLineNumber = new List<int>();
                IfLineNumber.Add(0);

                // 1. create file "function Name.c"
                List<string> FunctionLines = new List<string>();

                NewFileName = string.Concat(string.Concat(string.Concat(FunctionReportPath, "\\"), func.Name), ".c");
                lines = System.IO.File.ReadAllLines(Path + func.SourcePath);
                foreach (string line in lines)
                {
                    if ((line.Contains(func.ReturnValue + " " + func.Name)) || (startFunction == true))
                    {
                        string tmp;
                        string copy;

                        if (line.Contains("//"))
                            tmp = line.Remove(line.LastIndexOf("//"));
                        else
                            tmp = line;
                    
                        if ((tmp.Contains("/*")) && (tmp.Contains("*/")))
                        {
                            int Index1 = 0, Index2 = 0;
                            Index1 = tmp.IndexOf('/');
                            Index2 = tmp.IndexOf('*');

                            if (Index1+1 == Index2)
                                tmp = tmp.Remove(Index1);
                        }
                            

                        if (line.Contains("{"))
                            OpenBrace++;
                        if (line.Contains("}"))
                            CloseBrace++;
                        if ((OpenBrace > 0) && (CloseBrace > 0) && (OpenBrace == CloseBrace))
                        {
                            if (!string.IsNullOrWhiteSpace(tmp))
                            {
                                /// add checking
                                copy = NumerateLineOfFunction(tmp, func.Name, func.ReturnValue, ElementsNumber, IfLineNumber[IfLineNumber.Count()-1]);
                                if (!tmp.Equals(copy))
                                {
                                    if ((copy.Contains(Constants.MARK_START)) || (copy.Contains(Constants.MARK_END)))
                                    {
                                        if (copy.Contains(Constants.MARK_END))
                                        {
                                            ReturnFlag = 1;
                                        }
                                        tmp = copy;
                                    }
                                    else if ((copy.Contains(Constants.C_IF)) && (!copy.Contains(Constants.C_ELSE_IF)))
                                    {
                                        IfLineNumber.Add(ElementsNumber);
                                        tmp = copy;
                                    }
                                    else if ((copy.Contains(Constants.C_ELSE)) && (!copy.Contains(Constants.C_ELSE_IF)))
                                    {
                                        if (IfLineNumber.Count() > 0)
                                            IfLineNumber.RemoveAt(IfLineNumber.Count()-1);
                                        tmp = copy;
                                    }
                                    else if (copy.Contains(Constants.C_ELSE_IF))
                                    {
                                        tmp = copy;
                                    }
                                    else
                                    {
                                        ElementsNumber++;
                                        tmp = copy;
                                    }
                                }
                                FunctionLines.Add(tmp);
                                if (ReturnFlag == 0)
                                    FunctionLines.Add(Constants.MARK_END + "\r\n");
                            }
                            startFunction = false;
                            OpenBrace = CloseBrace = 0;
                            break;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(tmp))
                            {
                                /// add checking
                                ///                             
                                copy = NumerateLineOfFunction(tmp, func.Name, func.ReturnValue, ElementsNumber, IfLineNumber[IfLineNumber.Count()-1]);
                                if (!tmp.Equals(copy))
                                {
                                    if ((copy.Contains(Constants.MARK_START)) || (copy.Contains(Constants.MARK_END)))
                                    {
                                        if (copy.Contains(Constants.MARK_END))
                                        {
                                            ReturnFlag = 1;
                                        }
                                        tmp = copy;
                                    }
                                    else if ((copy.Contains(Constants.C_IF))&& (!copy.Contains(Constants.C_ELSE_IF)))
                                    {
                                        IfLineNumber.Add(ElementsNumber);
                                        ElementsNumber++;
                                        tmp = copy;
                                    }
                                    else if ((copy.Contains(Constants.C_ELSE)) && (!copy.Contains(Constants.C_ELSE_IF)))
                                    {
                                        if (IfLineNumber.Count() > 0)
                                            IfLineNumber.RemoveAt(IfLineNumber.Count() - 1);
                                        tmp = copy;
                                    }
                                    else if (copy.Contains(Constants.C_ELSE_IF))
                                    {
                                        tmp = copy;
                                    }
                                    else
                                    {
                                        ElementsNumber++;
                                        tmp = copy;
                                    }
                                }
                                FunctionLines.Add(tmp);
                            }
                            startFunction = true;
                        }
                    }
                }
                System.IO.File.WriteAllLines(NewFileName, FunctionLines);
                func.NumberOfElemntsInGraph = ElementsNumber + 1;
                func.SeparetedFunctionPath = NewFileName;
            }

        }

        public string NumerateLineOfFunction(string StringToCheck, string FunctName, string FunctRetType, int LineNum, int LastIfNum)
        {
            string Returned;

            if ((StringToCheck.Contains(FunctName)) && (StringToCheck.Contains(FunctRetType)))
            {
                Returned = string.Concat(Constants.MARK_START, StringToCheck);
            }
            else if ((!StringToCheck.Contains(Constants.C_BREAK))   && 
                     (!StringToCheck.Contains(Constants.C_DEFAULT)) &&
                     (!StringToCheck.Contains(Constants.C_CASE)))
            {
                    if (StringToCheck.Contains(Constants.C_RETURN))
                    {
                        Returned = string.Concat(Constants.MARK_END, StringToCheck);
                    }
                    else if ((StringToCheck.Contains(Constants.C_FOR))   ||
                             (StringToCheck.Contains(Constants.C_WHILE)) ||
                             (StringToCheck.Contains(Constants.C_SWITCH)))
                    {
                        Returned = string.Concat("/*" + LineNum + "*/", StringToCheck);
                    }
                    else if ((StringToCheck.Contains(Constants.C_IF)) &&
                            (!StringToCheck.Contains(Constants.C_ELSE)))
                    {
                        Returned = string.Concat("/*" + LineNum + "*/", StringToCheck);
                    }
                    else if ((StringToCheck.Contains(Constants.C_ELSE)) ||
                             (StringToCheck.Contains(Constants.C_ELSE_IF)))
                    {
                        Returned = string.Concat("/*" + LastIfNum + "*/", StringToCheck);
                    }

                    else if (!StringToCheck.Contains(";"))
                    {
                        Returned = StringToCheck;
                    }
                    else
                    {
                        Returned = string.Concat("/*" + LineNum + "*/", StringToCheck);
                    }
            }
            else
            {
                Returned = StringToCheck;
            }
            return Returned;
        }

        public int GetNumberOfEdgesFormFunction(string FunctionPath, string FunctionName)
        {
            string[] lines;
            int EdgesNumber = 0;

            lines = System.IO.File.ReadAllLines(FunctionPath);
            for (int tmp = 0; tmp <lines.Count(); tmp++)
            {
                if ((lines[tmp].Contains(FunctionName)) && (lines[tmp].Contains(Constants.MARK_START)))
                {
                    EdgesNumber++;
                }
                else if ((lines[tmp].Contains(Constants.C_IF)) && (!lines[tmp].Contains(Constants.C_ELSE_IF)) && (lines[tmp].Contains("/*")))
                {
                    byte ElsePres = 0;
                    int OpenB = 0;
                    int CloseB = 0;
                    
                    /// find else
                    for (int local = tmp; local < lines.Count(); local++)
                    {
                        if (lines[local].Contains("{"))
                            OpenB++;
                        if (lines[local].Contains("}"))
                            CloseB++;
                        if ((OpenB == CloseB) && (lines[local].Contains(Constants.C_ELSE)) &&
                               (!lines[local].Contains(Constants.C_ELSE_IF)))
                        {
                            ElsePres = 1;
                        }
                    }
                    if (ElsePres == 1)
                        EdgesNumber++;
                    else
                        EdgesNumber = EdgesNumber + 2;
                }
                else if ((lines[tmp].Contains(Constants.C_ELSE_IF)) || (lines[tmp].Contains(Constants.C_ELSE)))
                {
                    EdgesNumber++;
                }
                else if (((lines[tmp].Contains(Constants.C_FOR)) || (lines[tmp].Contains(Constants.C_WHILE)) && (lines[tmp].Contains("/*"))))
                {
                    EdgesNumber = EdgesNumber + 2;
                }
                else if (lines[tmp].Contains(Constants.C_SWITCH))
                {
                    
                }
                else if (lines[tmp].Contains(Constants.C_CASE))
                {
                    EdgesNumber++;
                }
                else if (lines[tmp].Contains(Constants.MARK_END))
                {
                    
                }
                else if (lines[tmp].Contains("/*"))
                {
                    EdgesNumber++;
                }

            }
            return EdgesNumber;
        }


        public void AddInfoInsideMatrix (string FunctionPath, string FunctionName, int[,] matrix, int ElemNum, int EdgesNum)
        {
            string[] lines;
           // int  CycleElemP = 0;
            int  CurrentElemNumber = 0;
            int  EdgesPointer = 0;
            byte elseIsPresent = 0;
            int OpenB = 0;
            int CloseB = 0;

            List<int> StartLoopOperator = new List<int>();
            List<int> AfterLoopOperator = new List<int>();

            List<int> ElemOfBrachOperator = new List<int>();
            List<int> LastElemOfBrachOperator = new List<int>();
            List<int> ElemAfterBranchOperator = new List<int>();

                       

            lines = System.IO.File.ReadAllLines(FunctionPath);
            for (int i =0; i< lines.Count(); i++)
            {
                CurrentElemNumber = GetElementNumber(lines[i], ElemNum);
                if (CurrentElemNumber != -1)
                {
                    if (LineIsNameOfFunction(lines[i], FunctionName) == 1)
                    {
                        matrix[CurrentElemNumber, EdgesPointer] = 1;
                    }
                    else if (LineIsSimpleOperation(lines[i]) == 1)
                    {
                        if ((StartLoopOperator.Count > 0) &&
                            (CurrentElemNumber == (AfterLoopOperator[AfterLoopOperator.Count - 1] - 1)))
                        {
                            matrix[CurrentElemNumber, EdgesPointer] = -1;
                            EdgesPointer++;
                            matrix[CurrentElemNumber, EdgesPointer] = 1;

                            matrix[StartLoopOperator[StartLoopOperator.Count - 1], EdgesPointer] = -1;
                            EdgesPointer++;
                            matrix[StartLoopOperator[StartLoopOperator.Count - 1], EdgesPointer] = 1;

                            if (StartLoopOperator.Count > 0)
                                StartLoopOperator.RemoveAt(StartLoopOperator.Count - 1);

                            if (AfterLoopOperator.Count > 0)
                                AfterLoopOperator.RemoveAt(AfterLoopOperator.Count - 1);

                        }
                        else if ((LastElemOfBrachOperator.Count > 0) &&
                                (CurrentElemNumber <= LastElemOfBrachOperator[LastElemOfBrachOperator.Count - 1]))
                        {
                            if (CurrentElemNumber < LastElemOfBrachOperator[LastElemOfBrachOperator.Count - 1])
                            {
                                matrix[CurrentElemNumber, EdgesPointer] = -1;
                                EdgesPointer++;
                                matrix[CurrentElemNumber, EdgesPointer] = 1;
                            }
                            else if (CurrentElemNumber == LastElemOfBrachOperator[LastElemOfBrachOperator.Count - 1])
                            {
                                if (elseIsPresent == 0)
                                {
                                    matrix[CurrentElemNumber, EdgesPointer] = -1;
                                    EdgesPointer++;
                                    matrix[CurrentElemNumber, EdgesPointer] = -1;
                                    matrix[ElemOfBrachOperator[ElemOfBrachOperator.Count - 1], EdgesPointer] = 1;
                                    EdgesPointer++;
                                    matrix[CurrentElemNumber, EdgesPointer] = 1;
                                }
                                else
                                {
                                    matrix[CurrentElemNumber, EdgesPointer] = -1;
                                    EdgesPointer++;
                                    matrix[CurrentElemNumber, EdgesPointer] = 1;
                                    matrix[ElemAfterBranchOperator[ElemAfterBranchOperator.Count - 1], EdgesPointer] = -1;
                                    EdgesPointer++;
                                    if (CurrentElemNumber == ElemAfterBranchOperator[ElemAfterBranchOperator.Count - 1])
                                        elseIsPresent--;
                                }
                                if (ElemAfterBranchOperator.Count > 0)
                                    ElemAfterBranchOperator.RemoveAt(ElemAfterBranchOperator.Count - 1);
                                if (LastElemOfBrachOperator.Count > 0)
                                    LastElemOfBrachOperator.RemoveAt(LastElemOfBrachOperator.Count - 1);
                                if (ElemOfBrachOperator.Count > 0)
                                    ElemOfBrachOperator.RemoveAt(ElemOfBrachOperator.Count - 1);
                            }
                        }
                        else
                        {
                            matrix[CurrentElemNumber, EdgesPointer] = -1;
                            if (EdgesPointer < EdgesNum)
                                EdgesPointer++;
                            matrix[CurrentElemNumber, EdgesPointer] = 1;

                        }
                    }
                    else if (LineIsCicleOperator(lines[i]) == 1)
                    {
                        int OpB = 0, ClB = 0, tmp = 0;
                        StartLoopOperator.Add(CurrentElemNumber);
                        //// search out state
                        if ((!lines[i].Contains("{")) && (!lines[i + 1].Contains("{")))/// issue
                        {
                            for (int localS = i + 2; localS < lines.Count(); localS++)
                            {
                                tmp = GetElementNumber(lines[localS], ElemNum);
                                if (tmp != -1)
                                    AfterLoopOperator.Add(tmp);
                            }
                        }
                        else
                        {
                            for (int localS = i; localS < lines.Count(); localS++)
                            {
                                tmp = GetElementNumber(lines[localS], ElemNum);
                                if (lines[localS].Contains("{"))
                                    OpB++;
                                if (lines[localS].Contains("}"))
                                    ClB++;

                                if ((OpB > 0) && (ClB > 0) && (OpB == ClB) && (tmp != -1))
                                {
                                    AfterLoopOperator.Add(tmp);
                                }
                            }
                        }
                        matrix[CurrentElemNumber, EdgesPointer] = -1;
                        EdgesPointer++;
                        matrix[CurrentElemNumber, EdgesPointer] = 1;
                    }
                    else if (LineIsBranchOperator(lines[i]) == 1)
                    {
                        ElemOfBrachOperator.Add(CurrentElemNumber);
                        if ((lines[i].Contains(Constants.C_IF)) &&
                            (!lines[i].Contains(Constants.C_ELSE_IF)) &&
                            (!lines[i].Contains(Constants.C_ELSE)))
                        {
                            matrix[CurrentElemNumber, EdgesPointer] = -1; /// set 1,-1 here
                            EdgesPointer++;
                            matrix[CurrentElemNumber, EdgesPointer] = 1;
                            int OpB = 0, ClB = 0;
                            for (int localS = i; localS < lines.Count(); localS++)
                            {
                                if (lines[localS].Contains("{"))
                                    OpB++;
                                if (lines[localS].Contains("}"))
                                    ClB++;

                                if ((OpB == ClB) && (lines[localS].Contains(Constants.C_ELSE)) &&
                                    (!lines[localS].Contains(Constants.C_ELSE_IF)))
                                {
                                    elseIsPresent++;
                                }
                            }
                        }
                        else
                        {
                            matrix[ElemOfBrachOperator[ElemOfBrachOperator.Count - 1], EdgesPointer] = 1;
                            OpenB = CloseB = 0;
                            //// set 1
                        }
                        //// find out element: local out elem and global out elem
                        for (int local = i; local < lines.Count(); local++)
                        {
                            int TMP;
                            if (lines[local].Contains("{"))
                                OpenB++;
                            if (lines[local].Contains("}"))
                                CloseB++;

                            if (GetElementNumber(lines[local], ElemNum) != -1)
                            {
                                if (((lines[local + 1].Contains("}")) && (OpenB == CloseB + 1)) ||
                                    ((OpenB == 0) && (GetElementNumber(lines[local + 1], ElemNum) != -1)))
                                {
                                    if (ElemOfBrachOperator.Count > LastElemOfBrachOperator.Count)
                                    {
                                        LastElemOfBrachOperator.Add(GetElementNumber(lines[local], ElemNum));
                                        for (int littleL = local + 1; littleL < lines.Count(); littleL++)
                                        {
                                            TMP = GetElementNumber(lines[littleL], ElemNum);
                                            if (lines[littleL].Contains("{"))
                                                OpenB++;
                                            if (lines[littleL].Contains("}"))
                                                CloseB++;

                                            if ((TMP != -1) && ((CloseB > OpenB) || (OpenB == CloseB)) && (TMP != ElemOfBrachOperator[ElemOfBrachOperator.Count - 1]))
                                                ElemAfterBranchOperator.Add(TMP);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (LineIsEndOfFunction(lines[i]) == 1)
                    {
                        matrix[ElemNum - 1, EdgesNum - 1] = -1;
                    }
                }
            }
        }


        public int GetElementNumber(string Line, int ElemNum)
        {
            string TMP;
            int NumVal = 0;
          

            if (Line.Contains(Constants.MARK_START))
                return 0;
            if (Line.Contains(Constants.MARK_END))
                return ElemNum - 1;
            if (Line.Contains("/*"))
            {
                if (Line.Contains("*/"))
                {
                    Line = Line.Remove(Line.LastIndexOf("*/"));
                    TMP = Regex.Replace(Line, "[^0-9]+", string.Empty);
                    //NumVal = Convert.ToInt32(TMP.Remove(0, 2));
                    NumVal = Convert.ToInt32(TMP);
                    if (NumVal > 0)
                        return NumVal;
                    else
                        return -1;
                }
                else
                    return -1;

            }    
            return -1;
        }


        public byte LineIsNameOfFunction (string Line, string FuncName)
        {
            if ((Line.Contains(FuncName)) && (Line.Contains(Constants.MARK_START)))
                return 1;
            else
                return 0;
        }
        public byte LineIsSimpleOperation(string Line)
        {
            if ((Line.Contains("/*")) && 
                (!Line.Contains(Constants.C_IF)) &&
                 (!Line.Contains(Constants.C_ELSE)) &&
                (!Line.Contains(Constants.C_FOR)) &&
                (!Line.Contains(Constants.C_SWITCH)) && 
                (!Line.Contains(Constants.C_WHILE)) && 
                (!Line.Contains(Constants.MARK_END)))
                return 1; 
            else
                return 0;
        }

        public byte LineIsCicleOperator(string Line)
        {
            if ((Line.Contains("/*")) && ((Line.Contains(Constants.C_WHILE)) || (Line.Contains(Constants.C_FOR))))
                return 1;
            else
                return 0;
        }

        public byte LineIsBranchOperator (string Line)
        {
            if ((Line.Contains(Constants.C_IF)) ||
                (Line.Contains(Constants.C_ELSE)) ||
                (Line.Contains(Constants.C_ELSE_IF)))
                return 1;
            else
                return 0;
        }

        public byte LineIsEndOfFunction(string Line)
        {
            if (Line.Contains(Constants.MARK_END))
                return 1;
            else
                return 0;
        }

        public void AddInforInFuctionFiles(string FilePath, int NumOfElem, int NumOfEdges)
        {
            string Tmp = "Number of Elements = " + NumOfElem +", " + "Number of Edges = " + NumOfEdges;

            System.IO.File.AppendAllText(FilePath, Tmp);

        }
        //public void Write
        public void AddMatrixInFuctionFiles(string FilePath, int[,] matrix, int ElemNum, int EdgesNum)
        {

            System.IO.File.AppendAllText(FilePath, " Matrix\r\n");
            for (int Elem = 0; Elem < ElemNum; Elem++)
            {
                string Tmp = "{";
                for ( int Edges = 0; Edges< EdgesNum;  Edges++)
                {
                    if (matrix[Elem, Edges] != -1)
                        Tmp = string.Concat((Tmp+" "), matrix[Elem, Edges]);
                    else
                        Tmp = string.Concat(Tmp, matrix[Elem, Edges]);
                    if (Edges < EdgesNum - 1)
                    {
                        Tmp = string.Concat(Tmp, ", ");
                    }
                    else
                    {
                        Tmp = string.Concat(Tmp, "}\r\n");
                    }
                }
                System.IO.File.AppendAllText(FilePath, Tmp);
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
