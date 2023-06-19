namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Set of utilities for picking random choices
    /// </summary>
    public class RandomUtils
    {
        /// <summary>
        /// Return random choice from weighted chances
        /// </summary>
        /// <param name="chances">Weighted chances</param>
        /// <returns>Random choice</returns>
        public static int RandomChoiceIndex(List<int> chances)
        {
            int totalSum = 0;
            for (int i = 0; i < chances.Count; i++)
            {
                totalSum += chances[i];
            }

            int randomChance = Random.Range(0, totalSum);

            int runningSum = 0;
            int choice = -1;
            for (int i = 0; i < chances.Count; i++)
            {
                runningSum += chances[i];
                if (randomChance <= runningSum)
                {
                    choice = i;
                    break;
                }
            }

            return choice;
        }

        /// <summary>
        /// Generate a random selection based on dungeon level
        /// </summary>
        /// <param name="brackets">Selection brackets</param>
        /// <param name="dungeonLevel">Current dungeon level</param>
        /// <returns>Random selection</returns>
        public static int FromDungeonLevel(Tuple<int, int>[] brackets, int dungeonLevel)
        {
            for (int i = brackets.Length - 1; i >= 0; i--)
            {
                if (dungeonLevel >= brackets[i].item2)
                {
                    return brackets[i].item1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Return random pitch level within given variance
        /// </summary>
        /// <param name="variance">Variance</param>
        /// <returns>Random pitch</returns>
        public static float RandomPitch(float variance)
        {
            return Random.Range(1.0f - variance, 1.0f + variance);
        }

        /// <summary>
        /// Return a random position along the perimeter of the rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>Random position</returns>
        public static Vector2i RandomRectPerimeterPos(Rect2i rect)
        {
            int pLen = (rect.width * 2) + (rect.height * 2) - 4;
            int rand = Random.Range(0, pLen);

            if (rand < rect.width)
            {
                return new Vector2i(rect.x + rand, rect.y);
            }

            rand -= rect.width;
            if (rand < rect.width)
            {
                return new Vector2i(rect.x + rand, rect.y + rect.height - 1);
            }

            rand -= rect.width;
            if (rand < rect.height - 2)
            {
                return new Vector2i(rect.x, rect.y + rand + 1);
            }

            rand -= rect.height - 2;
            return new Vector2i(rect.x + rect.width - 1, rect.y + rand + 1);
        }

        /// <summary>
        /// A tuple of two objects
        /// </summary>
        /// <typeparam name="T1">Type one</typeparam>
        /// <typeparam name="T2">Type two</typeparam>
        public struct Tuple<T1, T2>
        {
            /// <summary>
            /// Item of type one
            /// </summary>
            public T1 item1;

            /// <summary>
            /// Item of type two
            /// </summary>
            public T2 item2;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="item1">First object</param>
            /// <param name="item2">Second object</param>
            public Tuple(T1 item1, T2 item2)
            {
                this.item1 = item1;
                this.item2 = item2;
            }
        }
    }
}
