using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectorIntegration
{
    public class PutConstellation : EditorWindow
    {
        [SerializeField] private GameObject constellation;

        Camera camera;
        Transform OG;

        [MenuItem("Tools/Put Constellations")]

        static void CreatePutConstellation()
        {
            EditorWindow.GetWindow<PutConstellation>();
        }

        private void OnGUI()
        {
            constellation = (GameObject)EditorGUILayout.ObjectField("Prefab constellation", constellation, typeof(GameObject), false);
            if (GUILayout.Button("Replace"))
            {
                camera = Camera.main;
                OG = camera.transform;
                Transform t = camera.transform;
                t.position = camera.transform.position + (camera.transform.forward * 25);
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(constellation, t);
                newObject.transform.parent = camera.transform.parent;
            }
            Camera.main.transform.position = Vector3.zero;
            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }
    }
}
