using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public class MaterialsManager : INotifyPropertiesChanged
    {
        private readonly MaterialGroup frontMaterials;
        private readonly MaterialGroup backMaterials;

        internal MaterialsManager(MaterialGroup frontMaterials, MaterialGroup backMaterials)
        {
            this.frontMaterials = frontMaterials;
            this.backMaterials = backMaterials;
        }

        public IEnumerable<Material> FrontMaterials
        {
            get
            {
                foreach (Material material in this.frontMaterials.Children)
                {
                    yield return material;
                }
            }
        }

        public IEnumerable<Material> BackMaterials
        {
            get
            {
                foreach (Material material in this.backMaterials.Children)
                {
                    yield return material;
                }
            }
        }

        public void AddFrontMaterial(Material material)
        {
            this.frontMaterials.Children.Add(material);
            this.OnPropertyChanged(GraphicPropertyNames.FrontMaterial);
        }

        public void AddBackMaterial(Material material)
        {
            this.backMaterials.Children.Add(material);
            this.OnPropertyChanged(GraphicPropertyNames.BackMaterial);
        }

        public void AddFrontDiffuseMaterial(Color color)
        {
            this.AddFrontMaterial(CreateDiffuseMaterial(color));
        }

        public void AddBackDiffuseMaterial(Color color)
        {
            this.AddBackMaterial(CreateDiffuseMaterial(color));
        }

        public void AddFrontTexture(ImageSource image)
        {
            this.AddFrontMaterial(CreateDiffuseMaterial(image));
        }

        public void AddBackTexture(ImageSource image)
        {
            this.AddBackMaterial(CreateDiffuseMaterial(image));
        }

        public static DiffuseMaterial CreateDiffuseMaterial(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            brush.Freeze();
            DiffuseMaterial material = new DiffuseMaterial(brush);
            material.Freeze();

            return material;
        }

        public static DiffuseMaterial CreateDiffuseMaterial(ImageSource image)
        {
            ImageBrush brush = new ImageBrush(image) { ViewportUnits = BrushMappingMode.Absolute };
            brush.Freeze();
            DiffuseMaterial material = new DiffuseMaterial(brush);
            material.Freeze();

            return material;
        }

        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertiesChanged(new string[] { propertyName });
        }

        private void OnPropertiesChanged(IEnumerable<string> propertyNames)
        {
            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged(this, new PropertiesChangedEventArgs(propertyNames));
            }
        }
    }
}
