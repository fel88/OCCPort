using System;
using System.Reflection.Metadata;
using System.Threading;

namespace OCCPort.OpenGL
{
    internal class Graphic3d_TransformUtils
    {
        public static void Ortho2D(Graphic3d_Mat4 theOut,
            float theLeft,
            float theRight, float theBottom, float theTop)
        {

            Ortho(theOut, theLeft, theRight, theBottom, theTop, -1.0f, 1.0f);
        }

        public static void Ortho(Graphic3d_Mat4 theOut,
                                      float theLeft,
                                      float theRight,
                                      float theBottom,
                                      float theTop,
                                      float theZNear,
                                      float theZFar)
        {
            theOut.InitIdentity();

            var aData = theOut.ChangeData();

            var anInvDx = (1.0) / (theRight - theLeft);
            var anInvDy = (1.0) / (theTop - theBottom);
            var anInvDz = (1.0) / (theZFar - theZNear);

            aData[0] = (float)((2.0) * anInvDx);
            aData[5] = (float)((2.0) * anInvDy);
            aData[10] = (float)((-2.0) * anInvDz);

            aData[12] = (float)(-(theRight + theLeft) * anInvDx);
            aData[13] = (float)(-(theTop + theBottom) * anInvDy);
            aData[14] = (float)(-(theZFar + theZNear) * anInvDz);
        }

    }
}