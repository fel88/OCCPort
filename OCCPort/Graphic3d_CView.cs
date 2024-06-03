using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class Graphic3d_CView
    {
        internal void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
        {
            aSetOfStructures = new Graphic3d_MapOfStructure[0];
        }

        internal Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }
    }
}