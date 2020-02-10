using CAGD.Controls.Common;
using CAGD.Controls.Controls3D;
using UnityEngine;

namespace CAGD
{
    public abstract class BezierViewModelBase<T, U> : ViewModelBase
        where T : BezierGeometryContextBase
        where U : BezierGeometryManagerBase<T>
    {
        protected U geometryManager;
        [SerializeField]
        private BezierScene3D scene;
        private bool showControlPoints = true;
        private bool showControlLines = true;
        private bool showSurfaceLines = true;
        private bool showSurfaceGeometry = true;
        private bool showSmoothSurfaceGeometry = false;

        protected virtual void Start()
        {
            this.geometryManager = this.CreateGeometryManager(scene);
            this.RecalculateControlPointsGeometry();
        }

        public bool ShowControlPoints
        {
            get
            {
                return this.showControlPoints;
            }
            set
            {
                if (this.SetProperty(ref this.showControlPoints, value))
                {
                    if (value)
                    {
                        this.geometryManager.ShowControlPoints();
                    }
                    else
                    {
                        this.geometryManager.HideControlPoints();
                    }
                }
            }
        }

        public bool ShowControlLines
        {
            get
            {
                return this.showControlLines;
            }
            set
            {
                if (this.SetProperty(ref this.showControlLines, value))
                {
                    if (value)
                    {
                        this.geometryManager.ShowControlLines();
                    }
                    else
                    {
                        this.geometryManager.HideControlLines();
                    }
                }
            }
        }

        public bool ShowSurfaceLines
        {
            get
            {
                return this.showSurfaceLines;
            }
            set
            {
                if (this.SetProperty(ref this.showSurfaceLines, value))
                {
                    if (value)
                    {
                        this.geometryManager.ShowSurfaceLines(this.GeometryContext);
                    }
                    else
                    {
                        this.geometryManager.HideSurfaceLines();
                    }
                }
            }
        }

        public bool ShowSurfaceGeometry
        {
            get
            {
                return this.showSurfaceGeometry;
            }
            set
            {
                if (this.SetProperty(ref this.showSurfaceGeometry, value))
                {
                    if (value)
                    {
                        this.geometryManager.ShowSurfaceGeometry(this.GeometryContext);
                    }
                    else
                    {
                        this.geometryManager.HideSurfaceGeometry();
                    }
                }
            }
        }

        public bool ShowSmoothSurfaceGeometry
        {
            get
            {
                return this.showSmoothSurfaceGeometry;
            }
            set
            {
                if (this.SetProperty(ref this.showSmoothSurfaceGeometry, value))
                {
                    this.geometryManager.ChangeSurfaceSmoothness(value);
                }
            }
        }

        public bool CanMoveOnXAxis
        {
            get
            {
                return this.scene.iteractivePointsHandler.CanMoveOnXAxis;
            }
            set
            {
                if (this.scene.iteractivePointsHandler.CanMoveOnXAxis != value)
                {
                    this.scene.iteractivePointsHandler.CanMoveOnXAxis = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CanMoveOnYAxis
        {
            get
            {
                return this.scene.iteractivePointsHandler.CanMoveOnYAxis;
            }
            set
            {
                if (this.scene.iteractivePointsHandler.CanMoveOnYAxis != value)
                {
                    this.scene.iteractivePointsHandler.CanMoveOnYAxis = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CanMoveOnZAxis
        {
            get
            {
                return this.scene.iteractivePointsHandler.CanMoveOnZAxis;
            }
            set
            {
                if (this.scene.iteractivePointsHandler.CanMoveOnZAxis != value)
                {
                    this.scene.iteractivePointsHandler.CanMoveOnZAxis = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public T GeometryContext
        {
            get
            {
                return this.CreateGeometryContext();
            }
        }

        protected abstract T CreateGeometryContext();

        protected abstract U CreateGeometryManager(BezierScene3D scene);

        protected abstract void RecalculateControlPointsGeometry();

        protected void RecalculateSurfaceGeometry()
        {
            this.geometryManager.GenerateGeometry(this.GeometryContext);
        }
    }
}
