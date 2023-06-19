namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Function of an item
    /// </summary>
    public enum ItemFunction
    {
        /// <summary>
        /// No function, item can't be used
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Heals
        /// </summary>
        HEAL,

        /// <summary>
        /// Casts lightning
        /// </summary>
        CAST_LIGHTNING,

        /// <summary>
        /// Casts fireball
        /// </summary>
        CAST_FIREBALL,

        /// <summary>
        /// Casts confuse
        /// </summary>
        CAST_CONFUSE,

        /// <summary>
        /// Casts confuse
        /// </summary>
        CAST_TELEPORT,
    }

    /// <summary>
    /// Handles all item functions
    /// </summary>
    public static class ItemFunctions
    {
        /// <summary>
        /// Perform the given item function
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="function">Item function to perform</param>
        /// <param name="user">User of the item</param>
        /// <param name="item">The item</param>
        /// <param name="useParams">Usage parameters</param>
        public static void DoItemFunction(ResultSet resultSet, ItemFunction function, EntityID user, EntityID item, Item.UseParams useParams)
        {
            switch (function)
            {
                case ItemFunction.HEAL:
                    if (SkillFunctions.Heal(resultSet, user, item.e.item.int1))
                    {
                        resultSet.AddConsumed(user, item);
                    }

                    break;

                case ItemFunction.CAST_LIGHTNING:
                    if (SkillFunctions.CastLightning(resultSet, user, useParams.veci1, item.e.item.int2, item.e.item.int1))
                    {
                        resultSet.AddConsumed(user, item);
                    }

                    break;

                case ItemFunction.CAST_FIREBALL:
                    if (SkillFunctions.CastFireball(resultSet, user, item.e.item.int2, item.e.item.int1, useParams.veci1, true))
                    {
                        resultSet.AddConsumed(user, item);
                    }

                    break;

                case ItemFunction.CAST_CONFUSE:
                    if (SkillFunctions.CastConfuse(resultSet, user, useParams.veci1))
                    {
                        resultSet.AddConsumed(user, item);
                    }

                    break;

                case ItemFunction.CAST_TELEPORT:
                    if (SkillFunctions.Teleport(resultSet, user, useParams.veci1))
                    {
                        resultSet.AddConsumed(user, item);
                    }

                    break;
            }
        }
    }
}
