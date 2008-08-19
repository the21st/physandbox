using System;
using System.Drawing;

namespace Physics
{
    public class Board : PhysicsObject
    {
        public Line line;
        public Color Clr;

        public Board( World world )
        {
            this.world = world;
            line = new Line();
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

            g.DrawLine( p, line.Start.x, line.Start.y, line.End.x, line.End.y );

            p.Dispose();
        }
    }
}
