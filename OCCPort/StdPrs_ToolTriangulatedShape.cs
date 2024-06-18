using System;

namespace OCCPort.Tester
{
    internal class StdPrs_ToolTriangulatedShape: BRepLib_ToolTriangulatedShape
	
    {
        internal static void ClearOnOwnDeflectionChange(TopoDS_Shape myshape, Prs3d_Drawer myDrawer, bool standard_True)
        {
            throw new NotImplementedException();
        }

        internal static void GetDeflection(TopoDS_Shape myshape, Prs3d_Drawer myDrawer)
        {
            throw new NotImplementedException();
        }

        internal static bool Tessellate(TopoDS_Shape myshape, Prs3d_Drawer myDrawer)
        {
            throw new NotImplementedException();
        }
    }

	internal class BRepLib_ToolTriangulatedShape
	{
	}
}