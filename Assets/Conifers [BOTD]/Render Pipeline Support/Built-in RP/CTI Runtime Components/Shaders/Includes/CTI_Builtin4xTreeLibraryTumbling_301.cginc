// Upgrade NOTE: upgraded instancing buffer 'CTIProperties' to new syntax.

#ifndef CTI_BUILTIN_4X_TREE_LIBRARY_INCLUDED
#define CTI_BUILTIN_4X_TREE_LIBRARY_INCLUDED

#include "Tessellation.cginc"

struct appdata_ctitree {
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float2 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
    float3 texcoord2 : TEXCOORD2;
    fixed4 color : COLOR0;

    #if defined(CTIBARKTESS)
    //UNITY_VERTEX_INPUT_INSTANCE_ID // does not work, so we have to do it manually
	    #if defined(INSTANCING_ON)
	    	#ifdef SHADER_API_PSSL
    			uint instanceID;
    		#else
				uint instanceID : SV_InstanceID;
			#endif
		#endif
	#else
    	UNITY_VERTEX_INPUT_INSTANCE_ID
    #endif
};

#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND)
#if !defined(CTIBARKTESS)
	float4 _DetailAlbedoMap_ST;
#endif
#endif

float3 _BaseWindMultipliers; // x: main, y: turbulence, z: flutter

float _TumbleStrength;
float _TumbleFrequency;
float _TimeOffset;
float _LeafTurbulence;
float _EdgeFlutterInfluence;

float4 _TerrainLODWind;
float _FadeOutAllLeaves;
float _FadeOutWind;

#if defined(GEOM_TYPE_LEAF)
	float2 _AdvancedEdgeBending;
#endif

#if defined(CTITESS)
	float _Tess;
    float _minDist;
    float _maxDist;
    float _ExtrudeRange;
    float _Displacement;
    float _bendBounds;
    sampler2D _DispTex;
#endif

#if defined(DEBUG)
	fixed _Tangents;
#endif

// As we do not include the UnityBuiltin3xTreeLibrary we have to also declare the following params:
fixed4 _HueVariation; //_Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;

// As we do not include the TerrainEngine:

UNITY_INSTANCING_BUFFER_START (CTIProperties)
	UNITY_DEFINE_INSTANCED_PROP (float4, _Wind)
#define _Wind_arr CTIProperties
UNITY_INSTANCING_BUFFER_END(CTIProperties)

CBUFFER_START(CTITerrain)
	// trees
	fixed4 _TreeInstanceColor;
	float4 _TreeInstanceScale;
	float4x4 _TerrainEngineBendTree;
	float4 _SquashPlaneNormal;
	float _SquashAmount;
CBUFFER_END

#if defined(_PARALLAXMAP)
	float2 _CTI_TransFade;
#endif


struct LeafSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Translucency;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

inline half4 LightingTreeLeaf (LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	half nl = dot (s.Normal, lightDir);
	
	half nh = max (0, dot (s.Normal, h));
	half spec = pow (nh, s.Specular * 128.0) * s.Gloss;
	
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	
	// wrap-around diffuse
	nl = max(0, nl * 0.6 + 0.4);
	
	fixed4 c;
	/////@TODO: what is is this multiply 2x here???
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb + spec;
	
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(1, atten, _ShadowStrength);
	#else
	c.rgb *= atten;
	#endif

	c.a = s.Alpha;
	
	return c;
}

// Expand billboard and modify normal + tangent to fit
inline void ExpandBillboard (in float4x4 mat, inout float4 pos, inout float3 normal, inout float4 tangent)
{
	// tangent.w = 0 if this is a billboard
	float isBillboard = 1.0f - abs(tangent.w);
	// billboard normal
	float3 norb = normalize(mul(float4(normal, 0), mat)).xyz;
	// billboard tangent
	float3 tanb = normalize(mul(float4(tangent.xyz, 0.0f), mat)).xyz;
	pos += mul(float4(normal.xy, 0, 0), mat) * isBillboard;
	normal = lerp(normal, norb, isBillboard);
	tangent = lerp(tangent, float4(tanb, -1.0f), isBillboard);
}

inline float4 Squash(in float4 pos)
{
	float3 planeNormal = _SquashPlaneNormal.xyz;
	float3 projectedVertex = pos.xyz - (dot(planeNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * planeNormal;
	pos = float4(lerp(projectedVertex, pos.xyz, _SquashAmount), 1);
	return pos;
}

float4 SmoothCurve( float4 x ) {   
	return x * x *( 3.0 - 2.0 * x );   
}
float4 TriangleWave( float4 x ) {   
	return abs( frac( x + 0.5 ) * 2.0 - 1.0 );   
}
float4 SmoothTriangleWave( float4 x ) {   
	return SmoothCurve( TriangleWave( x ) );   
}

// Overlaods for single float 
float SmoothCurve( float x ) {   
	return x * x *( 3.0 - 2.0 * x );   
}
float TriangleWave( float x ) {   
	return abs( frac( x + 0.5 ) * 2.0 - 1.0 );   
}
float SmoothTriangleWave( float x ) {   
	return SmoothCurve( TriangleWave( x ) );   
}

// End of declarations formerly covered by TerrainEngine.cginc or UnityBuiltin3xTreeLibrary.cginc




struct Input {

	#if defined(CTIBARKTESS)
		float2 uv_MainTex;
		float2 uv2_DetailAlbedoMap;
	#else
		#if !defined(IS_CTIARRAY)	
			float2 uv_MainTex;
		#else
			float2 uv_MainTexArray;
		#endif
	#endif

	#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND)
		float2 ctiuv2_DetailAlbedoMap;
	#endif

//	#if !defined(UNITY_PASS_SHADOWCASTER) && !defined (DEPTH_NORMAL) && !defined(CTIBARKTESS) || defined (DEBUG)
		fixed4 color : COLOR; // color.a = AO
//	#endif
	#if !defined (DEPTH_NORMAL)
		#if UNITY_VERSION < 2017
			#ifdef LOD_FADE_CROSSFADE
			//	CTIBARKTESS needs both – but only screenPos gets setup
				#if defined(CTIBARKTESS)
					float4 screenPos;
				#endif
				half3 ditherScreenPos;
			#endif
		#endif
	#endif
	//UNITY_DITHER_CROSSFADE_COORDS
	#ifdef USE_VFACE
		float FacingSign : FACE;
	#endif

	#if defined (DEBUG)
		float2 my_uv2;
		float3 my_uv3;
	#endif



};

half3 CTI_UnpackScaleNormal(half4 packednormal, half bumpScale)
{
    half3 normal;
    normal.xy = (packednormal.wy * 2 - 1);
    #if (SHADER_TARGET >= 30)
        // SM2.0: instruction count limitation
        // SM2.0: normal scaler is not supported
        normal.xy *= bumpScale;
    #endif
    normal.z = sqrt(1.0f - saturate(dot(normal.xy, normal.xy)));
    return normal;
}

float4 AfsSmoothTriangleWave( float4 x ) {   
	return (SmoothCurve( TriangleWave( x )) - 0.5f) * 2.0f;   
}

// see http://www.neilmendoza.com/glsl-rotation-about-an-arbitrary-axis/
// 13fps
float3x3 AfsRotationMatrix(float3 axis, float angle)
{
    //axis = normalize(axis); // moved to calling function
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0f - c;

    return float3x3	(	oc * axis.x * axis.x + c,			oc * axis.x * axis.y - axis.z * s,	oc * axis.z * axis.x + axis.y * s,
                		oc * axis.x * axis.y + axis.z * s,	oc * axis.y * axis.y + c,			oc * axis.y * axis.z - axis.x * s,
                		oc * axis.z * axis.x - axis.y * s,	oc * axis.y * axis.z + axis.x * s,	oc * axis.z * axis.z + c);   
}

// Scriptable Render Loop
// 7.7fps
float3 Rotate(/*float3 pivot, */float3 position, float3 rotationAxis, float angle)
{
    //rotationAxis = normalize(rotationAxis); // moved to calling function
    float3 cpa = /*pivot + */rotationAxis * dot(rotationAxis, position/* - pivot*/);
    return cpa + ((position - cpa) * cos(angle) + cross(rotationAxis, (position - cpa)) * sin(angle));
}

// https://twitter.com/SebAaltonen/status/878250919879639040

#define FLT_MAX 3.402823466e+38 // Maximum representable floating-point number
float3 FastSign(float3 x)
{
    return saturate(x * FLT_MAX + 0.5f) * 2.0f - 1.0f;
}



// Detail bending

void CTI_AnimateVertex( inout appdata_ctitree v, float4 pos, float3 normal, float4 animParams, float3 pivot, float tumbleInfluence, float4 Wind, float packedBranchAxis) {	
	// animParams.x = branch phase
	// animParams.y = edge flutter factor
	// animParams.z = primary factor UV2.x
	// animParams.w = secondary factor UV2.y

//	Adjust base wind settings
	animParams.zwy *= _BaseWindMultipliers;	

	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f; // 0.3f;

	float fade = (_FadeOutWind == 1 && unity_LODFade.x > 0 ) ? unity_LODFade.x : 1.0;

//	Add extra animation to make it fit speedtree


	float3 TreeWorldPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
//	fern issue / this does not seem to fix the problem... / float3 TreeWorldPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
	TreeWorldPos.xyz = abs(TreeWorldPos.xyz * 0.125f);
	float sinuswave = _SinTime.z;
//	float4 vOscillations = AfsSmoothTriangleWave(float4(TreeWorldPos.x + sinuswave , TreeWorldPos.z + sinuswave * 0.8, 0.0, 0.0));

	#if defined (LEAFTUMBLING)
		float shiftedsinuswave = sin(_Time.y * 0.5 + _TimeOffset);
		float4 vOscillations = AfsSmoothTriangleWave(float4(TreeWorldPos.x + sinuswave, TreeWorldPos.z + sinuswave * 0.7, TreeWorldPos.x + shiftedsinuswave, TreeWorldPos.z + shiftedsinuswave * 0.8));
	#else
		float4 vOscillations = AfsSmoothTriangleWave(float4(TreeWorldPos.x + sinuswave, TreeWorldPos.z + sinuswave * 0.7, 0.0, 0.0));
	#endif
	// vOscillations.xz = lerp(vOscillations.xz, 1, vOscillations.xz );
	// x used for main wind bending / y used for tumbling
	float2 fOsc = vOscillations.xz + (vOscillations.yw * vOscillations.yw);
	fOsc = 0.75 + (fOsc + 3.33) * 0.33;

	Wind *= fade.xxxx; 

	float absWindStrength = length(Wind.xyz);

	// Phases (object, vertex, branch)
	// float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
	// new
	float fObjPhase = abs ( frac( (TreeWorldPos.x + TreeWorldPos.z) * 0.5 ) * 2 - 1 );
	float fBranchPhase = fObjPhase + animParams.x;
	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);
	
	// x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase );
	
	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);

	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;

//	Tumbling / Should be done before all other deformations
	#if defined (LEAFTUMBLING)

		// pos.w: upper bit = lodfade
		// Separate lodfade and twigPhase: lodfade stored in highest bit / twigphase compressed to 7 bits
		// moved to #ifs

		tumbleInfluence = frac(pos.w * 2.0);

		// Move point to 0,0,0
		pos.xyz -= pivot;

		float tumble = (_TumbleStrength == 0) ? 0 : 1;

		if ( (_TumbleStrength || _LeafTurbulence /*> 0*/) && absWindStrength * tumbleInfluence > 0 ) {
			// _Wind.w is turbulence
			// Add variance to the different leaf planes
		
			// good for palms and bananas - but we do it later
			//	float3 fracs = frac( pivot * 33.3 + animParams.x * frac(fObjPhase) * 0.25 ); //fBranchPhase * 0.1); // + pos.w
			// good for trees	 	
	 		float3 fracs = frac( pivot * 33.3 ); //fBranchPhase * 0.1); // + pos.w
	 		float offset = fracs.x + fracs.y + fracs.z;  ;
			float tFrequency = _TumbleFrequency * (_Time.y /* new */ + fObjPhase * 10 );
			// Add different speeds: (1.0 + offset * 0.25)
			// float4 vWaves1 = SmoothTriangleWave( float4( (tFrequency + offset) * (1.0 + offset * 0.25), tFrequency * 0.75 - offset, tFrequency * 0.05 + offset, tFrequency * 1.5 + offset));
			// less sharp
			float4 vWaves1 = SmoothTriangleWave( float4( (tFrequency + offset) * (1.0 + offset * 0.25), tFrequency * 0.75 + offset, tFrequency * 0.5 + offset, tFrequency * 1.5 + offset));
			// float4 vWaves1 = SmoothTriangleWave( float4( (tFrequency + offset), tFrequency * 0.75 - offset, tFrequency * 0.05 + offset, tFrequency * 2.5 + offset));
			float3 windDir = normalize (Wind.xyz);
			

			#if defined (_EMISSION)
// This was the root of the fern issue: branchAxes slightly varied on different LODs!
				float3 branchAxis = frac( packedBranchAxis * float3(1.0f, 256.0f, 65536.0f) );
				branchAxis = branchAxis * 2.0 - 1.0;
branchAxis = normalize(branchAxis);
				// we can do better in case we have the baked branch main axis
				float facingWind = dot(branchAxis, windDir);
			#else
				float facingWind = dot(normalize(float3(pos.x, 0, pos.z)), windDir); //saturate 
			#endif

			float3 windTangent = float3(-windDir.z, windDir.y, windDir.x);
			float twigPhase = vWaves1.x + vWaves1.y + (vWaves1.z * vWaves1.z);
			float windStrength = dot(abs(Wind.xyz), 1) * tumbleInfluence * (1.35 - facingWind) * Wind.w + absWindStrength; // Use abs(_Wind)!!!!!!

		//	turbulence
			#if defined (_EMISSION)
				// if(_LeafTurbulence) {
					float angle =
						// center rotation so the leaves rotate leftwards as well as rightwards according to the incoming waves
						// ((twigPhase + vWaves1.w + fBranchPhase) * 0.2 - 0.5) // not so good to add fBranchPhase here...
						((twigPhase + vWaves1.w ) * 0.25 - 0.5)
						// make rotation strength depend on absWindStrength and all other inputs
						* 4.0 * absWindStrength * _LeafTurbulence * tumbleInfluence * (0.5 + animParams.w) * saturate(lerp(1.0, animParams.y * 8, _EdgeFlutterInfluence))
					;

//branchAxis = normalize(branchAxis); // branch axis should be mostly normalized...
					float3x3 turbulenceRot = AfsRotationMatrix( -branchAxis, angle);
					pos.xyz = mul( turbulenceRot, pos.xyz);
					
					#if defined(_NORMALMAP)
						v.normal = mul( turbulenceRot, v.normal.xyz );
					#endif
					// #else
					//	pos.xyz = Rotate(pos.xyz, -branchAxis, angle);
					// #endif
				//}
			#endif
			
		//	tumbling
			// As used by the debug shader
			#if !defined (EFFECT_HUE_VARIATION)
				//if (_TumbleStrength) {
	//				tumbleInfluence = frac(pos.w * 2.0);
					// + 1 is correct for trees/palm / -1 is correct for fern? allow negative values in the material inspector
					float angleTumble = ( windStrength * (twigPhase + fBranchPhase * 0.25) * _TumbleStrength * tumbleInfluence * fOsc.y );
					
					// windTangent should be normalized
					float3x3 tumbleRot = AfsRotationMatrix( windTangent, angleTumble);
					pos.xyz = mul( tumbleRot, pos.xyz);
					
					#if defined(_NORMALMAP)
						v.normal = mul( tumbleRot, v.normal.xyz );
					#endif
					//#else
					//	pos.xyz = Rotate(pos.xyz, windTangent, angleTumble);
					//#endif
				//}
			#endif
		}
		
	//	crossfade – in case anybody uses it...
//		#if defined(LOD_FADE_CROSSFADE)
//			if (unity_LODFade.x != 0.0 && lodfade == 1.0) {
//				pos.xyz *= unity_LODFade.x;
//			}
//		#endif
	//	fade in/out leave planes
		#if defined(LOD_FADE_PERCENTAGE)
			//float lodfade = ceil(pos.w - 0.51);
			//float lodfade = (pos.w > 0.5) ? 1 : 0;
			float lodfade = (pos.w > (1.0f/255.0f*126.0f) ) ? 1 : 0; // Make sure that the 1st vertex is taken into account
			//lodfade += _FadeOutAllLeaves;
			//if (/*unity_LODFade.x < 1.0 && */ lodfade) {
				pos.xyz *= 1.0 - unity_LODFade.x * lodfade;
			//}
		#endif
		// Move point back to origin
		pos.xyz += pivot;
	#endif

//	Advanced edge fluttering (has to be outside preserve length)
	#if defined(GEOM_TYPE_LEAF)
		#if !defined(IS_BARK)
			pos.xyz += normal.xyz * SmoothTriangleWave( tumbleInfluence * _Time.y * _AdvancedEdgeBending.y + animParams.x ) * _AdvancedEdgeBending.x * animParams.y * absWindStrength;
		#endif
	#endif

//	Preserve Length
	float origLength = length(pos.xyz);

	Wind.xyz *= fOsc.x;

	// Edge (xz) and branch bending (y)
	#if !defined(IS_BARK)
		#if !defined(GEOM_TYPE_LEAF)
			float3 bend = animParams.y * fDetailAmp * normal.xyz
			#if !defined(USE_VFACE)
				* FastSign(normal)
			#endif
			;
		#else
			float3 bend = float3(0,0,0);
		#endif
		// Old style turbulence // bend.y = (animParams.w + animParams.y * _LeafTurbulence) * fBranchAmp;
		bend.y = (animParams.y + animParams.w) * fBranchAmp;
	#else
		float3 bend = float3(0,0,0);
		bend.y = (animParams.w) * fBranchAmp;
	#endif

//	This gets never zero even if there is no wind. So we have to multiply it by length(Wind.xyz)
	// if not disabled in debug shader
	#if !defined(EFFECT_BUMP)
		// this is still fucking sharp!!!!!
		pos.xyz += ( ((vWavesSum.xyx * bend) + (Wind.xyz * vWavesSum.y * animParams.w)) * Wind.w) * absWindStrength;
	#endif

//	Primary bending / Displace position
	#if !defined (ENABLE_WIND)
		pos.xyz += animParams.z * Wind.xyz;
	#endif

//	Preserve Length
	pos.xyz = normalize(pos.xyz) * origLength;
	v.vertex.xyz = pos.xyz;

//	Store Variation
	#if !defined(UNITY_PASS_SHADOWCASTER) && defined (IS_LODTREE) && !defined (DEBUG)
		v.color.r = saturate ( ( frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac( (TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3 ) ) * 0.5 );
	#endif
}


// ---------------------------
// Leaves

#if !defined(IS_BARK)
	#if defined (DEPTH_NORMAL)
	void CTI_TreeVertLeaf (inout appdata_ctitree v)
	{
	#else
	void CTI_TreeVertLeaf (inout appdata_ctitree v, out Input OUT)
	{
		UNITY_INITIALIZE_OUTPUT(Input, OUT);
	#endif
		
		#if !defined(IS_LODTREE)
			ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
			v.vertex.xyz *= _TreeInstanceScale.xyz;
		#endif
		
		//	Decode UV3
		float3 pivot;
		#if defined(LEAFTUMBLING)
			// 15bit compression 2 components only, important: sign of y
			pivot.xz = (frac(float2(1.0f, 32768.0f) * v.texcoord2.xx) * 2) - 1;
			pivot.y = sqrt(1 - saturate(dot(pivot.xz, pivot.xz)));
			pivot *= v.texcoord2.y;
			#if !defined(IS_LODTREE)
				pivot *= _TreeInstanceScale.xyz;
			#endif
		#endif
		#if defined(_METALLICGLOSSMAP)
			float4 TerrainLODWind = _TerrainLODWind;
			TerrainLODWind.xyz = mul((float3x3)unity_WorldToObject, _TerrainLODWind.xyz);
			CTI_AnimateVertex( v, float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), pivot, v.color.b, TerrainLODWind, v.texcoord2.z);
		#else
			CTI_AnimateVertex(v, float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), pivot, v.color.b, UNITY_ACCESS_INSTANCED_PROP(_Wind_arr, _Wind), v.texcoord2.z);
		#endif

		#if !defined(IS_LODTREE)
			v.vertex = Squash(v.vertex);
			#if !defined(UNITY_PASS_SHADOWCASTER) && !defined (DEBUG)
				v.color.rgb = _TreeInstanceColor.rgb; // *_Color.rgb;
			#endif
		#endif
		v.normal = normalize(v.normal);
		v.tangent.xyz = normalize(v.tangent.xyz);

		//UNITY_TRANSFER_DITHER_CROSSFADE(OUT, v.vertex)
		#if UNITY_VERSION < 2017
			#if !defined(DEPTH_NORMAL)
				#ifdef LOD_FADE_CROSSFADE
					OUT.ditherScreenPos = ComputeDitherScreenPos(UnityObjectToClipPos(v.vertex));
				#endif
			#endif
		#endif

		#if defined(_PARALLAXMAP)
			float3 worldPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
			float3 distVec = _WorldSpaceCameraPos - worldPos;
			float distSq = dot(distVec, distVec);
			v.color.b = saturate( (_CTI_TransFade.x - distSq) / _CTI_TransFade.y);
		#endif

		#if defined (DEBUG)
			OUT.my_uv2 = v.texcoord1.xy;
			OUT.my_uv3 = v.texcoord2.xyz;
			// tumbleinfluence
			//v.color.b = saturate( frac(v.color.b * 2.0) - 10/255);
			v.color.b = frac(v.color.b * 2.0);

			if (_Tangents == 1) {
				OUT.my_uv3 = v.tangent.xyz;
			}
		#endif
	}
#endif


// ---------------------------
// Bark


#if defined (DEPTH_NORMAL)
	void CTI_TreeVertBark (inout appdata_ctitree v)
	{
#else
	void CTI_TreeVertBark (inout appdata_ctitree v, out Input OUT)
	{
		UNITY_INITIALIZE_OUTPUT(Input, OUT);
#endif

#if !defined(DEPTH_NORMAL)
	#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_BRANCH_DETAIL) || defined(GEOM_TYPE_FROND)
		OUT.ctiuv2_DetailAlbedoMap = v.texcoord1.zw; // * _DetailAlbedoMap_ST.xy;
	#endif
#endif

	#if !defined(IS_LODTREE)
		ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
		v.vertex.xyz *= _TreeInstanceScale.xyz;
	#endif
	#if defined(_METALLICGLOSSMAP)
		float4 TerrainLODWind = _TerrainLODWind;
		TerrainLODWind.xyz = mul((float3x3)unity_WorldToObject, _TerrainLODWind.xyz);
		CTI_AnimateVertex( v, float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), float3(0,0,0), 0, TerrainLODWind, 0); //v.texcoord2.z);
	#else
		CTI_AnimateVertex( v, float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), float3(0,0,0), 0, UNITY_ACCESS_INSTANCED_PROP(_Wind_arr, _Wind), 0); //v.texcoord2.z);
	#endif
	#if !defined(IS_LODTREE)
		v.vertex = Squash(v.vertex);
		#if !defined(UNITY_PASS_SHADOWCASTER) && !defined (DEBUG)
			v.color.rgb = _TreeInstanceColor.rgb; // *_Color.rgb;
		#endif
	#endif
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);

	#if UNITY_VERSION < 2017
		#if !defined(DEPTH_NORMAL) && !defined(CTIBARKTESS)
			#ifdef LOD_FADE_CROSSFADE
				OUT.ditherScreenPos = ComputeDitherScreenPos(UnityObjectToClipPos(v.vertex));
			#endif
		#endif
	#endif
}



#endif // CTI_BUILTIN_4X_TREE_LIBRARY_INCLUDED
