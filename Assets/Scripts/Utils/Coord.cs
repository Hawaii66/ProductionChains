using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains
{
    [System.Serializable]
    public class Coord
    {
        public int x;
        public int y; // Up down
        public int z;

        public Coord(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Coord(Vector3 pos)
        {
            x = Mathf.RoundToInt(pos.x);
            y = Mathf.RoundToInt(pos.y);
            z = Mathf.RoundToInt(pos.z);
        }

        public static Coord FloorToCoord(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            return new Coord(x, y, z);
        }

        public static Coord Zero()
        {
            return new Coord(0, 0, 0);
        }

        public static Coord Invalid()
        {
            return new Coord(-1,-1,-1);
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Coord operator /(Coord a, Coord b)
        {
            return new Coord(b.x != 0 ? a.x / b.x : 0, b.y != 0 ? a.y / b.y : 0, b.z != 0 ? a.z / b.z : 0);
        }

        public static Coord operator /(Coord a, int div)
        {
            return new Coord(a.x / div, a.y / div, a.z / div);
        }

        public static Coord operator *(Coord a, int mult)
        {
            return new Coord(a.x * mult, a.y * mult, a.z * mult);
        }

        public static Coord operator *(Coord a, Coord b)
        {
            return new Coord(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static implicit operator Vector3(Coord v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static bool operator ==(Coord a, Coord b)
        {
            return a.x == b.x && a.z == b.z && a.y == b.y;
        }

        public static bool operator !=(Coord a, Coord b)
        {
            return a.x != b.x || a.z != b.z || a.y != b.y;
        }

        public override string ToString()
        {
            return "(" + x + " ; " + y + " ; " + z + ")";
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        //From seblauge WTF
        public override bool Equals(object other)
        {
            return (Coord)other == this;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}