using System.Collections;
using UnityEngine;
using UnityEditor;
using ProduktionChains.Buildings;

namespace ProduktionChains.UnityEditor
{
    [CustomEditor(typeof(BuildManager))]
    public class BuildManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BuildManager buildManager = (BuildManager)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Start build"))
            {
                buildManager.StartBuild(buildManager.debugOBJs[buildManager.currentDebugOBJ]);
            }

            if(GUILayout.Button("Start road"))
            {
                buildManager.currentType = CellType.Road;
                buildManager.StartBuild(buildManager.roadPrefab);
            }

            if (GUILayout.Button("Cancel build"))
            {
                buildManager.CancelAll();
            }
        }
    }
}