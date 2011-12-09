using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PredatorPrey
{
    class GeneticAlgorithm
    {
        // the list of every genome in the population that this genetic
        // algorithm manages
        public List<Genome> population { get; private set; }

        // the generation number this population is currently on
        public int generationCount { get; private set; }

        // the lenght of the weight vector that defines a creature's genome
        public int genomeLength { get; private set; }

        // the sum of the fitness of every creature in this population
        public int populationFitness { get; private set; }

        // the highest fitness in the population
        public int bestFitness { get; private set; }

        // the lowest fitness in the population
        public int worstFitness { get; private set; }

        // the index of the fittest creature in the population
        private int fittestCreatureIndex;


        public GeneticAlgorithm(List<Creature> initialPopulation)
        {
            population = new List<Genome>(initialPopulation.Count);

            foreach (Creature creature in initialPopulation)
            {
                population.Add(new Genome(creature.genes));
            }

            generationCount = 0;
            genomeLength = initialPopulation[0].genomeLength();
            populationFitness = 0;
            bestFitness = 0;
            worstFitness = 0;
            fittestCreatureIndex = -1;
        }

        public double getAverageFitness()
        {
            return populationFitness / population.Count;
        }

        public void nextGeneration(List<Creature> oldPopulation)
        {
            if (oldPopulation.Count != population.Count)
            {
                throw new ArgumentException("The size of the old population (" + oldPopulation.Count +
                                            ") does not match the expected size of the old population (" + population.Count + ")");
            }

            updateFitness(oldPopulation);
            generationCount++;

            if (!Parameters.wulffiesLearn && oldPopulation[0] is Wulffies)
                return;
            if (!Parameters.fluffiesLearn && oldPopulation[0] is Fluffies)
                return;

            // breed new population
            for (int i = 0; i < oldPopulation.Count; i++)
            {
                int momIndex;
                int dadIndex;

                bool allBad = true;
                foreach (Creature c in oldPopulation)
                {
                    if (c.good)
                    {
                        allBad = false;
                        break;
                    }
                }

                if (allBad)
                {
                    selectParents(out momIndex, out dadIndex);
                    Genome baby = makeBaby(momIndex, dadIndex);

                    oldPopulation[i].genes = baby.genes;
                }
                else
                {
                    Boolean done = false;
                    while (!done)
                    {
                        selectParents(out momIndex, out dadIndex);
                        Genome baby = makeBaby(momIndex, dadIndex);

                        List<double> newWeights = baby.genes;
                        List<double> closest = new List<double>();
                        List<int> closestID = new List<int>();
                        for (int x = 0; x < oldPopulation.Count; x++)
                        {
                            List<double> xWeights = oldPopulation[x].genes;
                            double sum = 0;
                            for (int y = 0; y < newWeights.Count; y++)
                            {
                                sum = sum + ((newWeights[y] - xWeights[y]) * (newWeights[y] - xWeights[y]));
                            }
                            double distance = Math.Sqrt(sum);
                            if (closest.Count < Parameters.k)
                            {
                                closest.Add(distance);
                                closestID.Add(x);
                            }
                            else
                            {
                                foreach (double d in closest)
                                {
                                    if (distance < d)
                                    {
                                        closest.Remove(d);
                                        closest.Add(distance);
                                        closestID.Add(x);
                                        break;
                                    }
                                }
                            }
                        }
                        int countPlus = 0;
                        int countMinus = 0;
                        foreach (int a in closestID)
                        {
                            if (oldPopulation[a].good)
                            {
                                countPlus++;
                            }
                            else
                            {
                                countMinus++;
                            }
                        }
                        if (countPlus >= countMinus)
                        {
                            oldPopulation[i].genes = baby.genes;
                            done = true;
                        }
                    }
                }
            }
        }

        private void updateFitness(List<Creature> oldPopulation)
        {
            if (oldPopulation.Count != population.Count)
            {
                throw new ArgumentException("The size of the old population (" + oldPopulation.Count +
                                            ") does not match the expected size of the old population (" + population.Count + ")");
            }

            populationFitness = 0;
            fittestCreatureIndex = -1;

            for (int i = 0; i < population.Count; i++)
            {
                int fitness = oldPopulation[i].score;
                population[i].updateFitness(fitness);

                if (fittestCreatureIndex == -1)
                {
                    initializeFitness(fitness, i);
                }
                else if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    fittestCreatureIndex = i;
                }
                else if (fitness < worstFitness)
                {
                    worstFitness = fitness;
                }

                populationFitness += fitness;
            }

            if (populationFitness < 0)
            {
                populationFitness = 0;

                for (int i = 0; i < population.Count; i++)
                {
                    population[i].updateFitness(population[i].fitness - worstFitness);
                    populationFitness += population[i].fitness;
                }

                bestFitness -= worstFitness;
                worstFitness = 0;
            }
        }

        private void initializeFitness(int fitness, int index)
        {
            fittestCreatureIndex = index;
            bestFitness = fitness;
            worstFitness = fitness;
        }

        private void selectParents(out int momIndex, out int dadIndex)
        {
            // if no member of the population has any fitness rating
            if (populationFitness == 0)
            {
                momIndex = Parameters.random.Next(population.Count);
                dadIndex = Parameters.random.Next(population.Count);
                return;
            }

            int augmentedPopulationFitness = populationFitness + bestFitness * Parameters.numberOfFittestCopies;

            int roulette = Parameters.random.Next(augmentedPopulationFitness + 1);

            momIndex = 0;
            
            roulette -= population[momIndex].fitness;

            if (momIndex == fittestCreatureIndex)
                roulette -= population[momIndex].fitness * (Parameters.numberOfFittestCopies + 1);

            while (roulette > 0)
            {
                momIndex++;

                if (momIndex == fittestCreatureIndex)
                    roulette -= population[momIndex].fitness * (Parameters.numberOfFittestCopies + 1);
                else
                    roulette -= population[momIndex].fitness;
            }

            roulette = Parameters.random.Next(augmentedPopulationFitness + 1);

            dadIndex = 0;
            roulette -= population[dadIndex].fitness;

            if (dadIndex == fittestCreatureIndex)
                roulette -= population[dadIndex].fitness * (Parameters.numberOfFittestCopies + 1);

            while (roulette > 0)
            {
                dadIndex++;

                if (dadIndex == fittestCreatureIndex)
                    roulette -= population[dadIndex].fitness * (Parameters.numberOfFittestCopies + 1);
                else
                    roulette -= population[dadIndex].fitness;
            }

            //// bias towards fittest creature
            //if (Parameters.random.NextDouble() < Parameters.chanceFittestTakesOver)
            //{
            //    momIndex = fittestCreatureIndex;
            //}

            //if (Parameters.random.NextDouble() < Parameters.chanceFittestTakesOver)
            //{
            //    dadIndex = fittestCreatureIndex;
            //}

            //do
            //{
            //    roulette = Parameters.random.Next(populationFitness + 1);

            //    dadIndex = 0;
            //    roulette -= population[dadIndex].fitness;
            //    while (roulette > 0)
            //    {
            //        dadIndex++;
            //        roulette -= population[dadIndex].fitness;
            //    }
            //}
            //while (dadIndex == momIndex);
        }

        private Genome makeBaby(int momIndex, int dadIndex)
        {
            List<double> babyGenes = new List<double>(genomeLength);

            // no crossover
            if (Parameters.random.NextDouble() > Parameters.crossoverRate)
            {
                babyGenes = mutate(population[momIndex].genes);

                return new Genome(babyGenes);
            }
            // cross genes over
            else
            {
                int crossoverPoint = Parameters.random.Next(genomeLength + 1);
                babyGenes = population[momIndex].genes.GetRange(0, crossoverPoint);
                babyGenes.AddRange(population[dadIndex].genes.GetRange(crossoverPoint, population[dadIndex].genes.Count - crossoverPoint));

                babyGenes = mutate(babyGenes);

                return new Genome(babyGenes);
            }
        }

        private List<double> mutate(List<double> genes)
        {
            List<double> newGenes = new List<double>(genes.Count);

            foreach (double gene in genes)
            {
                // no mutation
                if (Parameters.random.NextDouble() > Parameters.mutationRate)
                {
                    newGenes.Add(gene);
                }
                // mutation occurs
                else
                {
                    newGenes.Add((2 * Parameters.random.NextDouble() - 1) * Parameters.weightRange);
                }
            }

            return newGenes;
        }
    }
}
