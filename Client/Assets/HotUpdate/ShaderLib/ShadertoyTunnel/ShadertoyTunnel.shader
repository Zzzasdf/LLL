// 适配Unity通用渲染管线（URP）的Shadertoy隧道效果
Shader "Custom/URP_Tunnel"
{
    Properties
    {
        // 可在材质面板调节的参数
        _Speed("Animation Speed", Range(0, 5)) = 1.0
        _Density("Tunnel Density", Range(1, 30)) = 9.0
    }
    
    SubShader
    {
        // URP关键标签：声明该着色器与URP兼容[citation:3]
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline" 
        }

        Pass
        {
            // URP中应使用HLSLPROGRAM而非CGPROGRAM[citation:1]
            HLSLPROGRAM
            // 定义顶点和片段着色器函数名
            #pragma vertex vert
            #pragma fragment frag

            // 必须包含URP的核心库，以获取正确的函数和宏[citation:3]
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // 将Properties中的属性在HLSL中声明，并放入常量缓冲区
            CBUFFER_START(UnityPerMaterial)
                float _Speed;
                float _Density;
            CBUFFER_END

            // 顶点着色器输入结构（来自网格数据）
            struct Attributes
            {
                float4 positionOS : POSITION;   // 物体空间顶点位置
                float2 uv : TEXCOORD0;          // 纹理坐标
            };

            // 顶点着色器输出/片段着色器输入结构（顶点间插值的数据）
            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // 齐次裁剪空间位置
                float2 uv : TEXCOORD0;           // 传递UV坐标
                float4 screenPos : TEXCOORD1;    // 用于计算屏幕空间坐标
            };

            // 顶点着色器
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // 将顶点位置从物体空间转换到齐次裁剪空间[citation:3]
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                // 计算用于后续获取屏幕坐标的参数
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            // 片段（像素）着色器 - 核心效果发生的地方
            half4 frag(Varyings IN) : SV_Target
            {
                // --- 1. 准备工作：获取基础参数 ---
                // URP中时间变量通过 _Time.y 获取
                float time = _Time.y * _Speed; // 应用速度控制
                // 获取屏幕分辨率
                float2 resolution = _ScreenParams.xy;
                // 精确计算当前像素在屏幕上的坐标（替代GLSL中的fragCoord）
                float2 fragCoord = (IN.screenPos.xy / IN.screenPos.w) * resolution;
                
                // --- 2. 核心算法：生成动态隧道 ---
                float3 color = float3(0, 0, 0); // 初始化RGB颜色
                float phase = time; // 动画相位
                
                // 分别处理RGB三个通道，产生彩色偏移效果
                for (int channel = 0; channel < 3; channel++)
                {
                    // 坐标标准化和中心化
                    float2 uv = fragCoord / resolution;
                    float2 p = uv - 0.5;
                    // 修正宽高比，防止图像拉伸
                    p.x *= resolution.x / resolution.y;
                    
                    // 更新相位并计算到屏幕中心的距离
                    phase += 0.07;
                    float radius = length(p);
                    
                    // --- 核心扭曲公式 ---
                    // 根据相位、半径和密度，对UV坐标进行动态扭曲
                    uv += (p / radius) * (sin(phase) + 1.0) * abs(sin(radius * _Density - phase * 2.0));
                    
                    // --- 生成重复的隧道单元图案 ---
                    // 通过取模（fmod）创建无限重复的网格
                    float2 cellUV = float2(fmod(uv.x, 1.0), fmod(uv.y, 1.0));
                    // 计算每个重复单元内像素到中心的距离
                    float distFromCellCenter = length(cellUV - 0.5);
                    // 距离越近，亮度越高（形成光点），并应用距离衰减塑造隧道纵深感
                    color[channel] = (0.01 / distFromCellCenter) / radius;
                }
                
                // --- 3. 输出最终颜色 ---
                return half4(color, 1.0); // Alpha固定为1（不透明）
            }
            ENDHLSL
        }
    }
    // 如果上述SubShader不支持，则回退到简单漫反射
    // FallBack "Universal Render Pipeline/Unlit"
}