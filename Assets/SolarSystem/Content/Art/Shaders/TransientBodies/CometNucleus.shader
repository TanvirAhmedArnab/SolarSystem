Shader "SolarSystem/Transient/Comet Nucleus"
{
    Properties
    {
        [HDR] _CoreColor("Core Color", Color) = (3.4, 2.1, 0.65, 1)
        [HDR] _FlameColor("Flame Color", Color) = (1.8, 0.28, 0.035, 1)
        [HDR] _RimColor("Rim Color", Color) = (1.2, 0.42, 0.08, 1)
        _Intensity("Intensity", Range(0, 6)) = 2.2
        _NoiseScale("Noise Scale", Range(1, 16)) = 6
        _FlickerSpeed("Flicker Speed", Range(0, 8)) = 1.7
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "CometNucleus"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                half3 normalWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _CoreColor;
                half4 _FlameColor;
                half4 _RimColor;
                half _Intensity;
                half _NoiseScale;
                half _FlickerSpeed;
            CBUFFER_END

            float Hash31(float3 value)
            {
                value = frac(value * 0.1031);
                value += dot(value, value.yzx + 33.33);
                return frac((value.x + value.y) * value.z);
            }

            float Noise3D(float3 value)
            {
                float3 cell = floor(value);
                float3 local = frac(value);
                local = local * local * (3.0 - (2.0 * local));

                float low = lerp(
                    lerp(Hash31(cell), Hash31(cell + float3(1, 0, 0)), local.x),
                    lerp(
                        Hash31(cell + float3(0, 1, 0)),
                        Hash31(cell + float3(1, 1, 0)),
                        local.x),
                    local.y);
                float high = lerp(
                    lerp(
                        Hash31(cell + float3(0, 0, 1)),
                        Hash31(cell + float3(1, 0, 1)),
                        local.x),
                    lerp(
                        Hash31(cell + float3(0, 1, 1)),
                        Hash31(cell + float3(1, 1, 1)),
                        local.x),
                    local.y);
                return lerp(low, high, local.z);
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positions =
                    GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normals =
                    GetVertexNormalInputs(input.normalOS);
                output.positionHCS = positions.positionCS;
                output.positionOS = input.positionOS.xyz;
                output.positionWS = positions.positionWS;
                output.normalWS = normals.normalWS;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float phase = _Time.y * _FlickerSpeed;
                float3 samplePosition =
                    (input.positionOS * _NoiseScale) +
                    float3(phase * 0.35, phase, -phase * 0.22);
                float broadNoise = Noise3D(samplePosition);
                float fineNoise = Noise3D(
                    (samplePosition * 2.15) + float3(7.1, 3.7, 11.3));
                float flame = saturate(
                    (broadNoise * 0.72) + (fineNoise * 0.28));

                half3 normal = normalize(input.normalWS);
                half3 viewDirection =
                    normalize(GetWorldSpaceViewDir(input.positionWS));
                half fresnel = pow(
                    saturate(1.0h - dot(normal, viewDirection)),
                    2.4h);
                half pulse = 0.92h + (sin(phase * 2.3h) * 0.08h);

                half3 surface = lerp(
                    _FlameColor.rgb,
                    _CoreColor.rgb,
                    smoothstep(0.28h, 0.82h, flame));
                surface += _RimColor.rgb * fresnel * 0.9h;
                return half4(surface * _Intensity * pulse, 1.0h);
            }
            ENDHLSL
        }
    }
}
