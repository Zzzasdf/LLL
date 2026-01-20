Shader "Custom/SimpleShader"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half3 _Color;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal: NORMAL;
                float4 textcoord: TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInputs.positionCS;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half3 finalColor = input.positionCS;

                finalColor *= _Color.rgb;
                
                return half4(finalColor, 1.0);
            }

            
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}
