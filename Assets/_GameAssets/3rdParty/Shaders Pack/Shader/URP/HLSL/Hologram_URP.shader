Shader "MatthewGuz/Hologram Shader"
{
    Properties
    {
        _Color("Color", Color) = (0, 1, 1, 1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _AlphaTexture ("Alpha Mask (R)", 2D) = "white" {}

        _Scale ("Alpha Tiling", Float) = 3
        _ScrollSpeedV("Alpha scroll Speed", Range(0, 5.0)) = 1.0

        _GlowIntensity ("Glow Intensity", Range(0.01, 1.0)) = 0.5

        _GlitchSpeed ("Glitch Speed", Range(0, 50)) = 50.0
        _GlitchIntensity ("Glitch Intensity", Range(0.0, 0.1)) = 0
    }

    SubShader
    {
        Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Lighting Off 
            ZWrite On
            Blend SrcAlpha One
            Cull Back

            CGPROGRAM

                #pragma vertex vertexFunc
                #pragma fragment fragmentFunc

                #include "UnityCG.cginc"

                struct appdata{
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f{
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float3 worldNormal : NORMAL;
                };

                fixed4 _Color, _MainTex_ST;
                sampler2D _MainTex, _AlphaTexture;
                half _Scale, _ScrollSpeedV, _GlowIntensity, _GlitchSpeed, _GlitchIntensity;

                v2f vertexFunc(appdata IN){
                    v2f OUT;

                    // Vertex displacement based on the Glitch effect
                    IN.vertex.z += sin(_Time.y * _GlitchSpeed * 5 * IN.vertex.y) * _GlitchIntensity;

                    // Convert the object position to clip space
                    OUT.position = UnityObjectToClipPos(IN.vertex);

                    // Keep the UVs of the object
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                    // Keep the position in world space
                    OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;

                    // Calculate the normal in world space
                    OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);

                    return OUT;
                }

                fixed4 fragmentFunc(v2f IN) : SV_Target{
                    
                    // Calculate the alpha texture using the world position
                    fixed4 alphaColor = tex2D(_AlphaTexture, IN.worldPos.xy * _Scale + _Time.y * _ScrollSpeedV);
                    
                    // Calculate the main texture
                    fixed4 pixelColor = tex2D (_MainTex, IN.uv);
                    
                    // Apply the alpha from the texture to the pixelColor
                    pixelColor.w = alphaColor.w;

                    // Edge (rim) effect using the normal in world space
                    half rim = 1.0 - saturate(dot(normalize(_WorldSpaceCameraPos - IN.worldPos), IN.worldNormal));

                    // Apply color, rim effect, and glow
                    return pixelColor * _Color * (rim + _GlowIntensity);
                }
            ENDCG
        }
    }
}

