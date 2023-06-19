namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Stores a single result of an action that was performed. This is a generic class
    /// that can store all kinds of result types and their corresponding data. This is
    /// preferred to subclassing this class so that it a single instance can be continuously
    /// reused for all result types without causing GC allocations
    /// </summary>
    public struct Result
    {
        /// <summary>
        /// Result type
        /// </summary>
        public Type type;

        /// <summary>
        /// Result integer value
        /// </summary>
        public int int1;

        /// <summary>
        /// Result vector value
        /// </summary>
        public Vector2i veci1;

        /// <summary>
        /// Result fast string
        /// </summary>
        public FastString str1;

        /// <summary>
        /// Result entity value one
        /// </summary>
        public EntityID entity1;

        /// <summary>
        /// Result entity value two
        /// </summary>
        public EntityID entity2;

        /// <summary>
        /// Result type
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Exit result
            /// </summary>
            Exit,

            /// <summary>
            /// Move result
            /// </summary>
            Move,

            /// <summary>
            /// Message result
            /// </summary>
            Message,

            /// <summary>
            /// Death result
            /// </summary>
            Dead,

            /// <summary>
            /// Item pickup result
            /// </summary>
            Pickup,

            /// <summary>
            /// Item added to inventory result
            /// </summary>
            ItemAdded,

            /// <summary>
            /// Show inventory menu result
            /// </summary>
            ShowInventory,

            /// <summary>
            /// Inventory item selected at index result
            /// </summary>
            InventoryIndex,

            /// <summary>
            /// Inventory item consumed result
            /// </summary>
            Consumed,

            /// <summary>
            /// Drop item from inventory result
            /// </summary>
            DropInventory,

            /// <summary>
            /// Item dropped result
            /// </summary>
            ItemDropped,

            /// <summary>
            /// Left click result
            /// </summary>
            LeftClick,

            /// <summary>
            /// Right click result
            /// </summary>
            RightClick,

            /// <summary>
            /// Targeting result
            /// </summary>
            Targeting,

            /// <summary>
            /// Targeting cancelled result
            /// </summary>
            TargetingCancelled,

            /// <summary>
            /// Show help result
            /// </summary>
            ShowHelp,

            /// <summary>
            /// Menu index selected result
            /// </summary>
            MenuIndex,

            /// <summary>
            /// Take the stairs result
            /// </summary>
            TakeStairs,

            /// <summary>
            /// Experience earned result
            /// </summary>
            Xp,

            /// <summary>
            /// Level up result
            /// </summary>
            LevelUp,

            /// <summary>
            /// Show character menu result
            /// </summary>
            ShowCharacterScreen,

            /// <summary>
            /// Item equipped result
            /// </summary>
            Equipped,

            /// <summary>
            /// Item de-equipped result
            /// </summary>
            Dequipped,

            /// <summary>
            /// Equip item result
            /// </summary>
            Equip,

            /// <summary>
            /// Boss defeated result
            /// </summary>
            BossDefeated,

            /// <summary>
            /// Game won result
            /// </summary>
            GameWon,

            /// <summary>
            /// Fell down well result
            /// </summary>
            FellDownWell,

            /// <summary>
            /// Boss encountered result
            /// </summary>
            BossEncounter,

            /// <summary>
            /// Cheat code entered
            /// </summary>
            Cheat
        }
    }

    /// <summary>
    /// Re-useable result set that won't cause any heap allocations/garbage collection
    /// </summary>
    public class ResultSet
    {
        /// <summary>
        /// Array of results
        /// </summary>
        public Result[] mResults = new Result[MAX_RESULTS];

        private const int MAX_RESULTS = 64;
        private int mCount = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResultSet()
        {
            for (int i = 0; i < mResults.Length; i++)
            {
                mResults[i].str1 = new FastString(256);
            }
        }

        /// <summary>
        /// Count of results in the set
        /// </summary>
        public int Count
        {
            get
            {
                return mCount;
            }
        }

        /// <summary>
        /// Indexer of results
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Result</returns>
        public Result this[int index]
        {
            get
            {
                return mResults[index];
            }
        }

        /// <summary>
        /// Clear the result set
        /// </summary>
        public void Clear()
        {
            mCount = 0;
        }

        /// <summary>
        /// Add a move result
        /// </summary>
        /// <param name="delta">Move delta vector</param>
        public void AddMove(Vector2i delta)
        {
            var i = NewResult(Result.Type.Move);
            mResults[i].veci1 = delta;
        }

        /// <summary>
        /// Add an exit game result
        /// </summary>
        public void AddExit()
        {
            NewResult(Result.Type.Exit);
        }

        /// <summary>
        /// Add an entity death result
        /// </summary>
        /// <param name="entity">Entity</param>
        public void AddDead(EntityID entity)
        {
            var i = NewResult(Result.Type.Dead);
            mResults[i].entity1 = entity;
        }

        /// <summary>
        /// Add a console log message result
        /// </summary>
        /// <param name="msg">Message</param>
        public void AddMessage(FastString msg)
        {
            var i = NewResult(Result.Type.Message);
            mResults[i].str1.Set(msg);
        }

        /// <summary>
        /// Add a pickup item result
        /// </summary>
        /// <param name="receiver">Item receiver</param>
        public void AddPickup(EntityID receiver)
        {
            var i = NewResult(Result.Type.Pickup);
            mResults[i].entity1 = receiver;
        }

        /// <summary>
        /// Add an item added result
        /// </summary>
        /// <param name="item">Item</param>
        public void AddItemAdded(EntityID item)
        {
            var i = NewResult(Result.Type.ItemAdded);
            mResults[i].entity1 = item;
        }

        /// <summary>
        /// Add a show inventory result
        /// </summary>
        public void AddShowInventory()
        {
            NewResult(Result.Type.ShowInventory);
        }

        /// <summary>
        /// Add an inventory index result
        /// </summary>
        /// <param name="owner">Owner of inventory</param>
        /// <param name="index">Index</param>
        public void AddInventoryIndex(EntityID owner, int index)
        {
            var i = NewResult(Result.Type.InventoryIndex);
            mResults[i].int1 = index;
            mResults[i].entity1 = owner;
        }

        /// <summary>
        /// Add a consumed item result
        /// </summary>
        /// <param name="user">Item user</param>
        /// <param name="item">Item</param>
        public void AddConsumed(EntityID user, EntityID item)
        {
            var i = NewResult(Result.Type.Consumed);
            mResults[i].entity1 = user;
            mResults[i].entity2 = item;
        }

        /// <summary>
        /// Add a drop inventory menu result
        /// </summary>
        public void AddDropInventory()
        {
            NewResult(Result.Type.DropInventory);
        }

        /// <summary>
        /// Add an item dropped event
        /// </summary>
        /// <param name="item">Item</param>
        public void AddItemDropped(EntityID item)
        {
            var i = NewResult(Result.Type.ItemDropped);
            mResults[i].entity1 = item;
        }

        /// <summary>
        /// Add a left click result
        /// </summary>
        /// <param name="pos">Position of click</param>
        public void AddLeftClick(Vector2i pos)
        {
            var i = NewResult(Result.Type.LeftClick);
            mResults[i].veci1 = pos;
        }

        /// <summary>
        /// Add a right click result
        /// </summary>
        /// <param name="pos">Position of click</param>
        public void AddRightClick(Vector2i pos)
        {
            var i = NewResult(Result.Type.RightClick);
            mResults[i].veci1 = pos;
        }

        /// <summary>
        /// Add a targeting result
        /// </summary>
        /// <param name="item">Targeted item</param>
        public void AddTargeting(EntityID item)
        {
            var i = NewResult(Result.Type.Targeting);
            mResults[i].entity1 = item;
        }

        /// <summary>
        /// Add a targeting cancelled result
        /// </summary>
        public void AddTargetingCancelled()
        {
            NewResult(Result.Type.TargetingCancelled);
        }

        /// <summary>
        /// Add a show help menu result
        /// </summary>
        public void AddShowHelp()
        {
            NewResult(Result.Type.ShowHelp);
        }

        /// <summary>
        /// Add a menu index selected result
        /// </summary>
        /// <param name="index">Index</param>
        public void AddMenuIndex(int index)
        {
            var i = NewResult(Result.Type.MenuIndex);
            mResults[i].int1 = index;
        }

        /// <summary>
        /// Add a take stairs result
        /// </summary>
        public void AddTakeStairs()
        {
            NewResult(Result.Type.TakeStairs);
        }

        /// <summary>
        /// Add an experienced earned result
        /// </summary>
        /// <param name="amount">Amount of experience</param>
        public void AddXp(int amount)
        {
            var i = NewResult(Result.Type.Xp);
            mResults[i].int1 = amount;
        }

        /// <summary>
        /// Add a level up result with a select attribute to boost
        /// </summary>
        /// <param name="type">Level up boosted attribute</param>
        public void AddLevelUp(LevelUp type)
        {
            var i = NewResult(Result.Type.LevelUp);
            mResults[i].int1 = (int)type;
        }

        /// <summary>
        /// Add show character screen event
        /// </summary>
        public void AddShowCharacterScreen()
        {
            NewResult(Result.Type.ShowCharacterScreen);
        }

        /// <summary>
        /// Add item equipped event
        /// </summary>
        /// <param name="equipper">Equipper</param>
        /// <param name="item">Item</param>
        public void AddEquipped(EntityID equipper, EntityID item)
        {
            var i = NewResult(Result.Type.Equipped);
            mResults[i].entity1 = equipper;
            mResults[i].entity2 = item;
        }

        /// <summary>
        /// Add item unequipped event
        /// </summary>
        /// <param name="equipper">Equipper</param>
        /// <param name="item">Item</param>
        public void AddDequipped(EntityID equipper, EntityID item)
        {
            var i = NewResult(Result.Type.Dequipped);
            mResults[i].entity1 = equipper;
            mResults[i].entity2 = item;
        }

        /// <summary>
        /// Add equip event
        /// </summary>
        /// <param name="equipper">Equipper</param>
        /// <param name="item">Item</param>
        public void AddEquip(EntityID equipper, EntityID item)
        {
            var i = NewResult(Result.Type.Equip);
            mResults[i].entity1 = equipper;
            mResults[i].entity2 = item;
        }

        /// <summary>
        /// Add boss defeated event
        /// </summary>
        /// <param name="boss">Boss</param>
        public void AddBossDefeated(EntityID boss)
        {
            var i = NewResult(Result.Type.BossDefeated);
            mResults[i].entity1 = boss;
        }

        /// <summary>
        /// Add a cheat code result, used to enter test mode
        /// </summary>
        public void AddCheat()
        {
            NewResult(Result.Type.Cheat);
        }

        /// <summary>
        /// Game won!
        /// </summary>
        public void AddGameWon()
        {
            NewResult(Result.Type.GameWon);
        }

        /// <summary>
        /// Fell down the well
        /// </summary>
        public void AddFellDownWell()
        {
            NewResult(Result.Type.FellDownWell);
        }

        /// <summary>
        /// Encountered/Aggroed boss
        /// </summary>
        public void AddBossEncounter()
        {
            NewResult(Result.Type.BossEncounter);
        }

        /// <summary>
        /// Initialize event
        /// </summary>
        /// <param name="type">Event type</param>
        /// <returns>Index of the event</returns>
        private int NewResult(Result.Type type)
        {
            if (mCount >= mResults.Length)
            {
                throw new System.Exception("ResultSet is full!");
            }

            int i = mCount;

            mResults[i].type = type;
            mResults[i].int1 = 0;
            mResults[i].veci1 = Vector2i.zero;
            mResults[i].entity1 = EntityID.empty;
            mResults[i].entity2 = EntityID.empty;
            mResults[i].str1.Clear();

            mCount++;

            return i;
        }
    }
}
