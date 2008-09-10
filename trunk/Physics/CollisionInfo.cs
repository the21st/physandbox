using System;

namespace Physics
{
    /// <summary>
    /// Trieda sluziaca na uchovanie informacii o konkretnej kolizii gule s doskou. 
    /// Je to len kolekcia troch typov premennych, a teda pravdepodobne stacilo pouzit typ struct, namiesto class. 
    /// Ale autor tejto kniznice nema rad struct-y, lebo ich nevie dobre pouzivat, takze je pouzita trieda.
    /// </summary>
    public class CollisionInfo
    {
        public Vector Location; // suradnice stredu gule v momente narazu do dosky
        public Board With;      // pointer na dosku, ktorej sa kolizia tyka
        public byte Type;       // nadobuda hodnoty 1 alebo 2; vysvetlene v metode Sphere.move

        public CollisionInfo( Vector location, Board with, byte type )
        {
            Location = location;
            With = with;
            Type = type;
        }
    }
}
