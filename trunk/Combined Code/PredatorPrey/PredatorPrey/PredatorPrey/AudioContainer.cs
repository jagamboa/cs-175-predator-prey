using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class AudioContainer
    {
        List<Vector2> objectsHeard;

        public AudioContainer()
        {
            objectsHeard = new List<Vector2>();
        }

        public void add(Vector2 v)
        {
            objectsHeard.Add(v);
        }

        public void reset()
        {
            objectsHeard.Clear();
        }

        public Vector2 getHeardObject(int index)
        {
            return objectsHeard[index];
        }
        public int size()
        {
            return objectsHeard.Count;
        }
    }
}