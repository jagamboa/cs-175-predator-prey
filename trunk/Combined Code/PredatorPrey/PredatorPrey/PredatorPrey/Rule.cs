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

        public abstract void run(VisionContainer vc, AudioContainer ac);

        public abstract void run(Vector2 goal);
    }
}
