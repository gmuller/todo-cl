using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace Todo
{
    class Options
    {
        [Option("u", "UserId", Required = false, HelpText = "UserId")]
        public string username = null;

        [Option("p", "password", HelpText = "password")]
        public string password = null;

        [Option("t", "todo", HelpText = "todo")]
        public string todo = null;

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Todo Command Line");
            usage.AppendLine("use -u or -p to enter username and password");
            usage.AppendLine("or just enter a todo");
            return usage.ToString();
        }
    }
}
