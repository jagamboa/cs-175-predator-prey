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
            for (int i = 0; i < neurons.Count; i++)
            {
                neurons[i].replaceWeights(newWeights);
            }
        }

        // updates the weights given a list of delta values
        public void updateWeights(List<double> delta)
        {
            for (int i = 0; i < numberOfNeurons; i++)
            {
                neurons[i].updateWeights(delta[i]);
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

        // returns the outputs from the pervious run of this layer
        public List<double> getOutputs()
        {
            List<double> output = new List<double>(neurons.Count);

            for (int i = 0; i < neurons.Count; i++)
            {
                output.Add(neurons[i].output);
            }

            return output;
        }

        // returns the weight of the neuron specified by "index"
        public List<double> getWeights(int index)
        {
            return neurons[index].weights;
        }

        //// runs input through this layer of neurons and returns a list of
        //// all the outputs
        //public List<Vector2> run(List<Vector2> inputs)
        //{
        //    List<Vector2> output = new List<Vector2>(numberOfNeurons);

        //    foreach (Neuron neuron in neurons)
        //    {
        //        output.Add(neuron.run(inputs));
        //    }

        //    return output;
        //}
    }
}
