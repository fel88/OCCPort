using System;

namespace OCCPort.Tester
{
	internal class Graphic3d_ArrayOfSegments: Graphic3d_ArrayOfPrimitives
	{
		private int aNbVertices;

		public Graphic3d_ArrayOfSegments()
		{
		}

		public Graphic3d_ArrayOfSegments(int aNbVertices)
		{
			this.aNbVertices = aNbVertices;
		}

		
	}
}