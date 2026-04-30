using System;
using System.Linq;

namespace OCCPort
{
    public class NCollection_Vec3_float		
	{
		protected float[] v;
		//! Assign new values to the vector.
		public void SetValues(float theX,
                  float theY,
                  float theZ)
		{

			v[0] = theX;
			v[1] = theY;
			v[2] = theZ;
		}


		//! Compute maximum component of the vector.
		public double maxComp()
		{
			return v[0] > v[1] ? (v[0] > v[2] ? v[0] : v[2])
							   : (v[1] > v[2] ? v[1] : v[2]);
		}

		public NCollection_Vec3_float()
		{
			v = new float[3];
		}
		//! Computes the square of vector modulus (magnitude, length).
		//! This method may be used for performance tricks.
		public double SquareModulus()
		{
			return x() * x() + y() * y() + z() * z();
		}

		public NCollection_Vec3_float(float value1, float value2, float value3)
		{
			v = new float[3];

			v[0] = value1;
			v[1] = value2;
			v[2] = value3;
		}

		public NCollection_Vec3_float(float[] myRgb)
		{
			v = myRgb.Cast<float>().ToArray();
		}

		public static NCollection_Vec3_float operator -(NCollection_Vec3_float temp)
		{
			return new NCollection_Vec3_float(-temp.x(), -temp.y(), -temp.z());
		}

		//! Compute per-component subtraction.
		public static NCollection_Vec3_float operator -(NCollection_Vec3_float temp, NCollection_Vec3_float temp2)
		{
			return new NCollection_Vec3_float(temp.x() - temp2.x(), temp.y() - temp2.y(), temp.z() - temp2.z());
		}




		internal static NCollection_Vec3_float Cross(NCollection_Vec3_float theVec1, NCollection_Vec3_float theVec2)
		{
			return new NCollection_Vec3_float(theVec1.y() * theVec2.z() - theVec1.z() * theVec2.y(),
			theVec1.z() * theVec2.x() - theVec1.x() * theVec2.z(),
			theVec1.x() * theVec2.y() - theVec1.y() * theVec2.x());

		}

		public void Normalize()
		{
            float aModulus = Modulus();
			if (aModulus != (0.0)) // just avoid divide by zero
			{
				v[0] = x() / aModulus;
				v[1] = y() / aModulus;
				v[2] = z() / aModulus;
			}
		}


        //! Computes the vector modulus (magnitude, length).
        public float Modulus()
		{
			return (float)Math.Sqrt(x() * x() + y() * y() + z() * z());
		}


		internal float x()
		{
			return v[0];
		}
		internal float y()
		{
			return v[1];
		}
		internal float z()
		{
			return v[2];
		}
	}


}
