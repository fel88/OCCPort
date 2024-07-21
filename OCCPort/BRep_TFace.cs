using System;
using System.Linq;

namespace OCCPort
{
    internal class BRep_TFace : TopoDS_TFace
    {

        public BRep_TFace()
        {
            myLocation = new TopLoc_Location();
        }

        //! Returns current active triangulation.
        public Poly_Triangulation ActiveTriangulation() { return myActiveTriangulation; }
        //! Returns face surface.
        public Geom_Surface Surface() { return mySurface; }
        public Poly_Triangulation Triangulation(Poly_MeshPurpose thePurpose)
        {
            if (thePurpose == Poly_MeshPurpose.Poly_MeshPurpose_NONE)
            {
                return ActiveTriangulation();
            }
            foreach (var aTriangulation in myTriangulations)
            {
                //Poly_Triangulation aTriangulation = anIter.Value();
                if ((aTriangulation.MeshPurpose() & thePurpose) != 0)
                {
                    return aTriangulation;
                }
            }

            if ((thePurpose & Poly_MeshPurpose.Poly_MeshPurpose_AnyFallback) != 0
              && !myTriangulations.IsEmpty())
            {
                // if none matching other criteria was found return the first defined triangulation
                return myTriangulations.First();
            }
            Poly_Triangulation anEmptyTriangulation = new Poly_Triangulation();
            return anEmptyTriangulation;
        }

        //=======================================================================
        //function : Triangulation
        //purpose  :
        //=======================================================================
        void Triangulation(Poly_Triangulation theTriangulation,
                                 bool theToReset)
        {
            if (theToReset || theTriangulation == null)
            {
                if (myActiveTriangulation != null)
                {
                    // Reset Active bit
                    myActiveTriangulation.SetMeshPurpose(myActiveTriangulation.MeshPurpose() & ~Poly_MeshPurpose.Poly_MeshPurpose_Active);
                    myActiveTriangulation = null;
                }
                myTriangulations.Clear();
                if (theTriangulation != null)
                {
                    // Reset list of triangulations to new list with only one input triangulation that will be active
                    myTriangulations.Append(theTriangulation);
                    myActiveTriangulation = theTriangulation;
                    // Set Active bit
                    theTriangulation.SetMeshPurpose(theTriangulation.MeshPurpose() | Poly_MeshPurpose.Poly_MeshPurpose_Active);
                }
                return;
            }
            for (int i = 0; i < myTriangulations.Count(); i++)
            {
                var item = myTriangulations[i];


                // Make input triangulation active if it is already contained in list of triangulations
                if (item == theTriangulation)
                {
                    if (myActiveTriangulation != null)
                    {
                        // Reset Active bit
                        myActiveTriangulation.SetMeshPurpose(myActiveTriangulation.MeshPurpose() & ~Poly_MeshPurpose.Poly_MeshPurpose_Active);
                    }
                    myActiveTriangulation = theTriangulation;
                    // Set Active bit
                    theTriangulation.SetMeshPurpose(theTriangulation.MeshPurpose() | Poly_MeshPurpose.Poly_MeshPurpose_Active);
                    return;
                }
            }

            for (int i = 0; i < myTriangulations.Count(); i++)
            {
                var item = myTriangulations[i];
                // Replace active triangulation to input one
                if (item == myActiveTriangulation)
                {
                    // Reset Active bit
                    //   myActiveTriangulation.SetMeshPurpose(myActiveTriangulation->MeshPurpose() & ~Poly_MeshPurpose_Active);
                    myTriangulations.Set(i, theTriangulation);
                    myActiveTriangulation = theTriangulation;
                    // Set Active bit
                    //  theTriangulation.SetMeshPurpose(theTriangulation->MeshPurpose() | Poly_MeshPurpose_Active);
                    return;
                }
            }

        }


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
        public TopLoc_Location Location() { return myLocation; }

    }

}