using CAGD.Utilities;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public class Position3D : ICloneable<Position3D>
    {
        private Matrix3D matrix;

        public Position3D()
            : this(Matrix3D.Identity)
        {
        }

        public Position3D(Matrix3D matrix)
        {
            this.matrix = matrix;
        }

        public Matrix3D Matrix
        {
            get
            {
                return this.matrix;
            }
        }

        public void Translate(Vector3D offset)
        {
            this.matrix.Translate(offset);
        }

        public void Rotate(Quaternion quaternion)
        {
            this.matrix.Rotate(quaternion);
        }

        public void RotateAt(Quaternion quaternion, Point3D center)
        {
            this.matrix.RotateAt(quaternion, center);
        }

        public void Scale(Vector3D scale)
        {
            this.matrix.Scale(scale);
        }

        public void ScaleAt(Vector3D scale, Point3D center)
        {
            this.matrix.ScaleAt(scale, center);
        }

        public void Append(Matrix3D matrix)
        {
            matrix.Append(matrix);
        }

        public Position3D Clone()
        {
            return new Position3D(this.Matrix);
        }
    }
}
