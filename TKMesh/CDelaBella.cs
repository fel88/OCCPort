namespace TKMesh
{
    public class CDelaBella : IDelaBella
    {


        public override DelaBella_Triangle GetFirstDelaunayTriangle()
        {
            //var ret1 = d.GetFirstTriangle();
            throw new NotImplementedException();
            DelaBella_Triangle ret = null;
            DelaBella_Triangle root = null;
            /* while (ret1 != null)
             {
                 var old = ret;

                 ret = new DelaBella_Triangle();
                 if (root == null)
                 {
                     root = ret;
                 }
                 if (old != null)
                 {
                     old.next = ret;
                 }
                 /*for (int i = 0; i < ret1.v.Length; i++)
                 {
                     TVert item = ret1.v[i];
                     ret.v[i] = new DelaBella_Vertex();
                     ret.v[i].x = item.x;
                     ret.v[i].y = item.y;
                     ret.v[i].i = item.index;

                 }
                 ret1 = ret1.Next;*/
            //  }
            //  return root;

        }
        //DelabellaWrapper d;
        // return 0: no output 
        // negative: all points are colinear, output hull vertices form colinear segment list, no triangles on output
        // positive: output hull vertices form counter-clockwise ordered segment contour, delaunay and hull triangles are available
        // if 'y' pointer is null, y coords are treated to be located immediately after every x
        // if advance_bytes is less than 2*sizeof coordinate type, it is treated as 2*sizeof coordinate type  
        public override int Triangulate(int points, double[] xy, int advance_bytes = 0)
        {
            // d = new DelabellaWrapper();

            if (xy == null)
                return 0;

            double[] x = new double[points];
            double[] y = new double[points];
            for (int i = 0; i < points; i++)
            {
                x[i] = xy[i * 2];
                y[i] = xy[i * 2 + 1];
            }
            //    var res = d.Triangulate(points, x, y);
            throw new NotImplementedException();
            // return res;
            //   if (y == null)
            //     y = x + 1;
        }
    }


}



