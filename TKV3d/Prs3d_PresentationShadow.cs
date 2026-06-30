using OCCPort.Common;
using System.Reflection.Metadata;
using TKService;

namespace TKV3d
{
    //! Defines a "shadow" of existing presentation object with custom aspects.
    public class Prs3d_PresentationShadow : Graphic3d_Structure
    {
        //! Returns the id of the parent presentation
        public int ParentId() { return myParentStructId; }
        int myParentStructId;
        //! Returns view affinity of the parent presentation
        public Graphic3d_ViewAffinity ParentAffinity() { return myParentAffinity; }

        Graphic3d_ViewAffinity myParentAffinity;
  

    }
}

