using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    static class Parameters
    {
        public const int weightRange = 1;
        public const int numberOfInputs = 4;
        public const int numberOfOutputs = 2;
        public const int numberOfHiddenLayers = 0;
        public const int numberOfNeuronsPerHiddenLayer = 6;
        public const float responseCurve = 1F;
        public const float bias = -1F;
        public const float crossoverRate = 0.7F;
        public const float mutationRate = 0.3F;
        public const int numberOfFittestCopies = 5;
        public const int numberOfWolves = 4;
        public const int numberOfSheep = 1;
        public const int numberOfUpdates = 2000;
        public const float maxRotation = 0.1F;
        public const float maxMoveSpeed = 2F;
        public const float maxAcceleration = .25F;
        public const int minDistanceToTouch = 20;

        public static int worldWidth;
        public static int worldHeight;

        public static Random random;

        //these are values for the creatures
        public const float startingHunger = 5F;
        public const int eatingAddition = 1;
        public const float starvingSubtract =.1F;

        //these are values for the weights that determin the fitness function
        public const float hungerWeight = .5F;
        public const float numberOfWolvesWeight = .2F;
        public const float closestWolfWeight = .3F;

        //these are for size of vision
        public const int predatorVisionWidth = 40;
        public const int predatorVisionHeight = 50;
        public const int predatorVisionOffset = predatorVisionHeight/2;
        public const int preyVisionWidth = 50;
        public const int preyVisionHeight = 50;
        public const int preyMaxVisionDist = 25;

        //Duration of eating
        public const int eatTime = 20;

        //constants for rules
        public const int preyNumberOfRules = 4;
        public const int predatorNumberOfRules = 1;
        public const int inputsPerSensedObject = 3;
        public const int maxVisionInput = 15 * inputsPerSensedObject;
        public const int maxHearInput = 15 * inputsPerSensedObject;

        //behavior selection
        public const int behav_numOfHiddenLayers = 1;
        public const int behav_numOfNeuronsPerLayer = 5;

        // avoidance
        public const int avoid_numOfHiddenLayers = 1;
        public const int avoid_numOfNeuronsPerLayer = 10;

        // steering
        public const int steer_numOfHiddenLayers = 1;
        public const int steer_numOfNeuronsPerLayer = 10;

        // alignment
        public const int align_numOfHiddenLayers = 1;
        public const int align_numOfNeuronsPerLayer = 10;

        // goal
        public const int goal_numberOfExtraInputs = 1;
        public const int goal_numOfHiddenLayers = 1;
        public const int goal_numOfNeuronsPerLayer = 10;

        // steering modification
        public const float accel_clampVal = 0.015F;
    }
}
