using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class GoalRule
    {
        private NeuralNetwork ruleNet;

        public GoalRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork(Parameters.inputsPerSensedObject + Parameters.goal_numberOfExtraInputs, 
                Parameters.inputsPerSensedObject, Parameters.avoid_numOfHiddenLayers, Parameters.avoid_numOfNeuronsPerLayer);

            // replace default weights with custom weights
            int totalNumberOfWeights = ruleNet.getTotalNumberOfWeights();
            List<double> newWeights = ruleNet.getListOfWeights();

            newWeights[1] = 1;
            newWeights[2] = 0;
            newWeights[5] = 0;
            newWeights[6] = 1;
            newWeights[9] = 1;
            newWeights[10] = 0;
            newWeights[12] = 0;
            newWeights[13] = 1;

            ruleNet.replaceWeights(newWeights);
        }

        public Vector2 run(Vector2 pos, double hunger)
        {
            List<double> inputs = new List<double>(Parameters.inputsPerSensedObject);

            inputs.Add(pos.X);
            inputs.Add(pos.Y);
            inputs.Add(hunger);

            List<double> outputs = ruleNet.run(inputs);

            Vector2 result = new Vector2((float)outputs[0], (float)outputs[1]);

            return result;
        }
    }
}
