using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PredatorPrey
{
    abstract class Creature
    {

        public NeuralNetwork brain;
        public Boolean good;
        public int score;

        public int fitness { get; private set; }

        public int eatDuration;

        // the weights of the Creature's neural network
        public List<double> genes
        {
            get
            {
                return brain.getListOfWeights();
            }
            set
            {
                brain.replaceWeights(value);
            }
        }

        public Vector2 initPos;
        public Vector2 initDirection;
        public Vector2 position;
        public Vector2 velocity;

        public double rotation;

        public double leftSideSpeed;
        public double rightSideSpeed;

        public double hunger;
        public bool eating;
        public bool isAlive { get; protected set; }

        public Creature(Vector2 position)
        {
            //brain = new NeuralNetwork();
            rotation = 0;//Parameters.random.NextDouble() * Math.PI * 2;

            initPos = position;
            initDirection = new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));
            this.position = new Vector2(initPos.X, initPos.Y);
            this.velocity = new Vector2((float)(2 * Parameters.random.NextDouble() - 1), (float)(2 * Parameters.random.NextDouble() - 1));

            hunger = Parameters.startingHunger;
            eating = false;
            isAlive = true;
            fitness = 0;
            leftSideSpeed = 0;
            rightSideSpeed = 0;
        }

        public List<double> run(List<double> inputs)
        {
            return brain.run(inputs);
        }

        public void eat()
        {
            hunger += Parameters.eatingAddition;
            eatDuration = Parameters.eatTime;
            eating = true;
        }

        public void starve()
        {
            hunger -= Parameters.starvingSubtract;
        }

        public int genomeLength()
        {
            return brain.getTotalNumberOfWeights();
        }

        public void reset()
        {
            fitness = 0;
            position.X = initPos.X;
            position.Y = initPos.Y;
            velocity.X = 0;
            velocity.Y = 0;
            leftSideSpeed = 0;
            rightSideSpeed = 0;
        }

        public double getAngle()
        {
            return rotation;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void reposition()
        {
            position.X = Parameters.random.Next(Parameters.worldWidth);
            position.Y = Parameters.random.Next(Parameters.worldHeight);

            velocity = initDirection;
        }

        //this is to calculate the personal fitness function
        //but must be overrided in the prey and predetor classes
        public virtual double calculateFitness()
        {
            return 0;
        }

        public virtual void wrap(VisionContainer vc, AudioContainer ac)
        {
            // wrap around world if necessary
            if (position.X > Parameters.worldWidth)
                position.X = -Parameters.worldWidth;
            if (position.X < 0)
                position.X = Parameters.worldWidth + position.X;
            if (position.Y > Parameters.worldHeight)
                position.Y -= Parameters.worldHeight;
            if (position.Y < 0)
                position.Y = Parameters.worldHeight + position.Y;


        }
    }
}
