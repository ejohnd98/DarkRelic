namespace RetroBlitDemo
{
    using UnityEngine;

    /// <summary>
    /// Ceiling fan, just rotates around
    /// </summary>
    public class CeilingFan : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(new Vector3(0, -3.5f, 0));
        }
    }
}