using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    static class Parameters
    {
        // neural network
        public const int weightRange = 1;
        public const float responseCurve = 1F;
        public const float bias = -1F;
        public const float learningRate = 10000F;

        // genetic algorithm
        public const float crossoverRate = 0.7F;
        public const float mutationRate = 0.3F;
        public const int numberOfFittestCopies = 5;

        public const int numberOfWolves = 10;
        public const int numberOfSheep = 40;
        public const int numberOfGrassies = 5;
        public const int numberOfUpdates = 1000;
        public const int displayLivingTime = 350;

        public const float maxRotation = 0.1F;
        public const float maxMoveSpeed = 2F;
        public const float maxAcceleration = .25F;
        public const int minDistanceToTouch = 20;

        public const int k = 1;
        public const int fluffiesScore = 500;
        public const int wulffiesScore = 100;

        public static int worldWidth;
        public static int worldHeight;

        public static Random random;

        //these are values for the creatures
        public const float startingHunger = 5F;
        public const float hungryThreshold = 20F;
        public const float maxHunger = 30F;
        public const int eating = -1;
        public const float starving =.025F;

        //these are values for the weights that determin the fitness function
        public const int hungerWeight = -1;
        public const int numberOfWolvesWeight = -5;
        public const int closestWolfWeight = 1;
        //public const int closestWolfDist = 300;
        //public const int closestWolfWeight = 300;
        public const int numberOfFoodWeight = 5;
        public const int closestFoodWeight = -1;
        //public const int closestFoodMaxPenalty = 300;
        public const int numberOfSheepWeight = 5;
        public const int closestSheepWeight = -1;
        //public const int closestSheepMaxPenalty = 300;
        public const int maxFitness = 1000;
        public const int initFitness = 500;
        public const int minFitness = 0;

        //these are for size of vision
        //these were used for object detection
        /*
        public const int predatorVisionWidth = 30;
        public const int predatorVisionHeight = 30;
        public const int predatorVisionOffset = predatorVisionHeight/2;
        public const int preyVisionWidth = 30;
        public const int preyVisionHeight = 30;
        public const int preyMaxVisionDist = 25;
        */
        public const int wulffiesVisionThreashold = 400;
        public const int fluffiesVisionThreashold = 400;

        public const int boxNum1 = 10;
        public const int boxNum2 = 10;
        public const double histThreshold = 3;

        //Duration of eating
        public const int wulffieEatTime = 60;
        public const int fluffieEatTime = 40;
        public const int minDistanceToFluffyEat = 25;
        public const double eatingThreshold = .3;
        public const int dontEatCount = 30;

        //constants for rules
        public const int preyNumberOfRules = 4;
        public const int predatorNumberOfRules = 2;
        public const int inputsPerSensedObject = 2;
        public const int maxVisionInput = 15 * inputsPerSensedObject;
        public const int maxHearInput = 15 * inputsPerSensedObject;

        //behavior selection
        public const int behav_numOfHiddenLayers = 1;
        public const int behav_numOfNeuronsPerLayer = 5;

        // avoidance
        public const int avoid_numOfHiddenLayers = 0;
        public const int avoid_numOfNeuronsPerLayer = 2;

        // steering
        public const int steer_numOfHiddenLayers = 0;
        public const int steer_numOfNeuronsPerLayer = 2;

        // alignment
        public const int align_numOfHiddenLayers = 0;
        public const int align_numOfNeuronsPerLayer = 2;

        // goal
        public const int goal_numberOfExtraInputs = 1;
        public const int goal_numOfHiddenLayers = 0;
        public const int goal_numOfNeuronsPerLayer = 2;

        // steering modification
        public const float accel_clampVal = 0.15F;//0.015F;


        // test run modifiers
        public const bool wulffiesLearn = true;
        public const bool fluffiesLearn = true;

        // predator
        // 0 = run all rules + learning
        // 1 = run only avoid rule (no learning)
        // 2 = run only goal rule (no learning)
        public const int predatorRule = 0;

        // prey
        // 0 = run all rules + learning
        // 1 = run only avoid rule (no learning)
        // 2 = run only steer rule (no learning)
        // 3 = run only align rule (no learning)
        // 4 = run only goal rule (no learning)
        public const int preyRule = 0;

    }
}
