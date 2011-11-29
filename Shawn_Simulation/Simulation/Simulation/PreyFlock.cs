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
    class PreyFlock
    {
        public static List<Prey> fodder = new List<Prey>();
        public static Vector2[] food = new Vector2[3];

        public PreyFlock()
        {
            food[0] = new Vector2(250f, 250f);
            food[1] = new Vector2(800f, 800f);
            food[2] = new Vector2(800f, 350f);
        }

        public void addPrey(Prey p)
        {
            fodder.Add(p);
        }

        public void eatPrey(Prey p)
        {
            fodder.Remove(p);
        }

        public void draw_all(SpriteBatch spriteBatch, Texture2D prey_texture)
        {
            for (int i = 0; i < food.Length; i++)
            {
                spriteBatch.Draw(prey_texture, food[i], null, Color.CornflowerBlue, 0, Vector2.Zero, .5f, SpriteEffects.None, 1f);
            }
            
            foreach (Prey p in fodder)
            {
                p.draw(spriteBatch, prey_texture);
            }
        }

        public void update_all(ref PredatorPack pd)
        {
            foreach (Prey p in fodder)
            {
                p.update(ref pd);
            }
        }
    }
}
