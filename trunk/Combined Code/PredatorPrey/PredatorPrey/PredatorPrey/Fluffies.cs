using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Fluffies : Creature
    {
        private AvoidanceRule avoid;
        private SteeringRule steer;
        private GoalRule goal;
        private Vector2 currentGoal;

        public Fluffies(Vector2 position) : base(position)
        {
            brain = new NeuralNetwork(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject, Parameters.inputsPerSensedObject,
                Parameters.behav_numOfHiddenLayers, Parameters.behav_numOfNeuronsPerLayer);

            avoid = new AvoidanceRule();
            steer = new SteeringRule();
            goal = new GoalRule();
        }

        public override void update(VisionContainer vc, AudioContainer ac)
        {
            //step1: update values that change with time (hunger)
            
            // Stop the creature (fluffies or wulffies)
            if (eatDuration > 0)
            {
                eatDuration--;
                if (eatDuration == 0)
                {
                    eating = false;
                }
                return;
            }

            //update the hunger
            if (eating)
                eat();
            else
                starve();

            //step2: use prey rules (extract data from VisionContainer) to create a list of movement vectors
            List<Vector2> ruleVectors = new List<Vector2>(Parameters.preyNumberOfRules);

            //ruleVectors.Add(avoid.run(vc, ac));
            //ruleVectors.Add(steer.run(vc, ac));
            //ruleVectors.Add(goal.run());

            //step3: sum up movement vectors using stored weights

            //step4: update velocity, position, and direction
        }

        public void updateWeights()
        {
            // step1: calculate fitness of current state

            // step2: classify state as either good or not good

            // step3: if state is good do nothing
            //        if state is not good, attribute the bad state to 1 (or more) of the rules
            //              ???????
        }

        //this is just my first idea, feel free to change it if you think of something
        //also the weights are just temporary values right now
        public override double calculateFitness()
        {
            double numberOfWolves = 0;
            double closestWolf = 0;
            return Parameters.hungerWeight * base.hunger + Parameters.numberOfWolvesWeight * numberOfWolves + Parameters.closestWolfWeight * closestWolf;
        }
    }
}
