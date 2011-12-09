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
            brain = new NeuralNetwork(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject, Parameters.inputsPerSensedObject,
                Parameters.behav_numOfHiddenLayers, Parameters.behav_numOfNeuronsPerLayer);

            avoid = new AvoidanceRule(Classification.Unknown);
            steer = new SteeringRule(Classification.Prey);
            align = new AlignmentRule(Classification.Prey);
            goal = new GoalRule();
            currentGoal = new Vector2(500, 500);
            good = false;
            score = 0;
        }

        public override void wrap(VisionContainer vc, AudioContainer ac)
        {
            //step1: update values that change with time (hunger)

            if (eating)
            {
                eat();
                if (hunger <= 0)
                {
                    eating = false;
                    canEat = false;
                    dontEatDuration = Parameters.dontEatCount;
                }
                else
                    hunger--;
            }
            else
                starve();

            // Stop the creature (fluffies or wulffies)
            if (eatDuration > 0)
            {
                eatDuration--;
                if (eatDuration == 0)
                {
                    eating = false;
                    canEat = false;
                    dontEatDuration = Parameters.dontEatCount;
                }
            }

            if (dontEatDuration > 0)
            {
                dontEatDuration--;
                if (dontEatDuration == 0)
                {
                    canEat = true;
                }
            }
            

            // update the score
            if (isAlive)
                score++;

            // update previous values
            prevPos = position;
            prevVel = velocity;
            prevVision = vc;

            //step2: use prey rules (extract data from VisionContainer) to create a list of movement vectors
            List<Vector2> ruleVectors = new List<Vector2>(Parameters.preyNumberOfRules);
            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type.Equals(Classification.Food))
                {
                    currentGoal = vc.getSeenObject(i).position;
                }
            }

            ruleVectors.Add(avoid.run(vc, ac));
            ruleVectors.Add(steer.run(vc));
            ruleVectors.Add(align.run(vc));
            ruleVectors.Add(goal.run(Vector2.Multiply(Vector2.Subtract(currentGoal, position), 0), (float)hunger));

            //step3: pass vectors into neural network to get outputs
            List<double> inputs = new List<double>(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject);
            for (int i = 0; i < ruleVectors.Count; i++)
            {
                Vector2 pos = ruleVectors[i];

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
            }

            List<double> outputs = brain.run(inputs);

            //step4: update velocity, position, and direction
            Vector2 acceleration = new Vector2((float) outputs[0], (float) outputs[1]);

            if (float.IsNaN(acceleration.X))
            {
                List<double> weights = brain.getListOfWeights();
                Console.WriteLine("NaN");
            }

            prevAccMag = acceleration.Length();

            if (acceleration.Length() != 0)
                acceleration = Vector2.Normalize(acceleration);
            
            // update prev acceleration
            prevAcc = acceleration;

            acceleration = Vector2.Clamp(acceleration, new Vector2(-Parameters.accel_clampVal, -Parameters.accel_clampVal),
                new Vector2(Parameters.accel_clampVal, Parameters.accel_clampVal));
            acceleration = acceleration * Parameters.maxAcceleration;
            if(!eating || (Math.Max(acceleration.X,acceleration.Y)>Parameters.eatingThreshold || Math.Min(acceleration.X,acceleration.Y)<-Parameters.eatingThreshold))
            {
                if (eating)
                {
                    eating = false;
                    canEat = false;
                    dontEatDuration = Parameters.dontEatCount;
                }
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
                brain.updateWeights(target);
            }

            position = tempPosition;
        }

        //this is just my first idea, feel free to change it if you think of something
        //also the weights are just temporary values right now
        public override double calculateFitness(VisionContainer vc)
        {
            int numberOfWolves = 0;
            double closestWolf = double.MaxValue;
            int numberOfFood = 0;
            double closestFood = double.MaxValue;

            for (int i = 0; i < vc.size(); i++)
            {
                if (vc.getSeenObject(i).type == Classification.Predator)
                {
                    numberOfWolves++;
                    double distance = Vector2.Subtract(vc.getSeenObject(i).position, position).Length();

                    if (distance < closestWolf)
                        closestWolf = distance;
                }
                else if (vc.getSeenObject(i).type == Classification.Food)
                {
                    numberOfFood++;
                    double distance = Vector2.Subtract(vc.getSeenObject(i).position, position).Length();

                    if (distance < closestFood)
                        closestFood = distance;
                }
            }

            fitness = Parameters.initFitness + numberOfWolves * Parameters.numberOfWolvesWeight + 
                        hunger * Parameters.hungerWeight + Parameters.numberOfFoodWeight * numberOfFood;

            if (numberOfWolves > 0)
            {
                fitness += Parameters.closestWolfWeight * closestWolf;
            }
            else
            {
                fitness += Parameters.fluffiesVisionThreashold;
            }

            if (numberOfFood > 0)
            {
                fitness += Parameters.closestFoodWeight * closestFood;
            }
            else
            {
                fitness -= Parameters.fluffiesVisionThreashold;
            }

            if (fitness < Parameters.minFitness)
                fitness = Parameters.minFitness;
            else if (fitness > Parameters.maxFitness)
                fitness = Parameters.maxFitness;

            return fitness;
        }

        public override void  eat()
        {
            eatDuration = Parameters.fluffieEatTime;
 	        base.eat();
        }

        public override void die()
        {
            score = (int)Math.Max(0,score- hunger);
            base.die();
        }
    }
}
