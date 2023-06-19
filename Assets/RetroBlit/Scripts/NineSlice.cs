/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// Defines a nine-slice image
/// </summary>
/// <remarks>
/// Defines a nine-slice image for use with <see cref="RB.DrawNineSlice"/>. See the <b>Constructor</b> documentation for various examples of
/// how to define a nine-slice image.
/// <seedoc>Features:Nine-Slice Sprite</seedoc>
/// </remarks>
public struct NineSlice
{
    /// <summary>
    /// True if this nine-slice is defined by source rectangles, rather than sprite ids
    /// </summary>
    /// <remarks>
    /// True if this nine-slice is defined by source rectangles, rather than sprite ids. This boolean is mostly used internally by RetroBlit.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    public bool IsRectBased;

    /// <summary>
    /// Top left corner
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the top left corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i TopLeftCornerRect;

    /// <summary>
    /// Top side
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the top side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    public Rect2i TopSideRect;

    /// <summary>
    /// Top right corner
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the top right corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i TopRightCornerRect;

    /// <summary>
    /// Left side
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the left side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i LeftSideRect;

    /// <summary>
    /// Middle
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the middle of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i MiddleRect;

    /// <summary>
    /// Right side
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the right side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i RightSideRect;

    /// <summary>
    /// Bottom left corner
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the bottom left corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i BottomLeftCornerRect;

    /// <summary>
    /// Bottom side
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the bottom side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i BottomSideRect;

    /// <summary>
    /// Bottom right corner
    /// </summary>
    /// <remarks>
    /// Sprite sheet source rectangle defining the bottom right corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public Rect2i BottomRightCornerRect;

    /// <summary>
    /// Top left corner
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the top left corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID TopLeftCornerID;

    /// <summary>
    /// Top side
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the top side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID TopSideID;

    /// <summary>
    /// Top right corner
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the top right corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID TopRightCornerID;

    /// <summary>
    /// Left side
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the left side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID LeftSideID;

    /// <summary>
    /// Middle
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the middle of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID MiddleID;

    /// <summary>
    /// Right side
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the right side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID RightSideID;

    /// <summary>
    /// Bottom left corner
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the bottom left corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID BottomLeftCornerID;

    /// <summary>
    /// Bottom side
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the bottom side of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID BottomSideID;

    /// <summary>
    /// Bottom right corner
    /// </summary>
    /// <remarks>
    /// Sprite ID defining the bottom right corner of the nine-slice image.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public PackedSpriteID BottomRightCornerID;

    /// <summary>
    /// Top left corner flags
    /// </summary>
    /// <remarks>
    /// Top left corner flags for rendering, for example <see cref="RB.FLIP_H"/>, or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsTopLeftCorner;

    /// <summary>
    /// Top side flags
    /// </summary>
    /// <remarks>
    /// Top side flags for rendering, for example <see cref="RB.FLIP_H"/>, or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsTopSide;

    /// <summary>
    /// Top right corner flags
    /// </summary>
    /// <remarks>
    /// Top right corner flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsTopRightCorner;

    /// <summary>
    /// Left side flags
    /// </summary>
    /// <remarks>
    /// Left side flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsLeftSide;

    /// <summary>
    /// Right side flags
    /// </summary>
    /// <remarks>
    /// Right side flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsRightSide;

    /// <summary>
    /// Bottom left corner flags
    /// </summary>
    /// <remarks>
    /// Bottom left corner flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsBottomLeftCorner;

    /// <summary>
    /// Bottom flags
    /// </summary>
    /// <remarks>
    /// Bottom side flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsBottomSide;

    /// <summary>
    /// Bottom right flags
    /// </summary>
    /// <remarks>
    /// Bottom right corner flags for rendering, for example <see cref="RB.FLIP_H"/> or <see cref="RB.ROT_90_CW"/>.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// </remarks>
    /// <seealso cref="RB.DrawNineSlice"/>
    public int FlagsBottomRightCorner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Creates a new NineSlice image definition. A nine-slice sprite is a special kind of sprite usually used to draw rectangular shapes for UI elements.
    /// A nine-slice sprite is defined by up to 9 separate sprites that are arranged and repeated as needed in such a way that they can
    /// fill any rectangular area.
    ///
    /// <image src="nineslice_parts.png">Nine parts that make up a nine slice image</image>
    ///
    /// The pieces *A*, *C*, *G*, and *I* are always drawn in the corners of the nine-slice sprite. *B* and *H* are repeated horizontally.
    /// *E* and *F* are repeated vertically. Finally, *X* is repeated in both directions to fill the middle of the nine-slice sprite.
    ///
    /// For best results each of these groups of pieces should be the same height:
    /// <ul>
    /// <li> *A*, *B*, *C*</li>
    /// <li> *E*, *F*</li>
    /// <li> *G*, *H*, *I*</li>
    /// </ul>
    /// These groups should be the same width:
    /// <ul>
    /// <li> *A*, *E*, *G*</li>
    /// <li> *B*, *H*</li>
    /// <li> *C*, *F*, *I*</li>
    /// </ul>
    /// The center piece *X* can be of any size.
    ///
    /// The pieces that make up a nine-slice image should not be too small to limit the amount of times they have to be repeated to fill
    /// your nine-slice sprite.
    ///
    /// In some cases, such as in the image above, many of the pieces are symmetric, for convenience there are <see cref="NineSlice.NineSlice"/> overloads that
    /// take only *srcTopLeftCornerRect*, *srcTopSideRect*, and *srcMiddleRect* and simply rotate and mirror the remaining pieces automatically.
    ///
    /// You can specify the pieces of a nine-slice image through either source rectangles, or sprite ids for sprites in a sprite pack.
    /// <seedoc>Features:Nine-Slice Sprite</seedoc>
    /// <seedoc>Features:Sprite Sheets</seedoc>
    /// <seedoc>Features:Sprite Packs</seedoc>
    /// </remarks>
    /// <param name="topLeftCornerRect">Top left corner rect</param>
    /// <param name="topSideRect">Top side rect</param>
    /// <param name="topRightCornerRect">Top right corner rect</param>
    /// <param name="leftSideRect">Left side rect</param>
    /// <param name="middleRect">Middle rect</param>
    /// <param name="rightSideRect">Right side rect</param>
    /// <param name="bottomLeftCornerRect">Bottom left corner rect</param>
    /// <param name="bottomSideRect">Bottom side rect</param>
    /// <param name="bottomRightCornerRect">Bottom right corner rect</param>
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
    /// <seealso cref="RB.DrawNineSlice"/>
    /// <seealso cref="RB.SpriteSheetSet"/>
    /// <seealso cref="SpriteSheetAsset"/>
    public NineSlice(
        Rect2i topLeftCornerRect,
        Rect2i topSideRect,
        Rect2i topRightCornerRect,
        Rect2i leftSideRect,
        Rect2i middleRect,
        Rect2i rightSideRect,
        Rect2i bottomLeftCornerRect,
        Rect2i bottomSideRect,
        Rect2i bottomRightCornerRect)
    {
        TopLeftCornerRect = topLeftCornerRect;
        TopSideRect = topSideRect;
        TopRightCornerRect = topRightCornerRect;
        LeftSideRect = leftSideRect;
        MiddleRect = middleRect;
        RightSideRect = rightSideRect;
        BottomLeftCornerRect = bottomLeftCornerRect;
        BottomSideRect = bottomSideRect;
        BottomRightCornerRect = bottomRightCornerRect;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;

        PackedSpriteID empty = new PackedSpriteID(0);

        TopLeftCornerID = empty;
        TopSideID = empty;
        TopRightCornerID = empty;
        LeftSideID = empty;
        MiddleID = empty;
        RightSideID = empty;
        BottomLeftCornerID = empty;
        BottomSideID = empty;
        BottomRightCornerID = empty;

        IsRectBased = true;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCornerRect">Top left corner rect</param>
    /// <param name="topSideRect">Top side rect</param>
    /// <param name="middleRect">Middle rect</param>
    public NineSlice(Rect2i topLeftCornerRect, Rect2i topSideRect, Rect2i middleRect)
    {
        TopLeftCornerRect = topLeftCornerRect;
        TopSideRect = topSideRect;
        TopRightCornerRect = topLeftCornerRect;
        LeftSideRect = topSideRect;
        MiddleRect = middleRect;
        RightSideRect = topSideRect;
        BottomLeftCornerRect = topLeftCornerRect;
        BottomSideRect = topSideRect;
        BottomRightCornerRect = topLeftCornerRect;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = RB.FLIP_H;
        FlagsLeftSide = RB.ROT_90_CCW;
        FlagsRightSide = RB.ROT_90_CW;
        FlagsBottomLeftCorner = RB.FLIP_V;
        FlagsBottomSide = RB.FLIP_V;
        FlagsBottomRightCorner = RB.FLIP_H | RB.FLIP_V;

        PackedSpriteID empty = new PackedSpriteID(0);

        TopLeftCornerID = empty;
        TopSideID = empty;
        TopRightCornerID = empty;
        LeftSideID = empty;
        MiddleID = empty;
        RightSideID = empty;
        BottomLeftCornerID = empty;
        BottomSideID = empty;
        BottomRightCornerID = empty;

        IsRectBased = true;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="topLeftCornerID">Top left corner sprite ID</param>
    /// <param name="topSideID">Top side sprite ID</param>
    /// <param name="topRightCornerID">Top right corner sprite ID</param>
    /// <param name="leftSideID">Left side sprite ID</param>
    /// <param name="middleID">Middle sprite ID</param>
    /// <param name="rightSideID">Right side sprite ID</param>
    /// <param name="bottomLeftCornerID">Bottom left corner sprite ID</param>
    /// <param name="bottomSideID">Bottom side sprite ID</param>
    /// <param name="bottomRightCornerID">Bottom right corner sprite ID</param>
    public NineSlice(
        PackedSpriteID topLeftCornerID,
        PackedSpriteID topSideID,
        PackedSpriteID topRightCornerID,
        PackedSpriteID leftSideID,
        PackedSpriteID middleID,
        PackedSpriteID rightSideID,
        PackedSpriteID bottomLeftCornerID,
        PackedSpriteID bottomSideID,
        PackedSpriteID bottomRightCornerID)
    {
        TopLeftCornerID = topLeftCornerID;
        TopSideID = topSideID;
        TopRightCornerID = topRightCornerID;
        LeftSideID = leftSideID;
        MiddleID = middleID;
        RightSideID = rightSideID;
        BottomLeftCornerID = bottomLeftCornerID;
        BottomSideID = bottomSideID;
        BottomRightCornerID = bottomRightCornerID;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCornerID">Top left corner sprite ID</param>
    /// <param name="topSideID">Top side sprite ID</param>
    /// <param name="middleID">Middle sprite ID</param>
    public NineSlice(PackedSpriteID topLeftCornerID, PackedSpriteID topSideID, PackedSpriteID middleID)
    {
        TopLeftCornerID = topLeftCornerID;
        TopSideID = topSideID;
        TopRightCornerID = topLeftCornerID;
        LeftSideID = topSideID;
        MiddleID = middleID;
        RightSideID = topSideID;
        BottomLeftCornerID = topLeftCornerID;
        BottomSideID = topSideID;
        BottomRightCornerID = topLeftCornerID;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = RB.FLIP_H;
        FlagsLeftSide = RB.ROT_90_CCW;
        FlagsRightSide = RB.ROT_90_CW;
        FlagsBottomLeftCorner = RB.FLIP_V;
        FlagsBottomSide = RB.FLIP_V;
        FlagsBottomRightCorner = RB.FLIP_H | RB.FLIP_V;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="topLeftCorner">Top left corner PackedSprite</param>
    /// <param name="topSide">Top side PackedSprite</param>
    /// <param name="topRightCorner">Top right corner PackedSprite</param>
    /// <param name="leftSide">Left side PackedSprite</param>
    /// <param name="middle">Middle PackedSprite</param>
    /// <param name="rightSide">Right side PackedSprite</param>
    /// <param name="bottomLeftCorner">Bottom left corner PackedSprite</param>
    /// <param name="bottomSide">Bottom side PackedSprite</param>
    /// <param name="bottomRightCorner">Bottom right corner PackedSprite</param>
    public NineSlice(
        PackedSprite topLeftCorner,
        PackedSprite topSide,
        PackedSprite topRightCorner,
        PackedSprite leftSide,
        PackedSprite middle,
        PackedSprite rightSide,
        PackedSprite bottomLeftCorner,
        PackedSprite bottomSide,
        PackedSprite bottomRightCorner)
    {
        TopLeftCornerID = topLeftCorner.id;
        TopSideID = topSide.id;
        TopRightCornerID = topRightCorner.id;
        LeftSideID = leftSide.id;
        MiddleID = middle.id;
        RightSideID = rightSide.id;
        BottomLeftCornerID = bottomLeftCorner.id;
        BottomSideID = bottomSide.id;
        BottomRightCornerID = bottomRightCorner.id;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCorner">Top left corner PackedSprite</param>
    /// <param name="topSide">Top side PackedSprite</param>
    /// <param name="middle">Middle PackedSprite</param>
    public NineSlice(PackedSprite topLeftCorner, PackedSprite topSide, PackedSprite middle)
    {
        TopLeftCornerID = topLeftCorner.id;
        TopSideID = topSide.id;
        TopRightCornerID = topLeftCorner.id;
        LeftSideID = topSide.id;
        MiddleID = middle.id;
        RightSideID = topSide.id;
        BottomLeftCornerID = topLeftCorner.id;
        BottomSideID = topSide.id;
        BottomRightCornerID = topLeftCorner.id;

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = RB.FLIP_H;
        FlagsLeftSide = RB.ROT_90_CCW;
        FlagsRightSide = RB.ROT_90_CW;
        FlagsBottomLeftCorner = RB.FLIP_V;
        FlagsBottomSide = RB.FLIP_V;
        FlagsBottomRightCorner = RB.FLIP_H | RB.FLIP_V;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="topLeftCornerName">Top left corner sprite name</param>
    /// <param name="topSideName">Top side sprite name</param>
    /// <param name="topRightCornerName">Top right corner sprite name</param>
    /// <param name="leftSideName">Left side sprite name</param>
    /// <param name="middleName">Middle sprite name</param>
    /// <param name="rightSideName">Right side sprite name</param>
    /// <param name="bottomLeftCornerName">Bottom left corner sprite name</param>
    /// <param name="bottomSideName">Bottom side sprite name</param>
    /// <param name="bottomRightCornerName">Bottom right corner sprite name</param>
    public NineSlice(
        string topLeftCornerName,
        string topSideName,
        string topRightCornerName,
        string leftSideName,
        string middleName,
        string rightSideName,
        string bottomLeftCornerName,
        string bottomSideName,
        string bottomRightCornerName)
    {
        TopLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        TopSideID = RB.PackedSpriteID(topSideName);
        TopRightCornerID = RB.PackedSpriteID(topRightCornerName);
        LeftSideID = RB.PackedSpriteID(leftSideName);
        MiddleID = RB.PackedSpriteID(middleName);
        RightSideID = RB.PackedSpriteID(rightSideName);
        BottomLeftCornerID = RB.PackedSpriteID(bottomLeftCornerName);
        BottomSideID = RB.PackedSpriteID(bottomSideName);
        BottomRightCornerID = RB.PackedSpriteID(bottomRightCornerName);

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCornerName">Top left corner sprite name</param>
    /// <param name="topSideName">Top side sprite name</param>
    /// <param name="middleName">Middle sprite name</param>
    public NineSlice(string topLeftCornerName, string topSideName, string middleName)
    {
        TopLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        TopSideID = RB.PackedSpriteID(topSideName);
        TopRightCornerID = RB.PackedSpriteID(topLeftCornerName);
        LeftSideID = RB.PackedSpriteID(topSideName);
        MiddleID = RB.PackedSpriteID(middleName);
        RightSideID = RB.PackedSpriteID(topSideName);
        BottomLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        BottomSideID = RB.PackedSpriteID(topSideName);
        BottomRightCornerID = RB.PackedSpriteID(topLeftCornerName);

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = RB.FLIP_H;
        FlagsLeftSide = RB.ROT_90_CCW;
        FlagsRightSide = RB.ROT_90_CW;
        FlagsBottomLeftCorner = RB.FLIP_V;
        FlagsBottomSide = RB.FLIP_V;
        FlagsBottomRightCorner = RB.FLIP_H | RB.FLIP_V;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="topLeftCornerName">Top left corner sprite name</param>
    /// <param name="topSideName">Top side sprite name</param>
    /// <param name="topRightCornerName">Top right corner sprite name</param>
    /// <param name="leftSideName">Left side sprite name</param>
    /// <param name="middleName">Middle sprite name</param>
    /// <param name="rightSideName">Right side sprite name</param>
    /// <param name="bottomLeftCornerName">Bottom left corner sprite name</param>
    /// <param name="bottomSideName">Bottom side sprite name</param>
    /// <param name="bottomRightCornerName">Bottom right corner sprite name</param>
    public NineSlice(
        FastString topLeftCornerName,
        FastString topSideName,
        FastString topRightCornerName,
        FastString leftSideName,
        FastString middleName,
        FastString rightSideName,
        FastString bottomLeftCornerName,
        FastString bottomSideName,
        FastString bottomRightCornerName)
    {
        TopLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        TopSideID = RB.PackedSpriteID(topSideName);
        TopRightCornerID = RB.PackedSpriteID(topRightCornerName);
        LeftSideID = RB.PackedSpriteID(leftSideName);
        MiddleID = RB.PackedSpriteID(middleName);
        RightSideID = RB.PackedSpriteID(rightSideName);
        BottomLeftCornerID = RB.PackedSpriteID(bottomLeftCornerName);
        BottomSideID = RB.PackedSpriteID(bottomSideName);
        BottomRightCornerID = RB.PackedSpriteID(bottomRightCornerName);

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = 0;
        FlagsLeftSide = 0;
        FlagsRightSide = 0;
        FlagsBottomLeftCorner = 0;
        FlagsBottomSide = 0;
        FlagsBottomRightCorner = 0;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Simple nine-slice constructor, all corners and sides are mirrored and rotated
    /// </summary>
    /// <param name="topLeftCornerName">Top left corner sprite name</param>
    /// <param name="topSideName">Top side rect sprite name</param>
    /// <param name="middleName">Middle rect sprite name</param>
    public NineSlice(FastString topLeftCornerName, FastString topSideName, FastString middleName)
    {
        TopLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        TopSideID = RB.PackedSpriteID(topSideName);
        TopRightCornerID = RB.PackedSpriteID(topLeftCornerName);
        LeftSideID = RB.PackedSpriteID(topSideName);
        MiddleID = RB.PackedSpriteID(middleName);
        RightSideID = RB.PackedSpriteID(topSideName);
        BottomLeftCornerID = RB.PackedSpriteID(topLeftCornerName);
        BottomSideID = RB.PackedSpriteID(topSideName);
        BottomRightCornerID = RB.PackedSpriteID(topLeftCornerName);

        FlagsTopLeftCorner = 0;
        FlagsTopSide = 0;
        FlagsTopRightCorner = RB.FLIP_H;
        FlagsLeftSide = RB.ROT_90_CCW;
        FlagsRightSide = RB.ROT_90_CW;
        FlagsBottomLeftCorner = RB.FLIP_V;
        FlagsBottomSide = RB.FLIP_V;
        FlagsBottomRightCorner = RB.FLIP_H | RB.FLIP_V;

        TopLeftCornerRect = Rect2i.zero;
        TopSideRect = Rect2i.zero;
        TopRightCornerRect = Rect2i.zero;
        LeftSideRect = Rect2i.zero;
        MiddleRect = Rect2i.zero;
        RightSideRect = Rect2i.zero;
        BottomLeftCornerRect = Rect2i.zero;
        BottomSideRect = Rect2i.zero;
        BottomRightCornerRect = Rect2i.zero;

        IsRectBased = false;
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <remarks>
    /// Convert to string.
    /// </remarks>
    /// <returns>String</returns>
    public override string ToString()
    {
        if (IsRectBased)
        {
            return string.Format(
                "(RectBased: [TopLeftCornerRect: {0} TopSideRect: {1} TopRightCornerRect: {2} LeftSideRect: {3} MiddleRect: {4} RightSideRect: {5} BottomLeftCornerRect: {6} BottomSideRect: {7} BottomRightCornerRect: {8}) Flags: [TopLeftCorner: {9} TopSide: {10} TopRightCorner: {11} LeftSide: {12} RightSide: {13} BottomLeftCorner: {14} BottomSide: {15} BottomRightCorner: {16}]",
                new object[]
                {
                    TopLeftCornerRect.ToString(),
                    TopSideRect.ToString(),
                    TopRightCornerRect.ToString(),
                    LeftSideRect.ToString(),
                    MiddleRect.ToString(),
                    RightSideRect.ToString(),
                    BottomLeftCornerRect.ToString(),
                    BottomSideRect.ToString(),
                    BottomRightCornerRect.ToString(),
                    FlagsTopLeftCorner,
                    FlagsTopSide,
                    FlagsTopRightCorner,
                    FlagsLeftSide,
                    FlagsRightSide,
                    FlagsBottomLeftCorner,
                    FlagsBottomSide,
                    FlagsBottomRightCorner
                });
        }
        else
        {
            return string.Format(
                "(IDBased: [TopLeftCornerID: {0} TopSideID: {1} TopRightCornerID: {2} LeftSideID: {3} MiddleID: {4} RightSideID: {5} BottomLeftCornerID: {6} BottomSideID: {7} BottomRightCornerID: {8}) Flags: [TopLeftCorner: {9} TopSide: {10} TopRightCorner: {11} LeftSide: {12} RightSide: {13} BottomLeftCorner: {14} BottomSide: {15} BottomRightCorner: {16}]",
                new object[]
                {
                    TopLeftCornerID.ToString(),
                    TopSideID.ToString(),
                    TopRightCornerID.ToString(),
                    LeftSideID.ToString(),
                    MiddleID.ToString(),
                    RightSideID.ToString(),
                    BottomLeftCornerID.ToString(),
                    BottomSideID.ToString(),
                    BottomRightCornerID.ToString(),
                    FlagsTopLeftCorner,
                    FlagsTopSide,
                    FlagsTopRightCorner,
                    FlagsLeftSide,
                    FlagsRightSide,
                    FlagsBottomLeftCorner,
                    FlagsBottomSide,
                    FlagsBottomRightCorner
                });
        }
    }

    /// <summary>
    /// NineSlice equality
    /// </summary>
    /// <remarks>
    /// NineSlice equality
    /// </remarks>
    /// <param name="other">Other NineSlice</param>
    /// <returns>True if equal</returns>
    public bool Equals(NineSlice other)
    {
        return
            IsRectBased == other.IsRectBased &&
            TopLeftCornerRect == other.TopLeftCornerRect &&
            TopSideRect == other.TopSideRect &&
            TopRightCornerRect == other.TopRightCornerRect &&
            LeftSideRect == other.LeftSideRect &&
            MiddleRect == other.MiddleRect &&
            RightSideRect == other.RightSideRect &&
            BottomLeftCornerRect == other.BottomLeftCornerRect &&
            BottomSideRect == other.BottomSideRect &&
            BottomRightCornerRect == other.BottomRightCornerRect &&
            TopLeftCornerID == other.TopLeftCornerID &&
            TopSideID == other.TopSideID &&
            TopRightCornerID == other.TopRightCornerID &&
            LeftSideID == other.LeftSideID &&
            MiddleID == other.MiddleID &&
            RightSideID == other.RightSideID &&
            BottomLeftCornerID == other.BottomLeftCornerID &&
            BottomSideID == other.BottomSideID &&
            BottomRightCornerID == other.BottomRightCornerID &&
            FlagsTopLeftCorner == other.FlagsTopLeftCorner &&
            FlagsTopSide == other.FlagsTopSide &&
            FlagsTopRightCorner == other.FlagsTopRightCorner &&
            FlagsLeftSide == other.FlagsLeftSide &&
            FlagsRightSide == other.FlagsRightSide &&
            FlagsBottomLeftCorner == other.FlagsBottomLeftCorner &&
            FlagsBottomSide == other.FlagsBottomSide &&
            FlagsBottomRightCorner == other.FlagsBottomRightCorner;
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <remarks>
    /// Object equality.
    /// </remarks>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

        if (!(other is NineSlice slice))
        {
            result = false;
        }
        else
        {
            NineSlice ns = slice;
            result =
                IsRectBased == ns.IsRectBased &&
                TopLeftCornerRect == ns.TopLeftCornerRect &&
                TopSideRect == ns.TopSideRect &&
                TopRightCornerRect == ns.TopRightCornerRect &&
                LeftSideRect == ns.LeftSideRect &&
                MiddleRect == ns.MiddleRect &&
                RightSideRect == ns.RightSideRect &&
                BottomLeftCornerRect == ns.BottomLeftCornerRect &&
                BottomSideRect == ns.BottomSideRect &&
                BottomRightCornerRect == ns.BottomRightCornerRect &&
                TopLeftCornerID == ns.TopLeftCornerID &&
                TopSideID == ns.TopSideID &&
                TopRightCornerID == ns.TopRightCornerID &&
                LeftSideID == ns.LeftSideID &&
                MiddleID == ns.MiddleID &&
                RightSideID == ns.RightSideID &&
                BottomLeftCornerID == ns.BottomLeftCornerID &&
                BottomSideID == ns.BottomSideID &&
                BottomRightCornerID == ns.BottomRightCornerID &&
                FlagsTopLeftCorner == ns.FlagsTopLeftCorner &&
                FlagsTopSide == ns.FlagsTopSide &&
                FlagsTopRightCorner == ns.FlagsTopRightCorner &&
                FlagsLeftSide == ns.FlagsLeftSide &&
                FlagsRightSide == ns.FlagsRightSide &&
                FlagsBottomLeftCorner == ns.FlagsBottomLeftCorner &&
                FlagsBottomSide == ns.FlagsBottomSide &&
                FlagsBottomRightCorner == ns.FlagsBottomRightCorner;
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <remarks>
    /// Get the hash code of the vector.
    /// </remarks>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return
            this.IsRectBased.GetHashCode() ^
            this.TopLeftCornerRect.GetHashCode() ^
            this.TopSideRect.GetHashCode() ^
            this.TopRightCornerRect.GetHashCode() ^
            this.LeftSideRect.GetHashCode() ^
            this.MiddleRect.GetHashCode() ^
            this.RightSideRect.GetHashCode() ^
            this.BottomLeftCornerRect.GetHashCode() ^
            this.BottomSideRect.GetHashCode() ^
            this.BottomRightCornerRect.GetHashCode() ^
            this.TopLeftCornerID.GetHashCode() ^
            this.TopSideID.GetHashCode() ^
            this.TopRightCornerID.GetHashCode() ^
            this.LeftSideID.GetHashCode() ^
            this.MiddleID.GetHashCode() ^
            this.RightSideID.GetHashCode() ^
            this.BottomLeftCornerID.GetHashCode() ^
            this.BottomSideID.GetHashCode() ^
            this.BottomRightCornerID.GetHashCode() ^
            this.FlagsTopLeftCorner.GetHashCode() ^
            this.FlagsTopSide.GetHashCode() ^
            this.FlagsTopRightCorner.GetHashCode() ^
            this.FlagsLeftSide.GetHashCode() ^
            this.FlagsRightSide.GetHashCode() ^
            this.FlagsBottomLeftCorner.GetHashCode() ^
            this.FlagsBottomSide.GetHashCode() ^
            this.FlagsBottomRightCorner.GetHashCode();
    }
}
