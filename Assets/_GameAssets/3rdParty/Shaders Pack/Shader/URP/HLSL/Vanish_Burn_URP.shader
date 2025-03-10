Shader "MatthewGuz/Vanish_Burn_URP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Base Map", 2D) = "white" {}
        _NoiseTex ("Noise Map", 2D) = "white" {}
        _BrightnessTex ("Brightness Map", 2D) = "white" {}

        _Cutoff ("Dissolve Amount", Range(0, 1)) = 0.25
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.05
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _BurnColor ("Burnt Color", Color) = (0.1,0.05,0.05,1)
        _BurnIntensity ("Burn Intensity", Range(0, 5)) = 1.0

        _OscillationEnabled ("Enable Oscillation", Range(0, 1)) = 1.0
        _OscillationAmplitude ("Oscillation Amplitude", Range(0, 5)) = 1.0
        _OscillationSpeed ("Oscillation Speed", Range(0, 5)) = 1.0

        _Speed ("Brightness Speed", Range(0, 5)) = 1.0
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
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _BrightnessTex;

            float4 _BaseColor;
            float4 _EdgeColor;
            float4 _BurnColor;
            float _Cutoff;
            float _EdgeWidth;
            float _BurnIntensity;

            float _OscillationEnabled;
            float _OscillationAmplitude;
            float _OscillationSpeed;

            float _Speed;

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
                float noiseValue = tex2D(_NoiseTex, IN.uv).r;

                // Offset the UV coordinates of the brightness texture to simulate movement
                float2 movingUV = IN.uv + float2(_Time.y * _Speed, 0); // Move along U axis
                float brightnessValue = tex2D(_BrightnessTex, movingUV).r;

                // Edge effect
                float edgeThreshold = _Cutoff * (1.0 + _EdgeWidth);

                // Burn logic:
                half4 finalColor;
                if (noiseValue < _Cutoff)
                {
                    // If the noise value is less than Cutoff, use the BurnColor
                    float oscillation = _OscillationEnabled > 0.5 ? 
                                        _OscillationAmplitude * sin(_Time.y * _OscillationSpeed) : 0.0; // Oscillation
                    finalColor = _BurnColor * (_BurnIntensity + oscillation) * brightnessValue; // Increase brightness
                }
                else
                {
                    // Otherwise, use the base texture
                    finalColor = baseTex;
                }

                // Apply edge effect only where it corresponds (on the edge)
                half4 edgeColor = (noiseValue < edgeThreshold && noiseValue >= _Cutoff) ? _EdgeColor : half4(0,0,0,0);

                // Return the final color
                return finalColor + edgeColor;
            }

            ENDHLSL
        }
    }
}
