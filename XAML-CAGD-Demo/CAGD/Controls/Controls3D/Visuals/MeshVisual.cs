using CAGD.Controls.Controls3D.Shapes;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class MeshVisual : IVisual3DOwner
    {
        private readonly Mesh mesh;
        private readonly ModelVisual3D visual;

        public MeshVisual(Mesh mesh)
        {
            this.mesh = mesh;

            this.visual = new ModelVisual3D()
            {
                Content = mesh.GeometryModel
            };
        }

        public Visual3D Visual
        {
            get
            {
                return this.visual;
            }
        }

        public Mesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }
    }
}
