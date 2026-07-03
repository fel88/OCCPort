using TKService;


namespace OCCPort.OpenGL
{
    //! Dummy structure which just redirects to groups of another structure.
    public class OpenGl_StructureShadow : OpenGl_Structure
    {
        public OpenGl_StructureShadow(Graphic3d_StructureManager theManager,
            OpenGl_Structure theStructure
            ) : base(theManager)
        {
            OpenGl_StructureShadow aShadow = (OpenGl_StructureShadow)(theStructure);
            myParent = aShadow==null ? theStructure : aShadow.myParent;

            IsInfinite = myParent.IsInfinite;
            myBndBox = myParent.BoundingBox();

            SetTransformation(myParent.Transformation());
            myInstancedStructure = (OpenGl_Structure)(myParent.InstancedStructure());
            myTrsfPers = myParent.TransformPersistence();

            // reuse instanced structure API
            myInstancedStructure = myParent;
        }

        OpenGl_Structure myParent;

    }
}



