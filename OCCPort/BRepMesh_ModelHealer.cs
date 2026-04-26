using OCCPort.Interfaces;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Class implements functionality of model healer tool.
    //! Iterates over model's faces and checks consistency of their wires, 
    //! i.e.whether wires are closed and do not contain self - intersections.
    //! In case if wire contains disconnected parts, ends of adjacent edges
    //! forming the gaps are connected in parametric space forcibly. The notion
    //! of this operation is to create correct discrete model defined relatively
    //! parametric space of target face taking into account connectivity and 
    //! tolerances of 3D space only. This means that there are no specific 
    //! computations are made for the sake of determination of U and V tolerance.
    //! Registers intersections on edges forming the face's shape and tries to
    //! amplify discrete representation by decreasing of deflection for the target edge.
    //! Checks can be performed in parallel mode.
    public class BRepMesh_ModelHealer : IMeshTools_ModelAlgo
    {

        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters;
        DMapOfIFacePtrsMapOfIEdgePtrs myFaceIntersectingEdges;

        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            //(void)theRange;
            myModel = theModel;
            myParameters = theParameters;
            if (myModel == null)
            {
                return false;
            }

            // MinSize is made as a constant. It is connected with
            // the fact that too rude discretisation can lead to 
            // self-intersecting polygon, which cannot be fixed.
            // As result the face will not be triangulated at all.
            // E.g. see "Test mesh standard_mesh C7", the face #17.
            myParameters.MinSize = Precision.Confusion();

            myFaceIntersectingEdges = new DMapOfIFacePtrsMapOfIEdgePtrs();
            for (int aFaceInd = 0; aFaceInd < myModel.FacesNb(); ++aFaceInd)
            {
             //   myFaceIntersectingEdges.Bind(myModel.GetFace(aFaceInd).get(), null);
            }

            // TODO: Here we can process edges in order to remove close discrete points.
            //OSD_Parallel::For(0, myModel->FacesNb(), *this, !isParallel());
         //   amplifyEdges();

            //MapOfIFacePtrsMapOfIEdgePtrs::Iterator aFaceIt(*myFaceIntersectingEdges);
            //for (; aFaceIt.More(); aFaceIt.Next())
            //{
            //    if (!aFaceIt.Value().IsNull())
            //    {
            //        const IMeshData::IFaceHandle aDFace = aFaceIt.Key();
            //        aDFace->SetStatus(IMeshData_SelfIntersectingWire);
            //        aDFace->SetStatus(IMeshData_Failure);
            //    }
            //}

            myFaceIntersectingEdges = null;
            myModel = null; // Do not hold link to model.
            return true;
        }
    }

}