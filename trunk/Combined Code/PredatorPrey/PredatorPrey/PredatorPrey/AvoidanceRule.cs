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

        public AvoidanceRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork(Parameters.maxVisionInput + Parameters.maxHearInput, Parameters.inputsPerSensedObject,
                Parameters.avoid_numOfHiddenLayers, Parameters.avoid_numOfNeuronsPerLayer);
            
            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = new List<double>(totalNumberOfWeights);

            for (int i = 0; i < totalNumberOfWeights; i++)
            {
                newWeights.Add(-1);
            }
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

            List<double> inputs = new List<double>(Parameters.maxVisionInput + Parameters.maxHearInput);
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

                double magnitudeInput = pos.Length() / Parameters.preyMaxVisionDist;

                if (pos != Vector2.Zero)
                    pos = Vector2.Normalize(pos);

                inputs.Add(pos.X);
                inputs.Add(pos.Y);
                inputs.Add(magnitudeInput);
            }

            // fill remaining inputs with 0's
            for (int i = inputs.Count(); i < Parameters.maxVisionInput + Parameters.maxHearInput; i++)
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
