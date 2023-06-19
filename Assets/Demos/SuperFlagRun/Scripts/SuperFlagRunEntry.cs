namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Super Flag Run entry
    /// </summary>
    public class SuperFlagRunEntry : MonoBehaviour
    {
        private void Awake()
        {
            // To get started call RetroBlit.Initialize with an instance of SuperFlagRun
            RB.Initialize(new SuperFlagRun(true, false));
        }
    }
}
