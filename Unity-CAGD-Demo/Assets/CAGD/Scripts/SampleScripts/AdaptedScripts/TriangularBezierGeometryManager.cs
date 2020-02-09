using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD
{
    public class TriangularBezierGeometryManager : BezierGeometryManagerBase<TriangularBezierGeometryContext>
    {
        private int degree;
        private int devisions;
        private BezierTriangle surfacePoints;
        private PointVisual[] controlPoints;

        public TriangularBezierGeometryManager(BezierScene3D scene)
            : base(scene)
        {
            this.degree = 0;
            this.devisions = 0;
            this.surfacePoints = null;
            this.controlPoints = null;
        }

        public void GenerateGeometry(Vector3[] controlPoints, int degree, TriangularBezierGeometryContext geometryContext)
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

        protected override IEnumerable<Vector3> GetControlPoints()
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
            Vector3[] surfacePoints = new Vector3[BezierTriangle.GetMeshPointsCount(this.devisions)];
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

        protected override Mesh CalculateSmoothSurfaceGeometry()
        {
            Mesh mesh = new Mesh();
            int pointsCount = this.surfacePoints.MeshPointsCount;
            Vector3[] vertices = new Vector3[pointsCount];

            for (int i = 0; i < pointsCount; i++)
            {
                vertices[i] = this.surfacePoints[i];
            }

            mesh.vertices = vertices;

            int[] triangleIndices = new int[this.surfacePoints.TrianglesCount * 3];
            int triangleVertexIndex = 0;

            this.surfacePoints.IterateTriangleIndexes(false, (a, b, c) =>
            {
                triangleIndices[triangleVertexIndex++] = a;
                triangleIndices[triangleVertexIndex++] = b;
                triangleIndices[triangleVertexIndex++] = c;
            });

            mesh.triangles = triangleIndices;
            mesh.RecalculateNormals();

            return mesh;
        }

        protected override Mesh CalculateSharpSurfaceGeometry()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[this.surfacePoints.TrianglesCount * 3];
            int[] triangleIndices = new int[this.surfacePoints.TrianglesCount * 3];
            int vertexIndex = 0;
            int triangleVertexIndex = 0;

            this.surfacePoints.IterateTrianlges(false, (a, b, c) =>
            {
                vertices[vertexIndex++] = a;
                vertices[vertexIndex++] = b;
                vertices[vertexIndex++] = c;
                triangleIndices[triangleVertexIndex++] = vertexIndex - 3;
                triangleIndices[triangleVertexIndex++] = vertexIndex - 2;
                triangleIndices[triangleVertexIndex++] = vertexIndex - 1;
            });

            mesh.vertices = vertices;
            mesh.triangles = triangleIndices;

            return mesh;
        }

        private static int CalculateTriangularMeshLinesCount(int devisions)
        {
            return 3 * ((devisions * (devisions + 1)) / 2);
        }

        private BezierTriangle GetCurrentControlTriange()
        {
            Vector3[] positions = new Vector3[this.controlPoints.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = this.controlPoints[i].Position;
            }

            return new BezierTriangle(positions);
        }

        private void GenerateNewControlPointsGeometry(Vector3[] points, int degree, bool showControlPoints)
        {
            int pointsCount = points.Length;
            this.controlPoints = new PointVisual[pointsCount];
            this.degree = degree;

            for (int i = 0; i < pointsCount; i++)
            {
                PointVisual controlPoint;
                if (!this.controlPointsPool.TryPopElementFromPool(out controlPoint))
                {
                    controlPoint = this.scene.AddIteractivePoint();
                }

                controlPoint.Position = points[i];
                this.controlPoints[i] = controlPoint;
                this.RegisterVisiblePoint(controlPoint);
            }

            if (!showControlPoints)
            {
                this.HideControlPoints();
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
