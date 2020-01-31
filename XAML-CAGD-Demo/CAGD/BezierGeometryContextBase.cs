namespace CAGD
{
    public abstract class BezierGeometryContextBase
    {
        public bool ShowControlPoints { get; set; }
        public bool ShowControlLines { get; set; }
        public bool ShowSurfaceLines { get; set; }
        public bool ShowSurfaceGeometry { get; set; }
        public bool ShowSmoothSurfaceGeometry { get; set; }
    }
}
