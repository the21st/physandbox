using System;
using System.Drawing;

namespace Physics
{
    public abstract class PhysicsObject
    {
        protected World world;


        protected const int maxChange = 15;

        public Color Clr;
        protected Color originalClr;
        protected bool selected = false;
        protected bool brighter = true;
        protected int phase = 0;

        public abstract void Tick( float time );

        public abstract void Render();

        public void Select()
        {
            if (!selected)
            {
                selected = true;
                originalClr = Clr;
            }
        }

        public void Deselect()
        {
            if (selected)
            {
                selected = false;
                Clr = originalClr;
            }
        }
    }
}
