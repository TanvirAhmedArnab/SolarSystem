Shader "SolarSystem/Celestial/Venus Cloud Deck"
{
    Properties
    {
        [MainTexture] _CloudMap("Anchored Cloud Deck", 2D) = "white" {}
        [MainColor] _CloudColor("Cloud Tint", Color) = (1, 0.9, 0.68, 1)
        _ReliefStrength("Cloud Relief", Range(0, 0.5)) = 0.16
        _SampleDistance("Relief Sample Distance", Range(0.5, 4)) = 1.5
        _AmbientBrightness("Nightside Brightness", Range(0, 1)) = 0.16
        _SunBrightness("Sunlight Brightness", Range(0, 2)) = 1.05
        _Specular("Specular", Range(0, 1)) = 0.05
        _Smoothness("Smoothness", Range(0, 1)) = 0.28
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "VenusCloudDeck"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CloudMap);
            SAMPLER(sampler_CloudMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _CloudMap_ST;
                half4 _CloudColor;
                half _ReliefStrength;
                half _SampleDistance;
                half _AmbientBrightness;
                half _SunBrightness;
                half _Specular;
                half _Smoothness;
            CBUFFER_END

            float4 _CloudMap_TexelSize;
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

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = half4(
                    normalInputs.tangentWS.xyz,
                    input.tangentOS.w * GetOddNegativeScale());
                output.uv = TRANSFORM_TEX(input.uv, _CloudMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half Luminance(half3 color)
            {
                return dot(color, half3(0.2126h, 0.7152h, 0.0722h));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float offset = _CloudMap_TexelSize.y * _SampleDistance;
                half3 anchored = SAMPLE_TEXTURE2D(
                    _CloudMap,
                    sampler_CloudMap,
                    input.uv).rgb;
                half3 north = SAMPLE_TEXTURE2D(
                    _CloudMap,
                    sampler_CloudMap,
                    input.uv + float2(0, offset)).rgb;
                half3 south = SAMPLE_TEXTURE2D(
                    _CloudMap,
                    sampler_CloudMap,
                    input.uv - float2(0, offset)).rgb;
                half slope =
                    (Luminance(north) - Luminance(south)) * _ReliefStrength;

                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 tangentWS = normalize(input.tangentWS.xyz);
                half3 bitangentWS =
                    input.tangentWS.w * cross(normalWS, tangentWS);
                half3 normalTS = normalize(half3(0, slope, 1));
                normalWS = NormalizeNormalPerPixel(
                    TransformTangentToWorld(
                        normalTS,
                        half3x3(tangentWS, bitangentWS, normalWS)));

                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half3 viewDirection =
                    GetWorldSpaceNormalizeViewDir(input.positionWS);
                half sunlight = saturate(dot(normalWS, sunDirection));
                half brightness =
                    _AmbientBrightness + sunlight * _SunBrightness;
                half3 halfDirection =
                    SafeNormalize(sunDirection + viewDirection);
                half specularPower = exp2(2.0h + _Smoothness * 9.0h);
                half specular = pow(
                    saturate(dot(normalWS, halfDirection)),
                    specularPower) * _Specular * sunlight;

                half3 color =
                    anchored * _CloudColor.rgb * brightness + specular;
                color = MixFog(color, input.fogFactor);
                return half4(color, 1);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
