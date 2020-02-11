using CAGD.Controls.Controls3D.Visuals;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace CAGD.SampleScripts
{
    public class ControlPointsGrabber : MonoBehaviour
    {
        [SerializeField]
        private XRNode handNode = XRNode.LeftHand;
        [SerializeField]
        private float grabRadius = 0.1f;
        private PointVisual capturedPoint;
        private Vector3 inititialPointPosition;
        private Vector3 initialHandPosition;
        private bool lastGripPressed;
        
        private void Update()
        {
            InputDevice hand = InputDevices.GetDeviceAtXRNode(this.handNode);

            if (hand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) && 
                hand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                this.transform.localPosition = position;
                this.transform.localRotation = rotation;

                bool isGripPressed = (hand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton) && gripButton)
                    || (hand.TryGetFeatureValue(CommonUsages.grip, out float grip) && grip > 0.7f);

                if (this.lastGripPressed != isGripPressed)
                {
                    this.lastGripPressed = isGripPressed;

                    if (isGripPressed)
                    {
                        this.Grab();
                    }
                    else
                    {
                        this.Drop();
                    }
                }
                else
                {
                    this.MovePoint();
                }
            }
        }

        private void Grab()
        {
            this.capturedPoint = Physics.OverlapSphere(this.transform.position, this.grabRadius).Select(c => c.GetComponent<PointVisual>()).FirstOrDefault(p => p);

            if (this.capturedPoint)
            {
                this.initialHandPosition = this.transform.position;
                this.inititialPointPosition = this.capturedPoint.Position;
            }
        }

        private void MovePoint()
        {
            if (this.capturedPoint)
            {
                Vector3 direction = this.transform.position - this.initialHandPosition;
                this.capturedPoint.Position = this.inititialPointPosition + direction;
            }
        }

        private void Drop()
        {
            this.capturedPoint = null;
        }
    }
}
