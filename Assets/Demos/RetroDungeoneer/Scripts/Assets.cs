namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Contains all game assets
    /// </summary>
    public class Assets
    {
        /// <summary>
        /// Sprite sheet
        /// </summary>
        public SpriteSheetAsset spriteSheet = new SpriteSheetAsset();

        /// <summary>
        /// Monster death sound
        /// </summary>
        public AudioAsset soundMonsterDeath = new AudioAsset();

        /// <summary>
        /// Player death sound
        /// </summary>
        public AudioAsset soundPlayerDeath = new AudioAsset();

        /// <summary>
        /// Player foot step sound
        /// </summary>
        public AudioAsset soundFootStep = new AudioAsset();

        /// <summary>
        /// Monster attack sound
        /// </summary>
        public AudioAsset soundMonsterAttack = new AudioAsset();

        /// <summary>
        /// Player attack sound
        /// </summary>
        public AudioAsset soundPlayerAttack = new AudioAsset();

        /// <summary>
        /// Inventory sound, used for pickup, drop, equip, de-equip
        /// </summary>
        public AudioAsset soundInventory = new AudioAsset();

        /// <summary>
        /// Drink sound
        /// </summary>
        public AudioAsset soundDrink = new AudioAsset();

        /// <summary>
        /// Menu open sound
        /// </summary>
        public AudioAsset soundMenuOpen = new AudioAsset();

        /// <summary>
        /// Menu close sound
        /// </summary>
        public AudioAsset soundMenuClose = new AudioAsset();

        /// <summary>
        /// Take stairs sound
        /// </summary>
        public AudioAsset soundStairs = new AudioAsset();

        /// <summary>
        /// Mouse pointer hovered selection changed
        /// </summary>
        public AudioAsset soundPointerSelect = new AudioAsset();

        /// <summary>
        /// Select option sound
        /// </summary>
        public AudioAsset soundSelectOption = new AudioAsset();

        /// <summary>
        /// Level up jingle sound
        /// </summary>
        public AudioAsset soundLevelUp = new AudioAsset();

        /// <summary>
        /// Fireball sound
        /// </summary>
        public AudioAsset soundFireBall = new AudioAsset();

        /// <summary>
        /// Lightning sound
        /// </summary>
        public AudioAsset soundLightning = new AudioAsset();

        /// <summary>
        /// Confusion sound
        /// </summary>
        public AudioAsset soundConfuse = new AudioAsset();

        /// <summary>
        /// Enter cheat mode sound
        /// </summary>
        public AudioAsset soundCheat = new AudioAsset();

        /// <summary>
        /// Aggro sound 1
        /// </summary>
        public AudioAsset soundAggro1 = new AudioAsset();

        /// <summary>
        /// Aggro sound 2
        /// </summary>
        public AudioAsset soundAggro2 = new AudioAsset();

        /// <summary>
        /// Player fall yell (for entrance effect)
        /// </summary>
        public AudioAsset soundPlayerFallYell = new AudioAsset();

        /// <summary>
        /// Portal teleport
        /// </summary>
        public AudioAsset soundPortal = new AudioAsset();

        /// <summary>
        /// Sound bow shoot
        /// </summary>
        public AudioAsset soundBowShoot = new AudioAsset();

        /// <summary>
        /// Sound bow hit
        /// </summary>
        public AudioAsset soundBowHit = new AudioAsset();

        /// <summary>
        /// Sound web
        /// </summary>
        public AudioAsset soundWeb = new AudioAsset();

        /// <summary>
        /// Portal teleport
        /// </summary>
        public AudioAsset soundJump = new AudioAsset();

        /// <summary>
        /// Teleport
        /// </summary>
        public AudioAsset soundTeleport = new AudioAsset();

        /// <summary>
        /// Slime
        /// </summary>
        public AudioAsset soundSlime = new AudioAsset();

        /// <summary>
        /// Music for main menu
        /// </summary>
        public AudioAsset musicMainMenu = new AudioAsset();

        /// <summary>
        /// Music for game play
        /// </summary>
        public AudioAsset musicGame = new AudioAsset();

        /// <summary>
        /// Music to play upon death
        /// </summary>
        public AudioAsset musicDeath = new AudioAsset();

        /// <summary>
        /// Music to play in forest
        /// </summary>
        public AudioAsset musicForest = new AudioAsset();

        /// <summary>
        /// Font
        /// </summary>
        public FontAsset fontRetroBlitDropShadow = new FontAsset();

        /// <summary>
        /// Small font to use
        /// </summary>
        public FontAsset fontSmall = new FontAsset();

        /// <summary>
        /// Shader to vignette effect
        /// </summary>
        public ShaderAsset shaderVignette = new ShaderAsset();

        /// <summary>
        /// Load all assets
        /// </summary>
        public void LoadAll()
        {
            spriteSheet.Load("RetroDungeoneer/SpritePack", SpriteSheetAsset.SheetType.SpritePack);
            spriteSheet.grid = new SpriteGrid(new Vector2i(16, 16));

            soundMonsterDeath.Load("RetroDungeoneer/Sounds/MonsterDeath");
            soundPlayerDeath.Load("RetroDungeoneer/Sounds/PlayerDeath");
            soundFootStep.Load("RetroDungeoneer/Sounds/FootStep");
            soundMonsterAttack.Load("RetroDungeoneer/Sounds/MonsterAttack");
            soundPlayerAttack.Load("RetroDungeoneer/Sounds/PlayerAttack");
            soundInventory.Load("RetroDungeoneer/Sounds/Inventory");
            soundDrink.Load("RetroDungeoneer/Sounds/Drink");
            soundMenuOpen.Load("RetroDungeoneer/Sounds/MenuOpen");
            soundMenuClose.Load("RetroDungeoneer/Sounds/MenuClose");
            soundStairs.Load("RetroDungeoneer/Sounds/Stairs");
            soundPointerSelect.Load("RetroDungeoneer/Sounds/PointerSelect");
            soundSelectOption.Load("RetroDungeoneer/Sounds/SelectOption");
            soundLevelUp.Load("RetroDungeoneer/Sounds/LevelUp");
            soundFireBall.Load("RetroDungeoneer/Sounds/Fireball");
            soundLightning.Load("RetroDungeoneer/Sounds/Lightning");
            soundConfuse.Load("RetroDungeoneer/Sounds/Confuse");
            soundCheat.Load("RetroDungeoneer/Sounds/CheatMode");
            soundAggro1.Load("RetroDungeoneer/Sounds/Aggro1");
            soundAggro2.Load("RetroDungeoneer/Sounds/Aggro2");
            soundPlayerFallYell.Load("RetroDungeoneer/Sounds/PlayerFallYell");
            soundPortal.Load("RetroDungeoneer/Sounds/Portal");
            soundJump.Load("RetroDungeoneer/Sounds/Jump");
            soundBowShoot.Load("RetroDungeoneer/Sounds/BowShoot");
            soundBowHit.Load("RetroDungeoneer/Sounds/BowHit");
            soundWeb.Load("RetroDungeoneer/Sounds/Web");
            soundTeleport.Load("RetroDungeoneer/Sounds/Teleport");
            soundSlime.Load("RetroDungeoneer/Sounds/Slime");

            musicMainMenu.Load("RetroDungeoneer/Music/ReturnToNowhere");
            musicGame.Load("RetroDungeoneer/Music/DungeonAmbience");
            musicDeath.Load("RetroDungeoneer/Music/DeathPiano");
            musicForest.Load("RetroDungeoneer/Music/ForestAmbience");

            RB.SpriteSheetSet(spriteSheet);
            var fontSprite = RB.PackedSpriteGet(S.FONT_RETROBLIT_DROPSHADOW);
            var fontPos = new Vector2i(fontSprite.SourceRect.x + 1, fontSprite.SourceRect.y + 1);

            fontRetroBlitDropShadow.Setup('!', (char)((int)'~' + 8), fontPos, spriteSheet, new Vector2i(6, 7), ((int)'~' + 8) - (int)'!' + 1, 1, 1, false);
            fontSmall = fontRetroBlitDropShadow;

            shaderVignette.Load("RetroDungeoneer/DrawVignette");
        }
    }
}
