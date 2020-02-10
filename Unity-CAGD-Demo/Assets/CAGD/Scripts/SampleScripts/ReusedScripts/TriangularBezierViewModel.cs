using CAGD.Controls.Controls3D;
using UnityEngine;

namespace CAGD
{
    public class TriangularBezierViewModel : BezierViewModelBase<TriangularBezierGeometryContext, TriangularBezierGeometryManager>
    {
        [SerializeField]
        private float initialSurfaceTriangleSide = 1.2f;
        [SerializeField]
        private float initialCenterOffset = -1.4f;
        private BezierTriangle initialBezierTriangle;
        private int surfaceDegree = 4;
        private int surfaceDevisions = 10;

        protected override void Start()
        {
            this.initialBezierTriangle = CalculateInitialBezierTriangle();
            base.Start();
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

        protected override TriangularBezierGeometryManager CreateGeometryManager(BezierScene3D scene)
        {
            return new TriangularBezierGeometryManager(scene);
        }

        protected override void RecalculateControlPointsGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.SurfaceDegree, this.CreateGeometryContext());
        }

        private BezierTriangle CalculateInitialBezierTriangle()
        {
            float triangleSide = this.initialSurfaceTriangleSide;
            Vector3 c = new Vector3(0, 0, triangleSide / Mathf.Sqrt(3));
            Quaternion rotation = Quaternion.AngleAxis(120, Vector3.up);
            Vector3 a = rotation * c;
            Vector3 b = rotation * a;
            Vector3 offset = new Vector3(this.initialCenterOffset, 0, 0);
            a += offset;
            b += offset;
            c += offset;

            return new BezierTriangle(new Vector3[] { a, b, c });
        }

        private Vector3[] CalculateControlPoints()
        {
            int index = 0;
            Vector3[] points = new Vector3[BezierTriangle.GetMeshPointsCount(this.SurfaceDegree)];

            BezierTriangle.IterateTriangleCoordinates(this.SurfaceDegree, (u, v) =>
            {
                points[index++] = this.initialBezierTriangle.GetMeshPoint(u, v);
            });

            return points;
        }
    }
}
