// SOURCE_FOLDER is the folder containing unpacked sprites, all subfolders will also be packed. The
// folder is relative to your project root folder (i.e. the folder containing the Assets folder). It
// is recommended that the folder is normally outside of your Assets folder so that Unity does not
// put the unpacked sprites into the asset database. 
//
// In the DemoReel example the unpacked sprites folder is placed under the Assets folder only because it
// would otherwise not be included at all in the Asset Store bundle!
//
// You can specify multiple SOURCE_FOLDERs and they will all be included in the sprite pack. This may
// be useful if you want to have some common subset of sprites (for example the player sprite) appear
// in all sprite packs. Note that if there is a duplicate sprite name between multiple SOURCE_FOLDERs
// then the duplicate sprite will be dropped, only unique names are allowed. If you require duplicate
// names then put the sprites under different subfolders:
//
// player/walking.png
// enemy/walking.png
//
// These would be two different sprites named "player/walking" and "enemy/walking"
//
SOURCE_FOLDER=Assets/Demos/RetroDungeoneer/Resources/RetroDungeoneer/SpritePack

// Output sprite texture width and height. If all sprites cannot fit in this space then the sprite pack
// generation will fail.
OUTPUT_WIDTH=1024
OUTPUT_HEIGHT=512

// Trim whitespace from sprites to save on texture space. If in doubt set this to true, you would only
// want to skip trimming in some rare specific cases.
TRIM=true
