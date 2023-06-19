namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemap properties
    /// </summary>
    public class SceneTMXProps : SceneDemo
    {
        private TMXMapAsset mMap = new TMXMapAsset();
        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();

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

            mSpriteSheet1.Load("DemoReel/Sprites");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));
            RB.SpriteSheetSet(mSpriteSheet1);

            mMap.Load("DemoReel/TilemapProps");

            RB.MapLayerSpriteSheetSet(1, mSpriteSheet1);
            RB.MapLayerSpriteSheetSet(2, mSpriteSheet1);
            RB.MapLayerSpriteSheetSet(3, mSpriteSheet1);

            if (mMap != null)
            {
                mMap.LoadLayer("Clouds", 1);
                mMap.LoadLayer("Decoration", 2);
                mMap.LoadLayer("Terrain", 3);
            }
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();
            mSpriteSheet1.Unload();
            mMap.Unload();

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
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawTMX(4, 4);
        }

        private void DrawTMX(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            Rect2i clipRect = new Rect2i(x, y, 340, 352);

            if (mMap != null)
            {
                RB.ClipSet(clipRect);
                RB.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));

                int scrollPos = -(int)RB.Ticks % (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width);

                RB.CameraSet(new Vector2i(256, 144));
                RB.DrawMapLayer(1, new Vector2i(scrollPos, 0));
                RB.DrawMapLayer(1, new Vector2i(scrollPos + (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width), 0));
                RB.DrawMapLayer(2);
                RB.DrawMapLayer(3);

                var objs = mMap.objectGroups["Objects"].objects;
                for (int i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];

                    switch (obj.shape)
                    {
                        case TMXMapAsset.TMXObject.Shape.Rectangle:
                            RB.DrawRect(obj.rect, Color.red);
                            RB.Print(obj.rect, Color.red, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, obj.properties.GetString("name"));
                            break;

                        case TMXMapAsset.TMXObject.Shape.Polyline:
                            for (int j = 0; j < obj.points.Count - 1; j++)
                            {
                                RB.DrawLine(
                                    obj.points[j] + new Vector2i(obj.rect.x, obj.rect.y),
                                    obj.points[j + 1] + new Vector2i(obj.rect.x, obj.rect.y),
                                    Color.green);
                            }

                            break;

                        case TMXMapAsset.TMXObject.Shape.Ellipse:
                            RB.DrawEllipse(obj.rect.center, new Vector2i(obj.rect.width / 2, obj.rect.height / 2), Color.yellow);
                            RB.Print(obj.rect, Color.yellow, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, obj.properties.GetString("name"));
                            break;
                    }
                }
            }
            else
            {
                RB.CameraReset();
                RB.ClipReset();
                RB.Print(new Vector2i(x + 4, y + 4), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/TilemapProps.tmx in Unity");
            }

            RB.CameraReset();
            RB.DrawRect(clipRect, DemoUtil.IndexToRGB(21));

            RB.ClipReset();

            x += 350;

            mFormatStr.Set("@C// Read objects and their custom properties from TMX file\n");
            mFormatStr.Append("@NmyTMXMap.Load(@S\"DemoReel/TilemapProps\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kvar@N objs = myTMXMap.objectGroups[@S\"My Objects Layer\"@N].objects;\n");
            mFormatStr.Append("@Kfor@N (@Kint@N i = @L0@N; i < objs.Count; i++)\n");
            mFormatStr.Append("{\n");
            mFormatStr.Append("    @Kvar@N obj = obj;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("    @Kswitch@N (obj.shape)\n");
            mFormatStr.Append("    {\n");
            mFormatStr.Append("        @Kcase@N @MTMXObject@N.Shape.Rectangle:\n");
            mFormatStr.Append("            @MRB@N.DrawRect(obj.rect, @MColor@N.red);\n");
            mFormatStr.Append("            @MRB@N.Print(obj.rect, @MColor@N.red,\n");
            mFormatStr.Append("               @MRB@N.ALIGN_H_CENTER | @MRB@N.ALIGN_V_CENTER,\n");
            mFormatStr.Append("               obj.properties.GetString(@S\"name\"@N));\n");
            mFormatStr.Append("            @Kbreak@N;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("        @Kcase@N @MTMXObject@N.Shape.Polyline:\n");
            mFormatStr.Append("            @Kfor@N (@Kint@N j = @L0@N; j < obj.points.Count - @L1@N; j++)\n");
            mFormatStr.Append("            {\n");
            mFormatStr.Append("                @MRB@N.DrawLine(\n");
            mFormatStr.Append("                   obj.points[j] + @Knew@N @MVector2i@N(obj.rect.x, obj.rect.y),\n");
            mFormatStr.Append("                   obj.points[j + @L1@N] + @Knew@N @MVector2i@N(obj.rect.x, obj.rect.y),\n");
            mFormatStr.Append("                   @MColor@N.green);\n");
            mFormatStr.Append("            }\n");
            mFormatStr.Append("            @Kbreak@N;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("        @Kcase@N @MTMXObject@N.Shape.Ellipse:\n");
            mFormatStr.Append("            @MRB@N.DrawEllipse(obj.rect.center,\n");
            mFormatStr.Append("               new @MVector2i@N(obj.rect.width / @L2@N, obj.rect.height / @L2@N),\n");
            mFormatStr.Append("               @MColor@N.yellow);\n");
            mFormatStr.Append("            @MRB@N.Print(obj.rect, @MColor@N.yellow,\n");
            mFormatStr.Append("               @MRB@N.ALIGN_H_CENTER | RB.ALIGN_V_CENTER,\n");
            mFormatStr.Append("               obj.properties.GetString(@S\"name\"@N));\n");
            mFormatStr.Append("            @Kbreak@N;\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("}\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Also easily read tile custom properties\n");
            mFormatStr.Append("@Kvar@N tileProps = @MRB@N.MapDataGet<@MTMXMapAsset.TMXProperties@N>(\n");
            mFormatStr.Append("                      @L0@N, @Knew@N @MVector2i@N(@L2@N, @L5@N));\n");
            mFormatStr.Append("@Kint@N blocking = tileProps.GetInt(@S\"blocking\"@N);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
