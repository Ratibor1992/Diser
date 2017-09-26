using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(FuncParameter))]
    public class Function
    {
        public Function()
        {
            Parameters = new List<FuncParameter>();
        }

        public Function(string name)
        {
            Name = name;
            Parameters = new List<FuncParameter>();
        }

        public Function(string name, string sourcePath, List<FuncParameter> funcParameters, string returnValue, int startAddress, int endAddress)
        {
            Name = name;
            SourcePath = sourcePath;
            Parameters = funcParameters;
            IsTested = false;
            ReturnValue = returnValue;
            StartAddress = startAddress;
            EndAddress = endAddress;
        }


        public string Name { get; set; }
        public List<FuncParameter> Parameters { get; set; }
        public string ReturnValue { get; set; }
        public bool IsTested { get; set; }
        public string SourcePath { get; set; }

        public string SeparetedFunctionPath { get; set; }
        public string FunctionCraphPath { get; set; }

        public int NumberOfElemntsInGraph { get; set; }

        public int NumberOfEdgesInGraph { get; set; }

        public int StartAddress { get; set; }
        public int EndAddress { get; set; }

        public int [,] matrix { get; set; }
    }

    [Serializable]
    public class FuncParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public FuncParameter()
        {

        }
        public FuncParameter(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

}
