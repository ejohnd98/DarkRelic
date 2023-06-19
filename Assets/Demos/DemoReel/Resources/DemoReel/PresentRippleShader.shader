Shader "Unlit/PresentRippleShader"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Lowest target for greatest cross-platform compatiblilty
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _PixelTexture;
            float2 _PixelTextureSize;
            float2 _PixelTextureSizeInverse;
            float2 _PixelTextureSizeRatio; // x = w/h, y = h/w

            float2 _PresentTextureSize;

            float _SampleFactor;

            /*** Insert custom shader variables here ***/

            float Wave;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xy, 0, 1));
                o.uv = v.uv;

                /*** Insert custom vertex shader here ***/

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                /*** Insert custom fragment shader here ***/
                float2 centerOffset = -1.0 + 2.0 * i.uv.xy;
                float len = length(centerOffset);
                i.uv.xy += (centerOffset / len) * cos(len * 10.0 - Wave) * 0.01;

                /* Here we sample neighbouring pixels to get some pixel smoothing when the RetroBlit.DisplaySize
                   doesn't divide evenly into the native window resolution. If you're using RetroBlit.Filter.Linear, or
                   can ensure that RetroBlit.DisplaySize divides nicely into window resolution then you can replace this
                   code with a simple and more efficient:

                      float4 color = tex2D(_PixelTexture, i.uv).rgba;
                */
                float2 pixelSize = float2(_PixelTextureSizeInverse.x, _PixelTextureSizeInverse.y);
                pixelSize *= _SampleFactor;

                float4 leftColor = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y)).rgba;
                float4 rightColor = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y)).rgba;
                float4 topColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y)).rgba;
                float4 bottomColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y)).rgba;

                float4 color = (leftColor + rightColor + topColor + bottomColor) * 0.25;

                return color;
            }
            ENDCG
        }
    }
}
