using OCCPort.Interfaces;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
using TriangleNet.Topology.DCEL;

namespace OCCPort
{
    //! Class implements functionality of model post-processing tool.
    //! Stores polygons on triangulations to TopoDS_Edge.
    public class BRepMesh_ModelPostProcessor : IMeshTools_ModelAlgo
    {
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            if (theModel == null)
            {
                return false;
            }

            // TODO: Force single threaded solution due to data races on edges sharing the same TShape
            //OSD_Parallel::For(0, theModel->EdgesNb(), PolygonCommitter(theModel), Standard_True/*!theParameters.InParallel*/);
            for (int i = 0; i < theModel.EdgesNb(); i++)
            {
                new PolygonCommitter(theModel).Run(i);
            }
            // Estimate deflection here due to BRepLib::EstimateDeflection requires
            // existence of both Poly_Triangulation and Poly_PolygonOnTriangulation.
            //OSD_Parallel::For(0, theModel->FacesNb(), DeflectionEstimator(theModel, theParameters), !theParameters.InParallel);
            for (int i = 0; i < theModel.FacesNb(); i++)
            {
                new DeflectionEstimator(theModel, theParameters).Run(i);

            }
            return true;
        }
    }
}