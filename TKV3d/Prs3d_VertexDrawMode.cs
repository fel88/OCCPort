namespace TKV3d
{
    //! Describes supported modes of visualization of the shape's vertices:
    //! VDM_Isolated  - only isolated vertices (not belonging to a face) are displayed.
    //! VDM_All       - all vertices of the shape are displayed.
    //! VDM_Inherited - the global settings are inherited and applied to the shape's presentation.
    public enum Prs3d_VertexDrawMode
    {
        Prs3d_VDM_Isolated,
        Prs3d_VDM_All,
        Prs3d_VDM_Inherited
    }
}

