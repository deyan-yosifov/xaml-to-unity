using UnityEngine;

namespace CAGD.Controls.Controls3D.Visuals
{
    [RequireComponent(typeof(MeshRenderer))]
    public class LineVisual : MonoBehaviour, IVisual3D
    {
        private static readonly Vector3 InitialVector = Vector3.forward;
        private MeshRenderer mesh;

        private void Awake()
        {
            this.mesh = this.GetComponent<MeshRenderer>();
        }

        public bool IsVisible
        {
            get => this.mesh.enabled;
            set => this.mesh.enabled = value;
        }

        public Vector3 Start { get; private set; }

        public Vector3 End { get; private set; }

        public float Thickness
        {
            get
            {
                return this.transform.localScale.x;
            }
            set
            {
                if (this.Thickness != value)
                {
                    this.transform.localScale = new Vector3(value, value, this.transform.localScale.z);
                }
            }
        }

        public void MoveTo(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;

            this.CalculatePositionTransform();
        }

        private void CalculatePositionTransform()
        {
            Vector3 direction = this.End - this.Start;
            this.transform.localScale = new Vector3(this.Thickness, this.Thickness, direction.magnitude);
            this.transform.localRotation = Quaternion.FromToRotation(LineVisual.InitialVector, direction);
            this.transform.localPosition = this.Start;
        }
    }
}
