using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class Ears
    {
        AudioContainer list;
        public Ears(Creature creat, Creature[] otherCreatures)
        {
            list = new AudioContainer();
            for (int i = 0; i < otherCreatures.GetLength(0); i++)
            {

            }
        }
    }
}
