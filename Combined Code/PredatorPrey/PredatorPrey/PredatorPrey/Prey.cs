using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Prey : Creature
    {



        public Prey(Vector2 position) : base(position)
        {

        }

        //public override void update(VisionContainer vc)
        //{
            // step1: update values that change with time (hunger)

            // step2: use prey rules (extract data from VisionContainer) to create a list of movement vectors

            // step3: sum up movement vectors using stored weights

            // step4: update velocity, position, and direction
        //}

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
