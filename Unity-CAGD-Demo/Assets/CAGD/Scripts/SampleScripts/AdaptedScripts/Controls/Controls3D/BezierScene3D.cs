using CAGD.Controls.Controls3D.Iteractions;
using CAGD.Controls.Controls3D.Visuals;
using UnityEngine;

namespace CAGD.Controls.Controls3D
{
    public class BezierScene3D : MonoBehaviour
    {
        public PointVisual controlPointPrefab;
        public LineVisual surfaceLinePrefab;
        public LineVisual controlLinePrefab;
        public SurfaceVisual surfacePrefab;
        public IteractivePointsHandler iteractivePointsHandler;

        public PointVisual AddIteractivePoint()
        {
            PointVisual instance = Instantiate<PointVisual>(this.controlPointPrefab, this.transform);

            if (this.iteractivePointsHandler && this.iteractivePointsHandler.isActiveAndEnabled)
            {
                this.iteractivePointsHandler.RegisterIteractivePoint(instance);
            }

            this.SetLayerRecursively(instance.gameObject);

            return instance;
        }

        public LineVisual AddLine(LineType lineType)
        {
            LineVisual prefab = lineType == LineType.SurfaceLine ? this.surfaceLinePrefab : this.controlLinePrefab;
            LineVisual instance = Instantiate<LineVisual>(prefab, this.transform);
            this.SetLayerRecursively(instance.gameObject);

            return instance;
        }

        public SurfaceVisual AddSurface()
        {
            SurfaceVisual instance = Instantiate<SurfaceVisual>(this.surfacePrefab, this.transform);
            this.SetLayerRecursively(instance.gameObject);

            return instance;
        }

        private void SetLayerRecursively(GameObject obj)
        {
            obj.layer = this.gameObject.layer;

            foreach (Transform child in obj.transform)
            {
                if (child)
                {
                    SetLayerRecursively(child.gameObject);
                }
            }
        }

        public enum LineType
        {
            SurfaceLine,
            ControlLine
        }
    }
}
