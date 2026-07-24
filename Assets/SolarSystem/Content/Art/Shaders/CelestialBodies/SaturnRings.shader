Shader "SolarSystem/Celestial/Saturn Rings"
{
    Properties
    {
        [MainTexture] _BaseMap("Ring Map", 2D) = "white" {}
        [MainColor] _BaseColor("Tint", Color) = (1, 1, 1, 1)
        _Opacity("Opacity", Range(0, 1)) = 0.9
        _AmbientBrightness("Ambient Brightness", Range(0, 1)) = 0.12
        _DayBrightness("Day Brightness", Range(0, 2)) = 1.05
        _ScatteringStrength("Grazing Visibility", Range(0, 1)) = 0.14
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "SaturnRings"
            Tags { "LightMode" = "UniversalForward" }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _Opacity;
                half _AmbientBrightness;
                half _DayBrightness;
                half _ScatteringStrength;
            CBUFFER_END

            float3 _SolarSystemSunPositionWS;

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
                half fogFactor : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(input.normalOS);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half4 source = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half alpha = saturate(source.a * _BaseColor.a * _Opacity);
                clip(alpha - 0.001h);

                half3 normalWS = SafeNormalize(input.normalWS);
                half3 sunDirection =
                    SafeNormalize(_SolarSystemSunPositionWS - input.positionWS);
                half3 viewDirection =
                    SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
                half sunAlignment = abs(dot(normalWS, sunDirection));
                half grazing = 1.0h - abs(dot(normalWS, viewDirection));
                half brightness = lerp(
                    _AmbientBrightness,
                    _DayBrightness,
                    sunAlignment);
                brightness *= 1.0h + grazing * _ScatteringStrength;

                half3 color = source.rgb * _BaseColor.rgb * brightness;
                color = MixFog(color, input.fogFactor);
                return half4(color * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
