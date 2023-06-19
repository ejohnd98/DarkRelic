namespace RetroBlitDemoOldDays
{
    using UnityEngine;

    /// <summary>
    /// HelloWorld entry
    /// </summary>
    public class OldDaysEntry : MonoBehaviour
    {
        private void Awake()
        {
            // To get started call RetroBlit.Initialize with an instance of HelloWorld
            RB.Initialize(new RetroBlitDemoSuperFlagRun.SuperFlagRun(false, true));
        }
    }
}
