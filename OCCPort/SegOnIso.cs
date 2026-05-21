using System.Data;

namespace OCCPort.Tester
{
    //! Auxiliary structure defining segment of isoline.
    public class SegOnIso
    {

        public PntOnIso[] Pnts = new PntOnIso[2];

        public SegOnIso()
        {
        }

        public PntOnIso this[int i]
        {
            get => Pnts[i];
            set => Pnts[i] = value;
        }
    }

}
