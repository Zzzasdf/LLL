using UnityEngine;

public class Photography : AssetBaseLoader<GameObject>
{
    [SerializeField] private Transform modelRootTra;
    
    [SerializeField] private Transform cameraRootTra;
    [SerializeField] private Transform cameraTra;
    [SerializeField] private Camera camera;

    public void Load(RenderTexture renderTexture, string location, float cameraDistance)
    {
        camera.targetTexture = renderTexture;
        
        Vector3 cameraLocalPosition = cameraTra.localPosition;
        cameraLocalPosition.z = -cameraDistance;
        cameraTra.localPosition = cameraLocalPosition;
        
        Load(location);
    }

    protected override void OnCompleted(GameObject assetObject)
    {
        GameObject instantiatedObject = Instantiate(assetObject, Vector3.zero, Quaternion.identity, modelRootTra);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {assetObject.name}");
            return;
        }
    }
}
