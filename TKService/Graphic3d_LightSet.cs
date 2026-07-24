using OCCPort.Common;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
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

        //! Returns cumulative ambient color, which is computed as sum of all enabled ambient light sources.
        //! Values are NOT clamped (can be greater than 1.0f) and alpha component is fixed to 1.0f.
        //! @sa UpdateRevision()
        public Graphic3d_Vec4 AmbientColor() { return myAmbient; }

        int myCacheRevision;          //!< revision of cached state
        Graphic3d_Vec4 myAmbient = new NCollection_Vec4<float>();                //!< cached value of cumulative ambient color

        //! Return TRUE if lights list is empty.
        public bool IsEmpty() { return myLights.IsEmpty(); }

        public int UpdateRevision()
        {
            if (myCacheRevision == myRevision)
            {
                // check implicit updates of light sources
                for (NCollection_IndexedDataMap<Graphic3d_CLight, int>.Iterator aLightIter = new NCollection_IndexedDataMap<Graphic3d_CLight, int, NCollection_DefaultHasher<Graphic3d_CLight>>.Iterator(myLights); aLightIter.More(); aLightIter.Next())
                {
                    Graphic3d_CLight aLight = aLightIter.Key();
                    if (aLightIter.Value() != aLight.Revision())
                    {
                        ++myRevision;
                        break;
                    }
                }
            }
            if (myCacheRevision == myRevision)
            {
                return myRevision;
            }

            myCacheRevision = myRevision;
            myNbCastShadows = 0;
            myAmbient.SetValues(0.0f, 0.0f, 0.0f, 0.0f);
            //memset(myLightTypesEnabled, 0, sizeof(myLightTypesEnabled));
            char[] aKeyLong = new char[(myLights.Extent() + 1)];
            int aLightLast = 0;
            for (NCollection_IndexedDataMap<Graphic3d_CLight, int>.Iterator aLightIter = new NCollection_IndexedDataMap<Graphic3d_CLight, int, NCollection_DefaultHasher<Graphic3d_CLight>>.Iterator(myLights); aLightIter.More(); aLightIter.Next())
            {
                Graphic3d_CLight aLight = aLightIter.Key();
                aLightIter.ChangeValue(aLight.Revision());
                if (!aLight.IsEnabled())
                    continue;

                myLightTypesEnabled[(int)aLight.Type()] += 1;
                if (aLight.Type() == Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient)
                {
                    myAmbient += aLight.PackedColor() * aLight.Intensity();
                }
                else
                {
                    if (aLight.ToCastShadows())
                    {
                        ++myNbCastShadows;
                        aKeyLong[aLightLast++] = UpperCase(THE_LIGHT_KEY_LETTERS[(int)aLight.Type()]);
                    }
                    else
                    {
                        aKeyLong[aLightLast++] = THE_LIGHT_KEY_LETTERS[(int)aLight.Type()];
                    }
                }
            }
            aKeyLong[aLightLast] = '\0';
            myAmbient.a(1.0f);
            myNbEnabled = myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional]
                        + myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional]
                        + myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot];
            myKeyEnabledLong = new string(aKeyLong);
            myKeyEnabledShort = "" + (myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional] > 0 ? THE_LIGHT_KEY_LETTERS[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional] : '\0')
                              + (myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional] > 0 ? THE_LIGHT_KEY_LETTERS[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional] : '\0')
                              + (myLightTypesEnabled[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot] > 0 ? THE_LIGHT_KEY_LETTERS[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot] : '\0');
            return myRevision;
        }

        private char UpperCase(char v)
        {
            return char.ToUpper(v);
        }

        //! Suffixes identifying light source type.
        static char[] THE_LIGHT_KEY_LETTERS =
        {
    'a', // Graphic3d_TypeOfLightSource_Ambient
    'd', // Graphic3d_TypeOfLightSource_Directional
    'p', // Graphic3d_TypeOfLightSource_Positional
    's'  // Graphic3d_TypeOfLightSource_Spot
  };
        int[] myLightTypesEnabled = new int[(int)Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_NB]; //!< counters per each light source type enabled in the list

        //! Returns total amount of enabled lights of specified type.
        //! @sa UpdateRevision()
        public int NbEnabledLightsOfType(Graphic3d_TypeOfLightSource theType) { return myLightTypesEnabled[(int)theType]; }

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

        public class Iterator
        {
            public Iterator(Graphic3d_LightSet theSet, IterationFilter theFilter)
            {
                myIter = new NCollection_IndexedDataMap<Graphic3d_CLight, int, NCollection_DefaultHasher<Graphic3d_CLight>>.Iterator(theSet.myLights);
                myFilter = (int)(theFilter);
                skipFiltered();
            }

            public bool More()
            {
                return myIter.More();
            }

            //! Skip filtered items.
            void skipFiltered()
            {
                if (myFilter == 0)
                {
                    return;
                }

                for (; myIter.More(); myIter.Next())
                {
                    if ((myFilter & (int)IterationFilter.IterationFilter_ExcludeAmbient) != 0
                     && myIter.Key().Type() == Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient)
                    {
                        continue;
                    }
                    else if ((myFilter & (int)IterationFilter.IterationFilter_ExcludeDisabled) != 0
                          && !myIter.Key().IsEnabled())
                    {
                        continue;
                    }
                    else if ((myFilter & (int)IterationFilter.IterationFilter_ExcludeNoShadow) != 0
                          && !myIter.Key().ToCastShadows())
                    {
                        continue;
                    }

                    break;
                }
            }
            NCollection_IndexedDataMap<Graphic3d_CLight, int>.Iterator myIter;
            int myFilter;
            public void Next()
            {
                myIter.Next();
                skipFiltered();
            }

            public Graphic3d_CLight Value()
            {
                return myIter.Key();
            }
        }
    }
}
