using System;

namespace OCCPort
{
    public class V3d_Viewer
    {


        //! Returns the structure manager associated to this viewer.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }

        Graphic3d_StructureManager myStructureManager;

        internal void Update()
        {
            throw new NotImplementedException();
        }
    }
}