﻿using System;

namespace OCCPort
{
    internal class BRep_TFace : TopoDS_TFace
    {

        public BRep_TFace()
        {
            myLocation = new TopLoc_Location();
        }
        //! Returns face surface.
        public Geom_Surface Surface() { return mySurface; }

        //! Sets surface for this face.
        public void Surface(Geom_Surface theSurface) { mySurface = theSurface; }


        Poly_ListOfTriangulation myTriangulations;
        Poly_Triangulation myActiveTriangulation;
        Geom_Surface mySurface;
        TopLoc_Location myLocation;
        double myTolerance;
        bool myNaturalRestriction;

        //! Returns the face tolerance.
        public double Tolerance() { return myTolerance; }

        //! Sets the tolerance for this face.
        public void Tolerance(double theTolerance) { myTolerance = theTolerance; }

        //! Returns the face location.
        public  TopLoc_Location Location()  { return myLocation; }

}
}