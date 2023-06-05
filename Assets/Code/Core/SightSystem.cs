using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightSystem
{
    public static void CalculateVisibleCells(DR_Entity entity, DR_Map map){
        Vector2 pos = entity.GetPosFloat();
        map.ClearVisible();

        //temp sightdist
        int sightDist = 8;

        float ox, oy; //offset values
        float stepDist = 0.025f;

        for (float a = 0; a < 360; a += 0.5f) {//sends 720 rays in 360 degrees (0.5 degrees each loop)
            ox = stepDist * Mathf.Cos(Mathf.PI * a / 180.0f);
            oy = stepDist * Mathf.Sin(Mathf.PI * a / 180.0f);
            CastRay(map, pos.x, pos.y, ox, oy, stepDist, sightDist);
        }
    }

    private static void CastRay(DR_Map map, float ex, float ey, float ox, float oy, float stepDist, int sight){
        float x, y;
        int lastX, lastY, dirXMod, dirYMod; //used for checking diagonal walls (an edge case where players see through 2 walls at a diagonal)
        dirXMod = (int)Mathf.Sign(ox); //sets to a value of 1 with sign depending on direction of ray (-1 or 1)
        dirYMod = (int)Mathf.Sign(oy);
        x = (float)ex + 0.5f; //initial position to start from
        y = (float)ey + 0.5f;
        lastX = (int)x;
        lastY = (int)y;
        for (int i = 0; i * stepDist <= sight; i++) { //increment ray position until reached sight limit
            bool isWall = false;
            if (!map.ValidPosition((int)x, (int)y)) { //return if reached an invalid position (outside map)
                return;
            }
            if (map.BlocksSight((int)x, (int)y)) {
                isWall = true;
            }
            if (((int)x) == lastX + dirXMod && ((int)y) == lastY + dirYMod) { //if moved diagonally, check for diagonal walls
                if (map.BlocksSight(lastX + dirXMod, lastY) && map.BlocksSight(lastX, lastY + dirYMod)) {
                    if (isWall) { //still want to see walls placed in corners (but not open cells)
                        map.IsVisible[(int)y, (int)x] = true;
                        map.IsKnown[(int)y, (int)x] = true;
                    }
                    return; //current cell is through a diagonal wall, so stop ray
                }
            }
            //make current space visible
            map.IsVisible[(int)y, (int)x] = true;
            map.IsKnown[(int)y, (int)x] = true;
            if (isWall) { //stop ray if current cell is a wall
                return;
            }
            lastX = (int)x;
            lastY = (int)y;
            //increment ray position
            x += ox;
            y += oy;
        }
    }
}
