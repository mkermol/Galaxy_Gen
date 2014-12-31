﻿Shader "specular_1" {
	Properties{
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess", Float) = 10
	}	
	SubShader {
		Tags{"LightMode" = "ForwardBase"}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			//user vars
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			//unity vars
			uniform float4 _LightColor0;
			
			//structs
			struct vertexInput{
				float4 vertex : Position;
				float3 normal : NORMAL;
			};
			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				
				//vectors
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), _World2Object).xyz);
				float3 viewDirection = normalize(float3(float4(_WorldSpaceCameraPos.xyz, 1.0) - mul(_Object2World, v.vertex).xyz));
				float3 lightDirection;
				float atten = 1.0;
				
				//Lighting
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflection = atten * _LightColor0.xyz * _Color.rgb * max(0.0,dot(normalDirection,lightDirection));
				float3 specularReflection =atten * _SpecColor.rgb * max(0.0,dot( reflect(- lightDirection,normalDirection), viewDirection)) * pow(max(0.0,dot(normalDirection,lightDirection)),_Shininess);
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;
				
				o.col = float4(lightFinal , 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			//fragment function
			float4 frag(vertexOutput i) : Color{
				return i.col;
			}			
			ENDCG
		}
	}
	//Fallback "Diffuse"
}