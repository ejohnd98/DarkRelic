using System.Collections.Generic;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// Font asset
/// </summary>
/// <remarks>
/// Font asset which holds various information about a font. Font assets use <see cref="SpriteSheetAsset"/> for the font bitmap data. Use <see cref="FontAsset.Setup"/> to configure the font asset.
/// </remarks>
public class FontAsset : RBAsset
{
    /// <summary>
    /// Internal font state, do not change.
    /// </summary>
    public FontInternalState internalState;

    /// <summary>
    /// SpriteSheet used by this Font
    /// </summary>
    public SpriteSheetAsset SpriteSheet
    {
        get
        {
            return internalState.spriteSheet;
        }
    }

    /// <summary>
    /// Setup a custom font from a sprite sheet.
    /// </summary>
    /// <remarks>
    /// Setup a custom font that can be used with <see cref="RB.Print"/>. Fonts are setup for a contiguous range of unicode characters
    /// specified by *unicodeStart* and *unicodeEnd* parameters. Unicode gaps are not allowed, all glyphs in the given range must be provided,
    /// even if they are blank.
    ///
    /// Glyphs in a font can either come from a grid of sprites in a sprite sheet with the given *glyphsPerRow*. Glyphs from a sprite pack
    /// can be explicitly listed with the *glyphSprites* parameter.
    ///
    /// The *glyphSize* specifies the maximum space a single glyph will take, and corresponds to the size of each grid cell when specifying
    /// glyphs by a grid of sprites. RetroBlit will automatically trim empty horizontal white space to the left and right of each glyph, unless
    /// the *monospaced* parameter is specified, in which case empty space is not trimmed at all, and all glyphs will occupy the same horizontal
    /// and vertical space.
    ///
    /// Printed glyphs are spaced out with 1 pixel horizontal and vertical spacing by default. The spacing can optionally be altered with the
    /// *characterSpacing* and *lineSpacing* parameters.
    /// <image src="custom_font_spritesheet.png">Example sprite sheet containing a grid of font glyphs. See example code below on how
    /// this font could be loaded.</image>
    /// <seedoc>Features:Fonts</seedoc>
    /// </remarks>
    /// <param name="unicodeStart">First unicode character in the font</param>
    /// <param name="unicodeEnd">Last unicode character in the font</param>
    /// <param name="srcPos">Top left corner of the set of glyphs in the sprite sheet</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="glyphSize">Dimensions of a single glyph</param>
    /// <param name="glyphsPerRow">Amount of glyphs per row in the sprite sheet</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    /// <code>
    /// SpriteSheetAsset spritesheetMain = new SpriteSheetAsset();
    /// FontAsset retroFont = new FontAsset();
    ///
    /// void Initialize() {
    ///     spritesheetMain.Load("spritesheets/main");
    ///
    ///     retroFont.Setup(
    ///         'A', 'Z', // Character range, all characters between A to Z
    ///         new Vector2i(0, 16), // Position of the top left corner of the sprite grid
    ///         spritesheetMain, // Sprite sheet
    ///         new Vector2i(12, 12), // Glyph size / grid cell size
    ///         10, // Glyphs per row
    ///         1, // Horizontal spacing between characters
    ///         2, // Vertical spacing between lines of text
    ///         false // Not mono-space
    ///         );
    /// }
    ///
    /// void Render() {
    ///     // These characters will all be be printed
    ///     RB.Print(retroFont, new Vector2i(0, 0), "HELLO THERE", Color.white, text);
    ///
    ///     // Only "H" and "T" will be printed because the other characters are not in
    ///     // the character set for this font!
    ///     RB.Print(retroFont, new Vector2i(0, 32), "Hello There", Color.white, text);
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public RB.AssetStatus Setup(char unicodeStart, char unicodeEnd, Vector2i srcPos, SpriteSheetAsset spriteSheet, Vector2i glyphSize, int glyphsPerRow, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (unicodeStart < 0 || unicodeEnd < unicodeStart)
        {
            Debug.LogError("Invalid unicode range");
            return status;
        }

        if (glyphSize.width < 0 || glyphSize.height < 0)
        {
            Debug.LogError("Invalid glyph size");
            return status;
        }

        if (glyphsPerRow <= 0)
        {
            Debug.LogError("Invalid glyphs per row");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (srcPos.x < 0 || srcPos.y < 0 ||
            srcPos.x + (glyphsPerRow * glyphSize.width) > spriteSheet.internalState.textureWidth ||
            srcPos.y + ((((unicodeEnd - unicodeStart) / glyphsPerRow) + 1) * glyphSize.height) > spriteSheet.internalState.texture.height)
        {
            Debug.LogError("Invalid font sprite sheet coordinates, out of bounds");
            return status;
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, unicodeStart, unicodeEnd, null, srcPos, spriteSheet, glyphSize, glyphsPerRow, characterSpacing, lineSpacing, monospaced, false, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from a sprite sheet.
    /// </summary>
    /// <remarks>
    /// Setup a custom font that can be used with <see cref="RB.Print"/>. Fonts are setup for a contiguous range of unicode characters
    /// specified by *unicodeStart* and *unicodeEnd* parameters. Unicode gaps are not allowed, all glyphs in the given range must be provided,
    /// even if they are blank.
    ///
    /// Glyphs in a font can either come from a grid of sprites in a sprite sheet with the given *glyphsPerRow*. Glyphs from a sprite pack
    /// can be explicitly listed with the *glyphSprites* parameter.
    ///
    /// The *glyphSize* specifies the maximum space a single glyph will take, and corresponds to the size of each grid cell when specifying
    /// glyphs by a grid of sprites. RetroBlit will automatically trim empty horizontal white space to the left and right of each glyph, unless
    /// the *monospaced* parameter is specified, in which case empty space is not trimmed at all, and all glyphs will occupy the same horizontal
    /// and vertical space.
    ///
    /// Printed glyphs are spaced out with 1 pixel horizontal and vertical spacing by default. The spacing can optionally be altered with the
    /// *characterSpacing* and *lineSpacing* parameters.
    /// <image src="custom_font_spritesheet.png">Example sprite sheet containing a grid of font glyphs. See example code below on how
    /// this font could be loaded.</image>
    /// <seedoc>Features:Fonts</seedoc>
    /// </remarks>
    /// <param name="characterList">List of all characters in this font. If there are duplicate characters the last copy is used.</param>
    /// <param name="srcPos">Top left corner of the set of glyphs in the sprite sheet</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="glyphSize">Dimensions of a single glyph</param>
    /// <param name="glyphsPerRow">Amount of glyphs per row in the sprite sheet</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public RB.AssetStatus Setup(List<char> characterList, Vector2i srcPos, SpriteSheetAsset spriteSheet, Vector2i glyphSize, int glyphsPerRow, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (glyphSize.width < 0 || glyphSize.height < 0)
        {
            Debug.LogError("Invalid glyph size");
            return status;
        }

        if (glyphsPerRow <= 0)
        {
            Debug.LogError("Invalid glyphs per row");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (srcPos.x < 0 || srcPos.y < 0 ||
            srcPos.x + (glyphsPerRow * glyphSize.width) > spriteSheet.internalState.texture.width ||
            srcPos.y + ((((characterList.Count - 1) / glyphsPerRow) + 1) * glyphSize.height) > spriteSheet.internalState.texture.height)
        {
            Debug.LogError("Invalid font sprite sheet coordinates, out of bounds");
            return status;
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, '\0', '\0', characterList, srcPos, spriteSheet, glyphSize, glyphsPerRow, characterSpacing, lineSpacing, monospaced, false, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="unicodeStart">First unicode character in the font</param>
    /// <param name="unicodeEnd">Last unicode character in the font</param>
    /// <param name="glyphSprites">List of packed sprites to use for the glyphs</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(char unicodeStart, char unicodeEnd, List<PackedSprite> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (unicodeStart < 0 || unicodeEnd < unicodeStart)
        {
            Debug.LogError("Invalid unicode range");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if ((unicodeEnd - unicodeStart + 1) != glyphSprites.Count)
        {
            Debug.LogError("Expected " + (unicodeEnd - unicodeStart + 1) + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, unicodeStart, unicodeEnd, null, glyphSprites, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="unicodeStart">First unicode character in the font</param>
    /// <param name="unicodeEnd">Last unicode character in the font</param>
    /// <param name="glyphSprites">List of packed sprite ids to use for the glyphs</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(char unicodeStart, char unicodeEnd, List<PackedSpriteID> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (unicodeStart < 0 || unicodeEnd < unicodeStart)
        {
            Debug.LogError("Invalid unicode range");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if ((unicodeEnd - unicodeStart + 1) != glyphSprites.Count)
        {
            Debug.LogError("Expected " + (unicodeEnd - unicodeStart + 1) + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, unicodeStart, unicodeEnd, null, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="unicodeStart">First unicode character in the font</param>
    /// <param name="unicodeEnd">Last unicode character in the font</param>
    /// <param name="glyphSprites">List of packed sprite names to use for the glyphs</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(char unicodeStart, char unicodeEnd, List<FastString> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 1;

        if (unicodeStart < 0 || unicodeEnd < unicodeStart)
        {
            Debug.LogError("Invalid unicode range");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if ((unicodeEnd - unicodeStart + 1) != glyphSprites.Count)
        {
            Debug.LogError("Expected " + (unicodeEnd - unicodeStart + 1) + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, unicodeStart, unicodeEnd, null, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="unicodeStart">First unicode character in the font</param>
    /// <param name="unicodeEnd">Last unicode character in the font</param>
    /// <param name="glyphSprites">List of packed sprite names to use for the glyphs</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(char unicodeStart, char unicodeEnd, List<string> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (unicodeStart < 0 || unicodeEnd < unicodeStart)
        {
            Debug.LogError("Invalid unicode range");
            return status;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if ((unicodeEnd - unicodeStart + 1) != glyphSprites.Count)
        {
            Debug.LogError("Expected " + (unicodeEnd - unicodeStart + 1) + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, unicodeStart, unicodeEnd, null, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="characterList">List of all characters in this font. Must be in same order as glyphSprites</param>
    /// <param name="glyphSprites">List of packed sprites to use for the glyphs. Must be in same order as characterList</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(List<char> characterList, List<PackedSprite> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if (characterList.Count != glyphSprites.Count)
        {
            Debug.LogError("Expected " + characterList.Count + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, 0, 0, characterList, glyphSprites, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="characterList">List of all characters in this font. Must be in same order as glyphSprites</param>
    /// <param name="glyphSprites">List of packed sprites to use for the glyphs. Must be in same order as characterList</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(List<char> characterList, List<PackedSpriteID> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if (characterList.Count != glyphSprites.Count)
        {
            Debug.LogError("Expected " + characterList.Count + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, 0, 0, characterList, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="characterList">List of all characters in this font. Must be in same order as glyphSprites</param>
    /// <param name="glyphSprites">List of packed sprites to use for the glyphs. Must be in same order as characterList</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(List<char> characterList, List<FastString> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if (characterList.Count != glyphSprites.Count)
        {
            Debug.LogError("Expected " + characterList.Count + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, 0, 0, characterList, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Setup a custom font from the sprite sheet.
    /// </summary>
    /// <remarks>The glyphs in your sprite sheet should be organized into a grid, with each cell size being the same. If <paramref name="monospaced"/> is false then RetroBlit will automatically trim any
    /// horizontal empty space on either side of the glyph. If <paramref name="monospaced"/> is true then the empty space is retained.</remarks>
    /// <param name="characterList">List of all characters in this font. Must be in same order as glyphSprites</param>
    /// <param name="glyphSprites">List of packed sprites to use for the glyphs. Must be in same order as characterList</param>
    /// <param name="spriteSheet">The sprite sheet containing the font</param>
    /// <param name="characterSpacing">Spacing between characters</param>
    /// <param name="lineSpacing">Line spacing between lines of text</param>
    /// <param name="monospaced">True if font is monospaced</param>
    /// <returns>Setup status</returns>
    public RB.AssetStatus Setup(List<char> characterList, List<string> glyphSprites, SpriteSheetAsset spriteSheet, int characterSpacing, int lineSpacing, bool monospaced)
    {
        InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);

        progress = 0;

        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return status;
        }

        if (spriteSheet.status != RB.AssetStatus.Ready)
        {
            Debug.LogError("Sprite sheet is not ready or not loaded");
            return status;
        }

        if (glyphSprites == null)
        {
            Debug.LogError("Glyph sprites list is null!");
            return status;
        }

        if (characterList.Count != glyphSprites.Count)
        {
            Debug.LogError("Expected " + characterList.Count + " glyph sprites to cover the given unicode range, was given " + glyphSprites.Count + " glyph sprites");
            return status;
        }

        var glyphSprites2 = new List<PackedSprite>(glyphSprites.Count);
        for (int i = 0; i < glyphSprites.Count; i++)
        {
            glyphSprites2.Add(RB.PackedSpriteGet(glyphSprites[i], spriteSheet));
        }

        RetroBlitInternal.RBAPI.instance.Font.FontSetup(this, 0, 0, characterList, glyphSprites2, spriteSheet, characterSpacing, lineSpacing, monospaced, true);

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        progress = 1;

        return status;
    }

    /// <summary>
    /// Unload a previously loaded FontAsset
    /// </summary>
    public override void Unload()
    {
        ResetInternalState();
        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);
        progress = 0;
    }

    private void ResetInternalState()
    {
        internalState = new FontInternalState();
    }

    /// <summary>
    /// Internal font state
    /// </summary>
    public struct FontInternalState
    {
        /// <summary>
        /// Source position in sprite sheet
        /// </summary>
        public Vector2i srcPos;

        /// <summary>
        /// Is monospaced
        /// </summary>
        public bool monospaced;

        /// <summary>
        /// Glyphs per row
        /// </summary>
        public int glyphsPerRow;

        /// <summary>
        /// Glyph size
        /// </summary>
        public Vector2i glyphSize;

        /// <summary>
        /// Glyph definition
        /// </summary>
        public RetroBlitInternal.RBFont.CharHashTable<RetroBlitInternal.RBFont.GlyphDef> glyphDef;

        /// <summary>
        /// Are glyph dimensions calculated
        /// </summary>
        public bool glyphsCalculated;

        /// <summary>
        /// Pixel width of the space character
        /// </summary>
        public int spaceWidth;

        /// <summary>
        /// Pixel spacing between characters
        /// </summary>
        public int characterSpacing;

        /// <summary>
        /// Pixel spacing between lines
        /// </summary>
        public int lineSpacing;

        /// <summary>
        /// Sprite sheet to use for this font
        /// </summary>
        public SpriteSheetAsset spriteSheet;

        /// <summary>
        /// If using sprite pack then this lists the glyph sprites to use
        /// </summary>
        public List<PackedSprite> glyphSprites;

        /// <summary>
        /// True if this is a built-in system font
        /// </summary>
        public bool isSystemFont;
    }
}
