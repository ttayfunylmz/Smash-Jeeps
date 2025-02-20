// Writen by Martin Nerurkar ( www.playful.systems).
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sharkbomb/Shine/SpriteDefault-Shine-Standalone"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_WaveFreq("Shine Frequency", Float) = 1
		_WavePause("Shine Pause", Float) = 0.5
		_WaveWidth("Shine Width", Float) = 0.05
		_WaveFade("Shine Fade", Float) = 0.15

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVertShine
			#pragma fragment SpriteFragShine
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#pragma multi_compile __ SB_SHINE_REVERSE
			#pragma multi_compile __ SB_SHINE_SMOOTH
			#pragma multi_compile __ SB_SHINE_CUBIC

			#include "UnitySprites.cginc"
			#include "ShineShader.cginc"

			struct v2fshiny
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float time : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed _WaveFreq;
			fixed _WavePause;
			fixed _WaveWidth;
			fixed _WaveFade;
			
			v2fshiny SpriteVertShine(appdata_t IN)
			{
				v2fshiny OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

				OUT.time = getShineTime(_WaveFreq, _WavePause, _Time.y, _WaveWidth, _WaveFade);
				return OUT;
			}

			fixed4 SpriteFragShine(v2fshiny IN) : SV_Target
			{
				fixed4 c = IN.color;
				c.a = getShinePixelAlpha(IN.time, _WaveWidth, _WaveFade, SampleSpriteTexture(IN.texcoord).rb);
				c.rgb *= c.a;
				return c;
			}
				
		ENDCG
		}
	}
	CustomEditor "ShineShaderMaterialInspector"
	Fallback "Sprites/Default"
}
