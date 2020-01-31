using CAGD.Controls.Controls3D.Shapes;
using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class PointVisual : IVisual3DOwner
    {
        private Point3D position;
        private double diameter;
        private readonly ModelVisual3D visual;
        private readonly MatrixTransform3D positionAndScaleTransform;

        public PointVisual(ShapeBase unitPointShape, double diameter)
        {
            this.diameter = 1;
            this.position = new Point3D();
            MatrixTransform3D initialTransform = new MatrixTransform3D(this.InitialUnitPointShapeTransformation);
            this.positionAndScaleTransform = new MatrixTransform3D();
            this.Diameter = diameter;

            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(initialTransform);
            transformGroup.Children.Add(this.positionAndScaleTransform);

            this.visual = new ModelVisual3D()
            {
                Content = unitPointShape.GeometryModel,
                Transform = transformGroup
            };
        }

        protected virtual Matrix3D InitialUnitPointShapeTransformation
        {
            get
            {
                return Matrix3D.Identity;
            }
        }

        public double Diameter
        {
            get
            {
                return this.diameter;
            }
            set
            {
                if (this.diameter != value)
                {
                    this.diameter = value;

                    Matrix3D matrix = this.positionAndScaleTransform.Matrix;
                    matrix.M11 = value;
                    matrix.M22 = value;
                    matrix.M33 = value;
                    this.positionAndScaleTransform.Matrix = matrix;
                }
            }
        }

        public Point3D Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (this.position != value)
                {
                    this.position = value;

                    Matrix3D matrix = this.positionAndScaleTransform.Matrix;
                    matrix.OffsetX = value.X;
                    matrix.OffsetY = value.Y;
                    matrix.OffsetZ = value.Z;
                    this.positionAndScaleTransform.Matrix = matrix;
                    this.OnPositionChanged();
                }
            }
        }

        public Visual3D Visual
        {
            get
            {
                return this.visual;
            }
        }

        public event EventHandler PositionChanged;

        protected void OnPositionChanged()
        {
            if (this.PositionChanged != null)
            {
                this.PositionChanged(this, new EventArgs());
            }
        }

        public override string ToString()
        {
            return string.Format("PointVisual: ({0})", this.Position);
        }
    }
}
