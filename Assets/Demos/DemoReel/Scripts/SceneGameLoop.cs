namespace RetroBlitDemoReel
{
    /// <summary>
    /// Demonstrate the basic game loop
    /// </summary>
    public class SceneGameLoop : SceneDemo
    {
        private const int EXAMPLE_WIDTH = 256;
        private const int EXAMPLE_HEIGHT = 144;
        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();

        private string mDesc =
            "@NWelcome to the RetroBlit Feature Demo Reel!\n" +
            "\n" +
            "@DRetroBlit aims to create an ideal, low friction framework for creating retro games from the early 90s era. RetroBlit takes advantage of the\n" +
            "portability, and the ease of deployment that Unity provides, but does away with the Unity Editor interface in favour of a traditional game loop,\n" +
            "and code-only development. There are no scene graphs, no GameObjects, no MonoBehaviour, there is only a simple low level API for rendering\n" +
            "sprites, fonts, primitives, and tilemaps.\n" +
            "\n" +
            "Have a look at the simple program below:\n";

        private string mMarkedUpCode =
            "@Kpublic class @MMyGame @N: @MRB@N.@EIRetroBlitGame @N{\n" +
            "   @MSpriteSheetAsset@N spriteSheet = @Knew @MSpriteSheetAsset@N();\n" +
            "\n" +
            "   @Kpublic @MHardwareSettings @NQueryHardware() {\n" +
            "      @C// Called at startup to query for your hardware capabilities.\n" +
            "      @Kvar @Nhw = @Knew @MHardwareSettings@N();\n" +
            "      @Nhw.DisplaySize = @Knew @MVector2i@N(@L" + EXAMPLE_WIDTH + "@N, @L" + EXAMPLE_HEIGHT + "@N);\n" +
            "\n" +
            "      @Kreturn @Nhw;\n" +
            "   }\n" +
            "\n" +
            "   @Kpublic bool @NInitialize() {\n" +
            "      @C// RetroBlit will call this method at startup after QueryHardware.\n" +
            "      @NspriteSheet.Load(@S\"HelloWorld/Sprites\"@N);\n" +
            "      @NspriteSheet.grid = @Knew @MSpriteGrid@N(@Knew@N @MVector2i@N(@L16@N, @L16@N));\n" +
            "\n" +
            "      @Kreturn true@N;\n" +
            "   }\n" +
            "\n" +
            "   @Kpublic void @NUpdate() {\n" +
            "      @C// This method is called at a fixed rate of 60 times per second.\n" +
            "   @N}\n" +
            "\n" +
            "   @Kpublic void @NRender() {\n" +
            "      @C// All rendering happens here\n" +
            "      @MRB@N.SpriteSheetSet(spriteSheet);\n" +
            "      @Kint @NspriteIndex = (@MRB@N.Ticks / @L20@N) % @L2@N;\n" +
            "\n" +
            "      @MRB@N.Clear(@I22);\n" +
            "      @MRB@N.DrawSprite@N(spriteIndex, @Knew @MVector2i@N(@L120@N, @L64@N));\n" +
            "      @MRB@N.Print(@Knew @MVector2i@N(@L110@N, @L52@N), @MColor@N.black, @S\"Hi there!\"@N);\n" +
            "   }\n" +
            "}";

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            mSpriteSheet1.Load("DemoReel/Sprites");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            mSpriteSheet1.Unload();
            base.Exit();
        }

        /// <summary>
        /// Handle scene update
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

            RB.Clear(DemoUtil.IndexToRGB(0));

            DrawDesc(4, 4);
            DrawCode(4, 77);
            DrawOutput(350, 77);

            int color = 3;
            if ((RB.Ticks % 200 > 170 && RB.Ticks % 200 < 180) || (RB.Ticks % 200) > 190)
            {
                color = 5;
            }

            RB.Print(new Vector2i(390, 300), DemoUtil.IndexToRGB(color), "LEFT CLICK or TOUCH the screen to move to\nthe next screen, RIGHT CLICK or TOUCH with\nTWO fingers to move to previous screen.");
        }

        private void DrawDesc(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            mFormatStr.Set(mDesc);
            DemoUtil.HighlightCode(mFormatStr, mFinalStr, false);

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), mFinalStr);
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), "Source code:");
            RB.DrawRectFill(new Rect2i(x, y + 10, 315, 270), DemoUtil.IndexToRGB(1));

            mFormatStr.Set(mMarkedUpCode);
            DemoUtil.HighlightCode(mFormatStr, mFinalStr);

            RB.Print(new Vector2i(x + 4, y + 14), DemoUtil.IndexToRGB(5), mFinalStr);
        }

        private void DrawOutput(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), "Output:");

            DrawOutputScreen(x, y + 10);
        }

        private void DrawOutputScreen(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.SpriteSheetSet(mSpriteSheet1);

            RB.CameraSet(new Vector2i(-x, -y));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, EXAMPLE_WIDTH, EXAMPLE_HEIGHT), 4, 2, 22);

            int spriteIndex = ((int)RB.Ticks / 20) % 2;
            RB.DrawSprite(spriteIndex, new Vector2i(120, 64));

            RB.Print(new Vector2i(110, 52), DemoUtil.IndexToRGB(0), "Hi there!");

            RB.CameraReset();
        }
    }
}
