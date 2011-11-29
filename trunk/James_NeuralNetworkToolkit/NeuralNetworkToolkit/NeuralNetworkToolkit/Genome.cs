using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetworkToolkit
{
    class Genome
    {
        public int fitness { get; private set; }

        public List<double> genes { get; private set; }

        public Genome(List<double> genes)
        {
            this.genes = genes;
            fitness = 0;
        }

        public void replaceGenes(List<double> newGenes)
        {
            if (newGenes.Count != genes.Count)
            {
                throw new ArgumentException("The number of genes in the new genome (" + newGenes.Count +
                                                ") does not equal the number of genes in the old genome (" + genes.Count + ")");
            }

            genes = newGenes;
        }

        public void updateFitness(int fitness)
        {
            this.fitness = fitness;
        }
    }
}
