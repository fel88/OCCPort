using System;

namespace OCCPort
{
    [Flags]
    enum MaskFlags
    {
        VoidMask = 0x01,
        XminMask = 0x02,
        XmaxMask = 0x04,
        YminMask = 0x08,
        YmaxMask = 0x10,
        ZminMask = 0x20,
        ZmaxMask = 0x40,
        WholeMask = 0x7e
    };
}