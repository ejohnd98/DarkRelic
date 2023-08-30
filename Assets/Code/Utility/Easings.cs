using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://easings.net

public class Easings
{
    public static Vector3 Slerp(Vector3 a, Vector3 b, float x) {
        return Vector3.Slerp(a, b, x);
    }

    public static Vector3 QuadEaseOut(Vector3 a, Vector3 b, float x) {
        float val = 1 - (1 - x) * (1 - x);
        val = 1.0f - val;
        return (a * val) + (b * (1.0f - val));
    }

    public static float QuadEaseOut(float a, float b, float x) {
        float val = 1 - (1 - x) * (1 - x);
        val = 1.0f - val;
        return (a * val) + (b * (1.0f - val));
    }

    public static float EaseInBack(float x) {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1.0f;
        return (c3 * x * x * x) - (c1 * x * x);
    }

    public static Vector3 EaseInBack(Vector3 a, Vector3 b, float x) {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1.0f;
        float val = 1.0f - ((c3 * x * x * x) - (c1 * x * x));
        return (a * val) + (b * (1.0f - val));
    }
}
