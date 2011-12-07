using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class AvoidanceRule : Rule
    {
        AvoidanceRule()
        {
            // create new neural network
            ruleNet = new NeuralNetwork((Parameters.numberOfSheep + Parameters.numberOfWolves) * 3, 3,
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

        public override void run(VisionContainer vc, AudioContainer ac)
        {
            vc.sortDistance();

            List<double> inputs = new List<double>((Parameters.numberOfSheep + Parameters.numberOfWolves) * 3);
            for (int i = 0; i < inputs.Count(); i++)
            {
                if (i < vc.size())
                {
                    Vector2 pos = vc.getSeenObject(i).position;

                    double magnitudeInput = pos.Length();

                    if (magnitudeInput == 0) ;

                }
                else
                {

                }
            }
        }


        public override void run(Vector2 goal)
        {
            throw new NotImplementedException();
        }
    }
}
