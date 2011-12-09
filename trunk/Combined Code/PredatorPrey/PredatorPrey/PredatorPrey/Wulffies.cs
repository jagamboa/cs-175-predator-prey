using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Wulffies : Creature
    {
        private AvoidanceRule avoid;
        private AlignmentRule align;
        private GoalRule goal;

        private Vector2 currentGoal;


        public Wulffies(Vector2 position) : base(position)
        {
            brain = new NeuralNetwork(Parameters.predatorNumberOfRules * Parameters.inputsPerSensedObject, Parameters.inputsPerSensedObject,
                    Parameters.behav_numOfHiddenLayers, Parameters.behav_numOfNeuronsPerLayer);
            good = false;
            score = 0;

            avoid = new AvoidanceRule(Classification.Predator);
            align = new AlignmentRule(Classification.Predator);
            goal = new GoalRule();

            currentGoal = Vector2.Zero;
        }

        public override void wrap(VisionContainer vc, AudioContainer ac)
        {
            // step1: update values that change with time (hunger)

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

            // step2: use predator rules (extract data from VisionContainer) to create a list of movement vectors
            List<Vector2> ruleVectors = new List<Vector2>(Parameters.predatorNumberOfRules);

            ruleVectors.Add(avoid.run(vc, ac));
            ruleVectors.Add(align.run(vc));

            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type == Classification.Prey)
                {
                    currentGoal = vc.getSeenObject(i).position;
                    break;
                }
            }
            ruleVectors.Add(Vector2.Multiply(goal.run(currentGoal, 0), (float)hunger));

            // step3: sum up movement vectors using stored weights
            List<double> inputs = new List<double>(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject);
            for (int i = 0; i < ruleVectors.Count; i++)
            {
                Vector2 pos = ruleVectors[i];

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
            }

            List<double> outputs = brain.run(inputs);

            // step4: update velocity, position, and direction

            Vector2 acceleration = new Vector2((float)outputs[0], (float)outputs[1]);

            if (acceleration.Length() != 0)
                acceleration = Vector2.Normalize(acceleration);
            acceleration = Vector2.Clamp(acceleration, new Vector2(-Parameters.accel_clampVal, -Parameters.accel_clampVal),
                new Vector2(Parameters.accel_clampVal, Parameters.accel_clampVal));
            acceleration = acceleration * Parameters.maxAcceleration;

            velocity = acceleration + velocity;

            if (velocity.Length() > Parameters.maxMoveSpeed)
            {
                velocity = Vector2.Normalize(velocity) * Parameters.maxMoveSpeed;
            }

            position = position + velocity;

            if (velocity != Vector2.Zero)
            {
                Vector2 direction = Vector2.Normalize(velocity);
                rotation = Math.Atan2(direction.Y, direction.X) - Math.PI / 2;
            }

            base.wrap(vc, ac);
        }

        public void updateWeights()
        {
            // step1: calculate fitness of current state

            // step2: classify state as either good or not good

            // step3: if state is good do nothing
            //        if state is not good, attribute the bad state to 1 (or more) of the rules
            //              ???????
        }

        public override int calculateFitness(VisionContainer vc)
        {
            int numberOfSheep = 0;
            int closestSheep = int.MaxValue;

            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type == Classification.Prey)
                {
                    numberOfSheep++;
                    int distance = (int)Vector2.Subtract(vc.getSeenObject(i).position, position).Length();

                    if (distance < closestSheep)
                        closestSheep = distance;
                }
            }

            fitness = (int)(Parameters.initFitness + hunger * Parameters.hungerWeight + 
                Parameters.numberOfSheepWeight * numberOfSheep);

            if (closestSheep < Parameters.closestSheepMaxPenalty)
            {
                fitness += Parameters.closestFoodWeight * closestSheep;
            }
            else
            {
                fitness -= Parameters.closestSheepMaxPenalty;
            }

            if (fitness < Parameters.minFitness)
                fitness = Parameters.minFitness;
            else if (fitness > Parameters.maxFitness)
                fitness = Parameters.maxFitness;

            return fitness;
        }

        public override void eat()
        {
            currentGoal = Vector2.Zero;
            eatDuration = Parameters.wulffieEatTime;
            base.eat();
        }
    }
}
