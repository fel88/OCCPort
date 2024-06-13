namespace OCCPort
{
	internal class NCollection_Vec4
	{
		public NCollection_Vec4()
		{
			v = new double[4];
		}

		protected double[] v; //!< define the vector as array to avoid structure alignment issues

	}
}
