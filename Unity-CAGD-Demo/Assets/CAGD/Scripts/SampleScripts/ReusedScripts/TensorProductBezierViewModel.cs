using CAGD.Controls.Controls3D;
using UnityEngine;

namespace CAGD
{
    public class TensorProductBezierViewModel : BezierViewModelBase<TensorProductBezierGeometryContext, TensorProductBezierGeometryManager>
    {
        [SerializeField]
        private float initialSurfaceSquareSide = 1;
        [SerializeField]
        private Vector3 initialCenterOffset = new Vector3(-0.7f, 1, 0);
        private int degreeInDirectionU = 3;
        private int degreeInDirectionV = 4;
        private int devisionsInDirectionU = 10;
        private int devisionsInDirectionV = 10;

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

        protected override TensorProductBezierGeometryManager CreateGeometryManager(BezierScene3D scene)
        {
            return new TensorProductBezierGeometryManager(scene);
        }

        private Vector3[,] CalculateControlPoints()
        {
            int uPoints = this.DegreeInDirectionU + 1;
            int vPoints = this.DegreeInDirectionV + 1;
            Vector3[,] points = new Vector3[uPoints, vPoints];
            float squareSize = initialSurfaceSquareSide;
            float startX = -squareSize / 2;
            float deltaX = squareSize / this.DegreeInDirectionU;
            float startY = -squareSize / 2;
            float deltaY = squareSize / this.DegreeInDirectionV;

            for (int u = 0; u < uPoints; u++)
            {
                for (int v = 0; v < vPoints; v++)
                {
                    float x = startX + u * deltaX;
                    float y = startY + v * deltaY;

                    points[u, v] = new Vector3(x, 0, y) + this.initialCenterOffset;
                }
            }

            return points;
        }
    }
}
