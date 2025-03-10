Shader "MatthewGuz/Vanish_2_URP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Base Map", 2D) = "white" {}
        _NoiseTex ("Noise Map", 2D) = "white" {}

        _Cutoff ("Dissolve Amount", Range(0, 1)) = 0.25
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.05
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Cull [_Cull]
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            float4 _BaseColor;
            float4 _EdgeColor;
            float _Cutoff;
            float _EdgeWidth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 baseTex = tex2D(_MainTex, IN.uv) * _BaseColor;
                float noiseValue = tex2D(_NoiseTex, float2(IN.uv.x, IN.uv.y + _Cutoff)).r;

                // Dissolve effect
                if (noiseValue < _Cutoff)
                    discard;

                // Calculate edge effect
                float edgeThreshold = _Cutoff * (1.0 + _EdgeWidth);
                half4 edgeColor = (noiseValue < edgeThreshold) ? _EdgeColor : 0;

                return baseTex + edgeColor;
            }

            ENDHLSL
        }
    }
}