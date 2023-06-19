using RetroBlitInternal;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// RetroBlit game framework
/// </summary>
/// <remarks>
/// An all-encompassing class which exposes all of the RetroBlit features with convenient, and easy to use methods!
///
/// All methods in this class are static, there is no need to pass or store the RetroBlit state in your game, simply use
/// the RetroBlit class from anywhere in your game at any time.
/// </remarks>
public sealed class RB
{
    /// <summary>
    /// Invalid sprite
    /// </summary>
    /// <remarks>
    /// Indciates an invalid sprite. This value could be returned if a method returning a sprite fails.
    /// <seedoc>Features:Sprites</seedoc>
    /// </remarks>
    /// <code>
    /// bool IsTerrain(Vector2i pos)
    /// {
    ///     int i = RB.MapSpriteGet(0, pos);
    ///
    ///     if (i != RB.SPRITE_INVALID) {
    ///         if (i > TERRAIN_INDEX_START && i < TERRAIN_INDEX_END) {
    ///             return true;
    ///         }
    ///     }
    ///
    ///     return false;
    /// }
    /// </code>
    public const int SPRITE_INVALID = 0x7FFFFFFF;

    /// <summary>
    /// An empty sprite index, use this to clear a tile in a tilemap
    /// </summary>
    /// <remarks>
    /// Indicates an empty sprite index. This value is most useful when clearing a tile in a tilemap using <see cref="RB.MapSpriteSet"/>.
    /// <seedoc>Features:Sprites</seedoc>
    /// <seedoc>Features:Setting or Getting Tile Info</seedoc>
    /// </remarks>
    /// <code>
    /// void ClearTile(Vector2i pos)
    /// {
    ///     RB.MapSpriteSet(0, pos, RB.SPRITE_EMPTY);
    /// }
    /// </code>
    /// <seealso cref="RB.MapSpriteSet"/>
    public const int SPRITE_EMPTY = 0x7FFFFFFE;

    /// <summary>
    /// Flip sprite horizontally
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be flipped horizontally when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This flag can be combined with <see cref="RB.FLIP_V"/> and/or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.FLIP_H);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int FLIP_H = 1 << 0;

    /// <summary>
    /// Flip sprite vertically
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be flipped vertically when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This flag can be combined with <see cref="RB.FLIP_H"/> and/or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.FLIP_V);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int FLIP_V = 1 << 1;

    /// <summary>
    /// Rotate sprite 90 degrees clockwise
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 90 degrees clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This flag can be combined with <see cref="RB.FLIP_H"/> and/or <see cref="RB.FLIP_V"/>.
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_90_CW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int ROT_90_CW = 1 << 2;

    /// <summary>
    /// Rotate sprite 90 degrees counter-clockwise.
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 90 degrees counter-clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This is equivalent to <see cref="RB.ROT_90_CW"/> | <see cref="RB.FLIP_H"/> | <see cref="RB.FLIP_V"/>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_90_CCW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int ROT_90_CCW = ROT_90_CW | FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 180 degrees counter-clockwise.
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 180 degrees counter-clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This is equivalent to <see cref="RB.FLIP_H"/> | <see cref="RB.FLIP_V"/>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_180_CCW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int ROT_180_CCW = FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 180 degrees clockwise.
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 180 degrees clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This is equivalent to <see cref="RB.FLIP_H"/> | <see cref="RB.FLIP_V"/>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_180_CCW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int ROT_180_CW = FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 270 degrees clockwise.
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 270 degrees clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This is equivalent to <see cref="RB.ROT_90_CW"/> | <see cref="RB.FLIP_H"/> | <see cref="RB.FLIP_V"/>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_270_CW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CCW"/>
    public const int ROT_270_CW = ROT_90_CW | FLIP_H | FLIP_V;

    /// <summary>
    /// Rotate sprite 270 degrees counter-clockwise.
    /// </summary>
    /// <remarks>
    /// Indicates that a sprite should be rotated 270 degrees counter-clockwise when rendered with <see cref="RB.DrawSprite"/> or <see cref="RB.DrawCopy"/>.
    ///
    /// This is equivalent to <see cref="RB.ROT_90_CW"/>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <code>
    /// void Render(Vector2i pos)
    /// {
    ///     RB.DrawSprite("hero_idle", pos, RB.ROT_270_CCW);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.FLIP_H"/>
    /// <seealso cref="RB.FLIP_V"/>
    /// <seealso cref="RB.ROT_90_CW"/>
    /// <seealso cref="RB.ROT_90_CCW"/>
    /// <seealso cref="RB.ROT_180_CW"/>
    /// <seealso cref="RB.ROT_180_CCW"/>
    /// <seealso cref="RB.ROT_270_CW"/>
    public const int ROT_270_CCW = ROT_90_CW;

    /// <summary>
    /// Align to the left edge
    /// </summary>
    /// <remarks>
    /// Align text to the left when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text aligned to the bottom-left corner of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_LEFT | RB.ALIGN_V_BOTTOM,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_H_LEFT = 1 << 9;

    /// <summary>
    /// Align to the right edge
    /// </summary>
    /// <remarks>
    /// Align text to the right when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text aligned to the bottom-right corner of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_RIGHT | RB.ALIGN_V_BOTTOM,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_H_RIGHT = 1 << 10;

    /// <summary>
    /// Center horizontally
    /// </summary>
    /// <remarks>
    /// Center text horizontal when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text horizontally centered on the bottom of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_CENTER | RB.ALIGN_V_BOTTOM,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_H_CENTER = 1 << 11;

    /// <summary>
    /// Align to the top edge
    /// </summary>
    /// <remarks>
    /// Align text to the top when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text aligned to the top-left corner of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_LEFT | RB.ALIGN_V_TOP,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_V_TOP = 1 << 12;

    /// <summary>
    /// Align to the bottom edge
    /// </summary>
    /// <remarks>
    /// Align text to the bottom when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text aligned to the bottom-left corner of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_LEFT | RB.ALIGN_V_BOTTOM,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_V_BOTTOM = 1 << 13;

    /// <summary>
    /// Center vertically
    /// </summary>
    /// <remarks>
    /// Center text vertically when printing with <see cref="RB.Print"/>, or measuring with <see cref="RB.PrintMeasure"/>.
    ///
    /// This flag can be logically *OR*'ed with other alignment flags.
    /// <seedoc>Features:Text Alignment</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text vertically centered on the left side of the screen
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize),
    ///         Color.white,
    ///         RB.ALIGN_H_LEFT | RB.ALIGN_V_CENTER,
    ///         "Hello there!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    public const int ALIGN_V_CENTER = 1 << 14;

    /// <summary>
    /// Clip text to given rectangular area
    /// </summary>
    /// <remarks>
    /// Clip text to given rectangular area.
    ///
    /// Text printed within a rectangular area using the <see cref="RB.Print"/> method, or measured using the <see cref="RB.PrintMeasure"/>
    /// method could potentially overflow the given area. When an overflow happens this flag indicates that the text should be clipped.
    ///
    /// This flag can be *OR*'ed with <see cref="RB.TEXT_OVERFLOW_WRAP"/>.
    /// <seedoc>Features:Text Clipping</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text in a rectangular area, clip off any letters that don't fit.
    ///     RB.Print(
    ///         new Rect2i(100, 100, 32, 48),
    ///         Color.white,
    ///         RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER | RB.TEXT_OVERFLOW_CLIP,
    ///         "This long text might need to be clipped!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    /// <seealso cref="RB.TEXT_OVERFLOW_WRAP"/>
    public const int TEXT_OVERFLOW_CLIP = 1 << 16;

    /// <summary>
    /// Wrap text to the next line when it overflows
    /// </summary>
    /// <remarks>
    /// Wrap text to the next line so that it doesn't overflow the given text area horizontally.
    ///
    /// Text printed within a rectangular area using the <see cref="RB.Print"/> method, or measured using the <see cref="RB.PrintMeasure"/>
    /// method could potentially overflow the given area. When an overflow happens this flag indicates that the text should be wrapped to
    /// the next line. Wrapping happens on whole word boundaries, if possible.
    ///
    /// This flag can be *OR*'ed with <see cref="RB.TEXT_OVERFLOW_CLIP"/>.
    /// <seedoc>Features:Text Clipping</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw text in a rectangular area, clip off any letters that don't fit.
    ///     RB.Print(
    ///         new Rect2i(100, 100, 32, 48),
    ///         Color.white,
    ///         RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER | RB.TEXT_OVERFLOW_WRAP,
    ///         "This long text might need to be wrapped!");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="RB.PrintMeasure"/>
    /// <seealso cref="RB.TEXT_OVERFLOW_CLIP"/>
    public const int TEXT_OVERFLOW_WRAP = 1 << 17;

    /// <summary>
    /// Ignore any inline color changes when printing text
    /// </summary>
    /// <remarks>
    /// Indicates that text inline color changes should be ignored when printing with <see cref="RB.Print"/>.
    ///
    /// Normally text color can be changed inline by using color escape sequences such as "#FF0000Red#00FF00Green". This flag indicates
    /// that the color escape sequences should be ignored. This can be useful in some situations such as when printing a drop shadow for text.
    /// <seedoc>Features:Inline String Coloring</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     var text = "You are @FF0000DEAD";
    ///
    ///     // Print drop shadow first
    ///     RB.Print(new Vector2i(101, 101), Color.black, RB.NO_INLINE_COLOR, text);
    ///
    ///     // Print foreground text
    ///     RB.Print(new Vector2i(100, 100), Color.white, text);
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    public const int NO_INLINE_COLOR = 1 << 18;

    /// <summary>
    /// Ignore all RetroBlit string escape codes
    /// </summary>
    /// <remarks>
    /// Indicates that all RetroBlit escape codes should be ignore and the string should be treated as a raw string
    /// <seedoc>Features:Inline String Coloring</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // This will bring the literal string "You are @FF0000DEAD" without
    ///     // processing the red inline color escape code
    ///     RB.Print(new Vector2i(101, 101), Color.white, RB.NO_ESCAPE_CODES, "You are @FF0000DEAD");
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    public const int NO_ESCAPE_CODES = 1 << 19;

    /// <summary>
    /// Pixel buffer is unchanged
    /// </summary>
    /// <remarks>
    /// Indicates that the pixel buffer contents and dimensions used in <see cref="RB.DrawPixelBuffer"/> are unchanged
    /// since the last call to that method. When this flag is set <see cref="RB.DrawPixelBuffer"/> is optimized by not
    /// uploading pixel data to the GPU again.
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Do something with the pixel buffer
    ///     updatePixelBuffer();
    ///
    ///     // Draw the pixel buffer
    ///     RB.DrawPixelBuffer(pixelBuffer, new Vector2i(100, 200), new Vector2i(0, 0));
    ///
    ///     // Draw the pixel buffer again at a new location with the pixel data unchanged
    ///     RB.DrawPixelBuffer(pixelBuffer, new Vector2i(100, 200), new Vector2i(0, 200), RB.PIXEL_BUFFER_UNCHANGED);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawPixelBuffer"/>
    public const int PIXEL_BUFFER_UNCHANGED = 1 << 20;

    /// <summary>
    /// Up D-pad button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Up* D-pad button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.W* for player one, or *KeyCode.UpArrow* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_UP, RB.PLAYER_ONE)) {
    ///         pos.y--;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_UP = 1 << 0;

    /// <summary>
    /// Down D-pad button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Down* D-pad button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.S* for player one, or *KeyCode.DownArrow* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_DOWN, RB.PLAYER_ONE)) {
    ///         pos.y++;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_DOWN = 1 << 1;

    /// <summary>
    /// Left D-pad button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Left* D-pad button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.A* for player one, or *KeyCode.LeftArrow* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_LEFT, RB.PLAYER_ONE)) {
    ///         pos.x--;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_LEFT = 1 << 2;

    /// <summary>
    /// Right D-pad button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Right* D-pad button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.D* for player one, or *KeyCode.RightArrow* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_RIGHT, RB.PLAYER_ONE)) {
    ///         pos.x++;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_RIGHT = 1 << 3;

    /// <summary>
    /// A button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *A* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.B*, *KeyCode.Space* for player one, or *KeyCode.Semicolon*,
    /// *KeyCode.Keypad1*, *KeyCode.RightControl* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_A, RB.PLAYER_ONE)) {
    ///         Jump();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_A = 1 << 4;

    /// <summary>
    /// B button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *B* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.N*, or *KeyCode.Quote*, *KeyCode.Keypad2* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_B, RB.PLAYER_ONE)) {
    ///         Shoot();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_B = 1 << 5;

    /// <summary>
    /// X button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *X* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.G* for player one, or *KeyCode.P*, *KeyCode.Keypad4* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_X, RB.PLAYER_ONE)) {
    ///         BlockAttack();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_X = 1 << 6;

    /// <summary>
    /// Y button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Y* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.H* for player one, or *KeyCode.LeftBracket*, *KeyCode.Keypad5* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_Y, RB.PLAYER_ONE)) {
    ///         Dash();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_Y = 1 << 7;

    /// <summary>
    /// Left shoulder button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Left Shoulder* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.T* for player one, or *KeyCode.Alpha0*, *KeyCode.Keypad7* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_LS, RB.PLAYER_ONE)) {
    ///         PreviousItem();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_LS = 1 << 8;

    /// <summary>
    /// Right shoulder button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Right Shoulder* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.Y* for player one, or *KeyCode.Minus*, *KeyCode.Keypad8* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_RS, RB.PLAYER_ONE)) {
    ///         NextItem();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_RS = 1 << 9;

    /// <summary>
    /// Menu button on gamepad.
    /// </summary>
    /// <remarks>
    /// The *Menu* button on a gamepad.
    ///
    /// This button is also mapped to *KeyCode.Alpha5* for player one, or *KeyCode.Backspace*, *KeyCode.KeypadDivide* for player two.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_MENU, RB.PLAYER_ONE)) {
    ///         ShowMenu();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_MENU = 1 << 10;

    /// <summary>
    /// System button on any gamepad
    /// </summary>
    /// <remarks>
    /// The *System* button on any gamepad.
    ///
    /// This button is also mapped to *KeyCode.Escape*.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_SYSTEM)) {
    ///         PauseGame();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_SYSTEM = 1 << 11;

    /// <summary>
    /// Left mouse button, or first finger touch.
    /// </summary>
    /// <remarks>
    /// Left mouse button, or first finger touch.
    ///
    /// This button is also mapped to *KeyCode.Mouse0*.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_POINTER_A)) {
    ///         Select();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_POINTER_A = 1 << 12;

    /// <summary>
    /// Right mouse button.
    /// </summary>
    /// <remarks>
    /// Right mouse button, or second finger touch.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_POINTER_B)) {
    ///         OpenContextMenu();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_POINTER_B = 1 << 13;

    /// <summary>
    /// Middle mouse button, or third finger touch
    /// </summary>
    /// <remarks>
    /// Middle mouse button, or third finger touch.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_POINTER_C)) {
    ///         ScrollLock();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_POINTER_C = 1 << 14;

    /// <summary>
    /// The forth finger touch
    /// </summary>
    /// <remarks>
    /// The forth finger touch.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_POINTER_D)) {
    ///         ScrollLock();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_POINTER_D = 1 << 15;

    /// <summary>
    /// A, B, X, or Y button on gamepad
    /// </summary>
    /// <remarks>
    /// *A*, *B*, *X*, or *Y* button on gamepad.
    ///
    /// Equivalent to <see cref="RB.BTN_A"/> | <see cref="RB.BTN_B"/> | <see cref="RB.BTN_X"/> | <see cref="RB.BTN_Y"/>.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_ABXY, RB.PLAYER_ONE)) {
    ///         AnyButtonPressed();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_ABXY = BTN_A | BTN_B | BTN_X | BTN_Y;

    /// <summary>
    /// Any finger touch, left, right, or middle mouse button.
    /// </summary>
    /// <remarks>
    /// Any finger *touch*, *left*, *right*, or *middle* mouse button.
    ///
    /// Equivalent to <see cref="RB.BTN_POINTER_A"/> | <see cref="RB.BTN_POINTER_B"/> | <see cref="RB.BTN_POINTER_C"/> | <see cref="RB.BTN_POINTER_D"/>.
    ///
    /// Use <see cref="RB.InputOverride"/> to override input mapping.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_POINTER_ABCD, RB.PLAYER_ONE)) {
    ///         Click();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_POINTER_ANY = BTN_POINTER_A | BTN_POINTER_B | BTN_POINTER_C | BTN_POINTER_D;

    /// <summary>
    /// Any shoulder button on gamepad.
    /// </summary>
    /// <remarks>
    /// Any shoulder button on gamepad.
    ///
    /// Equivalent to <see cref="RB.BTN_LS"/> | <see cref="RB.BTN_RS"/>.
    ///
    /// Use <see cref="RB.InputOverride(InputOverrideMethod)"/> to override input mapping.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_SHOULDER, RB.PLAYER_ONE)) {
    ///         Spin();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.InputOverride"/>
    public const int BTN_SHOUDLER = BTN_LS | BTN_RS;

    /// <summary>
    /// Player one input flag.
    /// </summary>
    /// <remarks>
    /// Indicates player one.
    ///
    /// Used with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/> and <see cref="RB.ButtonReleased"/>.
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_A, RB.PLAYER_ONE)) {
    ///         players[0].Jump();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public const int PLAYER_ONE = 1 << 0;

    /// <summary>
    /// Player two input flag.
    /// </summary>
    /// <remarks>
    /// Indicates player two.
    ///
    /// Used with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/> and <see cref="RB.ButtonReleased"/>
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_A, RB.PLAYER_TWO)) {
    ///         players[1].Jump();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public const int PLAYER_TWO = 1 << 1;

    /// <summary>
    /// Player three input flag.
    /// </summary>
    /// <remarks>
    /// Indicates player three.
    ///
    /// Used with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/> and <see cref="RB.ButtonReleased"/>
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_A, RB.PLAYER_THREE)) {
    ///         players[2].Jump();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public const int PLAYER_THREE = 1 << 2;

    /// <summary>
    /// Player four input flag.
    /// </summary>
    /// <remarks>
    /// Indicates player four.
    ///
    /// Used with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/> and <see cref="RB.ButtonReleased"/>
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_A, RB.PLAYER_FOUR)) {
    ///         players[3].Jump();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public const int PLAYER_FOUR = 1 << 3;

    /// <summary>
    /// Any player input flag.
    /// </summary>
    /// <remarks>
    /// Indicates any player.
    ///
    /// Used with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/> and <see cref="RB.ButtonReleased"/>
    ///
    /// Equivalent to <see cref="RB.PLAYER_ONE"/> | <see cref="RB.PLAYER_TWO"/> | <see cref="RB.PLAYER_THREE"/> | <see cref="RB.PLAYER_FOUR"/>
    /// <seedoc>Features:Gamepads</seedoc>
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     if (RB.ButtonPressed(RB.BTN_MENU, RB.PLAYER_ANY)) {
    ///         MenuOpen();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public const int PLAYER_ANY = PLAYER_ONE | PLAYER_TWO | PLAYER_THREE | PLAYER_FOUR;

    //// PRIVATE VARIABLES
    //// ========================
    private static readonly RB mInstance = new RB();

    private static readonly RetroBlitInternal.RBFont.TextWrapString mTextWrapString = new RetroBlitInternal.RBFont.TextWrapString();
    private static readonly RetroBlitInternal.RBFont.TextWrapFastString mTextWrapFastString = new RetroBlitInternal.RBFont.TextWrapFastString();

    private IRetroBlitGame mRetroBlitGame;
    private RetroBlitInternal.RBAPI mRetroBlitAPI;
    private Color32 mSolidWhite = new Color32(255, 255, 255, 255);
    private bool mInitialized = false;
    //// ========================

    /// <summary>
    /// Delegate for overriding input mapping
    /// </summary>
    /// <remarks>
    /// A delegate for overriding input mapping. This delegate can be set using <see cref="RB.InputOverride"/>. The delegate will be called just before every
    /// <see cref="IRetroBlitGame.Update"/>, and every time <see cref="RB.ButtonDown"/> is called.
    ///
    /// The delegate should return *true* if *button* is down, or *false* if it's up.
    ///
    /// The delegate can also ignore any button and defer to default handling by setting *handled* to *false*. If the delegate does handle the given *button*
    /// it must set *handled* to *true*.
    /// <seedoc>Features:Gamepad Input Override</seedoc>
    /// </remarks>
    /// <code>
    /// void Initialize() {
    ///     RB.InputOverride(CustomInputOverride);
    /// }
    ///
    /// void CustomInputOverride(int button, int player, out bool handled)
    /// {
    ///     handled = false;
    ///
    ///     // Map button A for player one to the tab key
    ///     if (player == RB.PLAYER_ONE) {
    ///         if (button == RB.BTN_A) {
    ///             handled = true;
    ///             return Unity.Input.GetKeyDown(KeyCode.Tab);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <param name="button">The button being queried</param>
    /// <param name="player">The player for which the button is being queried</param>
    /// <param name="handled">Set to true if the button override was handled, or false if default RetroBlit mapping should be used</param>
    /// <returns>Return true if the button is currently held down, or false if it is up</returns>
    /// <seealso cref="RB.InputOverride"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public delegate bool InputOverrideMethod(int button, int player, out bool handled);

    /// <summary>
    /// Post-processing effect type
    /// </summary>
    /// <remarks>
    /// Post-processing effects are applied with <see cref="RB.EffectSet"/>, these effects are rendered after <see cref="IRetroBlitGame.Render"/> exits.
    /// Unlike normal rendering, some of the post processing effects render in native window resolution and so can create
    /// sub-pixel details such as <mref refid="RB.Effect.Scanlines">Scanlines</mref>.
    ///
    /// Multiple effects can be layered together. For example you may want to enable the <mref refid="RB.Effect.Scanlines">Scanlines</mref>,
    /// <mref refid="RB.Effect.Noise">Noise</mref>, and <mref refid="RB.Effect.Curvature">Curvature</mref>
    /// effects together to create a nice retro CRT TV effect.
    ///
    /// It is also possible to write your own custom post-processing effects by loading shaders with <see cref="ShaderAsset.Load"/>
    /// and setting them with <see cref="RB.EffectShader"/>.
    /// <seedoc>Features:Postprocessing Effects</seedoc>
    /// <seedoc>Features:Postprocessing Shaders</seedoc>
    /// </remarks>
    public enum Effect
    {
        /// <summary>
        /// Retro CRT scanline effect
        /// </summary>
        /// <remarks>
        /// A retro CRT TV scanline effect. This effect has controllable intensity from *0.0f* (off) to *1.0f* (full).
        /// <img src="effect_scanlines.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Enable scanlines effect with intensity of 0.25
        ///     RB.EffectSet(RB.Effect.Scanlines, 0.25f);
        /// }
        /// </code>
        Scanlines,

        /// <summary>
        /// Retro CRT screen noise
        /// </summary>
        /// <remarks>
        /// A retro TV noise effect. This effect has controllable intensity from *0.0f* (off) to *1.0f* (full).
        /// <img src="effect_noise.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Enable noise effect with intensity of 0.25
        ///     RB.EffectSet(RB.Effect.Noise, 0.25f);
        /// }
        /// </code>
        Noise,

        /// <summary>
        /// Saturate colors
        /// </summary>
        /// <remarks>
        /// A color saturation effect. This effect has controllable intensity from *-1.0f* (fully de-saturated/grayscale), *0.0f* (normal), to *1.0f* (fully saturated).
        /// <img src="effect_desaturation.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Enable aturation effect with intensity of -0.9, almost grayscale
        ///     // with just a hint of color.
        ///     RB.EffectSet(RB.Effect.Saturation, -0.9f);
        /// }
        /// </code>
        Saturation,

        /// <summary>
        /// Retro CRT screen curvature
        /// </summary>
        /// <remarks>
        /// A retro CRT TV screen curvature effect. This effect has controllable intensity from *0.0f* (off) to *1.0f* (extreme curvature).
        /// <img src="effect_curvature.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Enable curvature effect with intensity of 0.5.
        ///     // with just a hint of color.
        ///     RB.EffectSet(RB.Effect.Curvature, 0.5f);
        /// }
        /// </code>
        Curvature,

        /// <summary>
        /// Slide display in or out from given direction
        /// </summary>
        /// <remarks>
        /// Slide display in or out from given direction. This effect is controlled by specifying the slide amount in pixels, negative values slide in opposite direction.
        /// You can animate the effect by changing the slide amount every frame.
        /// <img src="effect_slide.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// int slide = 0;
        ///
        /// public void Update()
        /// {
        ///     RB.EffectSet(RB.Effect.Slide, new Vector2i(slide, 0));
        ///
        ///     // Animate the slide by increasing it every frame
        ///     if (slide < RB.DisplaySize.width) {
        ///        slide++;
        ///     }
        /// }
        /// </code>
        Slide,

        /// <summary>
        /// Wipe display out from given direction
        /// </summary>
        /// <remarks>
        /// Wipe display out from given direction. This effect is controlled by specifying the slide amount in pixels, negative values slide in opposite direction.
        /// You can animate the effect by changing the wipe amount every frame.
        /// <img src="effect_wipe.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// int wipe = 0;
        ///
        /// public void Update()
        /// {
        ///     RB.EffectSet(RB.Effect.Wipe, new Vector2i(wipe, 0));
        ///
        ///     // Animate the wipe by increasing it every frame
        ///     if (wipe < RB.DisplaySize.width) {
        ///        wipe++;
        ///     }
        /// }
        /// </code>
        Wipe,

        /// <summary>
        /// Shake the display
        /// </summary>
        /// <remarks>
        /// A full screen shake effect. This effect has controllable intensity from *0.0f* (off) to *1.0f* (extreme shaking). The shaking
        /// will continue until the effect value is changed.
        /// <img src="effect_shake.gif"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Update()
        /// {
        ///     // Set shake effect to 0.15
        ///     RB.EffectSet(RB.Effect.Shake, 0.15f);
        /// }
        /// </code>
        Shake,

        /// <summary>
        /// Zoom display in or out
        /// </summary>
        /// <remarks>
        /// Zoom in and out effect. The zoom amount can be controlled from *0.0f* (invisible), *1.0f* (normal 100% zoom), > *1.0f* (magnified).
        /// <img src="effect_zoom.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Set zoom to 200%
        ///     RB.EffectSet(RB.Effect.Zoom, 2.0f);
        /// }
        /// </code>
        Zoom,

        /// <summary>
        /// Rotate display by given angle
        /// </summary>
        /// <remarks>
        /// Rotates the display. The rotation can be controlled by specifying the angle of clockwise rotation in degrees.
        /// <img src="effect_rotation.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Rotate the display clockwise by 25 degrees.
        ///     RB.EffectSet(RB.Effect.Rotation, 25.0f);
        /// }
        /// </code>
        Rotation,

        /// <summary>
        /// Fade the screen to given color
        /// </summary>
        /// <remarks>
        /// Fades the display to a given color. The color and amount of fade can be controlled.
        /// <img src="effect_colorfade.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Fade the display 50% to white. Note that the Vector2i parameter is ignored
        ///     // for ColorFade.
        ///     RB.EffectSet(RB.Effect.ColorFade, 0.5f, Vector2i.zero, Color.white);
        /// }
        /// </code>
        ColorFade,

        /// <summary>
        /// Apply a tint of given color
        /// </summary>
        /// <remarks>
        /// Tint the display with a given color. The color and the intensity of the tint can be controlled.
        /// <img src="effect_colortint.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Tint the display 100% to green, drowning out other colors entirely. Note that the Vector2i
        ///     // parameter is ignored for ColorTint.
        ///     RB.EffectSet(RB.Effect.ColorTint, 1.0f, Vector2i.zero, Color.red);
        /// }
        /// </code>
        ColorTint,

        /// <summary>
        /// Turn colors to negative
        /// </summary>
        /// <remarks>
        /// Turns the display colors into negative colors. The intensity of the effect can be set from *0.0f* (normal colors), to *1.0f* (fully negative colors).
        /// <img src="effect_negative.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Turn all colors to negative.
        ///     RB.EffectSet(RB.Effect.Negative, 1.0f);
        /// }
        /// </code>
        Negative,

        /// <summary>
        /// Pixelate effect
        /// </summary>
        /// <remarks>
        /// Applies the pixelate effect, distorting the display by enlarging pixels and reducing resolution.
        /// The intensity of this effect can be set from *0.0f* (no pixelation) to *1.0f* (extreme pixelation).
        /// This effect can be animated by changing the amount of pixelation every frame.
        ///
        /// This effect is also known as the "mosaic" effect.
        /// <img src="effect_pixelate.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Pixelate the display
        ///     RB.EffectSet(RB.Effect.Pixelate, 0.15f);
        /// }
        /// </code>
        Pixelate,

        /// <summary>
        /// Pinhole
        /// </summary>
        /// <remarks>
        /// Applies the pinhole effect to the display. The center, size, and color of the pinhole can be controlled. The size of the
        /// pinhole effect scales from *0.0f* (display fully visible) to *1.0f* (display hidden by pinhole effect).
        ///
        /// The pinhole effect can be animated by changing its parameters every frame.
        /// <img src="effect_pinhole.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Set the pinhole effect to slightly on the left side of the display, 60% closed.
        ///     RB.EffectSet(RB.Effect.Pinhole, 0.6f, new Vector2i(50, 50), Color.black);
        /// }
        /// </code>
        Pinhole,

        /// <summary>
        /// Inverted pinhole
        /// </summary>
        /// <remarks>
        /// Applies the inverted pinhole effect to the display. The center, size, and color of the pinhole can be controlled. The size of the
        /// pinhole effect scales from *0.0f* (display fully visible) to *1.0f* (display hidden by pinhole effect).
        ///
        /// The inverted pinhole effect can be animated by changing its parameters every frame.
        /// <img src="effect_invertedpinhole.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Set the inverted pinhole effect to slightly on the left side of the display, 40% closed.
        ///     RB.EffectSet(RB.Effect.InvertedPinhole, 0.4f, new Vector2i(50, 50), Color.black);
        /// }
        /// </code>
        InvertedPinhole,

        /// <summary>
        /// Fizzle out
        /// </summary>
        /// <remarks>
        /// Applies the fizzle out effect, covering the display with given percentage of randomly placed solid color pixels. The color
        /// and intensity of this effect can be controlled.
        ///
        /// This effect can be animated by changing it's parameters every frame.
        /// <img src="effect_fizzle.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Fizzle out the screen by 50% to dark red pixels.
        ///     // Note that the Vector2i parameter is ignored for Fizzle.
        ///     RB.EffectSet(RB.Effect.Fizzle, 0.50f, Vector2i.zero, new Color(0.4f, 0.0f, 0.0f));
        /// }
        /// </code>
        Fizzle,

        /// <summary>
        /// Chromatic Aberration
        /// </summary>
        /// <remarks>
        /// Applies a chromatic aberration effect. Chromatic aberration was an articaft on older CRT displays that caused the individual color
        /// channels to drift from the center of each pixel, causing red/blue fringes on the edges of pixels. Enabling this effect will produce
        /// a more genuine retro display look.
        ///
        /// This effect allows you to adjust the angle and magnitude of the chromatic aberration.
        /// <img src="effect_chromatic_aberration.png"/>
        /// <seedoc>Features:Postprocessing Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// void Initialize()
        /// {
        ///     // Apply a slight chromatic aberration effect at a 45 degree angle.
        ///     RB.EffectSet(RB.Effect.ChromaticAberration, new Vector2i(50, 50));
        /// }
        /// </code>
        /// </remarks>
        ChromaticAberration,
    }

    /// <summary>
    /// Display pixel style
    /// </summary>
    /// <remarks>
    /// Pixel style used by RetroBlit. In most cases the default <mref refid="RB.PixelStyle.Square">Square</mref> is used, but for some games
    /// more eccentric styles are available as well.
    /// <seedoc>Features:Pixel Style</seedoc>
    /// </remarks>
    public enum PixelStyle
    {
        /// <summary>
        /// Normal square pixels
        /// </summary>
        /// <remarks>
        /// Normal square pixels, this is the default pixel style if one is not specified.
        /// <seedoc>Features:Pixel Style</seedoc>
        /// </remarks>
        /// <code>
        /// public RB.HardwareSettings QueryHardware()
        /// {
        ///     var hw = new RB.HardwareSettings();
        ///
        ///     hw.DisplaySize = new Vector2i(320, 160);
        ///     hw.PixelStyle = RB.PixelStyle.Square;
        ///
        ///     return hw;
        /// }
        /// </code>
        Square,

        /// <summary>
        /// Wide pixels
        /// </summary>
        /// <remarks>
        /// Wide pixels, a single wide pixel is equivalent to a 2x1 pixel block.
        /// <seedoc>Features:Pixel Style</seedoc>
        /// </remarks>
        /// <code>
        /// public RB.HardwareSettings QueryHardware()
        /// {
        ///     var hw = new RB.HardwareSettings();
        ///
        ///     hw.DisplaySize = new Vector2i(320, 160);
        ///     hw.PixelStyle = RB.PixelStyle.Wide;
        ///
        ///     return hw;
        /// }
        /// </code>
        Wide,

        /// <summary>
        /// Tall pixels
        /// </summary>
        /// <remarks>
        /// Tall pixels, a single tall pixel is equivalent to a 1x2 pixel block.
        /// <seedoc>Features:Pixel Style</seedoc>
        /// </remarks>
        /// <code>
        /// public RB.HardwareSettings QueryHardware()
        /// {
        ///     var hw = new RB.HardwareSettings();
        ///
        ///     hw.DisplaySize = new Vector2i(320, 160);
        ///     hw.PixelStyle = RB.PixelStyle.Tall;
        ///
        ///     return hw;
        /// }
        /// </code>
        Tall
    }

    /// <summary>
    /// Texture filter to apply when using custom shaders
    /// </summary>
    /// <remarks>
    /// When using custom shaders it can sometimes be useful to change the sprite sheet texture filter.
    /// By default the texture filter is <mref refid="RB.Filter.Nearest">Nearest</mref> which produces nice
    /// crisp pixels. <mref refid="RB.Filter.Linear">Linear</mref> filtering samples neighbouring pixels and
    /// produces a smoothed pixel colors.
    ///
    /// Filter mode is set with the <see cref="RB.ShaderSpriteSheetFilterSet"/> method.
    /// <seedoc>Features:Sprite Sheets in Shaders</seedoc>
    /// </remarks>
    /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
    public enum Filter
    {
        /// <summary>
        /// No smoothing, sample from a single nearest texel only
        /// </summary>
        /// <remarks>
        /// No smoothing, sample from a single nearest texel (texture pixel) only.
        /// <image src="filter_point.png">Source sprite sheet, x50 scale.</image>
        /// <image src="filter_point.png">Nearest filtering, rendered at x50 scale.</image>
        /// <seedoc>Features:Sprite Sheets in Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// public void Render()
        /// {
        ///     RB.ShaderSet(0);
        ///     RB.ShaderSpriteSheetTextureSet(0, "Gradient", 3);
        ///     RB.ShaderSpriteSheetFilterSet(0, 3, RB.Filter.Point);
        /// }
        /// </code>
        /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
        Nearest,

        /// <summary>
        /// Linear interpolation, pixel color averaged between neighbouring texels
        /// </summary>
        /// <remarks>
        /// Linear interpolation, pixel color averaged between neighbouring texels (texture pixels).
        /// <image src="filter_point.png">Source sprite sheet, x50 scale.</image>
        /// <image src="filter_linear.png">Linear filtering, rendered at x50 scale.</image>
        /// <seedoc>Features:Sprite Sheets in Shaders</seedoc>
        /// </remarks>
        /// <code>
        /// public void Render()
        /// {
        ///     RB.ShaderSet(0);
        ///     RB.ShaderSpriteSheetTextureSet(0, "Gradient", 3);
        ///     RB.ShaderSpriteSheetFilterSet(0, 3, RB.Filter.Linear);
        /// }
        /// </code>
        /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
        Linear
    }

    /// <summary>
    /// Asset loading status
    /// </summary>
    /// <remarks>
    /// Asset loading status
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <seealso cref="Result"/>
    public enum AssetStatus
    {
        /// <summary>
        /// Invalid state, every asset starts in this state until it is loaded
        /// </summary>
        /// <remarks>
        /// Invalid state, every asset starts in this state until it is loaded. The asset will also return to this state if it is unloaded.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Invalid = 0,

        /// <summary>
        /// Currently loading asynchronously
        /// </summary>
        /// <remarks>
        /// The asset is currently loading asynchronously.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Loading,

        /// <summary>
        /// Loaded and ready to use
        /// </summary>
        /// <remarks>
        /// The asset has finished loading successfully, and it is ready to use.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Ready,

        /// <summary>
        /// Asset failed to load
        /// </summary>
        /// <remarks>
        /// The asset has failed to load, and it is not useable. See <see cref="RBAsset.error"/> for more error information.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Failed
    }

    /// <summary>
    /// Asset loading source type
    /// </summary>
    /// <remarks>
    /// Asset loading source type
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    public enum AssetSource
    {
        /// <summary>
        /// Asset loaded from Resources folder synchronously
        /// </summary>
        /// <remarks>
        /// Asset loaded from Resources folder somewhere in the Unity project. Loading is synchronous and will block the main thread until the asset is loaded.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Resources = 0,

        /// <summary>
        /// Asset loaded from Resources folder asynchronously
        /// </summary>
        /// <remarks>
        /// Asset loaded from Resources folder somewhere in the Unity project. Loading is asynchronous.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        ResourcesAsync,

        /// <summary>
        /// Asset loaded from Addressable Assets asynchronously
        /// </summary>
        /// <remarks>
        /// Asset loaded from Addressable Assets. To load from Addressable Assets you need to install the Addressable Asset package,
        /// and add <b>ADDRESSABLES_PACKAGE_AVAILABLE</b> in your <b>Build Settings -> Project Settings -> Scripting Define Symbols</b>.
        /// Addressable Assets are always loaded asynchronously.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        AddressableAssets,

        /// <summary>
        /// Asset loaded from web address asynchronously
        /// </summary>
        /// <remarks>
        /// Asset loaded from web address asynchronously. Note that ShaderAssets cannot be loaded from the web as they need to be compiled specifically for the client platform.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        WWW
    }

    /// <summary>
    /// Asset loading result
    /// </summary>
    /// <remarks>
    /// Asset loading result
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <seealso cref="AssetStatus"/>
    public enum Result
    {
        /// <summary>
        /// Successful
        /// </summary>
        /// <remarks>
        /// Asset loaded successfully.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Success = 0,

        /// <summary>
        /// Pending, loading asynchronously
        /// </summary>
        /// <remarks>
        /// Asset is currently loading asynchronously.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Pending,

        /// <summary>
        /// Asset was not found
        /// </summary>
        /// <remarks>
        /// Asset was not found.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        NotFound,

        /// <summary>
        /// Not enough resources available to load the asset
        /// </summary>
        /// <remarks>
        /// Not enough resources available to load the asset.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        NoResources,

        /// <summary>
        /// The asset is not supported
        /// </summary>
        /// <remarks>
        /// The asset is not supported.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        NotSupported,

        /// <summary>
        /// A network error occurred while loading the asset
        /// </summary>
        /// <remarks>
        /// A network error occurred while loading the asset. This is only applicable to <b>WWW</b> asset sources.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        NetworkError,

        /// <summary>
        /// A server error (HTTP error 505) occurred while loading the asset
        /// </summary>
        /// <remarks>
        /// A server error (HTTP error 505) occurred while loading the asset. This is only applicable to <b>WWW</b> asset sources.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        ServerError,

        /// <summary>
        /// A permissions error (HTTP error 403) occurred while loading the asset
        /// </summary>
        /// <remarks>
        /// A permissions error (HTTP error 403) occurred while loading the asset. This is only applicable to <b>WWW</b> asset sources.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        NoPermission,

        /// <summary>
        /// A parameter error (HTTP error 400) occurred while loading the asset
        /// </summary>
        /// <remarks>
        /// A parameter error (HTTP error 400) occurred while loading the asset. This is only applicable to <b>WWW</b> asset sources.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        BadParam,

        /// <summary>
        /// Asset data is in an invalid or in an unrecognized format
        /// </summary>
        /// <remarks>
        /// Asset data is in an invalid or in an unrecognized format.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        BadFormat,

        /// <summary>
        /// An unknown error occurred
        /// </summary>
        /// <remarks>
        /// An unknown error occurred. This can happen if an error occurred by the underlying Unity or system APIs did not return further information.
        /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
        /// </remarks>
        Undefined,
    }

    /// <summary>
    /// Defines IRetroBlitGame interface. Every RetroBlit game must implement this interface.
    /// </summary>
    /// <remarks>
    /// Defines IRetroBlitGame interface. Every RetroBlit game must implement this interface.
    /// <seedoc>Features:The Game Loop</seedoc>
    /// </remarks>
    public interface IRetroBlitGame
    {
        /// <summary>
        /// Called once on startup to query the game for its hardware capabilities. This call happens before any other call to this interface.
        /// </summary>
        /// <remarks>
        /// Called once on startup to query the game for its hardware capabilities. This call happens before any other call to this interface.
        /// The RetroBlit game must filled out the <see cref="HardwareSettings"/> class and return it.
        /// <seedoc>Features:The Game Loop</seedoc>
        /// </remarks>
        /// <returns>Hardware capabilities</returns>
        /// <seealso cref="RB.HardwareSettings"/>
        HardwareSettings QueryHardware();

        /// <summary>
        /// Called once after QueryHardware. The game should initialize it's state here.
        /// </summary>
        /// <remarks>
        /// Called once after QueryHardware. The game should initialize it's state here. This could involve loading resources like sprite sheets and
        /// sounds, and setting up the initial title screen.
        ///
        /// If initialization fails *false* should be returned to tell RetroBlit to terminate the game.
        /// <seedoc>Features:The Game Loop</seedoc>
        /// </remarks>
        /// <returns>Return true if initialization was successful, false otherwise</returns>
        bool Initialize();

        /// <summary>
        /// Called at a fixed interval set in <see cref="IRetroBlitGame.QueryHardware"/> (60 fps by default). All game logic should go in here
        /// </summary>
        /// <remarks>
        /// Called at a fixed interval set in <see cref="IRetroBlitGame.QueryHardware"/> (60 fps by default). All game logic should go in here.
        /// <seedoc>Features:The Game Loop</seedoc>
        /// </remarks>
        /// <seealso cref="RB.HardwareSettings.FPS"/>
        void Update();

        /// <summary>
        /// Called to render to display, all draw APIs should be called here
        /// </summary>
        /// <remarks>
        /// Called to render to display, all draw APIs should be called here. Unlike <see cref="IRetroBlitGame.Update"/> this method is not called at
        /// a fixed framerate.
        /// <seedoc>Features:The Game Loop</seedoc>
        /// </remarks>
        void Render();
    }

    /// <summary>
    /// Reference to an instance of <see cref="IRetroBlitGame"/> game running under RetroBlit.
    /// </summary>
    /// <remarks>
    /// A reference to the current <see cref="IRetroBlitGame"/> game instance running under RetroBlit. This is the same instance
    /// that is passed to RetroBlit by the call into the <see cref="RB.Initialize"/> method.
    ///
    /// This is a convenience method which allows the caller to get a reference to their game from anywhere.
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     var game = (MyGame)RB.Game;
    ///
    ///     if (!game.player.dead) {
    ///         game.player.score++;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.Initialize"/>
    /// <seealso cref="IRetroBlitGame"/>
    public static IRetroBlitGame Game
    {
        get { return mInstance.mRetroBlitGame; }
    }

    /// <summary>
    /// Display size as given in <see cref="IRetroBlitGame.QueryHardware"/>.
    /// </summary>
    /// <remarks>
    /// The current display size in pixels. This is a *read-only* property, to change the display size us the <see cref="RB.DisplayModeSet"/> method.
    /// The initial display size is determined by a call into <see cref="IRetroBlitGame.QueryHardware"/>.
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Draw a rectangle the size of the display
    ///     RB.DrawRect(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), Color.green);
    /// }
    /// </code>
    /// <seealso cref="RB.DisplayModeSet"/>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static Vector2i DisplaySize
    {
        get { return mInstance.mRetroBlitAPI.HW.DisplaySize; }
    }

    /// <summary>
    /// The display surface, which can then be rendered in a custom fashion.
    /// </summary>
    /// <remarks>
    /// The RetroBlit display surface as a *Texture*. This texture can be used just like any other texture, and incorporated into any Unity Scene. For an example see the *Old Days* demo.
    /// This property is only useful if you want to handle your own rendering of the RetroBlit display surface in a custom way, or if you want to get at the pixel data.
    /// <seedoc>Features:Taking Over the Display</seedoc>
    /// </remarks>
    /// <code>
    /// // Unity does not allow for conversion between Texture and Texture2D.
    /// // This utility method will do that for us.
    /// Texture2D TextureToTexture2D(Texture texture)
    /// {
    ///     Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
    ///
    ///     RenderTexture currentRenderTexture = RenderTexture.active;
    ///
    ///     RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
    ///     Graphics.Blit(texture, renderTexture);
    ///
    ///     RenderTexture.active = renderTexture;
    ///     texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
    ///     texture2D.Apply();
    ///
    ///     RenderTexture.active = currentRenderTexture;
    ///
    ///     return texture2D;
    /// }
    ///
    /// // Capture a screenshot and save it to the given filename
    /// void CaptureScreenshot(string filename)
    /// {
    ///     var tex = TextureToTexture2D(RB.DisplaySurface);
    ///     var pngBytes = tex.EncodeToPNG();
    ///
    ///     System.IO.File.WriteAllBytes(filename, pngBytes);
    /// }
    /// </code>
    public static Texture DisplaySurface
    {
        get { return mInstance.mRetroBlitAPI.Renderer.DisplaySurfaceGet(); }
    }

    /// <summary>
    /// Tilemap maximum size as given in <see cref="IRetroBlitGame.QueryHardware"/>.
    /// </summary>
    /// <remarks>
    /// The tilemap maximum size, as given in <see cref="IRetroBlitGame.QueryHardware"/>. The size is specified in tile counts.
    ///
    /// The size represents the maximum tilemap size that can be loaded into memory at one time. Very large, or <i>infinite</i> maps
    /// can be implemented by paging chunks of the map into the tilemap space defined by <see cref="RB.MapSize"/>. See the
    /// *Demo Reel* for an example of an <i>infinite</i> map.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// </remarks>
    /// <code>
    /// // Fill an entire tilemap layer with the given sprite
    /// void FillMap(int layer, string spriteName)
    /// {
    ///     var size = RB.MapSize;
    ///     for (int y = 0; y < size.height; y++) {
    ///         for (int x = 0; x < size.width; x++) {
    ///             RB.MapSpriteSet(layer, new Vector2i(x, y), spriteName);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static Vector2i MapSize
    {
        get { return mInstance.mRetroBlitAPI.HW.MapSize; }
    }

    /// <summary>
    /// Tilemap chunk size as given in <see cref="IRetroBlitGame.QueryHardware"/>
    /// </summary>
    /// <remarks>
    /// The tilemap chunk size, as given in <see cref="IRetroBlitGame.QueryHardware"/>. The size is specified in tile counts.
    ///
    /// A tilemap is internally divided up into chunks. When picking a non-default chunk size in <see cref="IRetroBlitGame.QueryHardware"/> there are several things to consider:
    /// <list type="bullet">
    /// <item>Each chunk has some drawing overhead, the more chunks visible on screen the slower the tilemap will render.</item>
    /// <item>Large chunks have greater chance to be only partially visible on screen and cause some unnecessary overdraw. This will eventually be optimized by the GPU, but some performance loss will happen.</item>
    /// <item>Whenever one or more tiles are updated in a chunk the entire chunk must be recreated and uploaded to the GPU. This is a batched operation, the chunk is not uploaded until it is next rendered.</item>
    /// </list>
    /// With these points in mind the general rule of thumb is that the larger your game resolution the larger the tilemap chunk size should be.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// </remarks>
    /// <code>
    /// // Fill an entire tilemap layer with the given sprite
    /// int GetTotalMapChunks()
    /// {
    ///     var mapSize = RB.MapSize;
    ///     var chunkSize = RB.MapChunkSize;
    ///
    ///     return (mapSize.width / chunkSize.width) * (mapSize.height / chunkSize.height);
    /// }
    /// </code>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static Vector2i MapChunkSize
    {
        get { return mInstance.mRetroBlitAPI.HW.MapChunkSize; }
    }

    /// <summary>
    /// Amount of map layers as given in <see cref="IRetroBlitGame.QueryHardware"/>
    /// </summary>
    /// <remarks>
    /// The tilemap layer count, as given in <see cref="IRetroBlitGame.QueryHardware"/>.
    ///
    /// A tilemap can have a fixed amount of layers. Each layer can be drawn and manipulated individually.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// </remarks>
    /// <code>
    /// void DrawAllLayers()
    /// {
    ///     for (int i = 0; i < RB.MapLayers; i++) {
    ///         RB.DrawMapLayer(i);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static int MapLayers
    {
        get { return mInstance.mRetroBlitAPI.HW.MapLayers; }
    }

    /// <summary>
    /// The target FPS as set by <see cref="IRetroBlitGame.QueryHardware"/>.
    /// </summary>
    /// <remarks>
    /// The target Frames Per Second (FPS) as set by <see cref="IRetroBlitGame.QueryHardware"/>. The FPS value
    /// reflects how many times <see cref="IRetroBlitGame.Update"/> will be called per second. Depending on system
    /// load it may not always be possible for RetroBlit to reach this framerate in realtime, it may potentially lag behind
    /// and catch up on subsequent frames.
    ///
    /// <see cref="RB.FPS"/> is equivalent to (1.0f / <see cref="RB.UpdateInterval"/>).
    ///
    /// Internally this property maps to the Unity property *Application.targetFrameRate*.
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     RB.Print(Vector2i.zero, Color.white, "Target FPS: " + RB.FPS);
    /// }
    /// </code>
    /// <seealso cref="RB.UpdateInterval"/>
    /// <seealso cref="IRetroBlitGame.Update"/>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static int FPS
    {
        get { return mInstance.mRetroBlitAPI.HW.FPS; }
    }

    /// <summary>
    /// Update interval which is equivalent to (1.0f / <see cref="RB.FPS"/>).
    /// </summary>
    /// <remarks>
    /// The update interval in seconds between calls to <see cref="IRetroBlitGame.Update"/>. RetroBlit runs a fixed time
    /// update loop, and so this value is does not change at runtime, and it is derived from the initial <see cref="RB.FPS"/>
    /// value set by <see cref="IRetroBlitGame.QueryHardware"/>.
    ///
    /// In variable time step frameworks it is important to scale all the calculations you do in your game logic by the current
    /// time step. This is not important in a fixed time step framework like RetroBlit, but it may still be wise to do so if
    /// you ever plan to change your fixed time step in the future by passing a different FPS value in <see cref="IRetroBlitGame.QueryHardware"/>.
    ///
    /// <see cref="RB.UpdateInterval"/> is equivalent to (1.0f / <see cref="RB.FPS"/>).
    /// </remarks>
    /// <code>
    /// void Update()
    /// {
    ///     pos.x += 10.0f * RB.UpdateInterval;
    /// }
    /// </code>
    /// <seealso cref="RB.FPS"/>
    /// <seealso cref="IRetroBlitGame.Update"/>
    /// <seealso cref="IRetroBlitGame.QueryHardware"/>
    public static float UpdateInterval
    {
        get { return mInstance.mRetroBlitAPI.HW.UpdateInterval; }
    }

    /// <summary>
    /// Represents the count of calls to <see cref="IRetroBlitGame.Update"/> since startup, or since <see cref="RB.TicksReset"/> was called.
    /// </summary>
    /// <remarks>
    /// Represents the count of calls to <see cref="IRetroBlitGame.Update"/> since startup, or since <see cref="RB.TicksReset"/> was called.
    /// </remarks>
    /// <code>
    /// int DrawAnimatedSprite(Vector2i pos, int firstFrame, int totalFrames)
    /// {
    ///     RB.DrawSprite(firstFrame + (int)(RB.Ticks % totalFrames), pos);
    /// }
    /// </code>
    /// <seealso cref="IRetroBlitGame.Update"/>
    /// <seealso cref="RB.TicksReset"/>
    public static ulong Ticks
    {
        get { return mInstance.mRetroBlitAPI.Ticks; }
    }

    /// <summary>
    /// Initialize RetroBlit with the given game instance.
    /// </summary>
    /// <remarks>
    /// Initializes a RetroBlit game. The initialization process will set up RetroBlit internal states, and also call <see cref="IRetroBlitGame.QueryHardware"/>,
    /// followed by <see cref="IRetroBlitGame.Initialize"/>.
    ///
    /// You can call this method multiple times to switch between entirely separate RetroBlit games! Each call to <see cref="RB.Initialize"/> will clean up the
    /// existing game and load a new game. You could also reload the same game to start from a clean state.
    ///
    /// As part of initialization RetroBlit will call <see cref="IRetroBlitGame.QueryHardware"/>,
    /// followed by <see cref="IRetroBlitGame.Initialize"/>.
    /// <seedoc>Features:A Game From Scratch</seedoc>
    /// </remarks>
    /// <param name="game">A RetroBlit Game</param>
    /// <returns>True if successful</returns>
    /// <example>
    /// <code>
    /// public class MyGameEntry : MonoBehaviour
    /// {
    ///     private void Awake()
    ///     {
    ///         // Initialize your game!
    ///         // You can call RB.Initialize multiple times to switch between RetroBlit games!
    ///         RB.Initialize(new MyGame());
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RB.IRetroBlitGame"/>
    public static bool Initialize(IRetroBlitGame game)
    {
        var rbInternalObj = GameObject.Find("RetroBlitInternal");
        if (rbInternalObj != null)
        {
            mInstance.mRetroBlitAPI = rbInternalObj.GetComponent<RetroBlitInternal.RBAPI>();
        }

        if (mInstance.mRetroBlitAPI == null)
        {
            Debug.LogError("Can't find RetroBlitInternal gameObject, did you add the RetroBlit prefab to your scene?");
            mInstance.mInitialized = false;
            return false;
        }

        mInstance.mRetroBlitGame = game;

        var settings = mInstance.mRetroBlitGame.QueryHardware();

        bool ret = mInstance.mRetroBlitAPI.Initialize(settings);
        mInstance.mInitialized = false;
        if (ret)
        {
            if (mInstance.mRetroBlitGame.Initialize())
            {
                mInstance.mInitialized = true;
            }
            else
            {
                Debug.LogError("Game Initialize() returned failure");
                return false;
            }
        }
        else
        {
            Debug.LogError("RetroBlit API failed to initialize");
            return false;
        }

        // Highly recommended to keep CPU usage low
        QualitySettings.vSyncCount = 1;

#if !UNITY_WEBGL
        Application.targetFrameRate = mInstance.mRetroBlitAPI.HW.FPS;
#endif

        Time.fixedDeltaTime = mInstance.mRetroBlitAPI.HW.UpdateInterval;

        mInstance.mRetroBlitAPI.FinalizeInitialization(mInstance.mInitialized);

        if (mInstance.mInitialized)
        {
            return true;
        }
        else
        {
            Debug.LogError("RetroBlit initialization failed for game " + game.GetType().Name);
            return false;
        }
    }

    /// <summary>
    /// Disable rendering to screen.
    /// </summary>
    /// <remarks>
    /// Disables the final RetroBlit render pass, and does not render to screen. This method is useful if custom rendering is needed.
    /// The display surface can be retrieved with <see cref="RB.DisplaySurface"/> as a *Texture* and then rendered in Unity in any way desired.
    ///
    /// See the *Old Days* demo scene for an example of integration of the RetroBlit display surface into an existing scene.
    /// <seedoc>Features:Taking Over The Display</seedoc>
    /// </remarks>
    /// <code>
    ///     private void Initialize()
    ///     {
    ///         RB.PresentDisable();
    ///     }
    ///
    ///     private void Update()
    ///     {
    ///         // Get the RetroBlit display surface and assign it to a material
    ///         // in a Unity scene
    ///         var tex = RB.DisplaySurface;
    ///         tex.filterMode = FilterMode.Bilinear;
    ///
    ///         myRenderer.materials[2].mainTexture = tex;
    ///         myRenderer.materials[2].color = Color.white;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.PresentEnable"/>
    /// <seealso cref="RB.DisplaySurface"/>
    public static void PresentDisable()
    {
        mInstance.mRetroBlitAPI.PresentCamera.PresentEnabledSet(false);
    }

    /// <summary>
    /// Re-enable rendering to screen
    /// </summary>
    /// <remarks>
    /// Enables the final RetroBlit render pass, and renders to screen. This is the default behaviour of RetroBlit.
    /// </remarks>
    /// <code>
    ///     private void Initialize()
    ///     {
    ///         // Enable rendering to screen. This is the default behaviour.
    ///         RB.PresentEnable();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.PresentDisable"/>
    public static void PresentEnable()
    {
        mInstance.mRetroBlitAPI.PresentCamera.PresentEnabledSet(true);
    }

    /// <summary>
    /// Reset the ticks counter
    /// </summary>
    /// <remarks>
    /// Resets the ticks counter back to 0. The tick counter is increased once per every call to <see cref="RB.IRetroBlitGame.Update"/>.
    /// The current tick counter value can be retrieved with the <see cref="RB.Ticks"/> property.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyDown(KeyCode.Space)) {
    ///         RB.TicksReset();
    ///     }
    /// }
    ///
    /// int DrawAnimatedSprite(Vector2i pos, int firstFrame, int totalFrames)
    /// {
    ///     RB.Print(new Vector2i(4, 4), Color.white, RB.Ticks + " frames have passed.");
    /// }
    /// </code>
    /// <seealso cref="RB.Ticks"/>
    public static void TicksReset()
    {
        mInstance.mRetroBlitAPI.TicksReset();
    }

    /// <summary>
    /// Set display mode to given resolution and pixel style.
    /// </summary>
    /// <remarks>
    /// Sets display mode to given resolution and pixel style. Note that this sets only the RetroBlit pixel resolution, and does not affect the native window size.
    /// To change the native window size you can use the Unity *Screen.SetResolution* method.
    ///
    /// Note that the initial display resolution is always set in <see cref="RB.IRetroBlitGame.QueryHardware"/>.
    ///
    /// This method cannot be called from within <see cref="RB.IRetroBlitGame.Render"/> method.
    /// </remarks>
    /// <param name="resolution">Resolution, dimensions must be multiple of 2.</param>
    /// <param name="pixelStyle">Pixel style, one of <see cref="RB.PixelStyle.Square"/>, <see cref="RB.PixelStyle.Wide"/>, <see cref="RB.PixelStyle.Tall"/></param>
    /// <returns>True if mode was successfully set, false otherwise.</returns>
    /// <code>
    /// // Track previous native screen size
    /// Vector2i previousNativeRes;
    ///
    /// void Update() {
    ///     // Get the current screen resolution from Unity
    ///     var currentNativeRes = new Vector2i(Screen.width, Screen.height);
    ///
    ///     // If the current size is different than previous size then update RetroBlit resolution.
    ///     // This keeps the RetroBlit resolution at half the native resolution, essentially keeping
    ///     // the pixel size at 2x2 native pixel size.
    ///     if (currentNativeRes != previousNativeRes) {
    ///         RB.DisplayModeSet(new Vector2i(Screen.width / 2, Screen.height / 2));
    ///     }
    ///
    ///     previousNativeRes = new Vector2i(Screen.width, Screen.height);
    /// }
    /// </code>
    /// <seealso cref="RB.DisplaySize"/>
    /// <seealso cref="RB.PixelStyle"/>
    /// <seealso cref="RB.IRetroBlitGame.QueryHardware"/>
    public static bool DisplayModeSet(Vector2i resolution, PixelStyle pixelStyle = PixelStyle.Square)
    {
        if (!mInstance.mInitialized)
        {
            Debug.LogError("RetroBlit is not initialized yet, set the initialize resolution via IRetroBlitGame.QueryHardware()");
            return false;
        }

        if (mInstance.mRetroBlitAPI.Renderer.RenderEnabled)
        {
            Debug.LogError("Can't change display mode in Render() call!");
            return false;
        }

        if (resolution.x <= 0 || resolution.y <= 0 ||
            resolution.x >= RetroBlitInternal.RBHardware.HW_MAX_DISPLAY_DIMENSION ||
            resolution.y >= RetroBlitInternal.RBHardware.HW_MAX_DISPLAY_DIMENSION)
        {
            Debug.LogError("Invalid resolution");
            return false;
        }

        if (resolution.x % 2 != 0 || resolution.y % 2 != 0)
        {
            Debug.LogError("Display width and height must both be divisible by 2!");
            return false;
        }

        if ((int)pixelStyle < (int)PixelStyle.Square || (int)pixelStyle > (int)PixelStyle.Tall)
        {
            Debug.LogError("Invalid pixel style");
            return false;
        }

        return mInstance.mRetroBlitAPI.Renderer.DisplayModeSet(resolution, pixelStyle);
    }

    /// <summary>
    /// Get a count of current asynchronously loading assets
    /// </summary>
    /// <remarks>
    /// Get a count of current asynchronously loading assets. This can be used to determine the loading progress of asynchronous assets.
    /// </remarks>
    /// <returns>Count of loading assets</returns>
    public static int LoadingAssetsCount()
    {
        return mInstance.mRetroBlitAPI.AssetManager.LoadingAssetsCount();
    }

    /// <summary>
    /// Clear the display
    /// </summary>
    /// <remarks>
    /// Clears the display to the given *color*. Normally the entire screen is cleared, but a clear *rect* can also be passed to only
    /// clear a portion of the screen.
    ///
    /// Note that clearing the screen or *rect* is not the same as drawing a solid color rect with
    /// <see cref="RB.DrawRectFill"/>. Drawing is an additive process, clearing sets pixels to the given color regardless of what the
    /// previous pixel color was. This distinction is most apparent when rendering to a sprite sheet by using <see cref="RB.Offscreen"/>.
    /// When rendering to a sprite sheet it may be desirable to sometimes erase a part of the sprite sheet by setting the pixels
    /// to be transparent (alpha = 0), the <see cref="RB.Clear"/> method is the only way to do that. See the *Demo Reel* example scene
    /// for clearing and drawing into a sprite sheet.
    /// <seedoc>Features:Drawing into a Sprite Sheet</seedoc>
    /// </remarks>
    /// <param name="color">Color to clear with</param>
    /// <code>
    /// void Render() {
    ///     // Clear the entire screen to black
    ///     RB.Clear(Color.black);
    ///
    ///     RB.Print(new Vector2i(32, 64), Color.white, "Ah, it's nice and clean here!");
    /// }
    /// </code>
    /// <seealso cref="RB.Offscreen"/>
    public static void Clear(Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.Clear(color);
    }

    /// <summary>
    /// Clear a region of the current target.
    /// </summary>
    /// <remarks>
    /// Useful for clearing sprite sheet areas to alpha 0.
    /// </remarks>
    /// <param name="color">Color to clear with</param>
    /// <param name="rect">Region to clear</param>
    public static void Clear(Color32 color, Rect2i rect)
    {
        mInstance.mRetroBlitAPI.Renderer.ClearRect(color, rect);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row.
    /// </summary>
    /// <remarks>
    /// A simple helper function for calculating sprite index given the sprite sheet *column* and *row*. A sprite index is
    /// used when rendering sprites using the <see cref="RB.DrawSprite"/> method.
    ///
    /// The sprite index is calculated using the <see cref="RB.SpriteSize"/> of the current sprite sheet, or the sprite sheet
    /// given by *spriteSheetIndex*.
    ///
    /// <see cref="RB.SpriteIndex"/> does not perform error checking, invalid *column* or *row* will produce an invalid sprite index.
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <param name="column">Column</param>
    /// <param name="row">Row</param>
    /// <returns>Sprite index</returns>
    /// <code>
    /// void Render() {
    ///    // Draw an animated sprite using frames in column 0-7, and row 5
    ///    RB.DrawSprite(RB.SpriteIndex((int)(RB.Ticks % 8), 5), pos);
    /// }
    /// </code>
    /// <seealso cref="RB.SpriteSize"/>
    /// <seealso cref="RB.DrawSprite"/>
    public static int SpriteIndex(int column, int row)
    {
        if (mInstance.mRetroBlitAPI.Renderer.CurrentSpriteSheet == null)
        {
            return 0;
        }

        return column + (row * mInstance.mRetroBlitAPI.Renderer.CurrentSpriteSheet.internalState.columns);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row.
    /// </summary>
    /// <remarks>There is no bounds checking performed, invalid column/row will produce invalid sprite index</remarks>
    /// <param name="spriteSheet">Spritesheet for which to get sprite index for</param>
    /// <param name="column">Column</param>
    /// <param name="row">Row</param>
    /// <returns>Sprite index</returns>
    public static int SpriteIndex(SpriteSheetAsset spriteSheet, int column, int row)
    {
        if (spriteSheet == null)
        {
            return 0;
        }

        return column + (row * spriteSheet.internalState.columns);
    }

    /// <summary>
    /// Helper function for calculating sprite index given sprite sheet column and row
    /// </summary>
    /// <remarks>There is no bounds checking performed, invalid column/row will produce invalid sprite index</remarks>
    /// <param name="spriteSheet">Spritesheet for which to get sprite index for</param>
    /// <param name="cell">Cell in sprite sheet where <see cref="Vector2i.x"/> is the column and <see cref="Vector2i.y"/> is the row.</param>
    /// <returns>Sprite index</returns>
    public static int SpriteIndex(SpriteSheetAsset spriteSheet, Vector2i cell)
    {
        if (spriteSheet == null)
        {
            return 0;
        }

        return cell.x + (cell.y * spriteSheet.internalState.columns);
    }

    /// <summary>
    /// Helper function for getting a <see cref="PackedSpriteID"/>
    /// </summary>
    /// <remarks>
    /// A helper function for getting a <see cref="PackedSpriteID"/> from a string. These ids can then be used for drawing sprites from
    /// sprite packs with <see cref="RB.DrawSprite"/>. <see cref="PackedSpriteID"/> is more efficient to use than a *string* or even a *FastString*.
    ///
    /// <see cref="PackedSpriteID"/> is independent of any particular sprite pack. If multiple sprite packs contain sprites with the same names the same
    /// <see cref="PackedSpriteID"/> can be reused.
    ///
    /// <see cref="RB.PackedSpriteID"/> is stable across sessions, and all devices. The same *string* or *FastString* will always result in the same
    /// <see cref="PackedSpriteID"/>. Therefore, it is safe to persist and reuse these ids if needed.
    ///
    /// See *Demo Reel* for example usage of sprite packs.
    ///
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <param name="name">Packed sprite name, which corresponds to the filename (without extension) of the source sprite before it was packed into the sprite pack.</param>
    /// <returns>Packed sprite id</returns>
    /// <code>
    /// PackedSpriteID heroSprite;
    ///
    /// void Initialize() {
    ///     heroSprite = PackedSpriteID("characters/hero_idle");
    /// }
    ///
    /// void Render() {
    ///    RB.DrawSprite(heroSprite, pos);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="SpriteSheetAsset"/>
    /// <seealso cref="PackedSpriteID"/>
    public static PackedSpriteID PackedSpriteID(string name)
    {
        return new PackedSpriteID(RetroBlitInternal.RBUtil.StableStringHash(name));
    }

    /// <summary>
    /// Helper function for getting a packed sprite id.
    /// </summary>
    /// <remarks>
    /// Packed sprite id is more efficient to use than a string name
    /// </remarks>
    /// <param name="name">Packed sprite name, which corresponds to the filename (without extension) of the source sprite before it was packed into the sprite pack.</param>
    /// <returns>Sprite index</returns>
    public static PackedSpriteID PackedSpriteID(FastString name)
    {
        return new PackedSpriteID(RetroBlitInternal.RBUtil.StableStringHash(name));
    }

    /// <summary>
    /// Get packed sprite info for the given sprite
    /// </summary>
    /// <remarks>
    /// Gets detailed sprite info for the given sprite in the current sprite sheet, or one specified by *spriteSheet*.
    ///
    /// See *Demo Reel* for example usage of sprite packs.
    ///
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <param name="spriteID">Sprite ID</param>
    /// <returns>Packed sprite info</returns>
    /// <code>
    /// PackedSprite hero;
    /// PackedSprite shadow;
    ///
    /// void Initialize() {
    ///     hero = PackedSprite("characters/hero_idle");
    ///     shadow = PackedSprite("other/drop_shadow");
    /// }
    ///
    /// void Render() {
    ///    // Calculate hero shadow offset position based on sprite sizes. The calculated offset will place
    ///    // the shadow underneat the heros feet.
    ///    var shadowOffset =
    ///        new Vector2i(hero.Size.width / 2 - shadow.width / 2, hero.Size.height - shadow.height / 2);
    ///
    ///    RB.DrawSprite(shadow.id, pos + shadowOffset);
    ///    RB.DrawSprite(hero.id, pos);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="SpriteSheetAsset"/>
    /// <seealso cref="PackedSprite"/>
    public static PackedSprite PackedSpriteGet(PackedSpriteID spriteID)
    {
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID.id);
    }

    /// <summary>
    /// Get packed sprite info for the given sprite, from the given sprite pack
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="spriteSheet">Sprite sheet</param>
    /// <returns>Packed sprite info</returns>
    public static PackedSprite PackedSpriteGet(PackedSpriteID spriteID, SpriteSheetAsset spriteSheet)
    {
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID.id, spriteSheet);
    }

    /// <summary>
    /// Get packed sprite info for the given sprite, from the current sprite pack
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <returns>Packed sprite info</returns>
    public static PackedSprite PackedSpriteGet(string spriteName)
    {
        int spriteID = RetroBlitInternal.RBUtil.StableStringHash(spriteName);
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID);
    }

    /// <summary>
    /// Get packed sprite info for the given sprite, from the given sprite pack
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="spriteSheet">Sprite sheet</param>
    /// <returns>Packed sprite info</returns>
    public static PackedSprite PackedSpriteGet(string spriteName, SpriteSheetAsset spriteSheet)
    {
        int spriteID = RetroBlitInternal.RBUtil.StableStringHash(spriteName);
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID, spriteSheet);
    }

    /// <summary>
    /// Get packed sprite info for the given sprite, from the current sprite pack
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <returns>Packed sprite info</returns>
    public static PackedSprite PackedSpriteGet(FastString spriteName)
    {
        int spriteID = RetroBlitInternal.RBUtil.StableStringHash(spriteName);
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID);
    }

    /// <summary>
    /// Get packed sprite info for the given sprite, from the given sprite pack
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="spriteSheet">Sprite sheet</param>
    /// <returns>Packed sprite info</returns>
    public static PackedSprite PackedSpriteGet(FastString spriteName, SpriteSheetAsset spriteSheet)
    {
        int spriteID = RetroBlitInternal.RBUtil.StableStringHash(spriteName);
        return mInstance.mRetroBlitAPI.Renderer.PackedSpriteGet(spriteID, spriteSheet);
    }

    /// <summary>
    /// Set the current sprite sheet
    /// </summary>
    /// <remarks>
    /// Sets the current sprite sheet to one previously loaded with <see cref="SpriteSheetAsset.Load"/> or created with <see cref="SpriteSheetAsset.Create"/>.
    /// This sprite sheet will be the source of all drawing that follows with the methods
    /// <see cref="RB.DrawSprite"/>, <see cref="RB.DrawCopy"/>, <see cref="RB.Print"/>, <see cref="RB.DrawNineSlice"/>, and <see cref="RB.DrawMapLayer"/>.
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <param name="spriteSheet">Sprite sheet asset to switch to.</param>
    /// <code>
    /// SpriteSheetAsset spriteSheetTiles = new SpriteSheetAsset();
    /// SpriteSheetAsset spritePackCharacters = new SpriteSheetAsset();
    ///
    /// void Initialize() {
    ///     spriteSheetTiles.Load("spritesheets/tiles");
    ///     spriteSheetTiles.grid = new Vector2i(32, 32);
    ///
    ///     spritePackCharacters.Load("spritepack/characters");
    /// }
    ///
    /// void Render() {
    ///     // Draw tilemap from the tiles spritesheet
    ///     RB.SpriteSheetSet(spriteSheetTiles);
    ///     RB.DrawMapLayer(0);
    ///
    ///     // Draw the hero from the characters sprite pack
    ///     // (a sprite pack is a type of sprite sheet with named sprites)
    ///     RB.SpriteSheetSet(spritePackCharacters);
    ///     RB.DrawSprite("hero/idle", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.SpriteSheetGet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    /// <seealso cref="RB.DrawSprite"/>
    public static void SpriteSheetSet(SpriteSheetAsset spriteSheet)
    {
        if (spriteSheet == null)
        {
            Debug.LogError("Invalid sprite sheet");
            return;
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;

        // Do nothing if no change
        if (spriteSheet == renderer.CurrentSpriteSheet)
        {
            return;
        }

        renderer.SpriteSheetSet(spriteSheet);
    }

    /// <summary>
    /// Gets the current sprite sheet.
    /// </summary>
    /// <remarks>
    /// Get the current sprite sheet which was previously set by <see cref="RB.SpriteSheetSet"/>.
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Drawing</seedoc>
    /// </remarks>
    /// <returns>Index of the current sprite sheet</returns>
    /// <code>
    /// SpriteSheetAsset spriteSheetTiles = new SpriteSheetAsset();
    /// SpriteSheetAsset spritePackCharacters = new SpriteSheetAsset();
    ///
    /// void Initialize() {
    ///     spriteSheetTiles.Load("spritesheets/tiles");
    ///     spriteSheetTiles.grid = new Vector2i(32, 32);
    ///
    ///     spritePackCharacters.Load("spritepack/characters");
    /// }
    ///
    /// void RenderPass() {
    ///     if (RB.SpriteSheetGet() == spriteSheetTiles) {
    ///         RB.DrawMapLayer(0);
    ///     }
    ///     else if (RB.SpriteSheetGet() == spritePackCharacters) {
    ///         RB.DrawSprite("hero/idle", pos);
    ///     }
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spriteSheetTiles);
    ///     RenderPass();
    ///
    ///     RB.SpriteSheetSet(spritePackCharacters);
    ///     RenderPass();
    /// }
    /// </code>
    /// <seealso cref="RB.SpriteSheetSet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public static SpriteSheetAsset SpriteSheetGet()
    {
        return mInstance.mRetroBlitAPI.Renderer.CurrentSpriteSheet;
    }

    /// <summary>
    /// Draw a sprite from the current sprite sheet
    /// </summary>
    /// <remarks>
    /// Draw a sprite from the current sprite sheet as set by <see cref="RB.SpriteSheetSet"/>. There are many overloads of this method to fit different needs.
    ///
    /// The destination position of the drawn sprite is specified with *pos*, or *destRect*. When using *destRect* the destination size could be different than the
    /// source sprite size, which allows for scaling the sprite to any size.
    ///
    /// Sprites can be drawn from two sources with <see cref="RB.DrawSprite"/>, either with a *spriteIndex* of a sprite in a sprite sheet, or
    /// with a sprite from a sprite pack specified with *spriteName*, *spriteID* or *sprite*.
    ///
    /// <h3>Sprite specified by sprite index</h3>
    /// When drawing by sprite index the current sprite sheet should be laid out in a grid fashion, with each sprite being the same size. The sprite size can be
    /// specified when setting up the sprite sheet with <see cref="SpriteSheetAsset"/>. The sprite index represents the cell number in this sprite grid, with the
    /// top left being index 0. The indices increase in a row-major order, meaning the index to the right of 0 is 1, and the index below 0 is equal to count of columns in a row.
    ///
    /// The convenience method <see cref="RB.SpriteIndex"/> can be used to calculate a sprite index given the row and column.
    ///
    /// <h3>Sprite specified by packed sprite</h3>
    ///
    /// When drawing with packed sprites the current sprite sheet must have been created by <see cref="SpriteSheetAsset"/> with a sprite pack. Packed sprites can
    /// be specified by their name which corresponds to the source filename of the unpacked sprite, or by their <see cref="PackedSprite"/> or <see cref="PackedSpriteID"/> representation.
    ///
    /// When using packed sprites it is more optimal to pre-fetch sprite IDs with <see cref="PackedSpriteGet"/> or <see cref="PackedSpriteID"/> to avoid a relatively
    /// expensive string hashing operation behind the scenes.
    ///
    /// <h3>Optional parameters</h3>
    /// Optional parameter *flags* allows for flipping the sprite horizontally or vertically by using <see cref="RB.FLIP_H"/> and <see cref="RB.FLIP_V"/> respectively.
    /// The <see cref="RB.ROT_90_CW"/> flag can be used to rotate the sprite by 90 degrees. A combination of these flags can be used to rotate and flip in any cardinal direction,
    /// for convenience these combinations are provided:
    /// <list type="bullet">
    /// <item><see cref="RB.ROT_90_CW"/></item>
    /// <item><see cref="RB.ROT_90_CCW"/></item>
    /// <item><see cref="RB.ROT_180_CW"/></item>
    /// <item><see cref="RB.ROT_180_CCW"/></item>
    /// <item><see cref="RB.ROT_270_CW"/></item>
    /// <item><see cref="RB.ROT_270_CCW"/></item>
    /// </list>
    /// For arbitrary rotations the *pivot* parameter can specify the rotation center (offset from the sprites top-left corner), and the *rotation* parameter can specify any angle in degrees.
    /// <seedoc>Features:Drawing</seedoc>
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// </remarks>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <code>
    /// SpriteSheetAsset spriteItems;
    /// SpriteSheetAsset spriteEffects;
    ///
    /// PackedSprite explosionSprite;
    ///
    /// void Initialize() {
    ///     // A sprite sheet with a mixture of tightly packed 32x32 sized sprites
    ///     spriteItems.Load("spritesheets/items");
    ///     spriteItems.grid = new SpriteGrid(new Vector2i(32, 32));
    ///
    ///     // A sprite sheet from a sprite pack
    ///     spriteEffects.Load("spritepacks/effects");
    ///
    ///     RB.SpriteSheetSet(spriteEffects);
    ///
    ///     explosionSprite = RB.PackedSpriteGet("explosion");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spriteItems);
    ///
    ///     // Draw an item from column 2 and row 4, optionally flipped horizontally
    ///     if (flip) {
    ///         RB.DrawSprite(RB.SpriteIndex(2, 4), pos, RB.FLIP_H);
    ///     } else {
    ///         RB.DrawSprite(RB.SpriteIndex(2, 4), pos);
    ///     }
    ///
    ///     RB.SpriteSheetSet(spriteEffects);
    ///
    ///     // Draw an explosion sprite from a sprite pack, rotated with the rotation pivot point at its center.
    ///     RB.DrawSprite(
    ///         explosionSprite, pos,
    ///         new Vector2i(explosionSprite.Size.width / 2, explosionSprite.Size.height / 2),
    ///         (int)RB.Ticks() % 360);
    /// }
    /// </code>
    /// <seealso cref="RB.DrawCopy"/>
    /// <seealso cref="RB.DrawNineSlice"/>
    /// <seealso cref="RB.SpriteSheetSet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public static void DrawSprite(int spriteIndex, Vector2i pos)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            pos.x,
            pos.y,
            spriteWidth,
            spriteHeight,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Vector2i pos, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            pos.x,
            pos.y,
            spriteWidth,
            spriteHeight,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(int spriteIndex, Vector2i pos, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            DrawSprite(spriteIndex, pos);
            return;
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            pos.x,
            pos.y,
            spriteWidth,
            spriteHeight,
            pivot.x,
            pivot.y,
            rotation,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            DrawSprite(spriteIndex, destRect);
            return;
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pivot.x,
            pivot.y,
            rotation,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        if (rotation == 0)
        {
            DrawSprite(spriteIndex, pos, flags);
            return;
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            pos.x,
            pos.y,
            spriteWidth,
            spriteHeight,
            pivot.x,
            pivot.y,
            rotation,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteIndex">Sprite index</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(int spriteIndex, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        if (rotation == 0)
        {
            DrawSprite(spriteIndex, destRect, flags);
            return;
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        renderer.DrawTexture(
            (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x,
            (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y,
            spriteWidth,
            spriteHeight,
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pivot.x,
            pivot.y,
            rotation,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    public static void DrawSprite(string spriteName, Vector2i pos)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawSprite(string spriteName, Rect2i destRect)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(string spriteName, Vector2i pos, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(string spriteName, Rect2i destRect, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(string spriteName, Vector2i pos, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(string spriteName, Rect2i destRect, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(string spriteName, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(string spriteName, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    public static void DrawSprite(FastString spriteName, Vector2i pos)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawSprite(FastString spriteName, Rect2i destRect)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(FastString spriteName, Vector2i pos, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(FastString spriteName, Rect2i destRect, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(FastString spriteName, Vector2i pos, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(FastString spriteName, Rect2i destRect, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(FastString spriteName, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, pos, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(FastString spriteName, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawSprite(sprite, destRect, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteID">Sprite SpriteID</param>
    /// <param name="pos">Position on display</param>
    public static void DrawSprite(PackedSpriteID spriteID, Vector2i pos)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, pos);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawSprite(PackedSpriteID spriteID, Rect2i destRect)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, destRect);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSpriteID spriteID, Vector2i pos, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, pos, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSpriteID spriteID, Rect2i destRect, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, destRect, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(PackedSpriteID spriteID, Vector2i pos, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, pos, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(PackedSpriteID spriteID, Rect2i destRect, Vector2i pivot, float rotation)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, destRect, pivot, rotation);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSpriteID spriteID, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, pos, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="spriteID">Sprite ID</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSpriteID spriteID, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawSprite(sprite, destRect, pivot, rotation, flags);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="pos">Position on display</param>
    public static void DrawSprite(PackedSprite sprite, Vector2i pos)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pos.x + sprite.TrimOffset.x,
            pos.y + sprite.TrimOffset.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawSprite(PackedSprite sprite, Rect2i destRect)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        float xScale = destRect.width / (float)sprite.Size.width;
        float yScale = destRect.height / (float)sprite.Size.height;

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            destRect.x + Mathf.RoundToInt(sprite.TrimOffset.x * xScale),
            destRect.y + Mathf.RoundToInt(sprite.TrimOffset.y * yScale),
            (destRect.width - Mathf.RoundToInt(sprite.Size.width * xScale)) + Mathf.RoundToInt(sprite.SourceRect.width * xScale),
            (destRect.height - Mathf.RoundToInt(sprite.Size.height * yScale)) + Mathf.RoundToInt(sprite.SourceRect.height * yScale),
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="pos">Position on display</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSprite sprite, Vector2i pos, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        int lookupFlags = flags & 0x7;

        // Need to calculate the drawing offset based on trimOffset and flags, gets a bit hairy,
        // using a lookup table and maths to avoid any branching
        int mx = RBRenderer.PackedSriteOffsetLookup[lookupFlags].x;
        int my = RBRenderer.PackedSriteOffsetLookup[lookupFlags].y;

        int offsetx = (mx * sprite.TrimOffset.x) + ((1 - mx) * (sprite.Size.width - sprite.SourceRect.width - sprite.TrimOffset.x));
        int offsety = (my * sprite.TrimOffset.y) + ((1 - my) * (sprite.Size.height - sprite.SourceRect.height - sprite.TrimOffset.y));

        int invert = ((flags & RB.ROT_90_CW) & 0x7) >> 2;

        int finalOffsetx = ((1 - invert) * offsetx) + (invert * offsety);
        int finalOffsety = ((1 - invert) * offsety) + (invert * offsetx);

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pos.x + finalOffsetx,
            pos.y + finalOffsety,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, and a destination rectangle.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSprite sprite, Rect2i destRect, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        float xScale = destRect.width / (float)sprite.Size.width;
        float yScale = destRect.height / (float)sprite.Size.height;

        int lookupFlags = flags & 0x7;

        // Need to calculate the drawing offset based on trimOffset and flags, gets a bit hairy,
        // using a lookup table and maths to avoid any branching
        int mx = RBRenderer.PackedSriteOffsetLookup[lookupFlags].x;
        int my = RBRenderer.PackedSriteOffsetLookup[lookupFlags].y;

        int scaledTrimOffsetX = Mathf.RoundToInt(sprite.TrimOffset.x * xScale);
        int scaledTrimOffsetY = Mathf.RoundToInt(sprite.TrimOffset.y * yScale);
        int scaledSourceSizeX = Mathf.RoundToInt(sprite.SourceRect.width * xScale);
        int scaledSourceSizeY = Mathf.RoundToInt(sprite.SourceRect.height * yScale);
        int scaledSpriteSizeX = Mathf.RoundToInt(sprite.Size.width * xScale);
        int scaledSpriteSizeY = Mathf.RoundToInt(sprite.Size.height * yScale);

        int offsetx = (mx * scaledTrimOffsetX) + ((1 - mx) * (scaledSpriteSizeX - scaledSourceSizeX - scaledTrimOffsetX));
        int offsety = (my * scaledTrimOffsetY) + ((1 - my) * (scaledSpriteSizeY - scaledSourceSizeY - scaledTrimOffsetY));

        int invert = ((flags & RB.ROT_90_CW) & 0x7) >> 2;

        int finalOffsetx = ((1 - invert) * offsetx) + (invert * offsety);
        int finalOffsety = ((1 - invert) * offsety) + (invert * offsetx);

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            destRect.x + finalOffsetx,
            destRect.y + finalOffsety,
            (destRect.width - scaledSpriteSizeX) + scaledSourceSizeX,
            (destRect.height - scaledSpriteSizeY) + scaledSourceSizeY,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(PackedSprite sprite, Vector2i pos, Vector2i pivot, float rotation)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pos.x + sprite.TrimOffset.x,
            pos.y + sprite.TrimOffset.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pivot.x - sprite.TrimOffset.x,
            pivot.y - sprite.TrimOffset.y,
            rotation,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawSprite(PackedSprite sprite, Rect2i destRect, Vector2i pivot, float rotation)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        float xScale = destRect.width / (float)sprite.Size.width;
        float yScale = destRect.height / (float)sprite.Size.height;

        int scaledTrimOffsetX = Mathf.RoundToInt(sprite.TrimOffset.x * xScale);
        int scaledTrimOffsetY = Mathf.RoundToInt(sprite.TrimOffset.y * yScale);
        int scaledSourceSizeX = Mathf.RoundToInt(sprite.SourceRect.width * xScale);
        int scaledSourceSizeY = Mathf.RoundToInt(sprite.SourceRect.height * yScale);
        int scaledSpriteSizeX = Mathf.RoundToInt(sprite.Size.width * xScale);
        int scaledSpriteSizeY = Mathf.RoundToInt(sprite.Size.height * yScale);

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            destRect.x + scaledTrimOffsetX,
            destRect.y + scaledTrimOffsetY,
            (destRect.width - scaledSpriteSizeX) + scaledSourceSizeX,
            (destRect.height - scaledSpriteSizeY) + scaledSourceSizeY,
            pivot.x - scaledTrimOffsetX,
            pivot.y - scaledTrimOffsetY,
            rotation,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="pos">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the sprites top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSprite sprite, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        int lookupFlags = flags & 0x7;

        // Need to calculate the drawing offset based on trimOffset and flags, gets a bit hairy,
        // using a lookup table and maths to avoid any branching
        int mx = RBRenderer.PackedSriteOffsetLookup[lookupFlags].x;
        int my = RBRenderer.PackedSriteOffsetLookup[lookupFlags].y;

        int offsetx = (mx * sprite.TrimOffset.x) + ((1 - mx) * (sprite.Size.width - sprite.SourceRect.width - sprite.TrimOffset.x));
        int offsety = (my * sprite.TrimOffset.y) + ((1 - my) * (sprite.Size.height - sprite.SourceRect.height - sprite.TrimOffset.y));

        int invert = ((flags & RB.ROT_90_CW) & 0x7) >> 2;

        int finalOffsetx = ((1 - invert) * offsetx) + (invert * offsety);
        int finalOffsety = ((1 - invert) * offsety) + (invert * offsetx);

        pivot.x -= finalOffsetx;
        pivot.y -= finalOffsety;

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pos.x + finalOffsetx,
            pos.y + finalOffsety,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            pivot.x,
            pivot.y,
            rotation,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Draw a sprite with a given sprite index from the sprite sheet, a destination rectangle, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="sprite">Sprite</param>
    /// <param name="destRect">Position on display</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawSprite(PackedSprite sprite, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        int lookupFlags = flags & 0x7;

        float xScale = destRect.width / (float)sprite.Size.width;
        float yScale = destRect.height / (float)sprite.Size.height;

        // Need to calculate the drawing offset based on trimOffset and flags, gets a bit hairy,
        // using a lookup table and maths to avoid any branching
        int mx = RBRenderer.PackedSriteOffsetLookup[lookupFlags].x;
        int my = RBRenderer.PackedSriteOffsetLookup[lookupFlags].y;

        int scaledTrimOffsetX = Mathf.RoundToInt(sprite.TrimOffset.x * xScale);
        int scaledTrimOffsetY = Mathf.RoundToInt(sprite.TrimOffset.y * yScale);
        int scaledSourceSizeX = Mathf.RoundToInt(sprite.SourceRect.width * xScale);
        int scaledSourceSizeY = Mathf.RoundToInt(sprite.SourceRect.height * yScale);
        int scaledSpriteSizeX = Mathf.RoundToInt(sprite.Size.width * xScale);
        int scaledSpriteSizeY = Mathf.RoundToInt(sprite.Size.height * yScale);

        int offsetx = (mx * scaledTrimOffsetX) + ((1 - mx) * (scaledSpriteSizeX - scaledSourceSizeX - scaledTrimOffsetX));
        int offsety = (my * scaledTrimOffsetY) + ((1 - my) * (scaledSpriteSizeY - scaledSourceSizeY - scaledTrimOffsetY));

        int invert = ((flags & RB.ROT_90_CW) & 0x7) >> 2;

        int finalOffsetx = ((1 - invert) * offsetx) + (invert * offsety);
        int finalOffsety = ((1 - invert) * offsety) + (invert * offsetx);

        pivot.x -= finalOffsetx;
        pivot.y -= finalOffsety;

        renderer.DrawTexture(
            sprite.SourceRect.x,
            sprite.SourceRect.y,
            sprite.SourceRect.width,
            sprite.SourceRect.height,
            destRect.x + finalOffsetx,
            destRect.y + finalOffsety,
            (destRect.width - scaledSpriteSizeX) + scaledSourceSizeX,
            (destRect.height - scaledSpriteSizeY) + scaledSourceSizeY,
            pivot.x,
            pivot.y,
            rotation,
            flags,
            1,
            1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to the display
    /// </summary>
    /// <remarks>
    /// Draw to display by copying from a rectangular region in a sprite sheet.
    ///
    /// The destination position is specified by *pos*, or *destRect*. When using *destRect* the destination size could be different than the
    /// source rectangle size, which allows for scaling to any size.
    ///
    /// Optional parameter *flags* allows for flipping the copied region horizontally or vertically by using <see cref="RB.FLIP_H"/> and <see cref="RB.FLIP_V"/> respectively.
    /// The <see cref="RB.ROT_90_CW"/> flag can be used to rotate the region by 90 degrees. A combination of these flags can be used to rotate and flip in any cardinal direction,
    /// for convenience these combinations are provided:
    /// <list type="bullet">
    /// <item><see cref="RB.ROT_90_CW"/></item>
    /// <item><see cref="RB.ROT_90_CCW"/></item>
    /// <item><see cref="RB.ROT_180_CW"/></item>
    /// <item><see cref="RB.ROT_180_CCW"/></item>
    /// <item><see cref="RB.ROT_270_CW"/></item>
    /// <item><see cref="RB.ROT_270_CCW"/></item>
    /// </list>
    /// For arbitrary rotations the *pivot* parameter can specify the rotation center (offset from the source region top-left corner), and the *rotation* parameter can specify any angle in degrees.
    /// <seedoc>Features:Drawing</seedoc>
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// </remarks>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position</param>
    /// <code>
    /// SpriteSheetAsset spritesBackground;
    ///
    /// PackedSprite explosionSprite;
    ///
    /// void Initialize() {
    ///     spritesBackground.Load("spritesheets/backgrounds");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spritesBackground);
    ///
    ///     // Copy a 256x128 region from the sprite sheet into a 512x256 destination (x2 scale)
    ///     RB.DrawCopy(new Rect2i(0, 0, 256, 128), new Rect2i(0, 0, 512, 256));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawNineSlice"/>
    /// <seealso cref="RB.SpriteSheetSet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, pos.x, pos.y, srcRect.width, srcRect.height, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to the given position.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, pos.x, pos.y, srcRect.width, srcRect.height, flags, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to the given position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangles top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            DrawCopy(srcRect, pos);
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, pos.x, pos.y, srcRect.width, srcRect.height, pivot.x, pivot.y, rotation, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to the given position, a pivot point and a rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="pos">Position</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangles top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        if (rotation == 0)
        {
            DrawCopy(srcRect, pos, flags);
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, pos.x, pos.y, srcRect.width, srcRect.height, pivot.x, pivot.y, rotation, flags, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, destRect.x, destRect.y, destRect.width, destRect.height, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect, int flags = 0)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, destRect.x, destRect.y, destRect.width, destRect.height, flags, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle, with the given pivot point and rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            DrawCopy(srcRect, destRect);
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, destRect.x, destRect.y, destRect.width, destRect.height, pivot.x, pivot.y, rotation, 1, 1);
    }

    /// <summary>
    /// Copy a rectangular region from the sprite sheet to a destination rectangle, with the given pivot point and rotation in degrees.
    /// </summary>
    /// <param name="srcRect">Source rectangle on the sprite sheet</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the destination rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawCopy(Rect2i srcRect, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        if (rotation == 0)
        {
            DrawCopy(srcRect, destRect, flags);
        }

        var renderer = mInstance.mRetroBlitAPI.Renderer;

        renderer.DrawTexture(srcRect.x, srcRect.y, srcRect.width, srcRect.height, destRect.x, destRect.y, destRect.width, destRect.height, pivot.x, pivot.y, rotation, flags, 1, 1);
    }

    /// <summary>
    /// Draw a nine-slice sprite
    /// </summary>
    /// <remarks>
    /// Draw a nine-slice sprite. A nine-slice sprite is a special kind of sprite usually used to draw rectangular shapes for UI elements.
    /// A nine-slice sprite is defined by up to 9 separate sprites that are arranged and repeated as needed in such a way that they can
    /// fill any rectangular area.
    ///
    /// <image src="nineslice_parts.png">Nine parts that make up a nine slice image</image>
    ///
    /// The pieces *A*, *C*, *G*, and *I* are always drawn in the corners of the nine-slice sprite. *B* and *H* are repeated horizontally.
    /// *E* and *F* are repeated vertically. Finally, *X* is repeated in both directions to fill the middle of the nine-slice sprite.
    ///
    /// For best results each of these groups of pieces should be the same height:
    /// <list type="bullet">
    /// <item> *A*, *B*, *C*</item>
    /// <item> *E*, *F*</item>
    /// <item> *G*, *H*, *I*</item>
    /// </list>
    /// These groups should be the same width:
    /// <list type="bullet">
    /// <item> *A*, *E*, *G*</item>
    /// <item> *B*, *H*</item>
    /// <item> *C*, *F*, *I*</item>
    /// </list>
    /// The center piece *X* can be of any size.
    ///
    /// The pieces that make up a nine-slice image should not be too small to limit the amount of times they have to be repeated to fill
    /// your nine-slice sprite.
    ///
    /// In some cases, such as in the image above, many of the pieces are symmetric, for convenience there are <see cref="RB.DrawNineSlice"/> overloads that
    /// take only *srcTopLeftCornerRect*, *srcTopSideRect*, and *srcMiddleRect* and simply rotate and mirror the remaining pieces automatically.
    ///
    /// There are a number of different ways to specify the pieces of a nine-slice image. You can use source rectangles in a sprite sheet, or packed sprites. You may also
    /// use <see cref="NineSlice"/> structure to predefine a nine-slice in a convenient format, and then reuse it to draw the nine-slice image with <see cref="RB.DrawNineSlice"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// </remarks>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerRect">Source rectangle of the top left corner</param>
    /// <param name="srcTopSideRect">Source rectangle of the top side</param>
    /// <param name="srcMiddleRect">Source rectangle of the middle</param>
    /// <code>
    /// SpriteSheetAsset uiSprites = new SpriteSheetAsset();
    /// NineSlice dialogFrame;
    ///
    /// void Initialize() {
    ///     uiSprites.Load("spritesheets/ui");
    ///     dialogFrame = new NineSlice(
    ///         new Rect2i(0, 0, 8, 8),
    ///         new Rect2i(8, 0, 8, 8),
    ///         new Rect2i(16, 0, 8, 8));
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(uiSprites);
    ///
    ///     var dialogRect = new Rect2i(100, 100, 128, 64);
    ///
    ///     // Draw a dialog frame using the predefined NineSlice
    ///     RB.DrawNineSlice(dialogRect, dialogFrame);
    ///     RB.Print(
    ///         dialogRect, Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER,
    ///         "It's boring to go alone, take me!");
    /// }
    /// </code>
    /// <seealso cref="RB.DrawSprite"/>
    /// <seealso cref="RB.DrawNineSlice"/>
    /// <seealso cref="RB.SpriteSheetSet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public static void DrawNineSlice(Rect2i destRect, Rect2i srcTopLeftCornerRect, Rect2i srcTopSideRect, Rect2i srcMiddleRect)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerRect,
            0,
            srcTopSideRect,
            0,
            srcTopLeftCornerRect,
            RB.FLIP_H,
            srcTopSideRect,
            RB.ROT_90_CCW,
            srcMiddleRect,
            srcTopSideRect,
            RB.ROT_90_CW,
            srcTopLeftCornerRect,
            RB.FLIP_V,
            srcTopSideRect,
            RB.FLIP_V,
            srcTopLeftCornerRect,
            RB.FLIP_H | RB.FLIP_V);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerRect">Source rectangle of the top left corner</param>
    /// <param name="srcTopSideRect">Source rectangle of the top side</param>
    /// <param name="srcTopRightCornerRect">Source rectangle of the top right corner</param>
    /// <param name="srcLeftSideRect">Source rectangle of the left side</param>
    /// <param name="srcMiddleRect">Source rectangle of the middle</param>
    /// <param name="srcRightSideRect">Source rectangle of the right side</param>
    /// <param name="srcBottomLeftCornerRect">Source rectangle of the bottom left corner</param>
    /// <param name="srcBottomSideRect">Source rectangle of the bottom side</param>
    /// <param name="srcBottomRightCornerRect">Source rectangle of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        Rect2i srcTopLeftCornerRect,
        Rect2i srcTopSideRect,
        Rect2i srcTopRightCornerRect,
        Rect2i srcLeftSideRect,
        Rect2i srcMiddleRect,
        Rect2i srcRightSideRect,
        Rect2i srcBottomLeftCornerRect,
        Rect2i srcBottomSideRect,
        Rect2i srcBottomRightCornerRect)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerRect,
            0,
            srcTopSideRect,
            0,
            srcTopRightCornerRect,
            0,
            srcLeftSideRect,
            0,
            srcMiddleRect,
            srcRightSideRect,
            0,
            srcBottomLeftCornerRect,
            0,
            srcBottomSideRect,
            0,
            srcBottomRightCornerRect,
            0);
    }

    /// <summary>
    /// Draw a nine-slice sprite. Only need to pass one corner, one side, and middle, the rest is mirrored.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerID">Source sprite ID of the top left corner</param>
    /// <param name="srcTopSideID">Source sprite ID of the top side</param>
    /// <param name="srcMiddleID">Source sprite ID of the middle</param>
    public static void DrawNineSlice(Rect2i destRect, PackedSpriteID srcTopLeftCornerID, PackedSpriteID srcTopSideID, PackedSpriteID srcMiddleID)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerID,
            0,
            srcTopSideID,
            0,
            srcTopLeftCornerID,
            RB.FLIP_H,
            srcTopSideID,
            RB.ROT_90_CCW,
            srcMiddleID,
            srcTopSideID,
            RB.ROT_90_CW,
            srcTopLeftCornerID,
            RB.FLIP_V,
            srcTopSideID,
            RB.FLIP_V,
            srcTopLeftCornerID,
            RB.FLIP_H | RB.FLIP_V);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerID">Source sprite ID of the top left corner</param>
    /// <param name="srcTopSideID">Source sprite ID of the top side</param>
    /// <param name="srcTopRightCornerID">Source sprite ID of the top right corner</param>
    /// <param name="srcLeftSideID">Source sprite ID of the left side</param>
    /// <param name="srcMiddleID">Source sprite ID of the middle</param>
    /// <param name="srcRightSideID">Source sprite ID of the right side</param>
    /// <param name="srcBottomLeftCornerID">Source sprite ID of the bottom left corner</param>
    /// <param name="srcBottomSideID">Source sprite ID of the bottom side</param>
    /// <param name="srcBottomRightCornerID">Source sprite ID of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        PackedSpriteID srcTopLeftCornerID,
        PackedSpriteID srcTopSideID,
        PackedSpriteID srcTopRightCornerID,
        PackedSpriteID srcLeftSideID,
        PackedSpriteID srcMiddleID,
        PackedSpriteID srcRightSideID,
        PackedSpriteID srcBottomLeftCornerID,
        PackedSpriteID srcBottomSideID,
        PackedSpriteID srcBottomRightCornerID)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCornerID,
            0,
            srcTopSideID,
            0,
            srcTopRightCornerID,
            0,
            srcLeftSideID,
            0,
            srcMiddleID,
            srcRightSideID,
            0,
            srcBottomLeftCornerID,
            0,
            srcBottomSideID,
            0,
            srcBottomRightCornerID,
            0);
    }

    /// <summary>
    /// Draw a nine-slice sprite. Only need to pass one corner, one side, and middle, the rest is mirrored.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCorner">Source sprite of the top left corner</param>
    /// <param name="srcTopSide">Source sprite of the top side</param>
    /// <param name="srcMiddle">Source sprite of the middle</param>
    public static void DrawNineSlice(Rect2i destRect, PackedSprite srcTopLeftCorner, PackedSprite srcTopSide, PackedSprite srcMiddle)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCorner.id,
            0,
            srcTopSide.id,
            0,
            srcTopLeftCorner.id,
            RB.FLIP_H,
            srcTopSide.id,
            RB.ROT_90_CCW,
            srcMiddle.id,
            srcTopSide.id,
            RB.ROT_90_CW,
            srcTopLeftCorner.id,
            RB.FLIP_V,
            srcTopSide.id,
            RB.FLIP_V,
            srcTopLeftCorner.id,
            RB.FLIP_H | RB.FLIP_V);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCorner">Source sprite of the top left corner</param>
    /// <param name="srcTopSide">Source sprite of the top side</param>
    /// <param name="srcTopRightCorner">sprite rectangle of the top right corner</param>
    /// <param name="srcLeftSide">Source sprite of the left side</param>
    /// <param name="srcMiddle">Source sprite of the middle</param>
    /// <param name="srcRightSide">Source sprite of the right side</param>
    /// <param name="srcBottomLeftCorner">Source sprite of the bottom left corner</param>
    /// <param name="srcBottomSide">Source sprite of the bottom side</param>
    /// <param name="srcBottomRightCorner">Source sprite of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        PackedSprite srcTopLeftCorner,
        PackedSprite srcTopSide,
        PackedSprite srcTopRightCorner,
        PackedSprite srcLeftSide,
        PackedSprite srcMiddle,
        PackedSprite srcRightSide,
        PackedSprite srcBottomLeftCorner,
        PackedSprite srcBottomSide,
        PackedSprite srcBottomRightCorner)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            srcTopLeftCorner.id,
            0,
            srcTopSide.id,
            0,
            srcTopRightCorner.id,
            0,
            srcLeftSide.id,
            0,
            srcMiddle.id,
            srcRightSide.id,
            0,
            srcBottomLeftCorner.id,
            0,
            srcBottomSide.id,
            0,
            srcBottomRightCorner.id,
            0);
    }

    /// <summary>
    /// Draw a nine-slice sprite. Only need to pass one corner, one side, and middle, the rest is mirrored.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerName">Source sprite name of the top left corner</param>
    /// <param name="srcTopSideName">Source sprite name of the top side</param>
    /// <param name="srcMiddleName">Source sprite name of the middle</param>
    public static void DrawNineSlice(Rect2i destRect, string srcTopLeftCornerName, string srcTopSideName, string srcMiddleName)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            PackedSpriteID(srcTopLeftCornerName),
            0,
            PackedSpriteID(srcTopSideName),
            0,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_H,
            PackedSpriteID(srcTopSideName),
            RB.ROT_90_CCW,
            PackedSpriteID(srcMiddleName),
            PackedSpriteID(srcTopSideName),
            RB.ROT_90_CW,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_V,
            PackedSpriteID(srcTopSideName),
            RB.FLIP_V,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_H | RB.FLIP_V);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerName">Source sprite name of the top left corner</param>
    /// <param name="srcTopSideName">Source sprite name of the top side</param>
    /// <param name="srcTopRightCornerName">Source sprite name of the top right corner</param>
    /// <param name="srcLeftSideName">Source sprite name of the left side</param>
    /// <param name="srcMiddleName">Source sprite name of the middle</param>
    /// <param name="srcRightSideName">Source sprite name of the right side</param>
    /// <param name="srcBottomLeftCornerName">Source sprite name of the bottom left corner</param>
    /// <param name="srcBottomSideName">Source sprite name of the bottom side</param>
    /// <param name="srcBottomRightCornerName">Source sprite name of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        string srcTopLeftCornerName,
        string srcTopSideName,
        string srcTopRightCornerName,
        string srcLeftSideName,
        string srcMiddleName,
        string srcRightSideName,
        string srcBottomLeftCornerName,
        string srcBottomSideName,
        string srcBottomRightCornerName)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            PackedSpriteID(srcTopLeftCornerName),
            0,
            PackedSpriteID(srcTopSideName),
            0,
            PackedSpriteID(srcTopRightCornerName),
            0,
            PackedSpriteID(srcLeftSideName),
            0,
            PackedSpriteID(srcMiddleName),
            PackedSpriteID(srcRightSideName),
            0,
            PackedSpriteID(srcBottomLeftCornerName),
            0,
            PackedSpriteID(srcBottomSideName),
            0,
            PackedSpriteID(srcBottomRightCornerName),
            0);
    }

    /// <summary>
    /// Draw a nine-slice sprite. Only need to pass one corner, one side, and middle, the rest is mirrored.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerName">Source sprite name of the top left corner</param>
    /// <param name="srcTopSideName">Source sprite name of the top side</param>
    /// <param name="srcMiddleName">Source sprite name of the middle</param>
    public static void DrawNineSlice(Rect2i destRect, FastString srcTopLeftCornerName, FastString srcTopSideName, FastString srcMiddleName)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            PackedSpriteID(srcTopLeftCornerName),
            0,
            PackedSpriteID(srcTopSideName),
            0,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_H,
            PackedSpriteID(srcTopSideName),
            RB.ROT_90_CCW,
            PackedSpriteID(srcMiddleName),
            PackedSpriteID(srcTopSideName),
            RB.ROT_90_CW,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_V,
            PackedSpriteID(srcTopSideName),
            RB.FLIP_V,
            PackedSpriteID(srcTopLeftCornerName),
            RB.FLIP_H | RB.FLIP_V);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="srcTopLeftCornerName">Source sprite name of the top left corner</param>
    /// <param name="srcTopSideName">Source sprite name of the top side</param>
    /// <param name="srcTopRightCornerName">Source sprite name of the top right corner</param>
    /// <param name="srcLeftSideName">Source sprite name of the left side</param>
    /// <param name="srcMiddleName">Source sprite name of the middle</param>
    /// <param name="srcRightSideName">Source sprite name of the right side</param>
    /// <param name="srcBottomLeftCornerName">Source sprite name of the bottom left corner</param>
    /// <param name="srcBottomSideName">Source sprite name of the bottom side</param>
    /// <param name="srcBottomRightCornerName">Source sprite name of the bottom right corner</param>
    public static void DrawNineSlice(
        Rect2i destRect,
        FastString srcTopLeftCornerName,
        FastString srcTopSideName,
        FastString srcTopRightCornerName,
        FastString srcLeftSideName,
        FastString srcMiddleName,
        FastString srcRightSideName,
        FastString srcBottomLeftCornerName,
        FastString srcBottomSideName,
        FastString srcBottomRightCornerName)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
            destRect,
            PackedSpriteID(srcTopLeftCornerName),
            0,
            PackedSpriteID(srcTopSideName),
            0,
            PackedSpriteID(srcTopRightCornerName),
            0,
            PackedSpriteID(srcLeftSideName),
            0,
            PackedSpriteID(srcMiddleName),
            PackedSpriteID(srcRightSideName),
            0,
            PackedSpriteID(srcBottomLeftCornerName),
            0,
            PackedSpriteID(srcBottomSideName),
            0,
            PackedSpriteID(srcBottomRightCornerName),
            0);
    }

    /// <summary>
    /// Draw a nine-slice sprite.
    /// </summary>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="nineSlice">NineSlice defining the parts of the nine-slice image</param>
    public static void DrawNineSlice(Rect2i destRect, NineSlice nineSlice)
    {
        if (nineSlice.IsRectBased)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
                destRect,
                nineSlice.TopLeftCornerRect,
                nineSlice.FlagsTopLeftCorner,
                nineSlice.TopSideRect,
                nineSlice.FlagsTopSide,
                nineSlice.TopRightCornerRect,
                nineSlice.FlagsTopRightCorner,
                nineSlice.LeftSideRect,
                nineSlice.FlagsLeftSide,
                nineSlice.MiddleRect,
                nineSlice.RightSideRect,
                nineSlice.FlagsRightSide,
                nineSlice.BottomLeftCornerRect,
                nineSlice.FlagsBottomLeftCorner,
                nineSlice.BottomSideRect,
                nineSlice.FlagsBottomSide,
                nineSlice.BottomRightCornerRect,
                nineSlice.FlagsBottomRightCorner);
        }
        else
        {
            mInstance.mRetroBlitAPI.Renderer.DrawNineSlice(
                destRect,
                nineSlice.TopLeftCornerID,
                nineSlice.FlagsTopLeftCorner,
                nineSlice.TopSideID,
                nineSlice.FlagsTopSide,
                nineSlice.TopRightCornerID,
                nineSlice.FlagsTopRightCorner,
                nineSlice.LeftSideID,
                nineSlice.FlagsLeftSide,
                nineSlice.MiddleID,
                nineSlice.RightSideID,
                nineSlice.FlagsRightSide,
                nineSlice.BottomLeftCornerID,
                nineSlice.FlagsBottomLeftCorner,
                nineSlice.BottomSideID,
                nineSlice.FlagsBottomSide,
                nineSlice.BottomRightCornerID,
                nineSlice.FlagsBottomRightCorner);
        }
    }

    /// <summary>
    /// Draw a pixel.
    /// </summary>
    /// <remarks>
    /// Draw a single pixel of a given *color* to the display at the given position specified by *pos*.
    ///
    /// Drawing single pixels is mostly useful for simple particle effects.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawPixel(new Vector2i(100, 50), Color.red));
    /// }
    /// </code>
    public static void DrawPixel(Vector2i pos, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixel(pos.x, pos.y, color);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <remarks>
    /// Draw a pixel array with the given *pixelsSize* dimensions to the display at the given *pos*. You may also provide destination
    /// rectangle, pivot point and rotation.
    ///
    /// If the same pixel buffer is used with no changes between calls to <b>DrawPixelBuffer</b> then the flag <see cref="RB.PIXEL_BUFFER_UNCHANGED"/> should be specified.
    /// This flag tells RetroBlit not to re-upload the buffer to the GPU, which improves performance.
    /// <seedoc>Features:Pixel Buffers</seedoc>
    /// </remarks>
    /// <code>
    /// void Render()
    /// {
    ///     // Do something with the pixel buffer
    ///     updatePixelBuffer();
    ///
    ///     // Draw the pixel buffer
    ///     RB.DrawPixelBuffer(pixelBuffer, new Vector2i(100, 200), new Vector2i(0, 0));
    ///
    ///     // Draw the pixel buffer again at a new location with the pixel data unchanged
    ///     RB.DrawPixelBuffer(pixelBuffer, new Vector2i(100, 200), new Vector2i(0, 200), RB.PIXEL_BUFFER_UNCHANGED);
    /// }
    /// </code>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="pos">Position to draw at</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Vector2i pos)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            pos.x,
            pos.y,
            pixelsSize.width,
            pixelsSize.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            0,
            0,
            0,
            0);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="pos">Position to draw at</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>, <see cref="RB.PIXEL_BUFFER_UNCHANGED"/></param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Vector2i pos, int flags = 0)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            pos.x,
            pos.y,
            pixelsSize.width,
            pixelsSize.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            0,
            0,
            0,
            flags);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="pos">Position to draw at</param>
    /// <param name="pivot">Rotation pivot point</param>
    /// <param name="rotation">Rotation angle</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Vector2i pos, Vector2i pivot, float rotation)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            pos.x,
            pos.y,
            pixelsSize.width,
            pixelsSize.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            pivot.x,
            pivot.y,
            rotation,
            0);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="pos">Position to draw at</param>
    /// <param name="pivot">Rotation pivot point</param>
    /// <param name="rotation">Rotation angle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Vector2i pos, Vector2i pivot, float rotation, int flags = 0)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            pos.x,
            pos.y,
            pixelsSize.width,
            pixelsSize.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            pivot.x,
            pivot.y,
            rotation,
            flags);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="destRect">Destination rectangle</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Rect2i destRect)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            0,
            0,
            0,
            0);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Rect2i destRect, int flags = 0)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            0,
            0,
            0,
            flags);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="pivot">Rotation pivot point</param>
    /// <param name="rotation">Rotation angle</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Rect2i destRect, Vector2i pivot, float rotation)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            pivot.x,
            pivot.y,
            rotation,
            0);
    }

    /// <summary>
    /// Draw a pixel buffer.
    /// </summary>
    /// <param name="pixels">Pixel array</param>
    /// <param name="pixelsSize">Pixel array dimensions</param>
    /// <param name="destRect">Destination rectangle</param>
    /// <param name="pivot">Rotation pivot point</param>
    /// <param name="rotation">Rotation angle</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.FLIP_H"/>, <see cref="RB.FLIP_V"/>,
    /// <see cref="RB.ROT_90_CW"/>, <see cref="RB.ROT_180_CW"/>, <see cref="RB.ROT_270_CW"/>,
    /// <see cref="RB.ROT_90_CCW"/>, <see cref="RB.ROT_180_CCW"/>, <see cref="RB.ROT_270_CCW"/>.</param>
    public static void DrawPixelBuffer(Color32[] pixels, Vector2i pixelsSize, Rect2i destRect, Vector2i pivot, float rotation, int flags = 0)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawPixelBuffer(
            destRect.x,
            destRect.y,
            destRect.width,
            destRect.height,
            pixels,
            pixelsSize.width,
            pixelsSize.height,
            pivot.x,
            pivot.y,
            rotation,
            flags);
    }

    /// <summary>
    /// Draw a triangle outline
    /// </summary>
    /// <remarks>
    /// Draw a triangle outline to the display with the given *color*. The 3 points of the triangle are defined by *p0*, *p1*, and *p2*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="p0">First point of the triangle</param>
    /// <param name="p1">Second point of the triangle</param>
    /// <param name="p2">Third point of the triangle</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawTriangle(new Vector2i(100, 50), new Vector2i(100, 100), new Vector2i(150, 75), Color.red));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawTriangleFill"/>
    public static void DrawTriangle(Vector2i p0, Vector2i p1, Vector2i p2, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawTriangle(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color);
    }

    /// <summary>
    /// Draw a triangle outline with a pivot point and rotation in degrees
    /// </summary>
    /// <param name="p0">First point of the triangle</param>
    /// <param name="p1">Second point of the triangle</param>
    /// <param name="p2">Third point of the triangle</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawTriangle(Vector2i p0, Vector2i p1, Vector2i p2, Color32 color, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawTriangle(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color);
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawTriangle(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color, pivot.x, pivot.y, rotation);
    }

    /// <summary>
    /// Draw a filled triangle
    /// </summary>
    /// <remarks>
    /// Draw a filled triangle to the display with the given *color*. The 3 points of the triangle are defined by *p0*, *p1*, and *p2*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    ///
    /// Filled triangle can be useful for some special effects like 2D shadow ray-casting.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="p0">First point of the triangle</param>
    /// <param name="p1">Second point of the triangle</param>
    /// <param name="p2">Third point of the triangle</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawTriangleFill(new Vector2i(100, 50), new Vector2i(100, 100), new Vector2i(150, 75), Color.red));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawTriangle"/>
    public static void DrawTriangleFill(Vector2i p0, Vector2i p1, Vector2i p2, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawTriangleFill(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color);
    }

    /// <summary>
    /// Draw a filled triangle with a pivot point and rotation in degrees
    /// </summary>
    /// <param name="p0">First point of the triangle</param>
    /// <param name="p1">Second point of the triangle</param>
    /// <param name="p2">Third point of the triangle</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawTriangleFill(Vector2i p0, Vector2i p1, Vector2i p2, Color32 color, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawTriangleFill(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color);
        }

        mInstance.mRetroBlitAPI.Renderer.DrawTriangleFill(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, color, pivot.x, pivot.y, rotation);
    }

    /// <summary>
    /// Draw a rectangle outline
    /// </summary>
    /// <remarks>
    /// Draw a rectangle outline to the display with the given *color*, and dimensions given by *rect*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawRect(new Rect2i(100, 50, 64, 64), Color.blue));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawRectFill"/>
    public static void DrawRect(Rect2i rect, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawRect(rect.x, rect.y, rect.width, rect.height, color);
    }

    /// <summary>
    /// Draw a rectangle outline with a pivot point and rotation in degrees
    /// </summary>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRect(Rect2i rect, Color32 color, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawRect(rect.x, rect.y, rect.width, rect.height, color);
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawRect(rect.x, rect.y, rect.width, rect.height, color, pivot.x, pivot.y, rotation);
    }

    /// <summary>
    /// Draw a filled rectangle
    /// </summary>
    /// <remarks>
    /// Draw a filled rectangle to the display with the given *color*, and dimensions given by *rect*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawRectFill(new Rect2i(100, 50, 64, 64), Color.blue));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawRect"/>
    public static void DrawRectFill(Rect2i rect, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawRectFill(rect.x, rect.y, rect.width, rect.height, color);
    }

    /// <summary>
    /// Draw a filled rectangle with a pivot point and rotation in degrees
    /// </summary>
    /// <param name="rect">Rectangular area</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Rotation pivot point, specified as an offset from the rectangle's top left corner</param>
    /// <param name="rotation">Rotation in degrees</param>
    public static void DrawRectFill(Rect2i rect, Color32 color, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawRectFill(rect.x, rect.y, rect.width, rect.height, color);
        }

        mInstance.mRetroBlitAPI.Renderer.DrawRectFill(rect.x, rect.y, rect.width, rect.height, color, pivot.x, pivot.y, rotation);
    }

    /// <summary>
    /// Draw a line between two points
    /// </summary>
    /// <remarks>
    /// Draw a line from point *p0* to point *p1* with the given *color*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawLine(new Vector2i(100, 50), new Vector2i(150, 75), Color.green));
    /// }
    /// </code>
    public static void DrawLine(Vector2i p0, Vector2i p1, Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.DrawLine(p0.x, p0.y, p1.x, p1.y, color);
    }

    /// <summary>
    /// Draw a line between two points
    /// </summary>
    /// <remarks>
    /// Draw a line from point *p0* to point *p1* with the given *color*.
    ///
    /// Optionally a *pivot* point, and *rotation* angle in degrees can also be specified.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawLine(new Vector2i(100, 50), new Vector2i(150, 75), 5, Color.green));
    /// }
    /// </code>
    public static void DrawLine(Vector2i p0, Vector2i p1, int thickness, Color32 color)
    {
        if (thickness < 1)
        {
            return;
        }

        if (thickness == 1)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLine(p0.x, p0.y, p1.x, p1.y, color);
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawLine(p0.x, p0.y, p1.x, p1.y, thickness, color);
    }

    /// <summary>
    /// Draw a line between two points, with a pivot point and rotation in degrees.
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="color">Color</param>
    /// <param name="pivot">Pivot point</param>
    /// <param name="rotation">Rotation in degrees, specified as an offset from the top left corner of an imaginary rectangle that would encompass the line</param>
    public static void DrawLine(Vector2i p0, Vector2i p1, Color32 color, Vector2i pivot, float rotation)
    {
        if (rotation == 0)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLine(p0.x, p0.y, p1.x, p1.y, color);
        }

        mInstance.mRetroBlitAPI.Renderer.DrawLine(p0.x, p0.y, p1.x, p1.y, color, pivot.x, pivot.y, rotation);
    }

    /// <summary>
    /// Draw a textured line between two points
    /// </summary>
    /// <remarks>
    /// Draw a line from point *p0* to point *p1* with the specified sprite. The sprite should be aligned horizontally in your sprite sheet.
    /// </remarks>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="srcRect">Rectangular texture region to use from current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    /// <code>
    /// void Render() {
    ///     PackedSprite laserSprite = RB.PackedSpriteGet("laser");
    ///
    ///     Vector2i lineStart = new Vector2i(100, 50);
    ///     Vector2i lineEnd = new Vector2i(150, 75);
    ///     float len = (lineEnd - lineStart).Magnitude();
    ///
    ///     float repeats = len / (float)laserSprite.Size.width;
    ///     float scroll = Time.realtimeSinceStartup;
    ///
    ///     RB.DrawLineTextured(lineStart, lineEnd, laserSprite, scroll, repeats));
    /// }
    /// </code>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, Rect2i srcRect, float scroll, float repeat)
    {
        if (thickness < 1)
        {
            return;
        }

        if (thickness == 1)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
        else
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, thickness, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="srcRect">Rectangular texture region to use from current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, Rect2i srcRect, float scroll, float repeat)
    {
        DrawLineTextured(p0, p1, srcRect.height, srcRect, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="spriteIndex">The sprite index to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, int spriteIndex, float scroll, float repeat)
    {
        if (thickness < 1)
        {
            return;
        }

        Rect2i srcRect;

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        srcRect.x = (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x;
        srcRect.y = (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y;
        srcRect.width = spriteWidth;
        srcRect.height = spriteHeight;

        if (thickness == 1)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
        else
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, thickness, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="spriteIndex">The sprite index to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int spriteIndex, float scroll, float repeat)
    {
        Rect2i srcRect;

        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var currentSpriteSheet = renderer.CurrentSpriteSheet;

        int row = spriteIndex % currentSpriteSheet.internalState.columns;
        int col = spriteIndex / currentSpriteSheet.internalState.columns;

        var spriteWidth = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.width;
        var spriteHeight = renderer.CurrentSpriteSheet.internalState.spriteGrid.cellSize.height;

        srcRect.x = (row * spriteWidth) + currentSpriteSheet.internalState.spriteGrid.region.x;
        srcRect.y = (col * spriteHeight) + currentSpriteSheet.internalState.spriteGrid.region.y;
        srcRect.width = spriteWidth;
        srcRect.height = spriteHeight;

        if (spriteHeight == 1)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
        else
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, spriteHeight, srcRect.x, srcRect.y, srcRect.width, srcRect.height, scroll, repeat);
        }
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="spriteName">The sprite name to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, string spriteName, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawLineTextured(p0, p1, thickness, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="spriteName">The sprite name to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, string spriteName, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawLineTextured(p0, p1, sprite.Size.height, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="spriteName">The sprite name to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, FastString spriteName, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawLineTextured(p0, p1, thickness, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="spriteName">The sprite name to use from the current sprite sheet</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, FastString spriteName, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteName);
        DrawLineTextured(p0, p1, sprite.Size.height, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="spriteID">The sprite ID to use</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, PackedSpriteID spriteID, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawLineTextured(p0, p1, thickness, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="spriteID">The sprite ID to use</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, PackedSpriteID spriteID, float scroll, float repeat)
    {
        var sprite = PackedSpriteGet(spriteID);
        DrawLineTextured(p0, p1, sprite.Size.height, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="thickness">Thickness of the line in pixels</param>
    /// <param name="sprite">The sprite to use</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, int thickness, PackedSprite sprite, float scroll, float repeat)
    {
        if (thickness < 1)
        {
            return;
        }

        if (thickness == 1)
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, sprite.SourceRect.x, sprite.SourceRect.y, sprite.SourceRect.width, sprite.SourceRect.height, scroll, repeat);
        }
        else
        {
            mInstance.mRetroBlitAPI.Renderer.DrawLineTextured(p0.x, p0.y, p1.x, p1.y, thickness, sprite.SourceRect.x, sprite.SourceRect.y, sprite.SourceRect.width, sprite.SourceRect.height, scroll, repeat);
        }
    }

    /// <summary>
    /// Draw a textured lined between two points
    /// </summary>
    /// <param name="p0">One end of the line</param>
    /// <param name="p1">The other end of the line</param>
    /// <param name="sprite">The sprite to use</param>
    /// <param name="scroll">Scrolls the sprite horizontally by the given amount</param>
    /// <param name="repeat">Repeat the sprite texture the given amount of time across the length of the line</param>
    public static void DrawLineTextured(Vector2i p0, Vector2i p1, PackedSprite sprite, float scroll, float repeat)
    {
        DrawLineTextured(p0, p1, sprite.Size.height, sprite, scroll, repeat);
    }

    /// <summary>
    /// Draw an ellipse outline
    /// </summary>
    /// <remarks>
    /// Draw an ellipse outline with the given *center*, a horizontal and vertical radius given by *radius*, and the given *color*.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawEllipse(new Vector2i(100, 50), new Vector2i(40, 20), Color.white));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawEllipseFill"/>
    /// <seealso cref="RB.DrawEllipseInvertedFill"/>
    public static void DrawEllipse(Vector2i center, Vector2i radius, Color32 color)
    {
        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawEllipse(center.x, center.y, radius.x, radius.y, color);
    }

    /// <summary>
    /// Draw a filled ellipse
    /// </summary>
    /// <remarks>
    /// Draw a filled ellipse with the given *center*, a horizontal and vertical radius given by *radius*, and the given *color*.
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawEllipseFill(new Vector2i(100, 50), new Vector2i(40, 20), Color.white));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawEllipseInvertedFill"/>
    /// <seealso cref="RB.DrawEllipse"/>
    public static void DrawEllipseFill(Vector2i center, Vector2i radius, Color32 color)
    {
        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawEllipseFill(center.x, center.y, radius.x, radius.y, color, false);
    }

    /// <summary>
    /// Draw an inversely filled ellipse
    /// </summary>
    /// <remarks>
    /// Draw an inversely filled ellipse with the given *center*, a horizontal and vertical radius given by *radius*, and the given *color*.
    ///
    /// An inversely filled ellipse occupies the space not filled by a normal ellipse. This can be useful for a variety of effects such as
    /// simulating a torch light around a player by using a combination of
    /// <see cref="RB.DrawEllipseInvertedFill"/> and <see cref="RB.DrawRectFill"/>
    /// to cover up an area further away from the player.
    ///
    /// <image src="ellipse_fill.png">Normally filled orange ellipse drawn with RB.DrawEllipseFill</image>
    /// <image src="ellipse_inverted_fill.png">Inversely filled orange ellipse drawn with RB.DrawEllipseInvertedFill</image>
    /// <seedoc>Features:Primitives</seedoc>
    /// </remarks>
    /// <param name="center">Center position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Render() {
    ///     RB.DrawEllipseInvertedFill(new Vector2i(100, 50), new Vector2i(40, 20), Color.white));
    /// }
    /// </code>
    /// <seealso cref="RB.DrawEllipseInvertedFill"/>
    /// <seealso cref="RB.DrawEllipse"/>
    public static void DrawEllipseInvertedFill(Vector2i center, Vector2i radius, Color32 color)
    {
        if (radius.x < 0 || radius.y < 0)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.DrawEllipseFill(center.x, center.y, radius.x, radius.y, color, true);
    }

    /// <summary>
    /// Draw the given map layer to the display.
    /// </summary>
    /// <remarks>
    /// Draws the given map *layer* to the display. The layer is drawn such that tile at position (0, 0) is aligned with the top left corner of the display by default.
    /// The optional *pos* parameter can be used to shift the tilemap in any direction.
    ///
    /// <see cref="RB.DrawMapLayer"/> also responds to the current camera setting as set by <see cref="RB.CameraSet"/>.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Camera</seedoc>
    /// </remarks>
    /// <param name="layer">Layer number to draw</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    /// const int LAYER_WALLS = 1;
    ///
    /// void Render() {
    ///     // Move the camera to the right
    ///     RB.CameraSet(new Vector2i(64, 0));
    ///
    ///     // Draw two map layers, which will be shifted by 64 pixels due to the
    ///     // camera setting above
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///     RB.DrawMapLayer(LAYER_WALLS);
    ///
    ///     // Draw a sprite which will also be shifted by the camera setting along
    ///     // with the tilemap
    ///     RB.DrawSprite("hero", new Vector2i(32, 32));
    /// }
    /// </code>
    /// <seealso cref="RB.CameraSet"/>
    public static void DrawMapLayer(int layer)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("DrawMapLayer invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.DrawMapLayer(layer, new Vector2i(0, 0));
    }

    /// <summary>
    /// Draw the given map layer to the display with an offset.
    /// </summary>
    /// <param name="layer">Layer number to draw</param>
    /// <param name="pos">Offset</param>
    public static void DrawMapLayer(int layer, Vector2i pos)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("DrawMapLayer invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.DrawMapLayer(layer, pos);
    }

    /// <summary>
    /// Set the alpha transparency value for drawing
    /// </summary>
    /// <remarks>
    /// Set the alpha transparency value for drawing. This transparency will remain in effect until a different value is set.
    /// <seedoc>Features:Alpha Transparency</seedoc>
    /// </remarks>
    /// <param name="a">A value between 0 (invisible) to 255 (solid)</param>
    /// <code>
    /// void Render()
    /// {
    ///     RB.Clear(new Color32(96, 96, 96, 255));
    ///
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     // Draw the next sprite at 50% transparency
    ///     RB.AlphaSet(127);
    ///     RB.DrawSprite("ghost", new Vector2i(100, 0));
    /// }
    /// </code>
    /// <seealso cref="RB.AlphaGet"/>
    public static void AlphaSet(byte a)
    {
        mInstance.mRetroBlitAPI.Renderer.AlphaSet(a);
    }

    /// <summary>
    /// Get the current alpha transparency value
    /// </summary>
    /// <remarks>
    /// Get the current alpha transparency value.
    /// <seedoc>Features:Alpha Transparency</seedoc>
    /// </remarks>
    /// <returns>Transparency</returns>
    /// <code>
    /// void Render()
    /// {
    ///     // Store current alpha
    ///     var prevAlpha = RB.AlphaGet();
    ///
    ///     RB.AlphaSet(127);
    ///     RB.DrawSprite("ghost", new Vector2i(0, 0));
    ///
    ///     // Restore previous alpha
    ///     RB.AlphaSet(prevAlpha);
    /// }
    /// </code>
    /// <seealso cref="RB.AlphaSet"/>
    public static byte AlphaGet()
    {
        return mInstance.mRetroBlitAPI.Renderer.AlphaGet();
    }

    /// <summary>
    /// Set the tint color for drawing
    /// </summary>
    /// <remarks>
    /// Set the tint color for drawing. A color tint has a similar effect to placing colored glass in front of a camera.
    /// This effect has many uses, such as coloring a character green to indicate that they are poisoned.
    ///
    /// The tint color will remain in effect until a different value is set.
    /// <seedoc>Features:Color Tinting</seedoc>
    /// </remarks>
    /// <param name="tintColor">Tint color</param>
    /// <code>
    /// void Render()
    /// {
    ///     var oldTint = RB.TintColorGet();
    ///
    ///     // Draw the the hero with a green tint if poisoned
    ///     if (poisoned) {
    ///         RB.TintColorSet(Color.green);
    ///     }
    ///
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     RB.TintColorSet(oldTint);
    /// }
    /// </code>
    /// <seealso cref="RB.TintColorGet"/>
    public static void TintColorSet(Color32 tintColor)
    {
        mInstance.mRetroBlitAPI.Renderer.TintColorSet(tintColor);
    }

    /// <summary>
    /// Get the current tint color.
    /// </summary>
    /// <remarks>
    /// Get the current tint color value.
    /// <seedoc>Features:Color Tinting</seedoc>
    /// </remarks>
    /// <returns>Tint color</returns>
    /// <code>
    /// void Render()
    /// {
    ///     var oldTint = RB.TintColorGet();
    ///
    ///     // Draw the the hero with a green tint if poisoned
    ///     if (poisoned) {
    ///         RB.TintColorSet(Color.green);
    ///     }
    ///
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     RB.TintColorSet(oldTint);
    /// }
    /// </code>
    /// <seealso cref="RB.TintColorSet"/>
    public static Color32 TintColorGet()
    {
        return mInstance.mRetroBlitAPI.Renderer.TintColorGet();
    }

    /// <summary>
    /// Set the current camera position
    /// </summary>
    /// <remarks>
    /// Set the current camera position which will offset all drawing that follows. This is especially useful for games
    /// that have a scrolling play field.
    /// <seedoc>Features:Camera</seedoc>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Render()
    /// {
    ///     var oldCameraPos = RB.CameraGet();
    ///
    ///     RB.CameraSet(worldPos);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     RB.CameraSet(oldCameraPos);
    /// }
    /// </code>
    /// <seealso cref="RB.CameraGet"/>
    /// <seealso cref="RB.CameraReset"/>
    public static void CameraSet(Vector2i pos)
    {
        mInstance.mRetroBlitAPI.Renderer.CameraSet(pos);
    }

    /// <summary>
    /// Reset the camera position back to 0, 0. This is equivalent to <see cref="RB.CameraSet"/>(0, 0).
    /// </summary>
    /// <remarks>
    /// A simple convenience function for resetting the camera back to (0, 0) position. This is equivalent to <see cref="RB.CameraSet"/>(0, 0).
    /// <seedoc>Features:Camera</seedoc>
    /// </remarks>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Render()
    /// {
    ///     RB.CameraSet(worldPos);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     RB.CameraReset();
    /// }
    /// </code>
    /// <seealso cref="RB.CameraSet"/>
    /// <seealso cref="RB.CameraGet"/>
    public static void CameraReset()
    {
        mInstance.mRetroBlitAPI.Renderer.CameraSet(Vector2i.zero);
    }

    /// <summary>
    /// Get the current camera position
    /// </summary>
    /// <remarks>
    /// Get the current camera position.
    /// <seedoc>Features:Camera</seedoc>
    /// </remarks>
    /// <returns>Camera position</returns>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Render()
    /// {
    ///     var oldCameraPos = RB.CameraGet();
    ///
    ///     RB.CameraSet(worldPos);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///     RB.DrawSprite("hero", new Vector2i(0, 0));
    ///
    ///     RB.CameraSet(oldCameraPos);
    /// }
    /// </code>
    /// <seealso cref="RB.CameraSet"/>
    /// <seealso cref="RB.CameraReset"/>
    public static Vector2i CameraGet()
    {
        return mInstance.mRetroBlitAPI.Renderer.CameraGet();
    }

    /// <summary>
    /// Set a rectangular clipping region
    /// </summary>
    /// <remarks>
    /// Set a rectangular clipping region that will affect all drawing that follows. Everything outside of the clipping region will not be drawn.
    ///
    /// By default the clipping region covers the entire display, which means that nothing is clipped.
    /// <seedoc>Features:Clip Region</seedoc>
    /// </remarks>
    /// <param name="rect">Rectangular clip region</param>
    /// <code>
    /// const int LAYER_TERRAIN_MINIMAP = 0;
    ///
    /// void Render()
    /// {
    ///     var oldClipRegion = RB.ClipGet();
    ///
    ///     // Use clip region to render a minimap in the top left corner of the screen
    ///     RB.ClipSet(new Vector2i(0, 0, 128, 128);
    ///     RB.DrawMapLayer(LAYER_TERRAIN_MINIMAP);
    ///
    ///     RB.ClipSet(oldClipRegion);
    /// }
    /// </code>
    /// <seealso cref="RB.ClipGet"/>
    /// <seealso cref="RB.ClipReset"/>
    /// <seealso cref="RB.ClipDebugEnable"/>
    /// <seealso cref="RB.ClipDebugDisable"/>
    public static void ClipSet(Rect2i rect)
    {
        if (rect.width < 0 || rect.height < 0)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.ClipSet(rect);
    }

    /// <summary>
    /// Reset the clipping region
    /// </summary>
    /// <remarks>
    /// A simple convenience function that sets the clipping region to cover the entire display, which means that nothing will be clipped.
    /// <seedoc>Features:Clip Region</seedoc>
    /// </remarks>
    /// <code>
    /// const int LAYER_TERRAIN_MINIMAP = 0;
    ///
    /// void Render()
    /// {
    ///     // Use clip region to render a minimap in the top left corner of the screen
    ///     RB.ClipSet(new Vector2i(0, 0, 128, 128);
    ///     RB.DrawMapLayer(LAYER_TERRAIN_MINIMAP);
    ///
    ///     RB.ClipReset();
    /// }
    /// </code>
    /// <seealso cref="RB.ClipSet"/>
    /// <seealso cref="RB.ClipGet"/>
    /// <seealso cref="RB.ClipDebugEnable"/>
    /// <seealso cref="RB.ClipDebugDisable"/>
    public static void ClipReset()
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;
        var renderTarget = renderer.CurrentRenderTexture();
        if (renderTarget != null)
        {
            renderer.ClipSet(new Rect2i(0, 0, renderTarget.width, renderTarget.height));
        }
    }

    /// <summary>
    /// Get the current clipping region.
    /// </summary>
    /// <remarks>
    /// Get the current clipping region. If a custom clipping region was not set then this will return the default clip region, which is a rect that covers the entire display.
    /// <seedoc>Features:Clip Region</seedoc>
    /// </remarks>
    /// <returns>Clipping region</returns>
    /// <code>
    /// const int LAYER_TERRAIN_MINIMAP = 0;
    ///
    /// void Render()
    /// {
    ///     var oldClipRegion = RB.ClipGet();
    ///
    ///     // Use clip region to render a minimap in the top left corner of the screen
    ///     RB.ClipSet(new Vector2i(0, 0, 128, 128);
    ///     RB.DrawMapLayer(LAYER_TERRAIN_MINIMAP);
    ///
    ///     RB.ClipSet(oldClipRegion);
    /// }
    /// </code>
    /// <seealso cref="RB.ClipSet"/>
    /// <seealso cref="RB.ClipReset"/>
    /// <seealso cref="RB.ClipDebugEnable"/>
    /// <seealso cref="RB.ClipDebugDisable"/>
    public static Rect2i ClipGet()
    {
        return mInstance.mRetroBlitAPI.Renderer.ClipGet();
    }

    /// <summary>
    /// Enable clip region debugging
    /// </summary>
    /// <remarks>
    /// Clip region debugging draws rectangles around the clip regions. The given *color* will apply to all following calls to <see cref="RB.ClipSet"/>.
    /// <see cref="RB.ClipDebugEnable"/> can be called multiple times to change the color of different clip regions.
    ///
    /// Clip region debugging can sometimes be helpful in figuring out why draw calls are being clipped!
    /// <seedoc>Features:Clip Region Debugging</seedoc>
    /// </remarks>
    /// <param name="color">RGBA color</param>
    /// <code>
    /// const int LAYER_TERRAIN_MINIMAP = 0;
    ///
    /// void Update() {
    ///     if (RB.KeyPressed(KeyCode.F12)) {
    ///         RB.ClipDebugEnable(Color.red);
    ///     } else if (RB.KeyPressed(KeyCode.F11)) {
    ///         RB.ClipDebugDisable();
    ///     }
    /// }
    ///
    /// void Render()
    /// {
    ///     var oldClipRegion = RB.ClipGet();
    ///
    ///     // Use clip region to render a minimap in the top left corner of the screen
    ///     RB.ClipSet(new Vector2i(0, 0, 128, 128);
    ///     RB.DrawMapLayer(LAYER_TERRAIN_MINIMAP);
    ///
    ///     RB.ClipSet(oldClipRegion);
    /// }
    /// </code>
    /// <seealso cref="RB.ClipDebugDisable"/>
    /// <seealso cref="RB.ClipSet"/>
    /// <seealso cref="RB.ClipGet"/>
    /// <seealso cref="RB.ClipReset"/>
    public static void ClipDebugEnable(Color32 color)
    {
        mInstance.mRetroBlitAPI.Renderer.ClipDebugSet(true, color);
    }

    /// <summary>
    /// Disable clip region debugging
    /// </summary>
    /// <remarks>
    /// Disable clip region debugging, clip regions will no longer be drawn.
    /// <seedoc>Features:Clip Region Debugging</seedoc>
    /// </remarks>
    /// <code>
    /// const int LAYER_TERRAIN_MINIMAP = 0;
    ///
    /// void Update() {
    ///     if (RB.KeyPressed(KeyCode.F12)) {
    ///         RB.ClipDebugEnable(Color.red);
    ///     } else if (RB.KeyPressed(KeyCode.F11)) {
    ///         RB.ClipDebugDisable();
    ///     }
    /// }
    ///
    /// void Render()
    /// {
    ///     var oldClipRegion = RB.ClipGet();
    ///
    ///     // Use clip region to render a minimap in the top left corner of the screen
    ///     RB.ClipSet(new Vector2i(0, 0, 128, 128);
    ///     RB.DrawMapLayer(LAYER_TERRAIN_MINIMAP);
    ///
    ///     RB.ClipSet(oldClipRegion);
    /// }
    /// </code>
    /// <seealso cref="RB.ClipDebugEnable"/>
    /// <seealso cref="RB.ClipSet"/>
    /// <seealso cref="RB.ClipGet"/>
    /// <seealso cref="RB.ClipReset"/>
    public static void ClipDebugDisable()
    {
        mInstance.mRetroBlitAPI.Renderer.ClipDebugSet(false, Color.white);
    }

    /// <summary>
    /// Enable a batch debugging overlay which shows how many draw batches are being used
    /// </summary>
    /// <remarks>
    /// RetroBlit batches as many drawing operations as it can before flushing them out to the GPU. The fewer the batches the faster
    /// the game will run. A batch can be flushed for a variety of reasons, such as:
    /// <list type="bullet">
    /// <item>Batch is full</item>
    /// <item>Spritesheet has changed</item>
    /// <item>Tilemap chunk drawn</item>
    /// <item>End of frame</item>
    /// <item>Clip region has changed</item>
    /// <item>Offscreen target has changed</item>
    /// <item>Post processing effect applied</item>
    /// <item>Shader changed</item>
    /// <item>Shader reset</item>
    /// <item>RetroBlit had to internally change materials</item>
    /// <item>RetroBlit had to internally change texture</item>
    /// </list>
    /// The batch counts displayed by <see cref="RB.BatchDebugEnable"/> may assist in identifying inefficiencies, and in better
    /// grouping operations to cause the least amount of batches.
    /// </remarks>
    /// <param name="fontColor">Font color to use</param>
    /// <param name="backgroundColor">Background color to use</param>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyPressed(KeyCode.F12)) {
    ///         RB.BatchDebugEnable(Color.white, Color.black);
    ///     } else if (RB.KeyPressed(KeyCode.F11)) {
    ///         RB.BatchDebugDisable();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.BatchDebugDisable"/>
    public static void BatchDebugEnable(Color32 fontColor, Color32 backgroundColor)
    {
        mInstance.mRetroBlitAPI.Renderer.FlashDebugSet(true, fontColor, backgroundColor);
    }

    /// <summary>
    /// Disable batch debugging overlay
    /// </summary>
    /// <remarks>
    /// Disable batch debugging overlay.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyPressed(KeyCode.F12)) {
    ///         RB.BatchDebugEnable(Color.white, Color.black);
    ///     } else if (RB.KeyPressed(KeyCode.F11)) {
    ///         RB.BatchDebugDisable();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.BatchDebugEnable"/>
    public static void BatchDebugDisable()
    {
        mInstance.mRetroBlitAPI.Renderer.FlashDebugSet(false, Color.white, Color.black);
    }

    /// <summary>
    /// Switch rendering target to a sprite sheet
    /// </summary>
    /// <remarks>
    /// Switch rendering target to a sprite sheet given by *spriteSheetIndex*. All rendering that follows will be done on the
    /// sprite sheet until end of frame, or until <see cref="RB.Offscreen"/> or <see cref="RB.Onscreen"/> is called.
    ///
    /// The target sprite sheet could be any sprite sheet, including one loaded from an image, a sprite pack, or a blank sprite sheet.
    ///
    /// The results can then be copied to the display by targeting the display again with <see cref="RB.Onscreen"/>, and copying
    /// from the previously targeted sprite sheet with <see cref="RB.DrawCopy"/>.
    ///
    /// This is a very powerful feature of RetroBlit, it can be used for numerous effects, and mixed with custom shaders.
    /// <seedoc>Features:Drawing into a Sprite Sheet</seedoc>
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="spriteSheet">SpriteSheet to switch to</param>
    /// <code>
    /// SpriteSheetAsset offscreenSurfaceA = new SpriteSheetAsset();
    /// SpriteSheetAsset offscreenSurfaceB = new SpriteSheetAsset();
    ///
    /// ShaderAsset shaderWater = new ShaderAsset();
    ///
    /// const int LAYER_TERRAIN = 0;
    /// const int LAYER_WATER_MASK = 0;
    ///
    /// void Initialize() {
    ///     // Create two blank sprite sheets the same size as the display
    ///     offscreenSurfaceA.Create(RB.DisplaySize);
    ///     offscreenSurfaceB.Create(RB.DisplaySize);
    ///
    ///     // Load terrain, and setup shader
    ///     // -- snip --
    /// }
    ///
    /// void Render() {
    ///     // Draw terrain into offscreen A
    ///     RB.Offscreen(offscreenSurfaceA);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///
    ///     // Draw a water mask into offscreen B
    ///     RB.Offscreen(offscreenSurfaceB);
    ///     RB.DrawMapLayer(LAYER_WATER_MASK);
    ///
    ///     // Switch back to drawing to display
    ///     RB.Onscreen();
    ///
    ///     // Use a custom shader that combines offscreen A and B to create a rippling water effect
    ///     RB.ShaderSet(shaderWater);
    ///     RB.SpriteSheetSet(offscreenSurfaceA);
    ///     shaderWater.SpriteSheetTextureSet(offscreenSurfaceB);
    ///
    ///     // Draw the results to display while the shader is active
    ///     RB.DrawCopy(new Rect2i(0, 0, offscreenSurfaceA.grid.width, offscreenSurfaceA.grid.height), Vector2i.zero);
    ///     RB.ShaderApplyNow();
    ///
    ///     RB.ShaderReset();
    /// }
    /// </code>
    /// <seealso cref="RB.Onscreen"/>
    /// <seealso cref="RB.ShaderSet"/>
    public static void Offscreen(SpriteSheetAsset spriteSheet)
    {
        var renderer = mInstance.mRetroBlitAPI.Renderer;

        if (spriteSheet == null)
        {
            Debug.LogError("Spritesheet is invalid!");
            return;
        }

        if (spriteSheet.status != AssetStatus.Ready)
        {
            RetroBlitInternal.RBUtil.LogErrorOnce("Sprite sheet is not loaded, or was deleted. Use SpriteSheetAsset.Load to create a sprite sheet surface.");
            return;
        }

        renderer.OffscreenTarget(spriteSheet);
    }

    /// <summary>
    /// Switch to drawing to the display
    /// </summary>
    /// <remarks>
    /// Switch rendering target back to the display. This is the default state of RetroBlit.
    /// <seedoc>Features:Drawing into a Sprite Sheet</seedoc>
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset offscreenSurfaceA = new SpriteSheetAsset();
    /// SpriteSheetAsset offscreenSurfaceB = new SpriteSheetAsset();
    ///
    /// ShaderAsset shaderWater = new ShaderAsset();
    ///
    /// const int LAYER_TERRAIN = 0;
    /// const int LAYER_WATER_MASK = 0;
    ///
    /// void Initialize() {
    ///     // Create two blank sprite sheets the same size as the display
    ///     offscreenSurfaceA.Create(RB.DisplaySize);
    ///     offscreenSurfaceB.Create(RB.DisplaySize);
    ///
    ///     // Load terrain, and setup shader
    ///     // -- snip --
    /// }
    ///
    /// void Render() {
    ///     // Draw terrain into offscreen A
    ///     RB.Offscreen(offscreenSurfaceA);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///
    ///     // Draw a water mask into offscreen B
    ///     RB.Offscreen(offscreenSurfaceB);
    ///     RB.DrawMapLayer(LAYER_WATER_MASK);
    ///
    ///     // Switch back to drawing to display
    ///     RB.Onscreen();
    ///
    ///     // Use a custom shader that combines offscreen A and B to create a rippling water effect
    ///     RB.ShaderSet(shaderWater);
    ///     RB.SpriteSheetSet(offscreenSurfaceA);
    ///     shaderWater.SpriteSheetTextureSet(offscreenSurfaceB);
    ///
    ///     // Draw the results to display while the shader is active
    ///     RB.DrawCopy(new Rect2i(0, 0, offscreenSurfaceA.grid.width, offscreenSurfaceA.grid.height), Vector2i.zero);
    ///     RB.ShaderApplyNow();
    ///
    ///     RB.ShaderReset();
    /// }
    /// </code>
    /// <seealso cref="RB.Offscreen"/>
    /// <seealso cref="RB.ShaderSet"/>
    public static void Onscreen()
    {
        mInstance.mRetroBlitAPI.Renderer.Onscreen();
    }

    /// <summary>
    /// Set the tilemap sprite at the given layer and tile position, with optional flags.
    /// </summary>
    /// <remarks>
    /// Set the tilemap sprite at the given layer and tile position, with optional *flags* and *tintColor*. Note that
    /// in a tilemap layer all tiles are of the same size, which is inherited from the sprite size of the sprite sheet
    /// assigned to the layer with <see cref="RB.MapLayerSpriteSheetSet"/>.
    ///
    /// The given *flags* correspond to the same flags as used when drawing sprites with <see cref="RB.DrawSprite"/>.
    /// This allows for flipping and rotating tiles.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Setting or Getting Tile Info</seedoc>
    /// <seedoc>Features:Changing Layer Sprite Sheet</seedoc>
    /// </remarks>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Sprite index</param>
    /// <param name="flags">Sprite flags</param>
    /// <code>
    /// SpriteSheetAsset spriteTerrain = new SpriteSheetAsset();
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Initialize() {
    ///     int width = 16;
    ///     int height = 16;
    ///
    ///     RB.MapLayerSpriteSheetSet(LAYER_TERRAIN, spriteTerrain);
    ///
    ///     // Generate tiles for square island
    ///     for (int x = 0; x < width; x++) {
    ///         for (int y = 0; y < height; y++) {
    ///             bool onEdge = x == 0 || y == 0 || x == (width - 1) || y == (height - 1);
    ///
    ///             string sprite = "grass";
    ///             if (onEdge) {
    ///                 sprite = "water";
    ///             }
    ///
    ///             RB.MapSpriteSet(LAYER_TERRAIN, new Vector2i(x, y), sprite);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.MapSpriteGet"/>
    /// <seealso cref="RB.MapLayerSpriteSheetSet"/>
    public static void MapSpriteSet(int layer, Vector2i tilePos, int sprite, int flags = 0)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("MapSpriteSet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, sprite, Color.white, flags);
    }

    /// <summary>
    /// Set the tilemap sprite index at the given tile position, with color tint and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Sprite index</param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, int sprite, Color32 tintColor, int flags = 0)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("MapSpriteSet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, sprite, tintColor, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteName">Packed sprite name</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, string spriteName, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, RB.PackedSpriteID(spriteName), Color.white, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with color tint and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteName">Packed sprite name</param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, string spriteName, Color32 tintColor, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, RB.PackedSpriteID(spriteName), tintColor, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteName">Packed sprite name</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, FastString spriteName, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, RB.PackedSpriteID(spriteName), Color.white, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with color tint and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteName">Packed sprite name</param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, FastString spriteName, Color32 tintColor, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, RB.PackedSpriteID(spriteName), tintColor, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteID">Packed Sprite ID from <see cref="RB.PackedSpriteID"/></param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, PackedSpriteID spriteID, int flags = 0)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("MapSpriteSet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, spriteID.id, Color.white, flags | RetroBlitInternal.RBTilemapTMX.SPRITEPACK);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with color tint and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="spriteID">Packed Sprite ID from <see cref="RB.PackedSpriteID"/></param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, PackedSpriteID spriteID, Color32 tintColor, int flags = 0)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("MapSpriteSet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.SpriteSet(layer, tilePos.x, tilePos.y, spriteID.id, tintColor, flags | RetroBlitInternal.RBTilemapTMX.SPRITEPACK);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Packed sprite from <see cref="RB.PackedSprite"/></param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, PackedSprite sprite, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, sprite.id, Color.white, flags);
    }

    /// <summary>
    /// Set the tilemap sprite id (from a sprite pack) at the given tile position, with color tint and optional flags.
    /// </summary>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="sprite">Packed sprite from <see cref="RB.PackedSprite"/></param>
    /// <param name="tintColor">Tint color</param>
    /// <param name="flags">Sprite flags</param>
    public static void MapSpriteSet(int layer, Vector2i tilePos, PackedSprite sprite, Color32 tintColor, int flags = 0)
    {
        MapSpriteSet(layer, tilePos, sprite.id, tintColor, flags);
    }

    /// <summary>
    /// Get the tilemap sprite at the given layer and tile position
    /// </summary>
    /// <remarks>
    /// Get the tilemap sprite at the given layer and tile position. If the the sprite sheet set with <see cref="RB.MapLayerSpriteSheetSet"/>
    /// is from an image grid then the sprite corresponds to the sprite index in the sprite sheet. If the sprite sheet represents a sprite pack
    /// then sprite corresponds to the packed sprite id.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Setting or Getting Tile Info</seedoc>
    /// </remarks>
    /// <param name="layer">Map layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Update() {
    ///     int tile = RB.MapSpriteGet(LAYER_TERRAIN, playerTilePos);
    ///
    ///     if (tile == RB.PackedSpriteID("fire").id) {
    ///         playerHealth -= 1;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.MapSpriteSet"/>
    /// <seealso cref="RB.MapLayerSpriteSheetSet"/>
    /// <returns>Sprite index or id</returns>
    public static int MapSpriteGet(int layer, Vector2i tilePos)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            Debug.LogError("MapSpriteGet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return RB.SPRITE_INVALID;
        }

        return mInstance.mRetroBlitAPI.Tilemap.SpriteGet(layer, tilePos.x, tilePos.y);
    }

    /// <summary>
    /// Set user tile data at the given layer and tile position
    /// </summary>
    /// <remarks>
    /// Set user tile data at the given *layer* and *tilePos*. This data could be anything, including simple primitive type, or structs. This can be very useful
    /// for storing information about the features of a tile.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Setting or Getting Tile Info</seedoc>
    /// </remarks>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="layer">Tilemap layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <param name="data">Data</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Initialize() {
    ///     int width = 16;
    ///     int height = 16;
    ///
    ///     for (int x = 0; x < 16; x++) {
    ///         for (int x = 0; x < 16; x++) {
    ///             blocking = Random.Range(0, 2) == 0 ? false : true;
    ///
    ///             RB.MapDataSet<bool>(LAYER_TERRAIN, new Vector2i(x, y), blocking);
    ///         }
    ///     }
    /// }
    ///
    /// void Update() {
    ///     bool blocking = RB.MapDataGet<bool>(LAYER_TERRAIN, playerTilePos);
    ///
    ///     if (blocking) {
    ///         KillPlayer();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.MapDataGet<T>"/>
    /// <seealso cref="RB.MapLayerSpriteSheetSet"/>
    public static void MapDataSet<T>(int layer, Vector2i tilePos, T data)
    {
        mInstance.mRetroBlitAPI.Tilemap.DataSet(layer, tilePos.x, tilePos.y, data);
    }

    /// <summary>
    /// Get user tile data at the given layer and tile position
    /// </summary>
    /// <remarks>
    /// Get user tile data at the given *layer* and *tilePos*, which was previously set with <see cref="RB.MapDataSet<T>"/>.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Setting or Getting Tile Info</seedoc>
    /// </remarks>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="layer">Tilemap layer</param>
    /// <param name="tilePos">Tile position</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// void Initialize() {
    ///     int width = 16;
    ///     int height = 16;
    ///
    ///     for (int x = 0; x < 16; x++) {
    ///         for (int x = 0; x < 16; x++) {
    ///             blocking = Random.Range(0, 2) == 0 ? false : true;
    ///
    ///             RB.MapDataSet<bool>(LAYER_TERRAIN, new Vector2i(x, y), blocking);
    ///         }
    ///     }
    /// }
    ///
    /// void Update() {
    ///     bool blocking = RB.MapDataGet<bool>(LAYER_TERRAIN, playerTilePos);
    ///
    ///     if (blocking) {
    ///         KillPlayer();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.MapDataGet<T>"/>
    /// <seealso cref="RB.MapLayerSpriteSheetSet"/>
    /// <returns>Data</returns>
    public static T MapDataGet<T>(int layer, Vector2i tilePos)
    {
        object data = mInstance.mRetroBlitAPI.Tilemap.DataGet<object>(layer, tilePos.x, tilePos.y);
        if (data != null)
        {
            return (T)data;
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// Clear the tilemap
    /// </summary>
    /// <remarks>
    /// Clear tilemap, on all layers, or only on the specified *layer*. This removes all tile information, and all user data associated with the tiles.
    /// </remarks>
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <code>
    /// void Update() {
    ///     if (gameOver) {
    ///         RB.MapClear();
    ///     }
    /// }
    /// </code>
    public static void MapClear()
    {
        mInstance.mRetroBlitAPI.Tilemap.Clear(-1);
    }

    /// <summary>
    /// Clear only the given tilemap layer.
    /// </summary>
    /// <param name="layer">Layer to clear</param>
    public static void MapClear(int layer)
    {
        if (layer < 0 || layer >= RB.MapLayers)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.Clear(layer);
    }

    /// <summary>
    /// Set the sprite sheet to be used by the given tilemap layer.
    /// </summary>
    /// <remarks>
    /// Set the sprite sheet to be used by the given tilemap layer. Any sprite can be used, including a sprite pack based sprite sheet.
    /// The tilemap will inherit the size of each tile from the sprite sheets sprite size as returned by <see cref="SpriteSheetAsset.grid"/>.
    /// Each layer in the tilemap can have a different sprite sheet, this also means that each layer could have different tile sizes!
    ///
    /// If using a sprite pack based sprite sheet the tiles in the tilemap should be set with sprite ids from the <see cref="RB.PackedSpriteID"/>
    /// class.
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// <seedoc>Features:Tilemaps</seedoc>
    /// </remarks>
    /// <param name="mapLayer">Map layer</param>
    /// <param name="spriteSheet">Sprite sheet to use</param>
    /// <code>
    /// SpriteSheetAsset spriteTerrain = new SpriteSheetAsset();
    /// SpriteSheetAsset spriteMinimap = new SpriteSheetAsset();
    /// const int LAYER_TERRAIN = 0;
    /// const int LAYER_MINIMAP = 1;
    ///
    /// void Initialize() {
    ///     // Load a sprite sheet from a sprite pack
    ///     spriteTerrain.Load("spritepacks/terrain");
    ///     spriteTerrain.grid = new Vector2i(32, 32);
    ///
    ///     // Load a sprite sheet from an image
    ///     spriteMinimap.Load(spriteMinimap, "spritesheets/minimap");
    ///     spriteMinimap.grid = new Vector2i(2, 2);
    ///
    ///     // Set terrain tilemap layer to use the terrain sprite sheet.
    ///     // This layer will have tile sizes of 32x32 pixels.
    ///     RB.MapLayerSpriteSheetSet(LAYER_TERRAIN, spriteTerrain);
    ///
    ///     // Set minimap tilemap layer to use the minimap sprite sheet.
    ///     // This layer will have tile sizes of 2x2 pixels.
    ///     RB.MapLayerSpriteSheetSet(LAYER_MINIMAP, spriteMinimap);
    /// }
    ///
    /// void Render() {
    ///     // Draw the terrain
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///
    ///     // Use clip region to draw a small minimap in the upper right corner of the screen
    ///     RB.ClipSet(new Rect2i(RB.DisplaySize.width - 64, 0, 64, 64));
    ///     RB.DrawMapLayer(LAYER_MINIMAP, new Vector2i(RB.DisplaySize.width - 64, 0));
    ///
    ///     RB.ClipReset();
    /// }
    /// </code>
    /// <seealso cref="SpriteSheetAsset"/>
    public static void MapLayerSpriteSheetSet(int mapLayer, SpriteSheetAsset spriteSheet)
    {
        if (mapLayer < 0 || mapLayer >= RB.MapLayers)
        {
            RetroBlitInternal.RBUtil.LogErrorOnce("MapLayerSpriteSheetSet invalid map layer, you can request more layers in IRetroBlitGame.QueryHardware");
            return;
        }

        mInstance.mRetroBlitAPI.Tilemap.MapLayerSpriteSheetSet(mapLayer, spriteSheet);
    }

    /// <summary>
    /// Check if a map chunk at the given position is empty/unloaded
    /// </summary>
    /// <remarks>
    /// Check if a map chunk at the given position is empty/unloaded. A chunk can become empty if its been shifted out by
    /// <see cref="RB.MapShiftChunks"/>, or has been released internally because it has not been visible
    /// for some time.
    ///
    /// This method is especially useful when handling infinite maps and dynamically loading map chunks as the player moves
    /// around the world.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    /// <param name="layer">Chunk layer</param>
    /// <param name="pos">Offset of the chunk in tile coordinates</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    /// TMXMap tmxMap;
    ///
    /// Vector2i cameraPos
    ///
    /// // Track the top-left-most currently loaded chunk
    /// Vector2i topLeftChunk;
    ///
    /// void Initialize() {
    ///     tmxMap.Load("tilemaps/world");
    /// }
    ///
    /// void Render() {
    ///     // Calculate the size of a single map chunk
    ///     var chunkPixelSize = new Vector2i(
    ///         RB.MapChunkSize.width * RB.SpriteSize(0).width,
    ///         RB.MapChunkSize.height * RB.SpriteSize(0).height);
    ///
    ///     // Figure out which map chunk is in the top left corner of the camera view
    ///     var newTopLeftChunk = new Vector2i(
    ///         cameraPos.x / chunkPixelSize.width,
    ///         cameraPos.y / chunkPixelSize.height);
    ///
    ///     if (newTopLeftChunk != topLeftChunk) {
    ///         // Calculate how much the chunks should be shifted
    ///         var shift = topLeftChunk - newTopLeftChunk;
    ///         RB.MapShiftChunks(0, shift);
    ///
    ///         // Iterate through all potentially visible chunks. If any are currently empty
    ///         // then load them
    ///         for (int cy = 0; cy <= (RB.DisplaySize.height / chunkPixelSize.height) + 1; cy++) {
    ///             for (int cx = 0; cx <= (RB.DisplaySize.width / chunkPixelSize.width) + 1; cx++) {
    ///                 var chunkPos = new Vector2i(cx * RB.MapChunkSize.x, cy * RB.MapChunkSize.y);
    ///                 var mapPos = new Vector2i(
    ///                     newTopLeftChunk.x * RB.MapChunkSize.x,
    ///                     newTopLeftChunk.y * RB.MapChunkSize.y) + chunkPos;
    ///                 mapPos.x = mapPos.x % tmxMap.size.width;
    ///
    ///                 if (RB.MapChunkEmpty(LAYER_TERRAIN, chunkPos)) {
    ///                     tmxMap.LoadLayerChunk("Terrain", LAYER_TERRAIN, mapPos, chunkPos);
    ///                 }
    ///             }
    ///         }
    ///
    ///         topLeftChunk = newTopLeftChunk;
    ///     }
    ///
    ///     // Calculate the new camera position
    ///     var newCameraPos = new Vector2i(
    ///         cameraPos.x % chunkPixelSize.width,
    ///         cameraPos.y % chunkPixelSize.height);
    ///
    ///     cameraPos = newCameraPos;
    ///
    ///     // Update the camera position before drawing
    ///     RB.CameraSet(cameraPos);
    ///     RB.DrawMapLayer(0, new Vector2i(x + 1, y + 1));
    ///
    ///     RB.CameraReset();
    /// }
    /// </code>
    /// <returns>True if empty, false otherwise</returns>
    /// <seealso cref="TMXMapAsset"/>
    /// <seealso cref="RB.MapShiftChunks"/>
    public static bool MapChunkEmpty(int layer, Vector2i pos)
    {
        return mInstance.mRetroBlitAPI.Tilemap.ChunkEmptyGet(layer, pos);
    }

    /// <summary>
    /// Shift the tilemap chunks by the given amount chunks in the X and Y direction
    /// </summary>
    /// <remarks>
    /// Shift the tilemap chunks by the given amount chunks in the X and Y direction. This method is particularly useful
    /// while loading infinite maps chunk by chunk.
    ///
    /// When handling infinite maps it's wise to only load as many chunks as needed. When the player moves outside
    /// of the currently loaded chunks new chunks can be loaded with <see cref="TMXMapAsset.LoadLayerChunk"/>. However,
    /// in the usual case when new chunks are needed many of the existing chunks still apply, and should not need
    /// to be reloaded. To help with this the <see cref="RB.MapShiftChunks"/> can be used to shift chunks by any amount
    /// in any direction. This is similar to shifting a 2D array.
    ///
    /// After shifting some chunks will be cleared. Which chunks get cleared can be determined algorithmically, or
    /// <see cref="RB.MapChunkEmpty"/> can be called on visible chunks to see if they've become empty and should be
    /// backfilled with newly loaded chunks.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    /// <param name="layer">Map layer</param>
    /// <param name="shiftChunks">Shift amount in chunk numbers in the x and y direction.</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    /// TMXMap tmxMap;
    ///
    /// Vector2i cameraPos
    ///
    /// // Track the top-left-most currently loaded chunk
    /// Vector2i topLeftChunk;
    ///
    /// void Initialize() {
    ///     tmxMap.Load("tilemaps/world");
    /// }
    ///
    /// void Render() {
    ///     // Calculate the size of a single map chunk
    ///     var chunkPixelSize = new Vector2i(
    ///         RB.MapChunkSize.width * RB.SpriteSize(0).width,
    ///         RB.MapChunkSize.height * RB.SpriteSize(0).height);
    ///
    ///     // Figure out which map chunk is in the top left corner of the camera view
    ///     var newTopLeftChunk = new Vector2i(
    ///         cameraPos.x / chunkPixelSize.width,
    ///         cameraPos.y / chunkPixelSize.height);
    ///
    ///     if (newTopLeftChunk != topLeftChunk) {
    ///         // Calculate how much the chunks should be shifted
    ///         var shift = topLeftChunk - newTopLeftChunk;
    ///         RB.MapShiftChunks(0, shift);
    ///
    ///         // Iterate through all potentially visible chunks. If any are currently empty
    ///         // then load them
    ///         for (int cy = 0; cy <= (RB.DisplaySize.height / chunkPixelSize.height) + 1; cy++) {
    ///             for (int cx = 0; cx <= (RB.DisplaySize.width / chunkPixelSize.width) + 1; cx++) {
    ///                 var chunkPos = new Vector2i(cx * RB.MapChunkSize.x, cy * RB.MapChunkSize.y);
    ///                 var mapPos = new Vector2i(
    ///                     newTopLeftChunk.x * RB.MapChunkSize.x,
    ///                     newTopLeftChunk.y * RB.MapChunkSize.y) + chunkPos;
    ///                 mapPos.x = mapPos.x % tmxMap.size.width;
    ///
    ///                 if (RB.MapChunkEmpty(LAYER_TERRAIN, chunkPos)) {
    ///                     tmxMap.LoadLayerChunk("Terrain", LAYER_TERRAIN, mapPos, chunkPos);
    ///                 }
    ///             }
    ///         }
    ///
    ///         topLeftChunk = newTopLeftChunk;
    ///     }
    ///
    ///     // Calculate the new camera position
    ///     var newCameraPos = new Vector2i(
    ///         cameraPos.x % chunkPixelSize.width,
    ///         cameraPos.y % chunkPixelSize.height);
    ///
    ///     cameraPos = newCameraPos;
    ///
    ///     // Update the camera position before drawing
    ///     RB.CameraSet(cameraPos);
    ///     RB.DrawMapLayer(0, new Vector2i(x + 1, y + 1));
    ///
    ///     RB.CameraReset();
    /// }
    /// </code>
    /// <seealso cref="TMXMapAsset"/>
    /// <seealso cref="RB.MapChunkEmpty"/>
    public static void MapShiftChunks(int layer, Vector2i shiftChunks)
    {
        mInstance.mRetroBlitAPI.Tilemap.ShiftChunks(layer, shiftChunks);
    }

    /// <summary>
    /// Setup a FontAsset index table
    /// </summary>
    /// <remarks>
    /// Setup a FontAsset index table to be used with inline font changes when printing text.
    /// <seedoc>Features:Inline Font Changes</seedoc>
    /// </remarks>
    /// <code>
    /// public void Initialize()
    /// {
    ///     FontAsset[] fontIndices = new FontAsset[] { myFontSmall, myFontWide };
    ///     RB.FontInlineIndexSetup(fontIndices);
    /// }
    ///
    /// public void Render()
    /// {
    ///     RB.Print(new Vector2i(0, 0), Color.black, "Inline font @g01changes@g99 are great!");
    /// }
    /// </code>
    /// <param name="fonts">FontAsset array</param>
    /// <seealso cref="RB.Print"/>
    public static void FontInlineIndexSetup(FontAsset[] fonts)
    {
        mInstance.mRetroBlitAPI.Font.FontIndexSetup(fonts);
    }

    /// <summary>
    /// Setup a FontAsset index table
    /// </summary>
    /// <param name="fonts">FontAsset list</param>
    public static void FontInlineIndexSetup(List<FontAsset> fonts)
    {
        mInstance.mRetroBlitAPI.Font.FontIndexSetup(fonts);
    }

    /// <summary>
    /// Print text to the display
    /// </summary>
    /// <remarks>
    /// Print text to the display. The position of the text is specified by either *pos* or *rect*. When specifying position by
    /// *rect* the *flags* should also be specified to determine how the text should be aligned and clipped within the given rect.
    ///
    /// Multiple flags can be combined together for the desired effect. The following text alignment flags are available:
    /// <list type="bullet">
    /// <item><see cref="RB.ALIGN_H_LEFT"/></item>
    /// <item><see cref="RB.ALIGN_H_CENTER"/></item>
    /// <item><see cref="RB.ALIGN_H_RIGHT"/></item>
    /// <item><see cref="RB.ALIGN_V_TOP"/></item>
    /// <item><see cref="RB.ALIGN_V_CENTER"/></item>
    /// <item><see cref="RB.ALIGN_V_BOTTOM"/></item>
    /// </list>
    ///
    /// The following clipping flags are available:
    /// <list type="bullet">
    /// <item><see cref="RB.TEXT_OVERFLOW_CLIP"/></item>
    /// <item><see cref="RB.TEXT_OVERFLOW_WRAP"/></item>
    /// </list>
    ///
    /// And these miscellaneous flags:
    /// <list type="bullet">
    /// <item><see cref="RB.NO_INLINE_COLOR"/></item>
    /// <item><see cref="RB.NO_ESCAPE_CODES"/></item>
    /// </list>
    ///
    /// The font used is specified with *font* which corresponds to the font created with <see cref="FontAsset.Setup"/>. If no font is specified the
    /// RetroBlit built-in system font is used instead.
    ///
    /// The color of the text is specified with the *color* parameter. This is only the starting color which can further be changed inline with the text content.
    ///
    /// RetroBlit supports a variety of inline escape sequences to control the appearance of the text. The escape sequences are specified with the "@" character
    /// in the given *text* string. The following escape sequences are supported:
    ///
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font. The new font must have the same glyph height as the previous font.
    /// </item>
    /// </list>
    /// <seedoc>Features:Fonts</seedoc>
    /// <seedoc>SystemFont:Built-in System Font</seedoc>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    /// <code>
    /// void Render() {
    ///     // Text string with the "damage" number in red
    ///     var text = "Player hit for @FF0000" + damage + "@-!!!";
    ///
    ///     // Manually align the text to the upper right corner of the screen with a 4 pixel offset
    ///     var textSize = RB.PrintMeasure(text);
    ///     var textPos = new Vector2i(RB.DisplaySize.width - text.width - 4, 4);
    ///     RB.Print(textPos, Color.white, text);
    ///
    ///     // Automatically align text using a rect and alignment flags
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height),
    ///         Color.white,
    ///         RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER,
    ///         text);
    /// }
    /// </code>
    /// <seealso cref="FontAsset"/>
    /// <seealso cref="RB.PrintMeasure"/>
    /// <seealso cref="RB.FontInlineIndexSetup"/>
    public static void Print(Vector2i pos, Color32 color, string text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(pos.x, pos.y, 1, 1), color, 0, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Optional flag <see cref="RB.NO_INLINE_COLOR"/>, and <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(Vector2i pos, Color32 color, int flags, string text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(pos.x, pos.y, 1, 1), color, flags, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(Rect2i rect, Color32 color, int flags, string text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, rect, color, flags, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    public static void Print(Vector2i pos, Color32 color, FastString text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(pos.x, pos.y, 1, 1), color, 0, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(Vector2i pos, Color32 color, int flags, FastString text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(pos.x, pos.y, 1, 1), color, flags, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using system font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(Rect2i rect, Color32 color, int flags, FastString text)
    {
        if (text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(null, rect, color, flags, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Vector2i pos, Color32 color, string text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(pos.x, pos.y, 1, 1), color, 0, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Vector2i pos, Color32 color, int flags, string text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(pos.x, pos.y, 1, 1), color, flags, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Rect2i rect, Color32 color, int flags, string text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, rect, color, flags, mTextWrapString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Vector2i pos, Color32 color, FastString text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(pos.x, pos.y, 1, 1), color, 0, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="pos">Position</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Vector2i pos, Color32 color, int flags, FastString text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(pos.x, pos.y, 1, 1), color, flags, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Print text to display using custom font at the given position.
    /// </summary>
    /// <remarks>
    /// In the <paramref name="text"/> string the "@" character is an escape character, and it enables the following options:
    /// <list type="bullet">
    /// <item>
    /// "@@" - Print @
    /// </item>
    /// <item>
    /// "@######" - Color change in the format RRGGBB, where each color channel has a hex value between 00 and FF
    /// </item>
    /// <item>
    /// "@-" - Revert back to the color specified in the <paramref name="color"/> parameter.
    /// </item>
    /// <item>
    /// "@w###" - Apply a wavy text effect in the format APS (wave amplitude, period, and speed), where each
    /// parameter has a value between 0 and 9. Default is "@w000"
    /// </item>
    /// <item>
    /// "@s#" - Apply a shaky text effect in the format M (magnitude), where M has a value between 0 and 9.
    /// Default is "@s0"
    /// </item>
    /// <item>
    /// "@g##" - Change the font to the given 2 digit font index. Default is "@g99" which is a special value
    /// indicating built-in system font.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="font">Font asset</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="color">Color</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>,
    /// <see cref="RB.NO_INLINE_COLOR"/>, <see cref="cref="RB.NO_ESCAPE_CODES/></param>
    /// <param name="text">Text</param>
    public static void Print(FontAsset font, Rect2i rect, Color32 color, int flags, FastString text)
    {
        if (font == null || text == null)
        {
            return;
        }

        int width, height;
        mTextWrapFastString.Set(text);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        mInstance.mRetroBlitAPI.Font.Print(font, rect, color, flags, mTextWrapFastString, false, out width, out height);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    /// <summary>
    /// Measures a text string without printing it
    /// </summary>
    /// <remarks>
    /// Measures a text string without printing it, and returns a <see cref="Vector2i"/> representing the
    /// width and height of the text string. All alignment and clipping flags are taken into account as well.
    ///
    /// Refer to <see cref="RB.Print"/> for more detail on aligning and setting clip regions for text.
    /// <seedoc>Features:Fonts</seedoc>
    /// </remarks>
    /// <param name="text">Text</param>
    /// <code>
    /// void Render() {
    ///     // Text string with the "damage" number in red
    ///     var text = "Player hit for @FF0000" + damage + "@-!!!";
    ///
    ///     // Manually align the text to the upper right corner of the screen with a 4 pixel offset
    ///     var textSize = RB.PrintMeasure(text);
    ///     var textPos = new Vector2i(RB.DisplaySize.width - text.width - 4, 4);
    ///     RB.Print(textPos, Color.white, text);
    ///
    ///     // Automatically align text using a rect and alignment flags
    ///     RB.Print(
    ///         new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height),
    ///         Color.white,
    ///         RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER,
    ///         text);
    /// }
    /// </code>
    /// <seealso cref="RB.Print"/>
    /// <seealso cref="FontAsset"/>
    /// <returns>Printed dimensions of the text</returns>
    public static Vector2i PrintMeasure(string text)
    {
        if (text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(0, 0, 1, 1), mInstance.mSolidWhite, 0, mTextWrapString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using system font.
    /// </summary>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(Rect2i rect, int flags, string text)
    {
        if (text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(null, rect, mInstance.mSolidWhite, flags, mTextWrapString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using system font.
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(FastString text)
    {
        if (text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapFastString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(null, new Rect2i(0, 0, 1, 1), mInstance.mSolidWhite, 0, mTextWrapFastString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using system font.
    /// </summary>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(Rect2i rect, int flags, FastString text)
    {
        if (text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapFastString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(null, rect, mInstance.mSolidWhite, flags, mTextWrapFastString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="font">Font asset</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(FontAsset font, string text)
    {
        if (font == null || text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(0, 0, 1, 1), mInstance.mSolidWhite, 0, mTextWrapString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="font">Font asset</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(FontAsset font, Rect2i rect, int flags, string text)
    {
        if (font == null || text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(font, rect, mInstance.mSolidWhite, flags, mTextWrapString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="font">Font asset</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(FontAsset font, FastString text)
    {
        if (font == null || text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapFastString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(font, new Rect2i(0, 0, 1, 1), mInstance.mSolidWhite, 0, mTextWrapFastString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Measure a text string without printing it, using custom font.
    /// </summary>
    /// <param name="font">Font asset</param>
    /// <param name="rect">Rectangular area to print to</param>
    /// <param name="flags">Any combination of flags: <see cref="RB.ALIGN_H_LEFT"/>, <see cref="RB.ALIGN_H_RIGHT"/>,
    /// <see cref="RB.ALIGN_H_CENTER"/>, <see cref="RB.ALIGN_V_TOP"/>, <see cref="RB.ALIGN_V_BOTTOM"/>,
    /// <see cref="RB.ALIGN_V_CENTER"/>, <see cref="RB.TEXT_OVERFLOW_CLIP"/>, <see cref="RB.TEXT_OVERFLOW_WRAP"/>.</param>
    /// <param name="text">Text</param>
    /// <returns>Dimensions</returns>
    public static Vector2i PrintMeasure(FontAsset font, Rect2i rect, int flags, FastString text)
    {
        if (font == null || text == null)
        {
            return Vector2i.zero;
        }

        int width, height;
        mTextWrapFastString.Set(text);
        mInstance.mRetroBlitAPI.Font.Print(font, rect, mInstance.mSolidWhite, flags, mTextWrapFastString, true, out width, out height);
        return new Vector2i(width, height);
    }

    /// <summary>
    /// Return true if any of the given button(s) are held down.
    /// </summary>
    /// <remarks>
    /// Return true if any of the given *button*(s) are held down. Supported buttons are:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_A"/></item>
    /// <item><see cref="RB.BTN_B"/></item>
    /// <item><see cref="RB.BTN_X"/></item>
    /// <item><see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_LS"/></item>
    /// <item><see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_UP"/></item>
    /// <item><see cref="RB.BTN_DOWN"/></item>
    /// <item><see cref="RB.BTN_LEFT"/></item>
    /// <item><see cref="RB.BTN_RIGHT"/></item>
    /// <item><see cref="RB.BTN_POINTER_A"/></item>
    /// <item><see cref="RB.BTN_POINTER_B"/></item>
    /// <item><see cref="RB.BTN_POINTER_C"/></item>
    /// <item><see cref="RB.BTN_POINTER_D"/></item>
    /// <item><see cref="RB.BTN_MENU"/></item>
    /// <item><see cref="RB.BTN_SYSTEM"/></item>
    /// </list>
    /// Multiple *buttons* can be checked at once by ORing them. A few useful combinations are provided:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_ABXY"/> - Equivalent of <see cref="RB.BTN_A"/> | <see cref="RB.BTN_B"/> | <see cref="RB.BTN_X"/> | <see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_SHOUDLER"/> - Equivalent of <see cref="RB.BTN_LS"/> | <see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_POINTER_ANY"/> - Equivalent of <see cref="RB.BTN_POINTER_A"/> | <see cref="RB.BTN_POINTER_B"/> | <see cref="RB.BTN_POINTER_C"/> | <see cref="RB.BTN_POINTER_D"/></item>
    /// </list>
    /// If *player* parameter is not provided then by default only <see cref="RB.PLAYER_ONE"/> buttons will be checked. Optionally any player can be specified:
    /// <list type="bullet">
    /// <item><see cref="RB.PLAYER_ONE"/></item>
    /// <item><see cref="RB.PLAYER_TWO"/></item>
    /// <item><see cref="RB.PLAYER_THREE"/></item>
    /// <item><see cref="RB.PLAYER_FOUR"/></item>
    /// </list>
    /// Players can also be ORed to check for more than one player, or <see cref="RB.PLAYER_ANY"/> can be used to check for any player.
    /// <seedoc>Features:Gamepads</seedoc>
    /// <seedoc>Features:Gamepad Input Override</seedoc>
    /// </remarks>
    /// <param name="button">A bitmask of one or multiple buttons</param>
    /// <param name="player">A bitmask of players to check, or <see cref="RB.PLAYER_ANY"/> to check any player. Defaults to <see cref="RB.PLAYER_ONE"/></param>
    /// <returns>True if button(s) held down</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.ButtonDown(RB.BTN_LEFT, RB.PLAYER_TWO) {
    ///         playerTwoPos.x--;
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.InputOverride"/>
    public static bool ButtonDown(int button, int player = RB.PLAYER_ONE)
    {
        return mInstance.mRetroBlitAPI.Input.ButtonDown(button, player);
    }

    /// <summary>
    /// Return true if any of the given button(s) went from an up to down state since <see cref="IRetroBlitGame.Update"/> was last called.
    /// </summary>
    /// <remarks>
    /// Return true if any of the given *button*(s) went from an up to down state since <see cref="IRetroBlitGame.Update"/> was last called. Supported buttons are:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_A"/></item>
    /// <item><see cref="RB.BTN_B"/></item>
    /// <item><see cref="RB.BTN_X"/></item>
    /// <item><see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_LS"/></item>
    /// <item><see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_UP"/></item>
    /// <item><see cref="RB.BTN_DOWN"/></item>
    /// <item><see cref="RB.BTN_LEFT"/></item>
    /// <item><see cref="RB.BTN_RIGHT"/></item>
    /// <item><see cref="RB.BTN_POINTER_A"/></item>
    /// <item><see cref="RB.BTN_POINTER_B"/></item>
    /// <item><see cref="RB.BTN_POINTER_C"/></item>
    /// <item><see cref="RB.BTN_POINTER_D"/></item>
    /// <item><see cref="RB.BTN_MENU"/></item>
    /// <item><see cref="RB.BTN_SYSTEM"/></item>
    /// </list>
    /// Multiple *buttons* can be checked at once by ORing them. A few useful combinations are provided:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_ABXY"/> - Equivalent of <see cref="RB.BTN_A"/> | <see cref="RB.BTN_B"/> | <see cref="RB.BTN_X"/> | <see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_SHOUDLER"/> - Equivalent of <see cref="RB.BTN_LS"/> | <see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_POINTER_ANY"/> - Equivalent of <see cref="RB.BTN_POINTER_A"/> | <see cref="RB.BTN_POINTER_B"/> | <see cref="RB.BTN_POINTER_C"/> | <see cref="RB.BTN_POINTER_D"/></item>
    /// </list>
    /// If *player* parameter is not provided then by default only <see cref="RB.PLAYER_ONE"/> buttons will be checked. Optionally any player can be specified:
    /// <list type="bullet">
    /// <item><see cref="RB.PLAYER_ONE"/></item>
    /// <item><see cref="RB.PLAYER_TWO"/></item>
    /// <item><see cref="RB.PLAYER_THREE"/></item>
    /// <item><see cref="RB.PLAYER_FOUR"/></item>
    /// </list>
    /// Players can also be ORed to check for more than one player, or <see cref="RB.PLAYER_ANY"/> can be used to check for any player.
    /// <seedoc>Features:Gamepads</seedoc>
    /// <seedoc>Features:Gamepad Input Override</seedoc>
    /// </remarks>
    /// <param name="button">A bitmask of one or multiple buttons</param>
    /// <param name="player">A bitmask of players to check, or <see cref="RB.PLAYER_ANY"/> to check any player. Defaults to <see cref="RB.PLAYER_ONE"/></param>
    /// <param name="repeatRate">Button repeat rate in frames, if > 0 then the button will re-trigger at this repeat rate</param>
    /// <returns>True if button(s) pressed</returns>
    /// <code>
    /// AudioAsset soundBeepBoop = new AudioAsset();
    ///
    /// void Update() {
    ///     if (RB.ButtonPressed(RB.BTN_A | RB.BTN_B, RB.PLAYER_ONE | RB.PLAYER_TWO) {
    ///         RB.SoundPlay(soundBeepBoop);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonReleased"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.InputOverride"/>
    public static bool ButtonPressed(int button, int player = RB.PLAYER_ONE, int repeatRate = 0)
    {
        return mInstance.mRetroBlitAPI.Input.ButtonPressed(button, player, repeatRate);
    }

    /// <summary>
    /// Return true if any of the given button(s) went from a down to up state since <see cref="IRetroBlitGame.Update"/> was last called.
    /// </summary>
    /// <remarks>
    /// Return true if any of the given button(s) went from a down to up state since <see cref="IRetroBlitGame.Update"/> was last called. Supported buttons are:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_A"/></item>
    /// <item><see cref="RB.BTN_B"/></item>
    /// <item><see cref="RB.BTN_X"/></item>
    /// <item><see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_LS"/></item>
    /// <item><see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_UP"/></item>
    /// <item><see cref="RB.BTN_DOWN"/></item>
    /// <item><see cref="RB.BTN_LEFT"/></item>
    /// <item><see cref="RB.BTN_RIGHT"/></item>
    /// <item><see cref="RB.BTN_POINTER_A"/></item>
    /// <item><see cref="RB.BTN_POINTER_B"/></item>
    /// <item><see cref="RB.BTN_POINTER_C"/></item>
    /// <item><see cref="RB.BTN_POINTER_D"/></item>
    /// <item><see cref="RB.BTN_MENU"/></item>
    /// <item><see cref="RB.BTN_SYSTEM"/></item>
    /// </list>
    /// Multiple *buttons* can be checked at once by ORing them. A few useful combinations are provided:
    /// <list type="bullet">
    /// <item><see cref="RB.BTN_ABXY"/> - Equivalent of <see cref="RB.BTN_A"/> | <see cref="RB.BTN_B"/> | <see cref="RB.BTN_X"/> | <see cref="RB.BTN_Y"/></item>
    /// <item><see cref="RB.BTN_SHOUDLER"/> - Equivalent of <see cref="RB.BTN_LS"/> | <see cref="RB.BTN_RS"/></item>
    /// <item><see cref="RB.BTN_POINTER_ANY"/> - Equivalent of <see cref="RB.BTN_POINTER_A"/> | <see cref="RB.BTN_POINTER_B"/> | <see cref="RB.BTN_POINTER_C"/> | <see cref="RB.BTN_POINTER_D"/></item>
    /// </list>
    /// If *player* parameter is not provided then by default only <see cref="RB.PLAYER_ONE"/> buttons will be checked. Optionally any player can be specified:
    /// <list type="bullet">
    /// <item><see cref="RB.PLAYER_ONE"/></item>
    /// <item><see cref="RB.PLAYER_TWO"/></item>
    /// <item><see cref="RB.PLAYER_THREE"/></item>
    /// <item><see cref="RB.PLAYER_FOUR"/></item>
    /// </list>
    /// Players can also be ORed to check for more than one player, or <see cref="RB.PLAYER_ANY"/> can be used to check for any player.
    /// <seedoc>Features:Gamepads</seedoc>
    /// <seedoc>Features:Gamepad Input Override</seedoc>
    /// </remarks>
    /// <param name="button">A bitmask of one or multiple buttons</param>
    /// <param name="player">A bitmask of players to check, or <see cref="RB.PLAYER_ANY"/> to check any player. Defaults to <see cref="RB.PLAYER_ONE"/></param>
    /// <returns>True if button(s) released</returns>
    /// <code>
    /// AudioAsset soundBeepBoop = new AudioAsset();
    ///
    /// void Update() {
    ///     if (RB.ButtonPressed(RB.BTN_A | RB.BTN_B, RB.PLAYER_ONE | RB.PLAYER_TWO) {
    ///         RB.SoundPlay(soundBeepBoop);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.InputOverride"/>
    public static bool ButtonReleased(int button, int player = RB.PLAYER_ONE)
    {
        return mInstance.mRetroBlitAPI.Input.ButtonReleased(button, player);
    }

    /// <summary>
    /// Return true if given key is held down
    /// </summary>
    /// <remarks>
    /// Return true if given key specified by *keyCode* is held down.
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <param name="keyCode">Key code</param>
    /// <returns>True if key held down</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyDown(UnityEngine.KeyCode.Z)) {
    ///         FireLasers();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.KeyReleased"/>
    /// <seealso cref="RB.KeyPressed"/>
    /// <seealso cref="RB.InputString"/>
    public static bool KeyDown(KeyCode keyCode)
    {
        return mInstance.mRetroBlitAPI.Input.KeyDown(keyCode);
    }

    /// <summary>
    /// Return true if given key specified by *keyCode* went from an up to down state since last call to <see cref="IRetroBlitGame.Update"/>
    /// </summary>
    /// <remarks>
    /// Return true if given key specified by *keyCode* went from an up to down state since last call to <see cref="IRetroBlitGame.Update"/>.
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <param name="keyCode">Key code</param>
    /// <param name="repeatRate">Key repeat rate in frames, if > 0 then the button will re-trigger at this repeat rate</param>
    /// <returns>True if key pressed</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyPressed(UnityEngine.KeyCode.Z)) {
    ///         DrinkPotion();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.KeyReleased"/>
    /// <seealso cref="RB.KeyDown"/>
    /// <seealso cref="RB.InputString"/>
    public static bool KeyPressed(KeyCode keyCode, int repeatRate = 0)
    {
        return mInstance.mRetroBlitAPI.Input.KeyPressed(keyCode, repeatRate);
    }

    /// <summary>
    /// Return true if given key specified by *keyCode* went from a down to up state since last call to <see cref="IRetroBlitGame.Update"/>
    /// </summary>
    /// <remarks>
    /// Return true if given key specified by *keyCode* went from a down to up state since last call to <see cref="IRetroBlitGame.Update"/>.
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <param name="keyCode">Key code</param>
    /// <returns>True if key released</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyReleased(UnityEngine.KeyCode.Z)) {
    ///         SelfDestruct();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.KeyPressed"/>
    /// <seealso cref="RB.KeyDown"/>
    /// <seealso cref="RB.InputString"/>
    public static bool KeyReleased(KeyCode keyCode)
    {
        return mInstance.mRetroBlitAPI.Input.KeyReleased(keyCode);
    }

    /// <summary>
    /// Return true if any key, touch, gamepad or mouse button is held down
    /// </summary>
    /// <remarks>
    /// Return true if any key, touch, gamepad or mouse button is held down
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <returns>True if key held down</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.AnyKeyDown()) {
    ///         FireLasers();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.AnyKeyReleased"/>
    /// <seealso cref="RB.AnyKeyPressed"/>
    public static bool AnyKeyDown()
    {
        return mInstance.mRetroBlitAPI.Input.AnyKeyDown();
    }

    /// <summary>
    /// Return true if any key is down and there are was no key down in the last call to <see cref="IRetroBlitGame.Update"/>
    /// </summary>
    /// <remarks>
    /// Return true if any key, touch, gamepad or mouse button is down and there are was no key, touch,
    /// gamepad or mouse button down in the last call to <see cref="IRetroBlitGame.Update"/>
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <param name="repeatRate">Any key repeat rate in frames, if > 0 then the any key will re-trigger at this repeat rate</param>
    /// <returns>True if any key pressed</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.AnyKeyPressed()) {
    ///         DrinkPotion();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.AnyKeyReleased"/>
    /// <seealso cref="RB.AnyKeyDown"/>
    public static bool AnyKeyPressed(int repeatRate = 0)
    {
        return mInstance.mRetroBlitAPI.Input.AnyKeyPressed(repeatRate);
    }

    /// <summary>
    /// Return true all keys are up and there are was at least one key down in the last call to <see cref="IRetroBlitGame.Update"/>
    /// </summary>
    /// <remarks>
    /// Return true if all keys, touches, gamepad or mouse buttons are up and there are was at least one key, touch,
    /// gamepad or mouse button down in the last call to <see cref="IRetroBlitGame.Update"/>
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <returns>True if key released</returns>
    /// <code>
    /// void Update() {
    ///     if (RB.KeyReleased(UnityEngine.KeyCode.Z)) {
    ///         SelfDestruct();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.AnyKeyPressed"/>
    /// <seealso cref="RB.AnyKeyDown"/>
    public static bool AnyKeyReleased()
    {
        return mInstance.mRetroBlitAPI.Input.AnyKeyReleased();
    }

    /// <summary>
    /// Returns a string of characters entered since last <see cref="IRetroBlitGame.Update"/> call.
    /// </summary>
    /// <remarks>
    /// Returns a string of characters entered since last <see cref="IRetroBlitGame.Update"/> call. Normally this string will be 0 or 1 characters long, but it is possible that the user may
    /// quickly perform two key strokes within the spawn of a single game loop. This method is the preferred way of capturing user text input.
    /// <seedoc>Features:Keyboard</seedoc>
    /// </remarks>
    /// <returns>Character string</returns>
    /// <code>
    /// string userInput = "";
    ///
    /// void Update() {
    ///     // Capture user input string from last frame
    ///     string input = RB.InputString();
    ///
    ///     // Iterate through the input and handle special characters
    ///     for (int i = 0; i < input.Length; i++) {
    ///         char c = input[i];
    ///
    ///         // Any character with unicode value >= ' ' can just be used as-is
    ///         if (c >= ' ') {
    ///             userInput.Append(c);
    ///         }
    ///         // Handle backspace
    ///         else if (c == (char)UnityEngine.KeyCode.Backspace && userInput.Length > 0) {
    ///             userInput.Truncate(userInput.Length - 1);
    ///         }
    ///         // Handle enter/return
    ///         else if (c == (char)KeyCode.Return) {
    ///             Execute(userInput)
    ///         }
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.KeyPressed"/>
    /// <seealso cref="RB.KeyReleased"/>
    /// <seealso cref="RB.KeyDown"/>
    public static string InputString()
    {
        return mInstance.mRetroBlitAPI.Input.InputString();
    }

    /// <summary>
    /// Get the current pointer position
    /// </summary>
    /// <remarks>
    /// Returns the current pointer position in game display coordinates. If *pointerIndex* is not specified then the mouse position, or first finger touch is returned.
    /// Alternatively, *pointerIndex* value of 0 to 3 can be specified to query a specific finger touch position.
    ///
    /// If there is no mouse connected, and there are no active touches then the position is undefined. See <see cref="RB.PointerPosValid"/> for checking
    /// if the pointer position is valid.
    ///
    /// Pointer button states can be checked with <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/>, and <see cref="RB.ButtonReleased"/>.
    /// <seedoc>Features:Pointer</seedoc>
    /// <seedoc>Features:Multi-touch Support</seedoc>
    /// </remarks>
    /// <param name="pointerIndex">Index of the pointer position to get</param>
    /// <returns>Pointer position</returns>
    /// <code>
    /// void Render() {
    ///     if (RB.PointerPosValid()) {
    ///         RB.DrawSprite("pointer", RB.PointerPos());
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.PointerPosValid"/>
    /// <seealso cref="RB.PointerScrollDelta"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public static Vector2i PointerPos(int pointerIndex = 0)
    {
        if (pointerIndex < 0 || pointerIndex >= RetroBlitInternal.RBInput.MAX_TOUCHES)
        {
            Debug.LogError("Invalid pointerIndex, only indecies 0 to " + RetroBlitInternal.RBInput.MAX_TOUCHES  + " are supported");
            return Vector2i.zero;
        }

        return mInstance.mRetroBlitAPI.Input.PointerPos(pointerIndex);
    }

    /// <summary>
    /// Returns true if the pointer position is valid
    /// current touches.
    /// </summary>
    /// <remarks>
    /// Returns true if the pointer position is valid. A pointer position may be undefined if there is no mouse connected, and there are no
    /// current touches. If *pointerIndex* is not specified then the mouse, or first finger touch position is checked. Alternatively, *pointerIndex*
    /// value of 0 to 3 can be specified to query a specific finger touch position validity.
    /// <seedoc>Features:Pointer</seedoc>
    /// <seedoc>Features:Multi-touch Support</seedoc>
    /// </remarks>
    /// <param name="pointerIndex">Index of the pointer to check</param>
    /// <returns>True if pointer position is valid</returns>
    /// <code>
    /// void Render() {
    ///     if (RB.PointerPosValid()) {
    ///         RB.DrawSprite("pointer", RB.PointerPos());
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.PointerPos"/>
    public static bool PointerPosValid(int pointerIndex = 0)
    {
        if (pointerIndex < 0 || pointerIndex >= RetroBlitInternal.RBInput.MAX_TOUCHES)
        {
            Debug.LogError("Invalid pointerIndex, only indecies 0 to " + RetroBlitInternal.RBInput.MAX_TOUCHES + " are supported");
            return false;
        }

        return mInstance.mRetroBlitAPI.Input.PointerPosValid(pointerIndex);
    }

    /// <summary>
    /// Get the delta of the mouse scroll wheel position since last <see cref="IRetroBlitGame.Update"/> call
    /// </summary>
    /// <remarks>
    /// Get the delta of the mouse scroll position since last <see cref="IRetroBlitGame.Update"/> call. The delta is always 0 if there
    /// is no mouse detected.
    ///
    /// To detect if the scroll wheel was pressed use <see cref="RB.BTN_POINTER_C"/> with
    /// <see cref="RB.ButtonDown"/>, <see cref="RB.ButtonPressed"/>, or <see cref="RB.ButtonReleased"/>.
    /// <seedoc>Features:Pointer</seedoc>
    /// </remarks>
    /// <returns>Scroll wheel delta position</returns>
    /// <code>
    /// int scrollPosition = 0;
    ///
    /// void Update() {
    ///     scrollPosition += RB.PointerScrollDelta();
    /// }
    /// </code>
    /// <seealso cref="RB.PointerPos"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public static float PointerScrollDelta()
    {
        return mInstance.mRetroBlitAPI.Input.PointerScrollDelta();
    }

    /// <summary>
    /// Translate native screen coordinates to RetroBlit display coordinates
    /// </summary>
    /// <remarks>
    /// Translate native screen coordinates to RetroBlit display coordinates. In most games this will likely not be needed, RetroBlit already translates
    /// mouse and touch positions to RetroBlit display coordinates, however this method may be useful when integrating RetroBlit into a more complicated Unity
    /// Scene.
    /// </remarks>
    /// <code>
    /// void Render() {
    ///     var nativeScreenPos = UnityEngine.Camera.WorldToScreenPoint(myGameObject.transform.position);
    ///     var retroBlitPos = RB.NativeScreenToDisplayPos(nativeScreenPos);
    ///
    ///     RB.DrawEllipse(retroBlitPos, new Vector2i(4, 4), Color.white);
    /// }
    /// </code>
    /// <param name="nativeScreenPos">Native screen coordinates</param>
    /// <returns>RetroBlit display coordinates</returns>
    public static Vector2i NativeScreenToDisplayPos(Vector2 nativeScreenPos)
    {
        return mInstance.mRetroBlitAPI.PresentCamera.ScreenToViewportPoint(nativeScreenPos);
    }

    /// <summary>
    /// Provide a delegate for overriding RetroBlit button mapping
    /// </summary>
    /// <remarks>
    /// Provide a delegate for overriding input mapping. The delegate will be called just before every
    /// <see cref="IRetroBlitGame.Update"/>, and every time <see cref="RB.ButtonDown"/> is called.
    /// <seedoc>Features:Gamepad Input Override</seedoc>
    /// </remarks>
    /// <code>
    /// void Initialize() {
    ///     RB.InputOverride(CustomInputOverride);
    /// }
    ///
    /// void CustomInputOverride(int button, int player, out bool handled)
    /// {
    ///     handled = false;
    ///
    ///     // Map button A for player one to the tab key
    ///     if (player == RB.PLAYER_ONE) {
    ///         if (button == RB.BTN_A) {
    ///             handled = true;
    ///             return Unity.Input.GetKeyDown(KeyCode.Tab);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <param name="overrideMethod">Input override delegate</param>
    /// <seealso cref="RB.InputOverrideMethod"/>
    /// <seealso cref="RB.ButtonDown"/>
    /// <seealso cref="RB.ButtonPressed"/>
    /// <seealso cref="RB.ButtonReleased"/>
    public static void InputOverride(InputOverrideMethod overrideMethod)
    {
        mInstance.mRetroBlitAPI.Input.InputOverride(overrideMethod);
    }

    /// <summary>
    /// Play the sound from the given AudioAsset
    /// </summary>
    /// <remarks>
    /// Play the sound from the given <see cref="AudioAsset"/>. This function returns the <see cref="SoundReference"/> of the playing
    /// sound, which can then be used to adjust the playback of the sound, stop the sound, or check its state.
    ///
    /// Optionally the initial *volume* and *pitch* can also be specified.
    ///
    /// Note that <see cref="RB.SoundPlay"/> can be called on the same SoundAsset multiple times, and the playing sound clip can overlap itself. Each instance of the
    /// playing sound will have its own independent <see cref="SoundReference"/> returned.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="audio">Audio Asset to play</param>
    /// <param name="volume">Volume to play at where 0 is silent, 1 is the original clip volume, and values greater than 1 indicate amplification beyond the recorded volume. Defaults to 1</param>
    /// <param name="pitch">Pitch to play at, where 1 is the original sound pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch. Defaults to 1</param>
    /// <param name="priority">Priority level of the sound. If there are no more sound channels left then the lowest priority sound below this priority will be stopped, and replace by this sound.</param>
    /// <returns>Sound reference</returns>
    /// <code>
    /// AudioAsset soundExplosion = new AudioAsset();
    ///
    /// void Initialize() {
    ///     soundExplosion.Load("sounds/explosion");
    /// }
    ///
    /// void Update()
    /// {
    ///     if (dead) {
    ///         float volume = 0.5f;
    ///         float pitch = Random.Range(0.9f, 1.1f);
    ///         RB.SoundPlay(soundExplosion, volume, pitch);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static SoundReference SoundPlay(AudioAsset audio, float volume = 1.0f, float pitch = 1.0f, int priority = 0)
    {
        return mInstance.mRetroBlitAPI.Audio.SoundPlay(audio, volume, pitch, priority);
    }

    /// <summary>
    /// Check if a sound is playing by using its <see cref="SoundReference"/>.
    /// </summary>
    /// <remarks>
    /// Check if a sound is playing by using its <see cref="SoundReference"/>.
    /// If a sound is no longer playing its reference is considered no longer valid and can be disposed of.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>True if still playing, false otherwise</returns>
    /// <code>
    /// List<SoundAsset> soundCar = new List<SoundAsset>();
    /// SoundReference carSoundRef;
    ///
    /// void Initialize() {
    ///     AudioAsset sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving1");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving2");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving3");
    ///     soundCar.Add(sound);
    /// }
    ///
    /// void Update()
    /// {
    ///     if (!RB.SoundIsPlaying(carSoundRef) {
    ///         // Car sound is done, pick a new random car sound
    ///         carSoundRef = RB.SoundPlay(soundCar[Random.Range(0, 3)]);
    ///     } else {
    ///         // Decrease volume and pitch with time
    ///         RB.SoundVolumeSet(carSoundRef, RB.SoundVolumeGet(carSoundRef) * 0.9f);
    ///         RB.SoundPitchSet(carSoundRef, RB.SoundPitchGet(carSoundRef) * 0.9f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static bool SoundIsPlaying(SoundReference soundReference)
    {
        return mInstance.mRetroBlitAPI.Audio.SoundIsPlaying(soundReference);
    }

    /// <summary>
    /// Set the volume of a currently playing sound with its <see cref="SoundReference"/>
    /// </summary>
    /// <remarks>
    /// Set the volume of a currently playing sound with its <see cref="SoundReference"/>.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="volume">Volume to play at where 0 is silent, 1 is the original clip volume, and values greater than 1 indicate amplification beyond the recorded volume. Defaults to 1</param>
    /// <code>
    /// List<SoundAsset> soundCar = new List<SoundAsset>();
    /// SoundReference carSoundRef;
    ///
    /// void Initialize() {
    ///     AudioAsset sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving1");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving2");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving3");
    ///     soundCar.Add(sound);
    /// }
    ///
    /// void Update()
    /// {
    ///     if (!RB.SoundIsPlaying(carSoundRef) {
    ///         // Car sound is done, pick a new random car sound
    ///         carSoundRef = RB.SoundPlay(soundCar[Random.Range(0, 3)]);
    ///     } else {
    ///         // Decrease volume and pitch with time
    ///         RB.SoundVolumeSet(carSoundRef, RB.SoundVolumeGet(carSoundRef) * 0.9f);
    ///         RB.SoundPitchSet(carSoundRef, RB.SoundPitchGet(carSoundRef) * 0.9f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static void SoundVolumeSet(SoundReference soundReference, float volume)
    {
        mInstance.mRetroBlitAPI.Audio.SoundVolumeSet(soundReference, volume);
    }

    /// <summary>
    /// Set the pitch of a currently playing sound with its <see cref="SoundReference"/>
    /// </summary>
    /// <remarks>
    /// Set the pitch of a currently playing sound with its <see cref="SoundReference"/>.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="pitch">Pitch to play at, where 1 is the original sound pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch. Defaults to 1</param>
    /// <code>
    /// List<SoundAsset> soundCar = new List<SoundAsset>();
    /// SoundReference carSoundRef;
    ///
    /// void Initialize() {
    ///     AudioAsset sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving1");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving2");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving3");
    ///     soundCar.Add(sound);
    /// }
    ///
    /// void Update()
    /// {
    ///     if (!RB.SoundIsPlaying(carSoundRef) {
    ///         // Car sound is done, pick a new random car sound
    ///         carSoundRef = RB.SoundPlay(soundCar[Random.Range(0, 3)]);
    ///     } else {
    ///         // Decrease volume and pitch with time
    ///         RB.SoundVolumeSet(carSoundRef, RB.SoundVolumeGet(carSoundRef) * 0.9f);
    ///         RB.SoundPitchSet(carSoundRef, RB.SoundPitchGet(carSoundRef) * 0.9f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static void SoundPitchSet(SoundReference soundReference, float pitch)
    {
        mInstance.mRetroBlitAPI.Audio.SoundPitchSet(soundReference, pitch);
    }

    /// <summary>
    /// Get the volume of a currently playing sound with its <see cref="SoundReference"/>
    /// </summary>
    /// <remarks>
    /// Get the volume of a currently playing sound with its <see cref="SoundReference"/>.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>Volume</returns>
    /// <code>
    /// List<SoundAsset> soundCar = new List<SoundAsset>();
    /// SoundReference carSoundRef;
    ///
    /// void Initialize() {
    ///     AudioAsset sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving1");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving2");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving3");
    ///     soundCar.Add(sound);
    /// }
    ///
    /// void Update()
    /// {
    ///     if (!RB.SoundIsPlaying(carSoundRef) {
    ///         // Car sound is done, pick a new random car sound
    ///         carSoundRef = RB.SoundPlay(soundCar[Random.Range(0, 3)]);
    ///     } else {
    ///         // Decrease volume and pitch with time
    ///         RB.SoundVolumeSet(carSoundRef, RB.SoundVolumeGet(carSoundRef) * 0.9f);
    ///         RB.SoundPitchSet(carSoundRef, RB.SoundPitchGet(carSoundRef) * 0.9f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static float SoundVolumeGet(SoundReference soundReference)
    {
        return mInstance.mRetroBlitAPI.Audio.SoundVolumeGet(soundReference);
    }

    /// <summary>
    /// Get the pitch of a currently playing sound with its <see cref="SoundReference"/>
    /// </summary>
    /// <remarks>
    /// Get the pitch of a currently playing sound with its <see cref="SoundReference"/>.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <returns>Pitch</returns>
    /// <code>
    /// List<SoundAsset> soundCar = new List<SoundAsset>();
    /// SoundReference carSoundRef;
    ///
    /// void Initialize() {
    ///     AudioAsset sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving1");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving2");
    ///     soundCar.Add(sound);
    ///
    ///     sound = new AudioAsset();
    ///     sound.Load("sounds/car_driving3");
    ///     soundCar.Add(sound);
    /// }
    ///
    /// void Update()
    /// {
    ///     if (!RB.SoundIsPlaying(carSoundRef) {
    ///         // Car sound is done, pick a new random car sound
    ///         carSoundRef = RB.SoundPlay(soundCar[Random.Range(0, 3)]);
    ///     } else {
    ///         // Decrease volume and pitch with time
    ///         RB.SoundVolumeSet(carSoundRef, RB.SoundVolumeGet(carSoundRef) * 0.9f);
    ///         RB.SoundPitchSet(carSoundRef, RB.SoundPitchGet(carSoundRef) * 0.9f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static float SoundPitchGet(SoundReference soundReference)
    {
        return mInstance.mRetroBlitAPI.Audio.SoundPitchGet(soundReference);
    }

    /// <summary>
    /// Stop the sound currently playing by using its <see cref="SoundReference"/>
    /// </summary>
    /// <remarks>
    /// Stop the sound currently playing by using its <see cref="SoundReference"/>. Once the sound is stopped it can no longer be resumed and the specified <see cref="SoundReference"/> is
    /// no longer valid.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <code>
    /// AudioAsset soundSpeech = new AudioAsset();
    /// SoundReference speechSoundRef;
    ///
    /// void Initialize() {
    ///     soundSpeech.Load("voice/tutorial");
    /// }
    ///
    /// void Update()
    /// {
    ///     if (RB.KeyDown(KeyCode.F1)) {
    ///         if (RB.SoundIsPlaying(speechSoundRef)) {
    ///             RB.SoundStop(speechSoundRef);
    ///         } else {
    ///             speechSoundRef = RB.SoundPlay(soundSpeech);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    /// <seealso cref="RB.SoundLoopSet"/>
    public static void SoundStop(SoundReference soundReference)
    {
        mInstance.mRetroBlitAPI.Audio.SoundStop(soundReference);
    }

    /// <summary>
    /// Specify whether the currently playing sound specified by <see cref="SoundReference"/> should keep looping.
    /// </summary>
    /// <remarks>
    /// Specify whether the currently playing sound specified by <see cref="SoundReference"/> should keep looping.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="soundReference">Sound reference</param>
    /// <param name="loop">True if sound should loop</param>
    /// <code>
    /// AudioAsset soundAlarm = new AudioAsset();
    /// SoundReference alarmSoundRef;
    ///
    /// void Initialize() {
    ///     soundAlarm.Load("sounds/alarm");
    /// }
    ///
    /// void Update()
    /// {
    ///     if (EnemyInRange()) {
    ///         if (!RB.SoundIsPlaying(alarmSoundRef)) {
    ///             alarmSoundRef = RB.SoundPlay(soundAlarm);
    ///             RB.SoundLoopSet(alarmSoundRef, true);
    ///         }
    ///     }
    ///     else if (RB.SoundIsPlaying(alarmSoundRef)) {
    ///         RB.SoundStop(alarmSoundRef);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.SoundPlay"/>
    /// <seealso cref="RB.SoundStop"/>
    /// <seealso cref="RB.SoundIsPlaying"/>
    /// <seealso cref="RB.SoundVolumeSet"/>
    /// <seealso cref="RB.SoundVolumeGet"/>
    /// <seealso cref="RB.SoundPitchSet"/>
    /// <seealso cref="RB.SoundPitchGet"/>
    public static void SoundLoopSet(SoundReference soundReference, bool loop)
    {
        mInstance.mRetroBlitAPI.Audio.SoundLoopSet(soundReference, loop);
    }

    /// <summary>
    /// Set position of a playing sound.
    /// </summary>
    /// <remarks>
    /// Set the position of the playing sound in 2D space. The position is absolute and does not take the camera position into account.
    /// </remarks>
    /// <param name="soundReference">Reference to the playing sound, as returned by <see cref="RB.SoundPlay"/></param>
    /// <param name="pos">Position of the sound</param>
    /// <seealso cref="RB.SoundPlay"/>
    public static void SoundPosSet(SoundReference soundReference, Vector2i pos)
    {
        RetroBlitInternal.RBAPI.instance.Audio.SoundPosSet(soundReference, new Vector3(pos.x, pos.y, 0));
    }

    /// <summary>
    /// Get the Unity AudioSource of a playing sound.
    /// </summary>
    /// <remarks>
    /// Get the Unity AudioSource of a playing sound. The AudioSource can be used to customize various properties of the playing sound.
    /// For more details see *AudioSource* in Unity documentation.
    /// </remarks>
    /// <param name="soundReference">Reference to the playing sound, as returned by <see cref="RB.SoundPlay"/></param>
    /// <returns>Unity AudioSource</returns>
    /// <seealso cref="RB.SoundPlay"/>
    public static AudioSource SoundSourceGet(SoundReference soundReference)
    {
        return RetroBlitInternal.RBAPI.instance.Audio.GetSourceForSoundReference(soundReference);
    }

    /// <summary>
    /// Set sound listener position.
    /// </summary>
    /// <remarks>
    /// Set sound listener position. For positional audio the listener position defines where the listener is located in relation to the sounds being played.
    /// This position should be set to the center of the players view, or the players position.
    /// </remarks>
    /// <param name="pos">Position</param>
    public static void SoundListenerPosSet(Vector2i pos)
    {
        RetroBlitInternal.RBAPI.instance.Audio.SoundListenerPosSet(pos);
    }

    /// <summary>
    /// Play music from the given<see cref="RB.AudioAsset"/>
    /// </summary>
    /// <remarks>
    /// Play music from the given<see cref="RB.AudioAsset"/>. If there already is music playing then it is cross-faded to the newly specified music.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <param name="music">Audio asset to play</param>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static void MusicPlay(AudioAsset music)
    {
        mInstance.mRetroBlitAPI.Audio.MusicPlay(music);
    }

    /// <summary>
    /// Stop currently playing music
    /// </summary>
    /// <remarks>
    /// Stop currently playing music.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    /// }
    ///
    /// void Update() {
    ///     if (gameStarted) {
    ///         RB.MusicStop();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static void MusicStop()
    {
        mInstance.mRetroBlitAPI.Audio.MusicStop();
    }

    /// <summary>
    /// Check if Music is currently playing
    /// </summary>
    /// <remarks>
    /// Check if Music is currently playing.
    /// </remarks>
    /// <returns>True if music is playing</returns>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static bool MusicIsPlaying()
    {
        return mInstance.mRetroBlitAPI.Audio.MusicIsPlaying();
    }

    /// <summary>
    /// Set the music volume
    /// </summary>
    /// <remarks>
    /// Set the music volume.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <param name="volume">New volume to play at where 0 is silent, 1 is the original music volume, and values greater than 1 indicate amplification beyond the recorded volume</param>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    ///     RB.MusicVolumeSet(0.75f);
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static void MusicVolumeSet(float volume = 1.0f)
    {
        mInstance.mRetroBlitAPI.Audio.MusicVolumeSet(volume);
    }

    /// <summary>
    /// Set the music pitch
    /// </summary>
    /// <remarks>
    /// Set the music pitch.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <param name="pitch">New pitch to play at, where 1 is the original music pitch, values less than 1 indicate lower pitch, and values greater than 1 indicate higher pitch</param>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    ///     RB.MusicPitchSet(0.75f);
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static void MusicPitchSet(float pitch = 1.0f)
    {
        mInstance.mRetroBlitAPI.Audio.MusicPitchSet(pitch);
    }

    /// <summary>
    /// Get the current music volume
    /// </summary>
    /// <remarks>
    /// Get the current music volume.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <returns>Volume</returns>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    ///     RB.MusicPitchSet(0.75f);
    /// }
    ///
    /// void Update() {
    ///     // Slowly decrease music volume down to 0.5f
    ///     if (RB.MusicVolumeGet() > 0.5f) {
    ///         RB.MusicVolumeSet(RB.MusicVolumeGet() * 0.95f);
    ///     } else {
    ///         RB.MusicVolumeSet(0.5f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    public static float MusicVolumeGet()
    {
        return mInstance.mRetroBlitAPI.Audio.MusicVolumeGet();
    }

    /// <summary>
    /// Get the current music pitch.
    /// </summary>
    /// <remarks>
    /// Get the current music pitch.
    /// <seedoc>Features:Music</seedoc>
    /// </remarks>
    /// <returns>Pitch</returns>
    /// <code>
    /// AudioAsset musicTitle = new AudioAsset();
    ///
    /// void Initialize() {
    ///     musicTitle.Load("music/title");
    ///     RB.MusicPlay(musicTitle);
    ///     RB.MusicPitchSet(0.75f);
    /// }
    ///
    /// void Update() {
    ///     // Slowly decrease music pitch down to 0.85f
    ///     if (RB.MusicPitchGet() > 0.85f) {
    ///         RB.MusicPitchSet(RB.MusicPitchGet() * 0.95f);
    ///     } else {
    ///         RB.MusicPitchSet(0.85f);
    ///     }
    /// }
    /// </code>
    /// <seealso cref="AudioAsset"/>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    public static float MusicPitchGet()
    {
        return mInstance.mRetroBlitAPI.Audio.MusicPitchGet();
    }

    /// <summary>
    /// Set music crossfade duration and overlap
    /// </summary>
    /// <remarks>
    /// Set music crossfade duration and overlap.
    /// 
    /// With overlap of 0.0 the previous music will completely fade out before new music starts fading in.
    /// 
    /// With overlap of 1.0 the previous and new music will fade at the same time.
    /// 
    /// With overlap of -1.0 the previous music will fade out completely, a moment a silence will follow, and then the new music will start fading in.
    /// </remarks>
    /// <param name="fadeDuration">Fade duration in seconds, default 1s</param>
    /// <param name="overlap">Overlap from -1.0 to 1.0, default 0</param>
    /// <param name="fadeOutFunc">Easing function to use while fading out, default Ease.QuadOut</param>
    /// <param name="fadeInFunc">Easing function to use while fading in, default Ease.QuadOut</param>
    /// <code>
    /// void PlayMusic(AudioAsset newMusic) {
    ///     float fadeDuration = 1.0f;
    ///     float fadeOverlap = 0.5f;
    ///     Ease.Func fadeOutFunc = Ease.Func.QuadOut;
    ///     Ease.Func fadeInFunc = Ease.Func.QuadOut;
    ///     RB.MusicCrossFadeSet(fadeDuration, fadeOverlap, fadeOutFunc, FadeInFunc);
    ///
    ///     RB.MusicPlay(newMusic);
    /// }
    /// </code>
    /// <seealso cref="RB.MusicPlay"/>
    /// <seealso cref="RB.MusicStop"/>
    /// <seealso cref="RB.MusicVolumeSet"/>
    /// <seealso cref="RB.MusicPitchSet"/>
    /// <seealso cref="RB.MusicVolumeGet"/>
    /// <seealso cref="RB.MusicPitchGet"/>
    /// <seealso cref="RB.MusicIsPlaying"/>
    /// <seealso cref="AudioAsset"/>
    public static void MusicCrossFadeSet(float fadeDuration, float overlap, Ease.Func fadeOutFunc = Ease.Func.QuadOut, Ease.Func fadeInFunc = Ease.Func.QuadOut)
    {
        mInstance.mRetroBlitAPI.Audio.MusicCrossFadeSet(fadeDuration, overlap, fadeOutFunc, fadeInFunc);
    }

    /// <summary>
    /// Set values for the given post processing effect
    /// </summary>
    /// <remarks>
    /// Set values for the given post processing effect *type*. Different effects use different parameters. See <see cref="RB.Effect"/> for a list
    /// of all built-in RetroBlit effects, and their preview.
    ///
    /// Post-processing effects are effects that can be applied after <see cref="IRetroBlitGame.Render"/> exits.
    /// Unlike normal rendering, some of the post processing effects render in native window resolution and so can create
    /// sub-pixel details such as <mref refid="RB.Effect.Scanlines">Scanlines</mref>.
    ///
    /// Multiple effects can be layered together. For example you may want to enable the <mref refid="RB.Effect.Scanlines">Scanlines</mref>,
    /// <mref refid="RB.Effect.Noise">Noise</mref>, and <mref refid="RB.Effect.Curvature">Curvature</mref>
    /// effects together to create a nice retro CRT TV effect.
    ///
    /// It is also possible to write your own custom post-processing effects by loading shaders with <see cref="ShaderAsset.Load"/>
    /// and setting them with <see cref="RB.EffectShader"/>.
    /// <seedoc>Features:Post-Processing Effects</seedoc>
    /// <seedoc>Features:Post-Processing Shaders</seedoc>
    /// </remarks>
    /// <param name="type">Effect type</param>
    /// <param name="intensity">Intensity</param>
    /// <param name="parameters">Additional parameters</param>
    /// <param name="color">Color</param>
    /// <code>
    /// void Initialize() {
    ///     // Setup a retro CRT TV effect with scanlines and a bit of noise
    ///     RB.EffectSet(RB.Effect.Scanlines, 0.25f);
    ///     RB.EffectSet(RB.Effect.Noise, 0.05f);
    ///     RB.EffectSet(RB.Effect.Curvature, 0.15f, new Color32(32, 32, 32, 255));
    /// }
    /// </code>
    /// <seealso cref="RB.EffectShader"/>
    /// <seealso cref="RB.EffectFilter"/>
    /// <seealso cref="RB.EffectReset"/>
    /// <seealso cref="RB.EffectApplyNow"/>
    public static void EffectSet(Effect type, float intensity, Vector2i parameters, Color32 color)
    {
        mInstance.mRetroBlitAPI.Effects.EffectSet(type, intensity, parameters, color);
    }

    /// <summary>
    /// Set intensity for the given post processing effect. Setting <paramref name="intensity"/> to 0 turns off the effect entirely.
    /// </summary>
    /// <param name="type">Effect type</param>
    /// <param name="intensity">Intensity</param>
    public static void EffectSet(Effect type, float intensity)
    {
        mInstance.mRetroBlitAPI.Effects.EffectSet(type, intensity, Vector2i.zero, Color.black);
    }

    /// <summary>
    /// Set parameters for the given post processing effect. The effect is not yet active if its intensity is 0.
    /// </summary>
    /// <param name="type">Effect type</param>
    /// <param name="parameters">Additional parameters</param>
    public static void EffectSet(Effect type, Vector2i parameters)
    {
        mInstance.mRetroBlitAPI.Effects.EffectSet(type, 0, parameters, Color.black);
    }

    /// <summary>
    /// Set color for the given post processing effect. The effect is not yet active if its intensity is 0.
    /// </summary>
    /// <param name="type">Effect type</param>
    /// <param name="color">Color</param>
    public static void EffectSet(Effect type, Color32 color)
    {
        mInstance.mRetroBlitAPI.Effects.EffectSet(type, 0, Vector2i.zero, color);
    }

    /// <summary>
    /// Set a custom post-processing effect shader.
    /// </summary>
    /// <remarks>
    /// Set a custom post-processing effect shader which was previously created with <see cref="ShaderAsset.Load"/>. Using this method
    /// any post-processing effect can be created which takes the RetroBlit display as input, and any other inputs provided by
    /// RetroBlit shader methods.
    ///
    /// Note that some RetroBlit built-in shaders will not work if a custom shader is specified, because they themselves are
    /// created by a built-in RetroBlit shader which will be replaced by a call to <see cref="RB.EffectShader"/>. To revert back
    /// to the built-in shader call <see cref="RB.EffectReset"/>.
    ///
    /// See Assets/RetroBlit/Internal/Materials/PresentBasicShader.shader for an example of a minimal present shader, and
    /// Assets/RetroBlit/Resources/Demos/DemoReel/PresentRippleShader.shader for a simple ripple-effect shader.
    /// <seedoc>Features:Post-Processing Effects</seedoc>
    /// <seedoc>Features:Post-Processing Shaders</seedoc>
    /// </remarks>
    /// <param name="shader">Shader asset</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/ripple");
    ///     RB.EffectShader(shaderEffect);
    /// }
    ///
    /// void Render() {
    ///     // Animate a value in the post processing shader to create a moving ripple effect
    ///     shaderEffect.FloatSet("wave", RB.Ticks / 25.0f);
    /// }
    /// </code>
    /// <seealso cref="RB.EffectSet"/>
    /// <seealso cref="RB.EffectFilter"/>
    /// <seealso cref="RB.EffectReset"/>
    /// <seealso cref="RB.EffectApplyNow"/>
    public static void EffectShader(ShaderAsset shader)
    {
        if (shader == null)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Effects.EffectShaderSet(shader);
    }

    /// <summary>
    /// Specify texture filtering to use with custom post-processing effect shader.
    /// </summary>
    /// <remarks>
    /// Specify texture filtering to use on the RetroBlit display with custom post-processing effect shader, see <see cref="EffectShader"/>.
    /// Default filter is <see cref="RB.Filter.Nearest"/> which creates sharp pixels, to get smooth interpolation use <see cref="RB.Filter.Linear"/>.
    /// <seedoc>Features:Post-Processing Effects</seedoc>
    /// <seedoc>Features:Post-Processing Shaders</seedoc>
    /// </remarks>
    /// <param name="filter"><see cref="RB.Filter.Nearest"/> or <see cref="RB.Filter.Linear"/></param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/ripple");
    ///     RB.EffectShader(shaderEffect);
    ///     RB.EffectFilter(RB.Filter.Linear);
    /// }
    ///
    /// void Render() {
    ///     // Animate a value in the post processing shader to create a moving ripple effect
    ///     shaderEffect.FloatSet("wave", RB.Ticks / 25.0f);
    /// }
    /// </code>
    /// <seealso cref="RB.EffectSet"/>
    /// <seealso cref="RB.EffectShader"/>
    /// <seealso cref="RB.EffectReset"/>
    /// <seealso cref="RB.EffectApplyNow"/>
    public static void EffectFilter(Filter filter)
    {
        FilterMode filterMode = filter == Filter.Nearest ? FilterMode.Point : FilterMode.Bilinear;
        mInstance.mRetroBlitAPI.Effects.EffectFilterSet(filterMode);
    }

    /// <summary>
    /// Reset all post-processing effect back to default/off state
    /// </summary>
    /// <remarks>
    /// Reset all post-processing effect back to default/off state.
    /// <seedoc>Features:Post-Processing Effects</seedoc>
    /// <seedoc>Features:Post-Processing Shaders</seedoc>
    /// </remarks>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/ripple");
    ///     RB.EffectShader(shaderEffect);
    /// }
    ///
    /// void Render() {
    ///     // Animate a value in the post processing shader to create a moving ripple effect
    ///     shaderEffect.FloatSet("wave", RB.Ticks / 25.0f);
    ///
    ///     // Reset all post processing effects if user presses F12
    ///     if (RB.KeyPressed(KeyCode.F12)) {
    ///         RB.EffectReset();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="RB.EffectSet"/>
    /// <seealso cref="RB.EffectShader"/>
    /// <seealso cref="RB.EffectFilter"/>
    /// <seealso cref="RB.EffectApplyNow"/>
    public static void EffectReset()
    {
        mInstance.mRetroBlitAPI.Effects.EffectReset();
    }

    /// <summary>
    /// Apply post-processing effects immediately
    /// </summary>
    /// <remarks>
    /// Apply post-processing effects immediately. This can be useful if the post processing effects should only be applied to certain
    /// layers of the scene. For example, it might be desirable to apply the <mref refid="RB.Effect.Shake">Shake</mref> effect to the game board, but not to the
    /// game UI.
    ///
    /// After calling <see cref="RB.EffectApplyNow"/> it is usually desirable to then reset post processing effects with <see cref="RB.EffectReset"/>,
    /// or change parameters of the effects currently applied with <see cref="RB.EffectSet"/>.
    ///
    /// <see cref="RB.EffectApplyNow"/> is called automatically at the end of <see cref="IRetroBlitGame.Render"/>.
    /// <seedoc>Features:Post-Processing Effects</seedoc>
    /// <seedoc>Features:Post-Processing Shaders</seedoc>
    /// </remarks>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/ripple");
    /// }
    ///
    /// void Render() {
    ///     // Enable a custom shader as a post processing effect, and render the game board
    ///     RB.EffectShader(shaderEffect);
    ///     shaderEffect.FloatSet("wave", RB.Ticks / 25.0f);
    ///     RenderGameBoard();
    ///
    ///     // Immediately apply post processing effect
    ///     RB.EffectApplyNow();
    ///
    ///     // Reset post processing effects and render the game UI without any effects.
    ///     RB.EffectReset();
    ///     RenderUI();
    /// }
    /// </code>
    /// <seealso cref="RB.EffectSet"/>
    /// <seealso cref="RB.EffectShader"/>
    /// <seealso cref="RB.EffectFilter"/>
    /// <seealso cref="RB.EffectReset"/>
    public static void EffectApplyNow()
    {
        mInstance.mRetroBlitAPI.Renderer.EffectApplyNow();
    }

    /// <summary>
    /// Set the current shader
    /// </summary>
    /// <remarks>
    /// Set the current shader previously loaded by <see cref="ShaderAsset.Load"/>. This shader will remain in effect until
    /// next call to <see cref="ShaderSet"/>, <see cref="RB.ShaderReset"/>, or when the current rendering frame ends.
    /// <seedoc>Features:Enabling a Shader</seedoc>
    /// </remarks>
    /// <param name="shader">Shader asset</param>
    /// <code>
    /// SpriteSheetAsset spriteTiles = new SpriteSheetAsset();
    /// SpriteSheetAsset offscreenRaw = new SpriteSheetAsset();
    /// SpriteSheetAsset offscreenRippleMask = new SpriteSheetAsset();
    ///
    /// ShaderAsset shaderRipple = new ShaderAsset();
    ///
    /// const int LAYER_TERRAIN = 0;
    /// const int LAYER_WATER = 1;
    ///
    /// void Initialize() {
    ///     spriteTiles.Load("spritesheet/tiles", new Vector2i(16, 16));
    ///     offscreenRaw.Load(RB.DisplaySize);
    ///     offscreenRippleMask.Load(RB.DisplaySize);
    ///
    ///     TMXMapAsset map = new TMXMapAsset();
    ///     map.Load("map/level1");
    ///     map.LoadLayer(map, "terrain", LAYER_TERRAIN);
    ///     map.LoadLayer(map, "water", LAYER_WATER);
    ///     RB.MapLayerSpriteSheetSet(LAYER_TERRAIN, spriteTiles);
    ///     RB.MapLayerSpriteSheetSet(LAYER_WATER, spriteTiles);
    ///
    ///     shaderRipple.Load("shaders/ripple");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spriteTiles);
    ///
    ///     // First draw the terrain tiles
    ///     RB.Offscreen(offscreenRaw);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    ///
    ///     // Now draw the ripple "mask" layer
    ///     RB.Offscreen(offscreenRippleMask);
    ///     RB.DrawMapLayer(LAYER_WATER);
    ///
    ///     // Combine the layers to create a rippling water effect
    ///     // First, set the source spritesheet DrawCopy will copy from
    ///     RB.SpriteSheetSet(offscreenRaw);
    ///
    ///     // Activate the shader
    ///     RB.ShaderSet(shaderRipple);
    ///
    ///     // Set shader properties, including an animating "wave" float value,
    ///     // and the source mask texture which will be used to animate pixels that
    ///     // are "under water"
    ///     shaderRipple.FloatSet("wave", RB.Ticks / 25.0f);
    ///     shaderRipple.SpriteSheetTextureSet("mask_tex", OFFSCREEN_RIPPLE_MASK);
    ///
    ///     // Draw with the shader enabled
    ///     RB.Onscreen();
    ///     RB.DrawCopy(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), Vector2i.zero);
    ///
    ///     // Turn off custom shader
    ///     RB.ShaderReset();
    /// }
    /// </code>
    /// <seealso cref="ShaderAsset"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="RB.ShaderColorSet"/>
    /// <seealso cref="RB.ShaderColorArraySet"/>
    /// <seealso cref="RB.ShaderFloatSet"/>
    /// <seealso cref="RB.ShaderFloatArrraySet"/>
    /// <seealso cref="RB.ShaderIntSet"/>
    /// <seealso cref="RB.ShaderMatrixSet"/>
    /// <seealso cref="RB.ShaderMatrixArraySet"/>
    /// <seealso cref="RB.ShaderVectorSet"/>
    /// <seealso cref="RB.ShaderVectorArraySet"/>
    /// <seealso cref="RB.ShaderSpriteSheetTextureSet"/>
    /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
    public static void ShaderSet(ShaderAsset shader)
    {
        if (shader == null || shader.status != AssetStatus.Ready)
        {
            return;
        }

        mInstance.mRetroBlitAPI.Renderer.ShaderSet(shader);
    }

    /// <summary>
    /// Apply shader immediately
    /// </summary>
    /// <remarks>
    /// Applies shader immediately by flushing out all drawing calls. This method should be used when changing some shader parameter between
    /// draw calls. This can be important because all drawing in RetroBlit is batched, when a batch is flushed it will use the latest
    /// shader parameters, which may not be what is desired.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset spritePackCharacters = new SpriteSheetAsset();
    /// ShaderAsset shaderEffects = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     spritePackCharacters.Load("spritesheet/tiles");
    ///     shaderEffects.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spritePackCharacters);
    ///
    ///     RB.ShaderSet(shaderEffects);
    ///
    ///     foreach (Character c in list) {
    ///         if (c.isPoisoned) {
    ///             shaderEffects.ColorSet("glow", Color.green);
    ///         } else {
    ///             shaderEffects.ColorSet("glow", Color.white);
    ///         }
    ///
    ///         RB.DrawSprite(c.sprite, c.pos);
    ///         RB.ShaderApplyNow();
    ///     }
    /// }
    /// </code>
    /// <seealso cref="ShaderAsset"/>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="RB.ShaderColorSet"/>
    /// <seealso cref="RB.ShaderColorArraySet"/>
    /// <seealso cref="RB.ShaderFloatSet"/>
    /// <seealso cref="RB.ShaderFloatArrraySet"/>
    /// <seealso cref="RB.ShaderIntSet"/>
    /// <seealso cref="RB.ShaderMatrixSet"/>
    /// <seealso cref="RB.ShaderMatrixArraySet"/>
    /// <seealso cref="RB.ShaderVectorSet"/>
    /// <seealso cref="RB.ShaderVectorArraySet"/>
    /// <seealso cref="RB.ShaderSpriteSheetTextureSet"/>
    /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
    public static void ShaderApplyNow()
    {
        mInstance.mRetroBlitAPI.Renderer.ShaderApplyNow();
    }

    /// <summary>
    /// Reset the shader back to the default RetroBlit shader
    /// </summary>
    /// <remarks>
    /// Reset the shader back to the default RetroBlit shader.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset spritePackCharacters = new SpriteSheetAsset();
    /// ShaderAsset shaderEffects = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     spritePackCharacters.Load("spritesheet/tiles");
    ///     shaderEffects.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spritePackCharacters);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffects);
    ///     shaderEffects.ColorSet("glow", Color.yellow);
    ///     RB.DrawSprite("glowing_hero", glowingHeroPos);
    ///
    ///     // Turn off shader and draw a sprite with default RetroBlit shader
    ///     RB.ShaderReset();
    ///     RB.DrawSprite("boring_hero", boringHeroPos);
    /// }
    /// </code>
    /// <seealso cref="ShaderAsset"/>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="RB.ShaderColorSet"/>
    /// <seealso cref="RB.ShaderColorArraySet"/>
    /// <seealso cref="RB.ShaderFloatSet"/>
    /// <seealso cref="RB.ShaderFloatArrraySet"/>
    /// <seealso cref="RB.ShaderIntSet"/>
    /// <seealso cref="RB.ShaderMatrixSet"/>
    /// <seealso cref="RB.ShaderMatrixArraySet"/>
    /// <seealso cref="RB.ShaderVectorSet"/>
    /// <seealso cref="RB.ShaderVectorArraySet"/>
    /// <seealso cref="RB.ShaderSpriteSheetTextureSet"/>
    /// <seealso cref="RB.ShaderSpriteSheetFilterSet"/>
    public static void ShaderReset()
    {
        mInstance.mRetroBlitAPI.Renderer.ShaderReset();
    }

    /// <summary>
    /// Defines hardware settings for the RetroBlit game
    /// </summary>
    /// <remarks>
    /// Defines hardware settings for the RetroBlit game. A RetroBlit Game fills out this structure in response to the call to <see cref="IRetroBlitGame.QueryHardware"/>.
    /// </remarks>
    public class HardwareSettings
    {
        /// <summary>
        /// Display size, dimensions must be divisible by 2
        /// </summary>
        /// <remarks>
        /// Display size, dimensions must be divisible by 2. The display dimensions can also be changed at runtime using <see cref="RB.DisplayModeSet"/>.
        /// </remarks>
        public Vector2i DisplaySize = new Vector2i(480, 270);

        /// <summary>
        /// Maximum tilemap size, in tiles
        /// </summary>
        /// <remarks>
        /// Maximum tilemap size, in tiles. This number should be kept close to the maximum size required by the game, in order to conserve memory. All tilemap layers
        /// will inherit this maximum size.
        /// </remarks>
        public Vector2i MapSize = new Vector2i(256, 256);

        /// <summary>
        /// Maximum amount of tilemap layers
        /// </summary>
        /// <remarks>
        /// Maximum amount of tilemap layers. This number should be kept close to the maximum layer count required by your game, in order to conserve memory.
        /// </remarks>
        public int MapLayers = 8;

        /// <summary>
        /// The internal size of a single map chunk
        /// </summary>
        /// <remarks>
        /// The internal size of a single map chunk. For majority of projects this value can be left at the default of 16 x 16, however a custom size might be desired if
        /// chunks of different sizes are being paged in manually for infinite maps or other purposes. Chunk size is a balance between fast drawing (large chunk size)
        /// or fast tilemap changes/updates (small chunk size).
        /// </remarks>
        public Vector2i MapChunkSize = new Vector2i(16, 16);

        /// <summary>
        /// Target frames per second. Defaults to 60
        /// </summary>
        /// <remarks>
        /// Target frames per second. Defaults to 60. This value reflects how many times a second the <see cref="IRetroBlitGame.Update"/> method will be called.
        /// </remarks>
        public int FPS = 60;

        /// <summary>
        /// Display pixel style
        /// </summary>
        /// <remarks>
        /// Display pixel style. Defaults to <mref refid="RB.PixelStyle.Square">Square</mref>, but can also be set to <mref refid="RB.PixelStyle.Wide">Wide</mref>,
        /// or <mref refid="RB.PixelStyle.Tall">Tall</mref>. The pixel style can also be changed at runtime using <see cref="RB.DisplayModeSet"/>.
        /// </remarks>
        public RB.PixelStyle PixelStyle = RB.PixelStyle.Square;
    }
}
