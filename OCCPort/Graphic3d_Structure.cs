using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace OCCPort
{
    public class Graphic3d_Structure
    {
        public Graphic3d_Structure()
        {
        }
        //
        //! Alias for porting code.
        //typedef Graphic3d_Structure Prs3d_Presentation;
        //! Returns the groups sequence included in this structure.
        public Graphic3d_SequenceOfGroup Groups()
        {
            return myCStructure.Groups();
        }
        //! Returns the visualisation mode for the structure <me>.
        public Graphic3d_TypeOfStructure Visual() { return myVisual; }
        Graphic3d_TypeOfStructure myVisual;

        //! Returns the display indicator for this structure.
        public virtual bool IsDisplayed()
        {
            return myCStructure != null
                && myCStructure.stick != 0;
        }
        //=============================================================================
        //function : StructureManager
        //purpose  :
        //=============================================================================
        public Graphic3d_StructureManager StructureManager()
        {
            return myStructureManager;

        }
        public virtual void Display()
        {

            if (IsDeleted()) return;

            if (myCStructure.stick == 0)
            {
                myCStructure.stick = 1;
                myStructureManager.Display(this);
            }

            if (myCStructure.visible != 1)
            {
                myCStructure.visible = 1;
                myCStructure.OnVisibilityChanged();
            }

        }
        public static void Network(Graphic3d_Structure theStructure,

                                        Graphic3d_TypeOfConnection theType,
                                       List<Graphic3d_Structure> theSet)
        {
            theSet.Add(theStructure);
        }


        public void SetVisible(bool theValue)
        {
            if (IsDeleted())
                return;

            int isVisible = theValue ? 1 : 0;
            if (myCStructure.visible == isVisible)
            {
                return;
            }

            myCStructure.visible = isVisible;
            myCStructure.OnVisibilityChanged();
            Update(true);
        }

        private void Update(bool theUpdateLayer)
        {
            if (IsDeleted())
            {
                return;
            }

            //myStructureManager.Update(theUpdateLayer ? myCStructure.ZLayer() : Graphic3d_ZLayerId_UNKNOWN);

        }


        //! Returns the highlight indicator for this structure.
        public bool IsHighlighted()
        {
            return myCStructure != null && myCStructure.highlight != 0;
        }

        //! Returns Standard_True if the structure <me> is infinite.
        internal bool IsInfinite()
        {
            return IsDeleted() || myCStructure.IsInfinite;
        }

        public bool IsDeleted()
        {
            return myCStructure == null;
        }

        internal bool IsVisible()
        {
            return myCStructure != null && myCStructure.visible != 0;
        }

        public Graphic3d_TransformPers TransformPersistence()
        {
            return myCStructure.TransformPersistence();
        }

        public Graphic3d_CStructure CStructure()
        {
            return myCStructure;
        }

        public Graphic3d_CStructure myCStructure;

        Graphic3d_StructureManager myStructureManager;

        public Graphic3d_Structure(Graphic3d_StructureManager theManager)
        {
            myStructureManager = theManager;
            myCStructure = theManager.GraphicDriver().CreateStructure(theManager);

        }

        internal Graphic3d_Group NewGroup()
        {
            return myCStructure.NewGroup(this);
        }

        internal void SetInfiniteState(bool v)
        {
            throw new NotImplementedException();
        }

        public virtual void Compute()
        {

        }
        public void SetIsForHighlight(bool isForHighlight)
        {
            if (myCStructure != null)
            {
                myCStructure.IsForHighlight = isForHighlight;
            }
        }

        public virtual void Clear(bool WithDestruction = true)
        {
            clear(WithDestruction);
        }

        public void clear(bool theWithDestruction)
        {
            if (IsDeleted())
                return;

            // clean groups in graphics driver at first
            GraphicClear(theWithDestruction);

            myCStructure.SetGroupTransformPersistence(false);
            myStructureManager.Clear(this, theWithDestruction);

            Update(true);
        }

        public void GraphicClear(bool theWithDestruction)
        {
            //throw new NotImplementedException();
        }

        internal void SetHLRValidation(bool v)
        {
            //throw new NotImplementedException();
        }

        public int Identification()
        {
            return myCStructure.Identification();
        }
        //! Returns the current display priority for this structure.
        public Graphic3d_DisplayPriority DisplayPriority()
        {
            return myCStructure.Priority();
        }

        //! Get Z layer ID of displayed structure.
        //! The method returns -1 if the structure has no ID (deleted from graphic driver).
        public Graphic3d_ZLayerId GetZLayer()
        {
            return myCStructure.ZLayer();
        }
    }

}
