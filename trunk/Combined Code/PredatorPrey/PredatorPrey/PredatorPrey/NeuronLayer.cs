using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class NeuronLayer
    {
        public int numberOfNeurons { get; private set; }

        public List<Neuron> neurons { get; private set; }

        public NeuronLayer(int numberOfNeurons, int numberOfInputsPerNeuron)
        {
            this.numberOfNeurons = numberOfNeurons;
            neurons = new List<Neuron>(numberOfNeurons);

            for (int i = 0; i < numberOfNeurons; i++)
            {
                neurons.Add(new Neuron(numberOfInputsPerNeuron));
            }
        }

        // replaces the weights of each neuron in this layer with new ones
        public void replaceWeights(List<double> newWeights)
        {
            foreach (Neuron neuron in neurons)
            {
                neuron.replaceWeights(newWeights);
            }
        }

        // runs input through this layer of neurons and returns a list of
        // all the outputs
        public List<double> run(List<double> inputs)
        {
            List<double> output = new List<double>(numberOfNeurons);

            foreach (Neuron neuron in neurons)
            {
                output.Add(neuron.run(inputs));
            }

            return output;
        }

        // runs input through this layer of neurons and returns a list of
        // all the outputs
        public List<Vector2> run(List<Vector2> inputs)
        {
            List<Vector2> output = new List<Vector2>(numberOfNeurons);

            foreach (Neuron neuron in neurons)
            {
                output.Add(neuron.run(inputs));
            }

            return output;
        }
    }
}
