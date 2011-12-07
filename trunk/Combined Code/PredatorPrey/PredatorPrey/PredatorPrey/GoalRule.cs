using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class GoalRule : Rule
    {
        GoalRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork(Parameters.inputsPerSensedObject, Parameters.inputsPerSensedObject,
                Parameters.avoid_numOfHiddenLayers, Parameters.avoid_numOfNeuronsPerLayer);

            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = new List<double>(totalNumberOfWeights);

            for (int i = 0; i < totalNumberOfWeights; i++)
            {
                newWeights.Add(1);
            }
            ruleNet.replaceWeights(newWeights);
        }

        public override Vector2 run(Vector2 pos)
        {
            List<double> inputs = new List<double>(Parameters.inputsPerSensedObject);

            double magnitudeInput = pos.Length() / Parameters.preyMaxVisionDist;

            if (pos != Vector2.Zero)
                pos.Normalize();

            inputs.Add(pos.X);
            inputs.Add(pos.Y);
            inputs.Add(magnitudeInput);

            List<double> outputs = ruleNet.run(inputs);

            Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            if (result != Vector2.Zero)
                result.Normalize();

            Vector2.Multiply(result, (float)(outputs[3] * Parameters.maxMoveSpeed));

            return result;
        }

        public override Vector2 run(VisionContainer vc, AudioContainer ac)
        {
            throw new NotImplementedException();
        }
    }
}
