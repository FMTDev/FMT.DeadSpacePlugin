using FMT.Core.Meshes;
using FMT.FileTools;

namespace DeadSpacePlugin.Meshes
{
    public class DeadSpaceCompositeMeshReader
    {
        public int MaxLodCount => (int)MeshLimits.MaxMeshLodCount;

        public void Read(NativeReader nativeReader, MeshSet meshSet)
        {
            nativeReader.Position = 0;

            new DeadSpaceMeshHeaderReader().Read(nativeReader, meshSet);
        }
    }
}
