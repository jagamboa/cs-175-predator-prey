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

        public double fitness;
        public bool canEat;
        public int eatDuration;
        public int dontEatDuration;

        protected Vector2 prevPos;
        protected Vector2 prevVel;
        protected Vector2 prevAcc;
        protected float prevAccMag;
        protected VisionContainer prevVision;

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
            this.velocity = Vector2.Zero;// new Vector2((float)(2 * Parameters.random.NextDouble() - 1), (float)(2 * Parameters.random.NextDouble() - 1));

            fitness = Parameters.initFitness;
            hunger = Parameters.startingHunger;
            eating = false;
            canEat = true;
            isAlive = true;
            score = 0;
            good = false;
        }

        public List<double> run(List<double> inputs)
        {
            return brain.run(inputs);
        }

        public virtual void eat()
        {
            hunger += Parameters.eating;
            eating = true;

            if (hunger < 0)
            {
                eating = false;
                hunger = 0;
            }
        }

        public void starve()
        {
            hunger += Parameters.starving;

            if (hunger > Parameters.maxHunger)
                hunger = Parameters.maxHunger;
        }

        public int genomeLength()
        {
            return brain.getTotalNumberOfWeights();
        }

        public void reset()
        {
            position.X = initPos.X;
            position.Y = initPos.Y;
            velocity = new Vector2((float)(2 * Parameters.random.NextDouble() - 1), (float)(2 * Parameters.random.NextDouble() - 1));
            fitness = Parameters.initFitness;
            hunger = Parameters.startingHunger;
            eating = false;
            canEat = true;
            isAlive = true;
            score = 0;
            good = false;
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
        public virtual double calculateFitness(VisionContainer vc)
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

        public virtual void die()
        {
            isAlive = false;
        }
    }
}
