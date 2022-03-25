using System.Collections;
using UnityEngine;

namespace ProduktionChains.Utilities
{
    public static class Utils
    {
        public static Coord[] offsets = new Coord[]
        {
            new Coord(0,0,1),
            new Coord(0,0,-1),
            new Coord(1,0,0),
            new Coord(-1,0,0),
        };

        public static Coord[] offsetsWithCenter = new Coord[]
        {
            new Coord(0,0,1),
            new Coord(0,0,-1),
            new Coord(1,0,0),
            new Coord(-1,0,0),
            new Coord(0,0,0),
        };

        public static bool IsOppositeDirection(CartesianDir a, CartesianDir b)
        {
            if(a == CartesianDir.Left && b == CartesianDir.Right) { return true; }
            if (a == CartesianDir.Right && b == CartesianDir.Left) { return true; }
            if (a == CartesianDir.Up && b == CartesianDir.Down) { return true; }
            if (a == CartesianDir.Down && b == CartesianDir.Up) { return true; }

            return false;
        }

        public static int GetRotation(CartesianDir a)
        {
            if(a == CartesianDir.Left) { return 270; }
            if(a == CartesianDir.Right) { return 90; }
            if(a == CartesianDir.Up) { return 0; }
            if(a == CartesianDir.Down) { return 180; }
            return 0;
        }

        public static Coord GetCoord(CartesianDir a)
        {
            if(a == CartesianDir.Left) { return new Coord(-1, 0, 0); }
            if(a == CartesianDir.Right) { return new Coord(1, 0, 0); }
            if(a == CartesianDir.Up) { return new Coord(0, 0, 1); }
            if(a == CartesianDir.Down) { return new Coord(0, 0, -1); }
            return Coord.Zero();
        }

        public static Coord GetOppositeCoord(CartesianDir a)
        {
            return GetCoord(a) * -1;
        }

        public static Vector3 RotateVector(this Vector3 v, float rot)
        {
            return Quaternion.AngleAxis(rot, Vector3.up) * v;
        }

        public static Coord RotateCoord(this Coord c, float rot)
        {
            return c.ToVector3().RotateVector(rot).ToCoord();
        }

        public static Coord ToCoord(this Vector3 v)
        {
            return new Coord(v);
        }

        public static Coord ClampTo01(this Coord c)
        {
            if(c.x > 1) { c.x = 1; }
            if(c.x < -1) { c.x = -1; }
            if (c.y > 1) { c.y = 1; }
            if (c.y < -1) { c.y = -1; }
            if (c.z > 1) { c.z = 1; }
            if (c.z < -1) { c.z = -1; }
            return c;
        }

        public static Coord GetGridPos(this Vector3 pos)
        {
            float x = pos.x - GridManager.gridSize / 2;
            float y = pos.y - GridManager.gridSize / 2;
            float z = pos.z - GridManager.gridSize / 2;

            return new Coord(Mathf.CeilToInt(x / GridManager.gridSize), Mathf.CeilToInt(y / GridManager.gridSize), Mathf.CeilToInt(z / GridManager.gridSize));
        }

        public static Coord[] RotateCoords(this Coord[] coords, float rot)
        {
            Coord[] rotated = new Coord[coords.Length];
            int i = 0;
            foreach(Coord c in coords)
            {
                rotated[i] = (c.RotateCoord(rot));
                i += 1;
            }

            return rotated;
        }

        public static Vector3[] RotateVectors(this Vector3[] coords, float rot)
        {
            Vector3[] rotated = new Vector3[coords.Length];
            int i = 0;
            foreach (Vector3 c in coords)
            {
                rotated[i] = (c.RotateVector(rot));
                i += 1;
            }

            return rotated;
        }

        public static Vector3[] AddVector3(this Vector3[] coords, Vector3 add)
        {
            Vector3[] rotated = new Vector3[coords.Length];
            int i = 0;
            foreach (Vector3 c in coords)
            {
                rotated[i] = c + add;
                i += 1;
            }

            return rotated;
        }

        public static float Remap(this float value, float input1, float output1, float input2, float output2)
        {
            return (value - input1) / (output1 - input1) * (output2 - input2) + input2;
        }

        public static string AddSpace(this string abc, int start, int space)
        {
            for (int i = start; i <= abc.Length; i += space)
            {
                abc = abc.Insert(i, " ");
                i++;
            }

            return abc;
        }

        public static string ReverseAddSpace(this string abc, int start, int space)
        {
            for(int i = abc.Length - start; i >= 0; i -= space)
            {
                abc = abc.Insert(i, " ");
            }

            return abc;
        }
    }
}

namespace ProduktionChains
{
    public enum CartesianDir { Right, Left, Up, Down, None};
    public enum CellType { None, Road, TruckSpawner, StonePickup, WoodPickup, Destruction, TableFactory, Clear};
}