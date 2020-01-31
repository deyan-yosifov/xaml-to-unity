using CAGD.Utilities;
using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Shapes;
using CAGD.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public abstract class BezierGeometryManagerBase<T> : IContentProvider
        where T : BezierGeometryContextBase
    {
        private readonly Scene3D scene;
        protected readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Visual3DPool<LineVisual> controlLinesPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual3DPool<VisualOwner> surfaceGeometryPool;
        private readonly Queue<PointVisual> visibleControlPoints;
        protected readonly List<LineVisual> visibleControlLines;
        protected readonly List<LineVisual> visibleSurfaceLines;
        private VisualOwner visibleSurfaceGeometry;
        private Mesh surfaceGeometry;
        private bool isSmoothSurface;

        protected BezierGeometryManagerBase(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(scene);
            this.controlLinesPool = new Visual3DPool<LineVisual>(scene);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(scene);
            this.surfaceGeometryPool = new Visual3DPool<VisualOwner>(scene);
            this.visibleControlPoints = new Queue<PointVisual>();
            this.visibleControlLines = new List<LineVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();

            this.visibleSurfaceGeometry = null;
            this.surfaceGeometry = null;
        }

        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
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
            while(this.visibleControlPoints.Count > 0)
            {
                PointVisual point = this.visibleControlPoints.Dequeue();
                this.controlPointsPool.PushElementToPool(point);
                this.DetachFromPointEvents(point);
            }
        }

        public void ShowControlLines()
        {
            int count = this.GetControlLinesCount();

            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.ControlLinesArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlLinesColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlLinesDiameter;

                this.EnsureVisibleLinesCount(this.visibleControlLines, this.controlLinesPool, count);
            }

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

            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.SurfaceLinesArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceLinesColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.SurfaceLinesDiameter;

                this.EnsureVisibleLinesCount(this.visibleSurfaceLines, this.surfaceLinesPool, count);
            }

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
            if (this.surfaceGeometry == null)
            {
                using (this.SceneEditor.SaveGraphicProperties())
                {
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceGeometryColor);
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(SceneConstants.SurfaceGeometryColor);
                    this.surfaceGeometry = this.SceneEditor.ShapeFactory.CreateMesh();
                    this.visibleSurfaceGeometry = this.SceneEditor.AddShapeVisual(this.surfaceGeometry);
                }
            }

            if (this.visibleSurfaceGeometry == null)
            {
                this.visibleSurfaceGeometry = this.surfaceGeometryPool.PopElementFromPool();
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

        public IEnumerable<Point3D> GetContentPoints()
        {
            return this.GetControlPoints();
        }

        protected void RegisterVisiblePoint(PointVisual point)
        {
            this.visibleControlPoints.Enqueue(point);
            this.AttachToPointEvents(point);
        }

        protected abstract IEnumerable<Point3D> GetControlPoints();

        protected abstract int ControlPointsCount();

        protected abstract int GetSurfaceLinesCount();

        protected abstract int GetControlLinesCount();

        protected abstract void RecalculateSurfacePoints(T geometryContext);

        protected abstract void RecalculateSurfacePoints();

        protected abstract void RecalculateControlLines();

        protected abstract void RecalculateSurfaceLines();

        protected abstract MeshGeometry3D CalculateSmoothSurfaceGeometry();

        protected abstract MeshGeometry3D CalculateSharpSurfaceGeometry();

        private void EnsureVisibleLinesCount(List<LineVisual> visibleLines, Visual3DPool<LineVisual> linesPool, int visibleCount)
        {
            while (visibleLines.Count < visibleCount)
            {
                LineVisual line;
                if (!linesPool.TryPopElementFromPool(out line))
                {
                    line = this.SceneEditor.AddLineVisual(new Point3D(), new Point3D());
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
                this.surfaceGeometry.Geometry = this.CalculateSmoothSurfaceGeometry();
            }
            else
            {
                this.surfaceGeometry.Geometry = this.CalculateSharpSurfaceGeometry();
            }
        }

        private void AttachToPointEvents(PointVisual point)
        {
            point.PositionChanged += PointPositionChanged;
            this.scene.IteractivePointsHandler.RegisterIteractivePoint(point);
        }

        private void DetachFromPointEvents(PointVisual point)
        {
            point.PositionChanged -= PointPositionChanged;
            this.scene.IteractivePointsHandler.UnRegisterIteractivePoint(point);
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
