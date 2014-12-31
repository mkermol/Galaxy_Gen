Shader "specular_2" {
	Properties{
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Diffuse Texture", 2D) = "white"{}
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess", Float) = 10
		//_Amplitude ("Displecement Amount", Float) = 1.0
	}	
	SubShader {
		Tags{"LightMode" = "ForwardBase"}
		Pass {
			CGPROGRAM
			#pragma target 3.0 
			#pragma glsl
			#pragma vertex vert
			#pragma fragment frag
			
			//user vars
			uniform float4 _Color;
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			//unity vars
			uniform float4 _LightColor0;
			
			//structs
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				
			};
			
			//vertex function
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				
				o.posWorld =  v.vertex ;
				o.normalDir = v.normal;
				
				
				float4 d = tex2Dlod (_MainTex, float4(v.texcoord.xy,0,0));
				
				v.vertex.y += d.y * 20.0;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex + float4(0.0,d.y,0.0,0.0));
				
				o.tex = v.texcoord;
				
				return o;						
			}
			
			//fragment function
			float4 frag(vertexOutput i) : Color{
				//vectors
				float3 normalDirection =normalize(mul(float4(i.normalDir,0.0), _World2Object).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(_Object2World, i.posWorld).xyz);
				float3 lightDirection;
				float atten = 1.0;
				
				//textures
				float4 tex = tex2D (_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				
				//Lighting
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflection = atten * _LightColor0.xyz * tex.rgb * _Color.rgb * max(0.0,dot(normalDirection,lightDirection));
				float3 specularReflection =atten * _SpecColor.rgb * max(0.0,dot( reflect(- lightDirection,normalDirection), viewDirection)) * pow(max(0.0,dot(normalDirection,lightDirection)),_Shininess);
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;
				
				
				
				return float4(lightFinal , 1.0);
				
			}			
			ENDCG
		}
	}
	//Fallback "Diffuse"
}