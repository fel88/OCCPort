using System;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;

namespace OCCPort
{
	//! Builds the mesh of a shape with respect of their 
	//! correctly triangulated parts 
	public class BRepMesh_IncrementalMesh : BRepMesh_DiscretRoot
	{
		public BRepMesh_IncrementalMesh()

		{
			myModified = (false);
			myStatus = (int)IMeshData_Status.IMeshData_NoError;
		}

		IMeshTools_Parameters myParameters;
		bool myModified;
		int myStatus;
		//! Constructor.
		//! Automatically calls method Perform.
		//! @param theShape shape to be meshed.
		//! @param theLinDeflection linear deflection.
		//! @param isRelative if TRUE deflection used for discretization of 
		//! each edge will be <theLinDeflection> * <size of edge>. Deflection 
		//! used for the faces will be the maximum deflection of their edges.
		//! @param theAngDeflection angular deflection.
		//! @param isInParallel if TRUE shape will be meshed in parallel.
		public BRepMesh_IncrementalMesh(TopoDS_Shape theShape,
										   double theLinDeflection,

										   bool isRelative = false,

										   double theAngDeflection = 0.5,

										   bool isInParallel = false)
		{

		}

		public static int Discret(TopoDS_Shape theShape,
			double theDeflection,
			double theAngle,
			ref BRepMesh_DiscretRoot theAlgo)
		{
			BRepMesh_IncrementalMesh anAlgo = new BRepMesh_IncrementalMesh();
			/*anAlgo.ChangeParameters().Deflection = theDeflection;
			anAlgo.ChangeParameters().Angle = theAngle;
			anAlgo.ChangeParameters().InParallel = IS_IN_PARALLEL;*/
			anAlgo.SetShape(theShape);
			theAlgo = anAlgo;
			return 0; // no error
		}

		public override void Perform(Message_ProgressRange theRange)
		{
			BRepMesh_Context aContext = new BRepMesh_Context(myParameters.MeshAlgo);
			Perform(aContext, theRange);
		}

		public void Perform(IMeshTools_Context theContext, Message_ProgressRange theRange = null)
		{
			//initParameters();

			//theContext.SetShape(Shape());
			//theContext->ChangeParameters() = myParameters;
			//theContext->ChangeParameters().CleanModel = Standard_False;

			//Message_ProgressScope aPS(theRange, "Perform incmesh", 10);
			IMeshTools_MeshBuilder aIncMesh = new IMeshTools_MeshBuilder(theContext);
			aIncMesh.Perform(aPS.Next(9));
			//if (!aPS.More())
			//{
			//	myStatus = IMeshData_UserBreak;
			//	return;
			//}
			//myStatus = IMeshData_NoError;
			IMeshData_Model aModel = theContext.GetModel();
			if (!aModel.IsNull())
			{
				for (int aFaceIt = 0; aFaceIt < aModel.FacesNb(); ++aFaceIt)
				{
					IMeshData.IFaceHandle aDFace = aModel.GetFace(aFaceIt);
					//		myStatus |= aDFace->GetStatusMask();

					//		for (Standard_Integer aWireIt = 0; aWireIt < aDFace->WiresNb(); ++aWireIt)
					//		{
					//			const IMeshData::IWireHandle&aDWire = aDFace->GetWire(aWireIt);
					//			myStatus |= aDWire->GetStatusMask();
					//		}
				}
			}
			//aPS.Next(1);
			//setDone();
		}
	}


}