using System;

namespace OCCPort
{
	internal class TransformMatrices<T>
	{
		public TransformMatrices()
		{
			myIsOrientationValid = false;
			myIsProjectionValid = false;

		}


		bool myIsOrientationValid;
		bool myIsProjectionValid;

		//! Initialize orientation.
		public void InitOrientation()
		{
			myIsOrientationValid = true;
			Orientation = new Graphic3d_Mat4d();
			Orientation.InitIdentity();
		}



		public Graphic3d_Mat4d Orientation { get; internal set; }



		//! Return true if Orientation was not invalidated.
		public bool IsOrientationValid() { return myIsOrientationValid; }


		internal void ResetOrientation()
		{

		}

		internal void ResetProjection()
		{

		}
	}
}