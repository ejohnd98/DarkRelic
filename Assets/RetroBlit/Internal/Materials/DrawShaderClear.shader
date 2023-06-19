Shader "RetroBlit/DrawShaderClear"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Ztest never
        Zwrite off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Lowest target for greatest cross-platform compatiblilty
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct vert_in
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 flags : TEXCOORD1;
                fixed4 color : COLOR;
            };

            struct frag_in
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            frag_in vert(vert_in i)
            {
                frag_in o;
                o.color = i.color;

                // Get onscreen position of the vertex
                o.vertex = UnityObjectToClipPos(float4(i.vertex.xy, 0, 1));

                return o;
            }

            float4 frag(frag_in i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
