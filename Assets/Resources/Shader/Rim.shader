Shader "Custom/Rim" 
{     
	Properties 
	{       
		_MainTex ("Texture", 2D) = "white" {}            
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)      
		_RimPower ("Rim Power", Range(0.5,8.0)) = 0.5
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	

	SubShader 
	{       
			Tags { "RenderType" = "Opaque" }       
			
			CGPROGRAM       
			#pragma surface surf Unlit  alphatest:_Cutoff 
			struct Input 
			{           
				float2 uv_MainTex;           
				float2 uv_BumpMap;           
				float3 viewDir;       
			};
			half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
			{
				half4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}
	
			sampler2D _MainTex;       
			sampler2D _BumpMap;       
			float4 _RimColor;       
			float _RimPower;       
			void surf (Input IN, inout SurfaceOutput o) 
			{
				float4 color = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo = color.rgb;      
				o.Alpha = color.a;     
				half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));           
				o.Emission = _RimColor.rgb * pow (rim, _RimPower);
			}
			ENDCG    
	}      
	Fallback "Diffuse"   
}