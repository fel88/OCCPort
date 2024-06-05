namespace OCCPort
{
    public struct gp_Vec
    {
        gp_XYZ coord;
        //! Multiplies a vector by a scalar
        public gp_Vec Multiplied(double theScalar)
        {
            gp_Vec aV = this;
            aV.coord.Multiply(theScalar);
            return aV;
        }

        public gp_XYZ XYZ()
        {
            return coord;
        }

        public gp_Vec Added(gp_Vec theOther)
        {
            gp_Vec aV = this;
            aV.coord.Add(theOther.coord);
            return aV;
        }
        
        public static gp_Vec operator *(gp_Vec v, double theScalar)
        {
            return v.Multiplied(theScalar);
        }

        public static gp_Vec operator +(gp_Vec v, gp_Vec theOther)
        {
            return v.Added(theOther);
        }

        public gp_Vec(gp_Dir theV)
        {
            coord = theV.XYZ();
        }
    }
}