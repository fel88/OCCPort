using TKService;


namespace OCCPort.OpenGL
{
    //! Class for iterating pair of texture sets through each defined texture slot.
    //! Note that iterator considers texture slots being in ascending order within OpenGl_TextureSet.
    public class OpenGl_TextureSetPairIterator
    {// ------------------------------------------------------------------
     // IntegerLast : Returns the maximum value of an integer
     // ------------------------------------------------------------------
        static int IntegerLast()
        {

            const int INT_MAX = 2147483647;
            return INT_MAX;
        }
        static int IntegerFirst()
        {

            const int INT_MIN = (-2147483647 - 1);
            return INT_MIN;
        }
        public OpenGl_TextureSetPairIterator(OpenGl_TextureSet theSet1, OpenGl_TextureSet theSet2)
        {
            ////: myIter1(theSet1),
            //myIter2(theSet2),
            myTexture1 = null;

            myTexture2 = null;
            myUnitLower = (IntegerLast());
            myUnitUpper = (IntegerFirst());
            myUnitCurrent = (0);
        }
        int myUnitLower;
        int myUnitUpper;
        int myUnitCurrent;
        //! Return TRUE if there are more texture units to pass through.
        public bool More() { return myUnitCurrent <= myUnitUpper; }

        //! Return current texture unit.
        public Graphic3d_TextureUnit Unit() { return (Graphic3d_TextureUnit)myUnitCurrent; }

        OpenGl_Texture myTexture1;
        OpenGl_Texture myTexture2;
        //! Access texture from first texture set.
        public OpenGl_Texture Texture1() { return myTexture1; }
        public OpenGl_Texture Texture2() { return myTexture2; }


        OpenGl_TextureSet.Iterator myIter1;
        OpenGl_TextureSet.Iterator myIter2;

        //! Move iterator position to the next pair.
        public  void Next()
        {
            ++myUnitCurrent;
            myTexture1 = myTexture2 = null;
            for (; myIter1.More(); myIter1.Next())
            {
                if ((int)myIter1.Unit() >= myUnitCurrent)
                {
                    myTexture1 = (int)myIter1.Unit() == myUnitCurrent ? myIter1.ChangeValue() .Texture: null;
                    break;
                }
            }
            for (; myIter2.More(); myIter2.Next())
            {
                if ((int)myIter2.Unit() >= myUnitCurrent)
                {
                    myTexture2 = (int)myIter2.Unit() == myUnitCurrent ? myIter2.ChangeValue().Texture : null;
                    break;
                }
            }
        }
    }

}