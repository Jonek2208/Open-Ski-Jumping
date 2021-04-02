#if defined(UNITY_PASS_SHADOWCASTER)
	uniform float4 unity_BillboardCameraParams;
	#define unity_BillboardCameraPosition (unity_BillboardCameraParams.xyz)
#endif

float3 unity_BillboardSize;

#if defined(_EMISSION)
	float4 _TerrainLODWind;
	float _WindStrength;
#endif

#if defined(_PARALLAXMAP)
	float2 _CTI_TransFade;
#endif

struct appdata_bb {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	fixed4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Input {
	float2 uv_MainTex;
	fixed4 color;
	UNITY_DITHER_CROSSFADE_COORDS
};

float SmoothCurve(float4 x) {
	return x * x * (3.0 - 2.0 * x);
}

float4 TriangleWave(float4 x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}

float4 AfsSmoothTriangleWave(float4 x) {
	return (SmoothCurve(TriangleWave(x)) - 0.5) * 2.0;
}

// Billboard Vertex Function
void AFSBillboardVert (inout appdata_bb v) {

	float4 position = v.vertex;
	float3 worldPos = v.vertex.xyz + float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
//	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

	// Add some kind of self shadowing when looking against sun
	// might need some more love - and looks weird in forward
//	#if defined(UNITY_PASS_SHADOWCASTER)
//		float offset = saturate(1.0 - saturate( dot(normalize(worldPos - _WorldSpaceCameraPos ), _WorldSpaceLightPos0.xyz) ) * 2.0); // ok
//		position.xz -= _WorldSpaceLightPos0.xz * saturate(offset) * _ShadowOffset;
//	#endif

	// Store Color Variation
	#if !defined(UNITY_PASS_SHADOWCASTER)
		float3 TreeWorldPos = abs(worldPos.xyz * 0.125f);
		v.color.r = saturate((frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac((TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3)) * 0.5);
	#endif

	#if defined(_PARALLAXMAP)
		//float3 worldPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
		float3 distVec = _WorldSpaceCameraPos - worldPos;
		float distSq = dot(distVec, distVec);
		v.color.b = saturate( (_CTI_TransFade.x - distSq) / _CTI_TransFade.y);
	#endif

// 	////////////////////////////////////
//	Set vertex position
	#if defined(UNITY_PASS_SHADOWCASTER)
		// We have to distinguish between depth and shadows (forward)
		// this is 0.0 while rendering the shadows but something else when unity renders depth
		float testShadowcasterPass = unity_BillboardCameraPosition.x + unity_BillboardCameraPosition.y + unity_BillboardCameraPosition.z;

		#if defined (SHADOWS_CUBE) || defined (SPOT)
			float3 eyeVec = (testShadowcasterPass == 0.0) ? normalize(_WorldSpaceLightPos0.xyz - worldPos) : normalize(_WorldSpaceCameraPos - worldPos);
		#else
			float3 eyeVec = (testShadowcasterPass == 0.0) ? -_WorldSpaceLightPos0.xyz : normalize(_WorldSpaceCameraPos - worldPos);
		#endif
	#else
		float3 eyeVec = normalize(_WorldSpaceCameraPos - worldPos);
	#endif

	float3 billboardTangent = normalize(float3(-eyeVec.z, 0, eyeVec.x));
	float3 billboardNormal = float3(billboardTangent.z, 0, -billboardTangent.x);	// cross({0,1,0},billboardTangent)

	float2 percent = v.texcoord.xy;
	float3 billboardPos = (percent.x - 0.5) * unity_BillboardSize.x * v.texcoord1.x * billboardTangent;

	billboardPos.y += (percent.y * unity_BillboardSize.y * 2.0 + unity_BillboardSize.z) * v.texcoord1.y;

	position.xyz += billboardPos;
	v.vertex.xyz = position.xyz;
	v.vertex.w = 1.0f;

//	Wind
	#if defined(_EMISSION)
		worldPos.xyz = abs(worldPos.xyz * 0.125f);
		float sinuswave = _SinTime.z;
		float4 vOscillations = AfsSmoothTriangleWave(float4(worldPos.x + sinuswave, worldPos.z + sinuswave * 0.8, 0.0, 0.0));
		float fOsc = vOscillations.x + (vOscillations.y * vOscillations.y);
		fOsc = 0.75 + (fOsc + 3.33) * 0.33;
		//v.vertex.xyz += _TerrainLODWind.xyz * fOsc * pow(percent.y, 1.5);			// pow(y,1.5) matches the wind baked to the mesh trees
	
	//	Needed since Unity 5.4., needed in Unity 5.5. as well. Not needed since 5.6. anymore
	//	Detect single billboards which seem to have a very special unity_ObjectToWorld matrix and get corrupted by wind

	 	#if (UNITY_VERSION >= 540 && UNITY_VERSION < 560)
			float3 p1 = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
			float3 p2 = mul(unity_ObjectToWorld, float4(0,0,1,1)).xyz;
			float scale = p2.z - p1.z;
			v.vertex.xyz += _WindStrength * _TerrainLODWind.xyz * fOsc * pow(percent.y, 1.5) / scale;	// pow(y,1.5) matches the wind baked to the mesh trees
		#else
			v.vertex.xyz += _WindStrength * _TerrainLODWind.xyz * fOsc * pow(percent.y, 1.5);	// pow(y,1.5) matches the wind baked to the mesh trees
		#endif
	#endif

// 	////////////////////////////////////
//	Get billboard texture coords
	float angle = atan2(billboardNormal.z, billboardNormal.x);						// signed angle between billboardNormal to {0,0,1}
	angle += angle < 0 ? 2 * UNITY_PI : 0;										

/*
//	SpeedTree Billboards seem to have shrinked uvs, so we expand them // padding seems to be 0.05?
	float minmax = v.texcoord.x * 2 - 1;
	minmax *= 1.0 / 0.85; // (1.0 - 0.06977); // 0.95
	v.texcoord.x = saturate( minmax * 0.5 + 0.5);
//	Adjust texccord to clamped height
	v.texcoord.y *= _TreeHeightLimit;
*/

//	Set Rotation
	angle += v.texcoord1.z;
//	Write final billboard texture coords
	const float invDelta = 1.0 / (45.0 * ((UNITY_PI * 2.0) / 360.0));
	float imageIndex = fmod(floor(angle * invDelta + 0.5f), 8);
	float2 column_row;
	column_row.x = imageIndex * 0.25; // we do not care about the horizontal coord that much as our billboard texture tiles
	//column_row.y = (imageIndex > 3) ? 0 : 0.5;
	column_row.y = saturate(4 - imageIndex) * 0.5;
	v.texcoord.xy = column_row + v.texcoord.xy * float2(0.25, 0.5);

// 	////////////////////////////////////
//	Set Normal and Tangent
	v.normal = billboardNormal.xyz;
	v.tangent = float4(billboardTangent.xyz, -1.0);

	//v.color.b = saturate( 1.0 - dot(eyeVec, billboardNormal.xyz) );

}
