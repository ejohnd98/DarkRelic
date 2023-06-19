/* Custom shaders should be based on PresentBasicShader.shader, not this shader */
Shader "RetroBlit/PresentShader"
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

            #pragma multi_compile_local __ CURVATURE
            #pragma multi_compile_local __ PIXELATE
            #pragma multi_compile_local __ CHROMA
            #pragma multi_compile_local __ SMOOTHING
            #pragma multi_compile_local __ SATURATE
            #pragma multi_compile_local __ SCANLINE
            #pragma multi_compile_local __ NOISE_OR_FIZZLE

            // Lowest target for greatest cross-platform compatiblilty
            #pragma target 3.0

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

#if SMOOTHING
            float _SampleFactor;
#endif

#if SCANLINE
            float _ScanlineIntensity;
            float _ScanlineOffset;
            float _ScanlineLength;
#endif

#if NOISE_OR_FIZZLE
            float _FizzleIntensity;
            float3 _FizzleColor;
            float _NoiseIntensity;
            float2 _NoiseSeed;
#endif

#if SATURATE
            float _SaturationIntensity;
#endif

#if CURVATURE
            float _CurvatureIntensity;
#endif

#if PIXELATE
            float _PixelateIntensity;
            float _PixelateIntensityInverse;
#endif

#if CHROMA
            float2 _ChromaticAberration;
#endif

            float3 _ColorFade;
            float _ColorFadeIntensity;

            float3 _ColorTint;
            float _ColorTintIntensity;

            float _NegativeIntensity;

            /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xy, 0, 1));
                o.uv = v.uv;

                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

#if PIXELATE
                // Pixelate
                float scaleUp = 100;
                float scaleUpInverse = 0.01;
                i.uv.x = (floor(((i.uv.x * _PixelTextureSize.x * scaleUp) + (_PixelateIntensity * 1.33333)) * _PixelateIntensityInverse) * _PixelateIntensity) * (_PixelTextureSizeInverse.x * scaleUpInverse);
                i.uv.y = (floor(((i.uv.y * _PixelTextureSize.y * scaleUp) + (_PixelateIntensity * 1.33333)) * _PixelateIntensityInverse) * _PixelateIntensity) * (_PixelTextureSizeInverse.y * scaleUpInverse);
#endif

#if CURVATURE
                // Curvature
                float2 centerUV = i.uv - 0.5;
                float curv = dot(centerUV.xy, centerUV.xy);
                float curvOffset = (curv * curv) * _CurvatureIntensity;
                i.uv = i.uv + centerUV * curvOffset;
#endif

#if SMOOTHING
                /* Here we sample neighbouring pixels to get some pixel smoothing when the RetroBlit.DisplaySize
                   doesn't divide evenly into the native window resolution. */
                float2 pixelSize = float2(_PixelTextureSizeInverse.x, _PixelTextureSizeInverse.y);
                pixelSize *= _SampleFactor;

#if CHROMA
                float4 leftColor;
                leftColor.r = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y) + _ChromaticAberration).r;
                leftColor.ga = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y)).ga;
                leftColor.b = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y) - _ChromaticAberration).b;

                float4 rightColor;
                rightColor.r = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y) + _ChromaticAberration).r;
                rightColor.ga = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y)).ga;
                rightColor.b = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y) - _ChromaticAberration).b;

                float4 topColor;
                topColor.r = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y) + _ChromaticAberration).r;
                topColor.ga = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y)).ga;
                topColor.b = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y) - _ChromaticAberration).b;

                float4 bottomColor;
                bottomColor.r = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y) + _ChromaticAberration).r;
                bottomColor.ga = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y)).ga;
                bottomColor.b = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y) - _ChromaticAberration).b;
#else
                float4 leftColor = tex2D(_PixelTexture, float2(i.uv.x - pixelSize.x, i.uv.y));
                float4 rightColor = tex2D(_PixelTexture, float2(i.uv.x + pixelSize.x, i.uv.y));
                float4 topColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y + pixelSize.y));
                float4 bottomColor = tex2D(_PixelTexture, float2(i.uv.x, i.uv.y - pixelSize.y));
#endif
                float4 color = (leftColor + rightColor + topColor + bottomColor) * 0.25;
#else
#if CHROMA
                float4 color;
                color.r = tex2D(_PixelTexture, i.uv + _ChromaticAberration).r;
                color.ga = tex2D(_PixelTexture, i.uv).ga;
                color.b = tex2D(_PixelTexture, i.uv - _ChromaticAberration).b;
#else
                float4 color = tex2D(_PixelTexture, i.uv).rgba;
#endif
#endif

#if SATURATE
                // Saturate
                float4 scaledColor = color * float4(0.3, 0.59, 0.11, 1);
                float luminance = scaledColor.r + scaledColor.g + scaledColor.b;
                float desatColor = float4(luminance, luminance, luminance, 1);
                color = lerp(color, desatColor, -_SaturationIntensity);
#endif

#if SCANLINE
                // Scanline
                float pixelLuminance = (color.r * 0.6) + (color.g * 0.3) + (color.b * 0.1) * 0.75;
                float scanWave = (sin((i.uv.y * _PixelTextureSize.y * 2.0) * 3.14159265) + 1.0) * 0.5;
                scanWave = (scanWave * scanWave);
                float scanFade = 1.0 - ((scanWave) * _ScanlineIntensity * (1.0 - pixelLuminance));
                color *= scanFade;
#endif

#if NOISE_OR_FIZZLE
                // Noise
                float noiseSample = frac(sin(dot(float2(floor((i.uv.x + _NoiseSeed.x) * _PixelTextureSize.x), floor((i.uv.y + _NoiseSeed.y) * _PixelTextureSize.y)), float2(1.9259, 7.247))) * 4385.21);
                float noiseFade = 1.0 - (noiseSample * _NoiseIntensity);
                color *= (noiseFade + (_NoiseIntensity * 0.5));

                float fizzleFactor = step(_FizzleIntensity, noiseSample);
                color = (fizzleFactor * color) + ((1 - fizzleFactor) * float4(_FizzleColor, 1));
#endif

                // Color Fade
                color = ((1.0 - _ColorFadeIntensity) * color) + (_ColorFadeIntensity * float4(_ColorFade, 1));

                // Color Tint
                color *= ((1.0 - _ColorTintIntensity) * float4(1, 1, 1, 1)) + (_ColorTintIntensity * float4(_ColorTint, 1));

                // Negative
                color = ((1.0 - _NegativeIntensity) * color) + (_NegativeIntensity * float4(1.0 - color.r, 1.0 - color.g, 1.0 - color.b, 1));

                /* Custom shaders should be based on PresentBasicShader.shader, not this shader */

                return color;
            }
            ENDCG
        }
    }
}
