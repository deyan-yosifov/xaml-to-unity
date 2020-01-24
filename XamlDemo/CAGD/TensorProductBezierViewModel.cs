using CAGD.Controls.Controls3D;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TensorProductBezierViewModel : BezierViewModelBase<TensorProductBezierGeometryContext, TensorProductBezierGeometryManager>
    {
        private int degreeInDirectionU = 3;
        private int degreeInDirectionV = 4;
        private int devisionsInDirectionU = 10;
        private int devisionsInDirectionV = 10;

        public TensorProductBezierViewModel(Scene3D scene)
            : base(scene)
        {
        }

        public int DegreeInDirectionU
        {
            get
            {
                return this.degreeInDirectionU;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionU, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        public int DegreeInDirectionV
        {
            get
            {
                return this.degreeInDirectionV;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionV, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        public int DevisionsInDirectionU
        {
            get
            {
                return this.devisionsInDirectionU;
            }
            set
            {
                if (this.SetProperty(ref this.devisionsInDirectionU, value))
                {
                    this.RecalculateSurfaceGeometry();
                }
            }
        }

        public int DevisionsInDirectionV
        {
            get
            {
                return this.devisionsInDirectionV;
            }
            set
            {
                if (this.SetProperty(ref this.devisionsInDirectionV, value))
                {
                    this.RecalculateSurfaceGeometry();
                }
            }
        }

        protected override void RecalculateControlPointsGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.GeometryContext);
        }

        protected override TensorProductBezierGeometryContext CreateGeometryContext()
        {
            return new TensorProductBezierGeometryContext()
            {
                DevisionsInDirectionU = this.DevisionsInDirectionU,
                DevisionsInDirectionV = this.DevisionsInDirectionV,
                ShowControlLines = this.ShowControlLines,
                ShowControlPoints = this.ShowControlPoints,
                ShowSurfaceGeometry = this.ShowSurfaceGeometry,
                ShowSurfaceLines = this.ShowSurfaceLines,
                ShowSmoothSurfaceGeometry = this.ShowSmoothSurfaceGeometry
            };
        }

        protected override TensorProductBezierGeometryManager CreateGeometryManager(Scene3D scene)
        {
            return new TensorProductBezierGeometryManager(scene);
        }

        private Point3D[,] CalculateControlPoints()
        {
            int uPoints = this.DegreeInDirectionU + 1;
            int vPoints = this.DegreeInDirectionV + 1;
            Point3D[,] points = new Point3D[uPoints, vPoints];
            double squareSize = SceneConstants.InitialSurfaceBoundingSquareSide;
            double startX = -squareSize / 2;
            double deltaX = squareSize / this.DegreeInDirectionU;
            double startY = -squareSize / 2;
            double deltaY = squareSize / this.DegreeInDirectionV;

            for (int u = 0; u < uPoints; u++)
            {
                for (int v = 0; v < vPoints; v++)
                {
                    double x = startX + u * deltaX;
                    double y = startY + v * deltaY;

                    points[u, v] = new Point3D(x, y, 0);
                }
            }

            return points;
        }
    }
}
