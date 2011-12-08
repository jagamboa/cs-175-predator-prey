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

        List<int[,]> initialHistograms;
        List<Classification> types;
        List<Vector2> directions;
        Color purple;
        Color green1;
        Color green2;
        Color black1;
        public ShapeMatcher(List<Texture2D> initialTextures)
        {
            green1 = new Color(0, 128, 0);
            green2 = new Color(0, 190, 0);
            types = new List<Classification>();
            types.Add(Classification.Predator);
            types.Add(Classification.Prey);
            types.Add(Classification.Food);
            purple = new Color(99, 0, 99);
            /*directions = new List<Vector2>();
            directions.Add(new Vector2(0));
            directions.Add(new Vector2(1,-1));
            directions.Add(new Vector2(1,0));
            directions.Add(new Vector2(1,1));
            directions.Add(new Vector2(0,1));
            directions.Add(new Vector2(-1,1));
            directions.Add(new Vector2(-1,0));
            directions.Add(new Vector2(-1,-1));
             */
            black1 = new Color(1, 1, 1);
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
            }
            
        }

        public ObjectSeen compareCreats(Creature visableCreat, Texture2D texture)
        {
            int count = 0;
            int width = texture.Width;
            int height = texture.Height;
            int max = Math.Max(width, height);
            int temp = width*height;
            Color[] list = new Color[temp];
            texture.GetData<Color>(list,0,temp);
            int[,] tempImage = new int[max, max];
            if (width == max)
            {
                for (int j=0; j < list.Length; j++)
                {
                    if (list[j].Equals(black1))
                        tempImage[j % width, (j / width)+((max-height)/2)] = 1;
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
            tempImage = createHistogram(tempImage,max,max);
            double tempHist;
            double best = int.MaxValue;
            ObjectSeen bestObject = null;
            for (int i = 0; i < initialHistograms.Count; i++)
            {
                tempHist = compareHistograms(tempImage, initialHistograms[i]);
                if (tempHist < best)
                {
                    best = tempHist;
                    bestObject = new ObjectSeen(types[i], visableCreat.position, visableCreat.velocity);
                }
            }
            if (best > Parameters.histThreshold)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (list[i].Equals(green1) || list[i].Equals(green2))
                        count++;

                }
                if (count > 30)
                    bestObject = new ObjectSeen(Classification.Food, visableCreat.position, Vector2.Zero);
                else
                    bestObject.type = Classification.Unknown;
            }
            return bestObject;
        }
            
        

        //this is not usesed as of now, object detection runs too slow
        public VisionContainer findObjects(Creature creat, Color[] visionArea,int width, int height, int creatureX, int creatureY)
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
                            container.add(detect(visionArea, topMostPoint,bottomMostPoint,leftMostPoint,rightMostPoint,width,height,creatureX, creatureY));
                        count= countStop+1;
                        inObject = false;
                        if (count == visionArea.Length)
                            break;
                    }
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    count = findNext(count, previousDirection, visionArea, width, countStop);
                    
                }
                else if (visionArea[count].B > 100 || visionArea[count].Equals(green1)||visionArea[count].Equals(green2))
                {

                    if (visionArea[count].Equals(green1) || visionArea[count].Equals(green2))
                    {
                        
                        int[] temp = findGreen(count, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint,0);
                        int grassWidth = temp[3]%width-temp[2]%width+1;
                        int grassHeight = temp[1]/width-temp[0]/width +1;
                        if(temp[4] > 10)
                        {
                            container.add(new ObjectSeen(Classification.Food,new Vector2((temp[2] + grassWidth / 2), creat.getPosition().Y - (temp[0] + height / 2)),Vector2.Zero));
                        }
                    }
                    else
                    {
                        countStop = count;
                        inObject = true;
                        leftMostPoint = count;
                        rightMostPoint = count;
                        topMostPoint = count;
                        bottomMostPoint = count;
                        visionArea[count] = purple;
                        count = findNext(count, previousDirection, visionArea, width, countStop);
                    }
                    
                }
                else
                    count++;
            }
            return container;
        }


        //not used - part of object detection
        //finds and outpust the dimensions of a block of grass
        private int[] findGreen(int count,Color[] visionArea, int width,int leftMostPoint,int rightMostPoint,int topMostPoint,int bottomMostPoint,int number)
        {
            int num = number;
            int[] returnValue = new int[5];
            try
            {
                if (visionArea[count + 1].G.Equals(green1) || visionArea[count + 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count + 1] = purple;
                    num++;
                    returnValue = findGreen(count + 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint =returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];

                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                if (visionArea[count - width].G.Equals(green1) || visionArea[count - width].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count - width] = purple;
                    num++;
                    returnValue = findGreen(count - width, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                if (visionArea[count - width + 1].G.Equals(green1) || visionArea[count - width + 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count - width + 1] = purple;
                    num++;
                    returnValue = findGreen(count - width + 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                //previousDirection.Y == 1 && 
                if (visionArea[count + width].G.Equals(green1) || visionArea[count + width].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count + width] = purple;
                    num++;
                    returnValue = findGreen(count + width, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                //(previousDirection.X != 1 && previousDirection.Y == 1) && 
                if (visionArea[count + width + 1].G.Equals(green1) || visionArea[count + width + 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count + width + 1] = purple;
                    num++;
                    returnValue = findGreen(count + width + 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                //(previousDirection.X != -1 && previousDirection.Y == -1) && 
                if (visionArea[count - width - 1].G.Equals(green1) || visionArea[count - width - 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count - width - 1] = purple;
                    num++;
                    returnValue = findGreen(count - width - 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                //(previousDirection.X != -1 && previousDirection.Y == 1) && 
                if (visionArea[count + width - 1].G.Equals(green1) || visionArea[count + width - 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count + width - 1] = purple;
                    num++;
                    returnValue = findGreen(count + width - 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint, num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            try
            {
                //previousDirection.X != -1 && 
                if (visionArea[count - 1].G.Equals(green1) || visionArea[count - 1].G.Equals(green2))
                {
                    if (count % width > rightMostPoint % width)
                        rightMostPoint = count;
                    else if (count % width < leftMostPoint % width)
                        leftMostPoint = count;
                    if (count / width > bottomMostPoint / width)
                        bottomMostPoint = count;
                    else if (count / width < topMostPoint / width)
                        topMostPoint = count;
                    visionArea[count - 1] = purple;
                    num++;
                    returnValue = findGreen(count - 1, visionArea, width, leftMostPoint, rightMostPoint, topMostPoint, bottomMostPoint,num);
                    topMostPoint = returnValue[0];
                    bottomMostPoint = returnValue[1];
                    leftMostPoint = returnValue[2];
                    rightMostPoint = returnValue[3];
                }
            }
            catch (IndexOutOfRangeException e) { }
            returnValue[0] = topMostPoint;
            returnValue[1] = bottomMostPoint;
            returnValue[2] = leftMostPoint;
            returnValue[3] = rightMostPoint;
            returnValue[4] = num;
            return returnValue;
        }

        //not used - part of object detection
        //finds next blueish pixel
        private int findNext(int count, Vector2 previousDirection, Color[] visionArea, int width, int countStop)
        {
            int currentHigh=0;
            int returnValue=countStop;
            try
            {
                //previousDirection.X != 1 && 
                if (visionArea[count + 1].B > currentHigh)
                {
                    returnValue = count + 1;
                    currentHigh = visionArea[count + 1].B;
                    visionArea[count+1] = purple;
                    //previousDirection.X = -1;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.Y == -1 &&
                if (visionArea[count - width].B > currentHigh)
                {
                    returnValue = count - width;
                    currentHigh = visionArea[count - width].B;
                    visionArea[count-width] = purple;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != 1 && previousDirection.Y == -1) 
                if (visionArea[count - width + 1].B >currentHigh)
                {
                    returnValue = count - width + 1;
                    currentHigh = visionArea[count - width + 1].B;
                    visionArea[count-width+1] = purple;
                }
            }
            catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.Y == 1 && 
                if (visionArea[count + width].B >currentHigh)
                {
                    returnValue = count + width;
                    currentHigh = visionArea[count + width].B;
                    visionArea[count+width] = purple;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != 1 && previousDirection.Y == 1) && 
            if (visionArea[count + width + 1].B > currentHigh)
                {
                    returnValue =  count + width + 1;
                    currentHigh = visionArea[count + width + 1].B;
                    visionArea[count+width+1] = purple;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != -1 && previousDirection.Y == -1) && 
                if (visionArea[count - width - 1].B > currentHigh)
                {
                    returnValue = count - width - 1;
                    currentHigh = visionArea[count - width - 1].B;
                    visionArea[count-width-1] = purple;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //(previousDirection.X != -1 && previousDirection.Y == 1) && 
                if (visionArea[count + width - 1].B >currentHigh)
                {
                    returnValue = count + width - 1;
                    currentHigh = visionArea[count + width - 1].B;
                    visionArea[count+width -1] = purple;
                }
            }catch(IndexOutOfRangeException e){}
            try{
                //previousDirection.X != -1 && 
                if (visionArea[count - 1].B > currentHigh)
                {
                    returnValue = count-1;
                    currentHigh = visionArea[count - 1].B;
                    visionArea[count-1] = purple;
                }
            }catch(IndexOutOfRangeException e){}
            if (currentHigh > 100)
                return returnValue;
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

        //not used - part of object detection
        //specific for object detection
        private ObjectSeen detect(Color[] visionArea2, int top, int bottom, int left, int right, int imageWidth, int imageHeight, int creatureX, int creatureY)
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
                for (int y = 0; y < height; y++)
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
                        bestObject = new ObjectSeen(types[i / 8], new Vector2(creatureX - (left%imageWidth + width / 2), creatureY - (top/imageWidth + height / 2)), directions[i % 8]);
                    }
                }
                if (best > Parameters.histThreshold)
                    bestObject.type = Classification.Unknown;
            }

            return bestObject;
        }

        private int[,] createHistogram(int[,] shape, int width, int height)
        {
            int boxDivider1 = (width + height) / Parameters.boxNum1;
            int boxDivider2 = 360 / Parameters.boxNum2;
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
            for (x = 0; x <Parameters.boxNum1; x++)
            {
                for (y = 0; y <Parameters.boxNum2; y++)
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
