using System.Collections;
using UnityEngine;
using UnityEditor;
using ProduktionChains.Vehicles;

namespace ProduktionChains.UnityEditor
{
    [CustomEditor(typeof(AIVehicle))]
    public class VehicleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AIVehicle vehicle = (AIVehicle)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Start drive"))
            {
                vehicle.SetTargetAndDrive(vehicle.start, vehicle.end);
            }
        }
    }
}