namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Various functions that work on entities in the map
    /// </summary>
    public static class EntityFunctions
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
        }

        /// <summary>
        /// Get all entities at the given position
        /// </summary>
        /// <param name="pos">Position to check</param>
        /// <returns>List of entities at position</returns>
        public static List<EntityID> GetEntitiesAtPos(Vector2i pos)
        {
            return RB.MapDataGet<List<EntityID>>(C.LAYER_TERRAIN, pos);
        }

        /// <summary>
        /// Get any blocking entity at the given position
        /// </summary>
        /// <param name="pos">Position</param>
        /// <returns>Blocking entity</returns>
        public static EntityID GetBlockingEntityAtPos(Vector2i pos)
        {
            var entityList = GetEntitiesAtPos(pos);
            if (entityList == null)
            {
                return EntityID.empty;
            }

            for (int i = 0; i < entityList.Count; i++)
            {
                var e = entityList[i].e;

                if (e.blocks && e.pos == pos)
                {
                    return e.id;
                }
            }

            return EntityID.empty;
        }

        /// <summary>
        /// Get a string describing all entities at given position
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="outStr">Entity string</param>
        public static void GetEntitiesStringAtTile(Vector2i pos, FastString outStr)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            outStr.Clear();

            for (int i = RenderFunctions.renderOrderList.Length - 1; i >= 0; i--)
            {
                var currentRenderOrder = RenderFunctions.renderOrderList[i];
                if (currentRenderOrder == RenderFunctions.RenderOrder.HIDDEN)
                {
                    continue;
                }

                var entityList = GetEntitiesAtPos(pos);
                if (entityList == null)
                {
                    continue;
                }

                for (int j = 0; j < entityList.Count; j++)
                {
                    var e = entityList[j].e;

                    if (e != null && e.renderOrder == currentRenderOrder && game.map.IsInFOV(e.pos))
                    {
                        if (outStr.Length > 0)
                        {
                            outStr.Append('\n');
                        }

                        if (currentRenderOrder == RenderFunctions.RenderOrder.CORPSE)
                        {
                            outStr.Append(C.STR_COLOR_CORPSE).Append(e.name).Append("@-");
                        }
                        else if (currentRenderOrder == RenderFunctions.RenderOrder.ACTOR)
                        {
                            outStr.Append(C.STR_COLOR_NAME).Append(e.name).Append("@-");
                        }
                        else
                        {
                            outStr.Append(e.name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a monster of the given type
        /// </summary>
        /// <param name="monsterType">Type</param>
        /// <param name="pos">Position</param>
        /// <returns>Monster entity</returns>
        public static EntityID CreateMonster(MonsterType monsterType, Vector2i pos)
        {
            EntityID monster = EntityID.empty;

            switch (monsterType)
            {
                case MonsterType.Rat:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.RAT,
                            new Color32(0xc0, 0x79, 0x58, 255),
                            C.FSTR.Set("Rat"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(20, 0, 4, 35);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Skeleton:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.SKELETON,
                            new Color32(0xdb, 0xd3, 0xc3, 255),
                            C.FSTR.Set("Skeletal Warrior"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(30, 2, 10, 100);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.ArcherSkeleton:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.SKELETON,
                            new Color32(0xdb, 0xd3, 0xc3, 255),
                            C.FSTR.Set("Skeletal Archer"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(30, 2, 5, 100);
                        monster.e.ai = new BasicMonster();
                        monster.e.inventory = new Inventory(5);

                        for (int i = 0; i < 4; i++)
                        {
                            monster.e.inventory.AddItem(null, CreateItem(ItemType.Arrow, new Vector2i(-1, -1)));
                        }

                        var bow = CreateItem(ItemType.Bow, new Vector2i(-1, -1));
                         monster.e.inventory.AddItem(null, bow);

                        monster.e.equipment = new Equipment();
                        monster.e.equipment.equipment[(int)EquipmentSlot.Ranged] = bow;

                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 0); // Always prefer to shoot bow, unless no arrows
                        ai.AddSkill(Skill.ShootBow, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Bat:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.BAT,
                            new Color32(0x7a, 0x44, 0x4a, 255),
                            C.FSTR.Set("Bat"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(15, 1, 3, 40);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Slime:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.SLIME,
                            new Color32(0x38, 0xd9, 0x73, 255),
                            C.FSTR.Set("Slime"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(30, 2, 5, 60);
                        var ai = new SlimeMonster();
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.SlimeSmall:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.SLIME_SMALL,
                            new Color32(0x38, 0xd9, 0x73, 255),
                            C.FSTR.Set("Baby Slime"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(15, 0, 2, 30);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Ghost:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.GHOST,
                            new Color32(0xdb, 0xd3, 0xc3, 180),
                            C.FSTR.Set("Ghost"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(25, 3, 10, 80);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 75);
                        ai.AddSkill(Skill.Teleport, 25);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Spider:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.SPIDER,
                            new Color32(0x5b, 0x5b, 0x7b, 255),
                            C.FSTR.Set("Spider"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(20, 2, 7, 50);
                        var ai = new BasicMonster();
                        ai.AddSkill(Skill.MeleeAttack, 75);
                        ai.AddSkill(Skill.CastWeb, 25);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Golem:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.GOLEM,
                            new Color32(0xe6, 0x48, 0x2e, 255),
                            C.FSTR.Set("Gatekeeper"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(250, 5, 20, 250);
                        var ai = new GolemMonster();
                        ai.AddSkill(Skill.MeleeAttack, 60);
                        ai.AddSkill(Skill.CastRandomFireball, 20);
                        ai.AddSkill(Skill.CastLightning, 20);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.InvincibleThug:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.THUG,
                            Color.white,
                            C.FSTR.Set("Brigand"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(100, 2, 20, 0);
                        var ai = new BasicMonster();
                        ai.aggroRange = 40;
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.InvincibleBlockade:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.BRANCHES,
                            new Color32(0xbf, 0x79, 0x58, 255),
                            C.FSTR.Set("Blockade"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.fighter = new Fighter(9999, 9999, 1, 1);
                        monster.e.ai = null;
                    }

                    break;

                case MonsterType.Thug:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.THUG,
                            Color.white,
                            C.FSTR.Set("Brigand"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.moveTrail = new MoveTrail();
                        monster.e.fighter = new Fighter(40, 0, 6, 100);
                        var ai = new BasicMonster();
                        ai.aggroRange = 40;
                        ai.AddSkill(Skill.MeleeAttack, 100);
                        monster.e.ai = ai;
                    }

                    break;

                case MonsterType.Blockade:
                    {
                        monster = EntityStore.CreateEntity();
                        monster.e.Initialize(
                            pos,
                            S.BRANCHES,
                            new Color32(0xbf, 0x79, 0x58, 255),
                            C.FSTR.Set("Blockade"),
                            RenderFunctions.RenderOrder.ACTOR,
                            true);

                        monster.e.fighter = new Fighter(16, 0, 0, 0);
                        monster.e.ai = null;
                    }

                    break;
            }

            return monster;
        }

        /// <summary>
        /// Create item of the given type
        /// </summary>
        /// <param name="itemType">Type</param>
        /// <param name="pos">Position</param>
        /// <returns>Item entity</returns>
        public static EntityID CreateItem(ItemType itemType, Vector2i pos)
        {
            EntityID item = EntityID.empty;

            switch (itemType)
            {
                case ItemType.HealingPotion:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.POTION,
                        new Color32(255, 100, 100, 255),
                        C.FSTR.Set("Healing Potion"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.item.useFunction = ItemFunction.HEAL;
                    item.e.item.int1 = 40;
                    break;

                case ItemType.FireBallScroll:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SCROLL,
                        new Color32(220, 150, 150, 255),
                        C.FSTR.Set("Fireball Scroll"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.item.useFunction = ItemFunction.CAST_FIREBALL;
                    item.e.item.int1 = 15;
                    item.e.item.int2 = 3;
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click a target tile for the fireball, or right-click to cancel.");
                    break;

                case ItemType.ConfusionScroll:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SCROLL,
                        new Color32(220, 220, 150, 255),
                        C.FSTR.Set("Confusion Scroll"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.item.useFunction = ItemFunction.CAST_CONFUSE;
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click an enemy to confuse it, or right-click to cancel.");
                    break;

                case ItemType.LightningScroll:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SCROLL,
                        new Color32(150, 150, 220, 255),
                        C.FSTR.Set("Lightning Scroll"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.item.useFunction = ItemFunction.CAST_LIGHTNING;
                    item.e.item.int1 = 25;
                    item.e.item.int2 = 5;
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click an enemy to strike, or right-click to cancel.");
                    break;

                case ItemType.TeleportScroll:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SCROLL,
                        new Color32(0xc1, 0x7a, 0xd6, 255),
                        C.FSTR.Set("Teleport Scroll"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.item.useFunction = ItemFunction.CAST_TELEPORT;
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click location to teleport to, or right-click to cancel.");
                    break;

                case ItemType.Dagger:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.DAGGER,
                        new Color32(0xfe, 0x79, 0x58, 255),
                        C.FSTR.Set("Dagger"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.powerBonus = 2;
                    item.e.equippable.slot = EquipmentSlot.MainHand;
                    item.e.item.paperDoll = S.PAPERDOLL_DAGGER;

                    break;

                case ItemType.ShortSword:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SHORT_SWORD,
                        new Color32(120, 120, 140, 255),
                        C.FSTR.Set("Short Sword"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.powerBonus = 4;
                    item.e.equippable.slot = EquipmentSlot.MainHand;
                    item.e.item.paperDoll = S.PAPERDOLL_SHORT_SWORD;

                    break;

                case ItemType.Sword:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SWORD,
                        new Color32(150, 150, 180, 255),
                        C.FSTR.Set("Sword"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.powerBonus = 6;
                    item.e.equippable.slot = EquipmentSlot.MainHand;
                    item.e.item.paperDoll = S.PAPERDOLL_SWORD;

                    break;

                case ItemType.SerpentineSword:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SERPENTINE_SWORD,
                        new Color32(150, 200, 150, 255),
                        C.FSTR.Set("Serpentine Sword"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.powerBonus = 8;
                    item.e.equippable.slot = EquipmentSlot.MainHand;
                    item.e.item.paperDoll = S.PAPERDOLL_SWORD;

                    break;

                case ItemType.Shield:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.SHIELD,
                        new Color32(0xbf, 0x79, 0x58, 255),
                        C.FSTR.Set("Buckler"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 1;
                    item.e.equippable.slot = EquipmentSlot.OffHand;
                    item.e.item.paperDoll = S.PAPERDOLL_SHIELD;

                    break;

                case ItemType.LargeShield:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.LARGE_SHIELD,
                        new Color32(180, 170, 170, 255),
                        C.FSTR.Set("Heater Shield"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 3;
                    item.e.equippable.slot = EquipmentSlot.OffHand;
                    item.e.item.paperDoll = S.PAPERDOLL_LARGE_SHIELD;

                    break;

                case ItemType.Helmet:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.HELMET,
                        new Color32(180, 180, 200, 255),
                        C.FSTR.Set("Helmet"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 1;
                    item.e.equippable.slot = EquipmentSlot.Head;
                    item.e.item.paperDoll = S.PAPERDOLL_HELMET;

                    break;

                case ItemType.LeatherArmor:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.ARMOR,
                        new Color32(0xbf, 0x79, 0x58, 255),
                        C.FSTR.Set("Leather Armor"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 1;
                    item.e.equippable.slot = EquipmentSlot.Armor;
                    item.e.item.paperDoll = S.PAPERDOLL_ARMOR;

                    break;

                case ItemType.ChainArmor:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.ARMOR,
                        new Color32(175, 175, 220, 255),
                        C.FSTR.Set("Chainmail Armor"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 2;
                    item.e.equippable.slot = EquipmentSlot.Armor;
                    item.e.item.paperDoll = S.PAPERDOLL_ARMOR;

                    break;

                case ItemType.PlateArmor:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.ARMOR,
                        new Color32(200, 210, 200, 255),
                        C.FSTR.Set("Platemail Armor"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.defenseBonus = 3;
                    item.e.equippable.slot = EquipmentSlot.Armor;
                    item.e.item.paperDoll = S.PAPERDOLL_ARMOR;

                    break;

                case ItemType.Bow:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.BOW,
                        new Color32(0xbf, 0x79, 0x58, 255),
                        C.FSTR.Set("Bow"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.slot = EquipmentSlot.Ranged;
                    item.e.item.paperDoll = S.PAPERDOLL_BOW;
                    item.e.item.int1 = 5; // Damage
                    item.e.item.int2 = 8; // Range
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click an enemy to shoot it, or right-click to cancel. Press shoot again to target nearest.");

                    break;

                case ItemType.BowFine:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.BOW_FINE,
                        new Color32(150, 150, 180, 255),
                        C.FSTR.Set("Recurve Bow"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.slot = EquipmentSlot.Ranged;
                    item.e.item.paperDoll = S.PAPERDOLL_BOW;
                    item.e.item.int1 = 8; // Damage
                    item.e.item.int2 = 8; // Range
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click an enemy to shoot it, or right-click to cancel. Press shoot again to target nearest.");

                    break;

                case ItemType.BowElven:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.BOW_ELVEN,
                        new Color32(150, 200, 150, 255),
                        C.FSTR.Set("Elven Bow"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();
                    item.e.equippable = new Equippable();
                    item.e.equippable.slot = EquipmentSlot.Ranged;
                    item.e.item.paperDoll = S.PAPERDOLL_BOW;
                    item.e.item.int1 = 12; // Damage
                    item.e.item.int2 = 8; // Range
                    item.e.item.targeting = true;
                    item.e.item.targetingMessage = new FastString(96);
                    item.e.item.targetingMessage.Append("Left-click an enemy to shoot it, or right-click to cancel. Press shoot again to target nearest.");

                    break;

                case ItemType.Arrow:
                    item = EntityStore.CreateEntity();
                    item.e.Initialize(
                        pos,
                        S.ARROW,
                        new Color32(0xbf, 0x79, 0x58, 255),
                        C.FSTR.Set("Arrow"),
                        RenderFunctions.RenderOrder.ITEM);

                    item.e.item = new Item();

                    break;

                default:
                    return item;
            }

            item.e.item.type = itemType;

            return item;
        }

        /// <summary>
        /// Create interactable object of the given type
        /// </summary>
        /// <param name="objType">Type</param>
        /// <param name="pos">Position</param>
        /// <returns>Item entity</returns>
        public static EntityID CreateInteractable(InteractableType objType, Vector2i pos)
        {
            EntityID obj = EntityID.empty;

            switch (objType)
            {
                case InteractableType.Stairs:
                    {
                        obj = EntityStore.CreateEntity();
                        obj.e.Initialize(
                            pos,
                            S.STAIRS_DOWN,
                            Color.white,
                            C.FSTR.Set("Stairs down"),
                            RenderFunctions.RenderOrder.STAIRS);
                    }

                    break;

                case InteractableType.Portal:
                    {
                        obj = EntityStore.CreateEntity();
                        obj.e.Initialize(
                            pos,
                            S.PORTAL,
                            new Color32(0x3c, 0xac, 0xd7, 255),
                            C.FSTR.Set("Escape Portal"),
                            RenderFunctions.RenderOrder.INTERACTABLE);
                    }

                    break;

                case InteractableType.Well:
                    {
                        obj = EntityStore.CreateEntity();
                        obj.e.Initialize(
                            pos,
                            S.WELL,
                            new Color32(0xbb, 0xbb, 0xbb, 255),
                            C.FSTR.Set("Old Well"),
                            RenderFunctions.RenderOrder.INTERACTABLE);
                    }

                    break;

                case InteractableType.GameExit:
                    {
                        obj = EntityStore.CreateEntity();
                        obj.e.Initialize(
                            pos,
                            PackedSpriteID.empty,
                            new Color32(0xbb, 0xbb, 0xbb, 255),
                            C.FSTR.Set("Old Well"),
                            RenderFunctions.RenderOrder.INTERACTABLE);

                        obj.e.groundTrigger = new GroundTrigger(GroundTrigger.GroundTriggerType.GameWon);
                    }

                    break;

                case InteractableType.Web:
                    {
                        obj = EntityStore.CreateEntity();
                        obj.e.Initialize(
                            pos,
                            S.WEB,
                            new Color32(0xff, 0xff, 0xee, 150),
                            C.FSTR.Set("Web"),
                            RenderFunctions.RenderOrder.INTERACTABLE);

                        obj.e.groundTrigger = new GroundTrigger(GroundTrigger.GroundTriggerType.Web);
                    }

                    break;
            }

            return obj;
        }

        /// <summary>
        /// Create the player
        /// </summary>
        /// <param name="pos">Position</param>
        /// <returns>Player entity</returns>
        public static EntityID CreatePlayer(Vector2i pos)
        {
            var player = EntityStore.CreateEntity();
            var playerEntity = player.e;

            playerEntity.Initialize(
                pos,
                S.HERO,
                Color.white,
                C.FSTR.Set("Player"),
                RenderFunctions.RenderOrder.ACTOR,
                true);

            playerEntity.moveTrail = new MoveTrail();
            playerEntity.fighter = new Fighter(100, 1, 3);
            playerEntity.inventory = new Inventory(26);
            playerEntity.level = new Level();
            playerEntity.equipment = new Equipment();

            return player;
        }

        /// <summary>
        /// Get monster spawn chances for given dungeon level
        /// </summary>
        /// <param name="dungeonLevel">Level</param>
        /// <returns>Chances</returns>
        public static List<int> GetMonsterChances(int dungeonLevel)
        {
            var monsterChances = new List<int>((int)MonsterType.Count);
            for (int i = 0; i < (int)MonsterType.Count; i++)
            {
                monsterChances.Add(0);
            }

            monsterChances[(int)MonsterType.Rat] = 80;
            monsterChances[(int)MonsterType.Bat] = 60;

            monsterChances[(int)MonsterType.Spider] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(50, 2),
                    new RandomUtils.Tuple<int, int>(60, 3)
                },
                dungeonLevel);

            monsterChances[(int)MonsterType.Slime] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 1),
                    new RandomUtils.Tuple<int, int>(20, 2),
                    new RandomUtils.Tuple<int, int>(40, 3)
                },
                dungeonLevel);

            monsterChances[(int)MonsterType.Skeleton] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 2),
                    new RandomUtils.Tuple<int, int>(20, 3),
                    new RandomUtils.Tuple<int, int>(40, 4)
                },
                dungeonLevel);

            monsterChances[(int)MonsterType.ArcherSkeleton] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 3),
                    new RandomUtils.Tuple<int, int>(20, 4)
                },
                dungeonLevel);

            monsterChances[(int)MonsterType.Ghost] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 2),
                    new RandomUtils.Tuple<int, int>(20, 3),
                    new RandomUtils.Tuple<int, int>(30, 4),
                    new RandomUtils.Tuple<int, int>(40, 5)
                },
                dungeonLevel);

            return monsterChances;
        }

        /// <summary>
        /// Get item spawn chances for given dungeon level
        /// </summary>
        /// <param name="dungeonLevel">Level</param>
        /// <returns>Chances</returns>
        public static List<int> GetItemChances(int dungeonLevel)
        {
            var itemChances = new List<int>((int)ItemType.Count);
            for (int i = 0; i < (int)ItemType.Count; i++)
            {
                itemChances.Add(0);
            }

            itemChances[(int)ItemType.HealingPotion] = 35;

            itemChances[(int)ItemType.Helmet] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 1),
                    new RandomUtils.Tuple<int, int>(20, 2),
                    new RandomUtils.Tuple<int, int>(10, 3),
                    new RandomUtils.Tuple<int, int>(5, 4),
                },
                dungeonLevel);

            itemChances[(int)ItemType.ShortSword] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 1),
                    new RandomUtils.Tuple<int, int>(20, 2),
                    new RandomUtils.Tuple<int, int>(5, 3),
                    new RandomUtils.Tuple<int, int>(0, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.SerpentineSword] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 4),
                    new RandomUtils.Tuple<int, int>(10, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.LightningScroll] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(25, 3),
                    new RandomUtils.Tuple<int, int>(30, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.TeleportScroll] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 1),
                    new RandomUtils.Tuple<int, int>(10, 2),
                    new RandomUtils.Tuple<int, int>(20, 3),
                    new RandomUtils.Tuple<int, int>(10, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.FireBallScroll] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(25, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.ConfusionScroll] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 1),
                    new RandomUtils.Tuple<int, int>(15, 3)
                },
                dungeonLevel);

            itemChances[(int)ItemType.Sword] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 2)
                },
                dungeonLevel);

            itemChances[(int)ItemType.Shield] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 2),
                    new RandomUtils.Tuple<int, int>(10, 3),
                    new RandomUtils.Tuple<int, int>(15, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.LargeShield] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 3),
                    new RandomUtils.Tuple<int, int>(10, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.LeatherArmor] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 1),
                    new RandomUtils.Tuple<int, int>(20, 2),
                    new RandomUtils.Tuple<int, int>(10, 3),
                    new RandomUtils.Tuple<int, int>(5, 4),
                    new RandomUtils.Tuple<int, int>(0, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.ChainArmor] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 3),
                    new RandomUtils.Tuple<int, int>(20, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.PlateArmor] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.Bow] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 1),
                    new RandomUtils.Tuple<int, int>(15, 2),
                    new RandomUtils.Tuple<int, int>(5, 3),
                    new RandomUtils.Tuple<int, int>(0, 4)
                },
                dungeonLevel);

            itemChances[(int)ItemType.BowFine] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(5, 2),
                    new RandomUtils.Tuple<int, int>(10, 3),
                    new RandomUtils.Tuple<int, int>(15, 4),
                    new RandomUtils.Tuple<int, int>(5, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.BowElven] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 4),
                    new RandomUtils.Tuple<int, int>(15, 5)
                },
                dungeonLevel);

            itemChances[(int)ItemType.Arrow] =
                RandomUtils.FromDungeonLevel(
                new RandomUtils.Tuple<int, int>[]
                {
                    new RandomUtils.Tuple<int, int>(10, 1),
                    new RandomUtils.Tuple<int, int>(20, 2),
                    new RandomUtils.Tuple<int, int>(30, 3),
                    new RandomUtils.Tuple<int, int>(35, 4)
                },
                dungeonLevel);

            return itemChances;
        }
    }
}