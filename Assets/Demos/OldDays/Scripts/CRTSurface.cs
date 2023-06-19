namespace RetroBlitDemo
{
    using UnityEngine;

    /// <summary>
    /// Renders RetroBlit display surface to CRT display
    /// </summary>
    public class CRTSurface : MonoBehaviour
    {
        private Renderer mRenderer;

        // Use this for initialization
        private void Start()
        {
            mRenderer = GetComponent<Renderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            var tex = RB.DisplaySurface;
            tex.filterMode = FilterMode.Bilinear;

            mRenderer.materials[2].mainTexture = tex;
            mRenderer.materials[2].color = Color.white;
        }
    }
}