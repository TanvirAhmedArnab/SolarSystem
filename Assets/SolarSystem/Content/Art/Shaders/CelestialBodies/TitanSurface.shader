Shader "SolarSystem/Celestial/Titan Surface"
{
    Properties
    {
        [MainTexture] _BaseMap("Anchored Near-IR Surface Mosaic", 2D) = "white" {}
        [MainColor] _BaseColor("Surface Tint", Color) = (0.78, 0.58, 0.34, 1)
        _DetailStrength("Source Detail Strength", Range(0, 1)) = 0.12
        _AmbientBrightness("Nightside Brightness", Range(0, 1)) = 0.035
        _SunBrightness("Sunlight Brightness", Range(0, 2)) = 0.42
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
            Name "TitanSurface"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _DetailStrength;
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
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positions.positionCS.z);
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

                half3 source = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv).rgb;
                half sourceLuminance = Luminance(source);
                half subduedLuminance = lerp(
                    0.5h,
                    sourceLuminance,
                    _DetailStrength);
                half3 subduedSource = lerp(
                    subduedLuminance.xxx,
                    source,
                    _DetailStrength * 0.25h);
                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half3 sunDirection = SafeNormalize(
                    _SolarSystemSunPositionWS.xyz - input.positionWS);
                half sunlight = saturate(dot(normalWS, sunDirection));
                half brightness =
                    _AmbientBrightness + sunlight * _SunBrightness;
                half3 color =
                    subduedSource * _BaseColor.rgb * brightness;
                color = MixFog(color, input.fogFactor);
                return half4(color, 1);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
