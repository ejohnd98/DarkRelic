using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// RetroBlit Extensions to Unity built-in classes
/// </summary>
public static class RetroBlitExtensions
{
    /// <summary>
    /// Check if two colors are equal
    /// </summary>
    /// <param name="ths">This reference</param>
    /// <param name="other">Other color</param>
    /// <returns>True if equal</returns>
    public static bool Equals(this Color32 ths, Color32 other)
    {
        if (ths.r == other.r &&
            ths.g == other.g &&
            ths.b == other.b &&
            ths.a == other.a)
        {
            return true;
        }

        return false;
    }
}
