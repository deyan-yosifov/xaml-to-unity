using System;
using UnityEngine;

namespace CAGD.Controls.Controls3D.Visuals
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PointVisual : MonoBehaviour, IVisual3D
    {
        private MeshRenderer mesh;
        private Collider colliderComponent;

        private void Awake()
        {
            this.mesh = this.GetComponent<MeshRenderer>();
            this.colliderComponent = this.GetComponent<Collider>();
        }

        public bool IsVisible
        {
            get => this.mesh.enabled;
            set
            {
                this.mesh.enabled = value;

                if (this.colliderComponent)
                {
                    this.colliderComponent.enabled = value;
                }
            }
        }

        public float Diameter
        {
            get
            {
                return this.transform.localScale.x;
            }
            set
            {
                if (this.Diameter != value)
                {
                    this.transform.localScale = new Vector3(value, value, value);
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.transform.localPosition;
            }
            set
            {
                if (this.Position != value)
                {
                    this.transform.localPosition = value;
                    this.OnPositionChanged();
                }
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
    }
}
