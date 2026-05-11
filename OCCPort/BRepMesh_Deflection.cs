using OCCPort.Interfaces;
using System;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Auxiliary tool encompassing methods to compute deflection of shapes.
    public class BRepMesh_Deflection
    {
        //! Checks if the deflection of current polygonal representation
        //! is consistent with the required deflection.
        //! @param theCurrent [in] Current deflection.
        //! @param theRequired [in] Required deflection.
        //! @param theAllowDecrease [in] Flag controlling the check. If decrease is allowed,
        //! to be consistent the current and required deflections should be approximately the same.
        //! If not allowed, the current deflection should be less than required.
        //! @param theRatio [in] The ratio for comparison of the deflections (value from 0 to 1).
        internal static bool IsConsistent(double theCurrent, double theRequired, bool theAllowDecrease, double theRatio = 0.1)
        {
            // Check if the deflection of existing polygonal representation
            // fits the required deflection.
            bool isConsistent = theCurrent < (1.0 + theRatio) * theRequired
                   && (!theAllowDecrease || theCurrent > (1.0 - theRatio) * theRequired);
            return isConsistent;
        }


        //! Returns absolute deflection for theShape with respect to the 
        //! relative deflection and theMaxShapeSize.
        //! @param theShape shape for that the deflection should be computed.
        //! @param theRelativeDeflection relative deflection.
        //! @param theMaxShapeSize maximum size of the whole shape.
        //! @return absolute deflection for the shape.
        public static double ComputeAbsoluteDeflection(
            TopoDS_Shape theShape,
            double theRelativeDeflection,
            double theMaxShapeSize)
        {
            if (theShape.IsNull())
                return theRelativeDeflection;

            Bnd_Box aBox = new Bnd_Box();
            BRepBndLib.Add(theShape, aBox, false);

            double aShapeSize = theRelativeDeflection;
            BRepMesh_ShapeTool.BoxMaxDimension(aBox, ref aShapeSize);

            // Adjust resulting value in relation to the total size

            double aX1, aY1, aZ1, aX2, aY2, aZ2;
            aBox.Get(out aX1, out aY1, out aZ1, out aX2, out aY2, out aZ2);
            double aMaxShapeSize = (theMaxShapeSize > 0.0) ? theMaxShapeSize :
                                                 Math.Max(aX2 - aX1, Math.Max(aY2 - aY1, aZ2 - aZ1));

            double anAdjustmentCoefficient = aMaxShapeSize / (2 * aShapeSize);
            if (anAdjustmentCoefficient < 0.5)
            {
                anAdjustmentCoefficient = 0.5;
            }
            else if (anAdjustmentCoefficient > 2.0)
            {
                anAdjustmentCoefficient = 2.0;
            }

            return (anAdjustmentCoefficient * aShapeSize * theRelativeDeflection);
        }


        //================================================
        // Function: ComputeDeflection (edge)
        // Purpose : 
        //=======================================================================
        public static void ComputeDeflection(
          IMeshData_Edge theDEdge,
          double theMaxShapeSize,
          IMeshTools_Parameters theParameters)
        {
            double aAngDeflection = theParameters.Angle;
            double aLinDeflection =
              !theParameters.Relative ? theParameters.Deflection :
              ComputeAbsoluteDeflection(theDEdge.GetEdge(),
                                        theParameters.Deflection,
                                        theMaxShapeSize);

            TopoDS_Edge anEdge = theDEdge.GetEdge();

            TopoDS_Vertex aFirstVertex = new TopoDS_Vertex(), aLastVertex = new TopoDS_Vertex();
            TopExp.Vertices(anEdge, ref aFirstVertex, ref aLastVertex);

            Geom_Curve aCurve = null;
            double aFirstParam = 0, aLastParam = 0;
            if (BRepMesh_ShapeTool.Range(anEdge, out aCurve, ref aFirstParam, ref aLastParam))
            {
                double aDistF = aFirstVertex.IsNull() ? -1.0 :
                                    BRep_Tool.Pnt(aFirstVertex).Distance(aCurve.Value(aFirstParam));
                double aDistL = aLastVertex.IsNull() ? -1.0 :
                                    BRep_Tool.Pnt(aLastVertex).Distance(aCurve.Value(aLastParam));

                double aVertexAdjustDistance = Math.Max(aDistF, aDistL);

                aLinDeflection = Math.Max(aVertexAdjustDistance, aLinDeflection);
            }

            theDEdge.SetDeflection(aLinDeflection);
            theDEdge.SetAngularDeflection(aAngDeflection);
        }

    }

}