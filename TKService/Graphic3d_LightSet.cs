using OCCPort.Common;
using System;
using System.Reflection.Metadata;
using TKernel;

namespace TKService
{
    //! Class defining the set of light sources.
    public class Graphic3d_LightSet
    {
        public Graphic3d_LightSet()
        {
            //myAmbient=new   (0.0f, 0.0f, 0.0f, 0.0f),
            myNbEnabled = (0);
            myNbCastShadows = 0;
            myRevision = (1);
            //myCacheRevision(0)

            // memset(myLightTypes, 0, sizeof(myLightTypes));
            //    memset(myLightTypesEnabled, 0, sizeof(myLightTypesEnabled));
        }


        NCollection_IndexedDataMap<Graphic3d_CLight, int> myLights = new NCollection_IndexedDataMap<Graphic3d_CLight, int>();              //!< list of light sources with their cached state (revision)
        int[] myLightTypes = new int[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_NB]; //!< counters per each light source type defined in the list
        public bool Add(Graphic3d_CLight theLight)
        {
            if (theLight == null)
            {
                throw new Standard_ProgramError("Graphic3d_LightSet::Add(), NULL argument");
            }

            int anOldExtent = myLights.Extent();
            int anIndex = myLights.Add(theLight, 0);
            if (anIndex <= anOldExtent)
            {
                return false;
            }

            myLightTypes[(int)theLight.Type()] += 1;
            myLights.ChangeFromIndex(anIndex, theLight.Revision());
            ++myRevision;
            return true;
        }

        int myRevision;               //!< current revision of light source set

        //! Returns total amount of enabled lights EXCLUDING ambient.
        public int NbEnabled() { return myNbEnabled; }
        //! Returns a string defining a list of enabled light sources as concatenation of letters 'd' (Directional), 'p' (Point), 's' (Spot)
        //! depending on the type of light source in the list.
        //! Example: "dppp".
        //! @sa UpdateRevision()
        public string KeyEnabledLong() { return myKeyEnabledLong; }

        //! Returns a string defining a list of enabled light sources as concatenation of letters 'd' (Directional), 'p' (Point), 's' (Spot)
        //! depending on the type of light source in the list, specified only once.
        //! Example: "dp".
        //! @sa UpdateRevision()
        public string KeyEnabledShort() { return myKeyEnabledShort; }

        //! Returns total amount of enabled lights castings shadows.
        //! @sa UpdateRevision()
        public int NbCastShadows()
        {
            return myNbCastShadows;
        }

        //int myLightTypes[Graphic3d_TypeOfLightSource_NB]; //!< counters per each light source type defined in the list
        //int myLightTypesEnabled[Graphic3d_TypeOfLightSource_NB]; //!< counters per each light source type enabled in the list
        int myNbEnabled;              //!< number of enabled light sources, excluding ambient
        int myNbCastShadows;          //!< number of enabled light sources casting shadows
                                      //Standard_Size myRevision;               //!< current revision of light source set
                                      //Standard_Size myCacheRevision;          //!< revision of cached state

        string myKeyEnabledLong;         //!< key identifying the list of enabled light sources by their type
        string myKeyEnabledShort;        //!< key identifying the list of enabled light sources by the number of sources of each type
    }

}
