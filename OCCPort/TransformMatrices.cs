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

		//! Invalidate orientation.
		public void ResetOrientation() { myIsOrientationValid = false; }

		//! Invalidate projection.
		public void ResetProjection() { myIsProjectionValid = false; }



		//! Return true if Orientation was not invalidated.
		public bool IsOrientationValid() { return myIsOrientationValid; }

		//! Return true if Projection was not invalidated.
		public bool IsProjectionValid() { return myIsProjectionValid; }

		public NCollection_Mat4 MProjection;
		public NCollection_Mat4 LProjection;
		public NCollection_Mat4 RProjection;

		//! Initialize projection.
		public void InitProjection()
		{
			myIsProjectionValid = true;
			MProjection = new NCollection_Mat4();
			LProjection = new NCollection_Mat4();
			RProjection = new NCollection_Mat4();
			MProjection.InitIdentity();
			LProjection.InitIdentity();
			RProjection.InitIdentity();
		}
	}
}