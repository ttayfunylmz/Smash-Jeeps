Shader "MatthewGuz/Interference_VHS_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.2
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _TimeMultiplier ("Time Multiplier", Range(0.1, 10)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderPipeline" = "UniversalRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color; // Color multiplier
            float _DistortionStrength;
            float _NoiseStrength;
            float _TimeMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex); // URP-compatible transformation
                o.uv = v.uv;
                return o;
            }

            float rand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Distortion based on a sine wave with time dependency
                uv += sin(uv.y * 500 + _Time.y * _TimeMultiplier) * _DistortionStrength;
                
                // Add random noise with time dependency
                uv += (rand(uv + _Time.xy * _TimeMultiplier) - 0.5) * _NoiseStrength;

                // Sample the texture color with the new UV coordinates
                float4 col = tex2D(_MainTex, uv);

                // Multiply the final color by the _Color property
                return col * _Color;
            }
            ENDHLSL
        }
    }
}
