Shader "Custom/URP/PaletteGridUnlit_Flat"
{
    Properties
    {
        _Palette("Palette Texture (NxN grid)", 2D) = "white" {}
        _GridSize("Grid Size (N)", Float) = 4
        _UseVertexColor("Use Vertex Color (1=Yes,0=Use _PaletteIndex)", Float) = 1
        _PaletteIndex("Palette Index (if not using VertexColor)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Palette);
            SAMPLER(sampler_Palette);

            CBUFFER_START(UnityPerMaterial)
                float _GridSize;
                float _UseVertexColor;
                float _PaletteIndex;
            CBUFFER_END

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _InstancedPaletteIndex)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                nointerpolation float3 normalWS : NORMAL;
                nointerpolation float paletteIdxRaw : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                float idxFromVertex = IN.color.r * max(1.0, _GridSize*_GridSize-1.0);
                float idxFromProperty = _PaletteIndex;

                #if defined(UNITY_INSTANCING_ENABLED)
                    float instIdx = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancedPaletteIndex);
                #else
                    float instIdx = idxFromProperty;
                #endif

                OUT.paletteIdxRaw = lerp(instIdx, idxFromVertex, (_UseVertexColor>0.5)?1.0:0.0);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float g = max(1.0,_GridSize);
                float maxIndex = g*g-1.0;
                float idx = clamp(IN.paletteIdxRaw,0,maxIndex);
                float row = floor(idx/g);
                float col = idx - row*g;
                float2 uv = float2( (col+0.5)/g, (row+0.5)/g );
                half4 colSample = SAMPLE_TEXTURE2D(_Palette, sampler_Palette, uv);

                float3 lightDir = normalize(float3(0.3,0.7,0.5));
                float NdotL = saturate(dot(IN.normalWS, lightDir));
                colSample.rgb *= 0.5 + 0.5 * NdotL;

                return colSample;
            }

            ENDHLSL
        }
    }
}
