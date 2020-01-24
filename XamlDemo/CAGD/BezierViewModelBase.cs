using CAGD.Controls.Common;
using CAGD.Controls.Controls3D;
using CAGD.Controls.Controls3D.Cameras;
using CAGD.Controls.Controls3D.Iteractions;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public abstract class BezierViewModelBase<T, U> :  ViewModelBase
        where T : BezierGeometryContextBase
        where U : BezierGeometryManagerBase<T>
    {
        private readonly Scene3D scene;
        protected readonly U geometryManager;
        private readonly IteractivePointsHandler iteractivePointsHandler;
        private bool showControlPoints;
        private bool showControlLines;
        private bool showSurfaceLines;
        private bool showSurfaceGeometry;
        private bool showSmoothSurfaceGeometry;

        protected BezierViewModelBase(Scene3D scene)
        {
            this.scene = scene;
            this.iteractivePointsHandler = this.scene.IteractivePointsHandler;
            this.geometryManager = this.CreateGeometryManager(scene);
            this.scene.PointerHandlersController.Handlers.AddFirst(new ZoomToContentsHandler(scene.Editor, this.geometryManager));
            this.showControlPoints = true;
            this.showControlLines = true;
            this.showSurfaceLines = true;
            this.showSurfaceGeometry = true;
            this.showSmoothSurfaceGeometry = false;

            this.scene.StartListeningToMouseEvents();
            this.InitializeScene();
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
                return this.iteractivePointsHandler.CanMoveOnXAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnXAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnXAxis = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CanMoveOnYAxis
        {
            get
            {
                return this.iteractivePointsHandler.CanMoveOnYAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnYAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnYAxis = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CanMoveOnZAxis
        {
            get
            {
                return this.iteractivePointsHandler.CanMoveOnZAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnZAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnZAxis = value;
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

        private SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        protected abstract T CreateGeometryContext();

        protected abstract U CreateGeometryManager(Scene3D scene);

        protected abstract void RecalculateControlPointsGeometry();

        protected void RecalculateSurfaceGeometry()
        {
            this.geometryManager.GenerateGeometry(this.GeometryContext);
        }

        private void InitializeScene()
        {
            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.SceneEditor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.SceneEditor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            this.SceneEditor.Look(new Point3D(25, 25, 35), new Point3D());
            
            this.RecalculateControlPointsGeometry();
        }
    }
}
