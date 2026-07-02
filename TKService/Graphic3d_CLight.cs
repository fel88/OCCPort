using OCCPort;
using OCCPort.Common;
using System.Drawing;
using System.Reflection.Metadata;
using TKernel;
using TKMath;

namespace TKService
{
    //! Generic light source definition.
    //! This class defines arbitrary light source - see Graphic3d_TypeOfLightSource enumeration.
    //! Some parameters are applicable only to particular light type;
    //! calling methods unrelated to current type will throw an exception.
    public class Graphic3d_CLight
    {
        public Graphic3d_CLight(Graphic3d_TypeOfLightSource theType)
        {
            myPosition = new(0.0, 0.0, 0.0);
            myColor = new Quantity_ColorRGBA (1.0f, 1.0f, 1.0f, 1.0f);
            myDirection = new(0.0f, 0.0f, 0.0f, 0.0f);
            myParams = new(0.0f, 0.0f, 0.0f, 0.0f);
            mySmoothness = (0.0f);
            myIntensity = (1.0f);
            myType = (theType);
            myRevision = (0);
            myIsHeadlight = (false);
            myIsEnabled = (true);
            myToCastShadows = false;

            switch (theType)
            {
                case Graphic3d_TypeOfLightSource. Graphic3d_TypeOfLightSource_Ambient:
                    {
                        break;
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional:
                    {
                        mySmoothness = 0.2f;
                        myIntensity = 20.0f;
                        break;
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional:
                    {
                      //  changeConstAttenuation() = 1.0f;
                     //   changeLinearAttenuation() = 0.0f;
                        break;
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot:
                    {
                        //changeConstAttenuation() = 1.0f;
                      ////  changeLinearAttenuation() = 0.0f;
                      //  changeConcentration() = 1.0f;
                      //  changeAngle() = 0.523599f;
                        break;
                    }
            }
            makeId();
        }
        void makeId()
        {
            string aTypeSuffix = "";
            switch (myType)
            {
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient: aTypeSuffix = "amb"; break;
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional: aTypeSuffix = "dir"; break;
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional: aTypeSuffix = "pos"; break;
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot: aTypeSuffix = "spot"; break;
            }

           
            myId = ("Graphic3d_CLight_") + aTypeSuffix
                 + (Interlocked.Increment(ref THE_LIGHT_COUNTER));
        }
        static volatile int THE_LIGHT_COUNTER = 0;

        string myId;          //!< resource id

        gp_Pnt myPosition;    //!< light position

        public void SetDirection(gp_Dir theDir)
        {
            Exceptions.Standard_ProgramError_Raise_if(myType != Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot
                                            && myType != Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional,
                                               "Graphic3d_CLight::SetDirection(), incorrect light type");
            updateRevisionIf(Math.Abs(myDirection.x() - (float)(theDir.X())) > Standard_ShortReal.ShortRealEpsilon()
                     || Math.Abs(myDirection.y() - (float)(theDir.Y())) > Standard_ShortReal.ShortRealEpsilon()
                     || Math.Abs(myDirection.z() - (float)(theDir.Z())) > Standard_ShortReal.ShortRealEpsilon());

            myDirection.x((float)(theDir.X()));
            myDirection.y((float)(theDir.Y()));
            myDirection.z((float)(theDir.Z()));
        }

        Graphic3d_Vec4 myDirection;   //!< direction of directional/spot light
        Graphic3d_Vec4 myParams;      //!< packed light parameters
        float mySmoothness;  //!< radius for point light or cone angle for directional light
        float myIntensity;   //!< intensity multiplier for light
        //! Update modification counter.
        void updateRevisionIf(bool theIsModified)
        {
            if (theIsModified)
            {
                ++myRevision;
            }
        }

        public void SetColor(Quantity_Color theColor)
        {
            updateRevisionIf(myColor.GetRGB().IsDifferent(theColor));
            myColor.SetRGB(theColor);
        }

        Quantity_ColorRGBA myColor;       //!< light color

        //! Sets light source name.
        public void SetName(string theName) { myName = theName; }
        public void SetHeadlight(bool theValue)
        {
            if (myType == Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient)
            {
                throw new Standard_ProgramError("Graphic3d_CLight::SetHeadlight() is not applicable to ambient light");
            }
            updateRevisionIf(myIsHeadlight != theValue);
            myIsHeadlight = theValue;
        }
        bool myIsHeadlight; //!< flag to mark head light
        bool myIsEnabled;   //!< enabled state
        bool myToCastShadows;//!< casting shadows is requested
        Graphic3d_TypeOfLightSource myType;        //!< Graphic3d_TypeOfLightSource enumeration
        string myName;        //!< user given name

        //! Returns the Type of the Light, cannot be changed after object construction.
        public Graphic3d_TypeOfLightSource Type() { return myType; }
        int myRevision;    //!< modification counter



        //! @return modification counter
        public int Revision() { return myRevision; }

    }

}
