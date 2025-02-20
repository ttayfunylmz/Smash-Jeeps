// Writen by Martin Nerurkar ( www.playful.systems).
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sharkbomb/Shine/SpriteDiffuse-ShineManual-Emission"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_RampTex("Ramp Texture", 2D) = "white" {}
		_RampColor("Ramp Color", Color) = (1,1,1,1)

		_WaveFreq("Shine Frequency", Float) = 1
		_WavePause("Shine Pause", Float) = 0.5
		_WaveWidth("Shine Width", Float) = 0.05
		_WaveFade("Shine Fade", Float) = 0.15

		_TimeControl("Time Control", Float) = -10

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

		CGPROGRAM
		#pragma surface SpriteSurfShine Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
		#pragma multi_compile_local _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

		#pragma multi_compile __ SB_SHINE_REVERSE
		#pragma multi_compile __ SB_SHINE_SMOOTH
		#pragma multi_compile __ SB_SHINE_CUBIC
		#include "UnitySprites.cginc"
		#include "ShineShader.cginc"

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};

		void vert(inout appdata_full v, out Input o)
		{
			v.vertex = UnityFlipSprite(v.vertex, _Flip);

			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap(v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _RendererColor;
		}

		sampler2D _RampTex;
		fixed4 _RampColor;
		fixed _WaveFreq;
		fixed _WavePause;
		fixed _WaveWidth;
		fixed _WaveFade;
		fixed _TimeControl;

		void SpriteSurfShine(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
			// Put the shinyness into Emission
			o.Emission = _RampColor * getShinePixelAlpha(_TimeControl, _WaveWidth, _WaveFade, (tex2D(_RampTex, IN.uv_MainTex).rb)) * c.a;
		}
		ENDCG
	}

	Fallback "Sprites/Diffuse"
}
