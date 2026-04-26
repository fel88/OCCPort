namespace OCCPort.Interfaces
{
    //! Interface class representing discrete model of an edge.
    public interface IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {  //! Sets 3d curve associated with current edge.
        void SetCurve(IMeshData_Curve theCurve);
        //! Returns 3d curve associated with current edge.
        IMeshData_Curve GetCurve();
        //! Gets value of angular deflection for the discrete model.
        double GetAngularDeflection();

        //! Returns degenerative flag.
        //! By default equals to flag stored in topological shape.
        bool GetDegenerated();
        //! Returns TopoDS_Edge attached to model.
        public TopoDS_Edge GetEdge()
        {
            return TopoDS.Edge(GetShape());
        }
        //! By default equals to flag stored in topological shape.
        public bool GetSameParam();

        //! Returns same range flag.
        //! By default equals to flag stored in topological shape.
        public bool GetSameRange();
        //! Returns number of pcurves assigned to current edge.
        public int PCurvesNb();
        IMeshData_PCurve GetPCurve(IMeshData_Face myDFace, TopAbs_Orientation topAbs_Orientation);
        //! Returns true in case if the edge is free one, i.e. it does not have pcurves.
        bool IsFree();
        //! Adds discrete pcurve for the specified discrete face.
        IMeshData_PCurve AddPCurve(
            IMeshData_Face theDFace,
            TopAbs_Orientation theOrientation);
        //! Returns pcurve with the given index.
        IMeshData_PCurve GetPCurve(int aPCurveIt);
    }
}