﻿namespace OCCPort
{
	public class IMeshTools_Parameters
	{

        //! Default constructor
        public IMeshTools_Parameters()

        {
            MeshAlgo = IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_DEFAULT;
            Angle = (0.5);
            Deflection = (0.001);
            AngleInterior = (-1.0);
            DeflectionInterior = (-1.0);
            MinSize = (-1.0);
    /*InParallel(Standard_False),
    Relative(Standard_False),
    InternalVerticesMode(Standard_True),
    ControlSurfaceDeflection(Standard_True),
    EnableControlSurfaceDeflectionAllSurfaces(Standard_False),
    CleanModel(Standard_True),
    AdjustMinSize(Standard_False),
    ForceFaceDeflection(Standard_False),
    AllowQualityDecrease(Standard_False)*/
        }

        //! Angular deflection used to tessellate the boundary edges
        public double Angle;

        //!Linear deflection used to tessellate the boundary edges
        public double Deflection;

        //! Angular deflection used to tessellate the face interior
        public double AngleInterior;

        //! Linear deflection used to tessellate the face interior
        public double DeflectionInterior;

        //! Minimum size parameter limiting size of triangle's edges to prevent 
        //! sinking into amplification in case of distorted curves and surfaces.
        public double MinSize;

        //! Switches on/off multi-thread computation
        public  bool InParallel;

        //! Switches on/off relative computation of edge tolerance<br>
        //! If true, deflection used for the polygonalisation of each edge will be 
        //! <defle> * Size of Edge. The deflection used for the faces will be the 
        //! maximum deflection of their edges.
        public bool Relative;

        //! Mode to take or not to take internal face vertices into account
        //! in triangulation process
        public double InternalVerticesMode;

        //! Parameter to check the deviation of triangulation and interior of
        //! the face
        public double ControlSurfaceDeflection;

        // Enables/disables check triggered by ControlSurfaceDeflection flag 
        // for all types of surfaces including analytical.
        public double EnableControlSurfaceDeflectionAllSurfaces;

        //! Cleans temporary data model when algorithm is finished.
        public double CleanModel;

        //! Enables/disables local adjustment of min size depending on edge size.
        //! Disabled by default.
        public double AdjustMinSize;

        //! Enables/disables usage of shape tolerances for computing face deflection.
        //! Disabled by default.
        public double ForceFaceDeflection;

        //! Allows/forbids the decrease of the quality of the generated mesh
        //! over the existing one.
        public double AllowQualityDecrease;
        //! 2D Delaunay triangulation algorithm factory to use
        public IMeshTools_MeshAlgoType MeshAlgo { get; set; }
	}
}