namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An entity
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Sprite id
        /// </summary>
        public PackedSpriteID sprite;

        /// <summary>
        /// Color
        /// </summary>
        public Color32 color;

        /// <summary>
        /// Whether the entity is blocking
        /// </summary>
        public bool blocks;

        /// <summary>
        /// Name string
        /// </summary>
        public FastString name = new FastString(32);

        /// <summary>
        /// Render order
        /// </summary>
        public RenderFunctions.RenderOrder renderOrder;

        private Vector2i mPos = new Vector2i(-1, -1);

        private Fighter mFighter;
        private Ai mAi;
        private Inventory mInventory;
        private Item mItem;
        private Stairs mStairs;
        private Level mLevel;
        private Equippable mEquippable;
        private Equipment mEquipment;
        private GroundTrigger mGroundTrigger;
        private MoveTrail mMoveTrail;

        private EntityID mId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">id of the Entity</param>
        public Entity(EntityID id)
        {
            mId = id;
        }

        /// <summary>
        /// Get or Set entity position. When set the entity is updated on the map
        /// </summary>
        public Vector2i pos
        {
            get
            {
                return mPos;
            }

            set
            {
                Vector2i oldPos = mPos;
                mPos = value;
                GameMap.UpdateEntityMapPos(id, oldPos);

                // Generate trail
                if (moveTrail != null && oldPos.x != -1 && oldPos.y != -1)
                {
                    var game = (RetroDungeoneerGame)RB.Game;

                    Vector2 startPos = new Vector2(oldPos.x * game.assets.spriteSheet.grid.cellSize.width, oldPos.y * game.assets.spriteSheet.grid.cellSize.height);
                    Vector2 endPos = new Vector2(mPos.x * game.assets.spriteSheet.grid.cellSize.width, mPos.y * game.assets.spriteSheet.grid.cellSize.height);

                    if (game.map.IsInFOV(oldPos) || game.map.IsInFOV(mPos))
                    {
                        Vector2 delta = endPos - startPos;
                        Vector2 deltaDir = delta.normalized;

                        int len = (int)delta.magnitude;
                        int spacing = (int)(game.assets.spriteSheet.grid.cellSize.width / 2.5f);

                        float div = (len / (float)spacing) - 1;
                        if (div < 1.0f)
                        {
                            div = 1.0f;
                        }

                        int fadeInc = (int)(140 / div);

                        if (fadeInc <= 0)
                        {
                            fadeInc = 1;
                        }

                        int fade = fadeInc;

                        Vector2 pos = startPos;

                        while (len > 0)
                        {
                            moveTrail.Add(pos, fade);

                            fade += fadeInc;
                            pos += deltaDir * spacing;
                            len -= spacing;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get entity id
        /// </summary>
        public EntityID id
        {
            get
            {
                return mId;
            }
        }

        /// <summary>
        /// True if entity is player
        /// </summary>
        public bool isPlayer
        {
            get
            {
                var game = (RetroDungeoneerGame)RB.Game;
                return mId == game.player;
            }
        }

        #region Component Getters/Setters
        /// <summary>
        /// Fighter component
        /// </summary>
        public Fighter fighter
        {
            get
            {
                return mFighter;
            }

            set
            {
                mFighter = value;
                if (mFighter != null)
                {
                    mFighter.owner = mId;
                }
            }
        }

        /// <summary>
        /// AI component
        /// </summary>
        public Ai ai
        {
            get
            {
                return mAi;
            }

            set
            {
                mAi = value;
                if (mAi != null)
                {
                    mAi.owner = mId;
                }
            }
        }

        /// <summary>
        /// Inventory component
        /// </summary>
        public Inventory inventory
        {
            get
            {
                return mInventory;
            }

            set
            {
                mInventory = value;
                if (mInventory != null)
                {
                    mInventory.owner = mId;
                }
            }
        }

        /// <summary>
        /// Item component
        /// </summary>
        public Item item
        {
            get
            {
                return mItem;
            }

            set
            {
                mItem = value;
                if (mItem != null)
                {
                    mItem.owner = mId;
                }
            }
        }

        /// <summary>
        /// Stairs component
        /// </summary>
        public Stairs stairs
        {
            get
            {
                return mStairs;
            }

            set
            {
                mStairs = value;
                if (mStairs != null)
                {
                    mStairs.owner = mId;
                }
            }
        }

        /// <summary>
        /// Level component
        /// </summary>
        public Level level
        {
            get
            {
                return mLevel;
            }

            set
            {
                mLevel = value;
                if (mLevel != null)
                {
                    mLevel.owner = mId;
                }
            }
        }

        /// <summary>
        /// Equippable component
        /// </summary>
        public Equippable equippable
        {
            get
            {
                return mEquippable;
            }

            set
            {
                mEquippable = value;
                if (mEquippable != null)
                {
                    mEquippable.owner = mId;
                }
            }
        }

        /// <summary>
        /// Equipment component
        /// </summary>
        public Equipment equipment
        {
            get
            {
                return mEquipment;
            }

            set
            {
                mEquipment = value;
                if (mEquipment != null)
                {
                    mEquipment.owner = mId;
                }
            }
        }

        /// <summary>
        /// Ground Trigger
        /// </summary>
        public GroundTrigger groundTrigger
        {
            get
            {
                return mGroundTrigger;
            }

            set
            {
                mGroundTrigger = value;
            }
        }

        /// <summary>
        /// Move trail
        /// </summary>
        public MoveTrail moveTrail
        {
            get
            {
                return mMoveTrail;
            }

            set
            {
                mMoveTrail = value;
            }
        }
        #endregion

        /// <summary>
        /// Initialize the entity
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="sprite">Sprite to draw with</param>
        /// <param name="color">Color</param>
        /// <param name="name">String name</param>
        /// <param name="renderOrder">Rendering order</param>
        /// <param name="blocks">Whether the entity is blocking</param>
        public void Initialize(
            Vector2i pos,
            PackedSpriteID sprite,
            Color32 color,
            FastString name,
            RenderFunctions.RenderOrder renderOrder,
            bool blocks = false)
        {
            this.pos = pos;
            this.sprite = sprite;
            this.color = color;
            this.name.Append(name);
            this.blocks = blocks;
            this.renderOrder = renderOrder;
        }

        /// <summary>
        /// Move by the given delta vector
        /// </summary>
        /// <param name="delta">Delta movement</param>
        public void Move(Vector2i delta)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!(game.map.IsBlocked(pos + delta) || !EntityFunctions.GetBlockingEntityAtPos(pos + delta).isEmpty))
            {
                pos += delta;
            }
        }

        /// <summary>
        /// Move towards target position, by one tile
        /// </summary>
        /// <param name="targetPos">Target position</param>
        public void MoveTowards(Vector2i targetPos)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var delta = targetPos - pos;
            var distance = delta.Magnitude();

            // Normalize
            delta.x = Mathf.RoundToInt(delta.x / distance);
            delta.y = Mathf.RoundToInt(delta.y / distance);

            if (!(game.map.IsBlocked(pos + delta) || !EntityFunctions.GetBlockingEntityAtPos(pos + delta).isEmpty))
            {
                pos += delta;
            }
        }

        /// <summary>
        /// Get distance from other entity
        /// </summary>
        /// <param name="other">Other entity</param>
        /// <returns>Distance to other entity</returns>
        public float DistanceTo(EntityID other)
        {
            return DistanceTo(other.e.pos);
        }

        /// <summary>
        /// Get distance from given position
        /// </summary>
        /// <param name="pos">Position</param>
        /// <returns>Distance</returns>
        public float DistanceTo(Vector2i pos)
        {
            return (pos - this.pos).Magnitude();
        }

        /// <summary>
        /// Get list of entities containing this one and all its inventory children
        /// </summary>
        /// <returns>List of self and child entities</returns>
        public List<EntityID> GetSelfAndChildren()
        {
            List<EntityID> entities = new List<EntityID>();
            entities.Add(mId);

            if (inventory != null)
            {
                for (int i = 0; i < inventory.items.Length; i++)
                {
                    entities.Add(inventory.items[i]);
                }
            }

            return entities;
        }
    }
}
