Shader "SolarSystem/Transient/Comet Trail"
{
    Properties
    {
        [HDR] _HeadColor("Head Color", Color) = (2.0, 1.1, 0.28, 1)
        [HDR] _TailColor("Tail Color", Color) = (0.55, 0.025, 0.005, 1)
        _Intensity("Intensity", Range(0, 8)) = 1.15
        _FlowSpeed("Flow Speed", Range(0, 8)) = 2.4
        _NoiseScale("Noise Scale", Range(1, 32)) = 11
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "CometTrail"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _HeadColor;
                half4 _TailColor;
                half _Intensity;
                half _FlowSpeed;
                half _NoiseScale;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half progress = saturate(input.uv.x);
                half flow = 0.52h + (
                    sin(
                        (progress * _NoiseScale - _Time.y * _FlowSpeed) *
                        6.2831853h) *
                    0.34h);
                half crossSection =
                    saturate(1.0h - abs((input.uv.y * 2.0h) - 1.0h));
                half softEdge = smoothstep(0.0h, 0.72h, crossSection);
                half3 authoredColor =
                    lerp(_HeadColor.rgb, _TailColor.rgb, progress);
                half3 color =
                    input.color.rgb * authoredColor * _Intensity * flow;
                half alpha =
                    input.color.a *
                    lerp(0.72h, 0.34h, progress) *
                    softEdge;
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
