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
        private AlignmentRule align;
        private GoalRule goal;
        private Vector2 currentGoal;

        public Fluffies(Vector2 position) : base(position)
        {
            brain = new NeuralNetwork(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject, 3,
                Parameters.behav_numOfHiddenLayers, Parameters.behav_numOfNeuronsPerLayer);

            avoid = new AvoidanceRule();
            steer = new SteeringRule();
            align = new AlignmentRule();
            //goal = new GoalRule();
            currentGoal = new Vector2(position.X, position.Y);
            good = false;
        }

        public override void wrap(VisionContainer vc, AudioContainer ac)
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

            ruleVectors.Add(avoid.run(vc, ac));
            ruleVectors.Add(steer.run(vc));
            ruleVectors.Add(align.run(vc));
            //ruleVectors.Add(goal.run(currentGoal, hunger));

            //step3: pass vectors into neural network to get outputs
            List<double> inputs = new List<double>(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject);
            for (int i = 0; i < ruleVectors.Count; i++)
            {
                Vector2 pos = ruleVectors[i];

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
            }

            //List<double> outputs = brain.run(inputs);

            //Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            //if (result != Vector2.Zero)
            //    result.Normalize();

            //Vector2.Multiply(result, (float)(outputs[3] * Parameters.maxMoveSpeed));

            //step4: update velocity, position, and direction
            Vector2 acceleration = new Vector2((float) ruleVectors[2].X, (float) ruleVectors[2].Y);
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
                rotation = Math.Atan2(direction.Y, direction.X) - Math.PI/2;
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

        //this is just my first idea, feel free to change it if you think of something
        //also the weights are just temporary values right now
        public override double calculateFitness()
        {
            double numberOfWolves = 0;
            double closestWolf = 0;
            return Parameters.hungerWeight * base.hunger + Parameters.numberOfWolvesWeight * numberOfWolves + Parameters.closestWolfWeight * closestWolf;
        }

        public void die()
        {
            isAlive = false;
        }
    }
}
