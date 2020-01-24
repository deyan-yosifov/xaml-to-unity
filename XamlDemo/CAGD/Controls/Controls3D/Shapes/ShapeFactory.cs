using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class ShapeFactory : GraphicStateFactory
    {
        internal ShapeFactory(GraphicState graphicState)
            : base(graphicState)
        {
        }

        public Cube CreateCube()
        {
            Cube cube = new Cube();
            this.ApplyMatherials(cube);

            return cube;
        }

        public Cylinder CreateCylinder(bool isClosed = true)
        {
            Cylinder cylinder = new Cylinder(this.GraphicProperties.ArcResolution, isClosed, this.GraphicProperties.IsSmooth);
            this.ApplyMatherials(cylinder);

            return cylinder;
        }

        public Line CreateLine()
        {
            Line line = new Line(this.GraphicProperties.ArcResolution);
            this.ApplyMatherials(line);

            return line;
        }

        public ISphereShape CreateSphere()
        {
            ISphereShape sphere;

            switch (this.GraphicProperties.SphereType)
            {
                case SphereType.UVSphere:
                    sphere = new Sphere(this.GraphicProperties.ArcResolution, this.GraphicProperties.ArcResolution, this.GraphicProperties.IsSmooth);
                    break;
                case SphereType.IcoSphere:
                    sphere = new IcoSphere(this.GraphicProperties.SubDevisions, this.GraphicProperties.IsSmooth);
                    break;
                case SphereType.OctaSphere:
                    sphere = new OctaSphere(this.GraphicProperties.SubDevisions, this.GraphicProperties.IsSmooth);
                    break;
                case SphereType.TetraSphere:
                    sphere = new TetraSphere(this.GraphicProperties.SubDevisions, this.GraphicProperties.IsSmooth);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported sphere type: {0}", this.GraphicProperties.SphereType));
            }


            this.ApplyMatherials(sphere.Shape);

            return sphere;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            this.ApplyMatherials(mesh);

            return mesh;
        }

        private void ApplyMatherials(ShapeBase shape)
        {
            foreach (Material material in this.GraphicProperties.MaterialsManager.FrontMaterials)
            {
                shape.MaterialsManager.AddFrontMaterial(material);
            }

            foreach (Material material in this.GraphicProperties.MaterialsManager.BackMaterials)
            {
                shape.MaterialsManager.AddBackMaterial(material);
            }
        }
    }
}
