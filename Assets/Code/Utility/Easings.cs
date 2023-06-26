using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Easings
{
    public static Vector3 QuadEaseOut(Vector3 a, Vector3 b, float time) {
        float val = 1 - (1 - time) * (1 - time);
        val = 1.0f - val;
        return (a * val) + (b * (1.0f - val));
    }

    public static float QuadEaseOut(float a, float b, float time) {
        float val = 1 - (1 - time) * (1 - time);
        val = 1.0f - val;
        return (a * val) + (b * (1.0f - val));
    }
}
