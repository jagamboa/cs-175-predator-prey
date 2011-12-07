using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class VisionContainer
    {
        List<ObjectSeen> objectsSeen;

        public VisionContainer()
        {
            objectsSeen = new List<ObjectSeen>();
        }

        public void add(ObjectSeen objectSeen)
        {
            objectsSeen.Add(objectSeen);
        }

        public void reset()
        {
            objectsSeen.Clear();
        }
        public ObjectSeen getSeenObject(int index)
        {
            return objectsSeen[index];
        }
        public int size()
        {
            return objectsSeen.Count;
        }
    }

    class ObjectSeen
    {
        public Classification type;
        public Vector2 position;
        public Vector2 direction;

        public ObjectSeen(Classification type, Vector2 position, Vector2 direction)
        {
            this.type = type;
            this.position = position;
            this.direction = direction;
        }
    }

    enum Classification
    {
        Predator,
        Prey,
        Food,
        Unknown
    };
}
