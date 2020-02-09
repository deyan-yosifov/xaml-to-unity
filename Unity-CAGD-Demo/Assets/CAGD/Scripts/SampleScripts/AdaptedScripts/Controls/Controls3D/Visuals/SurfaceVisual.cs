using UnityEngine;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class SurfaceVisual : MonoBehaviour, IVisual3D
    {
        [SerializeField]
        private MeshRenderer mesh;
        private MeshFilter filter;

        private void Awake()
        {
            this.filter = this.mesh.GetComponent<MeshFilter>();
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
