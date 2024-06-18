using System;
using System.Reflection;

namespace OCCPort
{
    public abstract class Graphic3d_Group
    {
        /*public Graphic3d_Group(Graphic3d_StructureManager m)
        {
            myStructureManager = m;
        }*/
        Graphic3d_TransformPers myTrsfPers; //!< current transform persistence
        protected Graphic3d_Structure myStructure;     //!< pointer to the parent structure
        //Graphic3d_BndBox4f myBounds;        //!< bounding box
        bool myIsClosed;      //!< flag indicating closed volume
							  //! Adds an array of primitives for display




		Graphic3d_BndBox4f myBounds;        //!< bounding box



		public void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives thePrim, Graphic3d_IndexBuffer graphic3d_IndexBuffer, bool theToEvalMinMax = true)
		{

			if (IsDeleted()
			|| !thePrim.IsValid())
			{
				return;
			}

			AddPrimitiveArray(thePrim.Type(), thePrim.Indices(), thePrim.Attributes(), thePrim.Bounds(), theToEvalMinMax);
		}

        public virtual void AddPrimitiveArray(Graphic3d_TypeOfPrimitiveArray theType,
                                                Graphic3d_IndexBuffer theIndices,
                                                Graphic3d_Buffer theAttribs,
                                                Graphic3d_BoundBuffer theBounds,
												bool theToEvalMinMax = true)
		{
			//(void)theType;
			if (IsDeleted()
			 || theAttribs == null)
			{
				return;
			}

			if (!theToEvalMinMax)
			{
				Update();
				return;
			}

			int aNbVerts = theAttribs.NbElements;
			int anAttribIndex = 0;
			int anAttribStride = 0;
			byte[] aDataPtr = theAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, anAttribIndex, anAttribStride);
			if (aDataPtr == null)
			{
				Update();
				return;
			}

			switch (theAttribs.Attribute(anAttribIndex).DataType)
			{
				case Graphic3d_TypeOfData.Graphic3d_TOD_VEC2:
					{
						for (int aVertIter = 0; aVertIter < aNbVerts; ++aVertIter)
        {
							//const Graphic3d_Vec2&aVert = *reinterpret_cast <const Graphic3d_Vec2* > (aDataPtr + anAttribStride * aVertIter);
							Graphic3d_Vec2 aVert = new Graphic3d_Vec2();

                            myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), 0.0f, 1.0f));
						}
						break;
					}
				case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3:
				case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4:
					{
						for (int aVertIter = 0; aVertIter < aNbVerts; ++aVertIter)
						{
							Graphic3d_Vec3 aVert = new Graphic3d_Vec3();
							//(Graphic3d_Vec3)(aDataPtr + anAttribStride * aVertIter);
							myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), aVert.z(), 1.0f));
						}
						break;
					}
				default: break;
			}
			Update();
		}

		private void Update()
		{
			throw new NotImplementedException();
		}

		private bool IsDeleted()
		{
			throw new NotImplementedException();
		}

        internal void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives aTriFreeEdges)
        {
            throw new NotImplementedException();
        }

        internal void SetPrimitivesAspect(object value)
        {
            throw new NotImplementedException();
        }
    }

	public enum Graphic3d_TypeOfData
	{

		//Type of the element in Vertex or Index Buffer.
		//Enumerator
		Graphic3d_TOD_USHORT,

		//unsigned 16-bit integer
		Graphic3d_TOD_UINT,

		//unsigned 32-bit integer
		Graphic3d_TOD_VEC2,

		//2-components float vector
		Graphic3d_TOD_VEC3,

		//3-components float vector
		Graphic3d_TOD_VEC4,

		//4-components float vector
		Graphic3d_TOD_VEC4UB,

		//4-components unsigned byte vector
		Graphic3d_TOD_FLOAT,

		//float value
        }

	enum Graphic3d_TypeOfAttribute
	{
		Graphic3d_TOA_POS = 0, Graphic3d_TOA_NORM, Graphic3d_TOA_UV, Graphic3d_TOA_COLOR,
		Graphic3d_TOA_CUSTOM
    }
}