using System;
using System.Drawing;

namespace Physics
{
    public class Board : PhysicsObject
    {
        public Vector Start, End;
        public Color Clr;

        public Board( World world )
        {
            Start = new Vector();
            End = new Vector();
            Clr = Color.Black;
        }

        public override void Tick( float time )
        {
            // asi nic
        }

        public override void Render()
        {
            Graphics g = world.Graph;
            Pen p = new Pen( Clr );
            //p.Width = 1; //%%%

            g.DrawLine( p, Start.x, Start.y, End.x, End.y );

            p.Dispose();
        }
    }
}
