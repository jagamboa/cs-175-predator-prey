using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }

    class ObjectSeen
    {
        public Classification type;
        Vector position;
        Vector direction;

        public ObjectSeen(Classification type, Vector position, Vector direction)
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
