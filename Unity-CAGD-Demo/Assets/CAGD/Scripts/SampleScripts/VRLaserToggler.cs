using UnityEngine;

namespace CAGD.SampleScripts
{
    [RequireComponent(typeof(LaserPointer))]
    public class VRLaserToggler : MonoBehaviour
    {
        private void Start()
        {
            this.GetComponent<LaserPointer>().laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;
        }
    }
}
