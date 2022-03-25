using UnityEditor;
using UnityEngine;

namespace ProduktionChains.Roads
{
    [CreateAssetMenu(fileName ="Road",menuName="ScriptableObj/Road")]
    public class RoadConfiguration : ScriptableObject
    {
        public Mesh mesh;

        public RoadThroughWaypoints[] throughWaypoints;
        public RoadStopWaypoints[] enterRoad;
        public RoadStopWaypoints[] exitRoad;
    }
}