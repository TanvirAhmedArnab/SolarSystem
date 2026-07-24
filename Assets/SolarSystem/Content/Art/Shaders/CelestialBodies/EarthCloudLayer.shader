Shader "SolarSystem/Celestial/Earth Cloud Layer"
{
    Properties
    {
        [MainTexture] _CloudMap("Cloud Coverage", 2D) = "black" {}
        [MainColor] _CloudColor("Cloud Tint", Color) = (0.95, 0.98, 1, 1)
        _CoverageThreshold("Coverage Threshold", Range(0, 1)) = 0.16
        _CoverageContrast("Coverage Contrast", Range(0.1, 8)) = 2.4
        _Opacity("Opacity", Range(0, 1)) = 0.62
        _AmbientBrightness("Nightside Brightness", Range(0, 1)) = 0.08
        _SunBrightness("Sunlight Brightness", Range(0, 2)) = 1
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
            Name "CloudForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex CloudVertex
            #pragma fragment CloudFragment
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CloudMap);
            SAMPLER(sampler_CloudMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _CloudMap_ST;
                half4 _CloudColor;
                half _CoverageThreshold;
                half _CoverageContrast;
                half _Opacity;
                half _AmbientBrightness;
                half _SunBrightness;
            CBUFFER_END

            float4 _SolarSystemSunPositionWS;

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

            Varyings CloudVertex(Attributes input)
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
                output.uv = TRANSFORM_TEX(input.uv, _CloudMap);
                output.fogFactor = ComputeFogFactor(positions.positionCS.z);
                return output;
            }

            half4 CloudFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half coverage =
                    SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, input.uv).r;
                half alpha = saturate(
                    (coverage - _CoverageThreshold) * _CoverageContrast) *
                    _Opacity;
                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half sunlight = saturate(dot(normalWS, sunDirection));
                half brightness =
                    _AmbientBrightness + sunlight * _SunBrightness;
                half3 color = _CloudColor.rgb * brightness;
                color = MixFog(color, input.fogFactor);
                return half4(color * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
