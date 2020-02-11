using UnityEngine;

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
        private bool horizontalMoveOnly = true;
        private float previousRotationDirection = 0;

        private void Update()
        {
            float movementAxis = Input.GetAxis("Vertical");
            float rotationAxis = Input.GetAxis("Horizontal");

            float movedirection = (movementAxis > threshold) ? 1 : (movementAxis < -threshold ? -1 : 0);
            Vector3 moveVector = this.horizontalMoveOnly ? new Vector3(headTransform.forward.x, 0, headTransform.forward.z) : headTransform.forward;
            this.transform.position += Time.deltaTime * movedirection * moveVector;
            
            float rotationDirection = (rotationAxis > threshold) ? 1 : (rotationAxis < -threshold ? -1 : 0);

            if (rotationDirection != this.previousRotationDirection)
            {
                this.previousRotationDirection = rotationDirection;
                this.transform.rotation = Quaternion.AngleAxis(rotationDirection * this.rotationAngle, Vector3.up) * this.transform.rotation;
            }
        }
    }
}
