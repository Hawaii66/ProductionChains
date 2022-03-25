using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Utilities;

namespace ProduktionChains.Buildings
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager instance;

        public Dictionary<Coord, Building> activeBuildings;

        public bool isBuilding;
        public bool isDestroying;

        public GameObject currentObj;
        public CellType currentType;

        public BuildingScriptableObj[] buildings;

        public Transform buildingParent;
        public Transform roadParent;

        [Header("Debug")]
        public GameObject roadPrefab;
        public GameObject[] debugOBJs;
        public int currentDebugOBJ;
        public CartesianDir currentDebugRot;

        public bool drawBuilding;
        public bool drawBuildingWaypoints;
        public bool drawBuildingConnections;

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

            activeBuildings = new Dictionary<Coord, Building>();
        }

        public void StartBuild(CellType buildType)
        {
            foreach(BuildingScriptableObj buildingScriptableObj in buildings)
            {
                if(buildingScriptableObj.type == buildType)
                {
                    StartBuild(buildingScriptableObj.prefab);
                }
            }

            if(buildType == CellType.Road)
            {
                currentType = CellType.Road;
                StartBuild(roadPrefab);
            }
        }

        public void StartBuild(GameObject obj)
        {
            currentObj = obj;
            isBuilding = true;
            isDestroying = false;
            if (currentObj.GetComponent<Building>())
            {
                currentType = currentObj.GetComponent<Building>().type;
            }
        }

        public void CancelAll()
        {
            isBuilding = false;
            isDestroying = false;
            currentObj = null;
            currentType = CellType.None;
        }

        public void StartDestroy()
        {
            currentType = CellType.None;
            currentObj = null;
            isDestroying = true;
            isBuilding = false;
        }

        private void Update()
        {
            if (isBuilding)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(mouseRay, out RaycastHit hit, 100f, 1 << LayerMask.NameToLayer("Ground")))
                {
                    int x = WorldFloatToGrid(hit.point.x);
                    int z = WorldFloatToGrid(hit.point.z);
                    Coord gridPos = new Coord(x, 0, z);

                    if (Input.GetMouseButtonDown(0))
                    {
                        Cell c = GridManager.instance.GetCellCreate(gridPos);
                        if (currentType == CellType.Road)
                        {
                            if (!c.IsSolid())
                            {
                                Instantiate(roadPrefab, gridPos * GridManager.gridSize, Quaternion.identity).transform.SetParent(roadParent);
                            }
                        }
                        else
                        {
                            if (!c.IsSolid())
                            {
                                if(!BuildingGroundIsSolid(currentType, c.gridPos, currentDebugRot)) { return; }
                                GameObject obj = InstantiateObj(currentType);
                            
                                obj.transform.SetPositionAndRotation(c.gridPos * GridManager.gridSize, Quaternion.Euler(0, Utils.GetRotation(currentDebugRot), 0));
                                Money.MoneyManager.instance.ModifyMoney(-GetBuildingFromType(currentType).cost);
                                if (obj.GetComponent<Building>() != null) { obj.GetComponent<Building>().direction = currentDebugRot; }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentDebugOBJ += 1;
                    if(currentDebugOBJ >= debugOBJs.Length)
                    {
                        currentDebugOBJ = 0;
                    }
                }else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentDebugOBJ -= 1;
                    if(currentDebugOBJ < 0)
                    {
                        currentDebugOBJ = debugOBJs.Length - 1;
                    }
                }
            }
        }

        public GameObject InstantiateObj(CellType type)
        {
            GameObject temp = Instantiate(GetBuildingFromType(type).prefab);
            temp.transform.SetParent(buildingParent);
            return temp;
        }

        public Building PlaceBuilding(Cell c, GameObject obj, CellType type)
        {
            if (c.IsSolid()) { return null; }

            //obj.transform.SetPositionAndRotation(c.gridPos * GridManager.gridSize, Quaternion.Euler(0, Utils.GetRotation(currentDebugRot), 0));

            Building b = obj.GetComponent<Building>();
            b.type = type;
            b.gridPos = c.gridPos;
            b.cell = c;

            BuildingScriptableObj buildingScriptableObj = GetBuildingFromType(type);
            foreach(Coord coord in buildingScriptableObj.footprint)
            {
                Coord gridPos = coord.RotateCoord(Utils.GetRotation(b.direction)) + c.gridPos;
                Cell cell = GridManager.instance.GetCellCreate(gridPos);
                if (cell.IsSolid())
                {
                    Debug.Log("Not good cell already solid: " + cell.gridPos);
                }
                cell.SetSolidBuilding(b);
            }
            /*
            for(int x = 0; x < buildingScriptableObj..x; x++)
            {
                for (int z = 0; z < buildingScriptableObj.size.z; z++)
                {
                    Coord gridPos = new Coord(x, 0, z).RotateCoord(Utils.GetRotation(b.direction)) + c.gridPos;
                    Cell cell = GridManager.instance.GetCellCreate(gridPos);
                    if (cell.IsSolid())
                    {
                        Debug.Log("Not good cell already solid: " + cell.gridPos);
                    }
                    cell.SetSolidBuilding(b);
                }
            }
            */
            c.SetSolidBuilding(b);

            activeBuildings.Add(b.gridPos, b);

            return b;
        }

        private Coord[] GetGridCoordsBuilding(CellType type, Coord startGrid, CartesianDir rotation)
        {
            BuildingScriptableObj buildingScriptableObj = GetBuildingFromType(type);

            Coord[] gridCoords = new Coord[buildingScriptableObj.footprint.Length];
            for(int i = 0; i < gridCoords.Length; i++)
            {
                Coord gridPos = buildingScriptableObj.footprint[i].RotateCoord(Utils.GetRotation(currentDebugRot)) + startGrid;
                gridCoords[i] = gridPos;
            }

            /*Coord[] gridCoords = new Coord[buildingScriptableObj.size.x * buildingScriptableObj.size.z];
            int count = 0;
            for (int x = 0; x < buildingScriptableObj.size.x; x++)
            {
                for (int z = 0; z < buildingScriptableObj.size.z; z++)
                {
                    Coord gridPos = new Coord(x, 0, z).RotateCoord(Utils.GetRotation(currentDebugRot)) + startGrid;
                    gridCoords[count] = gridPos;

                    count += 1;
                }
            }*/

            return gridCoords;
        }

        private bool BuildingGroundIsSolid(CellType type, Coord startGrid, CartesianDir rotation)
        {
            Coord[] offsets = GetGridCoordsBuilding(type, startGrid, rotation);
            foreach(Coord c in offsets)
            {
                if (GridManager.instance.GetCell(c) != null && GridManager.instance.GetCell(c).IsSolid())
                {
                    return false;
                }
            }

            return true;
        }

        private int WorldFloatToGrid(float point)
        {
            point -= GridManager.gridSize / 2;

            return Mathf.CeilToInt(point / GridManager.gridSize);
        }

        public BuildingScriptableObj GetBuildingFromType(CellType t)
        {
            foreach (BuildingScriptableObj b in buildings)
            {
                if (b.type == t)
                {
                    return b;
                }
            }

            return null;
        }
    }
}