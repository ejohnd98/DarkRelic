namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Component type
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        /// Basic monster AI
        /// </summary>
        BasicMonster = 1,

        /// <summary>
        /// Confused monster AI
        /// </summary>
        ConfusedMonster,

        /// <summary>
        /// Golem boss monster AI
        /// </summary>
        GolemMonster,

        /// <summary>
        /// Slime monster AI
        /// </summary>
        SlimeMonster,

        /// <summary>
        /// Fighter
        /// </summary>
        Fighter,

        /// <summary>
        /// Inventory
        /// </summary>
        Inventory,

        /// <summary>
        /// Item
        /// </summary>
        Item,

        /// <summary>
        /// Level
        /// </summary>
        Level,

        /// <summary>
        /// Stairs
        /// </summary>
        Stairs,

        /// <summary>
        /// Equipment storage
        /// </summary>
        Equipment,

        /// <summary>
        /// Is equippable
        /// </summary>
        Equippable,

        /// <summary>
        /// Ground trigger
        /// </summary>
        GroundTrigger,

        /// <summary>
        /// Move trail
        /// </summary>
        MoveTrail
    }

    /// <summary>
    /// Option to pick when leveling up
    /// </summary>
    public enum LevelUp
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Hitpoints
        /// </summary>
        Hp,

        /// <summary>
        /// Strength
        /// </summary>
        Str,

        /// <summary>
        /// Defense
        /// </summary>
        Def
    }

    /// <summary>
    /// Equipment slot, currently just hands
    /// </summary>
    public enum EquipmentSlot
    {
        /// <summary>
        /// Armor slot
        /// </summary>
        Armor,

        /// <summary>
        /// Ranged weapon
        /// </summary>
        Ranged,

        /// <summary>
        /// Main hand slot
        /// </summary>
        Head,

        /// <summary>
        /// Main hand slot
        /// </summary>
        MainHand,

        /// <summary>
        /// Off hand slot
        /// </summary>
        OffHand,

        /// <summary>
        /// Total slots count
        /// </summary>
        Count
    }

    /// <summary>
    /// Monster type
    /// </summary>
    public enum MonsterType
    {
        /// <summary>
        /// Rat
        /// </summary>
        Rat = 0,

        /// <summary>
        /// Skeleton
        /// </summary>
        Skeleton,

        /// <summary>
        /// Archer Skeleton
        /// </summary>
        ArcherSkeleton,

        /// <summary>
        /// Bat
        /// </summary>
        Bat,

        /// <summary>
        /// Slime
        /// </summary>
        Slime,

        /// <summary>
        /// Slime small
        /// </summary>
        SlimeSmall,

        /// <summary>
        /// Ghost
        /// </summary>
        Ghost,

        /// <summary>
        /// Spider
        /// </summary>
        Spider,

        /// <summary>
        /// Golem
        /// </summary>
        Golem,

        /// <summary>
        /// Invincible thug, used in intro
        /// </summary>
        InvincibleThug,

        /// <summary>
        /// Invincible blockade, used in intro
        /// </summary>
        InvincibleBlockade,

        /// <summary>
        /// Thug
        /// </summary>
        Thug,

        /// <summary>
        /// Blockade (indestructible)
        /// </summary>
        Blockade,

        /// <summary>
        /// Total monster types count
        /// </summary>
        Count
    }

    /// <summary>
    /// Item type
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Healing potion
        /// </summary>
        HealingPotion = 0,

        /// <summary>
        /// Fireball scroll
        /// </summary>
        FireBallScroll,

        /// <summary>
        /// Confusion scroll
        /// </summary>
        ConfusionScroll,

        /// <summary>
        /// Lightning scroll
        /// </summary>
        LightningScroll,

        /// <summary>
        /// Teleport scroll
        /// </summary>
        TeleportScroll,

        /// <summary>
        /// Dagger scroll
        /// </summary>
        Dagger,

        /// <summary>
        /// Sword
        /// </summary>
        Sword,

        /// <summary>
        /// Short sword
        /// </summary>
        ShortSword,

        /// <summary>
        /// Serpentine sword
        /// </summary>
        SerpentineSword,

        /// <summary>
        /// Helmet
        /// </summary>
        Helmet,

        /// <summary>
        /// Shield scroll
        /// </summary>
        Shield,

        /// <summary>
        /// Large shield
        /// </summary>
        LargeShield,

        /// <summary>
        /// Leather armor
        /// </summary>
        LeatherArmor,

        /// <summary>
        /// Chain armor
        /// </summary>
        ChainArmor,

        /// <summary>
        /// Plate armor
        /// </summary>
        PlateArmor,

        /// <summary>
        /// Bow
        /// </summary>
        Bow,

        /// <summary>
        /// Fine bow
        /// </summary>
        BowFine,

        /// <summary>
        /// Elven bow
        /// </summary>
        BowElven,

        /// <summary>
        /// Arrow
        /// </summary>
        Arrow,

        /// <summary>
        /// Total item types count
        /// </summary>
        Count
    }

    /// <summary>
    /// Interactable type enum
    /// </summary>
    public enum InteractableType
    {
        /// <summary>
        /// Stairs
        /// </summary>
        Stairs,

        /// <summary>
        /// Portal
        /// </summary>
        Portal,

        /// <summary>
        /// Well
        /// </summary>
        Well,

        /// <summary>
        /// Game exit
        /// </summary>
        GameExit,

        /// <summary>
        /// Web
        /// </summary>
        Web,

        /// <summary>
        /// Total count
        /// </summary>
        Count
    }

    /// <summary>
    /// Effect type enum
    /// </summary>
    public enum EffectType
    {
        /// <summary>
        /// Aggro
        /// </summary>
        Aggro,

        /// <summary>
        /// Confuse
        /// </summary>
        Confuse,

        /// <summary>
        /// Flame
        /// </summary>
        Flame,

        /// <summary>
        /// Lightning
        /// </summary>
        Lightning,

        /// <summary>
        /// Player death
        /// </summary>
        PlayerDeath,

        /// <summary>
        /// Swoosh
        /// </summary>
        Swoosh,

        /// <summary>
        /// Player entrance
        /// </summary>
        PlayerEntrance,

        /// <summary>
        /// End portal effect
        /// </summary>
        Portal,

        /// <summary>
        /// Teleport effect
        /// </summary>
        Teleport,

        /// <summary>
        /// Thrown object effect
        /// </summary>
        Throw
    }

    /// <summary>
    /// Skill that an AI can use
    /// </summary>
    public enum Skill
    {
        /// <summary>
        /// Perform a melee attack
        /// </summary>
        MeleeAttack,

        /// <summary>
        /// Cast fireball at random location
        /// </summary>
        CastRandomFireball,

        /// <summary>
        /// Cast lightning at nearest target
        /// </summary>
        CastLightning,

        /// <summary>
        /// Cast web at target
        /// </summary>
        CastWeb,

        /// <summary>
        /// Shoot bow at target
        /// </summary>
        ShootBow,

        /// <summary>
        /// Teleport
        /// </summary>
        Teleport
    }
}
