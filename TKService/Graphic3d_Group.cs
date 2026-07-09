using OCCPort.Common;
using TKernel;

namespace TKService
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
            var aDataPtr = theAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, ref anAttribIndex, ref anAttribStride);
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
                            Graphic3d_Vec2 aVert = new Graphic3d_Vec2(aDataPtr.Data, aDataPtr.Offset + offset, new FloatExtractor());

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
                            Graphic3d_Vec3 aVert = BinaryHelper.Get_Vec3(aDataPtr.Data, aDataPtr.Offset + offset);

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

        public void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives thePrim, bool theToEvalMinMax = true)
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

    //! Graphic3d_ArrayFlags bitmask values.
    [Flags]
    public enum Graphic3d_ArrayFlags : int
    {
        Graphic3d_ArrayFlags_None = 0x00,  //!< no flags
        Graphic3d_ArrayFlags_VertexNormal = 0x01,  //!< per-vertex normal attribute
        Graphic3d_ArrayFlags_VertexColor = 0x02,  //!< per-vertex color  attribute
        Graphic3d_ArrayFlags_VertexTexel = 0x04,  //!< per-vertex texel coordinates (UV) attribute
        Graphic3d_ArrayFlags_BoundColor = 0x10,
        // advanced
        Graphic3d_ArrayFlags_AttribsMutable = 0x20,  //!< mutable array, which can be invalidated during lifetime without re-creation
        Graphic3d_ArrayFlags_AttribsDeinterleaved = 0x40,  //!< non-interleaved vertex attributes packed into single array
        Graphic3d_ArrayFlags_IndexesMutable = 0x80,  //!< mutable index array, which can be invalidated during lifetime without re-creation
    };
    public class BinaryHelper
    {
        public static Graphic3d_Vec3 Get_Vec3(byte[] aDataPtr, int offset)
        {
            return new Graphic3d_Vec3(BitConverter.ToSingle(aDataPtr, offset),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float)),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float) * 2));


        }
    }

    //! Redefines BVH_Box<Standard_ShortReal, 4> for AABB representation
    //! Describes rendering parameters and effects.

    public class Graphic3d_BndBox4f : BVH_Box<Graphic3d_Vec4, Graphic3d_Vec4_BoxMinMax>//<float,4>
    {

    }
    public static class ByteHelper
    {

    }

    public class FloatExtractor : TKernel.IElementBinaryExtractor<float>
    {
        public float Get(byte[] data, int idx)
        {
            var offset = idx;//??
            return BitConverter.ToSingle(data, offset);
        }
    }

    //! Definition of line types
    public enum Aspect_TypeOfLine
    {
        Aspect_TOL_EMPTY = -1, //!< hidden
        Aspect_TOL_SOLID = 0, //!< continuous
        Aspect_TOL_DASH,       //!< dashed 2.0,1.0 (MM)
        Aspect_TOL_DOT,        //!< dotted 0.2,0.5 (MM)
        Aspect_TOL_DOTDASH,    //!< mixed  10.0,1.0,2.0,1.0 (MM)
        Aspect_TOL_USERDEFINED //!< defined by Users
    };


    public class BVH_Box<BVH_VecNt, MinMax> where BVH_VecNt : struct
       where MinMax : IBoxMinMax<BVH_VecNt>, new()
    {

        public BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?
                                   //! Returns minimum point of bounding box.

        MinMax minMax = new MinMax();


        //! Appends new point to the bounding box.
        public void Add(BVH_VecNt thePoint)
        {
            if (!myIsInited)
            {
                myMinPoint = thePoint;
                myMaxPoint = thePoint;
                myIsInited = true;
            }
            else
            {
                myMinPoint = minMax.CwiseMin(myMinPoint, thePoint);
                myMaxPoint = minMax.CwiseMax(myMaxPoint, thePoint);
            }
        }


        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }

        public void Combine(BVH_Box<BVH_VecNt, MinMax> theBox)
        {
            if (theBox.myIsInited)
            {
                if (!myIsInited)
                {
                    myMinPoint = theBox.myMinPoint;
                    myMaxPoint = theBox.myMaxPoint;
                    myIsInited = true;
                }
                else
                {
                    myMinPoint = minMax.CwiseMin(myMinPoint, theBox.myMinPoint);
                    myMaxPoint = minMax.CwiseMax(myMaxPoint, theBox.myMaxPoint);
                }
            }
        }

        //! Is bounding box valid?
        public bool IsValid() { return myIsInited; }
    }


    //! Tool class for calculate component-wise vector minimum
    //! and maximum (optimized version).
    //! \tparam T Numeric data type
    //! \tparam N Vector dimension
    public interface IBoxMinMax<BVH_VecNt>
    {

        BVH_VecNt CwiseMin(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMin(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{

        //    theVec1.X = Math.Min(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
        //}

        BVH_VecNt CwiseMax(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMax(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{
        //    theVec1.X = Math.Max(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
        //}
    }


    //! Buffer of vertex attributes.
    //! This class is intended for advanced usage allowing invalidation of entire buffer content or its sub-part.
    class Graphic3d_AttribBuffer : Graphic3d_Buffer
    {
        // =======================================================================
        public Graphic3d_AttribBuffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
            myIsInterleaved = (true);
            myIsMutable = false;
        }

        //! Return number of initially allocated elements which can fit into this buffer,
        //! while NbElements can be overwritten to smaller value.
        public int NbMaxElements() { return Stride != 0 ? (int)(mySize / (int)(Stride)) : 0; }

        public void SetInterleaved(bool theIsInterleaved)
        {
            if (NbMaxElements() != 0)
            {
                throw new Standard_ProgramError("Graphic3d_AttribBuffer::SetInterleaved() should not be called for allocated buffer");
            }
            myIsInterleaved = theIsInterleaved;
        }
        public void SetMutable(bool theMutable)
        {
            if (mySize > Standard_Integer.IntegerLast()
             && theMutable)
            {
                throw new Standard_OutOfRange("Graphic3d_AttribBuffer::SetMutable(), Mutable flag cannot be used for buffer exceeding 32-bit address space");
            }
            myIsMutable = theMutable;
        }

        Graphic3d_BufferRange myInvalidatedRange; //!< invalidated buffer data range (as byte offsets)
        bool myIsInterleaved;    //!< flag indicating the vertex attributes being interleaved
        bool myIsMutable;        //!< flag indicating that data can be invalidated
    }

    internal class Graphic3d_BufferRange
    {
    }

    //! Interior types for primitive faces.
    public enum Aspect_InteriorStyle
    {
        Aspect_IS_EMPTY = -1, //!< no interior
        Aspect_IS_SOLID = 0, //!< normally filled surface interior
        Aspect_IS_HATCH,      //!< hatched surface interior
        Aspect_IS_HIDDENLINE, //!< interior is filled with viewer background color
        Aspect_IS_POINT,      //!< display only vertices of surface (obsolete)

        // obsolete aliases
        Aspect_IS_HOLLOW = Aspect_IS_EMPTY, //!< transparent surface interior
    };


    public enum Graphic3d_TypeOfShadingModel
    {
        //! Use Shading Model, specified as default for entire Viewer.
        Graphic3d_TypeOfShadingModel_DEFAULT = -1,

        //! Unlit Shading (or shadeless), lighting is ignored and facet is fully filled by its material color.
        //! This model is useful for artificial/auxiliary objects, not intended to be lit,
        //! or for objects with pre-calculated lighting information (e.g. captured by camera).
        Graphic3d_TypeOfShadingModel_Unlit = 0,

        //! Flat Shading for Phong material model, calculated using triangle normal.
        //! Could be useful for mesh element analysis.
        //! This shading model does NOT require normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_PhongFacet,

        //! Gouraud shading uses the same material definition as Phong reflection model,
        //! but emulates an obsolete per-vertex calculations with result color interpolated across fragments,
        //! as implemented by T&L hardware blocks on old graphics hardware.
        //! This shading model requires normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_Gouraud,

        //! Phong reflection model, an empirical model defined by Diffuse/Ambient/Specular/Shininess components.
        //! Lighting is calculated per-fragment basing on nodal normal (normal is interpolated across fragments of triangle).
        //! This shading model requires normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_Phong,

        //! Metallic-roughness physically based (PBR) illumination system.
        Graphic3d_TypeOfShadingModel_Pbr,

        //! Same as Graphic3d_TypeOfShadingModel_Pbr but using flat per-triangle normal.
        Graphic3d_TypeOfShadingModel_PbrFacet,

        // obsolete aliases
        Graphic3d_TOSM_DEFAULT = Graphic3d_TypeOfShadingModel_DEFAULT,
        Graphic3d_TOSM_UNLIT = Graphic3d_TypeOfShadingModel_Unlit,
        Graphic3d_TOSM_FACET = Graphic3d_TypeOfShadingModel_PhongFacet,
        Graphic3d_TOSM_VERTEX = Graphic3d_TypeOfShadingModel_Gouraud,
        Graphic3d_TOSM_FRAGMENT = Graphic3d_TypeOfShadingModel_Phong,
        Graphic3d_TOSM_PBR = Graphic3d_TypeOfShadingModel_Pbr,
        Graphic3d_TOSM_PBR_FACET = Graphic3d_TypeOfShadingModel_PbrFacet,
        //
        Graphic3d_TOSM_NONE = Graphic3d_TOSM_UNLIT,
        V3d_COLOR = Graphic3d_TOSM_NONE,
        V3d_FLAT = Graphic3d_TOSM_FACET,
        V3d_GOURAUD = Graphic3d_TOSM_VERTEX,
        V3d_PHONG = Graphic3d_TOSM_FRAGMENT
    }



    public enum Graphic3d_AlphaMode
    {

        //Defines how alpha value of base color / texture should be treated.

        Graphic3d_AlphaMode_Opaque,

        //rendered output is fully opaque and alpha value is ignored
        Graphic3d_AlphaMode_Mask,

        //rendered output is either fully opaque or fully transparent depending on the alpha value and the alpha cutoff value
        Graphic3d_AlphaMode_Blend,

        //rendered output is combined with the background
        Graphic3d_AlphaMode_MaskBlend,
        //performs in-place blending (without implicit reordering of opaque objects) with alpha-test
        Graphic3d_AlphaMode_BlendAuto,

        //special value defined for backward compatibility - it is equal to Graphic3d_AlphaMode_Blend when Material transparency is not zero and Graphic3d_AlphaMode_Opaque otherwise;
    }

    internal class Graphic3d_MutableIndexBuffer : Graphic3d_IndexBuffer
    {
        public Graphic3d_MutableIndexBuffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
        }
    }

    //! Nature of the reflection of a material.
    public enum Graphic3d_TypeOfReflection
    {
        Graphic3d_TOR_AMBIENT = 0,
        Graphic3d_TOR_DIFFUSE,
        Graphic3d_TOR_SPECULAR,
        Graphic3d_TOR_EMISSION
    };

    public class Graphic3d_Vec4_BoxMinMax : IBoxMinMax<Graphic3d_Vec4>
    {
        public void CwiseMax(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Max(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMax(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            var ret = new Graphic3d_Vec4();
            ret.X = Math.Max(theVec1.X, theVec2.X);
            ret.Y = Math.Max(theVec1.Y, theVec2.Y);
            ret.Z = Math.Max(theVec1.Z, theVec2.Z);
            ret.W = Math.Max(theVec1.W, theVec2.W);
            return ret;
        }

        public void CwiseMin(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Min(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMin(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            var ret = new Graphic3d_Vec4();
            ret.X = Math.Min(theVec1.X, theVec2.X);
            ret.Y = Math.Min(theVec1.Y, theVec2.Y);
            ret.Z = Math.Min(theVec1.Z, theVec2.Z);
            ret.W = Math.Min(theVec1.W, theVec2.W);
            return ret;
        }
    }

}
