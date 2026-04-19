namespace OCCPort
{
    public class DelaBella_Triangle
    {
        public DelaBella_Vertex[] v = new DelaBella_Vertex[3]; // 3 vertices spanning this triangle
        public DelaBella_Triangle []f = new DelaBella_Triangle[3]; // 3 adjacent faces, f[i] is at the edge opposite to vertex v[i]
        public DelaBella_Triangle next; // next triangle (of delaunay set or hull set)
    }
}
