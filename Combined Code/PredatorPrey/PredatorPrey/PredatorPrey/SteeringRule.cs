using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class SteeringRule
    {
        public NeuralNetwork ruleNet;
        private Classification acceptType;

        public SteeringRule(Classification acceptType)
        {
            this.acceptType = acceptType;

            // create new neural network
            ruleNet = new NeuralNetwork(Parameters.maxVisionInput, Parameters.inputsPerSensedObject,
                Parameters.steer_numOfHiddenLayers, Parameters.steer_numOfNeuronsPerLayer);

            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = ruleNet.getListOfWeights();

            int i;
            int scale;
            for (i = 0, scale = 0; i < Parameters.maxVisionInput; i++, scale++)
            {
                if (i % 2 == 0)
                {
                    newWeights[i] = 1 * ((float)Parameters.maxVisionInput - scale) / Parameters.maxVisionInput;
                }
                else
                {
                    newWeights[i] = 0;
                }
            }
            newWeights[i] = 0;

            for (i = i + 1, scale = 0; i < 2 * Parameters.maxVisionInput + 1; i++)
            {
                if (i % 2 == 0)
                {
                    newWeights[i] = 1 * ((float)Parameters.maxVisionInput - scale) / Parameters.maxVisionInput;
                }
                else
                {
                    newWeights[i] = 0;
                }
            }
            newWeights[i] = 0;

            ruleNet.replaceWeights(newWeights);
        }

        public Vector2 run(VisionContainer vc)
        {
            if (vc.size() == 0)
                return Vector2.Zero;

            List<Vector2> visionPos = new List<Vector2>(vc.size());

            for (int i = 0; i < Math.Min(Parameters.maxVisionInput/Parameters.inputsPerSensedObject, vc.size()); i++)
            {
                if (vc.getSeenObject(i).type == acceptType)
                    visionPos.Add(vc.getSeenObject(i).position);
            }

            List<double> inputs = new List<double>(Parameters.maxVisionInput);
            for (int i = 0; i < visionPos.Count; i++)
            {
                Vector2 pos = visionPos[i];

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
            }

            // fill remaining inputs with 0's
            for (int i = inputs.Count(); i < Parameters.maxVisionInput; i++)
            {
                inputs.Add(0);
            }

            if (inputs.Count() != (Parameters.maxVisionInput))
                Console.WriteLine("miscounted: expected (" + Parameters.maxVisionInput +
                    "); actual (" + inputs.Count() + ")");

            List<double> outputs = ruleNet.run(inputs);

            Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            return result;
        }

        // updates the rule with a list of delta values
        public void update(List<double> deltaIn)
        {
            ruleNet.chainPropagation(deltaIn);
        }
    }
}
