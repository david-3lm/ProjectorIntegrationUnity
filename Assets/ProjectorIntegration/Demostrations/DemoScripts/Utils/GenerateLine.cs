using UnityEngine;
using UnityEditor;
namespace ProjectorIntegration
{
    public class GenerateLine : EditorWindow
    {
        [SerializeField] private GameObject prefab;
        [MenuItem("Tools/GenerateLine")]

        static void CreateGenerateLine()
        {
            EditorWindow.GetWindow<GenerateLine>();
        }
        private void OnGUI()
        {
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
            if (GUILayout.Button("Generate"))
            {
                var selection = Selection.gameObjects;

                var prefabType = PrefabUtility.GetPrefabType(prefab);
                GameObject newObject;

                if (prefabType == PrefabType.Prefab)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    return;
                }

                newObject.transform.SetParent(selection[0].transform.parent);


                newObject.GetComponent<StarLine>().star1 = selection[0].GetComponent<Star>();
                newObject.GetComponent<StarLine>().star2 = selection[1].GetComponent<Star>();

            }

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);

        }
    }
}
