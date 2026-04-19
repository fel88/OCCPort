using System;

namespace OCCPort
{
    public class Graphic3d_LightSet
    {
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

        public int NbCastShadows()
        {
            throw new NotImplementedException();
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