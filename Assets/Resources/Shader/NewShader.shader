Shader "Custom/NewShader" 
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BlockTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	Category
	{
		SubShader
		{
			//Tags { "Queue"="Overlay+1"
			//"RenderType"="Transparent"}
			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				ZTest Greater
				Lighting Off
				SetTexture [_BlockTex] {combine texture}
			}
			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest On
				ZTest Less
				SetTexture [_MainTex] {combine texture}
			}
		}
	}
	FallBack "Diffuse"
}