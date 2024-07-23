using System;

namespace OCCPort
{
	internal class StdPrs_ToolTriangulatedShape : BRepLib_ToolTriangulatedShape

	{
		internal static void ClearOnOwnDeflectionChange(TopoDS_Shape myshape, Prs3d_Drawer myDrawer, bool standard_True)
		{
			//throw new NotImplementedException();
		}

		public static double GetDeflection(TopoDS_Shape myshape, Prs3d_Drawer theDrawer)
		{
			throw new NotImplementedException();
			//if (theDrawer.TypeOfDeflection() != Aspect_TOD_RELATIVE)
			//{
			//	return theDrawer.MaximalChordialDeviation();
			//}

			//Bnd_Box aBndBox;
			//BRepBndLib.Add(theShape, aBndBox, false);
			//if (aBndBox.IsVoid())
			//{
			//	return theDrawer.MaximalChordialDeviation();
			//}
			//else if (aBndBox.IsOpen())
			//{
			//	if (!aBndBox.HasFinitePart())
			//	{
			//		return theDrawer.MaximalChordialDeviation();
			//	}
			//	aBndBox = aBndBox.FinitePart();
			//}

			//// store computed relative deflection of shape as absolute deviation coefficient in case relative type to use it later on for sub-shapes
			//double aDeflection = Prs3d.GetDeflection(aBndBox, theDrawer.DeviationCoefficient(), theDrawer.MaximalChordialDeviation());
			//theDrawer.SetMaximalChordialDeviation(aDeflection);
			//return aDeflection;
		}

		internal static bool Tessellate(TopoDS_Shape theShape, Prs3d_Drawer theDrawer)
		{
			bool wasRecomputed = false;
			// Check if it is possible to avoid unnecessary recomputation of shape triangulation
			if (IsTessellated(theShape, theDrawer))
			{
				return wasRecomputed;
			}

			double aDeflection = GetDeflection(theShape, theDrawer);

			// retrieve meshing tool from Factory
			

			BRepMesh_DiscretRoot aMeshAlgo = BRepMesh_DiscretFactory.Discret(theShape,
																							 aDeflection,
																							 theDrawer.DeviationAngle());

			if (aMeshAlgo != null)
			{
				aMeshAlgo.Perform();
				wasRecomputed = true;
			}

			return wasRecomputed;
		}

		private static bool IsTessellated(TopoDS_Shape theShape, Prs3d_Drawer theDrawer)
		{
            return BRepTools.Triangulation(theShape, GetDeflection(theShape, theDrawer), true);
        }
	}
}