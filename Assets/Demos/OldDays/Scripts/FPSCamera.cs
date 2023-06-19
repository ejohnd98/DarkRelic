namespace RetroBlitDemo
{
    using UnityEngine;

    /// <summary>
    /// Normal FPS camera
    /// </summary>
    public sealed class FPSCamera : MonoBehaviour
    {
        /// <summary>
        /// Reference to the camera
        /// </summary>
        public Camera CameraReference;

        private readonly float mSensitivityX = 2.5f;
        private readonly float mSensitivityY = 2.5f;
        private readonly float mMinimumX = -360.0f;
        private readonly float mMaximumX = 360.0f;
        private readonly float mMinimumY = -60.0f;
        private readonly float mMaximumY = 60.0f;
        private float mRotationX = 0.0f;
        private float mRotationY = 0.0f;

        private void Start()
        {
            mRotationX = CameraReference.transform.localRotation.x;
            mRotationY = CameraReference.transform.localRotation.y;
        }

        private void Update()
        {
            float rotationSensitivityAdjust = 1.0f;
            mRotationX += Input.GetAxis("Mouse X") * mSensitivityX * rotationSensitivityAdjust;
            mRotationY += Input.GetAxis("Mouse Y") * mSensitivityY * rotationSensitivityAdjust;

            Adjust360andClamp();

            KeyLookAround();
            KeyLookUp();
        }

        private void KeyLookAround()
        {
            Adjust360andClamp();
            transform.localRotation = Quaternion.AngleAxis(mRotationX, Vector3.up);
        }

        private void KeyLookUp()
        {
            Adjust360andClamp();
            transform.localRotation *= Quaternion.AngleAxis(mRotationY, Vector3.left);
        }

        private void Adjust360andClamp()
        {
            if (mRotationX < -360)
            {
                mRotationX += 360;
            }
            else if (mRotationX > 360)
            {
                mRotationX -= 360;
            }

            if (mRotationY < -360)
            {
                mRotationY += 360;
            }
            else if (mRotationY > 360)
            {
                mRotationY -= 360;
            }

            mRotationX = Mathf.Clamp(mRotationX, mMinimumX, mMaximumX);
            mRotationY = Mathf.Clamp(mRotationY, mMinimumY, mMaximumY);
        }
    }
}
