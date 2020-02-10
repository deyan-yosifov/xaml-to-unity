using UnityEngine;
using UnityEngine.XR;

namespace CAGD.SampleScripts
{
    public class ScenePlatformsManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] vrSpecificObjects;
        [SerializeField]
        private Transform[] nonVRSpecificObjects;

        private void Awake()
        {
            bool isVRPlatform = !string.IsNullOrEmpty(XRDevice.model);

            Transform[] platformSpecificObjects = isVRPlatform ? vrSpecificObjects : nonVRSpecificObjects;

            for (int i = 0; i < platformSpecificObjects.Length; i++)
            {
                platformSpecificObjects[i].gameObject.SetActive(true);
            }
        }
    }
}
