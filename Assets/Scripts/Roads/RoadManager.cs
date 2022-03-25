using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains.Roads
{
    public class RoadManager : MonoBehaviour
    {
        public static RoadManager instance;

        public Dictionary<Coord, Road> roadNetwork;
        public MeshFilter roadMesh;

        public RoadConfiguration[] roadConfigurations;

        [Header("Debug")]
        [SerializeField] private bool drawWaypoints = true;
        [SerializeField] private bool drawWhiteWaypoints = true;
        [SerializeField] private bool drawRedWaypoints = true;
        [SerializeField] private bool drawGreenWaypoints = true;

        public int roadCount
        {
            get
            {
                return roadNetwork.Count;
            }
        }

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
            roadNetwork = new Dictionary<Coord, Road>();
        }

        private void Start()
        {
            GenerateMesh();
        }

        public void AddRoad(Road r, bool generateMesh)
        {
            if (r != null)
            {
                if (!roadNetwork.ContainsKey(r.gridPos))
                {
                    roadNetwork.Add(r.gridPos, r);
                }

                UpdateNeighbourIndexes(r.gridPos);
            }

            if (generateMesh)
            {
                GenerateMesh();
            }
        }

        public void RegenerateMesh()
        {
            foreach(Road r in roadNetwork.Values)
            {
                UpdateNeighbourIndexes(r.gridPos);
            }

            GenerateMesh();
        }

        private void UpdateNeighbourIndexes(Coord centerPos)
        {
            foreach(Coord c in Utilities.Utils.offsetsWithCenter)
            {
                Coord gridPos = c + centerPos;
                if(roadNetwork.TryGetValue(gridPos, out Road road))
                {
                    bool roadSouth = roadNetwork.ContainsKey(gridPos + new Coord(0, 0, -1));
                    bool roadNorth = roadNetwork.ContainsKey(gridPos + new Coord(0, 0, 1));
                    bool roadEast = roadNetwork.ContainsKey(gridPos + new Coord(1, 0, 0));
                    bool roadWest = roadNetwork.ContainsKey(gridPos + new Coord(-1, 0, 0));

                    roadSouth |= SnapRoadToBuilding(road, new Coord(0, 0, -1));
                    roadNorth |= SnapRoadToBuilding(road, new Coord(0, 0, 1));
                    roadEast |= SnapRoadToBuilding(road, new Coord(1, 0, 0));
                    roadWest |= SnapRoadToBuilding(road, new Coord(-1, 0, 0));

                    int count = 0;
                    if (roadSouth) { count |= 1; }
                    if (roadNorth) { count |= 2; }
                    if (roadEast) { count |= 4; }
                    if (roadWest) { count |= 8; }

                    road.roadIndex = count;
                    road.SetMesh(roadConfigurations[count].mesh);
                }
            }
        }

        private bool SnapRoadToBuilding(Road r, Coord c)
        {
            if (r.snapRoadOffsets.Contains(c)) { return true; }

            return false;
        }

        public RoadConfiguration GetConfiguration(int index)
        {
            if(index > roadConfigurations.Length - 1) { return null; }
            return roadConfigurations[index];
        }

        public void GenerateMesh()
        {
            CombineInstance[] combine = new CombineInstance[roadCount];
            int i = 0;
            MeshFilter currentMesh;
            foreach(Road r in roadNetwork.Values)
            {
                r.GetComponentInChildren<MeshRenderer>().enabled = false;

                currentMesh = r.GetComponentInChildren<MeshFilter>();
                combine[i].mesh = currentMesh.sharedMesh;
                combine[i].transform = currentMesh.transform.localToWorldMatrix;

                i += 1;
            }

            roadMesh.mesh = new Mesh();
            roadMesh.mesh.CombineMeshes(combine);
        }

        private void OnDrawGizmos()
        {
            if (drawWaypoints && roadNetwork != null && roadNetwork.Count > 0)
            {
                foreach (Road r in roadNetwork.Values)
                {
                    if (GetConfiguration(r.roadIndex).throughWaypoints != null && drawWhiteWaypoints)
                    {
                        Gizmos.color = Color.white;
                        foreach (RoadThroughWaypoints roadThroughWaypoints in GetConfiguration(r.roadIndex).throughWaypoints)
                        {
                            if (roadThroughWaypoints.waypoints == null) { continue; }
                            for (int i = 0; i < roadThroughWaypoints.waypoints.Length; i++)
                            {
                                Vector3 point = roadThroughWaypoints.waypoints[i];
                                point += r.gridPos * GridManager.gridSize;
                                point.y += 0.5f;
                                if (i != 0)
                                {
                                    Gizmos.DrawLine(point, roadThroughWaypoints.waypoints[i - 1] + r.gridPos * GridManager.gridSize + new Vector3(0, 0.5f, 0));
                                }

                                Gizmos.DrawWireSphere(point, 0.2f);
                            }
                        }
                    }
                    if (GetConfiguration(r.roadIndex).exitRoad != null && drawRedWaypoints)
                    {
                        Gizmos.color = Color.red;
                        foreach (RoadStopWaypoints roadThroughWaypoints in GetConfiguration(r.roadIndex).exitRoad)
                        {
                            if (roadThroughWaypoints.waypoints == null) { continue; }
                            for (int i = 0; i < roadThroughWaypoints.waypoints.Length; i++)
                            {
                                Vector3 point = roadThroughWaypoints.waypoints[i];
                                point += r.gridPos * GridManager.gridSize;
                                point.y += 0.5f;
                                if (i != 0)
                                {
                                    Gizmos.DrawLine(point, roadThroughWaypoints.waypoints[i - 1] + r.gridPos * GridManager.gridSize + new Vector3(0, 0.5f, 0));
                                }

                                Gizmos.DrawWireSphere(point, 0.2f);
                            }
                        }
                    }
                    if (GetConfiguration(r.roadIndex).enterRoad != null && drawGreenWaypoints) 
                    {
                        Gizmos.color = Color.green;
                        foreach (RoadStopWaypoints roadThroughWaypoints in GetConfiguration(r.roadIndex).enterRoad)
                        {
                            if (roadThroughWaypoints.waypoints == null) { continue; }
                            for (int i = 0; i < roadThroughWaypoints.waypoints.Length; i++)
                            {
                                Vector3 point = roadThroughWaypoints.waypoints[i];
                                point += r.gridPos * GridManager.gridSize;
                                point.y += 0.5f;
                                if (i != 0)
                                {
                                    Gizmos.DrawLine(point, roadThroughWaypoints.waypoints[i - 1] + r.gridPos * GridManager.gridSize + new Vector3(0, 0.5f, 0));
                                }

                                Gizmos.DrawWireSphere(point, 0.2f);
                            }
                        }
                    }
                }
            }
        }
    }
}