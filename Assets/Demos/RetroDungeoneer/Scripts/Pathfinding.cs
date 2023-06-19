namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Represents a flood map originating from a given point. This can be used for path finding by
    /// following nodes in decreasing order to the origin (player's position in most cases)
    /// </summary>
    public class FloodMap
    {
        private const int CARDINAL_COST = 100;
        private const int DIAGONAL_COST = 141;
        private const int ENTITY_COST = 800;

        private int[] mCosts;
        private int[] mSearchNodes;
        private Vector2i mSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the flood map in tiles</param>
        public FloodMap(Vector2i size)
        {
            mCosts = new int[size.width * size.height];
            mSearchNodes = new int[size.width * size.height];
            mSize = size;
        }

        /// <summary>
        /// Refresh the floodmap, from the given origin
        /// </summary>
        /// <param name="origin">Origin position</param>
        /// <param name="maxDist">Maximum distance</param>
        public void Refresh(Vector2i origin, int maxDist)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            // Square the distance so we can use squared distance comparison
            // instead of having to use square root
            maxDist = maxDist * maxDist;

            // Reset all costs to max
            for (int i = 0; i < mCosts.Length; i++)
            {
                mCosts[i] = int.MaxValue;
            }

            // Set cost at origin to 0
            mCosts[origin.x + (origin.y * mSize.width)] = 0;
            int count = 0;

            mSearchNodes[count] = origin.x + (origin.y * mSize.width);
            count++;

            int firstNode = 0;
            int lastNode = 0;

            while (count > 0)
            {
                int i = mSearchNodes[firstNode];
                firstNode++;
                count--;

                int cost = mCosts[i];

                var pos = new Vector2i(i % mSize.width, i / mSize.width);

                // Expand flood into all cardinal directions.
                for (int d = 0; d < Direction.Cardinal.Length; d++)
                {
                    var dir = Direction.Cardinal[d];
                    int newCost = cost + CARDINAL_COST;

                    var dest = pos + dir;
                    var deltaFromOrigin = origin - dest;

                    if (!EntityFunctions.GetBlockingEntityAtPos(dest).isEmpty)
                    {
                        newCost += ENTITY_COST;
                    }

                    if (!game.map.IsBlocked(dest) && deltaFromOrigin.SqrMagnitude() <= maxDist)
                    {
                        int j = dest.x + (dest.y * mSize.width);
                        if (mCosts[j] > newCost)
                        {
                            mCosts[j] = newCost;
                            mSearchNodes[lastNode + 1] = j;
                            lastNode++;
                            count++;
                        }
                    }
                }

                // Expand flood into all diagonal directions.
                for (int d = 0; d < Direction.Diagonal.Length; d++)
                {
                    var dir = Direction.Diagonal[d];

                    int newCost = cost + DIAGONAL_COST;

                    var dest = pos + dir;
                    var deltaFromOrigin = origin - dest;

                    if (!EntityFunctions.GetBlockingEntityAtPos(dest).isEmpty)
                    {
                        newCost += ENTITY_COST;
                    }

                    if (!game.map.IsBlocked(dest) && deltaFromOrigin.SqrMagnitude() <= maxDist)
                    {
                        int j = dest.x + (dest.y * mSize.width);
                        if (mCosts[j] > newCost)
                        {
                            mCosts[j] = newCost;
                            mSearchNodes[lastNode + 1] = j;
                            lastNode++;
                            count++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the cheapest direction to move into from the origin towards the target of the floodmap
        /// </summary>
        /// <param name="origin">Origin position</param>
        /// <returns>Cheapest direction, or zero vector is none available</returns>
        public Vector2i GetCheapestDirection(Vector2i origin)
        {
            int cheapestCost = int.MaxValue;
            Vector2i cheapestDir = Vector2i.zero;

            for (int j = 0; j < Direction.All.Length; j++)
            {
                var dir = Direction.All[j];

                var dest = origin + dir;
                if (dest.x < 0 || dest.y < 0 || dest.x >= mSize.width || dest.y >= mSize.height)
                {
                    continue;
                }

                int i = dest.x + (dest.y * mSize.width);
                if (mCosts[i] < cheapestCost)
                {
                    cheapestDir = dir;
                    cheapestCost = mCosts[i];
                }
            }

            return cheapestDir;
        }

        /// <summary>
        /// Render floodmap for debugging purposes only
        /// </summary>
        public void DebugRender()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            int w = game.assets.spriteSheet.grid.cellSize.width;
            int h = game.assets.spriteSheet.grid.cellSize.height;
            int i = 0;

            for (int y = 0; y < mSize.height; y++)
            {
                for (int x = 0; x < mSize.width; x++)
                {
                    int cost = mCosts[i];

                    if (cost < int.MaxValue)
                    {
                        RB.Print(new Rect2i(x * w, y * h, game.assets.spriteSheet.grid.cellSize.width, game.assets.spriteSheet.grid.cellSize.height), Color.red, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, C.FSTR.Set(cost / 10));
                    }

                    i++;
                }
            }
        }
    }
}
