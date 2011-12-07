using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    abstract class Rule
    {
        protected NeuralNetwork ruleNet;

        public abstract Vector2 run(VisionContainer vc, AudioContainer ac);

        public abstract Vector2 run(Vector2 goal);
    }
}
