using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public abstract class ShapeBase
    {
        private readonly GeometryModel3D geometryModel;
        private readonly MaterialGroup frontMaterialGroup;
        private readonly MaterialGroup backMaterialGroup;
        private readonly MaterialsManager materialsManager;

        protected internal GeometryModel3D GeometryModel
        {
            get
            {
                return this.geometryModel;
            }
        }

        protected ShapeBase()
        {
            this.geometryModel = new GeometryModel3D();
            this.frontMaterialGroup = new MaterialGroup();
            this.backMaterialGroup = new MaterialGroup();
            this.geometryModel.Material = this.frontMaterialGroup;
            this.geometryModel.BackMaterial = this.backMaterialGroup;
            this.materialsManager = new MaterialsManager(this.frontMaterialGroup, this.backMaterialGroup);
        }

        public MaterialsManager MaterialsManager
        {
            get
            {
                return this.materialsManager;
            }
        }
    }
}
