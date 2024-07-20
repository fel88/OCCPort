using OCCPort.Tester;
using System;


namespace OCCPort
{
    public abstract class Graphic3d_CStructure
    {
        public Graphic3d_BndBox3d myBndBox;
        internal int highlight;
        public int visible;
        int myId;
        //! Return structure visibility flag
        public bool IsVisible() { return visible != 0; }

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
			IsForHighlight(Standard_False),
			IsMutable(Standard_False),
			Is2dText(Standard_False)*/


            myId = myGraphicDriver.NewIdentification();
        }

        Graphic3d_GraphicDriver myGraphicDriver;

        //! @return bounding box of this presentation

        internal Graphic3d_BndBox3d BoundingBox()
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

        Graphic3d_DisplayPriority myPriority;
        Graphic3d_DisplayPriority myPreviousPriority;

    }
}
