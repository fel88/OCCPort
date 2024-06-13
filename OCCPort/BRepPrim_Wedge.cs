using System;
using System.Security.Cryptography;

namespace OCCPort.Tester
{
	internal class BRepPrim_Wedge : BRepPrim_GWedge

	{
		public BRepPrim_Wedge(gp_Ax2 Axes, double dx, double dy, double dz) 
			: base(new BRepPrim_Builder(), Axes, dx, dy, dz)
		{
		

		}


		internal TopoDS_Shell Shell()
		{

			//if (IsDegeneratedShape())
			//	throw new Exception();

			//if (!ShellBuilt)
			//{
			//	myBuilder.MakeShell(myShell);

			//	if (HasFace(BRepPrim_XMin))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_XMin));
			//	if (HasFace(BRepPrim_XMax))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_XMax));
			//	if (HasFace(BRepPrim_YMin))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_YMin));
			//	if (HasFace(BRepPrim_YMax))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_YMax));
			//	if (HasFace(BRepPrim_ZMin))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_ZMin));
			//	if (HasFace(BRepPrim_ZMax))
			//		myBuilder.AddShellFace(myShell, Face(BRepPrim_ZMax));

			//	myShell.Closed(BRep_Tool::IsClosed(myShell));
			//	myBuilder.CompleteShell(myShell);
			//	ShellBuilt = Standard_True;
			//}
			return myShell;

		}
	}
}