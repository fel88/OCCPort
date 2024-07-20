using System;

namespace OCCPort
{
    public class Prs3d_ShadingAspect
    {
        //! Returns the polygons aspect properties.
        public Graphic3d_AspectFillArea3d Aspect() { return myAspect; }
        Graphic3d_AspectFillArea3d myAspect = new Graphic3d_AspectFillArea3d();
        public void SetAspect(Graphic3d_AspectFillArea3d theAspect) { myAspect = theAspect; }

        internal void SetMaterial(Graphic3d_MaterialAspect theMaterial)
        {
            // throw new NotImplementedException();
        }
    }
}