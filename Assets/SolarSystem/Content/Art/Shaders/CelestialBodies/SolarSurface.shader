Shader "SolarSystem/Celestial/Solar Surface"
{
    Properties
    {
        [MainTexture] _BaseMap("Solar Surface", 2D) = "white" {}
        [MainColor][HDR] _BaseColor("Surface Emission", Color) = (1.7, 0.86, 0.2, 1)
        [HDR] _HotColor("Hot Detail Emission", Color) = (0.62, 0.2, 0.025, 1)
        _FlowStrength("Periodic Flow Strength", Range(0, 0.02)) = 0.006
        _SecondaryBlend("Secondary Pattern Blend", Range(0, 0.4)) = 0.16
        [HideInInspector] _SimulationPhase("Simulation Phase", Range(0, 1)) = 0
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
            Name "SolarSurface"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex SolarVertex
            #pragma fragment SolarFragment
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _HotColor;
                half _FlowStrength;
                half _SecondaryBlend;
                half _SimulationPhase;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half fogFactor : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings SolarVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half4 SolarFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                const half TwoPi = 6.28318530718h;
                half phaseAngle = _SimulationPhase * TwoPi;
                half2 primaryOffset =
                    half2(sin(phaseAngle), cos(phaseAngle)) * _FlowStrength;
                half2 secondaryOffset =
                    half2(cos(phaseAngle * 2.0h), sin(phaseAngle * 2.0h)) *
                    _FlowStrength;

                half3 primary = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv + primaryOffset).rgb;
                half2 mirroredUv = half2(1.0h - input.uv.x, input.uv.y);
                half3 secondary = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    mirroredUv + secondaryOffset).rgb;
                half3 pattern = lerp(primary, secondary, _SecondaryBlend);
                half luminance = dot(pattern, half3(0.2126h, 0.7152h, 0.0722h));
                half hotDetail = smoothstep(0.42h, 0.88h, luminance);

                half3 color =
                    pattern * _BaseColor.rgb +
                    hotDetail * _HotColor.rgb;
                color = MixFog(color, input.fogFactor);
                return half4(color, 1.0h);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Unlit/DepthOnly"
    }

    FallBack Off
}
