using OCCPort;
using System.Xml.Linq;

namespace OCCPort
{
    public class Graphic3d_BoundBuffer
    {
        public Graphic3d_Vec4 Colors;      //!< pointer to facet color values
        public int[] Bounds;      //!< pointer to bounds array
        public int NbBounds;    //!< number of bounds
        public int NbMaxBounds; //!< number of allocated bounds
                                //! Empty constructor.
        public Graphic3d_BoundBuffer(/*NCollection_BaseAllocator theAlloc*/)
        {
            //NCollection_Buffer(theAlloc);
            Colors = (null);
            Bounds = (null);
            NbBounds = (0);
            NbMaxBounds = (0);
        }


        //! Allocates new empty array
        public bool Init(int theNbBounds,
              bool theHasColors)
        {
            Colors = null;
            Bounds = null;
            NbBounds = 0;
            NbMaxBounds = 0;
            //   Free();
            if (theNbBounds < 1)
            {
                return false;
            }

            /*  int aBoundsSize = sizeof(Standard_Integer) * theNbBounds;
              int aColorsSize = theHasColors
                                      ? sizeof(Graphic3d_Vec4) * theNbBounds
                                      : 0;*/
            // if (!Allocate(aColorsSize + aBoundsSize))
            {
                //   Free();
                //     return false;
            }

            NbBounds = theNbBounds;
            NbMaxBounds = theNbBounds;
            //  Colors = theHasColors ? reinterpret_cast<Graphic3d_Vec4*>(myData) : NULL;
            //Bounds = reinterpret_cast<Standard_Integer*>(theHasColors ? (myData + aColorsSize) : myData);
            return true;
        }
    }
}