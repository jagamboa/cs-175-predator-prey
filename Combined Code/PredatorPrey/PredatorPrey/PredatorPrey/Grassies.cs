using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Grassies : Creature
    {
        public Grassies(Vector2 pos) : base(pos)
        {
            position = pos;
        }
    }
}
