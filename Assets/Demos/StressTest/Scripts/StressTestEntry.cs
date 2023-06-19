namespace RetroBlitDemoStressTest
{
    using UnityEngine;

    /// <summary>
    /// Stress Test entry
    /// </summary>
    public class StressTestEntry : MonoBehaviour
    {
        private void Awake()
        {
            // To get started call RetroBlit.Initialize with an instance of DemoReel
            RB.Initialize(new StressTest());
        }
    }
}
