using System;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace OCCPort
{
	public class NCollection_Mat4
	{
		public NCollection_Mat4()
		{
			InitIdentity();

		}

		//! Compute matrix multiplication product: A * B.
		//! @param theMatA [in] the matrix "A".
		//! @param theMatB [in] the matrix "B".
		static NCollection_Mat4 Multiply(NCollection_Mat4 theMatA,
									 NCollection_Mat4 theMatB)
		{
			NCollection_Mat4 aMatRes = new NCollection_Mat4();

			int aInputElem;



			for (int aResElem = 0; aResElem < 16; ++aResElem)

			{
				aMatRes.myMat[aResElem] = 0;
				for (aInputElem = 0; aInputElem < 4; ++aInputElem)
				{
					aMatRes.myMat[aResElem] += theMatA.GetValue(aResElem % 4, aInputElem)
											 * theMatB.GetValue(aInputElem, aResElem / 4);
				}
			}

			return aMatRes;
		}


		//! Compute inverted matrix.
		//! @param theOutMx [out] the inverted matrix
		//! @return true if reversion success
		public bool Inverted(out NCollection_Mat4 theOutMx)
		{
			double aDet;
			return Inverted(out theOutMx, out aDet);
		}
		//! Compute inverted matrix.
		//! @param theOutMx [out] the inverted matrix
		//! @param theDet   [out] determinant of matrix
		//! @return true if reversion success
		public bool Inverted(out NCollection_Mat4 theOutMx, out double theDet)
		{
			theOutMx = new NCollection_Mat4();
			var inv = theOutMx.myMat;

			// use short-cut for better readability
			var m = myMat;

			inv[0] = m[5] * (m[10] * m[15] - m[11] * m[14]) -

				  m[9] * (m[6] * m[15] - m[7] * m[14]) -

				  m[13] * (m[7] * m[10] - m[6] * m[11]);

			inv[1] = m[1] * (m[11] * m[14] - m[10] * m[15]) -

				  m[9] * (m[3] * m[14] - m[2] * m[15]) -

				  m[13] * (m[2] * m[11] - m[3] * m[10]);

			inv[2] = m[1] * (m[6] * m[15] - m[7] * m[14]) -

				  m[5] * (m[2] * m[15] - m[3] * m[14]) -

				  m[13] * (m[3] * m[6] - m[2] * m[7]);

			inv[3] = m[1] * (m[7] * m[10] - m[6] * m[11]) -

				  m[5] * (m[3] * m[10] - m[2] * m[11]) -

				  m[9] * (m[2] * m[7] - m[3] * m[6]);

			inv[4] = m[4] * (m[11] * m[14] - m[10] * m[15]) -

				  m[8] * (m[7] * m[14] - m[6] * m[15]) -

				  m[12] * (m[6] * m[11] - m[7] * m[10]);

			inv[5] = m[0] * (m[10] * m[15] - m[11] * m[14]) -

				  m[8] * (m[2] * m[15] - m[3] * m[14]) -

				  m[12] * (m[3] * m[10] - m[2] * m[11]);

			inv[6] = m[0] * (m[7] * m[14] - m[6] * m[15]) -

				  m[4] * (m[3] * m[14] - m[2] * m[15]) -

				  m[12] * (m[2] * m[7] - m[3] * m[6]);

			inv[7] = m[0] * (m[6] * m[11] - m[7] * m[10]) -

				  m[4] * (m[2] * m[11] - m[3] * m[10]) -

				  m[8] * (m[3] * m[6] - m[2] * m[7]);

			inv[8] = m[4] * (m[9] * m[15] - m[11] * m[13]) -

				  m[8] * (m[5] * m[15] - m[7] * m[13]) -

				  m[12] * (m[7] * m[9] - m[5] * m[11]);

			inv[9] = m[0] * (m[11] * m[13] - m[9] * m[15]) -

				  m[8] * (m[3] * m[13] - m[1] * m[15]) -

				  m[12] * (m[1] * m[11] - m[3] * m[9]);

			inv[10] = m[0] * (m[5] * m[15] - m[7] * m[13]) -

				  m[4] * (m[1] * m[15] - m[3] * m[13]) -

				  m[12] * (m[3] * m[5] - m[1] * m[7]);

			inv[11] = m[0] * (m[7] * m[9] - m[5] * m[11]) -

				  m[4] * (m[3] * m[9] - m[1] * m[11]) -

				  m[8] * (m[1] * m[7] - m[3] * m[5]);

			inv[12] = m[4] * (m[10] * m[13] - m[9] * m[14]) -

				  m[8] * (m[6] * m[13] - m[5] * m[14]) -

				  m[12] * (m[5] * m[10] - m[6] * m[9]);

			inv[13] = m[0] * (m[9] * m[14] - m[10] * m[13]) -

				  m[8] * (m[1] * m[14] - m[2] * m[13]) -

				  m[12] * (m[2] * m[9] - m[1] * m[10]);

			inv[14] = m[0] * (m[6] * m[13] - m[5] * m[14]) -

				  m[4] * (m[2] * m[13] - m[1] * m[14]) -

				  m[12] * (m[1] * m[6] - m[2] * m[5]);

			inv[15] = m[0] * (m[5] * m[10] - m[6] * m[9]) -

				  m[4] * (m[1] * m[10] - m[2] * m[9]) -

				  m[8] * (m[2] * m[5] - m[1] * m[6]);

			theDet = m[0] * inv[0] +

				 m[1] * inv[4] +

				 m[2] * inv[8] +

				 m[3] * inv[12];
			if (theDet == 0)
			{
				return false;
			}

			double aDiv = (double)1.0 / theDet;
			for (int i = 0; i < 16; ++i)
			{
				inv[i] *= aDiv;
			}
			return true;
		}

		//! Return inverted matrix.
		public NCollection_Mat4 Inverted()
		{
			NCollection_Mat4 anInv;
			if (!Inverted(out anInv))
			{
				throw new Exception("NCollection_Mat4::Inverted() - matrix has zero determinant");
			}
			return anInv;
		}

		internal void Multiply(NCollection_Mat4 theMat)
		{

			var r = Multiply(this, theMat);
			Array.Copy(r.myMat, r.myMat, 16);

		}
		//! Multiply by the vector (M * V).
		//! @param theVec [in] the vector to multiply.
		public static NCollection_Vec4 operator *(NCollection_Mat4 mat, NCollection_Vec4 theVec)
		{
			return new NCollection_Vec4(
			  mat.GetValue(0, 0) * theVec.x() + mat.GetValue(0, 1) * theVec.y() + mat.GetValue(0, 2) * theVec.z() + mat.GetValue(0, 3) * theVec.w(),
			  mat.GetValue(1, 0) * theVec.x() + mat.GetValue(1, 1) * theVec.y() + mat.GetValue(1, 2) * theVec.z() + mat.GetValue(1, 3) * theVec.w(),
			  mat.GetValue(2, 0) * theVec.x() + mat.GetValue(2, 1) * theVec.y() + mat.GetValue(2, 2) * theVec.z() + mat.GetValue(2, 3) * theVec.w(),
			  mat.GetValue(3, 0) * theVec.x() + mat.GetValue(3, 1) * theVec.y() + mat.GetValue(3, 2) * theVec.z() + mat.GetValue(3, 3) * theVec.w());
		}

		public static NCollection_Mat4 operator *(NCollection_Mat4 mat, NCollection_Mat4 theMat)
		{			
			return NCollection_Mat4.Multiply(mat, theMat);
		}

		

		


		internal void Translate(NCollection_Vec3 theVec)
		{
			NCollection_Mat4 aTempMat = new NCollection_Mat4();
			aTempMat.SetColumn(3, theVec);
			Multiply(aTempMat);
		}
		double[] MyIdentityArray =
		 {1, 0, 0, 0,
   0, 1, 0, 0,
   0, 0, 1, 0,
   0, 0, 0, 1};

		public double[] myMat;
		public void InitIdentity()
		{
			myMat = new double[16];
			Array.Copy(MyIdentityArray, myMat, 16);
		}

		internal void ChangeValue(int theCol, int theRow, double val)
		{
			myMat[theCol * 4 + theRow] = val;
		}

		internal SetCollectionItem ChangeValue(int theCol, int theRow)
		{
			SetCollectionItem ret = new SetCollectionItem() { Column = theCol, Row = theRow };
			ret.Mat = this;
			return ret;
		}

		public class SetCollectionItem
		{
			public int Column;
			public int Row;
			public NCollection_Mat4 Mat;
			public void Assign(double v)
			{
				Mat.myMat[Column * 4 + Row] = v;
			}
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

		//! Take values from NCollection_Mat4 with a different element type with type conversion.

		public void ConvertFrom(NCollection_Mat4 theFrom)
		{
			for (int anIdx = 0; anIdx < 16; ++anIdx)
			{
				myMat[anIdx] = (theFrom.myMat[anIdx]);
			}
		}


	}
}
