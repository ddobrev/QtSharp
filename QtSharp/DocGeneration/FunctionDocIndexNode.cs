using System.Collections.Generic;

namespace QtSharp.DocGeneration
{
    public class FunctionDocIndexNode : FullNameDocIndexNode
    {
        public FunctionDocIndexNode()
        {
            this.ParametersModifiers = new List<string>();
        }

        public string Access { get; set; }
        public List<string> ParametersModifiers { get; private set; }
    }
}
