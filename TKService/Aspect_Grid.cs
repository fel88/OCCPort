using TKernel;

namespace TKService
{
    public abstract class Aspect_Grid
    {
        //! activates the grid. The Hit method will return
        //! gridx and gridx computed according to the steps
        //! of the grid.
        public void Activate() { myIsActive = true; }

        //! Returns TRUE when the grid is active.
        public bool IsActive() { return myIsActive; }

        //! Updates the grid parameters.
        public abstract   void UpdateDisplay() ;
        //! Display the grid at screen.
        public abstract   void Display() ;
  
        //! Erase the grid from screen.
        public abstract   void Erase() ;

        //! deactivates the grid. The hit method will return
        //! gridx and gridx as the enter value X & Y.
        public void Deactivate() { myIsActive = false; }

        public void SetDrawMode(Aspect_GridDrawMode theDrawMode)
        {
            myDrawMode = theDrawMode;
            UpdateDisplay();
        }
        Aspect_GridDrawMode myDrawMode;

        double myRotationAngle;
        double myXOrigin;
        double myYOrigin;
        //Quantity_Color myColor;
        //	Quantity_Color myTenthColor;
        bool myIsActive;
        //Aspect_GridDrawMode myDrawMode;

        public Aspect_Grid(double theXOrigin = 0,
                                double theYOrigin = 0,
                                double theAngle = 0,
                                Quantity_Color? theColor = null,
                                Quantity_Color? theTenthColor = null)
        {
            if (theColor == null)
                theColor = new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY50);

            if (theTenthColor == null)
                theTenthColor = new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY70);

            myRotationAngle = (theAngle);
            myXOrigin = (theXOrigin);
            myYOrigin = (theYOrigin);
            //myColor = (theColor);
            //myTenthColor = (theTenthColor);
            myIsActive = (false);
            //myDrawMode = (Aspect_GDM_Lines);

            //
        }



    }
}
