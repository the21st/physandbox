using System;

namespace Physics
{
    public class CollisionInfo
    {
        public Vector Location;
        public Board With;
        public byte Type;

        public CollisionInfo( Vector location, Board with, byte type )
        {
            Location = location;
            With = with;
            Type = type;
        }
    }
}
