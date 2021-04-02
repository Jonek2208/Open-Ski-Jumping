#ifndef CTI_TRANSLUCENT_LIGHTING_INCLUDED
#define CTI_TRANSLUCENT_LIGHTING_INCLUDED

#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityLightingCommon.cginc"
#include "UnityGlobalIllumination.cginc"

//-------------------------------------------------------------------------------------
// Compatibilty settings
// Uncomment either "#define USEALLOY" or "#define USEUBER" to enable deferred lighting support for the given shader package.
// Leave them commented in case you are using the Lux Foliage deferred lighting shader.
// More infos in the docs.

// #define USEALLOY
// #define USEUBER

//-------------------------------------------------------------------------------------
// Default BRDF to use:
#if !defined (UNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
	// the only exception is WebGL in 5.3 - it will be built with shader target 2.0 but we want it to get rid of constraints, as it is effectively desktop
	#if SHADER_TARGET < 30 && !UNITY_53_SPECIFIC_TARGET_WEBGL
		#define UNITY_BRDF_PBS BRDF3_Unity_PBS
	#elif UNITY_PBS_USE_BRDF3
		#define UNITY_BRDF_PBS BRDF3_Unity_PBS
	#elif UNITY_PBS_USE_BRDF2
		#define UNITY_BRDF_PBS BRDF2_Unity_PBS
	#elif UNITY_PBS_USE_BRDF1
		#define UNITY_BRDF_PBS BRDF1_Unity_PBS
	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
		#define UNITY_BRDF_PBS BRDF1_Unity_PBS
	#else
		#error something broke in auto-choosing BRDF
	#endif
#endif


//-------------------------------------------------------------------------------------
// BRDF for lights extracted from *indirect* directional lightmaps (baked and realtime).
// Baked directional lightmap with *direct* light uses UNITY_BRDF_PBS.
// For better quality change to BRDF1_Unity_PBS.
// No directional lightmaps in SM2.0.

// CAUTION: This is deprecated and not use in Untiy shader code, but some asset store plugin still use it, so let here for compatibility


#if !defined(UNITY_BRDF_PBS_LIGHTMAP_INDIRECT)
	#define UNITY_BRDF_PBS_LIGHTMAP_INDIRECT BRDF2_Unity_PBS
#endif
#if !defined (UNITY_BRDF_GI)
	#define UNITY_BRDF_GI BRDF_Unity_Indirect
#endif

//-------------------------------------------------------------------------------------


inline half3 BRDF_Unity_Indirect (half3 baseColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness, half3 normal, half3 viewDir, half occlusion, UnityGI gi)
{
	half3 c = 0;
	#if defined(DIRLIGHTMAP_SEPARATE)
		gi.indirect.diffuse = 0;
		gi.indirect.specular = 0;

		#ifdef LIGHTMAP_ON
			c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT (baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light2, gi.indirect).rgb * occlusion;
		#endif
		#ifdef DYNAMICLIGHTMAP_ON
			c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT (baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light3, gi.indirect).rgb * occlusion;
		#endif
	#endif
	return c;
}

//-------------------------------------------------------------------------------------

// little helpers for GI calculation

#define UNITY_GLOSSY_ENV_FROM_SURFACE(x, s, data)				\
	Unity_GlossyEnvironmentData g;								\
	g.roughness		= 1 - s.Smoothness;							\
	g.reflUVW		= reflect(-data.worldViewDir, s.Normal);	\


#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
	#define UNITY_GI(x, s, data) x = UnityGlobalIllumination (data, s.Occlusion, s.Normal);
#else
	#define UNITY_GI(x, s, data) 								\
		UNITY_GLOSSY_ENV_FROM_SURFACE(g, s, data);				\
		x = UnityGlobalIllumination (data, s.Occlusion, s.Normal, g);
#endif


//-------------------------------------------------------------------------------------


// Surface shader output structure to be used with physically
// based shading model.

struct SurfaceOutputStandardTranslucent {
	fixed3 Albedo;
	fixed3 Normal;
	half3 Emission;
	half3 Specular;
	half Translucency;
	half TransFade;
	half ScatteringPower;
	half Smoothness;
	half Occlusion;
	fixed Alpha;
};

half 	_AmbientTranslucency;

inline half4 LightingStandardTranslucent (SurfaceOutputStandardTranslucent s, half3 viewDir, UnityGI gi)
{
	s.Normal = normalize(s.Normal);

	// energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
//	Deprecated since Unity 5.6?
//	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
	c.a = outputAlpha;

//	Deprecated
//	Thin Layer Translucency
/*	// Only using dotNL gives us more lively lighting beyond the shadow distance.
	half backlight = saturate( dot(-s.Normal, gi.light.dir) + 0.2);
	half fresnel = (1.0 - backlight) * (1.0 - backlight);
	fresnel *= fresnel;
	//#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
	c.rgb += s.Albedo * backlight * (1.0 - fresnel) * 4.0 * s.Translucency * gi.light.color;
*/

	half transPower = s.ScatteringPower * 10.0f;
	half3 transLightDir = gi.light.dir + s.Normal * 0.01;
	half transDot = dot( transLightDir, -viewDir );
	transDot = exp2(saturate(transDot) * transPower - transPower);
	half3 lightScattering = transDot * gi.light.color * (1.0 - saturate(dot(s.Normal, gi.light.dir)) );
	c.rgb += s.Albedo * 4.0 * s.Translucency
	//	Fade out translucency	
	#if defined(_PARALLAXMAP)
		* s.TransFade
	#endif
	 * lightScattering;

	#if defined (UNITY_PASS_FORWARDBASE)
		half3 backft = ShadeSH9( half4( -s.Normal, 1.0h) );
		// for ()*()*() see: http://www.humus.name/Articles/Persson_LowLevelThinking.pdf
		//c.rgb += (s.Albedo * backft) * (_AmbientTranslucency * saturate(s.Translucency + _AmbientTranslucencyCancellation))  * s.Occlusion;
		c.rgb += (s.Albedo * backft) * (_AmbientTranslucency) * s.Translucency;
	#endif

	return c;
}


inline half4 LightingStandardTranslucent_Deferred (SurfaceOutputStandardTranslucent s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	
//	energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

//	For indirect lighting we simply use the built in BRDF
	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
//	Deprecated since Unity 5.6?
//	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);


	half3 backft = ShadeSH9( half4( -s.Normal, 1.0h) );
	// for ()*()*() see: http://www.humus.name/Articles/Persson_LowLevelThinking.pdf
	//c.rgb += (s.Albedo * backft) * (_AmbientTranslucency * saturate(s.Translucency + _AmbientTranslucencyCancellation))  * s.Occlusion;
	c.rgb += (s.Albedo * backft) * (_AmbientTranslucency) * s.Translucency;

//	Fade out translucency	
	#if defined(_PARALLAXMAP)
		s.Translucency *= s.TransFade;
	#endif

	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);
	
	half4 emission;
//	Alloy Support
	#if defined (USEALLOY)
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		outNormal = half4(s.Normal * 0.5 + 0.5, 1);
		emission = half4(s.Emission + c.rgb, 1.0 - s.Translucency);
//	UBER Support
	#elif defined (USEUBER)
		//outDiffuseOcclusion = half4(half3(1,0,0), s.Occlusion);
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		float translucency = floor(saturate(s.Translucency) * 15) * (-128);
		outNormal = half4(s.Normal * 0.5 + 0.5, 1);
		emission = half4(s.Emission + c.rgb, translucency);
// 	Lux Support
	#else
		outSpecSmoothness = half4(s.Specular.r, s.ScatteringPower, s.Translucency, s.Smoothness);
		s.Normal = normalize(s.Normal);
		// Mark as translucent
		outNormal = half4(s.Normal * 0.5 + 0.5, 0.66);
		emission = half4(s.Emission + c.rgb, 1);
	#endif
	
	return emission;
}

inline void LightingStandardTranslucent_GI (
	SurfaceOutputStandardTranslucent s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

#endif
