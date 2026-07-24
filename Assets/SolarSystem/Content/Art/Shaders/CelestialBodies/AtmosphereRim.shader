Shader "SolarSystem/Celestial/Atmosphere Rim"
{
    Properties
    {
        [MainColor] _AtmosphereColor("Atmosphere Color", Color) = (0.12, 0.46, 1, 1)
        _RimPower("Rim Power", Range(0.5, 12)) = 4.2
        _RimIntensity("Rim Intensity", Range(0, 2)) = 0.38
        _NightsideVisibility("Nightside Visibility", Range(0, 1)) = 0.12
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+10"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "AtmosphereForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex AtmosphereVertex
            #pragma fragment AtmosphereFragment
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _AtmosphereColor;
                half _RimPower;
                half _RimIntensity;
                half _NightsideVisibility;
            CBUFFER_END

            float4 _SolarSystemSunPositionWS;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half fogFactor : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings AtmosphereVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positions =
                    GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positions.positionCS;
                output.positionWS = positions.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.fogFactor = ComputeFogFactor(positions.positionCS.z);
                return output;
            }

            half4 AtmosphereFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 viewDirection =
                    GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half rim = pow(
                    saturate(1 - dot(normalWS, viewDirection)),
                    _RimPower);
                half sunFacing = saturate(
                    dot(normalWS, sunDirection) * 0.5h + 0.5h);
                half scattering = lerp(
                    _NightsideVisibility,
                    1,
                    sunFacing);
                half alpha = saturate(rim * _RimIntensity * scattering);
                half3 color = MixFog(_AtmosphereColor.rgb, input.fogFactor);
                return half4(color * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
