using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PredatorPrey
{
    class Neuron
    {
        public int numberOfInputs { get; private set; }

        public List<double> weights { get; private set; }

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
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = newWeights[i];
            }

            newWeights.RemoveRange(0, numberOfInputs);
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

            return sigmoidFunction(sumOfInputsAndWeights);
        }

        // the sigmoid function used to determine when and how strong this
        // neuron fires
        private double sigmoidFunction(double activation)
        {
            return 1 / (1 + Math.Pow(Math.E, -activation / Parameters.responseCurve));
        }
    }
}
