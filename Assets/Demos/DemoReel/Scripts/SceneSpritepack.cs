namespace RetroBlitDemoReel
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate sprite packs
    /// </summary>
    public class SceneSpritepack : SceneDemo
    {
        private PackedSprite mSpriteHero1;
        private PackedSprite mSpriteHero2;
        private PackedSprite mSpriteDirtCenter;
        private PackedSprite mSpriteDirtSide;
        private PackedSprite mSpriteGrassTop;
        private PackedSprite mSpriteGrassTopRight;
        private PackedSprite mSpriteWater;

        private NineSlice mNineSlice;

        private int mCounter = 0;

        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet2 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet3 = new SpriteSheetAsset();

        private FontAsset font = new FontAsset();

        private TMXMapAsset mMap = new TMXMapAsset();

        private bool mSpritePackPostLoadDone = false;

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

            mSpritePackPostLoadDone = false;

            spriteSheet1.Load("DemoReel/DemoSpritePack", SpriteSheetAsset.SheetType.SpritePack);
            spriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            spriteSheet2.Load("DemoReel/Sprites");
            spriteSheet2.grid = new SpriteGrid(new Vector2i(16, 16));

            spriteSheet3.Load("DemoReel/DemoSpritePack", SpriteSheetAsset.SheetType.SpritePack);
            spriteSheet3.grid = new SpriteGrid(new Vector2i(8, 8));

            RB.SpriteSheetSet(spriteSheet1);
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            spriteSheet1.Unload();
            spriteSheet2.Unload();
            spriteSheet3.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (spriteSheet1.status == RB.AssetStatus.Ready &&
                spriteSheet2.status == RB.AssetStatus.Ready &&
                spriteSheet3.status == RB.AssetStatus.Ready &&
                !mSpritePackPostLoadDone)
            {
                RB.SpriteSheetSet(spriteSheet1);

                mSpriteHero1 = RB.PackedSpriteGet("Characters/Hero1");
                mSpriteHero2 = RB.PackedSpriteGet("Characters/Hero2");
                mSpriteDirtCenter = RB.PackedSpriteGet("Terrain/DirtCenter");
                mSpriteDirtSide = RB.PackedSpriteGet("Terrain/DirtSide");
                mSpriteGrassTop = RB.PackedSpriteGet("Terrain/GrassTop");
                mSpriteGrassTopRight = RB.PackedSpriteGet("Terrain/GrassTopRight");
                mSpriteWater = RB.PackedSpriteGet("Terrain/Water");

                RB.MapSpriteSet(0, new Vector2i(0, 0), mSpriteGrassTopRight, RB.FLIP_H);
                RB.MapSpriteSet(0, new Vector2i(1, 0), mSpriteGrassTop);
                RB.MapSpriteSet(0, new Vector2i(2, 0), mSpriteGrassTop);
                RB.MapSpriteSet(0, new Vector2i(3, 0), mSpriteGrassTopRight);

                RB.MapSpriteSet(0, new Vector2i(0, 1), mSpriteDirtSide, RB.FLIP_H);
                RB.MapSpriteSet(0, new Vector2i(1, 1), mSpriteDirtCenter);
                RB.MapSpriteSet(0, new Vector2i(2, 1), mSpriteDirtCenter);
                RB.MapSpriteSet(0, new Vector2i(3, 1), mSpriteDirtSide);

                RB.MapLayerSpriteSheetSet(0, spriteSheet1);

                mNineSlice = new NineSlice("Other/NinesliceTopLeft", "Other/NinesliceTop", "Other/NinesliceMiddle");

                var glyphs = new List<string>();
                for (int i = 0; i <= 9; i++)
                {
                    glyphs.Add("Font/" + i);
                }

                glyphs.Add("Font/colon");

                List<char> chars = new List<char>();
                for (char c = '0'; c <= '9'; c++)
                {
                    chars.Add(c);
                }

                chars.Add(':');

                font.Setup(chars, glyphs, spriteSheet1, 1, 1, true);

                var mapping = new string[16];
                for (int i = 0; i < 16; i++)
                {
                    mapping[i] = "Terrain/Tiny" + i;
                }

                mMap.Load("DemoReel/TinyMap");
                mMap.LoadLayer("Terrain", 1, mapping);
                RB.MapLayerSpriteSheetSet(1, spriteSheet3);

                mSpritePackPostLoadDone = true;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            if (spriteSheet1.status == RB.AssetStatus.Failed)
            {
                DrawBadSpritepack(4, 4);
                return;
            }

            if (!mSpritePackPostLoadDone)
            {
                return;
            }

            DrawSpritePacking(4, 4);
            DrawTMX(310, 4);
        }

        private void DrawBadSpritepack(int x, int y)
        {
            RB.Print(new Vector2i(x + 4, y + 4), DemoUtil.IndexToRGB(14), "Failed to load sprite pack!\nPlease try re-importing the sprite pack Demos/DemoReel/DemoSpritePack.sp in Unity");
        }

        private void DrawSpritePacking(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            mFormatStr.Set(
                "@DRetroBlit features a sprite packer that can cram your folders\n" +
                "full of sprites into optimal sprite sheets! To use it simply\n" +
                "create a sprite pack file (.sp) and point it to your sprite folder.");

            RB.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(25), DemoUtil.HighlightCode(mFormatStr, mFinalStr, false));

            y += 16;

            int indent = 16;
            int row = 0;
            int rowSpacing = 9;
            int imageSpacing = 3;
            int fileColor = 5;
            int folderColor = 3;
            int alphaSprite = 7;

            // Draw sprite pack file
            RB.CameraSet(new Vector2i(-170, -16));
            RB.DrawRectFill(new Rect2i(x - 4, y - 4, 108, 112), DemoUtil.IndexToRGB(2));

            mFormatStr.Set("@C// Source folder(s)\n");
            mFormatStr.Append("@C// relative to project\n");
            mFormatStr.Append("@C// root\n");
            mFormatStr.Append("@MSOURCE_FOLDER@N=@LSprites@N\n\n");
            mFormatStr.Append("@C// Output size\n");
            mFormatStr.Append("@MOUTPUT_WIDTH@N=@L96@N\n");
            mFormatStr.Append("@MOUTPUT_HEIGHT@N=@L96@N\n\n");
            mFormatStr.Append("@C// Trim empty space\n");
            mFormatStr.Append("@MTRIM@N=@Ltrue@N");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(fileColor), "DemoSpritePack.sp");
            RB.DrawLine(new Vector2i(x, y + rowSpacing + 2), new Vector2i(x + 100, y + rowSpacing + 2), DemoUtil.IndexToRGB(3));
            RB.Print(new Vector2i(x, y + rowSpacing + rowSpacing), DemoUtil.IndexToRGB(fileColor), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.DrawRectFill(new Rect2i(-32, 60, 6 * 3, 6), DemoUtil.IndexToRGB(5));
            RB.DrawRectFill(new Rect2i(-26, 54, 6, 6 * 3), DemoUtil.IndexToRGB(5));

            // Draw file listing
            RB.CameraSet(new Vector2i(-4, -16));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawRectFill(new Rect2i(x - 4, y - 4, 120, 266), DemoUtil.IndexToRGB(2));

            mFormatStr.Set(
                "@DYour sprite source folders can be anywhere, but it's best to keep\n" +
                "them out of your @NAssets@D folder so that Unity does not put your\n" +
                "source sprites into the game @NAsset Resources@D!\n" +
                "\n" +
                "When you change your source sprites be sure to manually re-import\n" +
                "your sprite pack because Unity can't detect sprite changes outside\n" +
                "of the @NAssets@D folder!");

            RB.Print(new Vector2i(x - 4, y - 4 + 270), DemoUtil.IndexToRGB(25), DemoUtil.HighlightCode(mFormatStr, mFinalStr, false));

            RB.Print(new Vector2i(x, y + row), DemoUtil.IndexToRGB(folderColor), "Sprites/");
            row += rowSpacing;

            RB.Print(new Vector2i(x + indent, y + row), DemoUtil.IndexToRGB(folderColor), "Characters/");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteHero1, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteHero1.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "Hero1.png");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteHero2, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteHero2.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "Hero2.png");
            row += rowSpacing;

            RB.Print(new Vector2i(x + indent, y + row), DemoUtil.IndexToRGB(folderColor), "Terrain/");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteDirtCenter, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteDirtCenter.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "DirtCenter.png");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteDirtSide, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteDirtSide.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "DirtSide.png");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteGrassTop, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteGrassTop.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "GrassTop.png");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawSprite(alphaSprite, new Vector2i(x + (indent * 2), y + row));
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteGrassTopRight, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteGrassTopRight.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "GrassTopRight.png");
            row += rowSpacing;

            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawSprite(mSpriteWater, new Vector2i(x + (indent * 2), y + row));
            row += mSpriteWater.Size.height + imageSpacing;
            RB.Print(new Vector2i(x + (indent * 2), y + row), DemoUtil.IndexToRGB(fileColor), "Water.png");
            row += rowSpacing;

            RB.Print(new Vector2i(x + indent, y + row), DemoUtil.IndexToRGB(folderColor), "Other/");
            row += rowSpacing;
            RB.Print(new Vector2i(x + (indent * 3), y + row), DemoUtil.IndexToRGB(folderColor), "...\n...\n...");

            // Draw packed sprite sheet
            RB.CameraSet(new Vector2i(-168, -172));
            RB.DrawRectFill(new Rect2i(x, y, RB.SpriteSheetGet().sheetSize.width + 4, RB.SpriteSheetGet().sheetSize.height + 4), DemoUtil.IndexToRGB(2));

            RB.SpriteSheetSet(spriteSheet2);
            for (int gx = 0; gx < 6; gx++)
            {
                for (int gy = 0; gy < 6; gy++)
                {
                    RB.DrawSprite(alphaSprite, new Vector2i(x + 2 + (gx * 16), y + 2 + (gy * 16)));
                }
            }

            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawCopy(new Rect2i(0, 0, RB.SpriteSheetGet().sheetSize.width, RB.SpriteSheetGet().sheetSize.height), new Vector2i(x + 2, y + 2));

            Vector2i p0 = new Vector2i((RB.SpriteSheetGet().sheetSize.width / 2) + 6, 8);
            Vector2i p1 = new Vector2i(p0.x - 10, p0.y - 10);
            Vector2i p2 = new Vector2i(p0.x + 10, p0.y - 10);

            RB.DrawTriangleFill(p0, p1, p2, DemoUtil.IndexToRGB(5));
            RB.DrawRectFill(new Rect2i(p0.x - 6, p1.y - 10, 11, 10), DemoUtil.IndexToRGB(5));

            RB.CameraReset();
        }

        private void DrawTMX(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.SpriteSheetSet(spriteSheet1);

            mFormatStr.Set("@C// Load sprite packs at runtime just like any other spritesheet!\n");
            mFormatStr.Append("@NmySprites.Load(@S\"DemoSpritePack\"@N, @MSpriteSheetAsset@N.@MSheetType@N.SpritePack);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Now you can draw sprites using their sprite path and name. There is\n");
            mFormatStr.Append("@C// no need to specify source rectangle, RetroBlit sprite packing\n");
            mFormatStr.Append("@C// creates an internal lookup table that takes care of that for you!\n");

            var outputFrameRect = new Rect2i(x + 130, y + 66, 20, 20);

            DemoUtil.DrawOutputFrame(outputFrameRect, -1, 21, 22);

            if (RB.Ticks % 40 < 20)
            {
                mFormatStr.Append("@MRB@N.DrawSprite(@S\"Characters/Hero1\"@N, @Knew@N @MVector2i@N(@L32@N, @L48@N));\n");
                RB.DrawSprite("Characters/Hero1", new Vector2i(outputFrameRect.x + 2, outputFrameRect.y + 2));
            }
            else
            {
                mFormatStr.Append("@MRB@N.DrawSprite(@S\"Characters/Hero2\"@N, @Knew@N @MVector2i@N(@L32@N, @L48@N));\n");
                RB.DrawSprite("Characters/Hero2", new Vector2i(outputFrameRect.x + 2, outputFrameRect.y + 2));
            }

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            y += 94;

            mFormatStr.Set("@C// You can also use sprite packs with tile maps\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L0@N, @L0@N), @S\"Terrain/GrassTopRight\"@N, @MRB@N.FLIP_H);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L1@N, @L0@N), @S\"Terrain/GrassTop\"@N);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L2@N, @L0@N), @S\"Terrain/GrassTop\"@N);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L3@N, @L0@N), @S\"Terrain/GrassTopRight\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L0@N, @L1@N), @S\"Terrain/DirtSide\"@N, @MRB@N.FLIP_H);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L1@N, @L1@N), @S\"Terrain/DirtCenter\"@N);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L2@N, @L1@N), @S\"Terrain/DirtCenter\"@N);\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew@N @MVector2i@N(@L3@N, @L1@N), @S\"Terrain/DirtSide\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(0);");
            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            y -= 4;

            RB.MapSpriteSet(0, new Vector2i(0, 0), mSpriteGrassTopRight, RB.FLIP_H);
            RB.MapSpriteSet(0, new Vector2i(1, 0), mSpriteGrassTop);
            RB.MapSpriteSet(0, new Vector2i(2, 0), mSpriteGrassTop);
            RB.MapSpriteSet(0, new Vector2i(3, 0), mSpriteGrassTopRight);

            RB.MapSpriteSet(0, new Vector2i(0, 1), mSpriteDirtSide, RB.FLIP_H);
            RB.MapSpriteSet(0, new Vector2i(1, 1), mSpriteDirtCenter);
            RB.MapSpriteSet(0, new Vector2i(2, 1), mSpriteDirtCenter);
            RB.MapSpriteSet(0, new Vector2i(3, 1), mSpriteDirtSide);

            outputFrameRect = new Rect2i(x + 105, y + 95, (16 * 4) + 4, (16 * 2) + 4);
            DemoUtil.DrawOutputFrame(outputFrameRect, -1, 21, 22);

            RB.DrawMapLayer(0, new Vector2i(outputFrameRect.x + 2, outputFrameRect.y + 4));

            y += 140;

            mFormatStr.Set("@C// Sometimes it can be useful to get the sprite source rectangle\n");
            mFormatStr.Append("@Kvar@N sprite = @MRB@N.PackedSpriteGet(@S\"Terrain/Water\"@N);\n");
            mFormatStr.Append("@Kvar@N sourceRect = sprite.SourceRect;\n");
            mFormatStr.Append("sourceRect.x += @L").Append((int)((RB.Ticks / 2) % 16)).Append("@N;\n");
            mFormatStr.Append("@MRB@N.DrawCopy(sourceRect, @Knew@N @MVector2i@N(@L32@N, @L48@N));\n");
            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            outputFrameRect = new Rect2i(x + 130, y + 44, 16 + 4, 16 + 4);
            DemoUtil.DrawOutputFrame(outputFrameRect, -1, 21, 22);

            var sprite = RB.PackedSpriteGet("Terrain/Water");
            var sourceRect = sprite.SourceRect;
            sourceRect.x += (int)((RB.Ticks / 2) % 16);
            sourceRect.width = 16;
            RB.DrawCopy(sourceRect, new Vector2i(outputFrameRect.x + 2, outputFrameRect.y + 2));

            y += 72;

            mFormatStr.Set("@C// You can also use sprite packs for custom fonts, nine-slice images,\n");
            mFormatStr.Append("@C// and when loading map layers from TMX files!\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            y += 20;
            mFormatStr.Set("@w444");

            int count = mCounter;
            int ms = count % 60;
            count /= 60;
            int s = count % 60;
            count /= 60;
            int m = count;

            mFormatStr.Append(m, 2).Append(':').Append(s, 2).Append(':').Append(ms, 2);

            mCounter++;
            if (mCounter >= 60 * 60 * 60)
            {
                mCounter = 0;
            }

            if (font.status == RB.AssetStatus.Ready)
            {
                RB.Print(font, new Vector2i(x + 15, y + 8), Color.white, mFormatStr);
            }

            int xGrow = (int)(Mathf.Sin(RB.Ticks / 40.0f) * 20.0f) + 20 + 20;
            int yGrow = (int)(Mathf.Sin(RB.Ticks / 20.0f) * 18.0f) + 18 + 16;

            RB.DrawNineSlice(new Rect2i(x + 130 - (xGrow / 2), y + 18 - (yGrow / 2), xGrow, yGrow), mNineSlice);

            RB.DrawMapLayer(1, new Vector2i(x + 190, y - 1));
        }
    }
}
