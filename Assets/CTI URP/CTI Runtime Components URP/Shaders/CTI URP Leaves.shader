Shader "CTI/URP LOD Leaves"
{
    Properties
    {

        [Header(Surface Options)]
        [Space(5)]

        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull                           ("Culling", Float) = 0

        
        [Header(Surface Inputs)]
        [Space(5)]
        _HueVariation                   ("Color Variation", Color) = (0.9,0.5,0.0,0.1)
        [Space(5)]
        [NoScaleOffset]
        _BaseMap                        ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Cutoff                         ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Space(5)]
        [Toggle(_NORMALMAP)]
        _ApplyNormal                    ("Enable Normal (AG) Smoothness (B) Trans (R) Map", Float) = 1.0
        [NoScaleOffset]
        _BumpSpecMap                    ("    Normal (AG) Smoothness (B) Trans (R)", 2D) = "white" {}
        _Smoothness                     ("Smoothness", Range(0.0, 1.0)) = 0.5
        _SpecColor                      ("Specular", Color) = (0.2, 0.2, 0.2)
        

        [Header(Transmission)]
        [Space(5)]
        [CTI_URPTransDrawer]
        _Translucency                   ("Strength (X) Power (Y) Distortion (Z)", Vector) = (1, 8, 0.01, 0)
        
        
        [Header(Wind Multipliers)]
        [Space(5)]
        [CTI_URPWindDrawer]
        _BaseWindMultipliers            ("Main (X) Branch (Y) Flutter (Z)", Vector) = (1,1,1,0)

        [Header(Advanced Wind)]
        [Space(5)]
        [Toggle(_LEAFTUMBLING)]
        _EnableLeafTumbling             ("Enable Leaf Tumbling", Float) = 1.0
        _TumbleStrength                 ("    Tumble Strength", Range(-1,1)) = 0
        _TumbleFrequency                ("    Tumble Frequency", Range(0,4)) = 1

        [Toggle(_LEAFTURBULENCE)]
        _EnableLeafTurbulence           ("Enable Leaf Turbulence", Float) = 0.0
        _LeafTurbulence                 ("    Leaf Turbulence", Range(0,4)) = 0.2
        _EdgeFlutterInfluence           ("    Edge Flutter Influence", Range(0,1)) = 0.25

        [Space(5)]
        [Toggle(_NORMALROTATION)]
        _EnableNormalRotation           ("Enable Normal Rotation", Float) = 0.0

        
        [Header(Ambient)]
        [Space(5)]
        _AmbientReflection              ("Ambient Reflection", Range(0, 1)) = 1

        
        [Header(Shadows)]
        [Space(5)]
        [Enum(UnityEngine.Rendering.CullMode)]
        _ShadowCulling                  ("Shadow Caster Culling", Float) = 0
        //_ShadowOffsetBias             ("ShadowOffsetBias", Float) = 1

        
        // Needed by VegetationStudio's Billboard Rendertex Shaders
        [HideInInspector] _IsBark("Is Bark", Float) = 0
        
    }

    SubShader
    {

        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue"="AlphaTest"
            "DisableBatching" = "LODFading"
            "IgnoreProjector" = "True"
        }


//  Base -----------------------------------------------------
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            ZWrite On
//AlphaToMask On
            Cull [_Cull]
//ZTest Equal

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON 1
            #define _SPECULAR_SETUP

            #define CTILEAVES
            #pragma shader_feature_local _LEAFTUMBLING
            #pragma shader_feature_local _LEAFTURBULENCE
            #pragma shader_feature _NORMALMAP

            #pragma shader_feature_local _NORMALROTATION

            // -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            //#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            //#pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling maxcount:50

            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #pragma multi_compile_vertex LOD_FADE_PERCENTAGE

            // As Unity 2019.1 will always enable LOD_FADE_CROSSFADE and LOD_FADE_PERCENTAGE
            #if UNITY_VERSION < 201920
                #undef LOD_FADE_CROSSFADE
            #endif

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Includes/CTI URP Bending.hlsl"
            #include "Includes/CTI URP Lighting.hlsl"

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment


/// --------
            void InitializeInputData(CTIVertexOutput input, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;
                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                    inputData.positionWS = input.positionWS;
                #endif
                #ifdef _NORMALMAP
                    half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
                    inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
                #else
                    half3 viewDirWS = input.viewDirWS;
                    inputData.normalWS = input.normalWS;
                #endif

                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                viewDirWS = SafeNormalize(viewDirWS);
                inputData.viewDirectionWS = viewDirWS;

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    inputData.shadowCoord = input.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                #else
                    inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif

                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(input.texcoord1, input.vertexSH, inputData.normalWS);
            }

			CTIVertexOutput LitPassVertex(CTIVertexInput input)
			{
				CTIVertexOutput output = (CTIVertexOutput)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);


                CTI_AnimateVertex(
                    input,
                    #if defined (_BENDINGCOLRSONLY)
                        float4(input.color.rg, input.color.ab), // animParams,
                    #else
                        float4(input.color.rg, input.texcoord1.xy), // animParams,
                    #endif
                    _BaseWindMultipliers
                );

            //  CTI special
                output.occlusionVariation = input.color.ar;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                output.uv.xy = input.texcoord;

                #ifdef _NORMALMAP
                    output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
                    output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
                    output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
                #else
                    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
                    output.viewDirWS = viewDirWS;
                #endif

                //OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                    output.positionWS = vertexInput.positionWS;
                #endif

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = GetShadowCoord(vertexInput);
                #endif

                output.positionCS = vertexInput.positionCS;

				return output;
			}


            half4 LitPassFragment(CTIVertexOutput IN, half facing : VFACE) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(LOD_FADE_CROSSFADE) && !defined(SHADER_API_GLES)
                    LODDitheringTransition(IN.positionCS.xyz, unity_LODFade.x);
                #endif

                SurfaceDescriptionLeaves surfaceData;
            //  Get the surface description / defined in "Includes/CTI LWRP Inputs.hlsl"
                InitializeLeavesLitSurfaceData(IN.occlusionVariation.y, IN.uv.xy, surfaceData);

            //  Add ambient occlusion from vertex input
                surfaceData.occlusion = IN.occlusionVariation.x;


                #if defined(_NORMALMAP)
                    surfaceData.normalTS.z *= facing;
                #else
                    IN.normalWS *= facing;
                #endif 

                InputData inputData;
                InitializeInputData(IN, surfaceData.normalTS, inputData);

            //  Apply lighting
                half4 color = CTILightweightFragmentPBR(
                    inputData, 
                    surfaceData.albedo, 
                    surfaceData.metallic, 
                    surfaceData.specular, 
                    surfaceData.smoothness, 
                    surfaceData.occlusion, 
                    surfaceData.emission, 
                    surfaceData.alpha,
                    _Translucency * half3(surfaceData.translucency, 1, 1),
                    _AmbientReflection);

            //  Add fog
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                return color;
			}

            ENDHLSL
        }

//  Shadows -----------------------------------------------------
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            AlphaToMask On
            Cull [_ShadowCulling]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON 1
            #define CTILEAVES
            #pragma shader_feature_local _LEAFTUMBLING
            #pragma shader_feature_local _LEAFTURBULENCE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling maxcount:50
            
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #pragma multi_compile_vertex LOD_FADE_PERCENTAGE

            // As Unity 2019.1 will always enable LOD_FADE_CROSSFADE and LOD_FADE_PERCENTAGE
            #if UNITY_VERSION < 201920
                #undef LOD_FADE_CROSSFADE
            #endif

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Includes/CTI URP Bending.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;

            CTIVertexOutput ShadowPassVertex(CTIVertexInput input)
            {
                CTIVertexOutput output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                CTI_AnimateVertex(
                    input,
                    #if defined (_BENDINGCOLRSONLY)
                        float4(input.color.rg, input.color.ab), // animParams,
                    #else
                        float4(input.color.rg, input.texcoord1.xy), // animParams,
                    #endif
                    _BaseWindMultipliers
                ); 

                output.uv = input.texcoord;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldDir(input.normalOS);

                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                #if UNITY_REVERSED_Z
                    output.positionCS.z = min(output.positionCS.z, output.positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    output.positionCS.z = max(output.positionCS.z, output.positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                return output;
            }

            half4 ShadowPassFragment(CTIVertexOutput IN) : SV_TARGET {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(LOD_FADE_CROSSFADE) && !defined(SHADER_API_GLES)
                   LODDitheringTransition(IN.positionCS.xyz, unity_LODFade.x);
                #endif
                half alpha = SampleAlbedoAlpha(IN.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a;

                clip(alpha - _Cutoff);
                return 1;
            }
            ENDHLSL
        }

//  Depth -----------------------------------------------------
        Pass
        {
            Name "DepthOnly"
            Tags {"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            AlphaToMask On
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON 1
            #define CTILEAVES
            #pragma shader_feature_local _LEAFTUMBLING
            #pragma shader_feature_local _LEAFTURBULENCE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling maxcount:50
            
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #pragma multi_compile_vertex LOD_FADE_PERCENTAGE

            // As Unity 2019.1 will always enable LOD_FADE_CROSSFADE and LOD_FADE_PERCENTAGE
            #if UNITY_VERSION < 201920
                #undef LOD_FADE_CROSSFADE
            #endif

            #define DEPTHONLYPASS

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Includes/CTI URP Bending.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CTIVertexOutput DepthOnlyVertex(CTIVertexInput input)
            {
                CTIVertexOutput output = (CTIVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                CTI_AnimateVertex(
                    input,
                    #if defined (_BENDINGCOLRSONLY)
                        float4(input.color.rg, input.color.ab), // animParams,
                    #else
                        float4(input.color.rg, input.texcoord1.xy), // animParams,
                    #endif
                    _BaseWindMultipliers
                ); 

                VertexPositionInputs vertexPosition = GetVertexPositionInputs(input.positionOS);
                output.uv.xy = input.texcoord;
                output.positionCS = vertexPosition.positionCS;
                return output;
            }

            half4 DepthOnlyFragment(CTIVertexOutput IN) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(LOD_FADE_CROSSFADE) && !defined(SHADER_API_GLES) // enable dithering LOD transition if user select CrossFade transition in LOD group
                    LODDitheringTransition(IN.positionCS.xyz, unity_LODFade.x);
                #endif
                half alpha = SampleAlbedoAlpha(IN.uv.xy, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a;

                clip(alpha - _Cutoff);
                return 1;
            }

            ENDHLSL
        }

//  Selection = Depth -----------------------------------------------------
        Pass
        {
            Name "SceneSelectionPass"
            Tags{"LightMode" = "SceneSelectionPass"}
        
            ZWrite On
            ColorMask 0
            AlphaToMask On
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON 1
            #define CTILEAVES
            #pragma shader_feature_local _LEAFTUMBLING
            #pragma shader_feature_local _LEAFTURBULENCE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling maxcount:50
            
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #pragma multi_compile_vertex LOD_FADE_PERCENTAGE

            // As Unity 2019.1 will always enable LOD_FADE_CROSSFADE and LOD_FADE_PERCENTAGE
            #if UNITY_VERSION < 201920
                #undef LOD_FADE_CROSSFADE
            #endif

            #define DEPTHONLYPASS

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Includes/CTI URP Bending.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CTIVertexOutput DepthOnlyVertex(CTIVertexInput input)
            {
                CTIVertexOutput output = (CTIVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                CTI_AnimateVertex(
                    input,
                    #if defined (_BENDINGCOLRSONLY)
                        float4(input.color.rg, input.color.ab), // animParams,
                    #else
                        float4(input.color.rg, input.texcoord1.xy), // animParams,
                    #endif
                    _BaseWindMultipliers
                ); 

                VertexPositionInputs vertexPosition = GetVertexPositionInputs(input.positionOS);
                output.uv.xy = input.texcoord;
                output.positionCS = vertexPosition.positionCS;

                return output;
            }

            half4 DepthOnlyFragment(CTIVertexOutput IN) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(LOD_FADE_CROSSFADE) && !defined(SHADER_API_GLES) // enable dithering LOD transition if user select CrossFade transition in LOD group
                    LODDitheringTransition(IN.positionCS.xyz, unity_LODFade.x);
                #endif
                half alpha = SampleAlbedoAlpha(IN.uv.xy, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a;

                clip(alpha - _Cutoff);
                return 1;
            }

            ENDHLSL
        }

//  Meta -----------------------------------------------------
        Pass
        {
            Tags {"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta

            #define _SPECULAR_SETUP

            #pragma shader_feature _SPECGLOSSMAP

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "CTI_URP_ShaderGUI"
    FallBack "Hidden/InternalErrorShader"
}