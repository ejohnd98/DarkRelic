namespace RetroBlitDemoReel
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate sound and music
    /// </summary>
    public class SceneSound : SceneDemo
    {
        private const float PIANO_C_PITCH = 261.626f;

        private int mMusicTicks = 0;
        private int mMusicTurnSpeed = 0;
        private List<SoundReference> mFadeSounds = new List<SoundReference>();

        private Rect2i mPianoRect;

        private Button[] mEffectButtons;
        private Button[] mPianoButtons;

        private Button mMusicPlayButton;

        private Button mPositionalButton;

        private Button mNextButton;
        private Button mPrevButton;

        private AudioAsset soundCoin = new AudioAsset();
        private AudioAsset soundExplosion = new AudioAsset();
        private AudioAsset soundJump = new AudioAsset();
        private AudioAsset soundLaser = new AudioAsset();
        private AudioAsset soundC5Note = new AudioAsset();
        private AudioAsset soundWaterfall = new AudioAsset();

        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset mSpriteSheet2 = new SpriteSheetAsset();

        private SoundReference mPosSoundRef;

        private Vector2i mListenerPos;
        private Vector2i mSoundPos;

        private bool mPositionalPlaying = false;
        private int mPositionalTicks = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneSound()
        {
            InitPiano();
            InitNoises();
            InitMusic();
            InitPositional();

            mNextButton = new Button(new Rect2i(550, 334, 87, 23), new Rect2i(550, 334, 87, 23), 3, 2, "Touch here to go\nto the next screen", (KeyCode)555, 0, this.NextScreenButtonCB);
            mPrevButton = new Button(new Rect2i(550 - 114, 334, 87 + 20, 23), new Rect2i(550 - 114, 334, 87 + 20, 23), 3, 2, "Touch here to go\nto the previous screen", (KeyCode)554, 0, this.PrevScreenButtonCB);
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            soundCoin.Load("DemoReel/Coin");
            soundExplosion.Load("DemoReel/Explosion");
            soundJump.Load("DemoReel/Jump");
            soundLaser.Load("DemoReel/Laser");
            soundC5Note.Load("DemoReel/C5Note");
            soundWaterfall.Load("DemoReel/Waterfall");

            mMusicTicks = 0;
            mMusicTurnSpeed = 0;
            mNextButton.Reset();
            mPrevButton.Reset();
            mMusicPlayButton.Reset();
            mPositionalButton.Reset();

            for (int i = 0; i < mPianoButtons.Length; i++)
            {
                mPianoButtons[i].Reset();
            }

            for (int i = 0; i < mEffectButtons.Length; i++)
            {
                mEffectButtons[i].Reset();
            }

            mSpriteSheet1.Load("DemoReel/Sprites");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            mSpriteSheet2.Create(RB.DisplaySize);

            mPosSoundRef = RB.SoundPlay(soundWaterfall, 0, 1, 100);
            RB.SoundLoopSet(mPosSoundRef, true);

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            soundCoin.Unload();
            soundExplosion.Unload();
            soundJump.Unload();
            soundLaser.Unload();
            soundC5Note.Unload();
            soundWaterfall.Unload();

            mSpriteSheet1.Unload();
            mSpriteSheet2.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            var demo = (DemoReel)RB.Game;

            for (int i = mFadeSounds.Count - 1; i >= 0; i--)
            {
                var soundRef = mFadeSounds[i];
                RB.SoundVolumeSet(soundRef, RB.SoundVolumeGet(soundRef) * 0.75f);

                if (RB.SoundVolumeGet(soundRef) < 0.01f)
                {
                    mFadeSounds.RemoveAt(i);
                }
            }

            if (!RB.MusicIsPlaying())
            {
                mMusicTurnSpeed--;
                if (mMusicTurnSpeed < 0)
                {
                    mMusicTurnSpeed = 0;
                }
            }

            mMusicTicks += mMusicTurnSpeed;

            for (int i = 0; i < mPianoButtons.Length; i++)
            {
                mPianoButtons[i].Update();
                if (mPianoButtons[i].Pressed)
                {
                    break;
                }
            }

            for (int i = 0; i < mEffectButtons.Length; i++)
            {
                mEffectButtons[i].Update();
                if (mEffectButtons[i].Pressed)
                {
                    break;
                }
            }

            mMusicPlayButton.Update();
            mPositionalButton.Update();

            mNextButton.Update();
            mPrevButton.Update();

            int color = 1;
            if ((RB.Ticks % 200 > 170 && RB.Ticks % 200 < 180) || (RB.Ticks % 200) > 190)
            {
                color = 5;
            }

            mNextButton.LabelColor = color;
            mPrevButton.LabelColor = color;

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }

            mListenerPos = new Vector2i(330 + (Mathf.Sin(mPositionalTicks / 50.0f) * 100), (RB.DisplaySize.height / 2) + 32);

            if (mPositionalPlaying)
            {
                mSoundPos = RB.PointerPos();
                RB.SoundPosSet(mPosSoundRef, mSoundPos);
            }

            RB.SoundListenerPosSet(mListenerPos);

            var positionalVol = RB.SoundVolumeGet(mPosSoundRef);

            // Change music volume to inverse of waterfall volume
            if (demo.IsMusicPlaying())
            {
                RB.MusicVolumeSet((1.0f - positionalVol) * demo.MusicVolume());
            }

            if (mPositionalPlaying)
            {
                mPositionalTicks++;

                positionalVol += 0.05f;
                if (positionalVol > 1)
                {
                    positionalVol = 1;
                }

                RB.SoundVolumeSet(mPosSoundRef, positionalVol);
            }
            else
            {
                positionalVol -= 0.05f;
                if (positionalVol < 0)
                {
                    positionalVol = 0;
                }

                RB.SoundVolumeSet(mPosSoundRef, positionalVol);
            }

            UpdateMusicButtonLabel();
            UpdatePositionalButtonLabel();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawNoisePad(4, 4);
            DrawPiano(4, 200);
            DrawMusicPlayer(450, 4);
            DrawPositional(220, 4);

            mNextButton.Render();
            mPrevButton.Render();

            if (RB.PointerPosValid() && !mPositionalPlaying)
            {
                RB.DrawSprite(4, RB.PointerPos());
            }
        }

        private void EffectButtonPressedCB(Button button, object userData)
        {
            RB.SoundPlay((AudioAsset)userData);
        }

        private void PianoButtonPressedCB(Button button, object userData)
        {
            var info = (PianoKeyInfo)userData;
            info.SoundRef = PlayNote(info.Pitch);
        }

        private void PianoButtonReleasedCB(Button button, object userData)
        {
            var info = (PianoKeyInfo)userData;
            mFadeSounds.Add(info.SoundRef);
        }

        private void MusicButtonPressedCB(Button button, object userData)
        {
            var demo = (DemoReel)RB.Game;

            if (button == mMusicPlayButton)
            {
                if (!RB.MusicIsPlaying())
                {
                    demo.MusicPlay();
                    mMusicTurnSpeed = 50;
                }
                else
                {
                    demo.MusicStop();
                }
            }

            UpdateMusicButtonLabel();
        }

        private void PositionalButtonPressedCB(Button button, object userData)
        {
            var demo = (DemoReel)RB.Game;

            if (button == mPositionalButton)
            {
                mPositionalPlaying = !mPositionalPlaying;
            }

            UpdatePositionalButtonLabel();
        }

        private void NextScreenButtonCB(Button button, object userData)
        {
            var demo = (DemoReel)RB.Game;
            demo.NextScene();
        }

        private void PrevScreenButtonCB(Button button, object userData)
        {
            var demo = (DemoReel)RB.Game;
            demo.PreviousScene();
        }

        private SoundReference PlayNote(float pitch)
        {
            float basePitch = PIANO_C_PITCH;
            float finalPitch = 1.0f + ((pitch - basePitch) / basePitch);

            return RB.SoundPlay(soundC5Note, 1.0f, finalPitch);
        }

        private void InitPiano()
        {
            int xStart = 8;
            int yStart = 8;
            int w = 24;
            int h = 102;
            int bw = 14;
            int bh = 60;

            int space = 2;

            int x = xStart;
            int y = yStart;

            mPianoButtons = new Button[12];
            mPianoButtons[0] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "R", KeyCode.R, new PianoKeyInfo(261.626f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[1] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "T", KeyCode.T, new PianoKeyInfo(293.665f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[2] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "Y", KeyCode.Y, new PianoKeyInfo(329.628f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[3] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "U", KeyCode.U, new PianoKeyInfo(349.228f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[4] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "I", KeyCode.I, new PianoKeyInfo(391.995f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[5] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "O", KeyCode.O, new PianoKeyInfo(440.000f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[6] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y + 50, w, h - 50), 4, 0, "P", KeyCode.P, new PianoKeyInfo(493.883f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);

            x = xStart;
            y = yStart;

            x += (w / 2) + (space * 2);
            mPianoButtons[7] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "5", KeyCode.Alpha5, new PianoKeyInfo(277.182f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[8] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "6", KeyCode.Alpha6, new PianoKeyInfo(311.127f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            x += w + space;
            mPianoButtons[9] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "7", KeyCode.Alpha7, new PianoKeyInfo(369.994f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[10] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "8", KeyCode.Alpha8, new PianoKeyInfo(415.305f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;
            mPianoButtons[11] = new Button(new Rect2i(x, y, bw, bh), new Rect2i(x, y, bw, bh), 1, 4, "9", KeyCode.Alpha9, new PianoKeyInfo(466.164f), this.PianoButtonPressedCB, this.PianoButtonReleasedCB, true);
            x += w + space;

            mPianoRect = new Rect2i(xStart, yStart, (7 * (w + space)) - space, h);
        }

        private void InitNoises()
        {
            int w = 70;
            int h = 28;
            int x = 0;
            int y = 0;

            mEffectButtons = new Button[4];
            mEffectButtons[0] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "1 - Coin", KeyCode.Alpha1, soundCoin, this.EffectButtonPressedCB);
            x += w + 2;
            mEffectButtons[1] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "2 - Explosion", KeyCode.Alpha2, soundExplosion, this.EffectButtonPressedCB);
            y += h + 2;
            x = 0;
            mEffectButtons[2] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "3 - Jump", KeyCode.Alpha3, soundJump, this.EffectButtonPressedCB);
            x += w + 2;
            mEffectButtons[3] = new Button(new Rect2i(x, y, w, h), new Rect2i(x, y, w, h), 4, 0, "4 - Laser", KeyCode.Alpha4, soundLaser, this.EffectButtonPressedCB);
        }

        private void InitMusic()
        {
            mMusicPlayButton = new Button(new Rect2i(55, 90, 80, 36), new Rect2i(55, 90, 80, 36), 4, 0, "Play", KeyCode.H, 0, this.MusicButtonPressedCB);
        }

        private void InitPositional()
        {
            mPositionalButton = new Button(new Rect2i(55, 120, 80, 36), new Rect2i(55, 120, 80, 36), 4, 0, "Play", KeyCode.G, 0, this.PositionalButtonPressedCB);
        }

        private void DrawPiano(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y));

            mFormatStr.Set("@C// Play sound at specific volume and pitch\n");
            mFormatStr.Append("@NsoundC5Note.Load(@S\"Demo/C5Note\"@N);\n");
            mFormatStr.Append("@MRB@N.SoundPlay(soundC5Note, @L0.@L5f@N, @L1.@L2f@N);\n");

            RB.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraSet(new Vector2i(-x, -y - 35));

            Rect2i pianoRect = mPianoRect;
            Rect2i holeRect = pianoRect;
            pianoRect = pianoRect.Expand(8);
            holeRect = holeRect.Expand(2);

            int cornerSize = 8;

            RB.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(pianoRect.x + cornerSize, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + pianoRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawRect(new Rect2i(pianoRect.x + cornerSize, pianoRect.y, pianoRect.width - (cornerSize * 2), pianoRect.height), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(pianoRect.x + cornerSize, pianoRect.y + 1, pianoRect.width - (cornerSize * 2), pianoRect.height - 2), DemoUtil.IndexToRGB(3));

            RB.DrawRect(new Rect2i(pianoRect.x, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(pianoRect.x + 1, pianoRect.y + cornerSize, cornerSize - 1, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

            RB.DrawRect(new Rect2i(pianoRect.x + pianoRect.width - cornerSize, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(pianoRect.x + pianoRect.width - cornerSize - 1, pianoRect.y + cornerSize, cornerSize, pianoRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

            RB.DrawRectFill(holeRect, DemoUtil.IndexToRGB(2));

            for (int i = 0; i < mPianoButtons.Length; i++)
            {
                mPianoButtons[i].Render();
            }

            RB.CameraReset();
        }

        private void DrawNoisePad(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y));

            mFormatStr.Set("@C// Load sounds and play them\n");
            mFormatStr.Append("@NsoundCoin.Load(@S\"Demo/Coin\"@N);\n");
            mFormatStr.Append("@NsoundExplosion.Load(@S\"Demo/Explosion\"@N);\n");
            mFormatStr.Append("@NsoundJump.Load(@S\"Demo/Jump\"@N);\n");
            mFormatStr.Append("@NsoundLaser.Load(@S\"Demo/Laser\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kif@N (@MRB@N.KeyboardPressed(@MKeyCode@N.Alpha1) {\n");
            mFormatStr.Append("   @MRB@N.SoundPlay(soundCoin);\n");
            mFormatStr.Append("} @Kelse if@N (@MRB@N.KeyboardPressed(@MKeyCode@N.Alpha2) {\n");
            mFormatStr.Append("   @MRB@N.SoundPlay(soundExplosion);\n");
            mFormatStr.Append("} @Kelse if@N (@MRB@N.KeyboardPressed(@MKeyCode@N.Alpha3) {\n");
            mFormatStr.Append("   @MRB@N.SoundPlay(soundJump);\n");
            mFormatStr.Append("} @Kelse if@N (@MRB@N.KeyboardPressed(@MKeyCode@N.Alpha4) {\n");
            mFormatStr.Append("   @MRB@N.SoundPlay(soundLaser);\n");
            mFormatStr.Append("}");

            RB.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraSet(new Vector2i(-x, -y - 123));
            for (int i = 0; i < mEffectButtons.Length; i++)
            {
                mEffectButtons[i].Render();
            }

            RB.CameraReset();
        }

        private void DrawSpinner(int x, int y, int spinnerSize)
        {
            var demo = (DemoReel)RB.Game;

            RB.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), DemoUtil.IndexToRGB(1));
            RB.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize - 6, spinnerSize - 6), DemoUtil.IndexToRGB(4));
            RB.DrawEllipseFill(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(8, 8), DemoUtil.IndexToRGB(1));
            RB.DrawEllipse(new Vector2i(x + spinnerSize, y + spinnerSize), new Vector2i(spinnerSize, spinnerSize), DemoUtil.IndexToRGB(4));

            RB.SpriteSheetSet(mSpriteSheet2);
            RB.DrawCopy(new Rect2i(0, 0, (spinnerSize * 2) + 1, (spinnerSize * 2) + 1), new Rect2i(x, y, (spinnerSize * 2) + 1, (spinnerSize * 2) + 1), new Vector2i(spinnerSize, spinnerSize), mMusicTicks / 50);
            RB.SpriteSheetSet(mSpriteSheet1);
        }

        private void DrawMusicPlayer(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.Offscreen(mSpriteSheet2);
            RB.Clear(new Color32(0, 0, 0, 0));

            int spinnerSize = 32;
            RB.DrawRectFill(new Rect2i(spinnerSize / 4, spinnerSize - 2, (spinnerSize / 2) - 2, 5), DemoUtil.IndexToRGB(1));
            RB.DrawRectFill(new Rect2i(spinnerSize + (spinnerSize / 4) + 3, spinnerSize - 2, (spinnerSize / 2) - 2, 5), DemoUtil.IndexToRGB(1));
            RB.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize / 4, 5, (spinnerSize / 2) - 2), DemoUtil.IndexToRGB(1));
            RB.DrawRectFill(new Rect2i(spinnerSize - 2, spinnerSize + (spinnerSize / 4) + 3, 5, (spinnerSize / 2) - 2), DemoUtil.IndexToRGB(1));

            RB.Onscreen();

            RB.CameraSet(new Vector2i(-x, -y));

            mFormatStr.Set("@C// Load music and play it\n");
            mFormatStr.Append("@NdemoMusic.Load(@S\"Demo/Music\"@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kif@N (@MRB@N.KeyboardPressed(@MKeyCode@N.H) {\n");
            mFormatStr.Append("   @MRB@N.MusicPlay(demoMusic);\n");
            mFormatStr.Append("} @Kelse if@N (@MRB@N.KeyboardPressed(@MKeyCode@N.J) {\n");
            mFormatStr.Append("   @MRB@N.MusicStop();\n");
            mFormatStr.Append("}");

            RB.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraSet(new Vector2i(-x, -y - 80));

            int cornerSize = 8;
            var deckRect = new Rect2i(20, 40, 145, 100);

            RB.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(deckRect.x + cornerSize, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawEllipseFill(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(3));
            RB.DrawEllipse(new Vector2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + deckRect.height - cornerSize - 1), new Vector2i(cornerSize, cornerSize), DemoUtil.IndexToRGB(2));

            RB.DrawRect(new Rect2i(deckRect.x + cornerSize, deckRect.y, deckRect.width - (cornerSize * 2), deckRect.height), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(deckRect.x + cornerSize, deckRect.y + 1, deckRect.width - (cornerSize * 2), deckRect.height - 2), DemoUtil.IndexToRGB(3));

            RB.DrawRect(new Rect2i(deckRect.x, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(deckRect.x + 1, deckRect.y + cornerSize, cornerSize - 1, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

            RB.DrawRect(new Rect2i(deckRect.x + deckRect.width - cornerSize, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(2));
            RB.DrawRectFill(new Rect2i(deckRect.x + deckRect.width - cornerSize - 1, deckRect.y + cornerSize, cornerSize, deckRect.height - (cornerSize * 2)), DemoUtil.IndexToRGB(3));

            DrawSpinner(0, 0, spinnerSize);
            DrawSpinner(120, 0, spinnerSize);

            mMusicPlayButton.Render();

            RB.CameraReset();
        }

        private void DrawPositional(int x, int y)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            mFormatStr.Set("@C// RetroBlit supports positional sounds. Simply\n");
            mFormatStr.Append("@C// set the position of your sound, and the\n");
            mFormatStr.Append("@C// position of the listener!\n");
            mFormatStr.Append("@N\n");
            mFormatStr.Append("@Kvoid@N Init() {\n");
            mFormatStr.Append("   sndRef = @MRB@N.SoundPlay(waterfallSnd);\n");
            mFormatStr.Append("   @MRB@N.SoundLoopSet(sndRef, @Ltrue@N);\n");
            mFormatStr.Append("}\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kvoid@N Update() {\n");
            mFormatStr.Append("   @MRB@N.SoundPosSet(sndRef, @Knew@N @MVector2i@N(@L").Append(mSoundPos.x).Append("@N, @L").Append(mSoundPos.y).Append("@N));\n");
            mFormatStr.Append("   @MRB@N.SoundListenerPosSet(@Knew@N @MVector2i@N(@L").Append(mListenerPos.x).Append("@N, @L").Append(mListenerPos.y).Append("@N));\n");
            mFormatStr.Append("}\n");

            RB.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            mPositionalButton.Render();

            for (int i = 0; i < (int)(10 * RB.SoundVolumeGet(mPosSoundRef)); i++)
            {
                int radius = 3;
                RB.DrawEllipse(new Vector2i((i * 23) + 4, mListenerPos.y - 3), new Vector2i(radius, radius), DemoUtil.IndexToRGB(2));
            }

            // Use absolute coordinates for drawing ear/rain drop
            RB.CameraReset();

            var volAlpha = (byte)(RB.SoundVolumeGet(mPosSoundRef) * 255);

            RB.AlphaSet(volAlpha);
            RB.DrawLine(mListenerPos, mSoundPos, DemoUtil.IndexToRGB(17));
            RB.DrawCopy(new Rect2i(0, 224, 14, 24), mListenerPos - new Vector2i(14 / 2, 24 / 2));
            RB.DrawCopy(new Rect2i(14, 224, 14, 24), mSoundPos - new Vector2i(14 / 2, 24 / 2));
            RB.AlphaSet(255);
        }

        private void UpdateMusicButtonLabel()
        {
            var demo = (DemoReel)RB.Game;

            if (RB.MusicIsPlaying())
            {
                mMusicTurnSpeed = 50;
                mMusicPlayButton.Label = "H - Stop";
            }
            else
            {
                mMusicPlayButton.Label = "H - Play";
            }
        }

        private void UpdatePositionalButtonLabel()
        {
            if (mPositionalPlaying)
            {
                mPositionalButton.Label = "G - Stop";
            }
            else
            {
                mPositionalButton.Label = "G - Play";
            }
        }

        private class PianoKeyInfo
        {
            public float Pitch;
            public SoundReference SoundRef;

            public PianoKeyInfo(float pitch)
            {
                Pitch = pitch;
            }
        }
    }
}
