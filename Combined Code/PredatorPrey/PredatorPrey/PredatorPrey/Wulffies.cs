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

            // update previous values
            prevPos = position;
            prevVel = velocity;
            prevVision = vc;

            // step2: use predator rules (extract data from VisionContainer) to create a list of movement vectors
            List<Vector2> ruleVectors = new List<Vector2>(Parameters.predatorNumberOfRules);

            ruleVectors.Add(avoid.run(vc, ac));
            //ruleVectors.Add(align.run(vc));

            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type == Classification.Prey)
                {
                    currentGoal = vc.getSeenObject(i).position;
                    break;
                }
            }
            ruleVectors.Add(Vector2.Multiply(goal.run(currentGoal, 0), 1));

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
            Vector2 acceleration;
            if (Parameters.predatorRule == 1)
            {
                acceleration = new Vector2(ruleVectors[0].X, ruleVectors[0].Y);
            }
            else if (Parameters.predatorRule == 2)
            {
                acceleration = new Vector2(ruleVectors[1].X, ruleVectors[1].Y);
            }
            else
            {
                acceleration = new Vector2((float)outputs[0], (float)outputs[1]);
            }

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
            double fitnessForThisState = calculateFitness(prevVision);

            // step2: calculate fitness for orthogonal acceleration values
            Vector2 rightAcc = new Vector2((float)(prevAcc.X * Math.Cos(Math.PI / 2) - prevAcc.Y * Math.Sin(Math.PI / 2)),
                                            (float)(prevAcc.X * Math.Sin(Math.PI / 2) + prevAcc.Y * Math.Cos(Math.PI / 2)));
            Vector2 rightAccClamp = Vector2.Clamp(rightAcc, new Vector2(-Parameters.accel_clampVal, -Parameters.accel_clampVal),
                                                new Vector2(Parameters.accel_clampVal, Parameters.accel_clampVal));
            rightAccClamp = rightAccClamp * Parameters.maxAcceleration;

            Vector2 leftAcc = new Vector2((float)(prevAcc.X * Math.Cos(-Math.PI / 2) - prevAcc.Y * Math.Sin(-Math.PI / 2)),
                                            (float)(prevAcc.X * Math.Sin(-Math.PI / 2) + prevAcc.Y * Math.Cos(-Math.PI / 2)));
            Vector2 leftAccClamp = Vector2.Clamp(leftAcc, new Vector2(-Parameters.accel_clampVal, -Parameters.accel_clampVal),
                                                new Vector2(Parameters.accel_clampVal, Parameters.accel_clampVal));
            leftAccClamp = leftAccClamp * Parameters.maxAcceleration;

            Vector2 leftChoiceVelocity = prevVel + leftAccClamp;
            if (leftChoiceVelocity.Length() > Parameters.maxMoveSpeed)
                leftChoiceVelocity = Vector2.Normalize(leftChoiceVelocity) * Parameters.maxMoveSpeed;

            Vector2 rightChoiceVelocity = prevVel + rightAccClamp;
            if (rightChoiceVelocity.Length() > Parameters.maxMoveSpeed)
                rightChoiceVelocity = Vector2.Normalize(rightChoiceVelocity) * Parameters.maxMoveSpeed;

            //Console.WriteLine("Hindsight: Angle between right/left velocities = " 
            //    + (180 / Math.PI) * Math.Acos(Vector2.Dot(Vector2.Normalize(leftChoiceVelocity), Vector2.Normalize(rightChoiceVelocity))) + " degrees");

            Vector2 tempPosition = position;

            position = prevPos + leftChoiceVelocity;

            double leftFitness = calculateFitness(prevVision);

            position = prevPos + rightChoiceVelocity;

            double rightFitness = calculateFitness(prevVision);

            // step3: compare the 3 fitness values, if one of the two orthogonal
            //          fitness values is greater than the current fitness,
            //          generate a corrected acceleration vector
            bool isError = false;
            Vector2 correctedAcceleration = Vector2.Zero;
            if (leftFitness > fitnessForThisState || rightFitness > fitnessForThisState)
            {
                isError = true;

                if (leftFitness > rightFitness)
                {
                    correctedAcceleration = rightAcc * prevAccMag;
                }
                else
                {
                    correctedAcceleration = leftAcc * prevAccMag;
                }
            }

            // step4: pass corrected acceleration vector to the neural network for
            //          backpropagation
            if (isError)
            {
                List<double> target = new List<double>(2);
                target.Add(correctedAcceleration.X);
                target.Add(correctedAcceleration.Y);
                List<double> deltaOut = brain.updateWeights(target);

                List<double> deltaIn = new List<double>(Parameters.inputsPerSensedObject);
                deltaIn.Add(deltaOut[0]);
                deltaIn.Add(deltaOut[1]);
                avoid.update(deltaIn);
                deltaIn[0] = deltaOut[2];
                deltaIn[1] = deltaOut[3];
                goal.update(deltaIn);
               /* deltaIn[0] = deltaOut[4];
                deltaIn[1] = deltaOut[5];
                goal.update(deltaIn);*/
            }

            position = tempPosition;
        }

        public override double calculateFitness(VisionContainer vc)
        {
            int numberOfSheep = 0;
            double closestSheep = double.MaxValue;

            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type == Classification.Prey)
                {
                    numberOfSheep++;
                    double distance = Vector2.Subtract(vc.getSeenObject(i).position, position).Length();

                    if (distance < closestSheep)
                        closestSheep = distance;
                }
            }

            fitness = Parameters.initFitness + hunger * Parameters.hungerWeight + 
                Parameters.numberOfSheepWeight * numberOfSheep;

            if (numberOfSheep > 0)
            {
                fitness += Parameters.closestSheepWeight * closestSheep;
            }
            else
            {
                fitness -= Parameters.wulffiesVisionThreashold;
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

        public void printWeights()
        {
            Action<double> print = elem => { Console.Write(elem + ", "); };
            Console.Write("Behavior Neural Net:  ");
            brain.getListOfWeights().ForEach(print);
            Console.Write("\n");
            Console.Write("Avoid Rule:  ");
            avoid.ruleNet.getListOfWeights().ForEach(print);
            Console.Write("\n");
            Console.Write("Goal Rule:  ");
            goal.ruleNet.getListOfWeights().ForEach(print);
            Console.Write("\n");
        }
    }
}
