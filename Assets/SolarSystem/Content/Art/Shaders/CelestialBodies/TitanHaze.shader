Shader "SolarSystem/Celestial/Titan Haze"
{
    Properties
    {
        [MainColor] _HazeColor("Haze Color", Color) = (1, 0.56, 0.18, 1)
        _DiskOpacity("Disk Opacity", Range(0, 1)) = 0.64
        _RimIntensity("Rim Intensity", Range(0, 1)) = 0.31
        _RimPower("Rim Power", Range(0.5, 8)) = 2.2
        _NightsideVisibility("Nightside Visibility", Range(0, 1)) = 0.16
        _ForwardScatter("Forward Scatter", Range(0, 1)) = 0.14
        _VariationStrength("Presentation Variation", Range(0, 0.1)) = 0.018
        [HideInInspector] _SimulationPhase("Simulation Phase", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+12"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "TitanHaze"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _HazeColor;
                half _DiskOpacity;
                half _RimIntensity;
                half _RimPower;
                half _NightsideVisibility;
                half _ForwardScatter;
                half _VariationStrength;
                half _SimulationPhase;
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
                half3 normalOS : TEXCOORD2;
                half fogFactor : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
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
                output.normalOS = input.normalOS;
                output.fogFactor = ComputeFogFactor(positions.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 viewDirection =
                    GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half sunFacing = saturate(
                    dot(normalWS, sunDirection) * 0.5h + 0.5h);
                half rim = pow(
                    saturate(1 - dot(normalWS, viewDirection)),
                    _RimPower);
                half forwardScatter = pow(
                    saturate(dot(viewDirection, -sunDirection)),
                    4);
                half variation = 1 + sin(
                    input.normalOS.y * 14 +
                    _SimulationPhase * 6.2831853h) * _VariationStrength;
                half illumination = lerp(
                    _NightsideVisibility,
                    1,
                    sunFacing);
                half alpha = saturate(
                    (_DiskOpacity + rim * _RimIntensity) *
                    illumination *
                    variation);
                half brightness =
                    lerp(0.62h, 1.05h, sunFacing) +
                    forwardScatter * _ForwardScatter;
                half3 color = _HazeColor.rgb * brightness;
                color = MixFog(color, input.fogFactor);
                return half4(color * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
