using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace OCCPort
{

    //! This class allows the definition of groups
    //! of primitives inside of graphic objects (presentations).
    //! A group contains the primitives and attributes
    //! for which the range is limited to this group.
    //! The primitives of a group can be globally suppressed.
    //!
    //! There are two main group usage models:
    //!
    //! 1) Non-modifiable, or unbounded, group ('black box').
    //! Developers can repeat a sequence of
    //! SetPrimitivesAspect() with AddPrimitiveArray() methods arbitrary number of times
    //! to define arbitrary number of primitive "blocks" each having individual apect values.
    //! Any modification of such a group is forbidden, as aspects and primitives are mixed
    //! in memory without any high-level logical structure, and any modification is very likely to result
    //! in corruption of the group internal data.
    //! It is necessary to recreate such a group as a whole when some attribute should be changed.
    //! (for example, in terms of AIS it is necessary to re-Compute() the whole presentation each time).
    //! 2) Bounded group. Developers should specify the necessary group aspects with help of
    //! SetGroupPrimitivesAspect() and then add primitives to the group.
    //! Such a group have simplified organization in memory (a single block of attributes
    //! followed by a block of primitives) and therefore it can be modified, if it is necessary to
    //! change parameters of some aspect that has already been set, using methods:
    //! IsGroupPrimitivesAspectSet() to detect which aspect was set for primitives;
    //! GroupPrimitivesAspect() to read current aspect values
    //! and SetGroupPrimitivesAspect() to set new values.
    //!
    //! Developers are strongly recommended to take all the above into account when filling Graphic3d_Group
    //! with aspects and primitives and choose the group usage model beforehand out of application needs.
    //! Note that some Graphic3d_Group class virtual methods contain only base implementation
    //! that is extended by the descendant class in OpenGl package.
    public abstract class Graphic3d_Group
    {
        public Graphic3d_Group(Graphic3d_Structure theStruct)
        {
            myStructure = theStruct;
            myIsClosed = (false);
        }
        //! Return transformation persistence.
        public Graphic3d_TransformPers TransformPersistence() { return myTrsfPers; }

        /*public Graphic3d_Group(Graphic3d_StructureManager m)
        {
            myStructureManager = m;
        }*/
        protected Graphic3d_TransformPers myTrsfPers; //!< current transform persistence
        protected Graphic3d_Structure myStructure;     //!< pointer to the parent structure
        //Graphic3d_BndBox4f myBounds;        //!< bounding box
        bool myIsClosed;      //!< flag indicating closed volume
                              //! Adds an array of primitives for display

        Graphic3d_BndBox4f myBounds = new Graphic3d_BndBox4f();        //!< bounding box

        //! Modifies the context for all the face primitives of the group.
        public abstract void SetGroupPrimitivesAspect(Graphic3d_Aspects theAspect);

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
            byte[] aDataPtr = theAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, ref anAttribIndex, ref anAttribStride);
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
                            var offset = anAttribStride * aVertIter;
                            Graphic3d_Vec2 aVert = new Graphic3d_Vec2(aDataPtr, offset);

                            myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), 0.0f, 1.0f));
                        }
                        break;
                    }
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3:
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4:
                    {
                        for (int aVertIter = 0; aVertIter < aNbVerts; ++aVertIter)
                        {
                            var offset = anAttribStride * aVertIter;
                            Graphic3d_Vec3 aVert = BinaryHelper.Get_Vec3(aDataPtr, offset);

                            myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), aVert.z(), 1.0f));
                        }
                        break;
                    }
                default: break;
            }
            Update();
        }

        public void Update()
        {
            if (IsDeleted())
            {
                return;
            }

            myStructure.StructureManager().Update();
        }


        //! Returns Standard_True if the group <me> is deleted.
        //! <me> is deleted after the call Remove (me) or the
        //! associated structure is deleted.
        public bool IsDeleted()
        {
            return myStructure == null || myStructure.IsDeleted();
        }

        internal void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives thePrim, bool theToEvalMinMax = true)
        {
            if (IsDeleted() || !thePrim.IsValid())
                return;

            AddPrimitiveArray(thePrim.Type(), thePrim.Indices(), thePrim.Attributes(), thePrim.Bounds(), theToEvalMinMax);
        }

        //! Modifies the current context of the group to give another aspect for all the primitives created after this call in the group.
        public abstract void SetPrimitivesAspect(Graphic3d_Aspects theAspect);

        //! Changes property shown that primitive arrays within this group form closed volume (do no contain open shells).
        public void SetClosed(bool theIsClosed) { myIsClosed = theIsClosed; }

        //! Return true if primitive arrays within this graphic group form closed volume (do no contain open shells).
        public bool IsClosed() { return myIsClosed; }
        //! Returns boundary box of the group <me> without transformation applied,
        public Graphic3d_BndBox4f BoundingBox()
        {
            return myBounds;
        }
    }
}