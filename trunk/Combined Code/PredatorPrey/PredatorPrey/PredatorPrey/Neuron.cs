using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Neuron
    {
        public int numberOfInputs { get; private set; }

        public List<double> weights { get; private set; }

        public double output { get; private set; }

        public Neuron(int numberOfInputs)
        {
            this.numberOfInputs = numberOfInputs;
            weights = new List<double>(numberOfInputs);

            for (int i = 0; i < numberOfInputs + 1; i++)
            {
                weights.Add((2 * Parameters.random.NextDouble() - 1) * Parameters.weightRange);
            }
        }

        // replaces the weights of the neuron with new ones
        public void replaceWeights(List<double> newWeights)
        {
            for (int i = 0; i < numberOfInputs + 1; i++)
            {
                weights[i] = newWeights[i];
            }

            newWeights.RemoveRange(0, numberOfInputs + 1);
        }

        // updates the weights based on the given delta
        public void updateWeights(double delta)
        {
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] = weights[i] + delta;
            }
        }

        // runs input through this neuron and returns the output signal
        public double run(List<double> inputs)
        {
            if (inputs.Count != numberOfInputs)
            {
                throw new ArgumentException("The number of inputs passed to this neuron (" + inputs.Count +
                                            ") does not equal the number of inputs this neuron accepts (" + numberOfInputs + ")");
            }

            double sumOfInputsAndWeights = 0;

            // each input and weight
            for (int i = 0; i < numberOfInputs; i++)
            {
                sumOfInputsAndWeights += weights[i] * inputs[i];
            }

            // bias
            sumOfInputsAndWeights += weights[weights.Count - 1] * Parameters.bias;

            //return sigmoidFunction(sumOfInputsAndWeights);

            output = sumOfInputsAndWeights;
            return sumOfInputsAndWeights;
        }

        //// runs input through this neuron and returns the output signal
        //public Vector2 run(List<Vector2> inputs)
        //{
        //    if (inputs.Count != numberOfInputs)
        //    {
        //        throw new ArgumentException("The number of inputs passed to this neuron (" + inputs.Count +
        //                                    ") does not equal the number of inputs this neuron accepts (" + numberOfInputs + ")");
        //    }

        //    Vector2 sumOfInputsAndWeights = new Vector2();

        //    // each input and weight
        //    for (int i = 0; i < numberOfInputs; i++)
        //    {
        //        Vector2.Add(sumOfInputsAndWeights, Vector2.Multiply(inputs[i], (float)weights[i]));
        //    }

        //    // bias
        //    Vector2.Add(sumOfInputsAndWeights,
        //        Vector2.Multiply(new Vector2((float)Parameters.bias, (float)Parameters.bias), (float)weights[weights.Count - 1]));

        //    //return sigmoidFunction(sumOfInputsAndWeights);
            
        //    return sumOfInputsAndWeights;
        //}

        // the sigmoid function used to determine when and how strong this
        // neuron fires
        private double sigmoidFunction(double activation)
        {
            return 1 / (1 + Math.Pow(Math.E, -activation / Parameters.responseCurve));
        }
    }
}
