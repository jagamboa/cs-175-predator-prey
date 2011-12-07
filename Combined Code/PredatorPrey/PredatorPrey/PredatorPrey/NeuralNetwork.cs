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

            foreach (NeuronLayer neuronLayer in networkLayers)
            {
                neuronLayer.replaceWeights(newWeights);
            }
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

        // runs inputs through the neural network and returns a list of outputs
        public List<Vector2> run(List<Vector2> inputs)
        {
            if (inputs.Count != numberOfInputs)
            {
                throw new ArgumentException("The number of inputs passed to this neural network (" + inputs.Count +
                                            ") does not match the number of inputs this network accepts (" + numberOfInputs + ")");
            }

            List<Vector2> outputs = networkLayers[0].run(inputs);

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

    }
}
