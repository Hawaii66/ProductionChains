using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Buildings;
using ProduktionChains.Roads;

namespace ProduktionChains
{
    public class Cell
    {
        public CellType type;
        public Coord gridPos;
        public Building building;
        public Road road;

        bool isSolid;

        public Cell(Coord c, CellType t)
        {
            type = t;
            gridPos = c;
        }

        public bool SetSolidBuilding(Building b)
        {
            if (IsSolid()) { return false; }
            building = b;
            road = null;
            isSolid = true;
            type = b.type;
            return true;
        }

        public bool SetSolidRoad(Road r)
        {
            if (IsSolid()) { return false; }
            building = null;
            road = r;
            isSolid = true;
            type = CellType.Road;
            return true;
        }

        public bool IsSolid()
        {
            return isSolid;
        }

        public void RemoveBuilding()
        {
            isSolid = false;
            type = CellType.None;
            building = null;
        }
    }
}