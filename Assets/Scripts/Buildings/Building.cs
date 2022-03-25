using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Roads;
using ProduktionChains.Utilities;
using ProduktionChains.Vehicles;

namespace ProduktionChains.Buildings
{
    public class Building : MonoBehaviour
    {
        public CellType type;
        public Coord gridPos;
        public Cell cell;
        public CartesianDir direction;

        public virtual void Start()
        {
            gridPos = transform.position.GetGridPos();
            transform.rotation = Quaternion.Euler(0, Utils.GetRotation(direction), 0);

            BuildManager.instance.PlaceBuilding(GridManager.instance.GetCellCreate(gridPos), gameObject, type);

            UpdateRoadConnections();
        }

        public virtual void AddVehicle(Vehicle vehicle)
        {
            Destroy(vehicle);
        }
        public virtual void AddVehicle(Vehicle vehicle, Vector3 waypoint)
        {
            Destroy(vehicle);
        }

        public void UpdateRoadConnections()
        {
            foreach (Coord c in BuildManager.instance.GetBuildingFromType(type).enterRoadOffsets)
            {
                Coord enterRoadOffsetCoord = c.RotateCoord(Utils.GetRotation(direction));
                Coord offsetPos = enterRoadOffsetCoord + gridPos;
                //Debug.Log(c + " : " + enterRoadOffsetCoord + " : " + gridPos + " : " + Utils.GetRotation(direction) + " : " + enterRoadOffsetCoord * -1);
                if (RoadManager.instance.roadNetwork.TryGetValue(offsetPos, out Road r))
                {
                    if(r.snapRoadOffsets.Contains((enterRoadOffsetCoord * -1).ClampTo01())) { continue; }
                    r.snapRoadOffsets.Add((enterRoadOffsetCoord * -1).ClampTo01());
                    RoadManager.instance.AddRoad(r, true);
                }
            }
        }

        public Coord GetInsideCoord(Coord outside)
        {
            foreach (BuildingConnectionInside buildingStopWaypoints in BuildManager.instance.GetBuildingFromType(type).insideConnections)
            {
                if (outside == buildingStopWaypoints.outside.RotateCoord(Utils.GetRotation(direction)))
                {
                    return buildingStopWaypoints.inside;
                }
            }

            return null;
        }

        public Vector3[] GetExitWaypoints(Coord inside)
        {
            foreach(BuildingWaypointsInside waypoints in BuildManager.instance.GetBuildingFromType(type).waypointsInside)
            {
                if(waypoints.travelOut == true && waypoints.inside == inside)
                {
                    Vector3[] newPoints = new Vector3[waypoints.points.Length];
                    int count = 0;
                    foreach(Vector3 p in waypoints.points)
                    {
                        newPoints[count] = p.RotateVector(Utils.GetRotation(direction)) + gridPos * GridManager.gridSize;

                        count += 1;
                    }
                    return newPoints;
                }
            }

            return null;
        }

        public Vector3[] GetEnterWaypoints(Coord inside)
        {
            foreach (BuildingWaypointsInside waypoints in BuildManager.instance.GetBuildingFromType(type).waypointsInside)
            {
                if (waypoints.travelOut == false && waypoints.inside == inside)
                {
                    Vector3[] newPoints = new Vector3[waypoints.points.Length];
                    int count = 0;
                    foreach (Vector3 p in waypoints.points)
                    {
                        newPoints[count] = p.RotateVector(Utils.GetRotation(direction)) + gridPos * GridManager.gridSize;

                        count += 1;
                    }
                    return newPoints;
                }
            }

            return null;
        }

        public Coord[] GetExitConnections(Vector3 waypoint)
        {
            BuildingScriptableObj buildingScriptableObj = BuildManager.instance.GetBuildingFromType(type);
            for(int i = 0; i < buildingScriptableObj.waypointsInside.Length; i++)
            {
                BuildingWaypointsInside waypointsInside = buildingScriptableObj.waypointsInside[i];
                if (!waypointsInside.travelOut) { continue; }
                foreach(Vector3 v in waypointsInside.points)
                {
                    if (v.RotateVector(Utils.GetRotation(direction)) + gridPos * GridManager.gridSize == waypoint)
                    {
                        return new Coord[] { waypointsInside.travelOutTile };
                    }
                }
            }
            return buildingScriptableObj.buildingConnections;
        }

        private void OnDrawGizmos()
        {
            if(BuildManager.instance == null) { return; }
            
            if (BuildManager.instance.drawBuilding)
            {
                BuildingScriptableObj buildingScriptableObj = BuildManager.instance.GetBuildingFromType(type);
                if (BuildManager.instance.drawBuildingConnections)
                {
                    Gizmos.color = Color.white;
                    foreach (Coord c in buildingScriptableObj.buildingConnections)
                    {
                        Coord pos = c.RotateCoord(Utils.GetRotation(direction)) * GridManager.gridSize + gridPos * GridManager.gridSize;

                        Gizmos.DrawWireCube(pos, new Vector3(GridManager.gridSize, 2, GridManager.gridSize));
                    }

                    Gizmos.color = Color.yellow;
                    foreach (BuildingConnectionInside c in buildingScriptableObj.insideConnections)
                    {
                        Coord pos = c.inside.RotateCoord(Utils.GetRotation(direction)) * GridManager.gridSize + gridPos * GridManager.gridSize;

                        Gizmos.DrawWireCube(pos, new Vector3(GridManager.gridSize, 2, GridManager.gridSize));
                    }
                }

                if (BuildManager.instance.drawBuildingWaypoints)
                {
                    Gizmos.color = Color.blue;
                    foreach (BuildingWaypointsInside waypoints in buildingScriptableObj.waypointsInside)
                    {
                        Vector3 pos = waypoints.inside.RotateCoord(Utils.GetRotation(direction)) * GridManager.gridSize + gridPos * GridManager.gridSize;
                        pos.y += 0.5f;

                        Gizmos.DrawWireSphere(pos, 0.2f);
                    }

                    Gizmos.color = Color.green;
                    foreach (BuildingWaypointsInside waypoints in buildingScriptableObj.waypointsInside)
                    {
                        if (waypoints.travelOut) { continue; }

                        Vector3 prev = Vector3.zero;
                        bool isFirst = true;
                        foreach (Vector3 p in waypoints.points)
                        {
                            Vector3 pos = p.RotateVector(Utils.GetRotation(direction)) + gridPos * GridManager.gridSize;
                            pos.y += 0.5f;

                            Gizmos.DrawWireSphere(pos, 0.2f);

                            if (!isFirst)
                            {
                                Gizmos.DrawLine(pos, prev);
                            }

                            isFirst = false;
                            prev = pos;
                        }
                    }

                    Gizmos.color = Color.red;
                    foreach (BuildingWaypointsInside waypoints in buildingScriptableObj.waypointsInside)
                    {
                        if (!waypoints.travelOut) { continue; }

                        Vector3 prev = Vector3.zero;
                        bool isFirst = true;
                        foreach (Vector3 p in waypoints.points)
                        {
                            Vector3 pos = p.RotateVector(Utils.GetRotation(direction)) + gridPos * GridManager.gridSize;
                            pos.y += 0.5f;

                            Gizmos.DrawWireSphere(pos, 0.2f);

                            if (!isFirst)
                            {
                                Gizmos.DrawLine(pos, prev);
                            }

                            isFirst = false;
                            prev = pos;
                        }
                    }
                }

                Gizmos.color = Color.cyan;
                foreach(Coord footPrint in buildingScriptableObj.footprint)
                {
                    Coord pos = footPrint.RotateCoord(Utils.GetRotation(direction)) * GridManager.gridSize + gridPos * GridManager.gridSize;

                    Gizmos.DrawWireCube(pos, new Vector3(GridManager.gridSize, 1.5f, GridManager.gridSize));
                }
            }
        }
    }

}