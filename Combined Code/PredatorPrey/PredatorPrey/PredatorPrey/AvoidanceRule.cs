using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class AvoidanceRule
    {
        private NeuralNetwork ruleNet;
        private int totalInput = Parameters.maxVisionInput + Parameters.maxHearInput;
        public AvoidanceRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork(totalInput, Parameters.inputsPerSensedObject,
                Parameters.avoid_numOfHiddenLayers, Parameters.avoid_numOfNeuronsPerLayer);

            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = ruleNet.getListOfWeights();

            int i;
            int scale;
            for (i = 0, scale = 0; i < totalInput; i++, scale++)
            {
                if (i % 2 == 0)
                {
                    newWeights[i] = -1 * ((float)totalInput - scale) / totalInput;
                }
                else
                {
                    newWeights[i] = 0;
                }
            }
            newWeights[i] = 0;

            for (i = i + 1, scale = 0; i < 2 * totalInput + 1; i++)
            {
                if (i % 2 == 0)
                {
                    newWeights[i] = -1 * ((float)totalInput - scale) / totalInput;
                }
                else
                {
                    newWeights[i] = 0;
                }
            }
            newWeights[i] = 0;

            ruleNet.replaceWeights(newWeights);
        }

        public Vector2 run(VisionContainer vc, AudioContainer ac)
        {
            if (vc.size() == 0 && ac.size() == 0)
                return Vector2.Zero;

            List<Vector2> visionPos = new List<Vector2>(vc.size());
            List<Vector2> hearPos = new List<Vector2>(ac.size());

            for (int i = 0; i < Math.Min(Parameters.maxVisionInput, vc.size()); i++)
            {
                visionPos.Add(vc.getSeenObject(i).position);
            }
            for (int i = 0; i < Math.Min(Parameters.maxHearInput, ac.size()); i++)
            {
                hearPos.Add(ac.getHeardObject(i));
            }

            List<double> inputs = new List<double>(totalInput);
            for (int i = 0; i < vc.size() + ac.size(); i++)
            {
                Vector2 pos;
                if (hearPos.Count == 0 || (visionPos.Count != 0 && visionPos[0].Length() < hearPos[0].Length()))
                {
                    pos = visionPos[0];
                    visionPos.RemoveAt(0);
                }
                else
                {
                    pos = hearPos[0];
                    hearPos.RemoveAt(0);
                }

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
            }

            // fill remaining inputs with 0's
            for (int i = inputs.Count(); i < totalInput; i++)
            {
                inputs.Add(0);
            }

            if (inputs.Count() != totalInput)
                Console.WriteLine("miscounted: expected (" + totalInput +
                    "); actual (" + inputs.Count() + ")");

            List<double> outputs = ruleNet.run(inputs);

            Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            return result;
        }
    }
}
