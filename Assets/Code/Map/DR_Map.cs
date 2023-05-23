using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Map : MonoBehaviour
{
    public const int MAX_MAP_SIZE = 50;

    public DR_Cell[,] Cells;
    public Vector2Int MapSize;

    public static DR_Map CreateMapFromImage(Texture2D MapTexture){
        DR_Map NewMap = new DR_Map();

        int Width = MapTexture.width;
        int Height = MapTexture.height;
        NewMap.MapSize = new Vector2Int(Width, Height);

        Color[] Pixels = MapTexture.GetPixels();
        //Create test map
        NewMap.Cells = new DR_Cell[Height,Width];
        for (int y = 0; y < Height; y++){
            for (int x = 0; x < Width; x++){
                int Index1D = y*Width + x;
                NewMap.Cells[y,x] = new DR_Cell();
                NewMap.Cells[y,x].bBlocksMovement = Pixels[Index1D].r < 0.5;
            }
        }
        return NewMap;
    }
}
