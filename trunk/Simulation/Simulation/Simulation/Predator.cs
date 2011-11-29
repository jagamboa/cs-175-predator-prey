/*Predator.CS
 * By Shawn Merrill
 * 80199820
 * 
 * Encapsulates the behavior of the predator agents in the simulation.
 * 
 **/

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
    class Predator
    {
        static Random rand = new Random();
        Vector2 location;
        Vector2 velocity;

        float perception;
        float max_speed;
        float min_distance;

        float hunger;

        float[] v = new float[3];

        public Predator(Vector2 loc, float max_spd)
        {
            this.velocity = random_vel();
            this.location = loc;

            this.max_speed = max_spd;
            this.perception = 200.0f;
            this.min_distance = 40f;
            v[0] = 1.0f;
            v[1] = 0.5f;
            v[2] = 0.3f;
            hunger = 100f;
        }

        Vector2 random_vel()
        {
            Vector2 velo = new Vector2(0.0f);
            velo.X = rand.Next(-5, 5);
            velo.Y = rand.Next(-5, 5);

            return velo;
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

        Vector2 target(Vector2 target)
        {
            Vector2 targeting_vec = new Vector2(0.0f);
            targeting_vec = Vector2.Subtract(target, this.location);
            return targeting_vec;
        }

        Vector2 avoidance()
        {
            Vector2 avoidance = new Vector2(0.0f);
            foreach (Predator p in PredatorPack.predators)
            {
                if (Vector2.Distance(this.location, p.location) < min_distance)
                {
                    avoidance = Vector2.Subtract(avoidance, Vector2.Subtract(p.getLocation(), this.location));
                }
            }

            return avoidance;
        }

        public Vector2 getLocation()
        {
            return this.location;
        }

        void eatPrey(ref PreyFlock pf)
        {
            foreach (Prey p in PreyFlock.fodder)
            {
                // Only eat one prey in the area
                if (Vector2.Distance(this.location, p.getLocation()) < 15)
                {
                    //Console.WriteLine("Om nom!");
                    pf.eatPrey(p);
                    hunger = MathHelper.Min(100f, hunger + 5f);
                    return;
                }
            }
        }

        Vector2 align()     //Give the predators a slight emphasis to align themselves
        {
            Vector2 align = new Vector2(0.0f);
            foreach (Predator p in PredatorPack.predators)
            {
                if (Vector2.Distance(this.location, p.location) < perception)
                {
                    Vector2 perp = new Vector2(p.velocity.Y * -1.0f, p.velocity.X); // Perpendicular to their current path
                    Vector2 point1 = p.location + perp;
                    Vector2 point2 = p.location - perp;
                    if (Vector2.Distance(this.location, point1) < Vector2.Distance(this.location, point2))
                    {
                        align = target(point1);
                    }
                    else
                    {
                        align = target(point2);
                    }
                }
            }
            return align;
        }

        Vector2 towardsNearestPrey(ref PreyFlock pf)
        {
            Vector2 goal = new Vector2(0.0f);
            if (PreyFlock.fodder.Count > 0)
            {
                Prey nearest = PreyFlock.fodder.ElementAt(0);
                float distance = Vector2.Distance(this.location, nearest.getLocation());
                // Find the closest prey
                foreach (Prey p in PreyFlock.fodder)
                {
                    if ((Vector2.Distance(this.location, p.getLocation()) < distance) && !PredatorPack.targets.Contains(p))
                    {
                        nearest = p;
                        distance = Vector2.Distance(this.location, nearest.getLocation());
                    }
                }
                if (distance > perception)
                {
                    // If No Prey in range, slow down
                    return Vector2.Multiply(this.velocity, -.5f);
                }
                else
                {
                    // Go towards that prey
                    PredatorPack.targets.Add(nearest);
                    // Give the predators a bit of a leading trajectory
                    return target(Vector2.Add(nearest.getLocation(),(nearest.getVelocity() * 2f)));
                }
            }
            else
            {
                return Vector2.Multiply(this.velocity, -.5f);
            }
        }

        #region Update/Draw
        public void draw(SpriteBatch spriteBatch, Texture2D boid_texture)
        {
            spriteBatch.Draw(boid_texture, this.location, null, Color.Yellow, 0, Vector2.Zero, .55f, SpriteEffects.None, 0.0f);
        }

        public void update(ref PreyFlock pf)
        {
            Vector2 mod = new Vector2(0.0f);
            Vector2[] r = new Vector2[3];
            r[0] = towardsNearestPrey(ref pf) * v[0];
            r[1] = avoidance() * v[1];
            r[2] = align() * v[2];

            mod = Vector2.Add(mod, r[0]);
            mod = Vector2.Add(mod, r[1]);
            mod = Vector2.Add(mod, r[2]);

            hunger -= .015f;

            this.velocity = Vector2.Add(this.velocity, mod);
            if (velocity.Length() > 0)
            {
                this.velocity = Vector2.Multiply(Vector2.Normalize(this.velocity), (max_speed * 100f) / hunger);
            }
            this.location = Vector2.Add(this.location, this.velocity);

            this.location = wrap(this.location);


            eatPrey(ref pf);
        }
        #endregion
    }
}
