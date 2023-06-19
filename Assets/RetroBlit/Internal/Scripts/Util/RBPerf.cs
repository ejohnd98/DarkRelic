////#define RENDER_PERF_GRAPH
////#define RENDER_CHUNK_INFO

namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// Performance measuring tools
    /// </summary>
    public class RBPerf
    {
        private const int MAX_SAMPLE_POINTS = 128;

        private readonly float[] mUpdateDeltas = new float[MAX_SAMPLE_POINTS];
        private readonly float[] mRenderDeltas = new float[MAX_SAMPLE_POINTS];

#pragma warning disable 0414 // Unused warning
#pragma warning disable IDE0052 // Remove unread private members
        private RBAPI mRetroBlitAPI = null;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore 0414

        private int mUpdateDeltaIndex = 0;
        private int mRenderDeltaIndex = 0;

        private float mPreviousUpdateTime = -1;
        private float mPreviousRenderTime = -1;

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            for (int i = 0; i < MAX_SAMPLE_POINTS; i++)
            {
                mUpdateDeltas[i] = -1;
            }

            mRetroBlitAPI = api;
            return true;
        }

        /// <summary>
        /// Processes an update event, calculates timing
        /// </summary>
        public void UpdateEvent()
        {
            float currentTime = Time.realtimeSinceStartup;

            if (mPreviousUpdateTime > 0)
            {
                mUpdateDeltas[mUpdateDeltaIndex] = currentTime - mPreviousUpdateTime;

                mUpdateDeltaIndex++;
                if (mUpdateDeltaIndex == MAX_SAMPLE_POINTS)
                {
                    mUpdateDeltaIndex = 0;
                }
            }

            mPreviousUpdateTime = currentTime;
        }

        /// <summary>
        /// Processes a render event, calculates timing
        /// </summary>
        public void RenderEvent()
        {
            float currentTime = Time.realtimeSinceStartup;

            if (mPreviousRenderTime > 0)
            {
                mRenderDeltas[mRenderDeltaIndex] = currentTime - mPreviousRenderTime;

                mRenderDeltaIndex++;
                if (mRenderDeltaIndex == MAX_SAMPLE_POINTS)
                {
                    mRenderDeltaIndex = 0;
                }
            }

            mPreviousRenderTime = currentTime;
        }

        /// <summary>
        /// Draw performance data to display
        /// </summary>
        public void Draw()
        {
#if REDNER_CHUNK_INFO || RENDER_PERF_GRAPH
            var oldCamera = mRetroBlitAPI.Renderer.CameraGet();
#endif

#if RENDER_CHUNK_INFO
            mRetroBlitAPI.Renderer.CameraSet(new Vector2i(0, 0));

            mRetroBlitAPI.Tilemap.DebugRender();

            mRetroBlitAPI.Renderer.CameraSet(new Vector2i(oldCamera.x, oldCamera.y));
#endif

#if RENDER_PERF_GRAPH
            var displaySize = mRetroBlitAPI.HW.DisplaySize;

            mRetroBlitAPI.Renderer.CameraSet(new Vector2i(0, 0));

            var graphSize = new Vector2i(MAX_SAMPLE_POINTS, 80);

            mRetroBlitAPI.Renderer.DrawRectFill(displaySize.width - graphSize.width, displaySize.height - graphSize.height, graphSize.width, graphSize.height, new Color32(0, 0, 255, 64));

            for (int i = 0; i < MAX_SAMPLE_POINTS; i++)
            {
                int val = (int)(displaySize.height - (mUpdateDeltas[i] * 2000)) - 1;
                mRetroBlitAPI.Renderer.DrawPixel(displaySize.width - graphSize.width + i, val, new Color32(255, 128, 32, 255));

                val = (int)(displaySize.height - (mRenderDeltas[i] * 2000)) - 1;
                mRetroBlitAPI.Renderer.DrawPixel(displaySize.width - graphSize.width + i, val, new Color32(64, 255, 64, 255));
            }

            mRetroBlitAPI.Renderer.CameraSet(new Vector2i(oldCamera.x, oldCamera.y));
#endif
        }
    }
}
