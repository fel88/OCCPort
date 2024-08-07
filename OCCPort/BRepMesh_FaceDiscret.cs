﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Class implements functionality starting triangulation of model's faces.
    //! Each face is processed separately and can be executed in parallel mode.
    //! Uses mesh algo factory passed as initializer to create instance of triangulation 
    //! algorithm according to type of surface of target face.
    public class BRepMesh_FaceDiscret : IMeshTools_ModelAlgo
    {
        public BRepMesh_FaceDiscret(IMeshTools_MeshAlgoFactory theAlgoFactory)
        {
            myAlgoFactory = (theAlgoFactory);
        }
        //! Auxiliary functor for parallel processing of Faces.
        public class FaceListFunctor
        {
            BRepMesh_FaceDiscret myAlgo;
            Message_ProgressScope myScope;
            List<Message_ProgressRange> myRanges = new List<Message_ProgressRange>();

            public FaceListFunctor(BRepMesh_FaceDiscret theAlgo,
                   Message_ProgressRange theRange)

            {
                myAlgo = theAlgo;
                myScope = new Message_ProgressScope(theRange, "Face Discret", theAlgo.myModel.FacesNb());
                //myRanges.reserve(theAlgo->myModel->FacesNb());
                for (int aFaceIter = 0; aFaceIter < theAlgo.myModel.FacesNb(); ++aFaceIter)
                {
                    myRanges.Add(myScope.Next());
                }
            }
            public void call(int theFaceIndex)
            {
                if (!myScope.More())
                {
                    return;
                }
                Message_ProgressScope aFaceScope = new Message_ProgressScope(myRanges[theFaceIndex], null, 1);
                myAlgo.process(theFaceIndex, aFaceScope.Next());
            }
        }

        IMeshTools_MeshAlgoFactory myAlgoFactory;
        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters;

        void process(int theFaceIndex,
            Message_ProgressRange theRange)
        {
            IMeshData_Face aDFace = myModel.GetFace(theFaceIndex);
            try
            {
                IMeshTools_MeshAlgo aMeshingAlgo =
                  myAlgoFactory.GetAlgo(aDFace.GetSurface().GetType(), ref myParameters);




                aMeshingAlgo.Perform(aDFace, myParameters, theRange);
            }
            catch (Exception ex)
            {

            }

        }

        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            myModel = theModel;
            myParameters = theParameters;
            if (myModel == null)
            {
                return false;
            }

            FaceListFunctor aFunctor = new FaceListFunctor(this, theRange);
            //use Parallel here in future
            for (int i = 0; i < myModel.FacesNb(); i++)
            {
                aFunctor.call(i);
            }

            //            OSD_Parallel::For(0, myModel->FacesNb(), aFunctor, !(myParameters.InParallel && myModel->FacesNb() > 1));
            if (!theRange.More())
            {
                return false;
            }

            myModel = null; // Do not hold link to model.
            return true;
        }
    }


}