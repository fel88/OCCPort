namespace OCCPort.OpenGL
{
    //! This class contains specular and diffuse maps required for Image Base Lighting (IBL) in PBR shading model with it's generation methods.
    internal class OpenGl_PBREnvironment: OpenGl_NamedResource
    {
        uint mySpecMapLevelsNumber; //!< number of mipmap levels used in specular IBL map

        //! Returns number of mipmap levels used in specular IBL map.
        //! It can be different from value passed to creation method.
        public uint SpecMapLevelsNumber() { return mySpecMapLevelsNumber; }

    }
}