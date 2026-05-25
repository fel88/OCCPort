namespace OCCPort
{
    internal class Graphic3d_Vec4d : NCollection_Vec4, IVectorType
    {


        public Graphic3d_Vec4d(double v1, double v2, double v3, double v4)
        {
            this.v[0] = v1;
            this.v[1] = v2;
            this.v[2] = v3;
            this.v[3] = v4;
        }

        public double X { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double Y { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double Z { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

     

        public IVectorType cwiseMax(IVectorType thePoint)
        {
            throw new System.NotImplementedException();
        }

    
        public IVectorType cwiseMin(IVectorType thePoint)
        {
            throw new System.NotImplementedException();
        }

        internal double w()
        {
            return v[3];
        }

        internal double x()
        {
            return v[0];

        }

        internal double y()
        {
            return v[1];

        }

        internal double z()
        {
            return v[2];

        }
    }
}
