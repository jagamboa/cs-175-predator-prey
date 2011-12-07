using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PredatorPrey
{
    class Creature
    {

        public NeuralNetwork brain;

        public int fitness { get; private set; }

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
        public Vector2 direction;

        public double rotation;

        public double leftSideSpeed;
        public double rightSideSpeed;

        public double hunger;
        public bool eating;

        public Creature(Vector2 position)
        {
            brain = new NeuralNetwork();
            rotation = Parameters.random.NextDouble() * Math.PI * 2;

            initPos = position;
            initDirection = new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));

            this.position = new Vector2(initPos.X, initPos.Y);
            this.direction = new Vector2(initDirection.X, initDirection.Y);

            hunger = Parameters.startingHunger;
            eating = false;
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
            direction.X = initDirection.X;
            direction.Y = initDirection.Y;
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

            direction = initDirection;
        }

        //this is to calculate the personal fitness function
        //but must be overrided in the prey and predetor classes
        public virtual double calculateFitness()
        {
            return 0;
        }

        public void update(VisionContainer vc)
        {
            // calculate the rotation
            double rotationChange = leftSideSpeed - rightSideSpeed;
            double movementSpeed = leftSideSpeed + rightSideSpeed;

            if (movementSpeed > Parameters.maxMoveSpeed)
            {
                movementSpeed = Parameters.maxMoveSpeed;
            }

            // ensure the creature does not turn faster than is allowed
            if (rotationChange < -Parameters.maxRotation)
            {
                rotationChange = -Parameters.maxRotation;
            }
            else if (rotationChange > Parameters.maxRotation)
            {
                rotationChange = Parameters.maxRotation;
            }

            // update the direction the creature is facing

            rotation += rotationChange;

            direction.X = (float)-Math.Sin(rotation);
            direction.Y = (float)Math.Cos(rotation);

            double newrotation = getAngle();

            // update the creature's position
            position.X += (float)(direction.X * movementSpeed);
            position.Y += (float)(direction.Y * movementSpeed);

            // wrap around world if necessary
            if (position.X > Parameters.worldWidth)
                position.X = -Parameters.worldWidth;
            if (position.X < 0)
                position.X = Parameters.worldWidth + position.X;
            if (position.Y > Parameters.worldHeight)
                position.Y -= Parameters.worldHeight;
            if (position.Y < 0)
                position.Y = Parameters.worldHeight + position.Y;

            //update the hunger
            if (eating)
                eat();
            else
                starve();
        }
    }
}
