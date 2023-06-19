Shader "Unlit/DrawVignette"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Ztest never
        Zwrite off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 spritesheet_uv1 : TEXCOORD1;
                float2 spritesheet_uv2 : TEXCOORD2;
                fixed4 color : COLOR;
            };

            struct frag_in
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                float2 spritesheet_uv1 : TEXCOORD1;
                float2 spritesheet_uv2 : TEXCOORD2;
                float2 screen_pos : TEXCOORD3;
                fixed4 color : COLOR;
            };

            sampler2D _SpritesTexture;

            float2 _DisplaySize;
            float4 _Clip;
            float4 _GlobalTint;

            /*** Insert custom shader properties here ***/

            frag_in vert(vert_in i)
            {
                frag_in o;
                o.uv = float3(i.uv, i.vertex.z);
                o.spritesheet_uv1 = i.spritesheet_uv1;
                o.spritesheet_uv2 = i.spritesheet_uv2;
                o.color = i.color;

                // Get onscreen position of the vertex
                o.vertex = UnityObjectToClipPos(float4(i.vertex.xy, 0, 1));
                o.screen_pos = ComputeScreenPos(o.vertex) * float4(_DisplaySize.xy, 1, 1);

                /*** Insert custom vertex shader code here ***/

                return o;
            }

            // Performs test against the clipping region
            float clip_test(float2 p, float2 bottom_left, float2 top_right)
            {
                float2 s = step(bottom_left, p) - step(top_right, p);
                return s.x * s.y;
            }

            // Map UV from sprite to spritesheet
            float2 map_uv(float2 uv, float2 spritesheet_uv1, float2 spritesheet_uv2) {
                float2 out_uv;

                out_uv.x = ((spritesheet_uv2.x - spritesheet_uv1.x) * abs(uv.x % 1.0)) + spritesheet_uv1.x;
                out_uv.y = ((spritesheet_uv2.y - spritesheet_uv1.y) * abs(uv.y % 1.0)) + spritesheet_uv1.y;

                return out_uv;
            }

            float4 frag(frag_in i) : SV_Target
            {
                // 0 if we're drawing from a sprite sheet texture, 1 if not
                float solid_color_flag = 1 - i.uv.z;

                float4 sprite_pixel_color = (tex2D(_SpritesTexture, map_uv(i.uv, i.spritesheet_uv1, i.spritesheet_uv2)) * (1 - solid_color_flag)) + (float4)solid_color_flag;

                // Perform clip test on the pixel
                sprite_pixel_color.a *= clip_test(i.screen_pos.xy, _Clip.xy, _Clip.zw);

                // Multiply in vertex alpha and current global alpha setting
                sprite_pixel_color *= i.color;
                sprite_pixel_color *= _GlobalTint;

                /*** Insert custom fragment shader code here ***/

				float deltaX = (i.screen_pos.x / _DisplaySize.x) - 0.5;
				float deltaY = (i.screen_pos.y / _DisplaySize.y) - 0.5;
				
				float vignette = (1.0 - sqrt(deltaX * deltaX + deltaY * deltaY)) + 0.25;
				vignette *= vignette;

				vignette = clamp(vignette, 0.0, 1.0);
				
				sprite_pixel_color.rgb *= vignette;

                return sprite_pixel_color;
            }
            ENDCG
        }
    }
}
