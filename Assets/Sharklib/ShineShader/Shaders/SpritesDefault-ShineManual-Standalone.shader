// Writen by Martin Nerurkar ( www.playful.systems).
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sharkbomb/Shine/SpriteDefault-ShineManual-Standalone"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

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

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment SpriteFragShine
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#include "UnitySprites.cginc"
			#include "ShineShader.cginc"

			fixed _WaveWidth;
			fixed _WaveFade;
			fixed _TimeControl;

			fixed4 SpriteFragShine(v2f IN) : SV_Target
			{
				fixed4 c = IN.color;
				c.a = getShinePixelAlpha(_TimeControl, _WaveWidth, _WaveFade, SampleSpriteTexture(IN.texcoord).r);
				c.rgb *= c.a;
				return c;
			}
				
		ENDCG
		}
	}
	Fallback "Sprites/Default"
}
