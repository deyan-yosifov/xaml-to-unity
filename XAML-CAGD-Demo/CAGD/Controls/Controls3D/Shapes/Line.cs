using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class Line : Cylinder
    {
        internal static readonly Vector3D InitialVector = new Vector3D(0, 0, 1);

        public Line(int sidesCount)
            : base(sidesCount, true, true)
        {
        }
    }
}
