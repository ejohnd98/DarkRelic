using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SightData{
    //put list of actors?
}

public class SightSystem
{
    public static SightData CalculateVisibleCells(DR_Entity entity, DR_Map map){
        SightData sightData;

        //TODO determine if this should modify the map, or only return what cells are visible
        // the former is probably easiest, and probably only the player actually cares about what cells exactly can be seen

        // for AI it would be enough to just check if it has LOS with any important objects nearby

        return sightData;
    }
}
