using System;

namespace OCCPort
{

    public enum PrsMgr_DisplayStatus
    {

        //To give the display status of an Interactive Object.

        PrsMgr_DisplayStatus_Displayed,

        //the Interactive Object is displayed in the main viewer
        PrsMgr_DisplayStatus_Erased,

        //the Interactive Object is hidden in main viewer
        PrsMgr_DisplayStatus_None,

        //the Interactive Object is nowhere displayed
        AIS_DS_Displayed,
        AIS_DS_Erased,
        AIS_DS_None
    }
}