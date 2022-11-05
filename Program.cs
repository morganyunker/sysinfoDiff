using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SysinfoDiff
{
    internal class PostlilionComponent
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int[] Patches { get; set; }
        public int GetMaxPatch()    // not currently used
        {
            if (Patches.Length > 0)
            {
                return Patches.Max();
            }
            else
            {
                return 0;
            }
        }
    }
    internal class Program
    {
        static Dictionary<string, List<int>> ComponentUniquePatches = new Dictionary<string, List<int>>();
        static void Main(string[] args)
        {
#if DEBUG
            args = new[] { @"C:\\Development\\SysinfoDiff\\Sysinfos" };
#endif
            string[] inputlines;
            List<string> inputList;
            List<string> lineElements;
            char[] whitespace = new char[] { ' ', '\t' };
            int componentIndex = 3;
            string currentComponent = "";
            string currentVersion = "";

            List<PostlilionComponent> components = new List<PostlilionComponent>();

            string[] sysinfoFiles = { };
            string outputPath = "";
            if (args.Length > 0)
            {
                Console.WriteLine("Looking for sysinfo files in: {0}", args[0]);
                outputPath = args[0];
                sysinfoFiles = System.IO.Directory.GetDirectories(@args[0]);
                foreach (var myDir in sysinfoFiles)
                {
                    Console.WriteLine(myDir);
                }
            }

            foreach (string Path in sysinfoFiles)
            {
                string usePath = System.IO.Path.Combine(Path, "Systeminfo.txt");
                string usePathParent = new DirectoryInfo(usePath).Parent.Name;
                currentComponent = "";
                currentVersion = "";
                List<int> currentPatches = new List<int>();
                inputlines = null;
                inputList = null;
                inputlines = File.ReadAllLines(usePath);
                inputList = inputlines.ToList();

                foreach (var myLine in inputList)
                {
                    if (myLine.Contains("Realtime Software"))
                    {
                        lineElements = myLine.Split(whitespace, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int i = 0; i <= lineElements.Count - 1; i++)
                        {
                            if (Convert.ToInt32(lineElements[5]) >= componentIndex) // Reached Realtime Software
                            {
                                if (lineElements[i] == "Name")
                                {
                                    if (lineElements[i + 1] != currentComponent)
                                    {
                                        if (currentComponent != "")
                                        {
                                            components.Add(new PostlilionComponent() { Name = currentComponent, Version = currentVersion, Path = usePathParent, Patches = currentPatches.ToArray() });
                                        }
                                        currentComponent = lineElements[i + 1];
                                        currentPatches.Clear();
                                    }
                                }
                                if (lineElements[i] == "Version")
                                {
                                    currentVersion = lineElements[i + 1];
                                }
                                if (lineElements[i] == "Patch")
                                {
                                    currentPatches.Add(int.Parse(lineElements[i + 1]));
                                    if (!ComponentUniquePatches.ContainsKey(currentComponent))
                                    {
                                        ComponentUniquePatches.Add(currentComponent, new List<int>());
                                    }
                                    ComponentUniquePatches[currentComponent].Add(int.Parse(lineElements[i + 1]));
                                }
                            }
                        }
                    }
                }
            }

            foreach (var tmp in ComponentUniquePatches.ToList())  // convert the list of "all patches" to a unique list
            {
                ComponentUniquePatches[tmp.Key] = tmp.Value.Distinct().ToList();
            }

            // Pretty Print the table here (to HTML)
            StringBuilder HTML = new StringBuilder();
            HTML.Append("<head>\r\n <style type=\"text/css\">\r\n .auto-style1 {\r\n width: 100%;\r\n border-style: solid;\r\n border-width: 2px;\r\n }\r\n table, th, td {\r\n border: 1px solid black;\r\n border-collapse: collapse;\r\n}\r\n </style>\r\n</head>");
            HTML.Append("<table class=\"auto-style1\">\r\n");
            HTML.Append("<tr>\r\n <th style=\"text-align:center\"><b>Component</b></th>\r\n");

            List<string> sysinfos = new List<string>();
            string tmpSysInfo = "";

            foreach (var comp in components)
            {
                if (comp.Path != tmpSysInfo)
                {
                    tmpSysInfo = comp.Path;
                    HTML.Append($"<th style=\"text-align:center\"><b>{comp.Path}</b></th>\r\n");
                    sysinfos.Add(comp.Path);
                }
            }
            HTML.Append("</tr>\r\n<tr>");

            foreach (var comp in ComponentUniquePatches.Keys)
            {
                HTML.Append($"<td><b>{comp}</b></td>");
                foreach (var sysinfo in sysinfos)                  // add foreach here to display array of versions
                {
                    foreach (var tmpComp in components)
                    {
                        if (tmpComp.Path == sysinfo && tmpComp.Name == comp)
                        {
                            HTML.Append(value: $"<td>{string.Join(", ", tmpComp.Patches)}");
                            string strDiffV2 = ShowDiffs(comp, tmpComp.Patches);
                            if (strDiffV2.Length != 0)
                            {
                                HTML.Append($"<span style='color:red;font-weight:bold;'><br>Missing Patch(es): {strDiffV2}</span></td>");
                            }
                            else
                            {
                                HTML.Append("</td>");
                            }
                        }
                    }
                }
                HTML.Append("</tr><tr>");
            }
            string outPath = System.IO.Path.Combine(outputPath, "result.html");
            File.WriteAllText(outPath, HTML.ToString());
        }
        public static string ShowDiffs(string comp, int[] Patches)
        {
            if (ComponentUniquePatches.ContainsKey(comp))
            {
                var strDiff = ComponentUniquePatches[comp].ToArray().Except(Patches).ToArray();
                return string.Join(",", strDiff);
            }
            else
            {
                return "";
            }
        }
    }
}