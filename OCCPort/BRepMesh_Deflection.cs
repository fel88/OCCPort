using System;

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
    }

}