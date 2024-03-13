
using UnityEngine;

namespace HoloToolkit.Unity
{
    public enum PivotAxis
    {
        // Rotate about all axes.
        Free,
        // Rotate about an individual axis.
        X,
        Y
    }

    /// <summary>
    /// The Billboard class implements the behaviors needed to keep a GameObject
    /// oriented towards the user.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        [Tooltip("Specifies the axis about which the object will rotate (Free rotates about both X and Y).")]
        public PivotAxis PivotAxis = PivotAxis.Free;

        /// <summary>
        /// Overrides the cached value of the GameObject's default rotation.
        /// </summary>
        public Quaternion DefaultRotation { get; private set; }

        private void Awake()
        {
            // Cache the GameObject's default rotation.
            DefaultRotation = gameObject.transform.rotation;
        }
        public Camera targetCamera;
        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            Camera cam = targetCamera ? targetCamera :Camera.main;
            // Get a Vector that points from the Camera to the target.
            Vector3 forward;
            Vector3 up;

            // Adjust for the pivot axis. We need a forward and an up for use with Quaternion.LookRotation
            switch (PivotAxis)
            {
                // If we're fixing one axis, then we're projecting the camera's forward vector onto
                // the plane defined by the fixed axis and using that as the new forward.
                case PivotAxis.X:
                    Vector3 right = transform.right; // Fixed right
                    forward = Vector3.ProjectOnPlane(cam.transform.forward, right).normalized;
                    up = Vector3.Cross(forward, right); // Compute the up vector
                    break;

                case PivotAxis.Y:
                    up = transform.up; // Fixed up
                    forward = Vector3.ProjectOnPlane(cam.transform.forward, up).normalized;
                    break;

                // If the axes are free then we're simply aligning the forward and up vectors
                // of the object with those of the camera. 
                case PivotAxis.Free:
                default:
                    forward = cam.transform.forward;
                    up = cam.transform.up;
                    break;
            }


            // Calculate and apply the rotation required to reorient the object
            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }
}