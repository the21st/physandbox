using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Physics
{
    public abstract class PhysicsObject
    {
        protected World world;

        public abstract void Tick( float time );

        public abstract void Render();
    }
}
