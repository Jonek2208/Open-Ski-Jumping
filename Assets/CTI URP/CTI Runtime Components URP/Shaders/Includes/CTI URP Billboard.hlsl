#if defined(UNITY_PASS_SHADOWCASTER)
	uniform float4 unity_BillboardCameraParams;
	#define unity_BillboardCameraPosition (unity_BillboardCameraParams.xyz)
#endif

	float3 unity_BillboardSize;
	//float4 _LightDirection;
	float4 _CTI_SRP_Wind;
	float _CTI_SRP_Turbulence;

#if defined(_PARALLAXMAP)
	float2 _CTI_TransFade;
#endif

struct CTIVertexBBInput {
	float4 positionOS : POSITION;
	float3 normalOS : NORMAL;
	float4 tangentOS : TANGENT;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	half4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct CTIVertexBBOutput {
	float4 positionCS	: SV_POSITION;
	float2 uv           : TEXCOORD0;

	#if !defined(UNITY_PASS_SHADOWCASTER) && !defined(DEPTHONLYPASS)
        //DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
        half3 vertexSH                  : TEXCOORD1;
        //#ifdef _ADDITIONAL_LIGHTS
            float3 positionWS           : TEXCOORD2;
        //#endif
        #ifdef _NORMALMAP
            half4 normalWS              : TEXCOORD3;    // xyz: normal, w: viewDir.x
            half4 tangentWS             : TEXCOORD4;    // xyz: tangent, w: viewDir.y
            half4 bitangentWS           : TEXCOORD5;    // xyz: binormal, w: viewDir.z
        #else
            half3 normalWS              : TEXCOORD3;
            half3 viewDirWS             : TEXCOORD4;
        #endif
        half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light
        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            float4 shadowCoord          : TEXCOORD7;
        #endif
        half colorVariation             : TEXCOORD8;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

float4 SmoothCurve(float4 x) {
	return x * x * (3.0 - 2.0 * x);
}

float4 TriangleWave(float4 x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}

float4 AfsSmoothTriangleWave(float4 x) {
	return (SmoothCurve(TriangleWave(x)) - 0.5) * 2.0;
}

#define UNITY_PI 3.1415927f

// Billboard Vertex Function
void CTIBillboardVert (inout CTIVertexBBInput v, float3 lightDir) {

	float4 position = v.positionOS;
	float3 worldPos = v.positionOS.xyz + UNITY_MATRIX_M._m03_m13_m23;

//	Store Color Variation
	#if !defined(UNITY_PASS_SHADOWCASTER)
		float3 TreeWorldPos = abs(worldPos.xyz * 0.125f);
		v.color.r = saturate((frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac((TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3)) * 0.5);
	#endif

	#if defined(_PARALLAXMAP)
		float3 distVec = _WorldSpaceCameraPos - worldPos;
		float distSq = dot(distVec, distVec);
		v.color.b = saturate( (_CTI_TransFade.x - distSq) / _CTI_TransFade.y);
	#endif

// 	////////////////////////////////////
//	Set vertex position
	#if defined(UNITY_PASS_SHADOWCASTER)
		float3 eyeVec = -lightDir;
	#else
		float3 eyeVec = normalize(_WorldSpaceCameraPos - worldPos);
	#endif

	float3 billboardTangent = normalize(float3(-eyeVec.z, 0, eyeVec.x));
	float3 billboardNormal = float3(billboardTangent.z, 0, -billboardTangent.x);	// cross({0,1,0},billboardTangent)
	float2 percent = v.texcoord.xy;
	float3 billboardPos = (percent.x - 0.5) * unity_BillboardSize.x * v.texcoord1.x * billboardTangent;

	//billboardPos.y += (percent.y * unity_BillboardSize.y * 2.0 + unity_BillboardSize.z) * v.texcoord1.y;
	// nope: not y * 2 other wise billbords get culled too early: double the height in the bb asset!
	billboardPos.y += (percent.y * unity_BillboardSize.y * _BillboardScale + unity_BillboardSize.z) * v.texcoord1.y;


	position.xyz += billboardPos;
	v.positionOS.xyz = position.xyz;
	v.positionOS.w = 1.0f;

//	Wind
	//#if defined(_EMISSION)
		worldPos.xyz = abs(worldPos.xyz * 0.125f);
		float sinuswave = _SinTime.z;
		float4 vOscillations = AfsSmoothTriangleWave(float4(worldPos.x + sinuswave, worldPos.z + sinuswave * 0.8, 0.0, 0.0));
		float fOsc = vOscillations.x + (vOscillations.y * vOscillations.y);
		fOsc = 0.75 + (fOsc + 3.33) * 0.33;
	
	//	saturate added to stop warning on dx11...
		v.positionOS.xyz += _CTI_SRP_Wind.w * _CTI_SRP_Wind.xyz * _WindStrength * fOsc * pow(saturate(percent.y), 1.5);	// pow(y,1.5) matches the wind baked to the mesh trees
	//#endif


// 	////////////////////////////////////
//	Get billboard texture coords
	float angle = atan2(billboardNormal.z, billboardNormal.x);	// signed angle between billboardNormal to {0,0,1}
	angle += angle < 0 ? 2 * UNITY_PI : 0;										

//	Set Rotation
	angle += v.texcoord1.z;
//	Write final billboard texture coords
	const float invDelta = 1.0 / (45.0 * ((UNITY_PI * 2.0) / 360.0));
	float imageIndex = fmod(floor(angle * invDelta + 0.5f), 8);
	float2 column_row;
	column_row.x = imageIndex * 0.25; 							// we do not care about the horizontal coord that much as our billboard texture tiles
	column_row.y = saturate(4 - imageIndex) * 0.5;
	v.texcoord.xy = column_row + v.texcoord.xy * float2(0.25, 0.5);

// 	////////////////////////////////////
//	Set Normal and Tangent
	v.normalOS = billboardNormal.xyz;
	v.tangentOS = float4(billboardTangent.xyz, -1.0);
}
