Shader "SolarSystem/Celestial/Solar Corona"
{
    Properties
    {
        _SolarMap("Solar Modulation", 2D) = "white" {}
        [HDR] _CoronaColor("Corona Color", Color) = (6, 2, 0.2, 1)
        _RimPower("Rim Power", Range(0.5, 8)) = 3.2
        _Intensity("Corona Intensity", Range(0, 1)) = 0.22
        _PulseAmplitude("Pulse Amplitude", Range(0, 0.2)) = 0.025
        _FlowStrength("Periodic Flow Strength", Range(0, 0.03)) = 0.012
        [HideInInspector] _SimulationPhase("Simulation Phase", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+20"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "SolarCorona"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            Cull Front
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex CoronaVertex
            #pragma fragment CoronaFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_SolarMap);
            SAMPLER(sampler_SolarMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _SolarMap_ST;
                half4 _CoronaColor;
                half _RimPower;
                half _Intensity;
                half _PulseAmplitude;
                half _FlowStrength;
                half _SimulationPhase;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings CoronaVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS =
                    TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _SolarMap);
                return output;
            }

            half4 CoronaFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                const half TwoPi = 6.28318530718h;
                half phaseAngle = _SimulationPhase * TwoPi;
                half2 flowOffset =
                    half2(sin(phaseAngle), cos(phaseAngle)) * _FlowStrength;
                half modulation = SAMPLE_TEXTURE2D(
                    _SolarMap,
                    sampler_SolarMap,
                    input.uv + flowOffset).r;

                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 viewDirection =
                    GetWorldSpaceNormalizeViewDir(input.positionWS);
                half rim = pow(
                    saturate(1.0h - abs(dot(normalWS, viewDirection))),
                    _RimPower);
                half pulse =
                    1.0h + sin(phaseAngle) * _PulseAmplitude;
                half alpha = saturate(
                    rim *
                    _Intensity *
                    lerp(0.72h, 1.0h, modulation) *
                    pulse);
                half3 premultipliedColor = _CoronaColor.rgb * alpha;
                return half4(premultipliedColor, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
