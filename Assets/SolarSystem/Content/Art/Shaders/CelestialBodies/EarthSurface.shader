Shader "SolarSystem/Celestial/Earth Surface"
{
    Properties
    {
        [MainTexture] _BaseMap("Day Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Surface Tint", Color) = (1, 1, 1, 1)
        [Normal] _BumpMap("Surface Normal", 2D) = "bump" {}
        _BumpScale("Normal Strength", Range(0, 1)) = 0.28
        _SpecularMap("Ocean Specular Mask", 2D) = "black" {}
        _LandSpecular("Land Specular", Range(0, 1)) = 0.025
        _OceanSpecular("Ocean Specular", Range(0, 1)) = 0.62
        _LandSmoothness("Land Smoothness", Range(0, 1)) = 0.18
        _OceanSmoothness("Ocean Smoothness", Range(0, 1)) = 0.78
        _NightMap("Night Emission", 2D) = "black" {}
        [HDR] _NightColor("Night Light Tint", Color) = (1.2, 0.68, 0.28, 1)
        _NightIntensity("Night Light Intensity", Range(0, 4)) = 1.1
        _NightFadeStart("Night Fade Start", Range(-1, 1)) = 0.02
        _NightFadeEnd("Night Fade End", Range(-1, 1)) = 0.22
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
            #pragma vertex EarthVertex
            #pragma fragment EarthFragment
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
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_SpecularMap);
            SAMPLER(sampler_SpecularMap);
            TEXTURE2D(_NightMap);
            SAMPLER(sampler_NightMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _BumpScale;
                half _LandSpecular;
                half _OceanSpecular;
                half _LandSmoothness;
                half _OceanSmoothness;
                half4 _NightColor;
                half _NightIntensity;
                half _NightFadeStart;
                half _NightFadeEnd;
            CBUFFER_END

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

            Varyings EarthVertex(Attributes input)
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

            half4 EarthFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half3 normalTS = UnpackNormalScale(
                    SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv),
                    _BumpScale);
                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 tangentWS = normalize(input.tangentWS.xyz);
                half3 bitangentWS =
                    input.tangentWS.w * cross(normalWS, tangentWS);
                half3x3 tangentToWorld =
                    half3x3(tangentWS, bitangentWS, normalWS);
                normalWS = NormalizeNormalPerPixel(
                    TransformTangentToWorld(normalTS, tangentToWorld));

                half4 daySample =
                    SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) *
                    _BaseColor;
                half oceanMask = saturate(
                    SAMPLE_TEXTURE2D(
                        _SpecularMap,
                        sampler_SpecularMap,
                        input.uv).r);
                half3 nightSample =
                    SAMPLE_TEXTURE2D(_NightMap, sampler_NightMap, input.uv).rgb;

                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half sunFacing = dot(normalWS, sunDirection);
                half nightWeight = smoothstep(
                    _NightFadeStart,
                    _NightFadeEnd,
                    -sunFacing);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = daySample.rgb;
                surfaceData.specular = lerp(
                    _LandSpecular.xxx,
                    _OceanSpecular.xxx,
                    oceanMask);
                surfaceData.metallic = 0;
                surfaceData.smoothness = lerp(
                    _LandSmoothness,
                    _OceanSmoothness,
                    oceanMask);
                surfaceData.normalTS = normalTS;
                surfaceData.emission =
                    nightSample * _NightColor.rgb * _NightIntensity * nightWeight;
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
