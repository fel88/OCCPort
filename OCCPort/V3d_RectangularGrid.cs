namespace OCCPort
{
	internal class V3d_RectangularGrid : Aspect_RectangularGrid
	{
		public V3d_RectangularGrid(V3d_Viewer aViewer, Quantity_Color quantity_Color1, Quantity_Color quantity_Color2)
			: base(1.0, 1.0)
		{
			myViewer = (aViewer);

		}

		Graphic3d_Structure myStructure;
		Graphic3d_Group myGroup;
		gp_Ax3 myCurViewPlane;
		V3d_Viewer myViewer;
	}
}