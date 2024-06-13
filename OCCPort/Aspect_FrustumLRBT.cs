using System;

namespace OCCPort
{
	public struct Aspect_FrustumLRBT
	{

		public Aspect_FrustumLRBT(Aspect_FrustumLRBT theOther)
		{
			Left = theOther.Left;
			Right = theOther.Right;
			Top = theOther.Top;
			Bottom = theOther.Bottom;
		}
		

		public double Right { get; internal set; }
		public double Top { get; internal set; }
		public double Left { get; internal set; }
		public double Bottom { get; internal set; }


		//! Return multiplied frustum.
		public Aspect_FrustumLRBT Multiplied(double theScale)
		{
			Aspect_FrustumLRBT aCopy=this;
			aCopy.Multiply(theScale);
			return aCopy;
		}

		private void Multiply(double theScale)
		{

			Left *= theScale;
			Right *= theScale;
			Bottom *= theScale;
			Top *= theScale;

		}
	}
}