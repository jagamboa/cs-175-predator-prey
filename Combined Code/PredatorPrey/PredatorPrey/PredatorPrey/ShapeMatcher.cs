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
        public const int boxNum1 = 1;
        public const int boxNum2 = 10;
        public const double histThreshold = 3;
        List<int[,]> initialHistograms;
        List<Classification> types;
        List<Vector2> directions;
        Color purple;
        public ShapeMatcher(List<Texture2D> initialTextures)
        {

            types = new List<Classification>();
            types.Add(Classification.Predator);
            types.Add(Classification.Prey);
            purple = new Color(255, 0, 255);
            directions = new List<Vector2>();
            directions.Add(new Vector2(0));
            directions.Add(new Vector2((float)Math.PI/4));
            directions.Add(new Vector2((float)Math.PI / 2));
            directions.Add(new Vector2((float)(3 * Math.PI) / 4));
            directions.Add(new Vector2((float)Math.PI));
            directions.Add(new Vector2((float)(5 * Math.PI) / 4));
            directions.Add(new Vector2((float)(3*Math.PI) / 2));
            directions.Add(new Vector2((float)(7 * Math.PI) / 4));
            initialHistograms = new List<int[,]>();
            for(int i=0; i<initialTextures.Count;i++)
            {
                int width = initialTextures[i].Width;
                int height = initialTextures[i].Height;
                int max = Math.Max(width, height);
                int temp = width*height;
                Color[] list = new Color[temp];
                initialTextures[i].GetData<Color>(list,0,temp);
                int[,] tempImage = new int[max, max];
                Color black1 = new Color(1, 1, 1);
                if (width == max)
                {
                    for (int j=0; j < list.Length; j++)
                    {
                        if (list[j].Equals(black1))
                            tempImage[j % width, j / width+((max-height)/2)] = 1;
                    }
                }
                else
                {
                    for (int j=0; j < list.Length; j++)
                    {
                        if (list[j].Equals(black1))
                            tempImage[j % width+((max-width)/2), j / width] = 1;
                    }
                }
                initialHistograms.Add(createHistogram(tempImage,max,max));
                

                /*int[,] rotatedImage = new int[max,max];
                double angle;
                int newX;
                int newY;
                for (angle = 0; angle < (2 * Math.PI); angle +=Math.PI / 4)
                {
                    for (int x = 0; x < max; x++)
                    {
                        for (int y = 0; y < max; y++)
                        {
                            newX = (int)(x * Math.Cos(angle) - y * Math.Sign(angle));
                            newY = (int)(x * Math.Sin(angle) + y * Math.Cos(angle));
                            if(newX<max && newY<max)
                                rotatedImage[newX,newY] = tempImage[x, y];
                        }
                    }
                    initialHistograms.Add(createHistogram(rotatedImage,max,max));
                    directions.Add(new Vector2((float)angle));
                }*/
            }
            
        }

        public VisionContainer findObjects(Creature creat, Color[] visionArea,int width, int height)
        {
            VisionContainer container = new VisionContainer();
            int count = 0;
            int countStop=0;
            Vector2 previousDirection = Vector2.Zero;
            int leftMostPoint = int.MaxValue;
            int rightMostPoint = 0;
            int topMostPoint = int.MaxValue;
            int bottomMostPoint = 0;
            bool inObject = false;
            while (count < visionArea.Length)
            {
                if (inObject)
                {
                    if (count == countStop)
                    {
                        visionArea[count] = purple;
                        if(width!=0 && height !=0)
                            container.add(detect(creat, visionArea, topMostPoint,bottomMostPoint,leftMostPoint,rightMostPoint,width,height));
                        count= countStop+1;
                        inObject = false;
                        if (count == visionArea.Length)
                            break;
                    }
                        /*
                    else if(visionArea[count].Equals(Color.Red))
                    {
                        if(width!=0&&height!=0)
                            container.add(detect(creat, visionArea, topMostPoint, bottomMostPoint, leftMostPoint, rightMostPoint,width, height));
                        count = countStop + 1;
                        inObject = false;
                        if (count == visionArea.Length)
                            break;
                    }*/
                }

                if (visionArea[count].R + visionArea[count].G + visionArea[count].B >= 1 &&
                    visionArea[count].R + visionArea[count].G + visionArea[count].B <= 350)
                {
                    if (inObject)
                    {
                        if (count % width > rightMostPoint%width)
                            rightMostPoint = count;
                        else if (count % width < leftMostPoint%width)
                            leftMostPoint = count;
                        if (count / width > bottomMostPoint/width)
                            bottomMostPoint = count;
                        else if (count / width < topMostPoint/width)
                            topMostPoint = count;
                        visionArea[count] = purple;
                        count = findNext(count, previousDirection, visionArea, width, countStop);
                        
                    }
                    else
                    {
                            
                            countStop = count;
                            inObject = true;
                            leftMostPoint = count;
                            rightMostPoint = count;
                            topMostPoint = count;
                            bottomMostPoint = count;
                            count = findNext(count, previousDirection, visionArea, width, countStop);

                    }
                }
                else
                    count++;
            }
            return container;
        }

        private int findNext(int count, Vector2 previousDirection, Color[] visionArea, int width, int countStop)
        {
            try
            {
                //previousDirection.X != 1 && 
                if (visionArea[count + 1].R + visionArea[count + 1].G + visionArea[count + 1].B >= 1 &&
                    visionArea[count + 1].R + visionArea[count + 1].G + visionArea[count + 1].B <= 350)
                {
                    return count+1;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.Y == -1 &&
                 if (visionArea[count - width].R + visionArea[count - width].G + visionArea[count - width].B >= 1 &&
                     visionArea[count - width].R + visionArea[count - width].G + visionArea[count - width].B <= 350)
                {
                    return count - width;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != 1 && previousDirection.Y == -1) 
                if (visionArea[count - width + 1].R + visionArea[count - width + 1].G + visionArea[count - width + 1].B >= 1 &&
                    visionArea[count - width + 1].R + visionArea[count - width + 1].G + visionArea[count - width + 1].B <= 350)
                {
                    return count - width + 1;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.Y == 1 && 
                if (visionArea[count + width].R + visionArea[count + width].G + visionArea[count + width].B >= 1 &&
                    visionArea[count + width].R + visionArea[count + width].G + visionArea[count + width].B <= 350)
                {
                    return count + width;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != 1 && previousDirection.Y == 1) && 
            if (visionArea[count + width + 1].R + visionArea[count + width + 1].G + visionArea[count + width + 1].B >= 1 &&
                visionArea[count + width + 1].R + visionArea[count + width + 1].G + visionArea[count + width + 1].B <= 350)
                {
                    return count + width + 1;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != -1 && previousDirection.Y == -1) && 
                if (visionArea[count - width - 1].R + visionArea[count - width - 1].G + visionArea[count - width - 1].B >= 1 &&
                    visionArea[count - width - 1].R + visionArea[count - width - 1].G + visionArea[count - width - 1].B <= 350)
                {
                    return count - width - 1;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != -1 && previousDirection.Y == 1) && 
                if (visionArea[count + width - 1].R + visionArea[count + width - 1].G + visionArea[count + width - 1].B >= 1 &&
                    visionArea[count + width - 1].R + visionArea[count + width - 1].G + visionArea[count + width - 1].B <= 350)
                {
                    return count + width - 1;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.X != -1 && 
                if (visionArea[count - 1].R + visionArea[count - 1].G + visionArea[count - 1].B >= 1 &&
                    visionArea[count - 1].R + visionArea[count - 1].G + visionArea[count - 1].B <= 350)
                {
                    return count-1;
                }
            }catch(IndexOutOfRangeException e){}
            try
            {
                if (visionArea[count + width].G - visionArea[count + width].B > 200)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        if (count + i * width < visionArea.Count() && visionArea[count + i * width].A + visionArea[count + i * width].G + visionArea[count + i * width].B == 6)
                        {
                            return count + i * width;
                        }
                    }
                    return countStop;
                }
            }
            catch (IndexOutOfRangeException e) { }
                return countStop;
        }

        private ObjectSeen detect(Creature creat, Color[] visionArea2, int top, int bottom, int left, int right, int imageWidth, int imageHeight)
        {
            double best = int.MaxValue;
            ObjectSeen bestObject = null;
            double tempHist;

            int width = right%imageWidth-left%imageWidth+1;
            int height = bottom/imageWidth-top/imageWidth +1;
            int max = Math.Max(width, height);
            int[,] tempImage = new int[max,max];
            int start = ((top / imageWidth) * imageWidth + left % imageWidth);
            if (width == max)
            {
                for (int y = 0; y < max; y++)
                {
                    for (int j = start; j < start + width; j++)
                    {
                        if (visionArea2[j].Equals(purple))
                            tempImage[j - start, y + ((max - height) / 2)] = 1;
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int j = start; j < start +width; j++)
                    {
                        if (visionArea2[j+(y*imageWidth)].Equals(purple))
                            tempImage[j - start +((max - width) / 2), y] = 1;
                    }
                }
            }
            
            if (max != 0)
            {
                tempImage = createHistogram(tempImage, max, max);
                for (int i = 0; i < initialHistograms.Count; i++)
                {
                    tempHist = compareHistograms(tempImage, initialHistograms[i]);
                    if (tempHist < best)
                    {
                        best = tempHist;
                        bestObject = new ObjectSeen(types[i / 8], new Vector2(Math.Abs(creat.getPosition().X - (left + width / 2)), Math.Abs(creat.getPosition().Y - (top + height / 2))), directions[i % 8]);
                    }
                }
                if (best > histThreshold)
                    bestObject.type = Classification.Unknown;
            }

            return bestObject;
        }

        private int[,] createHistogram(int[,] shape, int width, int height)
        {
            int boxDivider1 = (width + height) / boxNum1;
            int boxDivider2 = 360 / boxNum2;
            int[,] hist = new int[width + height / boxDivider1, 360 / boxDivider2];
            int[] currentPoint = new int[2];
            currentPoint[0] = width/2;
            currentPoint[1] = height / 2;
            //int counter = 0;

            //this finds the initial point to look at
            //this will be changed so that it looks at all of the points
            /*while (currentPoint[0] == 0)
            {
                if (shape[counter, height / 2] == 1)
                {
                    currentPoint[0] = counter;
                    currentPoint[1] = height / 2;
                }
                else
                    counter++;
            }*/

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
