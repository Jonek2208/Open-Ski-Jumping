Shader "CTI/LOD Billboard 301" {
	Properties{
		[Space(5)]
		_HueVariation 							("Color Variation (RGB) Strength (A)", Color) = (0.9,0.5,0.0,0.1)

		[Space(5)]
		[NoScaleOffset] _MainTex 				("Albedo (RGB) Alpha/Occlusion (A)", 2D) = "white" {}
		_Cutoff 								("Alpha Cutoff", Range(0, 1.0)) = 0.3
		_AlphaLeak 								("Alpha Leak Suppression", Range(0.5,1.0)) = 0.6
		_OcclusionStrength 						("Occlusion Strength", Range(0,1)) = 1		
		
		[Space(5)]
		[NoScaleOffset] _BumpTex 				("Normal (AG) Translucency(R) Smoothness(B)", 2D) = "bump" {}
		_NormalScale 							("Normal Scale", Float) = 1
		_SpecColor 								("Specular", Color) = (0.2,0.2,0.2)

		[Space(5)]
		_TranslucencyStrength 					("Translucency Strength", Range(0,1)) = .5
		_ViewDependency 						("View Dependency", Range(0,1)) = 0.8
		_AmbientTranslucency					("Ambient Scattering", Range(0,8)) = 1.0
		[Toggle(_PARALLAXMAP)]
		_EnableTransFade 						("Fade out Translucency", Float) = 0.0

		[Header(Wind)]
		[Space(3)]
		[Toggle(_EMISSION)] _UseWind			("Enable Wind", Float) = 0.0
		[Space(5)]
		_WindStrength							("Wind Strength", Float) = 1.0
		
		[HideInInspector] _TreeScale 			("Tree Scale", Range(0,50)) = 1
		[HideInInspector] _TreeWidth 			("Tree Width Factor", Range(0,1)) = 1
	}
		SubShader{
		Tags{
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
			"RenderType" = "CTI-Billboard"
			"DisableBatching" = "LODFading"
		}
		
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf StandardTranslucent vertex:BillboardVertInit nolightmap nodynlightmap keepalpha addshadow noinstancing dithercrossfade
		// addshadow
		#pragma target 3.0
		#pragma multi_compile __ LOD_FADE_CROSSFADE
		#pragma shader_feature _EMISSION
		// Fade out Translucency
		#pragma shader_feature _PARALLAXMAP

		#define IS_LODTREE

		#include "Includes/CTI_BillboardVertex_301.cginc"
		#include "Includes/CTI_TranslucentLighting_301.cginc"

		sampler2D _MainTex;
		sampler2D _BumpTex;

		float _Cutoff;
		float _AlphaLeak;
		half4 _HueVariation;

		fixed _OcclusionStrength;

		half _TranslucencyStrength;
		half _ViewDependency;

		half _NormalScale;

		// All other inputs moved to include

		void BillboardVertInit(inout appdata_bb v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			AFSBillboardVert(v);
			o.uv_MainTex = v.texcoord;
			o.color.rgb = v.color.rgb;
			UNITY_TRANSFER_DITHER_CROSSFADE(o, v.vertex)
		}

		void surf(Input IN, inout SurfaceOutputStandardTranslucent o) {

			#if UNITY_VERSION < 2017
    			UNITY_APPLY_DITHER_CROSSFADE(IN)
   			#endif

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			clip(c.a - _Cutoff);

			o.Albedo = c.rgb;
			//	Add Color Variation
			o.Albedo = lerp(o.Albedo, (o.Albedo + _HueVariation.rgb) * 0.5, IN.color.r * _HueVariation.a);

			float4 normal = tex2D(_BumpTex, IN.uv_MainTex);
			o.Normal.xy = normal.ag * 2 - 1;
			o.Normal.xy *= _NormalScale;
			o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));
//o.Normal = normalize(o.Normal);

			o.Translucency = (normal.r) * _TranslucencyStrength;
			o.ScatteringPower = _ViewDependency;
			o.TransFade = IN.color.b;
			
			o.Specular = _SpecColor;
			o.Smoothness = normal.b;

			#if defined(_PARALLAXMAP)
				o.Smoothness *= IN.color.b;
			#endif
		
			c.a = (c.a <= _AlphaLeak) ? 1 : c.a; // Eliminate alpha leaking into ao
			o.Occlusion = lerp(1, c.a * 2 - 1, _OcclusionStrength);

			o.Translucency *= o.Occlusion;

		}
		ENDCG

		// Pass to render object as a shadow caster
/*		Pass{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }
//		Cull Front

		CGPROGRAM
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile __ LOD_FADE_CROSSFADE
			#include "HLSLSupport.cginc"
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#define UNITY_PASS_SHADOWCASTER
			#include "Includes/CTI_BillboardVertex.cginc"

			sampler2D _MainTex;

			struct v2f_surf {
				V2F_SHADOW_CASTER;
				float2 hip_pack0 : TEXCOORD1;
				//#ifdef LOD_FADE_CROSSFADE
				//	half3 ditherScreenPos : TEXCOORD2;
				//#endif
				UNITY_DITHER_CROSSFADE_COORDS_IDX(2)
			};

			float4 _MainTex_ST;

			v2f_surf vert_surf(appdata_bb v) {
				v2f_surf o;
				AFSBillboardVert(v);
				o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				//UNITY_TRANSFER_DITHER_CROSSFADE(o, v.vertex)
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)
				return o;
			}
			fixed _Cutoff;

			float4 frag_surf(v2f_surf IN) : SV_Target{
				UNITY_APPLY_DITHER_CROSSFADE(IN)
				half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
				//	alpha = (unity_LODFade.x > 0) ? alpha * unity_LODFade.x : alpha;
				clip(alpha - _Cutoff);
				SHADOW_CASTER_FRAGMENT(IN)
			}
		ENDCG
	} */

		///
	}
		FallBack "Transparent/Cutout/VertexLit"
}
