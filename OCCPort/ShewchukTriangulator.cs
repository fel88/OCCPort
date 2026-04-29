using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    public class ShewchukTriangulator : IDelaBella
    {
        public override DelaBella_Triangle GetFirstDelaunayTriangle()
        {
            return triangles[0];

        }
        DelaBella_Triangle[] triangles;

        public override int Triangulate(int points, double[] xy, int advance_bytes = 0)
        {
            if (xy == null)
                return 0;

            double[] x = new double[points];
            double[] y = new double[points];
            List<Vector2d> vv = new List<Vector2d>();
            for (int i = 0; i < points - 4; i++)// exclude 2 super-triangles
            {
                x[i] = xy[i * 2];
                y[i] = xy[i * 2 + 1];
                vv.Add(new Vector2d(x[i], y[i]));
            }
            if (vv.Count < 3)
                return 0;

            var results = GeomHelpers.TriangulateWithHoles([vv.ToArray()], null);
            //convert to DelaBella_Triangle
            List<DelaBella_Triangle> tt = new List<DelaBella_Triangle>();
            foreach (var item in results)
            {
                var ret = new DelaBella_Triangle();
                ret.v = new DelaBella_Vertex[item.Length];
                for (int i = 0; i < item.Length; i++)
                {
                    ret.v[i] = new DelaBella_Vertex();
                    ret.v[i].x = item[i].X;
                    ret.v[i].y = item[i].Y;
                    for (int j = 0; j < vv.Count; j++)
                    {
                        if ((vv[j] - item[i]).Length < 1e-8)
                        {
                            ret.v[i].i = j;
                            break;
                        }
                    }
                }

                tt.Add(ret);
            }

            triangles = tt.ToArray();
            for (int i = 0; i < triangles.Length - 1; i++)
            {
                triangles[i].next = triangles[i + 1];
            }
            return results.Length;
        }
    }
}