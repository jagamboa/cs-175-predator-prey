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
    class PredatorPack
    {
        public static List<Predator> predators = new List<Predator>();
        public static List<Prey> targets = new List<Prey>();

        public void addPred(Predator p)
        {
            predators.Add(p);
        }

        public void draw_all(SpriteBatch spriteBatch, Texture2D pred_texture)
        {
            foreach (Predator p in predators)
            {
                p.draw(spriteBatch, pred_texture);
            }
        }

        public void update_all(ref PreyFlock pf)
        {
            targets.Clear();
            foreach (Predator p in predators)
            {
                p.update(ref pf);
            }
        }
    }
}
