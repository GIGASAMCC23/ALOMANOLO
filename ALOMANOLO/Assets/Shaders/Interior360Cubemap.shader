Shader "Custom/Interior360Cubemap"
{
    Properties
    {
        _Cube ("360 Cubemap", Cube) = "" {}
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        // Renderizamos la cara interior de la esfera.
        Cull Front

        Pass
        {
            Name "Interior360"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            float4 _Tint;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput =
                    GetVertexPositionInputs(input.positionOS.xyz);

                output.positionHCS = vertexInput.positionCS;

                // Dirección desde el centro hacia el vértice.
                output.direction = TransformObjectToWorldDir(
                    input.positionOS.xyz
                );

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.direction);

                half4 color = SAMPLE_TEXTURECUBE(
                    _Cube,
                    sampler_Cube,
                    direction
                );

                return color * _Tint;
            }

            ENDHLSL
        }
    }
}
