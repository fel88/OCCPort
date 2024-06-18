using System;

namespace OCCPort
{
	public class Graphic3d_ArrayOfPrimitives
	{
		internal Graphic3d_IndexBuffer Indices()
		{
			throw new NotImplementedException();
		}

		Graphic3d_IndexBuffer myIndices;
		Graphic3d_Buffer myAttribs;
		Graphic3d_BoundBuffer myBounds;

		//! Adds a vertice in the array.
		//! @return the actual vertex number
		public int AddVertex(gp_Pnt theVertex)
		{
			return AddVertex(theVertex.X(), theVertex.Y(), theVertex.Z());
		}
		//! Adds a vertice in the array.
		//! @return the actual vertex number
		public int AddVertex(double theX, double theY, double theZ)
		{
			return AddVertex(Standard_Real.RealToShortReal(theX), Standard_Real.RealToShortReal(theY), Standard_Real.RealToShortReal(theZ));
		}

		public int AddVertex(float theX, float theY, float theZ)
		{

			int anIndex = myAttribs.NbElements + 1;
			SetVertice(anIndex, theX, theY, theZ);
			return anIndex;
		}

		private void SetVertice(int anIndex, float theX, float theY, float theZ)
		{
			//Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > myAttribs->NbMaxElements(), "BAD VERTEX index");
		/*	Graphic3d_Vec3  aVec = *reinterpret_cast<Graphic3d_Vec3*>(myAttribs->ChangeData() + myPosStride * ((Standard_Size)theIndex - 1));
			aVec.x() = theX;
			aVec.y() = theY;
			aVec.z() = theZ;
			if (myAttribs->NbElements < theIndex)
			{
				myAttribs->NbElements = theIndex;
			}*/

		}

		internal bool IsValid()
		{
			throw new NotImplementedException();
		}

        internal Graphic3d_Buffer Attributes()
        {
            throw new NotImplementedException();
        }

        internal Graphic3d_BoundBuffer Bounds()
        {
            throw new NotImplementedException();
        }
        Graphic3d_TypeOfPrimitiveArray myType;
        //! Returns the type of this primitive
        public Graphic3d_TypeOfPrimitiveArray Type()  { return myType; }

}
}