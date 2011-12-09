using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class NeuralNetwork
    {
        private int numberOfInputs;
        private int numberOfOutputs;
        private int numberOfHiddenLayers;
        private int numberOfNeuronsPerHiddenLayer;
        private int totalNumberOfWeights;
        private List<NeuronLayer> networkLayers;

        public NeuralNetwork(int numberOfInputs, int numberOfOutputs, int numberOfHiddenLayers, int numberOfNeuronsPerHiddenLayer)
        {
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;
            this.numberOfHiddenLayers = numberOfHiddenLayers;
            this.numberOfNeuronsPerHiddenLayer = numberOfNeuronsPerHiddenLayer;
            totalNumberOfWeights = 0;

            networkLayers = new List<NeuronLayer>(numberOfHiddenLayers + 1);

            if (numberOfHiddenLayers == 0)
            {
                networkLayers.Add(new NeuronLayer(numberOfOutputs, numberOfInputs));
                totalNumberOfWeights += numberOfOutputs * (numberOfInputs + 1);
            }
            else
            {
                // add first layer that accepts the inputs
                networkLayers.Add(new NeuronLayer(numberOfNeuronsPerHiddenLayer, numberOfInputs));
                totalNumberOfWeights += numberOfNeuronsPerHiddenLayer * (numberOfInputs + 1);

                // add the rest of the hidden layers
                for (int i = 1; i < numberOfHiddenLayers; i++)
                {
                    networkLayers.Add(new NeuronLayer(numberOfNeuronsPerHiddenLayer, numberOfNeuronsPerHiddenLayer));
                    totalNumberOfWeights += numberOfNeuronsPerHiddenLayer * (numberOfNeuronsPerHiddenLayer + 1);
                }

                // add the output layer
                networkLayers.Add(new NeuronLayer(numberOfOutputs, numberOfNeuronsPerHiddenLayer));
                totalNumberOfWeights += numberOfOutputs * (numberOfNeuronsPerHiddenLayer + 1);
            }
        }

        // returns a list of all the weights in the neural network starting from the
        // layer that receives the inputs to the layer that produces the outputs
        public List<double> getListOfWeights()
        {
            List<double> weights = new List<double>();

            foreach (NeuronLayer neuronLayer in networkLayers)
                foreach (Neuron neuron in neuronLayer.neurons)
                {
                    weights.AddRange(neuron.weights);
                }

            return weights;
        }

        public int getTotalNumberOfWeights()
        {
            return totalNumberOfWeights;
        }

        // replaces the weights in this network with new ones
        public void replaceWeights(List<double> newWeights)
        {
            if (newWeights.Count != totalNumberOfWeights)
            {
                throw new ArgumentException("The number of weights passed in to replace the old weights" +
                            "is not equal to the total number of weights in this neural network!" +
                            "\nold weight count: " + totalNumberOfWeights + ", new weight count: " + newWeights.Count);
            }

            for (int i = 0; i < networkLayers.Count; i++)
            {
                networkLayers[i].replaceWeights(newWeights);
            }
        }

        // takes in a target output and propagates error throughout the network
        // outputs the error propagated from the input layer
        public List<double> updateWeights(List<double> targetOutput)
        {
            if (targetOutput.Count != numberOfOutputs)
            {
                throw new ArgumentException("The number of target outputs passed in to update the weights" +
                            "is not equal to the total number of outputs in this neural network!" +
                            "\nold weight count: " + numberOfOutputs + ", new weight count: " + targetOutput.Count);
            }

            // construct delta lists
            List<List<double>> delta = new List<List<double>>(numberOfOutputs);
            for (int i = 0; i < numberOfHiddenLayers; i++)
            {
                delta.Add(new List<double>(numberOfNeuronsPerHiddenLayer));

                for (int j = 0; j < numberOfNeuronsPerHiddenLayer; j++)
                {
                    delta[i].Add(0);
                }
            }
            delta.Add(new List<double>(numberOfOutputs));
            for (int j = 0; j < numberOfOutputs; j++)
            {
                delta[numberOfHiddenLayers].Add(0);
            }

            // compute delta list for output layer
            for (int i = 0; i < targetOutput.Count; i++)
            {
                List<double> outputs = networkLayers[numberOfHiddenLayers].getOutputs();

                delta[numberOfHiddenLayers][i] = (targetOutput[i] - outputs[i]) / Parameters.learningRate;
            }

            // compute delta for hidden layers
            for (int i = numberOfHiddenLayers - 1; i >= 0; i--)
            {
                for (int j = 0; j < networkLayers[i].numberOfNeurons; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < networkLayers[i + 1].numberOfNeurons; k++)
                    {
                        sum += delta[i + 1][k] * networkLayers[i + 1].getWeights(k)[j];
                    }
                    delta[i][j] = sum;
                }
            }

            // update weights
            for (int i = 0; i < numberOfHiddenLayers + 1; i++)
            {
                networkLayers[i].updateWeights(delta[i]);
            }

            // normalize weights
            List<double> weights = getListOfWeights();

            double max = double.MinValue;

            for (int i = 0; i < weights.Count; i++)
            {
                if (Math.Abs(weights[i]) > max)
                {
                    max = Math.Abs(weights[i]);
                }
            }

            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] = weights[i] / max;
            }

            replaceWeights(weights);

            return delta[0];
        }




        // runs inputs through the neural network and returns a list of outputs
        public List<double> run(List<double> inputs)
        {
            if (inputs.Count != numberOfInputs)
            {
                throw new ArgumentException("The number of inputs passed to this neural network (" + inputs.Count +
                                            ") does not match the number of inputs this network accepts (" + numberOfInputs + ")");
            }

            List<double> outputs = networkLayers[0].run(inputs);

            for (int i = 1; i < numberOfHiddenLayers + 1; i++)
            {
                outputs = networkLayers[i].run(outputs);
            }

            if (outputs.Count != numberOfOutputs)
            {
                throw new Exception("The number of outputs returned by this neural network (" + outputs.Count +
                                    ") does not match the number of outputs this network should return (" + numberOfOutputs + ")");
            }

            return outputs;
        }

        //// runs inputs through the neural network and returns a list of outputs
        //public List<Vector2> run(List<Vector2> inputs)
        //{
        //    if (inputs.Count != numberOfInputs)
        //    {
        //        throw new ArgumentException("The number of inputs passed to this neural network (" + inputs.Count +
        //                                    ") does not match the number of inputs this network accepts (" + numberOfInputs + ")");
        //    }

        //    List<Vector2> outputs = networkLayers[0].run(inputs);

        //    for (int i = 1; i < numberOfHiddenLayers + 1; i++)
        //    {
        //        outputs = networkLayers[i].run(outputs);
        //    }

        //    if (outputs.Count != numberOfOutputs)
        //    {
        //        throw new Exception("The number of outputs returned by this neural network (" + outputs.Count +
        //                            ") does not match the number of outputs this network should return (" + numberOfOutputs + ")");
        //    }

        //    return outputs;
        //}

    }
}
