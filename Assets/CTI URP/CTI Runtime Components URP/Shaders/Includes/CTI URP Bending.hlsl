float4 _CTI_SRP_Wind;
float _CTI_SRP_Turbulence;


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

    return float3x3 (   oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                        oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
                        oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);   
}

void CTI_AnimateVertex( inout CTIVertexInput v, float4 animParams, float3 baseWindMultipliers) {  

    // animParams.x = branch phase
    // animParams.y = edge flutter factor
    // animParams.z = primary factor UV2.x
    // animParams.w = secondary factor UV2.y

     
    float fDetailAmp = 0.1f;
    float fBranchAmp = 0.3f;

    // float3 TreeWorldPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
    float3 TreeWorldPos = UNITY_MATRIX_M._m03_m13_m23; //SHADERGRAPH_OBJECT_POSITION; //float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w);
    
    TreeWorldPos.xyz = abs(TreeWorldPos.xyz * 0.125f);
    float sinuswave = _SinTime.z;

    //#if defined (_LEAFTUMBLING)
    //    float shiftedsinuswave = sin(_Time.y * 0.5 + _TimeOffset);
    //    float4 vOscillations = AfsSmoothTriangleWave(float4(TreeWorldPos.x + sinuswave, TreeWorldPos.z + sinuswave * 0.7, TreeWorldPos.x + shiftedsinuswave, TreeWorldPos.z + shiftedsinuswave * 0.8));
    //#else
        float4 vOscillations = AfsSmoothTriangleWave(float4(TreeWorldPos.x + sinuswave, TreeWorldPos.z + sinuswave * 0.7, 0.0, 0.0));
    //#endif

    // x used for main wind bending / y used for tumbling
    float2 fOsc = vOscillations.xz + (vOscillations.yw * vOscillations.yw);
    fOsc = 0.75 + (fOsc + 3.33) * 0.33;

    // cti: float fObjPhase = abs ( frac( (TreeWorldPos.x + TreeWorldPos.z) * 0.5 ) * 2 - 1 );
    float fObjPhase = dot(TreeWorldPos, 1);
    float fBranchPhase = fObjPhase + animParams.x; // * 3.3;
    float fVtxPhase = dot(v.positionOS.xyz, animParams.y + fBranchPhase);
    
    // x is used for edges; y is used for branches
    float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase );
    
    // 1.975, 0.793, 0.375, 0.193 are good frequencies
    float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
    vWaves = SmoothTriangleWave( vWaves );
    float2 vWavesSum = vWaves.xz + vWaves.yw;

//  --------------

//  Get local Wind
    float4 xWind = _CTI_SRP_Wind;
    xWind.xyz = mul(GetWorldToObjectMatrix(), float4(xWind.xyz, 0)).xyz;
//  Animate Wind
    xWind.w *= fOsc.x;

    animParams.zwy *= baseWindMultipliers.xyz;

//  --------------

    #if defined(CTILEAVES)
    //  Decode UV3
        float3 pivot;
        // 15bit compression 2 components only, important: sign of y
        pivot.xz = (frac(float2(1.0f, 32768.0f) * v.texcoord2.xx) * 2) - 1;
        pivot.y = sqrt(1 - saturate(dot(pivot.xz, pivot.xz)));
        pivot *= v.texcoord2.y;

    //  Move point to 0,0,0
        v.positionOS.xyz -= pivot;


    //  Tumbling
        #if defined(_LEAFTUMBLING) || defined (_LEAFTURBULENCE)
            real3 fracs = frac( pivot * 33.3 ); //fBranchPhase * 0.1); // + pos.w
            real offset = fracs.x + fracs.y + fracs.z;
            real tFrequency = _TumbleFrequency * (_Time.y + fObjPhase * 10.0 );
            real4 vWaves1 = SmoothTriangleWave( real4( (tFrequency + offset) * (1.0 + offset * 0.25), tFrequency * 0.75 + offset, tFrequency * 0.5 + offset, tFrequency * 1.5 + offset));

            #define packedBranchAxis v.texcoord2.z
            #define windDir xWind.xyz
            #define absWindStrength xWind.w

            //#if defined (_EMISSION)
                // This was the root of the fern issue: branchAxes slightly varied on different LODs!
                real3 branchAxis = frac( packedBranchAxis * float3(1.0f, 256.0f, 65536.0f) );
                branchAxis = branchAxis * 2.0 - 1.0;
                branchAxis = normalize(branchAxis);
                // we can do better in case we have the baked branch main axis
                real facingWind = (dot(branchAxis, windDir));
            //#else
            //    half facingWind = (dot(normalize(float3(v.positionOS.x, 0, v.positionOS.z)), windDir)); //saturate 
            //#endif

            real3 windTangent = real3(-windDir.z, windDir.y, windDir.x);
            real twigPhase = vWaves1.x + vWaves1.y + (vWaves1.z * vWaves1.z);

            //float windStrength = dot(abs(xWind.xyz), 1) * tumbleInfluence * (1.35 - facingWind) * xWind.w + absWindStrength; // Use abs(_Wind)!!!!!!
            half tumbleInfluence = frac(v.color.b * 2.0h);
            half windStrength = (1.35h - facingWind) * tumbleInfluence * xWind.w;

        //  turbulence
            #if defined (_LEAFTURBULENCE)
                float angleTurbulence =
                    // center rotation so the leaves rotate leftwards as well as rightwards according to the incoming waves
                    // ((twigPhase + vWaves1.w + fBranchPhase) * 0.2 - 0.5) // not so good to add fBranchPhase here...
                    ((twigPhase + vWaves1.w ) * 0.25 - 0.5)
                    // make rotation strength depend on absWindStrength and all other inputs
                    * 4.0 * absWindStrength * _LeafTurbulence * tumbleInfluence * (0.5 + animParams.w) * saturate(lerp(1.0, animParams.y * 8, _EdgeFlutterInfluence))
                ;
                float3x3 turbulenceRot = AfsRotationMatrix( -branchAxis, angleTurbulence);
                v.positionOS.xyz = mul( turbulenceRot, v.positionOS.xyz);
                #if defined(_NORMALROTATION)
                    v.normalOS = mul(turbulenceRot, v.normalOS);
                #endif
            #endif

            #if defined(_LEAFTUMBLING)
                //float angleTumble = ( windStrength * (twigPhase + fBranchPhase * 0.25) * _TumbleStrength * tumbleInfluence); // * fOsc.y );
                float angleTumble = _TumbleStrength * tumbleInfluence * windStrength * (twigPhase) ;
                float3x3 tumbleRot = AfsRotationMatrix( windTangent, angleTumble);
                v.positionOS.xyz = mul(tumbleRot, v.positionOS.xyz);
                #if defined(_NORMALROTATION)
                    v.normalOS = mul(tumbleRot, v.normalOS);
                #endif
            #endif

        #endif


    //  fade in/out leave planes
        #if defined(LOD_FADE_PERCENTAGE)
            real lodfade = (v.color.b > 0.5) ? 1 : 0;
            if (lodfade) {
                v.positionOS.xyz *= 1.0 - unity_LODFade.x;
            }
        #endif
    //  Move point back to origin
        v.positionOS.xyz += pivot;
    #endif


//  --------------


//  Preserve Length
    float origLength = length(v.positionOS.xyz);

//  Primary bending / Displace position
    v.positionOS.xyz += animParams.z * xWind.xyz   * xWind.w ;

    #if defined(_NORMALIZEBRANCH)
//  Preserve Length - good here but stretches real branches
        v.positionOS.xyz = normalize(v.positionOS.xyz) * origLength;
    #endif

    float3 bend = animParams.y * fDetailAmp * abs(v.normalOS.xyz);
    bend.y = animParams.w * fBranchAmp;

//  Apply secondary bending and edge flutter
    v.positionOS.xyz += ((vWavesSum.xyx * bend) + (xWind.xyz * vWavesSum.y * animParams.w)) * xWind.w  * _CTI_SRP_Turbulence; 

    #if !defined(_NORMALIZEBRANCH)
//  Preserve Length - good here but stretches real branches
        v.positionOS.xyz = normalize(v.positionOS.xyz) * origLength;
    #endif

    //  Store Variation
    #if !defined(UNITY_PASS_SHADOWCASTER) && !defined(DEPTHONLYPASS)
        v.color.r = saturate ( ( frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac( (TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3 ) ) * 0.5 );
    #endif

}