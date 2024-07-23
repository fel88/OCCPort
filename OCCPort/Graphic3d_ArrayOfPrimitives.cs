using System;
using System.Security.AccessControl;

namespace OCCPort
{
	public class Graphic3d_ArrayOfPrimitives
	{
		public Graphic3d_ArrayOfPrimitives()
		{
		}
		internal Graphic3d_IndexBuffer Indices()
		{
			throw new NotImplementedException();
		}
		public Graphic3d_ArrayOfPrimitives(Graphic3d_TypeOfPrimitiveArray theType,
							   int theMaxVertexs,
							   int theMaxBounds,
							   int theMaxEdges,
							   Graphic3d_ArrayFlags theArrayFlags)
		{
			/*myNormData = (null);
            myTexData = (null);
            myColData = (null);
            myPosStride = (0);
            myNormStride = (0);
            myTexStride = (0);
            myColStride = (0);*/
			myType = Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_UNDEFINED;

			init(theType, theMaxVertexs, theMaxBounds, theMaxEdges, theArrayFlags);
		}

		//! Returns the number of defined vertex
		public int VertexNumber() { return myAttribs.NbElements; }

		void init(Graphic3d_TypeOfPrimitiveArray theType,
											int theMaxVertexs,
											int theMaxBounds,
											int theMaxEdges,
											Graphic3d_ArrayFlags theArrayOptions)
		{
			myType = theType;
			/*myNormData = NULL;
            myTexData = NULL;
            myColData = NULL;*/
			myAttribs = null;

			myIndices = null;/*
            myBounds.Nullify();*/

			NCollection_BaseAllocator anAlloc = Graphic3d_Buffer.DefaultAllocator();

			if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsMutable) != 0
			 || (theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsDeinterleaved) != 0)
			{
				Graphic3d_AttribBuffer anAttribs = new Graphic3d_AttribBuffer(anAlloc);
				anAttribs.SetMutable((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsMutable) != 0);
				anAttribs.SetInterleaved((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsDeinterleaved) == 0);
				myAttribs = anAttribs;
			}
			else
			{
				myAttribs = new Graphic3d_Buffer(anAlloc);
			}
			if (theMaxVertexs < 1)
			{
				return;
			}

			if (theMaxEdges > 0)
			{
				if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_IndexesMutable) != 0)
				{
					myIndices = new Graphic3d_MutableIndexBuffer(anAlloc);
				}
				else
				{
					myIndices = new Graphic3d_IndexBuffer(anAlloc);
				}
				if (theMaxVertexs < (int)(ushort.MaxValue))
				{
					/*if (!myIndices.Init(theMaxEdges))
					{
						myIndices = null;
						return;
					}*/
				}
				else
				{
				//	if (!myIndices->Init < unsigned int> (theMaxEdges))
				//{
				//		myIndices.Nullify();
				//		return;
				//	}
				}
				//          myIndices.NbElements = 0;
				//      }

				//      Graphic3d_Attribute []anAttribs=new Graphic3d_Attribute[4];
				//      in aNbAttribs = 0;
				//      anAttribs[aNbAttribs].Id = Graphic3d_TOA_POS;
				//      anAttribs[aNbAttribs].DataType = Graphic3d_TOD_VEC3;
				//      ++aNbAttribs;
				//      if ((theArrayOptions & Graphic3d_ArrayFlags_VertexNormal) != 0)
				//      {
				//          anAttribs[aNbAttribs].Id = Graphic3d_TOA_NORM;
				//          anAttribs[aNbAttribs].DataType = Graphic3d_TOD_VEC3;
				//          ++aNbAttribs;
				//      }
				//      if ((theArrayOptions & Graphic3d_ArrayFlags_VertexTexel) != 0)
				//      {
				//          anAttribs[aNbAttribs].Id = Graphic3d_TOA_UV;
				//          anAttribs[aNbAttribs].DataType = Graphic3d_TOD_VEC2;
				//          ++aNbAttribs;
				//      }
				//      if ((theArrayOptions & Graphic3d_ArrayFlags_VertexColor) != 0)
				//      {
				//          anAttribs[aNbAttribs].Id = Graphic3d_TOA_COLOR;
				//          anAttribs[aNbAttribs].DataType = Graphic3d_TOD_VEC4UB;
				//          ++aNbAttribs;
				//      }

				//      if (!myAttribs->Init(theMaxVertexs, anAttribs, aNbAttribs))
				//      {
				//          myAttribs.Nullify();
				//          myIndices.Nullify();
				//          return;
				//      }

				//      int anAttribDummy = 0;
				//      myAttribs->ChangeAttributeData(Graphic3d_TOA_POS, anAttribDummy, myPosStride);
				//      myNormData = myAttribs->ChangeAttributeData(Graphic3d_TOA_NORM, anAttribDummy, myNormStride);
				//      myTexData = myAttribs->ChangeAttributeData(Graphic3d_TOA_UV, anAttribDummy, myTexStride);
				//      myColData = myAttribs->ChangeAttributeData(Graphic3d_TOA_COLOR, anAttribDummy, myColStride);

				//      memset(myAttribs->ChangeData(), 0, size_t(myAttribs->Stride) * size_t(myAttribs->NbMaxElements()));
				//      if ((theArrayOptions & Graphic3d_ArrayFlags_AttribsMutable) == 0
				//       && (theArrayOptions & Graphic3d_ArrayFlags_AttribsDeinterleaved) == 0)
				//      {
				//          myAttribs->NbElements = 0;
			}

			if (theMaxBounds > 0)
			{
				//myBounds = new Graphic3d_BoundBuffer(anAlloc);
				myBounds = new Graphic3d_BoundBuffer();
				if (!myBounds.Init(theMaxBounds, (theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_BoundColor) != 0))
				{
					myAttribs = null;
					myIndices = null;
					myBounds = null;
					return;
				}
				myBounds.NbBounds = 0;
			}
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
			if (myAttribs == null)
			{
				return false;
			}

			int nvertexs = myAttribs.NbElements;
			int nbounds = myBounds == null ? 0 : myBounds.NbBounds;
			int nedges = myIndices == null ? 0 : myIndices.NbElements;
			switch (myType)
			{
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POINTS:
					if (nvertexs < 1)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYLINES:
					if (nedges > 0
					 && nedges < 2)
					{
						return false;
					}
					if (nvertexs < 2)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_SEGMENTS:
					if (nvertexs < 2)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYGONS:
					if (nedges > 0
					 && nedges < 3)
					{
						return false;
					}
					if (nvertexs < 3)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES:
					if (nedges > 0)
					{
						if (nedges < 3
						 || nedges % 3 != 0)
						{
							if (nedges <= 3)
							{
								return false;
							}
							myIndices.NbElements = 3 * (nedges / 3);
						}
					}
					else if (nvertexs < 3
						  || nvertexs % 3 != 0)
					{
						if (nvertexs <= 3)
						{
							return false;
						}
						myAttribs.NbElements = 3 * (nvertexs / 3);
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLES:
					if (nedges > 0)
					{
						if (nedges < 4
						 || nedges % 4 != 0)
						{
							if (nedges <= 4)
							{
								return false;
							}
							myIndices.NbElements = 4 * (nedges / 4);
						}
					}
					else if (nvertexs < 4
						  || nvertexs % 4 != 0)
					{
						if (nvertexs <= 4)
						{
							return false;
						}
						myAttribs.NbElements = 4 * (nvertexs / 4);
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLEFANS:
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLESTRIPS:
					if (nvertexs < 3)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLESTRIPS:
					if (nvertexs < 4)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINES_ADJACENCY:
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINE_STRIP_ADJACENCY:
					if (nvertexs < 4)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES_ADJACENCY:
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLE_STRIP_ADJACENCY:
					if (nvertexs < 6)
					{
						return false;
					}
					break;
				case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_UNDEFINED:
				default:
					return false;
			}

			// total number of edges(vertices) in bounds should be the same as variable
			// of total number of defined edges(vertices); if no edges - only vertices
			// could be in bounds.
			if (nbounds > 0)
			{
				int n = 0;
				for (int aBoundIter = 0; aBoundIter < nbounds; ++aBoundIter)
				{
					n += myBounds.Bounds[aBoundIter];
				}
				if (nedges > 0
				 && n != nedges)
				{
					if (nedges <= n)
					{
						return false;
					}
					myIndices.NbElements = n;
				}
				else if (nedges == 0
					  && n != nvertexs)
				{
					if (nvertexs <= n)
					{
						return false;
					}
					myAttribs.NbElements = n;
				}
			}

			// check that edges (indexes to an array of vertices) are in range.
			if (nedges > 0)
			{
				for (int anEdgeIter = 0; anEdgeIter < nedges; ++anEdgeIter)
				{
					if (myIndices.Index(anEdgeIter) >= myAttribs.NbElements)
					{
						return false;
					}
				}
			}
			return true;
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
		public Graphic3d_TypeOfPrimitiveArray Type() { return myType; }

	}

}