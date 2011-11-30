using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PredatorPrey
{
    class Predator : Creature
    {
        public Predator(Vector position) : base(position)
        {

        }

        public override void update(VisionContainer vc)
        {
            // step1: update values that change with time (hunger)

            // step2: use predator rules (extract data from VisionContainer) to create a list of movement vectors

            // step3: sum up movement vectors using stored weights

            // step4: update velocity, position, and direction
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
