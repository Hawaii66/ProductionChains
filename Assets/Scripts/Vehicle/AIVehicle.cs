using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Roads;
using ProduktionChains.Buildings;
using ProduktionChains.Utilities;

namespace ProduktionChains.Vehicles
{
    public class AIVehicle : Vehicle
    {
        public Mission[] mission;
        public Transform vehicleCollision;
        public int missionIndex;
        public Vector3[] path;
        public Vector3 target;
        public int targetIndex;
        public bool isDriving;
        public bool isBlocked;
        public bool followMission = true;
        public EndAction endAction;

        [Header("Debug")]
        public Coord start;
        public Coord end;

        private float vehicleCollisionSize = 0.1f;

        public override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (!isDriving) { return; }

            if (Vector3.Distance(transform.position, target) > 0.1f)
            {
                if(Physics.OverlapSphere(vehicleCollision.position, vehicleCollisionSize, 1 << LayerMask.NameToLayer("Trucks")).Length > 0)
                {
                    return;
                }

                Quaternion q = Quaternion.LookRotation(target - transform.position);
                if (Mathf.Abs(q.eulerAngles.y - transform.rotation.eulerAngles.y) < snapTurn)
                {
                    transform.LookAt(target);
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            }
            else
            {
                transform.position = target;
                transform.LookAt(target);
                NextTarget();
            }
        }

        private void OnDrawGizmos()
        {
            if(vehicleCollision == null) { return; }
            Gizmos.DrawWireSphere(vehicleCollision.position, vehicleCollisionSize);
        }

        public void NextMission(Coord s)
        {
            start = s;
            NextMission();
        }

        public void NextMission()
        {
            if (followMission)
            {
                missionIndex += 1;
                if(missionIndex >= mission.Length)
                {
                    missionIndex = 0;
                }

                UpdateGridPos();
                endAction = mission[missionIndex].endAction;
                SetTargetAndDrive(start, mission[missionIndex].end);
            }
        }

        private void NextTarget()
        {
            targetIndex += 1;
            if(targetIndex >= path.Length)
            {
                UpdateGridPos();

                if(endAction == EndAction.EnterBuilding)
                {
                    Cell c = GridManager.instance.GetCell(gridPos);

                    if(c.building != null)
                    {
                        c.building.AddVehicle(this);
                    }
                }
                return;
            }

            target = path[targetIndex];
        }

        public void SetTargetAndDrive(Coord s, Coord e)
        {
            start = s;
            end = e;

            target = Vector3.zero;
            targetIndex = 0;

            SetPath();

            isDriving = true;
        }

        private void SetPath()
        {
            Building startB = null;
            Building endB = null;
            List<(Coord, Building)> startPositions = new List<(Coord, Building)>();
            List<(Coord, Building)> endPositions = new List<(Coord, Building)>();

            if (BuildManager.instance.activeBuildings.TryGetValue(start, out Building startBuilding))
            {
                BuildingScriptableObj buildingScriptableObj = BuildManager.instance.GetBuildingFromType(startBuilding.type);
                Coord[] connections = startBuilding.GetExitConnections((path == null || path.Length == 0) ? Vector3.zero : path[path.Length - 1]);
                for (int i = 0; i < connections.Length; i++)
                {
                    startPositions.Add((connections[i].RotateCoord(Utils.GetRotation(startBuilding.direction)) + startBuilding.gridPos, startBuilding));
                }
            }
            else
            {
                startPositions.Add((start, null));
            }

            if (BuildManager.instance.activeBuildings.TryGetValue(end, out Building endBuilding))
            {
                BuildingScriptableObj buildingScriptableObj = BuildManager.instance.GetBuildingFromType(endBuilding.type);
                for (int i = 0; i < buildingScriptableObj.buildingConnections.Length; i++)
                {
                    endPositions.Add((buildingScriptableObj.buildingConnections[i].RotateCoord(Utils.GetRotation(endBuilding.direction)) + endBuilding.gridPos, endBuilding));
                }
            }
            else
            {
                endPositions.Add((end, null));
            }

            Coord[] coordPath = null;

            bool hasFoundPath = false;
            foreach ((Coord, Building) startC in startPositions)
            {
                foreach ((Coord, Building) endC in endPositions)
                {
                    Coord[] tempPath = RoadPathfinding.GetPath(startC.Item1, endC.Item1);
                    if (tempPath == null) { continue; }
                    if (coordPath == null || coordPath.Length > tempPath.Length)
                    {
                        coordPath = tempPath;
                        startB = startC.Item2;
                        endB = endC.Item2;
                        start = startC.Item1;
                        end = endC.Item1;
                        hasFoundPath = true;
                    }
                }
            }

            if (!hasFoundPath)
            {
                return;
            }

            coordPath = RoadPathfinding.GetPath(start, end);

            List<Vector3> worldPoints = new List<Vector3>();

            if (startB)
            {
                Vector3[] points = startB.GetExitWaypoints(startB.GetInsideCoord(start - startB.gridPos));
                foreach (Vector3 p in points)
                {
                    worldPoints.Add(p);
                }
            }

            for (int i = 0; i < coordPath.Length; i++)
            {
                if (!RoadManager.instance.roadNetwork.ContainsKey(coordPath[i])) { continue; }

                Coord prevCoord;
                if (i == 0 && startB != null)
                {
                    prevCoord = startB.GetInsideCoord(start - startB.gridPos).RotateCoord(Utils.GetRotation(startB.direction)) + startB.gridPos;
                }
                else if (i == 0 && startB == null)
                {
                    prevCoord = coordPath[0];
                }
                else
                {
                    prevCoord = coordPath[i - 1];
                }

                Coord currentCoord = coordPath[i];

                Coord nextCoord;
                if (i == coordPath.Length - 1 && endB != null)
                {
                    nextCoord = endB.GetInsideCoord(end - endB.gridPos).RotateCoord(Utils.GetRotation(endB.direction)) + endB.gridPos;
                }
                else if (i == coordPath.Length - 1 && endB == null)
                {
                    nextCoord = coordPath[coordPath.Length - 1];
                }
                else
                {
                    nextCoord = coordPath[i + 1];
                }

                CartesianDir startDir = CartesianDir.None;
                CartesianDir endDir = CartesianDir.None;

                if (prevCoord.z > currentCoord.z) { startDir = CartesianDir.Up; }
                if (prevCoord.z < currentCoord.z) { startDir = CartesianDir.Down; }
                if (prevCoord.x > currentCoord.x) { startDir = CartesianDir.Right; }
                if (prevCoord.x < currentCoord.x) { startDir = CartesianDir.Left; }
                if (nextCoord.z > currentCoord.z) { endDir = CartesianDir.Up; }
                if (nextCoord.z < currentCoord.z) { endDir = CartesianDir.Down; }
                if (nextCoord.x > currentCoord.x) { endDir = CartesianDir.Right; }
                if (nextCoord.x < currentCoord.x) { endDir = CartesianDir.Left; }

                Vector3[] roadPath;

                if (startDir != CartesianDir.None && endDir != CartesianDir.None)
                {
                    roadPath = RoadManager.instance.roadNetwork[coordPath[i]].GetWaypoints(startDir, endDir);
                }
                else if (startDir != CartesianDir.None)
                {
                    roadPath = RoadManager.instance.roadNetwork[coordPath[i]].GetExitWaypoints(startDir);
                }
                else
                {
                    roadPath = RoadManager.instance.roadNetwork[coordPath[i]].GetEnterWaypoints(endDir);
                }

                foreach (Vector3 v in roadPath)
                {
                    worldPoints.Add(v + currentCoord * GridManager.gridSize);
                }
            }

            if (endB)
            {
                Vector3[] points = endB.GetEnterWaypoints(endB.GetInsideCoord(end - endB.gridPos));
                foreach (Vector3 p in points)
                {
                    worldPoints.Add(p);
                }
            }

            path = worldPoints.ToArray();
            target = path[0];
            targetIndex = 0;
        }

        [System.Serializable]
        public class Mission
        {
            public Coord end;
            public EndAction endAction;
        }
    }
}