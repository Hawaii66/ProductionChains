using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Utilities;

namespace ProduktionChains.Roads
{
    public class Road : MonoBehaviour
    {
        public Coord gridPos;
        public int roadIndex;
        public List<Coord> snapRoadOffsets;

        public Mesh mesh;

        private void Start()
        {
            gridPos = GridManager.WorldToGridCoord(transform.position);

            GridManager.instance.GetCellCreate(gridPos).SetSolidRoad(this);

            RoadManager.instance.AddRoad(this, true); // on start all roads generate whole network again BUG

            snapRoadOffsets = new List<Coord>();

            foreach(Coord c in Utils.offsets)
            {
                Coord pos = c + gridPos;
                Cell cell = GridManager.instance.GetCell(pos);
                if(cell != null && cell.building != null)
                {
                    Buildings.Building b = cell.building;
                    b.UpdateRoadConnections();
                }
            }
        }

        public void SetMesh(Mesh m)
        {
            mesh = m;
            GetComponentInChildren<MeshFilter>().mesh = mesh;
            GetComponentInChildren<MeshFilter>().sharedMesh = mesh;
        }

        public Coord[] GetEndConnections()
        {
            RoadConfiguration config = RoadManager.instance.GetConfiguration(roadIndex);

            Dictionary<CartesianDir, Coord> connectionsTest = new Dictionary<CartesianDir, Coord>();
            foreach(RoadThroughWaypoints waypoints in config.throughWaypoints)
            {
                if(waypoints.end == CartesianDir.Left && !connectionsTest.ContainsKey(CartesianDir.Left)) { connectionsTest.Add(CartesianDir.Left, new Coord(-1, 0, 0)); }
                if(waypoints.end == CartesianDir.Right && !connectionsTest.ContainsKey(CartesianDir.Right)) { connectionsTest.Add(CartesianDir.Right, new Coord(1, 0, 0)); }
                if(waypoints.end == CartesianDir.Up && !connectionsTest.ContainsKey(CartesianDir.Up)) { connectionsTest.Add(CartesianDir.Up, new Coord(0, 0, 1)); }
                if(waypoints.end == CartesianDir.Down && !connectionsTest.ContainsKey(CartesianDir.Down)) { connectionsTest.Add(CartesianDir.Down, new Coord(0, 0, -1)); }
            }

            Coord[] connections = new Coord[connectionsTest.Count];
            int count = 0;
            foreach (Coord c in connectionsTest.Values)
            {
                connections[count] = c;
                count += 1;
            }

            return connections;
        }

        public Vector3[] GetWaypoints(CartesianDir start, CartesianDir end)
        {
            RoadConfiguration config = RoadManager.instance.GetConfiguration(roadIndex);

            int count = 0;
            foreach (RoadThroughWaypoints waypoints in config.throughWaypoints)
            {
                if(waypoints.start == start && waypoints.end == end)
                {
                    return waypoints.waypoints;
                }

                count += 1;
            }

            return null;
        }
        public Vector3[] GetEnterWaypoints(CartesianDir start)
        {
            RoadConfiguration config = RoadManager.instance.GetConfiguration(roadIndex);

            int count = 0;
            foreach (RoadStopWaypoints waypoints in config.enterRoad)
            {
                if (waypoints.direction == start)
                {
                    return waypoints.waypoints;
                }

                count += 1;
            }

            return null;
        }
        public Vector3[] GetExitWaypoints(CartesianDir end)
        {
            RoadConfiguration config = RoadManager.instance.GetConfiguration(roadIndex);

            int count = 0;
            foreach (RoadStopWaypoints waypoints in config.exitRoad)
            {
                if (waypoints.direction == end)
                {
                    return waypoints.waypoints;
                }

                count += 1;
            }

            return null;
        }
    }

    [System.Serializable]
    public class RoadThroughWaypoints
    {
        public CartesianDir start;
        public CartesianDir end;
        public Vector3[] waypoints;
    }

    [System.Serializable]
    public class RoadStopWaypoints
    {
        public CartesianDir direction;
        public Vector3[] waypoints;
    }
}