using System.Collections.Generic;

namespace OCCPort
{
    public abstract class Graphic3d_CView : Graphic3d_DataStructureManager

    {
        protected Graphic3d_TextureMap myBackgroundImage;

        protected Graphic3d_CubeMap    myCubeMapBackground;  //!< Cubemap displayed at background
        protected Graphic3d_StructureManager myStructureManager;
        protected Graphic3d_Camera myCamera;
        protected Graphic3d_SequenceOfStructure myStructsToCompute;
        protected Graphic3d_SequenceOfStructure myStructsComputed;
        protected Graphic3d_MapOfStructure myStructsDisplayed;
        protected bool myIsInComputedMode;
        protected bool myIsActive;
        protected bool myIsRemoved;
        protected Graphic3d_TypeOfBackfacingModel myBackfacing;
        protected Graphic3d_TypeOfVisualization myVisualization;


        protected Graphic3d_TextureEnv myTextureEnvData;
        protected Graphic3d_GraduatedTrihedron myGTrihedronData;
        protected Graphic3d_TypeOfBackground myBackgroundType;     //!< Current type of background
        protected Aspect_SkydomeBackground mySkydomeAspect;
        protected bool myToUpdateSkydome;


        //! Redraw content of the view.
        public abstract void Redraw();

        //! Redraw immediate content of the view.
        public abstract void RedrawImmediate();

        //! Invalidates content of the view but does not redraw it.
        public abstract void Invalidate();

        public List<Graphic3d_MapOfStructure> Items = new List<Graphic3d_MapOfStructure>();

        internal void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
        {
            aSetOfStructures = Items.ToArray();
        }
        public abstract Graphic3d_Layer[] Layers();


        public abstract Aspect_Window Window();


        int myId;
        protected Graphic3d_RenderingParams myRenderParams;

        public virtual bool IsDefined()
        {
            return true;
        }

        internal Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }

    }
}