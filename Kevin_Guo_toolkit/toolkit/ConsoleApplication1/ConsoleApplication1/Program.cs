using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            KnowledgeBase kb = new KnowledgeBase();
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "/unlabeled.txt");
            foreach (string line in lines)
            {
                int temp = Convert.ToInt32(line);
                string result = kb.searchKnowledgeBase(temp);
                kb.insertToKnowledgeBase(line + ", " + result);
            }
        }
    }
}
