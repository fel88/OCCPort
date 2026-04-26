using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_PCurve 
    {
        
        public AbstractMeshData_PCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
        {
            myOrientation = theOrientation;
            myDFace = theDFace;
        }

        protected IMeshData_Face myDFace;
        protected TopAbs_Orientation myOrientation;

        
    }
}