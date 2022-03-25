using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProduktionChains.Roads;

namespace ProduktionChains.UnityEditor
{
    [CustomEditor(typeof(RoadManager))]
    public class RoadManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RoadManager roadManager = (RoadManager)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Regenerate roads"))
            {
                roadManager.RegenerateMesh();
            }

            if(GUILayout.Button("Generate Mesh"))
            {
                roadManager.GenerateMesh();
            }
        }
    }
}