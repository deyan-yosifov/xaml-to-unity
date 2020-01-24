using System;

namespace CAGD.Controls.Controls3D
{
    public class GraphicStateFactory
    {
        private readonly GraphicState graphicState;

        internal GraphicStateFactory(GraphicState graphicState)
        {
            this.graphicState = graphicState;
        }

        internal GraphicState GraphicState
        {
            get
            {
                return this.graphicState;
            }
        }

        protected GraphicProperties GraphicProperties
        {
            get
            {
                return this.graphicState.Value;
            }
        }
    }
}
