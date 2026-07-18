namespace TKMesh
{
    //! Auxiliary tool providing API for manipulation with BRepMesh_DataStructureOfDelaun.
    internal class BRepMesh_MeshTool
    {
        BRepMesh_DataStructureOfDelaun myStructure;

        public BRepMesh_MeshTool(BRepMesh_DataStructureOfDelaun theStructure)
        {

            myStructure = (theStructure);


        }

        public void EraseFreeLinks()
        {
            for (int i = 1; i <= myStructure.NbLinks(); i++)
            {
                if (myStructure.ElementsConnectedTo(i).IsEmpty())
                {
                    BRepMesh_Edge anEdge = (BRepMesh_Edge)myStructure.GetLink(i);
                    if (anEdge.Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
                    {
                        continue;
                    }

                    anEdge.SetMovability(BRepMesh_DegreeOfFreedom.BRepMesh_Free);
                    myStructure.RemoveLink(i);
                }
            }
        }
    }
}



