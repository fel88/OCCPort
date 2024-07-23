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

		internal static void Discret(TopoDS_Shape theShape, double theDeflection, double theAngle, BRepMesh_DiscretRoot anInstancePtr)
		{
			throw new NotImplementedException();
		}
		public override void Perform(Message_ProgressRange theRange)
		{
			//BRepMesh_Context aContext = new BRepMesh_Context(myParameters.MeshAlgo);
			//Perform(aContext, theRange);
		}

		public void Perform(IMeshTools_Context theContext, Message_ProgressRange theRange = null)
		{
			//initParameters();

			//theContext.SetShape(Shape());
			//theContext->ChangeParameters() = myParameters;
			//theContext->ChangeParameters().CleanModel = Standard_False;

			//Message_ProgressScope aPS(theRange, "Perform incmesh", 10);
			//IMeshTools_MeshBuilder aIncMesh(theContext);
			//aIncMesh.Perform(aPS.Next(9));
			//if (!aPS.More())
			//{
			//	myStatus = IMeshData_UserBreak;
			//	return;
			//}
			//myStatus = IMeshData_NoError;
			//const Handle(IMeshData_Model)&aModel = theContext->GetModel();
			//if (!aModel.IsNull())
			//{
			//	for (Standard_Integer aFaceIt = 0; aFaceIt < aModel->FacesNb(); ++aFaceIt)
			//	{
			//		const IMeshData::IFaceHandle&aDFace = aModel->GetFace(aFaceIt);
			//		myStatus |= aDFace->GetStatusMask();

			//		for (Standard_Integer aWireIt = 0; aWireIt < aDFace->WiresNb(); ++aWireIt)
			//		{
			//			const IMeshData::IWireHandle&aDWire = aDFace->GetWire(aWireIt);
			//			myStatus |= aDWire->GetStatusMask();
			//		}
			//	}
			//}
			//aPS.Next(1);
			//setDone();
		}
	}

	//! Class implementing default context of BRepMesh algorithm.
	//! Initializes context by default algorithms.
	public class BRepMesh_Context : IMeshTools_Context
	{
	}

	//! Interface class representing context of BRepMesh algorithm.
	//! Intended to cache discrete model and instances of tools for 
	//! its processing.
	public interface IMeshTools_Context : IMeshData_Shape
	{

	}

	//! Interface class representing model with associated TopoDS_Shape.
	//! Intended for inheritance by structures and algorithms keeping 
	//! reference TopoDS_Shape.
	public interface IMeshData_Shape
	{


	}

}