// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/AlphaTestToon" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { Texgen CubeNormal }
	
		_Amount ("Extrusion Amount", Float ) = 0.05
		_RimColor ("Rim Color", Color) = (1,0,1,1)

		_BlockTex ("Behind Wall (A)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
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
		Name "BASE"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float3 cubenormal : TEXCOORD1;
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			samplerCUBE _ToonShade;
			

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
				float4 cube = texCUBE(_ToonShade, i.cubenormal);
				clip(col.a - _Cutoff);
				return float4(2.0f * cube.rgb * col.rgb, col.a);
			}
		ENDCG
	}

	Cull Front

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alphatest:_Cutoff
			
		uniform float4 _RimColor;
			
		struct Input 
		{
			float2 uv_MainTex;
		};
		float _Amount;
		void vert (inout appdata_full v) 
		{
			v.vertex.xyz += v.normal * _Amount;
		}
		sampler2D _MainTex;
		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Emission = _RimColor;
		}
		ENDCG
}
FallBack "Diffuse"
}