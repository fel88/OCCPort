using System.Security.Cryptography;

namespace OCCPort.Interfaces
{
    public interface IMeshTools_MeshBuilder
    {
        void Perform(Message_ProgressRange message_ProgressRange);
    }
}