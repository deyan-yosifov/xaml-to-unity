using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class VisualOwner : IVisual3DOwner
    {
        private readonly Visual3D visual;

        public VisualOwner(Visual3D visual)
        {
            this.visual = visual;
        }

        public Visual3D Visual
        {
            get
            {
                return this.visual;
            }
        }
    }
}
