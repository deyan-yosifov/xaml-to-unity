using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class Mesh : ShapeBase
    {
        public Geometry3D Geometry
        {
            get
            {
                return this.GeometryModel.Geometry;
            }
            set
            {
                this.GeometryModel.Geometry = value;
            }
        }
    }
}
