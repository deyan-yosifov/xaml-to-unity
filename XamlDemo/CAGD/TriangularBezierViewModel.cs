using CAGD.Controls.Controls3D;
using System;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TriangularBezierViewModel : BezierViewModelBase<TriangularBezierGeometryContext, TriangularBezierGeometryManager>
    {
        private readonly BezierTriangle initialBezierTriangle = CalculateInitialBezierTriangle();
        private int surfaceDegree = 4;
        private int surfaceDevisions = 10;

        public TriangularBezierViewModel(Scene3D scene)
            : base(scene)
        {            
        }

        public int SurfaceDegree
        {
            get
            {
                return this.surfaceDegree;
            }
            set
            {
                if (this.SetProperty(ref this.surfaceDegree, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        public int SurfaceDevisions
        {
            get
            {
                return this.surfaceDevisions;
            }
            set
            {
                if (this.SetProperty(ref this.surfaceDevisions, value))
                {
                    this.RecalculateSurfaceGeometry();
                }
            }
        }

        protected override TriangularBezierGeometryContext CreateGeometryContext()
        {
            return new TriangularBezierGeometryContext()
            {
                SurfaceDevisions = this.SurfaceDevisions,
                ShowControlLines = this.ShowControlLines,
                ShowControlPoints = this.ShowControlPoints,
                ShowSurfaceGeometry = this.ShowSurfaceGeometry,
                ShowSurfaceLines = this.ShowSurfaceLines,
                ShowSmoothSurfaceGeometry = this.ShowSmoothSurfaceGeometry
            };
        }

        protected override TriangularBezierGeometryManager CreateGeometryManager(Scene3D scene)
        {
            return new TriangularBezierGeometryManager(scene);
        }

        protected override void RecalculateControlPointsGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.SurfaceDegree, this.CreateGeometryContext());
        }

        private static BezierTriangle CalculateInitialBezierTriangle()
        {
            double triangleSide = SceneConstants.InitialSurfaceBoundingTriangleSide;
            Matrix3D matrix = new Matrix3D();
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), 120));
            Point3D c = new Point3D(0, triangleSide / Math.Sqrt(3), 0);
            Point3D a = matrix.Transform(c);
            Point3D b = matrix.Transform(a);

            return new BezierTriangle(new Point3D[] { a, b, c });
        }

        private Point3D[] CalculateControlPoints()
        {
            int index = 0;
            Point3D[] points = new Point3D[BezierTriangle.GetMeshPointsCount(this.SurfaceDegree)];  

            BezierTriangle.IterateTriangleCoordinates(this.SurfaceDegree, (u, v) =>
                {
                    points[index++] = this.initialBezierTriangle.GetMeshPoint(u, v);
                });

            return points;
        }
    }
}
