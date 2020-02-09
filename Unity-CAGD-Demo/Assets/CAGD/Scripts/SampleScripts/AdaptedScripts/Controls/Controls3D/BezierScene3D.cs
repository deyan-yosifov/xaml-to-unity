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
            this.iteractivePointsHandler.RegisterIteractivePoint(instance);

            return instance;
        }

        public LineVisual AddLine(LineType lineType)
        {
            LineVisual prefab = lineType == LineType.SurfaceLine ? this.surfaceLinePrefab : this.controlLinePrefab;
            LineVisual instance = Instantiate<LineVisual>(prefab, this.transform);

            return instance;
        }

        public SurfaceVisual AddSurface()
        {
            SurfaceVisual instance = Instantiate<SurfaceVisual>(this.surfacePrefab, this.transform);

            return instance;
        }

        public enum LineType
        {
            SurfaceLine,
            ControlLine
        }
    }
}
