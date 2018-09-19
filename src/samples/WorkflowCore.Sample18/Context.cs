using System.Collections.Generic;

namespace WorkflowCore.Sample18
{
    public class Context
    {
        public Dictionary<string, string> Vars { get; set; }

        public Context()
        {
            Vars = new Dictionary<string, string>() {{"var1", " "}};
        }
    }

    public class Val
    {
        public string Value { get; set; }
    }
}