using System;

namespace OCCPort
{
    public class Poly_Triangulation
    {
        //! Returns mesh purpose bits.
        public Poly_MeshPurpose MeshPurpose() { return myPurpose; }

        //! Sets mesh purpose bits.
        public void SetMeshPurpose(Poly_MeshPurpose thePurpose) { myPurpose = thePurpose; }


        Poly_MeshPurpose myPurpose;


    }
}
