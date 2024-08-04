using System;

namespace OCCPort
{
    public abstract class IDelaBella
    {

        // return 0: no output 
        // negative: all points are colinear, output hull vertices form colinear segment list, no triangles on output
        // positive: output hull vertices form counter-clockwise ordered segment contour, delaunay and hull triangles are available
        // if 'y' pointer is null, y coords are treated to be located immediately after every x
        // if advance_bytes is less than 2*sizeof coordinate type, it is treated as 2*sizeof coordinate type  

        public abstract int Triangulate(int points, double[] x, int xi, int yi, int advance_bytes = 0);
        public static IDelaBella Create()
        {
            CDelaBella db = new CDelaBella();
            //if (!db)
            //   return 0;

            /*db->vert_alloc = 0;
            db->face_alloc = 0;
            db->max_verts = 0;
            db->max_faces = 0;

            db->first_dela_face = 0;
            db->first_hull_face = 0;
            db->first_hull_vert = 0;

            db->inp_verts = 0;
            db->out_verts = 0;

            db->errlog_proc = 0;
            db->errlog_file = 0;*/

            return db;
        }

    }
}
