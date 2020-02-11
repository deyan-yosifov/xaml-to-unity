using UnityEngine;
using UnityEngine.XR;

namespace CAGD.SampleScripts
{
    public class CameraMovement : MonoBehaviour
    {
        private const float threshold = 0.8f;
        [SerializeField]
        private Transform headTransform;
        [SerializeField, Range(0, 2)]
        private float movementSpeed = 0.2f;
        [SerializeField, Range(0, 90)]
        private float rotationAngle = 30f;
        [SerializeField]
        private bool horizontalMoveOnly = false;
        private float previousRotationDirection = 0;

        private void Update()
        {
            Vector2 axis2D = OVRInput.Get(OVRInput.Axis2D.Any);

            if (axis2D.magnitude > threshold)
            {
                float angle = -Vector2.SignedAngle(Vector2.up, axis2D);
                Vector3 moveVector = Quaternion.AngleAxis(angle, Vector3.up) * headTransform.forward;

                if (this.horizontalMoveOnly)
                {
                    moveVector.y = 0;
                }

                this.transform.position += Time.deltaTime * moveVector;
            }
            
            float rotationDirection = (OVRInput.GetDown(OVRInput.Button.One | OVRInput.Button.Three)) ? 1 : (OVRInput.GetDown(OVRInput.Button.Two | OVRInput.Button.Four) ? -1 : 0);

            if (rotationDirection != this.previousRotationDirection)
            {
                this.previousRotationDirection = rotationDirection;
                this.transform.rotation = Quaternion.AngleAxis(rotationDirection * this.rotationAngle, Vector3.up) * this.transform.rotation;
            }
        }
    }
}
