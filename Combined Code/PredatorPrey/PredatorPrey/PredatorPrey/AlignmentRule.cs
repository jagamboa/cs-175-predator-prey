using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class AlignmentRule
    {
        private NeuralNetwork ruleNet;

        public AlignmentRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork(Parameters.maxVisionInput, Parameters.inputsPerSensedObject,
                Parameters.align_numOfHiddenLayers, Parameters.align_numOfNeuronsPerLayer);

            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = new List<double>(totalNumberOfWeights);

            for (int i = 0; i < totalNumberOfWeights; i++)
            {
                newWeights.Add(1);
            }
            ruleNet.replaceWeights(newWeights);
        }

        public Vector2 run(VisionContainer vc)
        {
            if (vc.size() == 0)
                return Vector2.Zero;

            List<Vector2> visionDir = new List<Vector2>(vc.size());

            for (int i = 0; i < Math.Min(Parameters.maxVisionInput, vc.size()); i++)
            {
                visionDir.Add(vc.getSeenObject(i).direction);
            }

            List<double> inputs = new List<double>(Parameters.maxVisionInput);
            for (int i = 0; i < vc.size(); i++)
            {
                Vector2 pos = vc.getSeenObject(i).position;

                double magnitudeInput = pos.Length() / Parameters.preyMaxVisionDist;

                if (pos != Vector2.Zero)
                    pos = Vector2.Normalize(pos);

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
                inputs.Add(magnitudeInput);
            }

            // fill remaining inputs with 0's
            for (int i = inputs.Count(); i < Parameters.maxVisionInput; i++)
            {
                inputs.Add(0);
            }

            if (inputs.Count() != (Parameters.maxVisionInput + Parameters.maxHearInput))
                Console.WriteLine("miscounted: expected (" + (Parameters.maxVisionInput + Parameters.maxHearInput) +
                    "); actual (" + inputs.Count() + ")");

            List<double> outputs = ruleNet.run(inputs);

            Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            if (result != Vector2.Zero)
                result.Normalize();

            Vector2.Multiply(result, (float)(outputs[3] * Parameters.maxMoveSpeed));

            return result;
        }
    }
}
