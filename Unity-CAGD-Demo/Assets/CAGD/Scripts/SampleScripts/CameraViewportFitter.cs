using UnityEngine;

namespace CAGD.SampleScripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraViewportFitter : MonoBehaviour
    {        
        [SerializeField]
        private RectTransform viewportBounds;
        private Camera viewportCamera;

        private void Awake()
        {
            this.viewportCamera = this.GetComponent<Camera>();
        }

        private void Update()
        {
            Vector3[] corners = new Vector3[4];
            viewportBounds.GetWorldCorners(corners);
            this.viewportCamera.pixelRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }
}
