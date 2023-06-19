namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demo Reel entry
    /// </summary>
    public class DemoReelEntry : MonoBehaviour
    {
        private void Awake()
        {
            // To get started call RetroBlit.Initialize with an instance of DemoReel
            RB.Initialize(new DemoReel());
        }
    }
}
