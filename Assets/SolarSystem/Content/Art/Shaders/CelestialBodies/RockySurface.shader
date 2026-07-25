Shader "SolarSystem/Celestial/Rocky Surface"
{
    Properties
    {
        [MainTexture] _BaseMap("Anchored Surface", 2D) = "white" {}
        [MainColor] _BaseColor("Surface Tint", Color) = (1, 1, 1, 1)
        _ReliefStrength("Source Relief", Range(0, 1)) = 0.28
        _ReliefSampleDistance("Relief Sample Distance", Range(0.5, 4)) = 1.5
        _Specular("Specular", Range(0, 1)) = 0.025
        _Smoothness("Smoothness", Range(0, 1)) = 0.1
        _NightsideReadability("Nightside Readability", Range(0, 0.1)) = 0.018
        _CoverageFallbackColor("Unobserved Coverage Fill", Color) = (0, 0, 0, 1)
        _CoverageFallbackStrength("Unobserved Coverage Fill Strength", Range(0, 1)) = 0
        _CoverageThreshold("Unobserved Coverage Threshold", Range(0, 0.1)) = 0
        [HideInInspector] _Cutoff("Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex RockyVertex
            #pragma fragment RockyFragment
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP

            #define _SPECULAR_SETUP 1
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _ReliefStrength;
                half _ReliefSampleDistance;
                half _Specular;
                half _Smoothness;
                half _NightsideReadability;
                half4 _CoverageFallbackColor;
                half _CoverageFallbackStrength;
                half _CoverageThreshold;
            CBUFFER_END

            float4 _BaseMap_TexelSize;
            float4 _SolarSystemSunPositionWS;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half4 tangentWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                half fogFactor : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings RockyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(input.normalOS, input.tangentOS);
                real tangentSign = input.tangentOS.w * GetOddNegativeScale();

                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS =
                    half4(normalInputs.tangentWS.xyz, tangentSign);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half Luminance(half3 color)
            {
                return dot(color, half3(0.2126h, 0.7152h, 0.0722h));
            }

            half4 RockyFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 sampleOffset =
                    _BaseMap_TexelSize.xy * _ReliefSampleDistance;
                half3 anchored = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv).rgb;
                half coverageMask = 0;
                if (_CoverageFallbackStrength > 0.0001h)
                {
                    half threshold = max(_CoverageThreshold, 0.0001h);
                    coverageMask = 1.0h - smoothstep(
                        threshold * 0.5h,
                        threshold,
                        Luminance(anchored));
                    anchored = lerp(
                        anchored,
                        _CoverageFallbackColor.rgb,
                        coverageMask * _CoverageFallbackStrength);
                }
                half east = Luminance(SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv + float2(sampleOffset.x, 0)).rgb);
                half west = Luminance(SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv - float2(sampleOffset.x, 0)).rgb);
                half north = Luminance(SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv + float2(0, sampleOffset.y)).rgb);
                half south = Luminance(SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv - float2(0, sampleOffset.y)).rgb);
                half observedCoverage = 1.0h - coverageMask;
                half3 normalTS = normalize(half3(
                    (west - east) * _ReliefStrength * observedCoverage,
                    (south - north) * _ReliefStrength * observedCoverage,
                    1));

                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 tangentWS = normalize(input.tangentWS.xyz);
                half3 bitangentWS =
                    input.tangentWS.w * cross(normalWS, tangentWS);
                half3x3 tangentToWorld =
                    half3x3(tangentWS, bitangentWS, normalWS);
                normalWS = NormalizeNormalPerPixel(
                    TransformTangentToWorld(normalTS, tangentToWorld));

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = anchored * _BaseColor.rgb;
                surfaceData.specular = _Specular.xxx;
                surfaceData.metallic = 0;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = normalTS;
                surfaceData.emission = half3(0, 0, 0);
                surfaceData.occlusion = 1;
                surfaceData.alpha = 1;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.positionCS = input.positionCS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS =
                    GetWorldSpaceNormalizeViewDir(input.positionWS);
                inputData.shadowCoord =
                    TransformWorldToShadowCoord(input.positionWS);
                inputData.fogCoord = input.fogFactor;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV =
                    GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                inputData.tangentToWorld = tangentToWorld;

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                half3 sunDirectionWS = normalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half sunFacing = dot(normalWS, sunDirectionWS);
                half nightsideWeight =
                    1.0h - smoothstep(-0.12h, 0.08h, sunFacing);
                color.rgb +=
                    surfaceData.albedo *
                    (_NightsideReadability * nightsideWeight);
                color.rgb = MixFog(color.rgb, input.fogFactor);
                color.a = 1;
                return color;
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }

    FallBack Off
}
