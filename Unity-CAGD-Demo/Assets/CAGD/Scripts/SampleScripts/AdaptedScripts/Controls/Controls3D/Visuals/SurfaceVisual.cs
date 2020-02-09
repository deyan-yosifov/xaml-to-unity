using UnityEngine;

namespace CAGD.Controls.Controls3D.Visuals
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SurfaceVisual : MonoBehaviour, IVisual3D
    {
        private MeshRenderer mesh;
        private MeshFilter filter;

        private void Awake()
        {
            this.mesh = this.GetComponent<MeshRenderer>();
        }

        public bool IsVisible
        {
            get => this.mesh.enabled;
            set => this.mesh.enabled = value;
        }

        public Mesh Mesh
        {
            get => this.filter.mesh;
            set => this.filter.mesh = value;
        }
    }
}
