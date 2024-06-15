namespace OCCPort.OpenGL
{
    //! Implementation of low-level graphic structure.
    public class OpenGl_Structure : Graphic3d_CStructure
    {
        // =======================================================================
        public void renderGeometry(OpenGl_Workspace theWorkspace,
                                         ref bool theHasClosed)
        {


        }
        public void Render(OpenGl_Workspace theWorkspace)
        {
            // Process the structure only if visible
            //if (!visible)
            {
                return;
            }

        }
    }
}


