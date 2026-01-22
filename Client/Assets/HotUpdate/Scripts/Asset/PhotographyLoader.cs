using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RawImage))]
public class PhotographyLoader : AssetBaseLoader<GameObject>
{
    private bool init;

    private string modelLocation;
    private float cameraDistance;
    
    private RectTransform rt;
    private RawImage rImg;

    private RenderTexture renderTexture;
    
    private void Init()
    {
        if (init) return;
        rt = GetComponent<RectTransform>();
        rImg = GetComponent<RawImage>();
        init = !init;
    }

    public void Load(string modelLocation, float cameraDistance)
    {
        Init();
        this.modelLocation = modelLocation;
        this.cameraDistance = cameraDistance;
        base.Load(nameof(Photography));
    }

    protected override void OnCompleted(GameObject assetObject)
    {
        GameObject instantiatedObject = Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {assetObject.name}");
            return;
        }
        Photography photography = instantiatedObject.GetComponent<Photography>();
        if (photography == null)
        {
            LLogger.FrameError($"{assetObject.name} 无法找到组件 {nameof(Photography)}");
            return;
        }
        Vector2 sizeDelta = rt.sizeDelta;
        int width = (int)sizeDelta.x;
        int height = (int)sizeDelta.y;
        int depth = 24;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(width, height, depth)
            {
                antiAliasing = 4,                    // 抗锯齿级别
                filterMode = FilterMode.Bilinear,    // 过滤模式
                wrapMode = TextureWrapMode.Clamp,    // 环绕模式
                enableRandomWrite = false,            // 允许计算着色器写入
                useMipMap = false,                    // 生成MipMap
                autoGenerateMips = false              // 自动生成MipMap
            };
        }
        else
        {
            renderTexture.width = width;
            renderTexture.height = height;
            renderTexture.depth = depth;
        }
        rImg.texture = renderTexture;
        photography.Load(renderTexture, modelLocation, cameraDistance);
    }
}
