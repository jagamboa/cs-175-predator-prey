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

namespace PredatorPrey
{
    class ShapeMatcher
    {
        public const int boxNum1 = 10;
        public const int boxNum2 = 10;
        public const double histThreshold = .1;
        List<int[,]> initialHistograms;
        List<Classification> types;
        List<Vector> directions;
        public ShapeMatcher(Texture2D[] initialTextures,List<Classification> type, List<Vector> direction)
        {
            
            types = type;
            directions = direction;
            for(int i=0; i<initialTextures.Length;i++)
            {
                int width = initialTextures[i].Width;
                int height = initialTextures[i].Height;
                int max = Math.Max(width, height);
                int temp = width*height;
                Color[] list = new Color[temp];
                initialTextures[0].GetData<Color>(list,0,temp);
                int[,] tempImage = new int[max, max];
                if (width == max)
                {
                    for (int j=0; j < list.Length; j++)
                    {
                        if (list[j].Equals(Color.Black))
                            tempImage[j % width, j / width+((max-height)/2)] = 1;
                    }
                }
                else
                {
                    for (int j=0; j < list.Length; j++)
                    {
                        if (list[j].Equals(Color.Black))
                            tempImage[j % width+((max-width)/2), j / width] = 1;
                    }
                }
                initialHistograms.Add(createHistogram(tempImage,max,max));
            }
            
        }

        private VisionContainer findObjects(Creature creat, Color[] visionArea,int width, int height)
        {
            VisionContainer container = new VisionContainer();
            int count = 0;
            int countStop=0;
            Vector2 previousDirection = Vector2.Zero;
            int leftMostPoint = 100;
            int rightMostPoint = 0;
            int topMostPoint = 100;
            int bottomMostPoint = 0;
            bool inObject = false;
            while (count < visionArea.Length)
            {
                if (inObject)
                {
                    if (count == countStop)
                    {
                        visionArea[count] = Color.Red;
                        container.add(detect(creat, visionArea, topMostPoint,bottomMostPoint,leftMostPoint,rightMostPoint,width,height));
                        count= countStop+1;
                    }
                    else if(visionArea[count].Equals(Color.Red))
                    {
                        container.add(detect(creat, visionArea, topMostPoint, bottomMostPoint, leftMostPoint, rightMostPoint, width, height));
                        count = countStop + 1;
                    }
                }

                if (visionArea[count].Equals(Color.Black))
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

                        if (previousDirection.X != 1 && visionArea[count + 1].Equals(Color.Black))
                        {
                            count++;
                        }
                        else if (previousDirection.Y == -1 && visionArea[count - width].Equals(Color.Black))
                        {
                            count = count - width;
                        }
                        else if ((previousDirection.X != 1 && previousDirection.Y == -1) && visionArea[count - width + 1].Equals(Color.Black))
                        {
                            count = count - width + 1;
                        }
                        else if (previousDirection.Y == 1 && visionArea[count + width].Equals(Color.Black))
                        {
                            count = count + width;
                        }
                        else if ((previousDirection.X != 1 && previousDirection.Y == 1) && visionArea[count + width + 1].Equals(Color.Black))
                        {
                            count = count + width + 1;
                        }
                        else if ((previousDirection.X != -1 && previousDirection.Y == -1) && visionArea[count - width - 1].Equals(Color.Black))
                        {
                            count = count - width - 1;
                        }
                        else if ((previousDirection.X != -1 && previousDirection.Y == 1) && visionArea[count + width - 1].Equals(Color.Black))
                        {
                            count = count + width - 1;
                        }
                        else if (previousDirection.X != -1 && visionArea[count - 1].Equals(Color.Black))
                        {
                            count--;
                        }
                        visionArea[count] = Color.Red;
                    }
                    else
                    {
                        countStop = count;
                        inObject = true;
                        leftMostPoint = count;
                        rightMostPoint = count;
                        topMostPoint = count;
                        bottomMostPoint = count;
                    }
                }
                else
                    count++;
            }
            return container;
        }

        private ObjectSeen detect(Creature creat, Color[] visionArea2, int top, int bottom, int left, int right, int areaWidth, int areaHeight)
        {
            double best = int.MaxValue;
            ObjectSeen bestObject = null;
            double tempHist;
            int width = right-left;
            int height = bottom-top;
            int max = Math.Max(width, height);
            int[,] tempImage = new int[width,height];
            if (width == max)
            {
                for (int j=0; j < visionArea2.Length; j++)
                {
                    if (visionArea2[j].Equals(Color.Black))
                        tempImage[j % width, j / width + ((max - height) / 2)] = 1;
                }
            }
            else
            {
                for (int j=0; j < visionArea2.Length; j++)
                {
                    if (visionArea2[j].Equals(Color.Black))
                        tempImage[j % width + ((max - width) / 2), j / width] = 1;
                }
            }
            tempImage = createHistogram(tempImage, max, max);
            for (int i = 0; i < initialHistograms.Count; i++)
            {
                tempHist = compareHistograms(tempImage, initialHistograms[i]);
                if(tempHist<best)
                {
                    best = tempHist;
                    bestObject = new ObjectSeen(types[i / 8], new Vector(Math.Abs(creat.getPosition().X - (left + width / 2)), Math.Abs(creat.getPosition().Y - (top + height / 2))), directions[i % 8]);
                }
            }
            if (best > histThreshold)
                bestObject.type = Classification.Unknown;
            return bestObject;
        }

        private int[,] createHistogram(int[,] shape, int width, int height)
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

        private double compareHistograms(int[,] hist1, int[,] hist2)
        {
            int x;
            int y;
            double cost=0;
            for (x = 0; x <boxNum1; x++)
            {
                for (y = 0; y <boxNum2; y++)
                {
                    try
                    {
                        if (hist1[x, y] - hist2[x, y] != 0)
                        {
                            cost = cost + ((Math.Pow((hist1[x, y] - hist2[x, y]), 2)) / (hist1[x, y] + hist2[x, y]));
                        }
                    }
                    catch (DivideByZeroException e) { }
                }
            }
            return cost;
        }
    }
}
