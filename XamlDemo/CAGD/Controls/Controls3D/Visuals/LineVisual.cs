using CAGD.Utilities;
using CAGD.Controls.Controls3D.Shapes;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class LineVisual : IVisual3DOwner
    {
        private Point3D start;
        private Point3D end;
        private double thickness;
        private readonly ModelVisual3D visual;
        private readonly MatrixTransform3D positionTransform;
        private readonly ScaleTransform3D thicknessTransform;

        public LineVisual(Line line, double thickness)
        {
            this.thickness = thickness;

            this.thicknessTransform = new ScaleTransform3D() { ScaleX = thickness, ScaleY = thickness };
            this.positionTransform = new MatrixTransform3D();
            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(this.thicknessTransform);
            transformGroup.Children.Add(this.positionTransform);

            this.visual = new ModelVisual3D() 
            {
                Content = line.GeometryModel ,
                Transform = transformGroup
            };
        }

        public Visual3D Visual
        {
            get
            {
                return this.visual;
            }
        }

        public Point3D Start
        {
            get
            {
                return this.start;
            }
        }

        public Point3D End
        {
            get
            {
                return this.end;
            }
        }

        public double Thickness
        {
            get
            {
                return this.thickness;
            }
            set
            {
                if (this.thickness != value)
                {
                    this.thickness = value;
                    this.thicknessTransform.ScaleX = value;
                    this.thicknessTransform.ScaleY = value;
                }
            }
        }

        private Vector3D Direction
        {
            get
            {
                Vector3D direction = this.End - this.Start;
                direction.Normalize();

                return direction;
            }
        }

        public void MoveTo(Point3D start, Point3D end)
        {
            this.start = start;
            this.end = end;

            this.CalculatePositionTransform();
        }

        private void CalculatePositionTransform()
        {
            Matrix3D matrix = new Matrix3D();
            Vector3D direction = this.end - this.start;

            matrix.Scale(new Vector3D(1, 1, direction.Length));

            Vector3D rotationAxis = new Vector3D(-direction.Y, +direction.X, 0);
            if (!rotationAxis.LengthSquared.IsZero())
            {
                rotationAxis.Normalize();
                double angle = Vector3D.AngleBetween(Line.InitialVector, direction);
                matrix.Rotate(new Quaternion(rotationAxis, angle));
            }
            else if (direction.Z < 0)
            {
                matrix.Scale(new Vector3D(1, 1, -1));
            }

            matrix.Translate(new Vector3D(this.start.X, this.start.Y, this.start.Z));

            this.positionTransform.Matrix = matrix;
        }

        public override string ToString()
        {
            return string.Format("LineSegment: ({0})->({1})=({2})", this.Start, this.End, this.Direction);
        }
    }
}
