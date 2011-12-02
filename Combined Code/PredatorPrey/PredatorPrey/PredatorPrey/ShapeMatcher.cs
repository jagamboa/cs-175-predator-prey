using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShapeMatching
{
    class ShapeMatcher
    {

        ArrayList<int[,]> initialHistograms;
        ArrayList<Classification> types;
        public ShapeMatcher(Texture2D[] initialTextures,ArrayList<Classifications> type)
        {
            types = type;
            for(int i; i<initialTextures.Length;i++)
            {
                int width = initialTextues[i].Width;
                int height = initialTextures[i].Height;
                int max = Math.Max(width, height);
                int temp = width*height;
                Color[] list = new Color[temp];
                initialTextures[0].GetData<Color>(list,0,temp);
                int[,] tempImage = newint[max, max];
                if (width == max)
                {
                    for (int j; j < list.Length; j++)
                    {
                        if (list[j].equals(Color.Black))
                            tempImage[j % width, j / width+((max-height)/2)] = 1;
                    }
                }
                else
                {
                    for (int j; j < list.Length; j++)
                    {
                        if (list[j].equals(Color.Black))
                            tempImage[j % width+((max-width)/2), j / width] = 1;
                    }
                }
                initialHistograms.add(createHistogram(tempImage,width,height));
            }
            
        }

        public VisualCollection look(Creature creat, Color[] visionArea)
        {

        }

        private ArrayList<int[,]> findObjects(Color[] list,int width, int height)
        {
            int count = 0;
            int countStop=0;
            int[,] thing;
            Vector previousDirection = Vector.Zero;
            int leftMostPoint = 100;
            int rightMostPoint = 0;
            int topMostPoint = 100;
            int bottomMostPoint = 0;
            bool inObject = false;
            while (count < list.Length)
            {

                if (list[count].Equals(Color.Black))
                {
                    if (inObject)
                    {
                        if (count % width > rightMostPoint)
                            rightMostPoint = count;
                        else if (count % width < leftMostPoint)
                            leftMostPoint = count;
                        if (count / width > bottomMostPoint)
                            bottomMostPoint = count;
                        else if (count / width < topMostPoint)
                            topMostPoint = count;

                        if (previousDirection.X != 1 && list[count + 1].Equals(Color.Black))
                            count++;
                        else if (previousDirection.Y == -1 && list[count - width].equals(Color.Black))
                            count = count - width;
                        else if ((previousDirection.X != 1 && previousDirection.Y == -1) && list[count - width + 1].equals(Color.Black))
                            count = count - width + 1;
                        else if (previousDirection.Y == 1 && list[count + width].equals(Color.Black))
                            count = count + Width;
                        else if ((previousDirection.X != 1 && previousDirection.Y == 1) && list[count + width + 1].equals(Color.Black))
                            count = count + width + 1;
                        else if ((previousDirection.X != -1 && previousDirection.Y == -1) && list[count - width - 1].equals(Color.Black))
                            count = count - width - 1;
                        else if ((previousDirection.X!=-1 && previousDirection.Y==1) && list[count + witdh - 1].equals(Color.Black))
                            count = count + width - 1;
                        else if (previousDirection.X!=-1 && list[count - 1].equals(Color.Black))
                            count--;
                    }
                    else
                    { 
                        countStop=count;
                        inObject=true;
                    }
                }
            }
        }

        private int[,] createHistogram(int[,] shape, int width, int height, int boxNum1, int boxNum2)
        {
            int boxDivider1 = (width + height) / boxNum1;
            int boxDivider2 = 360 / boxNum2;
            int[,] hist = new int[width + height / boxDivider1, 360 / boxDivider2];
            int[] currentPoint = new int[2];
            currentPoint[0] = 0;
            int counter = 0;

            //this finds the initial point to look at
            //this will be changed so that it looks at all of the points
            while (currentPoint[0] == 0)
            {
                if (shape[counter, height / 2] == 1)
                {
                    currentPoint[0] = counter;
                    currentPoint[1] = height / 2;
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
