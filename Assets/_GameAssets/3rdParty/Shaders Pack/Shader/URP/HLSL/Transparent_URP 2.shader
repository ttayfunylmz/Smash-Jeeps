Shader "MatthewGuz/Transparent_2_URP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0, 1)) = 1
        _Smoothness ("Smoothness", Range(0, 1)) = 1
        _MainTex ("Base Map", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front   // Hide back faces to eliminate internal joints
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _BaseColor;
            float _Metallic;
            float _Smoothness;
            float _Transparency;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                // Calculate view direction using the global variable _WorldSpaceCameraPos
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Get the main light
                Light mainLight = GetMainLight(IN.positionHCS);

                // Base texture and color
                half4 baseTex = tex2D(_MainTex, IN.uv) * _BaseColor;

                // Calculate the angle between the normal and the view to adjust transparency
                float visibility = dot(IN.normalWS, IN.viewDirWS);
                visibility = saturate(visibility);

                // Adjust transparency based on the angle and given value
                float finalAlpha = _Transparency * visibility;

                // Apply basic lighting using the main light
                float3 lightDir = normalize(mainLight.direction);
                float ndotl = max(0, dot(IN.normalWS, lightDir));
                float3 lighting = ndotl * baseTex.rgb;

                // Final color with transparency
                return half4(lighting, finalAlpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
