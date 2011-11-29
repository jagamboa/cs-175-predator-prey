/* Prey.CS
 * By Shawn Merrill
 * 80199820
 * 
 * Encapsulates all the prey behaviors in the simulation.
 * 
 * Pseudocode used as an example for flocking behavior in the code:
 */
/***************************************************************************************
* 
*    Title: Boids Pseudocode
*    Author: Conrad Parker
*    Date: Sept 2007
*    Availability: http://www.vergenet.net/~conrad/boids/pseudocode.html
*
***************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Simulation
{
    class Prey
    {
        static Random rand = new Random();

        Vector2 location;
        Vector2 velocity;

        float min_distance;
        float perception;
        float max_speed;
        bool left;

        float hunger;

        float[] v = new float[5];   // Weights for the various forces.
                                    // global to allow for some actions to overcome
                                    // other impulses

        public Prey(Vector2 loc, float max_spd)
        {
            this.velocity = random_vel();
            this.location = loc;
            left = true;
            this.max_speed = max_spd;
            this.perception = 100.0f;
            this.min_distance = 25.0f;

            hunger = 100.0f;

            v[0] = 1.0f;
            v[1] = .75f;
            v[2] = 1.5f;
            v[3] = 0.5f;
            v[4] = 0.5f;
        }

        Vector2 random_vel()
        {
            Vector2 velo = new Vector2(0.0f);
            velo.X = rand.Next(-5, 5);
            velo.Y = rand.Next(-5, 5);

            return velo;
        }

        Vector2 goToFood()
        {
            Vector2 direction = new Vector2(0.0f);
            float dist = Vector2.Distance(PreyFlock.food[0], this.location);
            int foodToHit = 0;
            for (int i = 1; i < PreyFlock.food.Length; i++)
            {   // Go to the nearest food
                float temp = Vector2.Distance(this.location, PreyFlock.food[i]);
                if (temp < dist)
                {
                    foodToHit = i;
                    dist = Vector2.Distance(this.location, PreyFlock.food[i]);
                }
            }
            return target(PreyFlock.food[foodToHit]);
        }

        Vector2 avoid_neighbors()
        {
            Vector2 avoiding_vec = new Vector2(0.0f);

            foreach (Prey p in PreyFlock.fodder)
            {
                if (Vector2.Distance(this.location, p.location) < this.min_distance)
                {
                    avoiding_vec = Vector2.Subtract(avoiding_vec, Vector2.Subtract(p.location, this.location));
                }
            }

            return avoiding_vec;
        }

        public Vector2 getLocation()
        {
            return this.location;
        }

        public Vector2 getVelocity()
        {
            return this.velocity;
        }

        Vector2 avoidPreds(ref PredatorPack pd)
        {
            Vector2 avoiding_pred = new Vector2(0.0f);

            foreach (Predator p in PredatorPack.predators)
            {
                float d = Vector2.Distance(this.location, p.getLocation());
                if (d < (perception / 1.7f))
                {
                    avoiding_pred = Vector2.Subtract(avoiding_pred, Vector2.Subtract(p.getLocation(), this.location));
                    // Add a bit of variation.
                }
            }
            return avoiding_pred;
        }

        Vector2 neighborhood_center()
        {
            Vector2 centering_vec = new Vector2(0.0f);
            int count = 0;
            foreach (Prey p in PreyFlock.fodder)
            {
                if (p != this)
                {
                    if (Vector2.Distance(this.location, p.location) < this.perception)
                    {
                        centering_vec = Vector2.Add(centering_vec, p.location);
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                centering_vec = Vector2.Divide(centering_vec, count);
                centering_vec = Vector2.Subtract(centering_vec, this.location);
                centering_vec = Vector2.Divide(centering_vec, 100.0f);
                return centering_vec;
            }
            else
            {
                return new Vector2(0.0f);
            }
        }

        Vector2 velocity_match()
        {
            Vector2 averaging_vec = new Vector2(0.0f);
            int count = 0;
            foreach (Prey p in PreyFlock.fodder)
            {
                if (this != p)
                {
                    if (Vector2.Distance(this.location, p.location) < this.perception)
                    {
                        averaging_vec = Vector2.Add(averaging_vec, p.velocity);
                        count++;
                    }
                }
            }
            if (count > 0)
            {
                averaging_vec = Vector2.Divide(averaging_vec, count);
                averaging_vec = Vector2.Subtract(averaging_vec, this.velocity);
                averaging_vec = Vector2.Divide(averaging_vec, 10);
                return averaging_vec;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        Vector2 target(Vector2 target)
        {
            Vector2 targeting_vec = new Vector2(0.0f);
            targeting_vec = Vector2.Subtract(target, this.location);
            if (targeting_vec.Length() > 0)
            {

                return Vector2.Divide(targeting_vec, 40f);
            }
            else
            {
                return Vector2.Zero;
            }
        }

        Vector2 wrap(Vector2 location)
        {
            float lim = 1050f;

            if (location.X > lim)
            {

                location.X = 0;
            }
            else if (location.X < -50)
            {
                location.X = lim;
            }

            if (location.Y > lim)
            {
                location.Y = 0;
            }
            else if (location.Y < -50)
            {
                location.Y = lim;
            }
            return location;
        }

        void eat()
        {
            for (int i = 0; i < PreyFlock.food.Length; i++)
            {
                if (Vector2.Distance(this.location, PreyFlock.food[i]) < 40f)
                {
                    hunger += 1f;
                    hunger = MathHelper.Min(hunger, 100f);
                }
            }
        }

        #region Update/Draw
        public void draw(SpriteBatch spriteBatch, Texture2D boid_texture)
        {
            spriteBatch.Draw(boid_texture, this.location, null, Color.Red, 0, Vector2.Zero, .5f, SpriteEffects.None, 0.0f);
        }

        public void update(ref PredatorPack pd)
        {
            Vector2 mod = new Vector2(0.0f);
            Vector2[] r = new Vector2[5];

            hunger -= .015f;

            if (hunger >= 85)
            {
                v[4] = 0;
            }
            else
            {
                v[4] = .05f + (100f - hunger) / 100f - .063f;
            }
            
            r[0] = Vector2.Multiply(avoid_neighbors(), v[0]);
            r[1] = Vector2.Multiply(velocity_match(), v[1]);
            r[2] = Vector2.Multiply(neighborhood_center(), v[2]);
            r[3] = Vector2.Multiply(avoidPreds(ref pd), v[3]);
            r[4] = Vector2.Multiply(goToFood(), v[4]);

            //r2 = new Vector2(0.0f);
            //r1 = new Vector2(0.0f);
            //r3 = new Vector2(0.0f);
            //r4 = new Vector2(0.0f);

            mod = Vector2.Add(mod, r[0]);
            mod = Vector2.Add(mod, r[1]);
            mod = Vector2.Add(mod, r[2]);
            mod = Vector2.Add(mod, r[3]);
            mod = Vector2.Add(mod, r[4]);
            this.velocity = Vector2.Add(this.velocity, mod);

            if (velocity.Length() > 0)
            {
                this.velocity = Vector2.Multiply(Vector2.Normalize(this.velocity), (max_speed * hunger) / 100.0f);
            }
            this.location = Vector2.Add(this.location, this.velocity);
            
            this.location = wrap(this.location);

            eat();
        }
        #endregion
    }
}