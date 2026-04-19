namespace OCCPort
{
    public class CDelaBella : IDelaBella
    {
        public override DelaBella_Triangle GetFirstDelaunayTriangle()
        {
            throw new System.NotImplementedException();
        }

        // return 0: no output 
        // negative: all points are colinear, output hull vertices form colinear segment list, no triangles on output
        // positive: output hull vertices form counter-clockwise ordered segment contour, delaunay and hull triangles are available
        // if 'y' pointer is null, y coords are treated to be located immediately after every x
        // if advance_bytes is less than 2*sizeof coordinate type, it is treated as 2*sizeof coordinate type  
        public override int Triangulate(int points, double[] xy, int advance_bytes = 0)
        {
            if (xy == null)
                return 0;

            double[] x = new double[points];
            double[] y = new double[points];
            for (int i = 0; i < points; i++)
            {
                x[i] = xy[i * 2];
                y[i] = xy[i * 2 + 1];
            }

            return 1;
            //   if (y == null)
            //     y = x + 1;
        }
    }
}