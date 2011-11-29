using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetworkToolkit
{
    static class Parameters
    {
        public const int weightRange = 1;
        public const int numberOfInputs = 4;
        public const int numberOfOutputs = 2;
        public const int numberOfHiddenLayers = 0;
        public const int numberOfNeuronsPerHiddenLayer = 6;
        public const double responseCurve = 1;
        public const double bias = -1;
        public const double crossoverRate = 0.7;
        public const double mutationRate = 0.3;
        public const int numberOfFittestCopies = 5;
        public const int numberOfWolves = 20;
        public const int numberOfSheep = 40;
        public const int numberOfTicks = 2000;
        public const double maxRotation = 0.1;
        public const double maxMoveSpeed = 2;
        public const int minDistanceToTouch = 15;

        public static int worldWidth;
        public static int worldHeight;

        public static Random random;
    }
}
