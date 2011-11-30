using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            //this sets the number of boxes for the histograms
            int boxNum1 = 6;
            int boxNum2 = 25;
            int boxDivider1 = 20 / boxNum1;
            int boxDivider2 = 360 / boxNum2;

            //this is just creating the test shapes
            int[,] test = new int[10, 10];
            int[,] test2 = new int[10, 10];
            int[,] test3 = new int[10, 10];
            int[,] test4 = new int[10, 10];
            int y;
            int x;
            
            for (y = 0; y < 10; y++)
            {
                for (x = 0; x < 10; x++)
                {
                    test[x, y] = 0;
                    test2[x, y] = 0;
                    test3[x, y] = 0;
                    test4[x, y] = 0;
                }
            }
            test[5, 2] = 1;
            test[5, 3] = 1;
            test[5, 4] = 1;
            test[4, 5] = 1;
            test[6, 5] = 1;
            test[3, 6] = 1;
            test[7, 6] = 1;
            test[2, 7] = 1;
            test[3, 7] = 1;
            test[4, 7] = 1;
            test[5, 7] = 1;
            test[6, 7] = 1;
            test[7, 7] = 1;
            test[1, 8] = 1;
            test[8, 8] = 1;
            test[0, 9] = 1;
            test[9, 9] = 1;
            test2[0, 9] = 1;
            test2[9, 9] = 1;
            test2[5, 3] = 1;
            test2[5, 4] = 1;
            test2[4, 5] = 1;
            test2[6, 5] = 1;
            test2[3, 6] = 1;
            test2[7, 6] = 1;
            test2[2, 7] = 1;
            test2[3, 7] = 1;
            test2[4, 7] = 1;
            test2[5, 7] = 1;
            test2[6, 7] = 1;
            test2[7, 7] = 1;
            test2[1, 8] = 1;
            test2[9, 8] = 1;
            test3[5, 2] = 1;
            test3[5, 3] = 1;
            test3[5, 4] = 1;
            test3[4, 5] = 1;
            test3[6, 5] = 1;
            test3[3, 6] = 1;
            test3[7, 6] = 1;
            test3[2, 7] = 1;
            test3[7, 7] = 1;
            test3[1, 8] = 1;
            test3[8, 8] = 1;

            test4[0, 1] = 1;
            test4[1, 2] = 1;
            test4[2, 3] = 1;
            test4[3, 4] = 1;
            test4[4, 5] = 1;
            test4[5, 6] = 1;
            test4[6, 7] = 1;
            test4[9, 2] = 1;
            test4[9, 3] = 1;
            test4[8, 4] = 1;
            test4[8, 5] = 1;
            test4[7, 5] = 1;
            test4[6, 5] = 1;
            test4[6, 6] = 1;
            test4[4, 6] = 1;
            test4[3, 8] = 1;
            test4[2, 9] = 1;
            test4[4, 9] = 1;

            //this calls to make each of the histograms
            int[,] hist1 =createHistogram(test, 10, 10,boxNum1,boxNum2);
            int[,] hist2 = createHistogram(test2, 10, 10,boxNum1,boxNum2);
            int[,] hist3 = createHistogram(test3, 10, 10, boxNum1, boxNum2);
            int[,] hist4 = createHistogram(test4, 10, 10, boxNum1, boxNum2);
            

            int cost1=0;
            int cost2 = 0;
            int cost3 = 0;


            for (x = 0; x < 20 / boxDivider1; x++)
            {
                for (y = 0; y < 360 / boxDivider2; y++)
                {
                    //I had the problem if the points were the same they would divide by zero
                    //but if the points are the same you would not add any to the cost so leaving an
                    //empty catch clause worked
                    try
                    {
                        if (hist1[x, y] - hist3[x, y] != 0)
                        {
                            cost2 = cost2 + (int)((Math.Pow((hist1[x, y] - hist3[x, y]), 2)) / (hist1[x, y] + hist3[x, y]));
                        }
                    }
                    catch (DivideByZeroException e) { }
                    try
                    {
                    if (hist1[x, y] - hist2[x, y] != 0)
                    {
                        cost1 = cost1 + (int)((Math.Pow((hist1[x, y] - hist2[x, y]), 2)) / (hist1[x, y] + hist2[x, y]));
                    }
                    }
                    catch (DivideByZeroException e) { }
                    try
                    {
                    if (hist1[x, y] - hist4[x, y] != 0)
                    {
                        cost3 = cost3 + (int)((Math.Pow((hist1[x, y] - hist4[x, y]), 2)) / (hist1[x, y] + hist4[x, y]));
                    }
                    }
                    catch (DivideByZeroException e) { }
                }
            }

            cost1 = cost1 / 2;
            cost2 = cost2 / 2;
            cost3 = cost3 / 2;

            Console.WriteLine("The cost of matching shape 1 with shape 2 is: {0}",cost1);
            Console.WriteLine("The cost of matching shape 1 with shape 3 is: {0}", cost2);
            Console.WriteLine("The cost of matching shape 1 with shape 4 is: {0}", cost3);
            
        }

        public static int[,] createHistogram(int[,] shape, int width, int height,int boxNum1,int boxNum2)
        {
            int boxDivider1 = (width + height) / boxNum1;
            int boxDivider2 = 360 / boxNum2;
            int[,] hist = new int[width + height / boxDivider1, 360 / boxDivider2];
            int[] currentPoint=new int[2];
            currentPoint[0] = 0;
            int counter = 0;

            //this finds the initial point to look at
            //this will be changed so that it looks at all of the points
            while (currentPoint[0] == 0)
            {
                if(shape[counter,height/2]==1)
                {
                    currentPoint[0]=counter;
                    currentPoint[1]=height/2;
                }
                else
                    counter++;
            }

            int x;
            int y;
            int distance;
            int angle;
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    if (shape[x, y] == 1)
                    {
                        if (x != currentPoint[0] && y != currentPoint[1])
                        {
                            distance = (int)Math.Sqrt(Math.Pow(x - currentPoint[0], 2) + Math.Pow(y - currentPoint[1], 2));

                            //I accidently went backwards with the angles but as long as they are the same it is ok
                            //0 degrees is right
                            //90 degrees is down
                            //180 degrees is left
                            //270 degrees is up
                            if (currentPoint[0] < x && currentPoint[1] >= y)
                            {
                                angle = (int)(Math.Atan2(currentPoint[1] - y, x - currentPoint[0]) * (180 / Math.PI));
                            }
                            else if (currentPoint[0] >= x && currentPoint[1] > y)
                            {
                                angle = 90 + (int)(Math.Atan2(currentPoint[0] - x, currentPoint[1] - y) * (180 / Math.PI));
                            }
                            else if (currentPoint[0] > x && currentPoint[1] <= y)
                            {
                                angle = 180 + (int)(Math.Atan2(y - currentPoint[1], currentPoint[0] - x) * (180 / Math.PI));
                            }
                            else
                            {
                                angle = 270 + (int)(Math.Atan2(x - currentPoint[0], y - currentPoint[1]) * (180 / Math.PI));
                            }

                            hist[distance / boxDivider1, angle / boxDivider2]++;
                        }
                    }
                }
            }
            return hist;
        }
    }
}
