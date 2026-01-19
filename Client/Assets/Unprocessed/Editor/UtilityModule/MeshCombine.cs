using UnityEditor;
using UnityEngine;

public class MeshCombine
{
    /// 网格合并
    [MenuItem("GameObject/MeshCombine", priority = 0)]
    public static void Run()
    {
        // MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        // if (meshFilters.Length == 0)
        // {
        //     return;
        // }
        // CombineInstance[] meshCombine = new CombineInstance[meshFilters.Length];
        // for (int i = 0; i < meshFilters.Length; i++)
        // {
        //     meshCombine[i].mesh = meshFilters[i].sharedMesh;
        //     meshCombine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        // }
        // Mesh mesh = new Mesh();
        // mesh.CombineMeshes(meshCombine);
        // AssetDatabase.CreateAsset(mesh, $"Assets/{Selection.activeGameObject.name}.asset");
        // AssetDatabase.SaveAssets();
        // GameObject obj = new GameObject(Selection.activeGameObject.name);
        // obj.AddComponent<MeshFilter>().mesh = mesh;
        // obj.AddComponent<MeshRenderer>().sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        GameObject obj = new GameObject(Selection.activeGameObject.name);
        obj.transform.position = Selection.activeGameObject.transform.position;
        obj.transform.rotation = Selection.activeGameObject.transform.rotation;
        GameObject tmpObj = Object.Instantiate(Selection.activeGameObject, obj.transform);
        tmpObj.transform.localPosition = Vector3.zero;
        tmpObj.transform.localRotation = Quaternion.identity;
        MeshFilter[] meshFilters = tmpObj.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length > 0)
        {
            CombineInstance[] meshCombine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                meshCombine[i].mesh = meshFilters[i].sharedMesh;
                meshCombine[i].transform = Matrix4x4.TRS(meshFilters[i].transform.localPosition, meshFilters[i].transform.localRotation, meshFilters[i].transform.localScale);
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(meshCombine);
            AssetDatabase.CreateAsset(mesh, string.Format("Assets/{0}.asset", Selection.activeGameObject.name));
            AssetDatabase.SaveAssets();
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>().sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        }
        Object.DestroyImmediate(tmpObj);
    }
}
    
    
