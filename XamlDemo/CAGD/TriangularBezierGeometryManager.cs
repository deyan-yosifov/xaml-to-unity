using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Visuals;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TriangularBezierGeometryManager : BezierGeometryManagerBase<TriangularBezierGeometryContext>
    {
        private int degree;
        private int devisions;
        private BezierTriangle surfacePoints;
        private PointVisual[] controlPoints;

        public TriangularBezierGeometryManager(Scene3D scene)
            : base(scene)
        {
            this.degree = 0;
            this.devisions = 0;
            this.surfacePoints = null;
            this.controlPoints = null;
        }

        public void GenerateGeometry(Point3D[] controlPoints, int degree, TriangularBezierGeometryContext geometryContext)
        {
            this.DeleteOldControlPoints();
            this.GenerateNewControlPointsGeometry(controlPoints, degree, geometryContext.ShowControlPoints);

            if (geometryContext.ShowControlLines)
            {
                this.ShowControlLines();
            }
            else
            {
                this.HideControlLines();
            }

            this.GenerateGeometry(geometryContext);
        }

        protected override IEnumerable<Point3D> GetControlPoints()
        {
            foreach (PointVisual point in this.controlPoints)
            {
                yield return point.Position;
            }
        }

        protected override int ControlPointsCount()
        {
            return this.controlPoints.Length;
        }

        protected override int GetSurfaceLinesCount()
        {
            return CalculateTriangularMeshLinesCount(this.devisions);
        }

        protected override int GetControlLinesCount()
        {
            return CalculateTriangularMeshLinesCount(this.degree);
        }

        protected override void RecalculateSurfacePoints(TriangularBezierGeometryContext geometryContext)
        {
            this.devisions = geometryContext.SurfaceDevisions;
            this.RecalculateSurfacePoints();
        }

        protected override void RecalculateSurfacePoints()
        {
            Point3D[] surfacePoints = new Point3D[BezierTriangle.GetMeshPointsCount(this.devisions)];
            BezierTriangle bezierTriangle = this.GetCurrentControlTriange();
            int index = 0;

            BezierTriangle.IterateTriangleCoordinates(this.devisions, (u, v) =>
                {
                    surfacePoints[index++] = bezierTriangle.GetMeshPoint(u, v);
                });

            this.surfacePoints = new BezierTriangle(surfacePoints);
        }

        protected override void RecalculateControlLines()
        {
            if (this.visibleControlLines.Count == 0)
            {
                return;
            }

            BezierTriangle controlTriangle = this.GetCurrentControlTriange();
            int index = 0;

            controlTriangle.IterateTrianlges(true, (a, b, c) =>
                {
                    this.visibleControlLines[index++].MoveTo(a, b);
                    this.visibleControlLines[index++].MoveTo(b, c);
                    this.visibleControlLines[index++].MoveTo(a, c);
                });
        }

        protected override void RecalculateSurfaceLines()
        {
            if (this.visibleSurfaceLines.Count == 0)
            {
                return;
            }

            int index = 0;

            this.surfacePoints.IterateTrianlges(true, (a, b, c) =>
                {
                    this.visibleSurfaceLines[index++].MoveTo(a, b);
                    this.visibleSurfaceLines[index++].MoveTo(b, c);
                    this.visibleSurfaceLines[index++].MoveTo(a, c);
                });
        }

        protected override MeshGeometry3D CalculateSmoothSurfaceGeometry()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            int pointsCount = this.surfacePoints.MeshPointsCount;

            for (int i = 0; i < pointsCount; i++)
            {
                mesh.Positions.Add(this.surfacePoints[i]);
            }

            this.surfacePoints.IterateTriangleIndexes(false, (a, b, c) =>
                {
                    mesh.TriangleIndices.Add(a);
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(c);
                });

            return mesh;
        }

        protected override MeshGeometry3D CalculateSharpSurfaceGeometry()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            this.surfacePoints.IterateTrianlges(false, (a, b, c) =>
                {
                    mesh.Positions.Add(a);
                    mesh.Positions.Add(b);
                    mesh.Positions.Add(c);

                    mesh.TriangleIndices.Add(mesh.Positions.Count - 3);
                    mesh.TriangleIndices.Add(mesh.Positions.Count - 2);
                    mesh.TriangleIndices.Add(mesh.Positions.Count - 1);
                });

            return mesh;
        }

        private static int CalculateTriangularMeshLinesCount(int devisions)
        {
            return 3 * ((devisions * (devisions + 1)) / 2);
        }

        private BezierTriangle GetCurrentControlTriange()
        {
            Point3D[] positions = new Point3D[this.controlPoints.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = this.controlPoints[i].Position;
            }

            return new BezierTriangle(positions);
        }

        private void GenerateNewControlPointsGeometry(Point3D[] points, int degree, bool showControlPoints)
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.SphereType = SceneConstants.ControlPointsSphereType;
                this.SceneEditor.GraphicProperties.SubDevisions = SceneConstants.ControlPointsSubDevisions;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlPointsColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlPointsDiameter;

                int pointsCount = points.Length;
                this.controlPoints = new PointVisual[pointsCount];
                this.degree = degree;

                for (int i = 0; i < pointsCount; i++)
                {
                    PointVisual controlPoint;
                    if (this.controlPointsPool.TryPopElementFromPool(out controlPoint))
                    {
                        controlPoint.Position = points[i];
                    }
                    else
                    {
                        controlPoint = this.SceneEditor.AddPointVisual(points[i]);
                    }

                    this.controlPoints[i] = controlPoint;
                    this.RegisterVisiblePoint(controlPoint);
                }

                if (!showControlPoints)
                {
                    this.HideControlPoints();
                }
            }
        }

        private void DeleteOldControlPoints()
        {
            this.HideControlPoints();
            this.controlPoints = null;
            this.degree = 0;
        }
    }
}
