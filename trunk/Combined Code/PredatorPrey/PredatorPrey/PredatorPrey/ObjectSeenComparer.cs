using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PredatorPrey
{
    class ObjectSeenComparer : IComparer<ObjectSeen>
    {
        Vector2 viewerPos;

        public ObjectSeenComparer(Vector2 viewerPos)
        {
            this.viewerPos = viewerPos;
        }

        int IComparer<ObjectSeen>.Compare(ObjectSeen ob1, ObjectSeen ob2)
        {
            //float distance1 = Vector2.Subtract(ob1.position, viewerPos).Length();
            //float distance2 = Vector2.Subtract(ob2.position, viewerPos).Length();

            float distance1 = Math.Abs(ob1.position.Length());
            float distance2 = Math.Abs(ob2.position.Length());

            if (distance1 < distance2)
                return -1;
            else if (distance1 > distance2)
                return 1;
            else
                return 0;
        }
    }
}
