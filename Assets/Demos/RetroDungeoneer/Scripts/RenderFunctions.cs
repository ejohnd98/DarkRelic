namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Functions for rendering the game state
    /// </summary>
    public static class RenderFunctions
    {
        /// <summary>
        /// List of render orders that is iterate when rendering
        /// </summary>
        public static RenderOrder[] renderOrderList;

        /// <summary>
        /// Rendering order, acts like a layer
        /// </summary>
        public enum RenderOrder
        {
            /// <summary>
            /// Hidden layer, this is where fog is rendered
            /// </summary>
            HIDDEN = 0,

            /// <summary>
            /// Stairs
            /// </summary>
            STAIRS = 1,

            /// <summary>
            /// Corpse
            /// </summary>
            CORPSE,

            /// <summary>
            /// Item
            /// </summary>
            ITEM,

            /// <summary>
            /// Effects underneath actor sprites
            /// </summary>
            ACTOR_UNDERLAY_EFFECTS,

            /// <summary>
            /// Interactable effects
            /// </summary>
            INTERACTABLE,

            /// <summary>
            /// A moving entity such as player or monster
            /// </summary>
            ACTOR,

            /// <summary>
            /// Visual effects that appear on top of entities
            /// </summary>
            ACTOR_OVERLAY_EFFECTS,

            /// <summary>
            /// Top layer
            /// </summary>
            TOP_MOST,
        }

        /// <summary>
        /// Initialize the render functions
        /// </summary>
        public static void Initialize()
        {
            var renderOrderArray = System.Enum.GetValues(typeof(RenderOrder));
            renderOrderList = new RenderOrder[renderOrderArray.Length];

            for (int i = 0; i < renderOrderArray.Length; i++)
            {
                renderOrderList[i] = (RenderOrder)renderOrderArray.GetValue(i);
            }
        }

        /// <summary>
        /// Render the game state
        /// </summary>
        /// <param name="gameState">Game state</param>
        /// <param name="player">Player</param>
        /// <param name="menu">Current menu</param>
        /// <param name="console">Console</param>
        public static void RenderAll(GameState gameState, EntityID player, Menu menu, Console console)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            RB.ShaderSet(game.assets.shaderVignette);

            // Use a rect fill instead of RB.Clear() so that the shader will apply
            var cameraPos = RB.CameraGet();
            RB.CameraReset();
            RB.DrawRectFill(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), game.map.backgroundColor);
            RB.CameraSet(cameraPos);

            RB.DrawMapLayer(C.LAYER_GRID);
            RB.DrawMapLayer(C.LAYER_TERRAIN);

            var previousTintColor = RB.TintColorGet();

            EffectManager.Instance.Render(RenderOrder.ACTOR_UNDERLAY_EFFECTS);

            foreach (var renderOrder in renderOrderList)
            {
                if (renderOrder == RenderOrder.HIDDEN)
                {
                    continue;
                }

                foreach (var entity in EntityStore.entities)
                {
                    if (renderOrder == RenderOrder.ACTOR_UNDERLAY_EFFECTS)
                    {
                        if (entity.e.moveTrail != null)
                        {
                            RenderTrail(entity);
                        }
                    }

                    if (entity.e.renderOrder == renderOrder)
                    {
                        RenderEntity(entity);
                    }
                }
            }

            EffectManager.Instance.Render(RenderOrder.ACTOR_OVERLAY_EFFECTS);

            RB.TintColorSet(previousTintColor);

            // Draw visibility layer on top, covering tiles as needed
            RB.DrawMapLayer(C.LAYER_VISIBILITY);

            RenderMapBorder();

            EffectManager.Instance.Render(RenderOrder.TOP_MOST);

            RB.ShaderReset();

            console.Render();

            RenderUI(gameState, menu, player);

            if (menu == null)
            {
                RenderMouseHover();
            }

            if (gameState == GameState.TARGETING)
            {
                RenderTargeting();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public static void Update()
        {
            const int FADE_SPEED = 10;

            // Update trails
            foreach (var entity in EntityStore.entities)
            {
                if (entity.e.moveTrail != null)
                {
                    var trail = entity.e.moveTrail.trail;
                    for (int i = 0; i < trail.Count; i++)
                    {
                        if (trail[i].fade < FADE_SPEED)
                        {
                            trail[i].fade = 0;
                        }
                        else
                        {
                            trail[i].fade -= FADE_SPEED;
                        }
                    }

                    // Popup a fade trail bit
                    if (trail.Count > 0 && trail[0].fade == 0)
                    {
                        trail.RemoveAt(0);
                    }
                }
            }
        }

        private static void RenderTrail(EntityID entity)
        {
            var game = (RetroDungeoneerGame)RB.Game;
            var moveTrail = entity.e.moveTrail;

            RB.TintColorSet(Color.white);

            for (int i = 0; i < moveTrail.trail.Count; i++)
            {
                var t = moveTrail.trail[i];
                RB.AlphaSet(t.fade);
                RB.DrawEllipseFill(t.pos + new Vector2i(game.assets.spriteSheet.grid.cellSize.width / 2, game.assets.spriteSheet.grid.cellSize.height / 2), new Vector2i(2, 2), Color.white);
            }

            RB.AlphaSet(255);
        }

        private static void RenderMapBorder()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            // Draw filled rectangles on all sides of the map to cover up areas with no tiles in the same color
            // as tiles that are currently not visible
            RB.DrawRectFill(
                new Rect2i(
                    -RB.DisplaySize.width,
                    0,
                    RB.DisplaySize.width,
                    game.map.size.height * game.assets.spriteSheet.grid.cellSize.height),
                    game.map.unexploredColor);

            RB.DrawRectFill(
                new Rect2i(
                    game.map.size.width * game.assets.spriteSheet.grid.cellSize.width,
                    0,
                    RB.DisplaySize.width,
                    game.map.size.height * game.assets.spriteSheet.grid.cellSize.height),
                    game.map.unexploredColor);

            RB.DrawRectFill(
                new Rect2i(
                    -RB.DisplaySize.width,
                    -RB.DisplaySize.height,
                    (game.map.size.width * game.assets.spriteSheet.grid.cellSize.width) + (RB.DisplaySize.width * 2),
                    RB.DisplaySize.height),
                    game.map.unexploredColor);

            RB.DrawRectFill(
                new Rect2i(
                    -RB.DisplaySize.width,
                    game.map.size.height * game.assets.spriteSheet.grid.cellSize.height,
                    (game.map.size.width * game.assets.spriteSheet.grid.cellSize.width) + (RB.DisplaySize.width * 2),
                    RB.DisplaySize.height),
                    game.map.unexploredColor);
        }

        private static void RenderEntity(EntityID entity)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var e = entity.e;

            if (game.map.IsInFOV(e.pos) || (e.stairs != null && game.map.IsInExplored(e.pos)))
            {
                RB.TintColorSet(e.color);
                RB.AlphaSet(e.color.a);

                RB.DrawSprite(e.sprite, new Vector2i(e.pos.x * game.assets.spriteSheet.grid.cellSize.width, e.pos.y * game.assets.spriteSheet.grid.cellSize.height));

                // Render paper doll items
                if (e.equipment != null)
                {
                    for (int i = 0; i < e.equipment.equipment.Length; i++)
                    {
                        if (!e.equipment.equipment[i].isEmpty && e.equipment.equipment[i].e.item != null)
                        {
                            var itemEntity = e.equipment.equipment[i].e;
                            RB.TintColorSet(itemEntity.color);
                            RB.DrawSprite(itemEntity.item.paperDoll, new Vector2i(e.pos.x * game.assets.spriteSheet.grid.cellSize.width, e.pos.y * game.assets.spriteSheet.grid.cellSize.height));
                        }
                    }
                }

                RB.AlphaSet(255);
            }
        }

        private static void RenderBar(Vector2i pos, Vector2i size, string name, int value, int maximum, Color32 barColor, Color32 backColor, Color32 frameColor)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            int barWidth = (int)((float)value / maximum * (size.width - 2));

            RB.DrawRectFill(new Rect2i(pos.x, pos.y, size.width, size.height), backColor);

            if (barWidth > 0)
            {
                RB.DrawRectFill(new Rect2i(pos.x + 1, pos.y, barWidth, size.height), barColor);
            }

            RB.DrawRect(new Rect2i(pos.x, pos.y, size.width, size.height), frameColor);

            C.FSTR.Set(name).Append(": ").Append(value).Append(" / ").Append(maximum);

            RB.Print(game.assets.fontSmall, new Rect2i(pos.x, pos.y + 1, size.width, size.height), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, C.FSTR);
        }

        private static void RenderUI(GameState gameState, Menu menu, EntityID player)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            // Preserve camera location
            var cameraPos = RB.CameraGet();
            RB.CameraReset();

            var pe = player.e;

            var barSize = new Vector2i(96, 12);
            var barPos = new Vector2i(RB.DisplaySize.width - barSize.width - 4, RB.DisplaySize.height - barSize.height - 4 - 10);
            RenderBar(
                barPos,
                barSize,
                "HP",
                pe.fighter.hp,
                pe.fighter.maxHp,
                new Color32(0xFF, 0x50, 0x50, 255),
                new Color32(0x50, 0x20, 0x20, 255),
                Color.white);

            if (game.map.dungeonLevel == 0 || game.map.dungeonLevel == 6)
            {
                RB.Print(game.assets.fontSmall, new Vector2i(barPos.x, barPos.y + barSize.height + 4), Color.white, C.FSTR.Set("Forest Clearing"));
            }
            else
            {
                RB.Print(game.assets.fontSmall, new Vector2i(barPos.x, barPos.y + barSize.height + 4), Color.white, C.FSTR.Set("Dungeon level: ").Append(game.map.dungeonLevel));
            }

            if (gameState == GameState.SHOW_INVENTORY || gameState == GameState.DROP_INVENTORY)
            {
                int itemsInInventory = 0;

                if (gameState == GameState.SHOW_INVENTORY)
                {
                    menu.SetSummary(C.FSTR.Set("Choose an item to use, or ESC to cancel."));
                }
                else
                {
                    menu.SetSummary(C.FSTR.Set("Choose an item to drop, or ESC to cancel."));
                }

                if (pe.inventory != null)
                {
                    menu.ClearOptions();

                    for (int i = 0; i < pe.inventory.items.Length; i++)
                    {
                        var item = pe.inventory.items[i].e;
                        if (item != null)
                        {
                            bool equipped = false;

                            if (pe.equipment != null)
                            {
                                equipped = pe.equipment.ContainsItem(item.id);
                            }

                            C.FSTR.Set(item.name);

                            if (item.equippable != null && (item.equippable.powerBonus != 0 || item.equippable.defenseBonus != 0))
                            {
                                C.FSTR.Append(" @707070(");

                                if (item.equippable.powerBonus != 0)
                                {
                                    if (item.equippable.powerBonus > 0)
                                    {
                                        C.FSTR.Append("+");
                                    }

                                    C.FSTR.Append(item.equippable.powerBonus).Append(" pwr");

                                    if (item.equippable.defenseBonus != 0)
                                    {
                                        C.FSTR.Append(" ");
                                    }
                                }

                                if (item.equippable.defenseBonus != 0)
                                {
                                    if (item.equippable.defenseBonus > 0)
                                    {
                                        C.FSTR.Append("+");
                                    }

                                    C.FSTR.Append(item.equippable.defenseBonus).Append(" def");
                                }

                                C.FSTR.Append(")");
                            }

                            if (equipped)
                            {
                                C.FSTR.Append(" @7070B0[equipped]@-");
                            }

                            menu.AddOption(C.FSTR);

                            itemsInInventory++;
                        }
                    }
                }

                if (itemsInInventory == 0)
                {
                    menu.SetSummary(C.FSTR.Set("Your inventory is empty. Press ESC to exit."));
                }

                menu.Render();
            }
            else if (menu != null)
            {
                menu.Render();
            }

            RB.CameraSet(cameraPos);
        }

        private static void RenderMouseHover()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var tilePos = SceneGame.GetMouseTilePos();
            if (tilePos.x == -1)
            {
                return;
            }

            EntityFunctions.GetEntitiesStringAtTile(tilePos, C.FSTR);

            if (C.FSTR.Length > 0)
            {
                // Preserve camera location
                var cameraPos = RB.CameraGet();
                RB.CameraReset();

                var mousePos = RB.PointerPos();

                var anchorPos = new Vector2i(mousePos.x + 4, mousePos.y + 12);

                var textSize = RB.PrintMeasure(game.assets.fontSmall, C.FSTR);
                var textRect = new Rect2i(anchorPos.x, anchorPos.y, textSize.width, textSize.height);
                var textFrameRect = new Rect2i(textRect.x - 4, textRect.y - 4, textRect.width + 8, textRect.height + 8);

                RB.DrawRectFill(textFrameRect, C.COLOR_MENU_BACKGROUND);
                RB.DrawRect(textFrameRect, Color.white);
                RB.Print(game.assets.fontSmall, textRect, Color.white, 0, C.FSTR);

                RB.CameraSet(cameraPos);
            }
        }

        private static void RenderTargeting()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            var tilePos = SceneGame.GetMouseTilePos();

            if (tilePos.x != -1)
            {
                var renderPos = new Vector2i(
                    tilePos.x * game.assets.spriteSheet.grid.cellSize.width,
                    tilePos.y * game.assets.spriteSheet.grid.cellSize.height);

                PackedSpriteID sprite = S.TARGET_1;

                if (RB.Ticks % 20 < 10)
                {
                    sprite = S.TARGET_2;
                }

                RB.TintColorSet(new Color32(255, 0, 0, 200));
                RB.DrawSprite(sprite, renderPos);
                RB.TintColorSet(Color.white);
            }
        }
    }
}
