using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class KnowledgeBase
    {
        private string startupPath;
        private List<int> timeList;
        private List<string> actionList;
        private int prevCount;
        private int prevTime;
        private int count;
        int current;

        public KnowledgeBase()
        {
            timeList = new List<int>();
            actionList = new List<string>();
            startupPath = System.IO.Directory.GetCurrentDirectory();
            string[] lines = System.IO.File.ReadAllLines(startupPath + "/labeled.txt");
            foreach (string line in lines)
            {
                string[] sp = line.Split(new char[] { ',' });
                timeList.Add(Convert.ToInt32(sp[0]));
                actionList.Add(sp[1]);
            }
        }
        ~KnowledgeBase()
        {

        }

        public void insertToKnowledgeBase(string newKnowledge)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(startupPath + "/result.txt", true))
            {
                file.WriteLine(newKnowledge);
            }
        }

        public string searchKnowledgeBase(int time)
        {
            count = 0;
            foreach (int i in timeList)
            {
                current = i;
                if (i == time)
                {
                    timeList.Insert(count, time);
                    actionList.Insert(count, actionList[count]);
                    return actionList[count];
                }
                else if (i < time)
                {
                    prevCount = count;
                    prevTime = i;
                }
                else
                {
                    if (actionList[prevCount].Equals(actionList[count], StringComparison.OrdinalIgnoreCase))
                    {
                        timeList.Insert(count, time);
                        actionList.Insert(count, actionList[count]);
                        return actionList[count];
                    }
                    else
                    {
                        int diff1 = time - prevTime;
                        int diff2 = i - time;
                        if (diff1 < diff2)
                        {
                            timeList.Insert(count, time);
                            actionList.Insert(count, actionList[prevCount]);
                            return actionList[prevCount];
                        }
                        else if (diff2 < diff1)
                        {
                            timeList.Insert(count, time);
                            actionList.Insert(count, actionList[count]);
                            return actionList[count];
                        }
                        else
                        {
                            Random r = new Random();
                            int x = r.Next(0, 2);
                            if (x == 0)
                            {
                                timeList.Insert(count, time);
                                actionList.Insert(count, actionList[count]);
                                return "stop";
                            }
                            else
                            {
                                timeList.Insert(count, time);
                                actionList.Insert(count, actionList[count]);
                                return "go";
                            }
                        }
                    }
                }
                count++;
            }
            timeList.Insert(count, time);
            actionList.Insert(count, "go");
            return "go";
        }
    }
}
