using OCCPort.Tester;
using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    internal class StdPrs_ToolTriangulatedShape : BRepLib_ToolTriangulatedShape

    {  //! If presentation has own deviation coefficient and IsAutoTriangulation() is true,
       //! function will compare actual coefficients with previous values and will clear triangulation on their change
       //! (regardless actual tessellation quality).
       //! Function is placed here for compatibility reasons - new code should avoid using IsAutoTriangulation().
       //! @param theShape  [in] the shape
       //! @param theDrawer [in] the display settings
       //! @param theToResetCoeff [in] updates coefficients in theDrawer to actual state to avoid redundant recomputations
        public static void ClearOnOwnDeflectionChange(TopoDS_Shape theShape,
                                                                 Prs3d_Drawer theDrawer,
                                                                 bool theToResetCoeff)
        {
            if (!theDrawer.IsAutoTriangulation()
              || theShape.IsNull())
            {
                return;
            }

            bool isOwnDeviationAngle = theDrawer.HasOwnDeviationAngle();
            bool isOwnDeviationCoefficient = theDrawer.HasOwnDeviationCoefficient();
            double anAngleNew = theDrawer.DeviationAngle();
            double anAnglePrev = theDrawer.PreviousDeviationAngle();
            double aCoeffNew = theDrawer.DeviationCoefficient();
            double aCoeffPrev = theDrawer.PreviousDeviationCoefficient();
            if ((!isOwnDeviationAngle || Math.Abs(anAngleNew - anAnglePrev) <= Precision.Angular())
             && (!isOwnDeviationCoefficient || Math.Abs(aCoeffNew - aCoeffPrev) <= Precision.Confusion()))
            {
                return;
            }

            BRepTools.Clean(theShape);
            if (theToResetCoeff)
            {
                theDrawer.UpdatePreviousDeviationAngle();
                theDrawer.UpdatePreviousDeviationCoefficient();
            }
        }

        public static double GetDeflection(TopoDS_Shape theShape, Prs3d_Drawer theDrawer)
        {
            if (theDrawer.TypeOfDeflection() != Aspect_TypeOfDeflection.Aspect_TOD_RELATIVE)
            {
                return theDrawer.MaximalChordialDeviation();
            }

            Bnd_Box aBndBox = new Bnd_Box();
            BRepBndLib.Add(theShape, aBndBox, false);
            if (aBndBox.IsVoid())
            {
                return theDrawer.MaximalChordialDeviation();
            }
            else if (aBndBox.IsOpen())
            {
                if (!aBndBox.HasFinitePart())
                {
                    return theDrawer.MaximalChordialDeviation();
                }
                aBndBox = aBndBox.FinitePart();
            }

            // store computed relative deflection of shape as absolute deviation coefficient in case relative type to use it later on for sub-shapes
            double aDeflection = Prs3d.GetDeflection(aBndBox, theDrawer.DeviationCoefficient(), theDrawer.MaximalChordialDeviation());
            theDrawer.SetMaximalChordialDeviation(aDeflection);
            return aDeflection;
        }

        internal static void ComputeNormals(TopoDS_Face aFace, Poly_Triangulation aT)
        {
            throw new NotImplementedException();
        }

        internal static bool Tessellate(TopoDS_Shape theShape, Prs3d_Drawer theDrawer)
        {
            bool wasRecomputed = false;
            // Check if it is possible to avoid unnecessary recomputation of shape triangulation
            if (IsTessellated(theShape, theDrawer))
            {
                return wasRecomputed;
            }

            double aDeflection = GetDeflection(theShape, theDrawer);

            // retrieve meshing tool from Factory


            BRepMesh_DiscretRoot aMeshAlgo = BRepMesh_DiscretFactory.Discret(theShape,
                                                                                             aDeflection,
                                                                                             theDrawer.DeviationAngle());

            if (aMeshAlgo != null)
            {
                aMeshAlgo.Perform();
                wasRecomputed = true;
            }

            return wasRecomputed;
        }

        private static bool IsTessellated(TopoDS_Shape theShape, Prs3d_Drawer theDrawer)
        {
            return BRepTools.Triangulation(theShape, GetDeflection(theShape, theDrawer), true);
        }
    }
}