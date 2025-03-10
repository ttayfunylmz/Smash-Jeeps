Shader "MatthewGuz/Transparent_URP"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _AngleThreshold ("Angle Threshold", Range(0, 1)) = 0.3 // Controls the visibility of internal parts
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back   // Hide back faces
        ZWrite On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _Transparency;
            float _AngleThreshold; // New parameter to control visibility angle

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                // Calculate view direction from the object to the camera
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Calculate the angle between the view direction and the normal to hide internal parts
                float visibility = dot(i.worldNormal, i.viewDir);
                
                // Discard fragments that are not sufficiently visible
                if (visibility > -_AngleThreshold && visibility < _AngleThreshold)
                {
                    discard;
                }

                // Base texture
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Outline based on the camera angle
                float outlineFactor = smoothstep(1.0 - _OutlineWidth, 1.0, abs(visibility));

                // Apply transparency and mix the color
                float4 finalColor = lerp(_OutlineColor, texColor, outlineFactor);
                finalColor.a *= _Transparency;

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
