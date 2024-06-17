using System;

namespace OCCPort
{
	//! Root   class    for    the    geometric     curves
	//! representation. Contains a range.
	//! Contains a first and a last parameter.

	internal class BRep_GCurve: BRep_CurveRepresentation
	{
		internal bool IsCurve3D()
		{
			throw new NotImplementedException();
		}

		public void Update()
		{

		}

		protected double myFirst;
		protected double myLast;

		internal void Range(double f, double l)
		{
			throw new NotImplementedException();
		}
	}
}