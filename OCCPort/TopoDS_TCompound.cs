using OCCPort.Tester;

namespace OCCPort
{
	internal class TopoDS_TCompound : TopoDS_TShape
	{
		public TopoDS_TCompound()
		{
		}

		public override TopAbs_ShapeEnum ShapeType()
		{
			return TopAbs_ShapeEnum.TopAbs_COMPOUND;
		}
	}
}