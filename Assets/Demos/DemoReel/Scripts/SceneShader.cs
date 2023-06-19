namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class SceneShader : SceneDemo
    {
        private Vector2 mBouncePos;
        private Vector2 mSpeed = new Vector2(2.0f, 2.0f);
        private Vector2 mVelocity;

        private FastString mCodeStr = new FastString(8192);
        private FastString mShaderStr = new FastString(8192);

        private TMXMapAsset mMap = new TMXMapAsset();

        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet2 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet3 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet4 = new SpriteSheetAsset();

        private ShaderAsset shader1 = new ShaderAsset();

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            spriteSheet1.Load("DemoReel/Sprites");
            spriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            spriteSheet2.Load("DemoReel/Ghost");
            spriteSheet2.grid = new SpriteGrid(new Vector2i(104, 106));

            RB.SpriteSheetSet(spriteSheet1);

            mMap.Load("DemoReel/Tilemap");

            if (mMap != null)
            {
                mMap.LoadLayer("Decoration", 0);
                mMap.LoadLayer("Terrain", 1);

                RB.MapLayerSpriteSheetSet(0, spriteSheet1);
                RB.MapLayerSpriteSheetSet(1, spriteSheet1);
            }

            var demo = (DemoReel)RB.Game;

            shader1.Load("DemoReel/WavyMaskShader");

            spriteSheet3.Create(RB.DisplaySize);
            spriteSheet4.Create(RB.DisplaySize);

            mBouncePos = new Vector2(RB.DisplaySize.width * 0.5f, RB.DisplaySize.height * 0.55f);
            mVelocity = mSpeed;
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();
            spriteSheet1.Unload();
            spriteSheet2.Unload();
            spriteSheet3.Unload();
            spriteSheet4.Unload();
            mMap.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mBouncePos += mVelocity;

            if (mBouncePos.x < 32)
            {
                mBouncePos.x = 32;
                mVelocity.x = mSpeed.x;
            }

            if (mBouncePos.x > RB.DisplaySize.width - 128 - 32)
            {
                mBouncePos.x = RB.DisplaySize.width - 128 - 32;
                mVelocity.x = -mSpeed.x;
            }

            if (mBouncePos.y < RB.DisplaySize.height / 2)
            {
                mBouncePos.y = RB.DisplaySize.height / 2;
                mVelocity.y = mSpeed.y;
            }

            if (mBouncePos.x > RB.DisplaySize.width - spriteSheet2.grid.cellSize.x - 4)
            {
                mBouncePos.x = RB.DisplaySize.width - spriteSheet2.grid.cellSize.x - 4;
                mVelocity.x = -mSpeed.x;
            }

            if (mBouncePos.y > RB.DisplaySize.height - spriteSheet2.grid.region.height - 4)
            {
                mBouncePos.y = RB.DisplaySize.height - spriteSheet2.grid.region.height - 4;
                mVelocity.y = -mSpeed.y;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawTMX(4, 4);
        }

        private void DrawTMX(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            if (mMap != null)
            {
                RB.Offscreen(spriteSheet3);

                RB.DrawRectFill(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), DemoUtil.IndexToRGB(22));

                RB.DrawMapLayer(0);
                RB.DrawMapLayer(1);

                RB.Offscreen(spriteSheet4);
                RB.Clear(new Color32(0, 0, 0, 0));
                RB.SpriteSheetSet(spriteSheet2);
                RB.DrawSprite(0, new Vector2i((int)mBouncePos.x, (int)mBouncePos.y), mVelocity.x > 0 ? RB.FLIP_H : 0);

                RB.Onscreen();

                RB.ShaderSet(shader1);
                shader1.SpriteSheetTextureSet("Mask", spriteSheet4);
                shader1.FloatSet("Wave", RB.Ticks / 10.0f);
                shader1.SpriteSheetFilterSet(spriteSheet4, RB.Filter.Linear);

                RB.SpriteSheetSet(spriteSheet3);
                RB.DrawCopy(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), Vector2i.zero);

                RB.ShaderReset();

                RB.SpriteSheetSet(spriteSheet1);
            }
            else
            {
                RB.Print(new Vector2i(x, y + 250), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/Tilemap.tmx in Unity");
            }

            string shaderName = "WavyMaskShader";

            mFormatStr.Set("@C// Custom shaders can be used for many things, like masking!\n");
            mFormatStr.Append("@NmyShader.Load(@S\"DemoReel/").Append(shaderName).Append("\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Draw a tilemap to one offscreen surface\n");
            mFormatStr.Append("@MRB@N.Offscreen(myOffscreenSurface1);\n");
            mFormatStr.Append("@MRB@N.DrawRectFill(@Knew @MRect2i@N(@L0@N, @L0@N,\n");
            mFormatStr.Append("   @MRB@N.DisplaySize.width, @MRB@N.DisplaySize.height),\n");
            mFormatStr.Append("   @I22@N);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Draw a mask to the other offscreen surface\n");
            mFormatStr.Append("@MRB@N.Offscreen(myOffscreenSurface2);\n");
            mFormatStr.Append("@MRB@N.Clear(@Knew @MColor32@N(@L0@N, @L0@N, @L0@N, @L0@N));\n");
            mFormatStr.Append("@MRB@N.SpriteSheetSet(@NmyGhostMaskSpriteSheet);\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L0@N, @Knew @MVector2i@N(@L").Append((int)mBouncePos.x).Append("@N, @L").Append((int)mBouncePos.y).Append("@N)").Append(mVelocity.x > 0 ? ", RB.FLIP_H" : string.Empty).Append(");\n");

            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Use a custom shader to combine the two!\n");
            mFormatStr.Append("@MRB@N.Onscreen();\n");
            mFormatStr.Append("@MRB@N.ShaderSet(@NmyShader);\n");
            mFormatStr.Append("@NmyShader.SpriteSheetTextureSet(@S\"Mask\"@N, myOffscreenSurface2);\n");
            mFormatStr.Append("@NmyShader.FloatSet(@S\"Wave\"@N, @L").Append(RB.Ticks / 10.0f, 2).Append("f@N);\n");
            mFormatStr.Append("@NmyShader.SpriteSheetFilterSet(myOffscreenSurface2, @MRB@N.@MFilter@N.Linear);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.SpriteSheetSet(@NmyOffscreenSurface1);\n");
            mFormatStr.Append("@MRB@N.DrawCopy(@Knew @MRect2i@N(@L0@N, @L0@N,\n   @MRB@N.DisplaySize.width, @MRB@N.DisplaySize.height),\n   @MVector2i@N.zero);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.ShaderReset();\n");
            DemoUtil.HighlightCode(mFormatStr, mCodeStr);

            mFormatStr.Set("@C// This shader multiplies in a mask and applies a wavy effect!\n");
            mFormatStr.Append("@KShader@N \"Unlit/").Append(shaderName).Append("\" {\n");
            mFormatStr.Append("  @KSubShader@N {\n");
            mFormatStr.Append("    @C...\n");
            mFormatStr.Append("    @KPass@N {\n");
            mFormatStr.Append("      @C...\n");
            mFormatStr.Append("      @C/*** Insert custom shader variables here ***/\n");
            mFormatStr.Append("      @Ksampler2D@N Mask;\n");
            mFormatStr.Append("      @Kfloat@N Wave;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("      @Nfrag_in vert(vert_in v, @Kout float4@N screen_pos : @MSV_POSITION@N) {\n");
            mFormatStr.Append("        @C...@N\n");
            mFormatStr.Append("      }\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("      @Kfloat4@N frag(frag_in i, @MUNITY_VPOS_TYPE@N screen_pos : @MVPOS@N) : @MSV_Target@N {\n");
            mFormatStr.Append("        @C...\n");
            mFormatStr.Append("        @C/*** Insert custom fragment shader code here ***/@N\n");
            mFormatStr.Append("        @C// Sample the mask texture@N\n");
            mFormatStr.Append("        i.uv.x += sin(Wave + i.uv.y * @L8@N) * @L0.025@N;\n");
            mFormatStr.Append("        i.uv.y += cos(Wave - i.uv.x * @L8@N) * @L0.015@N;\n");
            mFormatStr.Append("        @Kfloat4@N mask_color = @Mtex2D@N(Mask, i.uv).rgba;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("        @C// Multiply the sprite pixel by mask color@N\n");
            mFormatStr.Append("        @Kreturn@N sprite_pixel_color * mask_color;\n");
            mFormatStr.Append("      }\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("  }\n");
            mFormatStr.Append("}\n");
            DemoUtil.HighlightCode(mFormatStr, mShaderStr);

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), mCodeStr);
            RB.Print(new Vector2i(x + 300, y), DemoUtil.IndexToRGB(5), mShaderStr);
        }
    }
}
