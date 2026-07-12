using TKBRep;
using TKernel;
using TKMath;
using TKTopAlgo;

namespace TKMesh
{
    //! Class implements interface representing tool for discrete model building.
    //! 
    //! The following statuses should be used by default:
    //! Message_Done1 - model has been successfully built.
    //! Message_Fail1 - empty shape.
    //! Message_Fail2 - model has not been build due to unexpected reason.
    public class BRepMesh_ModelBuilder : IMeshTools_ModelBuilder
    {
        //=======================================================================
        // Function: Perform
        // Purpose : 
        //=======================================================================
        public override IMeshData_Model performInternal(
  TopoDS_Shape theShape,
  IMeshTools_Parameters theParameters)
        {
            BRepMeshData_Model aModel = null;

            Bnd_Box aBox = new Bnd_Box();
            BRepBndLib.Add(theShape, aBox, false);

            if (!aBox.IsVoid())
            {
                // Build data model for further processing.
                aModel = new BRepMeshData_Model(theShape);

                if (theParameters.Relative)
                {
                    double aMaxSize = 0;
                    BRepMesh_ShapeTool.BoxMaxDimension(aBox, ref aMaxSize);
                    aModel.SetMaxSize(aMaxSize);
                }
                else
                {
                    aModel.SetMaxSize(Math.Max(theParameters.Deflection,
                                           theParameters.DeflectionInterior));
                }

                IMeshTools_ShapeVisitor aVisitor = new BRepMesh_ShapeVisitor(aModel);

                IMeshTools_ShapeExplorer aExplorer = new MeshTools_ShapeExplorer(theShape);
                aExplorer.Accept(aVisitor);
                SetStatus(Message_Status.Message_Done1);
            }
            else
            {
                SetStatus(Message_Status.Message_Fail1);
            }

            return aModel;
        }

        //! Sets maximum size of shape's bounding box.
        public void SetMaxSize(double theValue)
        {
            myMaxSize = theValue;
        }

        double myMaxSize;
        /*Handle(NCollection_IncAllocator) myAllocator;
  IMeshData::VectorOfIFaceHandles myDFaces;
		IMeshData::VectorOfIEdgeHandles myDEdges;*/
    }


}


