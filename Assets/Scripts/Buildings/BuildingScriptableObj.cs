using UnityEditor;
using UnityEngine;
using ProduktionChains.Roads;

namespace ProduktionChains.Buildings
{
    [CreateAssetMenu(fileName="Building",menuName= "ScriptableObj/Building")]
    public class BuildingScriptableObj : ScriptableObject
    {
        [Header("Info")]
        [Tooltip("Type of building")]
        public CellType type;
        [Tooltip("Prefab to spawn when placing building")]
        public GameObject prefab;
        [Tooltip("Size of the building")]
        public Coord[] footprint;
        [Tooltip("Cost to build this building")]
        public int cost;

        [Header("Connections")]
        [Tooltip("Road tiles the building is connected to")]
        public Coord[] enterRoadOffsets;

        [Tooltip("Connections inside building?")]
        public Coord[] buildingConnections;
        [Tooltip("Connections between roads outside and roads inside")]
        public BuildingConnectionInside[] insideConnections;

        [Tooltip("Waypoints and how they are connected")]
        public BuildingWaypointsInside[] waypointsInside;
    }

    [System.Serializable]
    public class BuildingConnectionInside
    {
        public Coord outside;
        public Coord inside;
    }

    [System.Serializable]
    public class BuildingWaypointsInside
    {
        [Tooltip("Inside connection")]
        public Coord inside;
        [Tooltip("Is a path to a outside road and in that case the road you will land on")]
        public bool travelOut;
        public Coord travelOutTile;
        [Tooltip("Points to reach the destination")]
        public Vector3[] points;
    }
}