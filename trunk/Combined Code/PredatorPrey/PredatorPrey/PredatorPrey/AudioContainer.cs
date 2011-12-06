using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class AudioContainer
    {
        List<Vector> objectsHeard;

        public AudioContainer()
        {
            objectsHeard = new List<Vector>();
        }

        public void add(Vector v)
        {
            objectsHeard.Add(v);
        }

        public void reset()
        {
            objectsHeard.Clear();
        }

        public Vector getHeardObject(int index)
        {
            return objectsHeard[index];
        }
        public int size()
        {
            return objectsHeard.Count;
        }


    }
}