using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class Graphic3d_CView: Graphic3d_DataStructureManager

    {
        internal void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
        {
            aSetOfStructures = new Graphic3d_MapOfStructure[0];
        }

        internal void Invalidate()
        {
            
        }

        public virtual bool IsDefined()
        {
            return true;
        }

        internal Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }
    }
}