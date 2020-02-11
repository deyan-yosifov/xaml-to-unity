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
            QualitySettings.antiAliasing = 4;
            bool isVRPlatform = !string.IsNullOrEmpty(XRDevice.model);

            if (isVRPlatform)
            {
                XRSettings.eyeTextureResolutionScale = 1.33f;
                XRSettings.renderViewportScale = 1f;
            }

            Transform[] platformSpecificObjects = isVRPlatform ? vrSpecificObjects : nonVRSpecificObjects;

            for (int i = 0; i < platformSpecificObjects.Length; i++)
            {
                platformSpecificObjects[i].gameObject.SetActive(true);
            }
        }
    }
}
