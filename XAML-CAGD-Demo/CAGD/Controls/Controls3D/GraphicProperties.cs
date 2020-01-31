using CAGD.Utilities;
using CAGD.Controls.Controls3D.Shapes;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public class GraphicProperties : ICloneable<GraphicProperties>, INotifyPropertiesChanged
    {
        private readonly MaterialGroup frontMaterials;
        private readonly MaterialGroup backMaterials;
        private readonly MaterialsManager materialsManager;
        private readonly GraphicsManager2D graphicsManager2d;
        private SphereType sphereType;
        private int arcResolution;
        private int subDevisions;
        private double thickness;
        private bool isSmooth;

        public GraphicProperties()
        {
            this.frontMaterials = new MaterialGroup();
            this.backMaterials = new MaterialGroup();
            this.materialsManager = new MaterialsManager(this.frontMaterials, this.backMaterials);
            this.graphicsManager2d = new GraphicsManager2D();
            this.materialsManager.PropertiesChanged += this.MaterialsManagerPropertiesChanged;
            this.graphicsManager2d.PropertiesChanged += GraphicsManager2dPropertiesChanged;
            this.Thickness = 1;
            this.ArcResolution = 10;
            this.SubDevisions = 2;
            this.SphereType = Shapes.SphereType.UVSphere;
            this.IsSmooth = true;
        }

        private GraphicProperties(GraphicProperties other)
            : this()
        {
            this.thickness = other.Thickness;
            this.isSmooth = other.IsSmooth;
            this.arcResolution = other.ArcResolution;
            this.graphicsManager2d = other.graphicsManager2d.Clone();

            foreach(Material material in other.frontMaterials.Children)
            {
                this.frontMaterials.Children.Add(material);
            }

            foreach (Material material in other.backMaterials.Children)
            {
                this.backMaterials.Children.Add(material);
            }
        }

        public GraphicsManager2D Graphics2D
        {
            get
            {
                return this.graphicsManager2d;
            }
        }

        public MaterialsManager MaterialsManager
        {
            get
            {
                return this.materialsManager;
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
                    this.OnPropertyChanged(GraphicPropertyNames.Thickness);
                }
            }
        }

        public SphereType SphereType
        {
            get
            {
                return this.sphereType;
            }
            set
            {
                if (this.sphereType != value)
                {
                    this.sphereType = value;
                    this.OnPropertyChanged(GraphicPropertyNames.SphereType);
                }
            }
        }

        public int ArcResolution
        {
            get
            {
                return this.arcResolution;
            }
            set
            {
                if (this.arcResolution != value)
                {
                    this.arcResolution = value;
                    this.OnPropertyChanged(GraphicPropertyNames.ArcResolution);
                }
            }
        }

        public int SubDevisions
        {
            get
            {
                return this.subDevisions;
            }
            set
            {
                if (this.subDevisions != value)
                {
                    this.subDevisions = value;
                    this.OnPropertyChanged(GraphicPropertyNames.SubDevisions);
                }
            }
        }

        public bool IsSmooth
        {
            get
            {
                return this.isSmooth;
            }
            set
            {
                if (this.isSmooth != value)
                {
                    this.isSmooth = value;
                    this.OnPropertyChanged(GraphicPropertyNames.IsSmooth);
                }
            }
        }

        public GraphicProperties Clone()
        {
            return new GraphicProperties(this);
        }

        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertiesChanged(new string[] { propertyName });
        }

        private void OnPropertiesChanged(IEnumerable<string> propertyNames)
        {
            this.OnPropertiesChanged(this, new PropertiesChangedEventArgs(propertyNames));
        }

        private void OnPropertiesChanged(object sender, PropertiesChangedEventArgs args)
        {
            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged(sender, args);
            }
        }

        private void MaterialsManagerPropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            this.OnPropertiesChanged(this, e);
        }

        private void GraphicsManager2dPropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            this.OnPropertiesChanged(this, e);
        }
    }
}
