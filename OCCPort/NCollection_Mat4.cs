using System;

namespace OCCPort
{
	internal class NCollection_Mat4
	{
		public NCollection_Mat4()
		{
			myMat = new double[16];
		}

		double[] MyIdentityArray =
		 {1, 0, 0, 0,
   0, 1, 0, 0,
   0, 0, 1, 0,
   0, 0, 0, 1};

		public double[] myMat;
		public void InitIdentity()
		{

			Array.Copy(MyIdentityArray, myMat, 16);
		}

		internal void ChangeValue(int theCol, int theRow, double val)
		{
			myMat[theCol * 4 + theRow] = val;
		}

		internal void SetRow(int theRow, NCollection_Vec3 theVec)
		{
			SetValue(theRow, 0, theVec.x());
			SetValue(theRow, 1, theVec.y());
			SetValue(theRow, 2, theVec.z());

		}

		private void SetValue(int theRow, int theCol, double theValue)
		{
			myMat[theCol * 4 + theRow] = theValue;
		}

		internal double GetValue(int theRow, int theCol)
		{

			return myMat[theCol * 4 + theRow];

		}

		internal void SetColumn(int theCol, NCollection_Vec3 theVec)
		{

			SetValue(0, theCol, theVec.x());
			SetValue(1, theCol, theVec.y());
			SetValue(2, theCol, theVec.z());

		}


	}
}
