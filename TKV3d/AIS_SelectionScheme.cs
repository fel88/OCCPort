namespace TKV3d
{
    //! Sets selection schemes for interactive contexts.
    public enum AIS_SelectionScheme
    {
        AIS_SelectionScheme_UNKNOWN = -1, //!< undefined scheme
        AIS_SelectionScheme_Replace = 0,  //!< clears current selection and select detected objects
        AIS_SelectionScheme_Add,          //!< adds    detected object to current selection
        AIS_SelectionScheme_Remove,       //!< removes detected object from the current selection
        AIS_SelectionScheme_XOR,          //!< performs XOR for detected objects, other selected not touched
        AIS_SelectionScheme_Clear,        //!< clears current selection
        AIS_SelectionScheme_ReplaceExtra, //!< replace with one difference: if result of replace is an empty,
                                          //!< and current selection contains detected element, it will be selected
    };
}

