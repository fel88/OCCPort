using OCCPort.Common;
using System.Reflection.Metadata;
using TKernel;
using TKMath;

namespace TKService
{
    //! Low-level graphic structure interface
    public abstract class Graphic3d_CStructure
    {
        public Graphic3d_BndBox3d myBndBox;
        internal int highlight;
        public int visible;
        int myId;
        Graphic3d_SequenceOfHClipPlane myClipPlanes;

        //! Set transformation persistence.
        public virtual void SetTransformPersistence(Graphic3d_TransformPers theTrsfPers) { myTrsfPers = theTrsfPers; }



        //! Set z layer ID to display the structure in specified layer
        public virtual void SetZLayer(Graphic3d_ZLayerId theLayerIndex) { myZLayer = theLayerIndex; }

        //! Pass clip planes to the associated graphic driver structure
        public void SetClipPlanes(Graphic3d_SequenceOfHClipPlane thePlanes) { myClipPlanes = thePlanes; }

        //! Return transformation.
        public TopLoc_Datum3D Transformation() { return myTrsf; }

        TopLoc_Datum3D myTrsf;
        //! Connect other structure to this one
        public abstract void Connect(Graphic3d_CStructure theStructure);

        //! Return structure visibility flag
        public bool IsVisible() { return visible != 0; }
        //! Assign transformation.
        public virtual void SetTransformation(TopLoc_Datum3D theTrsf) { myTrsf = theTrsf; }

        public bool IsInfinite { get; internal set; }
        public bool IsForHighlight;
        public bool IsMutable;
        public bool Is2dText;


        protected bool myIsCulled; //!< A status specifying is structure needs to be rendered after BVH tree traverse
        protected bool myBndBoxClipCheck;  //!< Flag responsible for checking of bounding box clipping before drawing of object

        protected bool myHasGroupTrsf;     //!< flag specifying that some groups might have transform persistence


        //! Returns FALSE if the structure hits the current view volume, otherwise returns TRUE.
        public bool IsCulled() { return myIsCulled; }

        public Graphic3d_ViewAffinity ViewAffinity; //!< view affinity mask

        //! Return structure visibility considering both View Affinity and global visibility state.
        public bool IsVisible(int theViewId)
        {
            return visible != 0
                && (ViewAffinity == null
                 || ViewAffinity.IsVisible(theViewId));
        }

        public Graphic3d_CStructure(Graphic3d_StructureManager theManager)
        {
            myGraphicDriver = (theManager.GraphicDriver());
            myId = (-1);
            myZLayer = Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default;
            /*
			myPriority(Graphic3d_DisplayPriority_Normal),
			myPreviousPriority(Graphic3d_DisplayPriority_Normal),
			myIsCulled(Standard_True),
			myBndBoxClipCheck(Standard_True),
			myHasGroupTrsf(Standard_False),*/
            //
            IsInfinite = false;
            stick = (0);
            highlight = (0);
            visible = (1);
            /*HLRValidation(0),
			IsForHighlight(Standard_False),*/
            IsMutable = false;/*
			Is2dText(Standard_False)*/


            myId = myGraphicDriver.NewIdentification();
        }

        protected Graphic3d_GraphicDriver myGraphicDriver;

        //! @return bounding box of this presentation

        public Graphic3d_BndBox3d BoundingBox()
        {
            return myBndBox;
        }

        protected Graphic3d_SequenceOfGroup myGroups = new Graphic3d_SequenceOfGroup();
        protected Graphic3d_TransformPers myTrsfPers;
        internal int stick;
        public int Identification()
        {
            return myId;
        }

        internal Graphic3d_TransformPers TransformPersistence()
        {
            return myTrsfPers;
        }

        public abstract Graphic3d_Group NewGroup(Graphic3d_Structure prs3d_Presentation);

        internal Graphic3d_SequenceOfGroup Groups()
        {
            return myGroups;
        }

        internal void OnVisibilityChanged()
        {
            throw new NotImplementedException();
        }

        internal void SetGroupTransformPersistence(bool v)
        {
            //throw new NotImplementedException();
        }

        //! Get z layer ID
        public Graphic3d_ZLayerId ZLayer() { return myZLayer; }

        Graphic3d_ZLayerId myZLayer;

        internal bool IsAlwaysRendered()
        {
            return IsInfinite
                || IsForHighlight
                || IsMutable
                || Is2dText
                || (myTrsfPers != null && myTrsfPers.IsTrihedronOr2d());

        }

        internal void MarkAsNotCulled()
        {

        }
        //! Return structure display priority.
        public Graphic3d_DisplayPriority Priority() { return myPriority; }

        internal void updateLayerTransformation()
        {
            throw new NotImplementedException();
        }

        internal void ChangeBoundingBox(Graphic3d_BndBox3d aBox)
        {
            myBndBox = aBox;
        }
        Graphic3d_PresentationAttributes myHighlightStyle; //! Current highlight style; is set only if highlight flag is true


        //! Returns valid handle to highlight style of the structure in case if
        //! highlight flag is set to true
        public Graphic3d_PresentationAttributes HighlightStyle()  { return myHighlightStyle; }


        Graphic3d_DisplayPriority myPriority;
        Graphic3d_DisplayPriority myPreviousPriority;


    }
    public class Graphic3d_SequenceOfGroup : List<Graphic3d_Group>
    {


        public bool IsEmpty()
        {
            return Count == 0;
        }
        public void Append(Graphic3d_Group aGroup)
        {
            Add(aGroup);
        }

        internal Graphic3d_Group Last()
        {
            return this[this.Count - 1];
        }
        //typedef NCollection_Sequence<Handle(Graphic3d_Group)> Graphic3d_SequenceOfGroup;
    }

    //! Structure display state.
    public class Graphic3d_ViewAffinity
    {
        //! Empty constructor.
        public Graphic3d_ViewAffinity()
        {
            SetVisible(true);
        }

        //! Return visibility flag.
        public bool IsVisible(int theViewId)
        {
            int aBit = 1 << theViewId;
            return (myMask & aBit) != 0;
        }

        //! Setup visibility flag.
        public void SetVisible(int theViewId,
                    bool theIsVisible)
        {
            uint aBit = (uint)1 << theViewId;
            if (theIsVisible)
            {
                myMask |= aBit;
            }
            else
            {
                myMask &= ~aBit;
            }
        }

        public void SetVisible(bool theIsVisible)
        {

            myMask = (uint)(theIsVisible ? 0xFF : 0x00);

        }
        uint myMask; //!< affinity mask

    }

    public class Graphic3d_TransformPers
    {
        internal bool IsTrihedronOr2d(Graphic3d_TransModeFlags theMode)
        {
            return (theMode & (Graphic3d_TransModeFlags.Graphic3d_TMF_TriedronPers | Graphic3d_TransModeFlags.Graphic3d_TMF_2d)) != 0;

        }
        //! Return true for Graphic3d_TMF_ZoomPers, Graphic3d_TMF_ZoomRotatePers or Graphic3d_TMF_RotatePers modes.
        public bool IsZoomOrRotate() { return IsZoomOrRotate(myMode); }
        //! Return true if specified mode is zoom/rotate transformation persistence.
        public static bool IsZoomOrRotate(Graphic3d_TransModeFlags theMode)
        {
            return (theMode & (Graphic3d_TransModeFlags.Graphic3d_TMF_ZoomPers | Graphic3d_TransModeFlags.Graphic3d_TMF_RotatePers)) != 0;
        }
        //template<class T>
        public void Apply<T, MinMax>(Graphic3d_Camera theCamera,
                                             NCollection_Mat4<double> theProjection,
                                             NCollection_Mat4<double> theWorldView,
                                             int theViewportWidth,
                                             int theViewportHeight,
                                           ref BVH_Box<T, MinMax> theBoundingBox) where T : struct
            where MinMax : IBoxMinMax<T>, new()
        {
            //NCollection_Mat4<T> aTPers = Compute(theCamera, theProjection, theWorldView, theViewportWidth, theViewportHeight);
            //if (aTPers.IsIdentity()
            //|| !theBoundingBox.IsValid())
            //{
            //    return;
            //}

            //const typename BVH_Box<T, 3 >::BVH_VecNt & aMin = theBoundingBox.CornerMin();
            //const typename BVH_Box<T, 3 >::BVH_VecNt & aMax = theBoundingBox.CornerMax();

            //typename BVH_Box<T, 4 >::BVH_VecNt anArrayOfCorners[8];
            //anArrayOfCorners[0] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMin.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[1] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMin.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[2] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMax.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[3] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMax.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[4] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMin.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[5] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMin.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[6] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMax.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[7] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMax.y(), aMax.z(), static_cast<T>(1.0));

            //theBoundingBox.Clear();
            //for (int anIt = 0; anIt < 8; ++anIt)
            //{
            //    typename BVH_Box<T, 4 >::BVH_VecNt & aCorner = anArrayOfCorners[anIt];
            //    aCorner = aTPers * aCorner;
            //    aCorner = aCorner / aCorner.w();
            //    theBoundingBox.Add(typename BVH_Box < T, 3 >::BVH_VecNt(aCorner.x(), aCorner.y(), aCorner.z()));
            //}
        }

        //! Return the anchor point for zoom/rotate transformation persistence.
        public gp_Pnt AnchorPoint()
        {
            if (!IsZoomOrRotate())
            {
                throw new Standard_ProgramError("Graphic3d_TransformPers::AnchorPoint(), wrong persistence mode.");
            }

            return new gp_Pnt(myParams.Params3d.PntX, myParams.Params3d.PntY, myParams.Params3d.PntZ);
        }

        //! 3D anchor point for zoom/rotate transformation persistence.
        public struct PersParams3d
        {
            public double PntX;
            public double PntY;
            public double PntZ;

        }
        public struct PersParams2d
        {
            public int OffsetX;
            public int OffsetY;
            public Aspect_TypeOfTriedronPosition Corner;

        }
        //union
        public class MyParamsUnion
        {
            //  {
            public PersParams3d Params3d;
            public PersParams2d Params2d;
            //  }
        }
        MyParamsUnion myParams = new MyParamsUnion();
        Graphic3d_TransModeFlags myMode;  //!< Transformation persistence mode flags

        //! Return true for Graphic3d_TMF_TriedronPers and Graphic3d_TMF_2d modes.
        public bool IsTrihedronOr2d() { return IsTrihedronOr2d(myMode); }
    }

    //! Transform Persistence Mode defining whether to lock in object position, rotation and / or zooming relative to camera position.
    public enum Graphic3d_TransModeFlags
    {
        Graphic3d_TMF_None = 0x0000,                  //!< no persistence attributes (normal 3D object)
        Graphic3d_TMF_ZoomPers = 0x0002,                  //!< object does not resize
        Graphic3d_TMF_RotatePers = 0x0008,                  //!< object does not rotate;
        Graphic3d_TMF_TriedronPers = 0x0020,                  //!< object behaves like trihedron - it is fixed at the corner of view and does not resizing (but rotating)
        Graphic3d_TMF_2d = 0x0040,                  //!< object is defined in 2D screen coordinates (pixels) and does not resize, pan and rotate
        Graphic3d_TMF_CameraPers = 0x0080,                  //!< object is in front of the camera
        Graphic3d_TMF_ZoomRotatePers = Graphic3d_TMF_ZoomPers
                                     | Graphic3d_TMF_RotatePers //!< object doesn't resize and rotate
    };


    internal class Aspect_GenId
    {
        int last = 0;
        internal int Next()
        {
            return last++;
            int aNewId = 0;
            //if (!Next(aNewId))
            {
                throw new Aspect_IdentDefinitionError("Aspect_GenId::Next(), Error: Available == 0");
            }
            return aNewId;

        }

        int myFreeCount;
        int myLength;
        int myLowerBound;
        int myUpperBound;

        public Aspect_GenId()
        {

        }

        public Aspect_GenId(int theLow,
                            int theUpper)
        {

            myFreeCount = (theUpper - theLow + 1);
            myLength = (theUpper - theLow + 1);
            myLowerBound = (theLow);
            myUpperBound = (theUpper);
            if (theLow > theUpper)
            {
                throw new Aspect_IdentDefinitionError("GenId Create Error: wrong interval");
            }
        }

        List<int> myFreeIds = new List<int>();
        //bool Next(ref int theId)
        //{
        //	if (!myFreeIds.IsEmpty())
        //	{
        //		theId = myFreeIds.First();
        //		myFreeIds.RemoveFirst();
        //		return true;
        //	}
        //	else if (myFreeCount < 1)
        //	{
        //		return false;
        //	}

        //	--myFreeCount;
        //	theId = myLowerBound + myLength - myFreeCount - 1;
        //	return Standard_True;
        //}



    }

    public class Aspect_DisplayConnection
    {
    }
}



