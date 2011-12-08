using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Wulffies : Creature
    {
        

        public Wulffies(Vector2 position) : base(position)
        {
            brain = new NeuralNetwork(Parameters.preyNumberOfRules * Parameters.inputsPerSensedObject, Parameters.inputsPerSensedObject,
                    Parameters.behav_numOfHiddenLayers, Parameters.behav_numOfNeuronsPerLayer);
            good = false;
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

            // step3: sum up movement vectors using stored weights

            // step4: update velocity, position, and direction

            Vector2 acceleration;
            if (position.X == 149)
                acceleration = new Vector2((float)0f, (float)0.5f);
            else
                acceleration = new Vector2(0.5f, 0f);

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

        public int calculateFitness()
        {
            // step1: ???????

            return 0;
        }
    }
}
