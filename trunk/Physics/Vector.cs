using System;

namespace Physics
{
    public class Vector
    {
        public float x, y;

        public Vector()
        {
            x = y = 0f;
        }

        public Vector( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public Vector( float x, float y )
        {
            this.x = x;
            this.y = y;
        }

        public float Abs()
        {
            return (float)Math.Sqrt( x * x + y * y );
        }

        public Vector Normalized()
        {
            return this / this.Abs();
        }

        public static Vector Projection( Vector v, Vector target )
        {
            return ((v * target) / (target.Abs() * target.Abs())) * target;
        }

        public static float ProjectionLength( Vector v, Vector target )
        {
            return (v * target) / target.Abs();
        }

        public static Vector operator +( Vector v1, Vector v2 )
        {
            return new Vector( v1.x + v2.x, v1.y + v2.y );
        }

        public static Vector operator -( Vector v1, Vector v2 )
        {
            return new Vector( v1.x - v2.x, v1.y - v2.y );
        }

        public static Vector operator *( float a, Vector v )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static Vector operator *( Vector v, float a )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static float operator *( Vector v1, Vector v2 )
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static Vector operator /( Vector v, float a )
        {
            return new Vector( v.x / a, v.y / a );
        }

        public override string ToString()
        {
            return this.x + ", " + this.y;
            //return "(" + this.x + ", " + this.y + ")";
        }
    }
}
