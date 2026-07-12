namespace TKV3d
{
    //! Declares the type of Interactive Object.
    //! This type can be used for fast pre-filtering of objects of specific group.
    public enum AIS_KindOfInteractive
    {
        AIS_KindOfInteractive_None,        //!< object of unknown type
        AIS_KindOfInteractive_Datum,       //!< presentation of construction element (datum)
                                           //!  such as points, lines, axes and planes
        AIS_KindOfInteractive_Shape,       //!< presentation of topological shape
        AIS_KindOfInteractive_Object,      //!< presentation of group of topological shapes
        AIS_KindOfInteractive_Relation,    //!< presentation of relation  (dimensions and constraints)
        AIS_KindOfInteractive_Dimension,   //!< presentation of dimension (length, radius, diameter and angle)
        AIS_KindOfInteractive_LightSource, //!< presentation of light source

        // old aliases
        AIS_KOI_None = AIS_KindOfInteractive_None,
        AIS_KOI_Datum = AIS_KindOfInteractive_Datum,
        AIS_KOI_Shape = AIS_KindOfInteractive_Shape,
        AIS_KOI_Object = AIS_KindOfInteractive_Object,
        AIS_KOI_Relation = AIS_KindOfInteractive_Relation,
        AIS_KOI_Dimension = AIS_KindOfInteractive_Dimension
    };

}

