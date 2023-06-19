namespace RetroBlitDemoReel
{
    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffectShader : SceneDemo
    {
        private readonly FastString mCodeStr = new FastString(4096);
        private readonly FastString mShaderStr = new FastString(4096);
        private readonly TMXMapAsset mMap = new TMXMapAsset();
        private readonly SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();
        private readonly ShaderAsset mShader1 = new ShaderAsset();

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            mSpriteSheet1.Load("DemoReel/Sprites");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));
            RB.SpriteSheetSet(mSpriteSheet1);

            RB.EffectReset();

            mMap.Load("DemoReel/Tilemap");

            if (mMap != null)
            {
                mMap.LoadLayer("Decoration", 0);
                mMap.LoadLayer("Terrain", 1);

                RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);
                RB.MapLayerSpriteSheetSet(1, mSpriteSheet1);
            }

            mShader1.Load("DemoReel/PresentRippleShader");
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.EffectReset();
            RB.MapClear();

            mSpriteSheet1.Unload();
            mMap.Unload();
            mShader1.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            if (mMap != null)
            {
                RB.Clear(DemoUtil.IndexToRGB(22));
                RB.DrawMapLayer(0);
                RB.DrawMapLayer(1);
            }
            else
            {
                RB.Print(new Vector2i(2, 210), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/Tilemap.tmx in Unity");
            }

            RB.EffectShader(mShader1);
            mShader1.FloatSet("Wave", RB.Ticks / 25.0f);

            RB.DrawRectFill(new Rect2i(0, 0, RB.DisplaySize.width, 200), DemoUtil.IndexToRGB(1));

            string shaderName = "PresentRippleShader";

            mFormatStr.Set("@C// Custom post-processing shader\n");
            mFormatStr.Append("@NmyShader.Load(@S\"DemoReel/").Append(shaderName).Append("\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.EffectShader(myShader);\n");
            mFormatStr.Append("@MRB@N.ShaderFloatSet(myShader, @S\"Wave\"@N, @L").Append(RB.Ticks / 25.0f, 2).Append("f@N);\n");
            mFormatStr.Append("@MRB@N.EffectFilter(@MRB@N.@MFilter@N.Linear);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.EffectApplyNow();\n");
            mFormatStr.Append("@MRB@N.EffectReset();\n");
            DemoUtil.HighlightCode(mFormatStr, mCodeStr);

            mFormatStr.Set("@C// This creates a wavy effect!\n");
            mFormatStr.Append("@KShader@N \"Unlit/").Append(shaderName).Append("\" {\n");
            mFormatStr.Append("  @KSubShader@N {\n");
            mFormatStr.Append("    @C...\n");
            mFormatStr.Append("    @KPass@N {\n");
            mFormatStr.Append("      @C...\n");
            mFormatStr.Append("      @C/*** Insert custom shader variables here ***/\n");
            mFormatStr.Append("      @Kfloat@N Wave;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("      @Nfrag_in vert(appdata v) {\n");
            mFormatStr.Append("        @C...@N\n");
            mFormatStr.Append("      }\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("      @Kfloat4@N frag(v2f i) : @MSV_Target@N {\n");
            mFormatStr.Append("        @C/*** Insert custom fragment shader code here ***/@N\n");
            mFormatStr.Append("        @Kfloat2@N centerOffset = @L-1.0@N + @L2.0@N * i.uv.xy;\n");
            mFormatStr.Append("        @Kfloat@N len = @Klength@N(centerOffset);\n");
            mFormatStr.Append("        i.uv.xy += (centerOffset / len) * cos(len * @L10.0@N - Wave) * @L0.005@N;\n");
            mFormatStr.Append("        @C...@N\n");
            mFormatStr.Append("        @Kreturn@N color;\n");
            mFormatStr.Append("      }\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("  }\n");
            mFormatStr.Append("}\n");
            DemoUtil.HighlightCode(mFormatStr, mShaderStr);

            RB.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(0), mCodeStr);
            RB.Print(new Vector2i(304, 4), DemoUtil.IndexToRGB(0), mShaderStr);
        }
    }
}
