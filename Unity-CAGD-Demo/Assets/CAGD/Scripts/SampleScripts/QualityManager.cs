using UnityEngine;
using UnityEngine.XR;

namespace CAGD.SampleScripts
{
    public class QualityManager : MonoBehaviour
    {
        private void Awake()
        {
            XRSettings.eyeTextureResolutionScale = 1.33f;
            XRSettings.renderViewportScale = 1f;
            QualitySettings.antiAliasing = 4;
        }
    }
}
