using System.Reflection.Metadata;

namespace OCCPort
{
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
                    if (anEdge.Movability() == Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
                    {
                        continue;
                    }

                    anEdge.SetMovability(Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free);
                    myStructure.RemoveLink(i);
                }
            }
        }
    }
}