namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// Renderer subsystem
    /// </summary>
    public sealed partial class RBRenderer
    {
        /// <summary>
        /// Draw a vertical line with no checks
        /// </summary>
        /// <param name="x">Start x</param>
        /// <param name="y1">Start y</param>
        /// <param name="y2">End y</param>
        public void DrawVerticalLineNoChecks(int x, int y1, int y2)
        {
            x -= mCameraPos.x;
            y1 -= mCameraPos.y;
            y2 -= mCameraPos.y;

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = mCurrentColor.r;
            v.color_g = mCurrentColor.g;
            v.color_b = mCurrentColor.b;
            v.color_a = mCurrentColor.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = x - 0.1f;
            v.pos_y = y1 - 0.1f;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = x + 1.1f;
            v.pos_y = y1 - 0.1f;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = x + 0.5f;
            v.pos_y = y2 + 1.1f;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw a vertical line with no checks
        /// </summary>
        /// <param name="x1">Start x</param>
        /// <param name="x2">End x</param>
        /// <param name="y">Start y</param>
        public void DrawHorizontalLineNoChecks(int x1, int x2, int y)
        {
            x1 -= mCameraPos.x;
            x2 -= mCameraPos.x;
            y -= mCameraPos.y;

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = mCurrentColor.r;
            v.color_g = mCurrentColor.g;
            v.color_b = mCurrentColor.b;
            v.color_a = mCurrentColor.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = x1 - 0.1f;
            v.pos_y = y - 0.1f;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = x2 + 1.1f;
            v.pos_y = y + 0.5f;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = x1 - 0.1f;
            v.pos_y = y + 1.1f;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="color">RGB color</param>
        public void DrawLine(int lp0X, int lp0Y, int lp1X, int lp1Y, Color32 color)
        {
            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            // Trivial straight lines, use ortho, faster
            if (lp0X == lp1X || lp0Y == lp1Y)
            {
                DrawOrthoLine(lp0X, lp0Y, lp1X, lp1Y, color);
                return;
            }

            // Fast color multiply
            color.r = (byte)((color.r * mCurrentColor.r) / 255);
            color.g = (byte)((color.g * mCurrentColor.g) / 255);
            color.b = (byte)((color.b * mCurrentColor.b) / 255);
            color.a = (byte)((color.a * mCurrentColor.a) / 255);

            float lp0fX = lp0X;
            float lp0fY = lp0Y;
            float lp1fX = lp1X;
            float lp1fY = lp1Y;

            lp0fX -= mCameraPos.x;
            lp0fY -= mCameraPos.y;

            lp1fX -= mCameraPos.x;
            lp1fY -= mCameraPos.y;

            // Early clip test
            if (lp0fX < mClipRegion.x0 && lp1fX < mClipRegion.x0)
            {
                return;
            }
            else if (lp0fX > mClipRegion.x1 && lp1fX > mClipRegion.x1)
            {
                return;
            }
            else if (lp0fY < mClipRegion.y0 && lp1fY < mClipRegion.y0)
            {
                return;
            }
            else if (lp0fY > mClipRegion.y1 && lp1fY > mClipRegion.y1)
            {
                return;
            }

            float dirX = lp1fX - lp0fX;
            float dirY = lp1fY - lp0fY;
            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            /* Figure out which quadrant the angle is in
             *
             * \      0     /
             *   \        /
             *     \    /
             *       \/
             * 3     /\     1
             *     /    \
             *   /        \
             * /      2     \
             */
            if (System.Math.Abs(dirX) > System.Math.Abs(dirY))
            {
                if (dirX > 0)
                {
                    // quadrant 1
                    p0X = lp0fX;
                    p0Y = lp0fY;
                    p1X = lp1fX;
                    p1Y = lp1fY;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;
                }
                else
                {
                    // quadrant 3
                    p1X = lp0fX;
                    p1Y = lp0fY;
                    p0X = lp1fX;
                    p0Y = lp1fY;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;
                }
            }
            else
            {
                if (dirY < 0)
                {
                    // quadrant 0
                    p0X = lp0fX;
                    p0Y = lp0fY;
                    p1X = lp1fX;
                    p1Y = lp1fY;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;
                }
                else
                {
                    // quadrant 2
                    p1X = lp0fX;
                    p1Y = lp0fY;
                    p0X = lp1fX;
                    p0Y = lp1fY;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = color.r;
            v.color_g = color.g;
            v.color_b = color.b;
            v.color_a = color.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="thickness_i">Thickness of the line</param>
        /// <param name="color">RGB color</param>
        public void DrawLine(int lp0X, int lp0Y, int lp1X, int lp1Y, int thickness_i, Color32 color)
        {
            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            int clip_half_thickness = (int)((thickness_i + 0.5f) / 2);

            // Fast color multiply
            color.r = (byte)((color.r * mCurrentColor.r) / 255);
            color.g = (byte)((color.g * mCurrentColor.g) / 255);
            color.b = (byte)((color.b * mCurrentColor.b) / 255);
            color.a = (byte)((color.a * mCurrentColor.a) / 255);

            float lp0fX = lp0X;
            float lp0fY = lp0Y;
            float lp1fX = lp1X;
            float lp1fY = lp1Y;

            lp0fX -= mCameraPos.x;
            lp0fY -= mCameraPos.y;

            lp1fX -= mCameraPos.x;
            lp1fY -= mCameraPos.y;

            // Early clip test
            if (lp0fX + clip_half_thickness < mClipRegion.x0 && lp1fX + clip_half_thickness < mClipRegion.x0)
            {
                return;
            }
            else if (lp0fX - clip_half_thickness > mClipRegion.x1 && lp1fX - clip_half_thickness > mClipRegion.x1)
            {
                return;
            }
            else if (lp0fY + clip_half_thickness < mClipRegion.y0 && lp1fY + clip_half_thickness < mClipRegion.y0)
            {
                return;
            }
            else if (lp0fY - clip_half_thickness > mClipRegion.y1 && lp1fY - clip_half_thickness > mClipRegion.y1)
            {
                return;
            }

            float thicknessTop;
            float thicknessBottom;

            // For thickness that is not even we have to make one side of the line 1 pixel thicker
            if (thickness_i % 2 == 0)
            {
                thicknessTop = thickness_i / 2;
                thicknessBottom = thicknessTop;
            }
            else
            {
                thicknessTop = thickness_i / 2;
                thicknessBottom = thicknessTop + 1;
            }

            // Calculate line normal
            float dX = lp1fX - lp0fX;
            float dY = lp1fY - lp0fY;
            float tX = dY;
            float tY = -dX;

            float len = Mathf.Sqrt((tX * tX) + (tY * tY));
            tX /= len;
            tY /= len;

            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            p0X = lp0fX - (tX * thicknessTop);
            p0Y = lp0fY - (tY * thicknessTop);

            p3X = lp1fX - (tX * thicknessTop);
            p3Y = lp1fY - (tY * thicknessTop);

            p1X = lp0fX + (tX * thicknessBottom);
            p1Y = lp0fY + (tY * thicknessBottom);

            p2X = lp1fX + (tX * thicknessBottom);
            p2Y = lp1fY + (tY * thicknessBottom);

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = color.r;
            v.color_g = color.g;
            v.color_b = color.b;
            v.color_a = color.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="color">RGB color</param>
        /// <param name="pivotX">Rotation pivot point x</param>
        /// <param name="pivotY">Rotation pivot point y</param>
        /// <param name="rotation">Rotation in degrees</param>
        public void DrawLine(int lp0X, int lp0Y, int lp1X, int lp1Y, Color32 color, int pivotX, int pivotY, float rotation = 0)
        {
            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            rotation = RBUtil.WrapAngle(rotation);

            if (rotation == 0)
            {
                if (lp0X == lp1X || lp0Y == lp1Y)
                {
                    DrawOrthoLine(lp0X, lp0Y, lp1X, lp1Y, color);
                    return;
                }
            }
            else
            {
                // Adjust pivot so that it originates from top left corner of an imaginary rectangle that would encompass the line
                int topX = lp0X < lp1X ? lp0X : lp1X;
                int topY = lp0Y < lp1Y ? lp0Y : lp1Y;

                pivotX += topX;
                pivotY += topY;
            }

            // Fast color multiply
            color.r = (byte)((color.r * mCurrentColor.r) / 255);
            color.g = (byte)((color.g * mCurrentColor.g) / 255);
            color.b = (byte)((color.b * mCurrentColor.b) / 255);
            color.a = (byte)((color.a * mCurrentColor.a) / 255);

            Vector3 lp0f = new Vector3(lp0X, lp0Y, 0);
            Vector3 lp1f = new Vector3(lp1X, lp1Y, 0);

            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotation), Vector3.one);
            matrix *= Matrix4x4.TRS(new Vector3(-pivotX, -pivotY, 0), Quaternion.identity, Vector3.one);

            lp0f = matrix.MultiplyPoint3x4(lp0f);
            lp1f = matrix.MultiplyPoint3x4(lp1f);

            lp0f.x += pivotX;
            lp0f.y += pivotY;

            lp1f.x += pivotX;
            lp1f.y += pivotY;

            lp0f.x -= mCameraPos.x;
            lp0f.y -= mCameraPos.y;

            lp1f.x -= mCameraPos.x;
            lp1f.y -= mCameraPos.y;

            // Early clip test
            if (lp0f.x < mClipRegion.x0 && lp1f.x < mClipRegion.x0)
            {
                return;
            }
            else if (lp0f.x > mClipRegion.x1 && lp1f.x > mClipRegion.x1)
            {
                return;
            }
            else if (lp0f.y < mClipRegion.y0 && lp1f.y < mClipRegion.y0)
            {
                return;
            }
            else if (lp0f.y > mClipRegion.y1 && lp1f.y > mClipRegion.y1)
            {
                return;
            }

            float dirX = lp1f.x - lp0f.x;
            float dirY = lp1f.y - lp0f.y;
            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            /* Figure out which quadrant the angle is in
             *
             * \      0     /
             *   \        /
             *     \    /
             *       \/
             * 3     /\     1
             *     /    \
             *   /        \
             * /      2     \
             */
            if (System.Math.Abs(dirX) > System.Math.Abs(dirY))
            {
                if (dirX > 0)
                {
                    // quadrant 1
                    p0X = lp0f.x;
                    p0Y = lp0f.y;
                    p1X = lp1f.x;
                    p1Y = lp1f.y;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;
                }
                else
                {
                    // quadrant 3
                    p1X = lp0f.x;
                    p1Y = lp0f.y;
                    p0X = lp1f.x;
                    p0Y = lp1f.y;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;
                }
            }
            else
            {
                if (dirY < 0)
                {
                    // quadrant 0
                    p0X = lp0f.x;
                    p0Y = lp0f.y;
                    p1X = lp1f.x;
                    p1Y = lp1f.y;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;
                }
                else
                {
                    // quadrant 2
                    p1X = lp0f.x;
                    p1Y = lp0f.y;
                    p0X = lp1f.x;
                    p0Y = lp1f.y;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = color.r;
            v.color_g = color.g;
            v.color_b = color.b;
            v.color_a = color.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="color">RGB color</param>
        /// <param name="startPixel">Render the first pixel in the line</param>
        /// <param name="endPixel">Render the last pixel in the line</param>
        public void DrawLineWithCaps(int lp0X, int lp0Y, int lp1X, int lp1Y, Color32 color, bool startPixel = true, bool endPixel = true)
        {
            //// CheckFlush should already be called by caller

            if (lp0X == lp1X && lp0Y == lp1Y)
            {
                if (!startPixel && !endPixel)
                {
                    // Do nothing
                    return;
                }

                DrawPixel(lp0X, lp0Y, color);
                return;
            }

            // Trivial straight lines, use rect, faster
            if (lp0X == lp1X)
            {
                if (!startPixel)
                {
                    if (lp0Y < lp1Y)
                    {
                        lp0Y++;
                    }
                    else
                    {
                        lp0Y--;
                    }
                }

                if (!endPixel)
                {
                    if (lp0Y < lp1Y)
                    {
                        lp1Y--;
                    }
                    else
                    {
                        lp1Y++;
                    }
                }

                DrawOrthoLine(lp0X, lp0Y, lp1X, lp1Y, color);
                return;
            }

            if (lp0Y == lp1Y)
            {
                if (!startPixel)
                {
                    if (lp0X < lp1X)
                    {
                        lp0X++;
                    }
                    else
                    {
                        lp0X--;
                    }
                }

                if (!endPixel)
                {
                    if (lp0X < lp1X)
                    {
                        lp1X--;
                    }
                    else
                    {
                        lp1X++;
                    }
                }

                DrawOrthoLine(lp0X, lp0Y, lp1X, lp1Y, color);
                return;
            }

            // Fast color multiply
            color.r = (byte)((color.r * mCurrentColor.r) / 255);
            color.g = (byte)((color.g * mCurrentColor.g) / 255);
            color.b = (byte)((color.b * mCurrentColor.b) / 255);
            color.a = (byte)((color.a * mCurrentColor.a) / 255);

            lp0X -= mCameraPos.x;
            lp0Y -= mCameraPos.y;

            lp1X -= mCameraPos.x;
            lp1Y -= mCameraPos.y;

            // Early clip test
            if (lp0X < mClipRegion.x0 && lp1X < mClipRegion.x0)
            {
                return;
            }
            else if (lp0X > mClipRegion.x1 && lp1X > mClipRegion.x1)
            {
                return;
            }
            else if (lp0Y < mClipRegion.y0 && lp1Y < mClipRegion.y0)
            {
                return;
            }
            else if (lp0Y > mClipRegion.y1 && lp1Y > mClipRegion.y1)
            {
                return;
            }

            int dirX = lp1X - lp0X;
            int dirY = lp1Y - lp0Y;
            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            /* Figure out which quadrant the angle is in
             *
             * \      0     /
             *   \        /
             *     \    /
             *       \/
             * 3     /\     1
             *     /    \
             *   /        \
             * /      2     \
             */
            if (RBUtil.FastIntAbs(dirX) > RBUtil.FastIntAbs(dirY))
            {
                if (dirX > 0)
                {
                    // quadrant 1
                    p0X = lp0X + 0.5f;
                    p0Y = lp0Y;
                    p1X = lp1X + 0.5f;
                    p1Y = lp1Y;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    if (startPixel)
                    {
                        p0X += sideDirX * -0.5f;
                        p0Y += sideDirY * -0.5f;
                        p3X += sideDirX * -0.5f;
                        p3Y += sideDirY * -0.5f;
                    }
                    else
                    {
                        p0X += sideDirX * 0.5f;
                        p0Y += sideDirY * 0.5f;
                        p3X += sideDirX * 0.5f;
                        p3Y += sideDirY * 0.5f;
                    }

                    if (endPixel)
                    {
                        p1X += sideDirX * 0.5f;
                        p1Y += sideDirY * 0.5f;
                        p2X += sideDirX * 0.5f;
                        p2Y += sideDirY * 0.5f;
                    }
                    else
                    {
                        p1X += sideDirX * -0.5f;
                        p1Y += sideDirY * -0.5f;
                        p2X += sideDirX * -0.5f;
                        p2Y += sideDirY * -0.5f;
                    }
                }
                else
                {
                    // quadrant 3
                    p0X = lp1X + 0.5f;
                    p0Y = lp1Y;
                    p1X = lp0X + 0.5f;
                    p1Y = lp0Y;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    if (startPixel)
                    {
                        p1X += sideDirX * 0.5f;
                        p1Y += sideDirY * 0.5f;
                        p2X += sideDirX * 0.5f;
                        p2Y += sideDirY * 0.5f;
                    }
                    else
                    {
                        p1X += sideDirX * -0.5f;
                        p1Y += sideDirY * -0.5f;
                        p2X += sideDirX * -0.5f;
                        p2Y += sideDirY * -0.5f;
                    }

                    if (endPixel)
                    {
                        p0X += sideDirX * -0.5f;
                        p0Y += sideDirY * -0.5f;
                        p3X += sideDirX * -0.5f;
                        p3Y += sideDirY * -0.5f;
                    }
                    else
                    {
                        p0X += sideDirX * 0.5f;
                        p0Y += sideDirY * 0.5f;
                        p3X += sideDirX * 0.5f;
                        p3Y += sideDirY * 0.5f;
                    }
                }
            }
            else
            {
                if (dirY < 0)
                {
                    // quadrant 0
                    p0X = lp0X;
                    p0Y = lp0Y + 0.5f;
                    p1X = lp1X;
                    p1Y = lp1Y + 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    if (startPixel)
                    {
                        p0X += sideDirX * -0.5f;
                        p0Y += sideDirY * -0.5f;
                        p3X += sideDirX * -0.5f;
                        p3Y += sideDirY * -0.5f;
                    }
                    else
                    {
                        p0X += sideDirX * 0.5f;
                        p0Y += sideDirY * 0.5f;
                        p3X += sideDirX * 0.5f;
                        p3Y += sideDirY * 0.5f;
                    }

                    if (endPixel)
                    {
                        p1X += sideDirX * 0.5f;
                        p1Y += sideDirY * 0.5f;
                        p2X += sideDirX * 0.5f;
                        p2Y += sideDirY * 0.5f;
                    }
                    else
                    {
                        p1X += sideDirX * -0.5f;
                        p1Y += sideDirY * -0.5f;
                        p2X += sideDirX * -0.5f;
                        p2Y += sideDirY * -0.5f;
                    }
                }
                else
                {
                    // quadrant 2
                    p0X = lp1X;
                    p0Y = lp1Y + 0.5f;
                    p1X = lp0X;
                    p1Y = lp0Y + 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    if (startPixel)
                    {
                        p1X += sideDirX * 0.5f;
                        p1Y += sideDirY * 0.5f;
                        p2X += sideDirX * 0.5f;
                        p2Y += sideDirY * 0.5f;
                    }
                    else
                    {
                        p1X += sideDirX * -0.5f;
                        p1Y += sideDirY * -0.5f;
                        p2X += sideDirX * -0.5f;
                        p2Y += sideDirY * -0.5f;
                    }

                    if (endPixel)
                    {
                        p0X += sideDirX * -0.5f;
                        p0Y += sideDirY * -0.5f;
                        p3X += sideDirX * -0.5f;
                        p3Y += sideDirY * -0.5f;
                    }
                    else
                    {
                        p0X += sideDirX * 0.5f;
                        p0Y += sideDirY * 0.5f;
                        p3X += sideDirX * 0.5f;
                        p3Y += sideDirY * 0.5f;
                    }
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = color.r;
            v.color_g = color.g;
            v.color_b = color.b;
            v.color_a = color.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="srcX">X coordinate of sprite</param>
        /// <param name="srcY">Y coordinate of sprite</param>
        /// <param name="srcWidth">Width of sprite</param>
        /// <param name="srcHeight">Height of sprite</param>
        /// <param name="offset">Texture offset</param>
        /// <param name="repeat">How many times the texture repeats</param>
        public void DrawLineTextured(int lp0X, int lp0Y, int lp1X, int lp1Y, int srcX, int srcY, int srcWidth, int srcHeight, float offset, float repeat)
        {
            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            float lp0fX = lp0X;
            float lp0fY = lp0Y;
            float lp1fX = lp1X;
            float lp1fY = lp1Y;

            lp0fX -= mCameraPos.x;
            lp0fY -= mCameraPos.y;

            lp1fX -= mCameraPos.x;
            lp1fY -= mCameraPos.y;

            // Early clip test
            if (lp0fX < mClipRegion.x0 && lp1fX < mClipRegion.x0)
            {
                return;
            }
            else if (lp0fX > mClipRegion.x1 && lp1fX > mClipRegion.x1)
            {
                return;
            }
            else if (lp0fY < mClipRegion.y0 && lp1fY < mClipRegion.y0)
            {
                return;
            }
            else if (lp0fY > mClipRegion.y1 && lp1fY > mClipRegion.y1)
            {
                return;
            }

            float dirX = lp1fX - lp0fX;
            float dirY = lp1fY - lp0fY;
            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            float p0U, p0V;
            float p1U, p1V;
            float p2U, p2V;
            float p3U, p3V;

            float adjust = 0.001f;

            /* Figure out which quadrant the angle is in
             *
             * \      0     /
             *   \        /
             *     \    /
             *       \/
             * 3     /\     1
             *     /    \
             *   /        \
             * /      2     \
             */
            if (System.Math.Abs(dirX) > System.Math.Abs(dirY))
            {
                if (dirX > 0)
                {
                    // quadrant 1
                    p0X = lp0fX;
                    p0Y = lp0fY;
                    p1X = lp1fX;
                    p1Y = lp1fY;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0U = 0 + adjust + offset;
                    p0V = 0 + adjust;

                    p1U = repeat - adjust + offset;
                    p1V = 0 + adjust;

                    p2U = repeat - adjust + offset;
                    p2V = 1 - adjust;

                    p3U = 0 + adjust + offset;
                    p3V = 1 - adjust;
                }
                else
                {
                    // quadrant 3
                    p1X = lp0fX;
                    p1Y = lp0fY;
                    p0X = lp1fX;
                    p0Y = lp1fY;

                    p0X += 0.5f;
                    p1X += 0.5f;
                    p2X = p1X;
                    p2Y = p1Y + 1.0f;
                    p3X = p0X;
                    p3Y = p0Y + 1.0f;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p0U = repeat - adjust + offset;
                    p0V = 0 + adjust;

                    p1U = 0 + adjust + offset;
                    p1V = 0 + adjust;

                    p2U = 0 + adjust + offset;
                    p2V = 1 - adjust;

                    p3U = repeat - adjust + offset;
                    p3V = 1 - adjust;
                }
            }
            else
            {
                if (dirY < 0)
                {
                    // quadrant 0
                    p0X = lp0fX;
                    p0Y = lp0fY;
                    p1X = lp1fX;
                    p1Y = lp1fY;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0U = 0 + adjust + offset;
                    p0V = 0 + adjust;

                    p1U = repeat - adjust + offset;
                    p1V = 0 + adjust;

                    p2U = repeat - adjust + offset;
                    p2V = 1 - adjust;

                    p3U = 0 + adjust + offset;
                    p3V = 1 - adjust;
                }
                else
                {
                    // quadrant 2
                    p1X = lp0fX;
                    p1Y = lp0fY;
                    p0X = lp1fX;
                    p0Y = lp1fY;

                    p0Y += 0.5f;
                    p1Y += 0.5f;
                    p2X = p1X + 1.0f;
                    p2Y = p1Y;
                    p3X = p0X + 1.0f;
                    p3Y = p0Y;

                    var sideDirX = p1X - p0X;
                    var sideDirY = p1Y - p0Y;
                    float sideLen = Mathf.Sqrt((sideDirX * sideDirX) + (sideDirY * sideDirY));
                    sideDirX /= sideLen;
                    sideDirY /= sideLen;

                    p1X += sideDirX * 0.5f;
                    p1Y += sideDirY * 0.5f;
                    p2X += sideDirX * 0.5f;
                    p2Y += sideDirY * 0.5f;

                    p0X += sideDirX * -0.5f;
                    p0Y += sideDirY * -0.5f;
                    p3X += sideDirX * -0.5f;
                    p3Y += sideDirY * -0.5f;

                    p0U = repeat - adjust + offset;
                    p0V = 0 + adjust;

                    p1U = 0 + adjust + offset;
                    p1V = 0 + adjust;

                    p2U = 0 + adjust + offset;
                    p2V = 1 - adjust;

                    p3U = repeat - adjust + offset;
                    p3V = 1 - adjust;
                }
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            int sx0 = srcX;
            int sy0 = srcY;
            int sx1 = sx0 + srcWidth;
            int sy1 = sy0 + srcHeight;

            float tex_u0 = sx0 * CurrentSpriteSheetTextureWidthInverse;
            float tex_v0 = 1.0f - (sy0 * CurrentSpriteSheetTextureHeightInverse);
            float tex_u1 = sx1 * CurrentSpriteSheetTextureWidthInverse;
            float tex_v1 = 1.0f - (sy1 * CurrentSpriteSheetTextureHeightInverse);

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = mCurrentColor.r;
            v.color_g = mCurrentColor.g;
            v.color_b = mCurrentColor.b;
            v.color_a = mCurrentColor.a;
            v.pos_z = 1;

            v.tex_u0 = tex_u0;
            v.tex_v0 = tex_v0;
            v.tex_u1 = tex_u1;
            v.tex_v1 = tex_v1;

            v.u = p0U;
            v.v = p0V;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p1U;
            v.v = p1V;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p2U;
            v.v = p2V;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p3U;
            v.v = p3V;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw line from lp0 to lp1
        /// </summary>
        /// <param name="lp0X">Start point x</param>
        /// <param name="lp0Y">Start point y</param>
        /// <param name="lp1X">End point x</param>
        /// <param name="lp1Y">End point y</param>
        /// <param name="thickness_i">Thickness of the line</param>
        /// <param name="srcX">X coordinate of sprite</param>
        /// <param name="srcY">Y coordinate of sprite</param>
        /// <param name="srcWidth">Width of sprite</param>
        /// <param name="srcHeight">Height of sprite</param>
        /// <param name="offset">Texture offset</param>
        /// <param name="repeat">How many times the texture repeats</param>
        public void DrawLineTextured(int lp0X, int lp0Y, int lp1X, int lp1Y, int thickness_i, int srcX, int srcY, int srcWidth, int srcHeight, float offset, float repeat)
        {
            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            int clip_half_thickness = (int)((thickness_i + 0.5f) / 2);

            float lp0fX = lp0X;
            float lp0fY = lp0Y;
            float lp1fX = lp1X;
            float lp1fY = lp1Y;

            lp0fX -= mCameraPos.x;
            lp0fY -= mCameraPos.y;

            lp1fX -= mCameraPos.x;
            lp1fY -= mCameraPos.y;

            // Early clip test
            if (lp0fX + clip_half_thickness < mClipRegion.x0 && lp1fX + clip_half_thickness < mClipRegion.x0)
            {
                return;
            }
            else if (lp0fX - clip_half_thickness > mClipRegion.x1 && lp1fX - clip_half_thickness > mClipRegion.x1)
            {
                return;
            }
            else if (lp0fY + clip_half_thickness < mClipRegion.y0 && lp1fY + clip_half_thickness < mClipRegion.y0)
            {
                return;
            }
            else if (lp0fY - clip_half_thickness > mClipRegion.y1 && lp1fY - clip_half_thickness > mClipRegion.y1)
            {
                return;
            }

            float thicknessTop;
            float thicknessBottom;

            // For thickness that is not even we have to make one side of the line 1 pixel thicker
            if (thickness_i % 2 == 0)
            {
                thicknessTop = thickness_i / 2;
                thicknessBottom = thicknessTop;
            }
            else
            {
                thicknessTop = thickness_i / 2;
                thicknessBottom = thicknessTop + 1;
            }

            // Calculate line normal
            float dX = lp1fX - lp0fX;
            float dY = lp1fY - lp0fY;
            float tX = dY;
            float tY = -dX;

            float len = Mathf.Sqrt((tX * tX) + (tY * tY));
            tX /= len;
            tY /= len;

            float p0X, p0Y;
            float p1X, p1Y;
            float p2X, p2Y;
            float p3X, p3Y;

            float p0U, p0V;
            float p1U, p1V;
            float p2U, p2V;
            float p3U, p3V;

            float adjust = 0.0001f;

            p0U = 0 + offset + adjust;
            p0V = 1 - adjust;

            p1U = 0 + offset + adjust;
            p1V = 0 + adjust;

            p2U = repeat + offset - adjust;
            p2V = 0 + adjust;

            p3U = repeat + offset - adjust;
            p3V = 1 - adjust;

            p0X = lp0fX - (tX * thicknessTop);
            p0Y = lp0fY - (tY * thicknessTop);

            p3X = lp1fX - (tX * thicknessTop);
            p3Y = lp1fY - (tY * thicknessTop);

            p1X = lp0fX + (tX * thicknessBottom);
            p1Y = lp0fY + (tY * thicknessBottom);

            p2X = lp1fX + (tX * thicknessBottom);
            p2Y = lp1fY + (tY * thicknessBottom);

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            int sx0 = srcX;
            int sy0 = srcY;
            int sx1 = sx0 + srcWidth;
            int sy1 = sy0 + srcHeight;

            float adjustf = 0.00001f;

            float tex_u0 = sx0 * CurrentSpriteSheetTextureWidthInverse;
            float tex_v0 = 1.0f - (sy0 * CurrentSpriteSheetTextureHeightInverse);
            float tex_u1 = sx1 * CurrentSpriteSheetTextureWidthInverse;
            float tex_v1 = 1.0f - (sy1 * CurrentSpriteSheetTextureHeightInverse);

            if ((int)(tex_u0 + adjustf) <= (int)ushort.MaxValue)
            {
                tex_u0 += adjustf;
            }

            if ((int)(tex_v1 - adjustf) >= 0)
            {
                tex_v1 += adjustf;
            }

            if ((int)(tex_v0 + adjustf) <= (int)ushort.MaxValue)
            {
                tex_v0 -= adjustf;
            }

            if ((int)(tex_u1 - adjustf) >= 0)
            {
                tex_u1 -= adjustf;
            }

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = mCurrentColor.r;
            v.color_g = mCurrentColor.g;
            v.color_b = mCurrentColor.b;
            v.color_a = mCurrentColor.a;
            v.pos_z = 1;

            v.tex_u0 = tex_u0;
            v.tex_v0 = tex_v0;
            v.tex_u1 = tex_u1;
            v.tex_v1 = tex_v1;

            v.u = p0U;
            v.v = p0V;

            v.pos_x = p0X;
            v.pos_y = p0Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p1U;
            v.v = p1V;

            v.pos_x = p1X;
            v.pos_y = p1Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p2U;
            v.v = p2V;

            v.pos_x = p2X;
            v.pos_y = p2Y;

            mMeshStorage.Verticies[i++] = v;

            v.u = p3U;
            v.v = p3V;

            v.pos_x = p3X;
            v.pos_y = p3Y;

            mMeshStorage.Verticies[i++] = v;

            mMeshStorage.Indices[j++] = (ushort)(i - 4);
            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);

            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);
            mMeshStorage.Indices[j++] = (ushort)(i - 4);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        /// <summary>
        /// Draw a straight orthogonal line
        /// </summary>
        /// <param name="p0X">Start point x</param>
        /// <param name="p0Y">Start point y</param>
        /// <param name="p1X">End point x</param>
        /// <param name="p1Y">End point y</param>
        /// <param name="color">RGB color</param>
        private void DrawOrthoLine(int p0X, int p0Y, int p1X, int p1Y, Color32 color)
        {
            // Don't need to check for CheckFlush here, all callers of this API should check instead

            // Make sure p0 is before p1
            if (p0X > p1X || p0Y > p1Y)
            {
                int tpX = p0X;
                int tpY = p0Y;
                p0X = p1X;
                p0Y = p1Y;
                p1X = tpX;
                p1Y = tpY;
            }

            p0X -= mCameraPos.x;
            p0Y -= mCameraPos.y;
            p1X -= mCameraPos.x;
            p1Y -= mCameraPos.y;

            if ((p0X < mClipRegion.x0 && p1X < mClipRegion.x0) || (p0X > mClipRegion.x1 && p1X > mClipRegion.x1))
            {
                return;
            }

            if ((p0Y < mClipRegion.y0 && p1Y < mClipRegion.y0) || (p0Y > mClipRegion.y1 && p1Y > mClipRegion.y1))
            {
                return;
            }

            int i = mMeshStorage.CurrentVertex;
            int j = mMeshStorage.CurrentIndex;

            // Fast color multiply
            color.r = (byte)((color.r * mCurrentColor.r) / 255);
            color.g = (byte)((color.g * mCurrentColor.g) / 255);
            color.b = (byte)((color.b * mCurrentColor.b) / 255);
            color.a = (byte)((color.a * mCurrentColor.a) / 255);

            Vertex v;

            // Only have to set color once, will reuse for all vertices
            v.color_r = color.r;
            v.color_g = color.g;
            v.color_b = color.b;
            v.color_a = color.a;
            v.pos_z = 0;

            v.tex_u0 = 0;
            v.tex_v0 = 0;
            v.tex_u1 = 1;
            v.tex_v1 = 1;

            v.u = 0;
            v.v = 0;

            // Draw the line just one triangle, make sure it passes through the middle of the pixel
            // Horizontal
            if (p0Y == p1Y)
            {
                v.pos_x = p0X - 0.1f;
                v.pos_y = p0Y - 0.1f;

                mMeshStorage.Verticies[i++] = v;

                v.pos_x = p1X + 1.1f;
                v.pos_y = p1Y + 0.5f;

                mMeshStorage.Verticies[i++] = v;

                v.pos_x = p0X - 0.1f;
                v.pos_y = p0Y + 1.1f;

                mMeshStorage.Verticies[i++] = v;
            }
            else
            {
                v.pos_x = p0X - 0.1f;
                v.pos_y = p0Y - 0.1f;

                mMeshStorage.Verticies[i++] = v;

                v.pos_x = p0X + 1.1f;
                v.pos_y = p0Y - 0.1f;

                mMeshStorage.Verticies[i++] = v;

                v.pos_x = p1X + 0.5f;
                v.pos_y = p1Y + 1.1f;

                mMeshStorage.Verticies[i++] = v;
            }

            mMeshStorage.Indices[j++] = (ushort)(i - 3);
            mMeshStorage.Indices[j++] = (ushort)(i - 2);
            mMeshStorage.Indices[j++] = (ushort)(i - 1);

            mMeshStorage.CurrentVertex = i;
            mMeshStorage.CurrentIndex = j;
        }

        private void DrawLineStrip(Vector2i[] points, int pointCount, Color32 color)
        {
            if (pointCount < 2 || pointCount > points.Length)
            {
                return;
            }

            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6 * (pointCount + 1))
            {
                Flush(FlushReason.BATCH_FULL);
            }

            int i = 0;
            for (; i < pointCount - 2; i++)
            {
                if (points[i] != points[i + 1])
                {
                    DrawLineWithCaps(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, color, true, false);
                }
            }

            // Check flush
            if (MAX_VERTEX_PER_MESH - mMeshStorage.CurrentVertex < 6)
            {
                Flush(FlushReason.BATCH_FULL);
            }

            i++;
            if (points[i].x != points[0].x || points[i].y != points[0].y)
            {
                DrawLineWithCaps(points[i - 1].x, points[i - 1].y, points[i].x, points[i].y, color, true, true);
            }
            else
            {
                DrawLineWithCaps(points[i - 1].x, points[i - 1].y, points[i].x, points[i].y, color, true, false);
            }
        }
    }
}
