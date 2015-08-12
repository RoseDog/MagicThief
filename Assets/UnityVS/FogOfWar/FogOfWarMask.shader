Shader "Custom/FogOfWarMask" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "white" {}
		_BlurPower("BlurPower", float) = 0.002
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		LOD 200
		
		CGPROGRAM
		#pragma surface surf NoLighting alpha
		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, float aten)
		{
			fixed4 color;
			color.rgb = s.Albedo;
			color.a = s.Alpha;
			return color;
		}

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float _BlurPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 baseColor1 = tex2D (_MainTex, IN.uv_MainTex + float2(-_BlurPower, 0));
			half4 baseColor2 = tex2D (_MainTex, IN.uv_MainTex + float2(0, -_BlurPower));
			half4 baseColor3 = tex2D (_MainTex, IN.uv_MainTex + float2(_BlurPower, 0));
			half4 baseColor4 = tex2D (_MainTex, IN.uv_MainTex + float2(0, _BlurPower));
			half4 Alpha = 0.25 * (baseColor1 + baseColor2 + baseColor3 + baseColor4);
			//half4 baseColor = tex2D (_NoiseTex, IN.uv_MainTex * 16);

			//o.Albedo = baseColor.rgb;
			o.Alpha = (1.0f - Alpha.a)*_Color.a;
		}
		ENDCG
	} 
	Fallback "Diffuse"
}
