using System;
using System.Drawing;

namespace Physics
{
    /// <summary>
    /// Abstraktna trieda, ktora predpisuje niektore vlastnosti a metody
    /// vsetkym fyzikalnym objektom, ktore sa mozu vo World-e nachadzat.
    /// </summary>
    public abstract class PhysicsObject
    {
        protected World world;                  // svet, ktoremu prislucha dany objekt
        public long ID;                         // unikatne identifikacne cislo objektu


        protected const int maxChange = 15;     // sluzi na udanie rozpatia farieb, v ktorom kolise objekt, ak je oznaceny

        public Color Clr;                       // farba objektu
        protected bool selected = false;        // udava, ci je objekt prave oznaceny
        protected bool brighter = true;         // udava fazu zmeny farby oznaceneho objektu
        protected int phase = 0;                // udava fazu farby oznaceneho obejktu


        public abstract void Tick( float time );

        public abstract void Render();

        public void Select()
        {
            if (!selected)
            {
                selected = true;
                phase = 0;
                brighter = true;
            }
        }

        public void Deselect()
        {
            if (selected)
            {
                selected = false;
                phase = 0;
                brighter = true;
            }
        }
    }
}
