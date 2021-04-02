Shader "CTI/URP LOD Bark"
{
    Properties
    {
        
        [Header(Surface Inputs)]
        [Space(5)]
        _HueVariation                   ("Color Variation", Color) = (0.9,0.5,0.0,0.1)
        [Space(5)]
        [NoScaleOffset] _BaseMap        ("Albedo (RGB) Smoothness (A)", 2D) = "white" {}
        
        [Space(5)]
        [Toggle(_NORMALMAP)]
        _ApplyNormal                    ("Enable Normal (AG) Occlusion (B) Map", Float) = 1.0
        [NoScaleOffset]
        _BumpOcclusionMap               ("    Normal (AG) Occlusion (B)", 2D) = "white" {}

        _Smoothness                     ("Smoothness", Range(0.0, 1.0)) = 1.0
        _SpecColor                      ("Specular", Color) = (0.2, 0.2, 0.2)
        

        [Header(Wind Multipliers)]
        [Space(5)]
        [CTI_URPWindDrawer]
        _BaseWindMultipliers            ("Main (X) Branch (Y) Flutter (Z)", Vector) = (1,1,1,0)     
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue"="Geometry"
            "DisableBatching" = "LODFading"
            "IgnoreProjector" = "True"
        }


//  Base -----------------------------------------------------
        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            ZWrite On
            Cull Back

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature_local _NORMALIZEBRANCH

            #define CTIBARK
            #define _SPECULAR_SETUP

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
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #pragma multi_compile_vertex LOD_FADE_PERCENTAGE

            // As Unity 2019.1 will always enable LOD_FADE_CROSSFADE and LOD_FADE_PERCENTAGE
            #if UNITY_VERSION < 201920
                #undef LOD_FADE_CROSSFADE
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling maxcount:50

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Includes/CTI URP Bending.hlsl"

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

/// --------
            void InitializeInputData(CTIVertexOutput IN, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;
                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                    inputData.positionWS = IN.positionWS;
                #endif
                #ifdef _NORMALMAP
                    half3 viewDirWS = half3(IN.normalWS.w, IN.tangentWS.w, IN.bitangentWS.w);
                    inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(IN.tangentWS.xyz, IN.bitangentWS.xyz, IN.normalWS.xyz));
                #else
                    half3 viewDirWS = IN.viewDirWS;
                    inputData.normalWS = IN.normalWS;
                #endif

                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                viewDirWS = SafeNormalize(viewDirWS);
                inputData.viewDirectionWS = viewDirWS;

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    inputData.shadowCoord = IN.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                #else
                    inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif
                inputData.fogCoord = IN.fogFactorAndVertexLight.x;
                inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(IN.texcoord1, IN.vertexSH, inputData.normalWS);
            }

			CTIVertexOutput LitPassVertex(CTIVertexInput input)
			{
				CTIVertexOutput output = (CTIVertexOutput)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                CTI_AnimateVertex(
                    input,
                    float4(input.color.rg, input.texcoord1.xy), // animParams,
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


            half4 LitPassFragment(CTIVertexOutput IN) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(LOD_FADE_CROSSFADE) && !defined(SHADER_API_GLES)
                    LODDitheringTransition(IN.positionCS.xyz, unity_LODFade.x);
                #endif

                SurfaceData surfaceData;
            //  Get the surface description / defined in "Includes/CTI LWRP Inputs.hlsl"
                InitializeStandardLitSurfaceData(IN.occlusionVariation.y, IN.uv.xy, surfaceData);
            //  Add ambient occlusion from vertex input
                surfaceData.occlusion *= IN.occlusionVariation.x;

                InputData inputData;
                InitializeInputData(IN, surfaceData.normalTS, inputData);

            //  Apply lighting
                half4 color = LightweightFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);
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

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALIZEBRANCH

            #define CTIBARK

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
                    float4(input.color.rg, input.texcoord1.xy), // animParams,
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
            Cull Back

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALIZEBRANCH

            #define CTIBARK

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
                    float4(input.color.rg, input.texcoord1.xy), // animParams,
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
            Cull Back

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALIZEBRANCH

            #define CTIBARK

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
                    float4(input.color.rg, input.texcoord1.xy), // animParams,
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
                return 1;
            }

            ENDHLSL
        }

//  Meta -----------------------------------------------------
        Pass
        {
            Tags{"LightMode" = "Meta"}

            Cull Back

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles

            #pragma vertex LightweightVertexMeta
            #pragma fragment LightweightFragmentMeta

            #define _SPECULAR_SETUP
            #define CTIBARK
            #define BARKMETA

            #pragma shader_feature _SPECGLOSSMAP

            #include "Includes/CTI URP Inputs.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "CTI_URP_ShaderGUI"
    FallBack "Hidden/InternalErrorShader"
}