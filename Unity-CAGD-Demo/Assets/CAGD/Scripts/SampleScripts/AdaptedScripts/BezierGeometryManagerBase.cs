using CAGD.Utilities;
using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD
{
    public abstract class BezierGeometryManagerBase<T> : IContentProvider
        where T : BezierGeometryContextBase
    {
        protected readonly BezierScene3D scene;
        protected readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Visual3DPool<LineVisual> controlLinesPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual3DPool<SurfaceVisual> surfaceGeometryPool;
        private readonly Queue<PointVisual> visibleControlPoints;
        protected readonly List<LineVisual> visibleControlLines;
        protected readonly List<LineVisual> visibleSurfaceLines;
        private SurfaceVisual visibleSurfaceGeometry;
        private bool isSmoothSurface;

        protected BezierGeometryManagerBase(BezierScene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>();
            this.controlLinesPool = new Visual3DPool<LineVisual>();
            this.surfaceLinesPool = new Visual3DPool<LineVisual>();
            this.surfaceGeometryPool = new Visual3DPool<SurfaceVisual>();
            this.visibleControlPoints = new Queue<PointVisual>();
            this.visibleControlLines = new List<LineVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();

            this.visibleSurfaceGeometry = null;
        }

        public void GenerateGeometry(T geometryContext)
        {
            this.RecalculateSurfacePoints(geometryContext);

            if (geometryContext.ShowSurfaceLines)
            {
                this.ShowSurfaceLines(geometryContext);
            }
            else
            {
                this.HideSurfaceLines();
            }

            if (geometryContext.ShowSurfaceGeometry)
            {
                this.ShowSurfaceGeometry(geometryContext);
            }
            else
            {
                this.HideSurfaceGeometry();
            }
        }

        public void ShowControlPoints()
        {
            int count = this.ControlPointsCount();

            while (this.visibleControlPoints.Count < count)
            {
                PointVisual point = this.controlPointsPool.PopElementFromPool();
                this.RegisterVisiblePoint(point);
            }
        }

        public void HideControlPoints()
        {
            while (this.visibleControlPoints.Count > 0)
            {
                PointVisual point = this.visibleControlPoints.Dequeue();
                this.controlPointsPool.PushElementToPool(point);
                this.DetachFromPointEvents(point);
            }
        }

        public void ShowControlLines()
        {
            int count = this.GetControlLinesCount();
            this.EnsureVisibleLinesCount(this.visibleControlLines, this.controlLinesPool, BezierScene3D.LineType.ControlLine, count);
            this.RecalculateControlLines();
        }

        public void HideControlLines()
        {
            while (this.visibleControlLines.Count > 0)
            {
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.PopLast());
            }
        }

        public void ShowSurfaceLines(T geometryContext)
        {
            int count = this.GetSurfaceLinesCount();
            this.EnsureVisibleLinesCount(this.visibleSurfaceLines, this.surfaceLinesPool, BezierScene3D.LineType.SurfaceLine, count);
            this.RecalculateSurfaceLines();
        }

        public void HideSurfaceLines()
        {
            while (this.visibleSurfaceLines.Count > 0)
            {
                this.surfaceLinesPool.PushElementToPool(this.visibleSurfaceLines.PopLast());
            }
        }

        public void ShowSurfaceGeometry(T geometryContext)
        {
            if (this.visibleSurfaceGeometry == null && !this.surfaceGeometryPool.TryPopElementFromPool(out this.visibleSurfaceGeometry))
            {
                this.visibleSurfaceGeometry = this.scene.AddSurface();
            }

            this.isSmoothSurface = geometryContext.ShowSmoothSurfaceGeometry;
            this.RecalculateSurfaceGeometry();
        }

        public void HideSurfaceGeometry()
        {
            if (this.visibleSurfaceGeometry != null)
            {
                this.surfaceGeometryPool.PushElementToPool(this.visibleSurfaceGeometry);
                this.visibleSurfaceGeometry = null;
            }
        }

        public void ChangeSurfaceSmoothness(bool isSmooth)
        {
            this.isSmoothSurface = isSmooth;
            this.RecalculateSurfaceGeometry();
        }

        public IEnumerable<Vector3> GetContentPoints()
        {
            return this.GetControlPoints();
        }

        protected void RegisterVisiblePoint(PointVisual point)
        {
            this.visibleControlPoints.Enqueue(point);
            this.AttachToPointEvents(point);
        }

        protected abstract IEnumerable<Vector3> GetControlPoints();

        protected abstract int ControlPointsCount();

        protected abstract int GetSurfaceLinesCount();

        protected abstract int GetControlLinesCount();

        protected abstract void RecalculateSurfacePoints(T geometryContext);

        protected abstract void RecalculateSurfacePoints();

        protected abstract void RecalculateControlLines();

        protected abstract void RecalculateSurfaceLines();

        protected abstract Mesh CalculateSmoothSurfaceGeometry();

        protected abstract Mesh CalculateSharpSurfaceGeometry();

        private void EnsureVisibleLinesCount(List<LineVisual> visibleLines, Visual3DPool<LineVisual> linesPool, BezierScene3D.LineType lineType, int visibleCount)
        {
            while (visibleLines.Count < visibleCount)
            {
                LineVisual line;
                if (!linesPool.TryPopElementFromPool(out line))
                {
                    line = this.scene.AddLine(lineType);
                }

                visibleLines.Add(line);
            }

            while (visibleLines.Count > visibleCount)
            {
                linesPool.PushElementToPool(visibleLines.PopLast());
            }
        }

        private void DeleteControlLines()
        {
            while (this.visibleControlLines.Count > 0)
            {
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.PopLast());
            }
        }

        private void DeleteSurfaceLines()
        {
            while (this.visibleSurfaceLines.Count > 0)
            {
                this.surfaceLinesPool.PushElementToPool(this.visibleSurfaceLines.PopLast());
            }
        }

        private void RecalculateSurfaceGeometry()
        {
            if (this.visibleSurfaceGeometry == null)
            {
                return;
            }

            if (this.isSmoothSurface)
            {
                this.visibleSurfaceGeometry.Mesh = this.CalculateSmoothSurfaceGeometry();
            }
            else
            {
                this.visibleSurfaceGeometry.Mesh = this.CalculateSharpSurfaceGeometry();
            }
        }

        private void AttachToPointEvents(PointVisual point)
        {
            point.PositionChanged += this.PointPositionChanged;
        }

        private void DetachFromPointEvents(PointVisual point)
        {
            point.PositionChanged -= this.PointPositionChanged;
        }

        private void PointPositionChanged(object sender, EventArgs e)
        {
            this.RecalculateControlLines();
            this.RecalculateSurfacePoints();

            if (this.visibleSurfaceLines.Count > 0 || this.visibleSurfaceGeometry != null)
            {
                this.RecalculateSurfaceLines();
                this.RecalculateSurfaceGeometry();
            }
        }
    }
}
