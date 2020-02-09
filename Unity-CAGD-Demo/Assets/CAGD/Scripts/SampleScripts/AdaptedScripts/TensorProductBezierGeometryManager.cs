using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD
{
    public class TensorProductBezierGeometryManager : BezierGeometryManagerBase<TensorProductBezierGeometryContext>
    {
        private PointVisual[,] controlPoints;
        private Vector3[,] controlPointValues;
        private Vector3[,] surfacePoints;

        public TensorProductBezierGeometryManager(BezierScene3D scene)
            : base(scene)
        {
            this.controlPointValues = null;
            this.controlPoints = null;
            this.surfacePoints = null;
        }

        public void GenerateGeometry(Vector3[,] controlPoints, TensorProductBezierGeometryContext geometryContext)
        {
            this.DeleteOldControlPoints();
            this.GenerateNewControlPointsGeometry(controlPoints, geometryContext.ShowControlPoints);

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

        protected override void RecalculateSurfacePoints(TensorProductBezierGeometryContext geometryContext)
        {
            this.RecalculateSurfacePoints(geometryContext.DevisionsInDirectionU, geometryContext.DevisionsInDirectionV);
        }

        protected override void RecalculateSurfacePoints()
        {
            this.RecalculateSurfacePoints(this.surfacePoints.GetLength(0) - 1, this.surfacePoints.GetLength(1) - 1);
        }

        protected override void RecalculateControlLines()
        {
            RecalculateLinesMesh(this.controlPoints.GetLength(0), this.controlPoints.GetLength(1), this.visibleControlLines, (i, j) => this.controlPoints[i, j].Position);
        }

        protected override int ControlPointsCount()
        {
            return this.controlPoints.GetLength(0) * this.controlPoints.GetLength(1);
        }

        protected override void RecalculateSurfaceLines()
        {
            RecalculateLinesMesh(this.surfacePoints.GetLength(0), this.surfacePoints.GetLength(1), this.visibleSurfaceLines, (i, j) => this.surfacePoints[i, j]);
        }

        protected override Mesh CalculateSmoothSurfaceGeometry()
        {
            Mesh meshGeometry = new Mesh();

            int uCount = this.surfacePoints.GetLength(0);
            int vCount = this.surfacePoints.GetLength(1);
            Vector3[] vertices = CalculateSmoothSurfaceVertices(uCount, vCount);
            int[] triangleIndices = CalculateSmoothSurfaceTriangleIndices(uCount, vCount);
            meshGeometry.vertices = vertices;
            meshGeometry.triangles = triangleIndices;
            meshGeometry.RecalculateNormals();

            return meshGeometry;
        }

        private static int[] CalculateSmoothSurfaceTriangleIndices(int uCount, int vCount)
        {
            Func<int, int, int> getTriangleIndex = (i, j) => i * vCount + j;

            int uLast = uCount - 1;
            int vLast = vCount - 1;
            int[] triangleIndices = new int[uLast * vLast * 6];
            int triangleVertexIndex = 0;

            for (int i = 0; i < uLast; i++)
            {
                for (int j = 0; j < vLast; j++)
                {
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i, j);
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i + 1, j);
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i + 1, j + 1);
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i, j);
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i + 1, j + 1);
                    triangleIndices[triangleVertexIndex++] = getTriangleIndex(i, j + 1);
                }
            }

            return triangleIndices;
        }

        private Vector3[] CalculateSmoothSurfaceVertices(int uCount, int vCount)
        {
            Vector3[] vertices = new Vector3[uCount * vCount];
            int vertexIndex = 0;

            for (int i = 0; i < uCount; i++)
            {
                for (int j = 0; j < vCount; j++)
                {
                    vertices[vertexIndex] = this.surfacePoints[i, j];
                    vertexIndex++;
                }
            }

            return vertices;
        }

        protected override Mesh CalculateSharpSurfaceGeometry()
        {
            Mesh meshGeometry = new Mesh();

            int uLast = this.surfacePoints.GetLength(0) - 1;
            int vLast = this.surfacePoints.GetLength(1) - 1;
            Vector3[] vertices = new Vector3[uLast * vLast * 4];
            int[] triangleIndices = new int[uLast * vLast * 6];
            int vertexIndex = 0;
            int triangleVertexIndex = 0;

            for (int i = 0; i < uLast; i++)
            {
                for (int j = 0; j < vLast; j++)
                {
                    vertices[vertexIndex++] = this.surfacePoints[i, j];
                    vertices[vertexIndex++] = this.surfacePoints[i + 1, j];
                    vertices[vertexIndex++] = this.surfacePoints[i + 1, j + 1];
                    vertices[vertexIndex++] = this.surfacePoints[i, j + 1];
                    int index = vertexIndex - 4;
                    triangleIndices[triangleVertexIndex++] = index;
                    triangleIndices[triangleVertexIndex++] = index + 1;
                    triangleIndices[triangleVertexIndex++] = index + 2;
                    triangleIndices[triangleVertexIndex++] = index;
                    triangleIndices[triangleVertexIndex++] = index + 2;
                    triangleIndices[triangleVertexIndex++] = index + 3;
                }
            }

            meshGeometry.vertices = vertices;
            meshGeometry.triangles = triangleIndices;
            meshGeometry.RecalculateNormals();

            return meshGeometry;
        }

        protected override int GetSurfaceLinesCount()
        {
            return GetLinesCountOnMesh(this.surfacePoints.GetLength(0), this.surfacePoints.GetLength(1));
        }

        protected override int GetControlLinesCount()
        {
            return GetLinesCountOnMesh(this.controlPoints.GetLength(0), this.controlPoints.GetLength(1));
        }

        private void DeleteOldControlPoints()
        {
            this.HideControlPoints();
            this.controlPoints = null;
            this.controlPointValues = null;
        }

        private void RecalculateSurfacePoints(int uDevisions, int vDevisions)
        {
            for (int i = 0; i < this.controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < this.controlPoints.GetLength(1); j++)
                {
                    this.controlPointValues[i, j] = this.controlPoints[i, j].Position;
                }
            }

            this.surfacePoints = new BezierRectangle(this.controlPointValues).GetMeshPoints(uDevisions, vDevisions);
        }

        private void GenerateNewControlPointsGeometry(Vector3[,] points, bool showControlPoints)
        {
            int uLength = points.GetLength(0);
            int vLength = points.GetLength(1);
            this.controlPointValues = points;
            this.controlPoints = new PointVisual[uLength, vLength];

            for (int i = 0; i < uLength; i++)
            {
                for (int j = 0; j < vLength; j++)
                {
                    PointVisual controlPoint;
                    if (!this.controlPointsPool.TryPopElementFromPool(out controlPoint))
                    {
                        controlPoint = this.scene.AddIteractivePoint();
                    }

                    controlPoint.Position = points[i, j];

                    this.controlPoints[i, j] = controlPoint;
                    this.RegisterVisiblePoint(controlPoint);
                }
            }

            if (!showControlPoints)
            {
                this.HideControlPoints();
            }
        }

        private static int GetLinesCountOnMesh(int uMeshPointsCount, int vMeshPointsCount)
        {
            return (uMeshPointsCount - 1) * vMeshPointsCount + (vMeshPointsCount - 1) * uMeshPointsCount;
        }

        private static void RecalculateLinesMesh(int uPointsCount, int vPointsCount, List<LineVisual> visibleLines, Func<int, int, Vector3> getPosition)
        {
            if (visibleLines.Count == 0)
            {
                return;
            }

            int lastU = uPointsCount - 1;
            int lastV = vPointsCount - 1;
            int lineIndex = 0;

            for (int i = 0; i < lastU; i++)
            {
                for (int j = 0; j < lastV; j++)
                {
                    visibleLines[lineIndex++].MoveTo(getPosition(i, j), getPosition(i + 1, j));
                    visibleLines[lineIndex++].MoveTo(getPosition(i, j), getPosition(i, j + 1));
                }
            }

            for (int i = 0; i < lastU; i++)
            {
                visibleLines[lineIndex++].MoveTo(getPosition(i, lastV), getPosition(i + 1, lastV));
            }

            for (int j = 0; j < lastV; j++)
            {
                visibleLines[lineIndex++].MoveTo(getPosition(lastU, j), getPosition(lastU, j + 1));
            }
        }
    }
}
