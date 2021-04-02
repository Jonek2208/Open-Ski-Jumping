// Upgrade NOTE: removed variant '__' where variant LOD_FADE_PERCENTAGE is used.

Shader "CTI/LOD Bark 301" {
Properties {
	
	_HueVariation 						("Color Variation*", Color) = (0.9,0.5,0.0,0.1)
	
	[Header(Main Maps)]
	[Space(5)]
	_MainTex 							("Albedo (RGB) Smoothness (A)", 2D) = "white" {}
	[NoScaleOffset] _BumpSpecAOMap 		("Normal Map (GA) Specular (R) AO (B)", 2D) = "bump" {}

	[Header(Secondary Maps)]
	[Space(5)]
	[CTI_DetailsEnum] _DetailMode 		("Secondary Maps (need UV2)", Float) = 0
	[Toggle(_SWAP_UVS)] _SwapUVS 		("    Swap UVS", Float) = 0.0
	_AverageCol 						("    Average Color (RGB) Smoothness (A)", Color) = (1,1,1,0.5)
	[Space(5)]

	[NoScaleOffset] _DetailAlbedoMap	("    Detail Albedo x2 (RGB) Smoothness (A)", 2D) = "gray" {}
	[NoScaleOffset] _DetailNormalMapX	("    Normal Map (GA) Specular (R) AO (B)", 2D) = "gray" {}
	_DetailNormalMapScale				("    Normal Strength", Float) = 1.0

	_OcclusionStrength 					("Occlusion Strength", Range(0,1)) = 1

	[Header(Wind)]
	[Space(3)]
	_BaseWindMultipliers 				("Wind Multipliers (XYZ)*", Vector) = (1,1,1,0)
	[Space(5)]
	[Toggle(_METALLICGLOSSMAP)]
	_LODTerrain 						("Use Wind from Script*", Float) = 0.0

	[Header(Options for lowest LOD)]
	[Space(5)]
	[Toggle] _FadeOutWind				("Fade out Wind", Float) = 0.0

	// In case we switch to the debug shader
    [HideInInspector] _Cutoff ("Cutoff", Range(0,1)) = 1

	// Needed by VegetationStudio's Billboard Rendertex Shaders
	[HideInInspector] _IsBark("Is Bark", Float) = 1
}


SubShader { 
	Tags {
		"Queue"="Geometry"
		"RenderType"="CTI-TreeBarkLOD"
		"DisableBatching"="LODFading"
	}
	LOD 200
	CGPROGRAM
// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does	
		#pragma surface surf StandardSpecular vertex:CTI_TreeVertBark nodynlightmap keepalpha dithercrossfade
// nolightmap
		#pragma target 3.0
		#pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma multi_compile __ _METALLICGLOSSMAP
		#pragma multi_compile_instancing

	//	Detail modes: simply on / fade base textures / skip base textures
		#pragma shader_feature __ GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND
		#pragma shader_feature _SWAP_UVS

		//#if UNITY_VERSION >= 550
			#pragma instancing_options assumeuniformscaling lodfade procedural:setup
		//#endif

		#define IS_BARK
		#define IS_LODTREE
		#define IS_SURFACESHADER

		// #include "UnityBuiltin3xTreeLibrary.cginc" // We can not do this as we want instancing
		// We do NOT define LEAFTUMBLING here
		#include "Includes/CTI_Builtin4xTreeLibraryTumbling_301.cginc"
		#include "Includes/CTI_indirect.cginc"	

		sampler2D _MainTex;
		sampler2D _BumpSpecAOMap;
		fixed4 _AverageCol;

		#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND)
			sampler2D _DetailAlbedoMap;
			sampler2D _DetailNormalMapX;
			half _DetailNormalMapScale;
		#endif

		/* moved to include
		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR;
			UNITY_DITHER_CROSSFADE_COORDS
		};*/

		fixed _OcclusionStrength;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

			#if UNITY_VERSION < 2017
    			UNITY_APPLY_DITHER_CROSSFADE(IN)
   			#endif
		
		//	Swapped UVs	
			#if ( defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND) ) && defined (_SWAP_UVS)
			//	Skip base textures (details)
				#if defined(GEOM_TYPE_FROND)
					fixed4 c = _AverageCol;
					fixed4 bumpSpecAO = fixed4(unity_ColorSpaceDielectricSpec.r, 0.5, 1, 0.5);
				#else
					fixed4 c = tex2D(_MainTex, IN.ctiuv2_DetailAlbedoMap);
					fixed4 bumpSpecAO = tex2D(_BumpSpecAOMap, IN.ctiuv2_DetailAlbedoMap);
				//	Fade out base texture (details)
					#if defined(GEOM_TYPE_BRANCH_DETAIL) && defined(LOD_FADE_PERCENTAGE)
						c = lerp(c, _AverageCol, unity_LODFade.x);
						bumpSpecAO = lerp(bumpSpecAO, fixed4(unity_ColorSpaceDielectricSpec.r, 0.5, 1, 0.5), unity_LODFade.x);
					#endif
				#endif
		
		//	Regular UVs		
			#else
			//	Skip base textures (details)
				#if defined(GEOM_TYPE_FROND)
					fixed4 c = _AverageCol;
					fixed4 bumpSpecAO = fixed4(unity_ColorSpaceDielectricSpec.r, 0.5, 1, 0.5);
				#else
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
					fixed4 bumpSpecAO = tex2D(_BumpSpecAOMap, IN.uv_MainTex);
				//	Fade out base texture (details)
					#if defined(GEOM_TYPE_BRANCH_DETAIL) && defined(LOD_FADE_PERCENTAGE)
						c = lerp(c, _AverageCol, unity_LODFade.x);
						bumpSpecAO = lerp(bumpSpecAO, fixed4(unity_ColorSpaceDielectricSpec.r, 0.5, 1, 0.5), unity_LODFade.x);
					#endif
				#endif
			#endif

			o.Albedo = lerp(c.rgb, (c.rgb + _HueVariation.rgb) * 0.5, IN.color.r * _HueVariation.a);
			o.Smoothness = c.a * _HueVariation.r;

			o.Occlusion = lerp(1, bumpSpecAO.b * IN.color.a, _OcclusionStrength);
			
			o.Alpha = c.a;
			o.Specular = bumpSpecAO.r;

			#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND)

				#if !defined (_SWAP_UVS)
					fixed4 detailAlbedo = tex2D(_DetailAlbedoMap, IN.ctiuv2_DetailAlbedoMap);
					fixed4 detailNormalSample = tex2D(_DetailNormalMapX, IN.ctiuv2_DetailAlbedoMap);
				#else
					fixed4 detailAlbedo = tex2D(_DetailAlbedoMap, IN.uv_MainTex);
					fixed4 detailNormalSample = tex2D(_DetailNormalMapX, IN.uv_MainTex);
				#endif
				
			//  Here we use a custom function to make sure it works in all versions of Unity
                half3 detailNormalTangent = CTI_UnpackScaleNormal( detailNormalSample, _DetailNormalMapScale);

				o.Albedo *= detailAlbedo.rgb * unity_ColorSpaceDouble.rgb;
				o.Smoothness = (o.Smoothness + detailAlbedo.a) * 0.5;
				o.Specular = (o.Specular + detailNormalSample.rrr) * 0.5;
				o.Occlusion *= detailNormalSample.b;

			#endif
			
		//	Get normal
			#if defined(GEOM_TYPE_FROND)
				o.Normal = detailNormalTangent;
			#else
                o.Normal = UnpackNormalDXT5nm(bumpSpecAO);
			#endif
		//	Fade out base texture (details)
			#if defined(GEOM_TYPE_BRANCH_DETAIL) && defined(LOD_FADE_PERCENTAGE)
				o.Normal = lerp(o.Normal, half3(0,0,1), unity_LODFade.x);
			#endif
		//	Blend normals from UV2
			#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL)
				o.Normal = BlendNormals(o.Normal, detailNormalTangent);
			#endif

		}
	ENDCG

	// Pass to render object as a shadow caster
	// Do not forget to setup the instance ID!
	Pass{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma multi_compile_shadowcaster
		#pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma multi_compile __ _METALLICGLOSSMAP
		#pragma multi_compile_instancing
		//#if UNITY_VERSION >= 550
			#pragma instancing_options assumeuniformscaling lodfade procedural:setup
		//#endif
		
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define UNITY_PASS_SHADOWCASTER
		// #include "UnityBuiltin3xTreeLibrary.cginc" // We can not do this as we want instancing
		#define IS_BARK
		#define DEPTH_NORMAL
		#define IS_LODTREE
		#include "Includes/CTI_Builtin4xTreeLibraryTumbling_301.cginc"
		#include "Includes/CTI_indirect.cginc"

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			UNITY_DITHER_CROSSFADE_COORDS_IDX(2)
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f_surf vert_surf(appdata_ctitree v) {
			v2f_surf o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			CTI_TreeVertBark(v);
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)
			return o;
		}

		float4 frag_surf(v2f_surf IN) : SV_Target {
			UNITY_SETUP_INSTANCE_ID(IN);
			#if UNITY_VERSION < 2017
				UNITY_APPLY_DITHER_CROSSFADE(IN)
			#else
				UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
			#endif
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}

///
}

CustomEditor "CTI_ShaderGUI"

}
