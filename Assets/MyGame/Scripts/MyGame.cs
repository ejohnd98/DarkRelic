using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Your game! You can of course rename this class to whatever you'd like.
/// </summary>
public class MyGame : RB.IRetroBlitGame
{
    private readonly SpriteSheetAsset mSpriteSheet = new SpriteSheetAsset();

    /// <summary>
    /// Query hardware. Here you initialize your retro game hardware.
    /// </summary>
    /// <returns>Hardware settings</returns>
    public RB.HardwareSettings QueryHardware()
    {
        var hw = new RB.HardwareSettings
        {
            // Set your display size
            DisplaySize = new Vector2i(640, 360)

            // Set tilemap maximum size, default is 256, 256. Keep this close to your minimum required size to save on memory
            //// MapSize = new Vector2i(256, 256)

            // Set tilemap maximum layers, default is 8. Keep this close to your minimum required size to save on memory
            //// MapLayers = 8
        };

        return hw;
    }

    /// <summary>
    /// Initialize your game here.
    /// </summary>
    /// <returns>Return true if successful</returns>
    public bool Initialize()
    {
        // You can load a spritesheet here
        mSpriteSheet.Load("16x16_tileset");
        mSpriteSheet.grid = new SpriteGrid(new Vector2i(16, 16));

        RB.SpriteSheetSet(mSpriteSheet);

        return true;
    }

    /// <summary>
    /// Update, your game logic should live here. Update is called at a fixed interval of 60 times per second.
    /// </summary>
    ///
    public void Update()
    {
        if (RB.ButtonPressed(RB.BTN_SYSTEM))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Render, your drawing code should go here.
    /// </summary>
    public void Render()
    {
        RB.Clear(new Color32(15, 20, 40, 255));
        RB.SpriteSheetSet(mSpriteSheet);
        RB.DrawSprite(0, new Vector2i(100, 100)); 
        
    }
}
