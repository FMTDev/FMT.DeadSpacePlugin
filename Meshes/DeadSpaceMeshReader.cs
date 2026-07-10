using FMT.Core.Meshes;
using FMT.FileTools;
using FMT.PluginInterfaces;

namespace DeadSpacePlugin.Meshes
{
    internal class DeadSpaceMeshReader : IMeshSetReader
    {
        private DeadSpaceSkinnedMeshReader SkinnedMeshReader { get; } = new DeadSpaceSkinnedMeshReader();
        private DeadSpaceCompositeMeshReader CompositeMeshReader { get; } = new DeadSpaceCompositeMeshReader();
        private DeadSpaceRigidMeshReader RigidMeshReader { get; } = new DeadSpaceRigidMeshReader();

        public int MaxLodCount => (int)MeshLimits.MaxMeshLodCount;

        public void Read(NativeReader nativeReader, MeshSet meshSet)
        {
            meshSet.BoundingBox = nativeReader.ReadAxisAlignedBox();
            meshSet.LodOffsets.Clear();
            for (int i2 = 0; i2 < MaxLodCount; i2++)
            {
                meshSet.LodOffsets.Add(nativeReader.ReadLong());
            }
            meshSet.UnknownPostLODCount = nativeReader.ReadLong();
            long offsetNameLong = nativeReader.ReadLong();
            long offsetNameShort = nativeReader.ReadLong();
            meshSet.nameHash = nativeReader.ReadUInt();
            meshSet.Type = (MeshType)nativeReader.ReadByte();
            if (meshSet.Type == MeshType.MeshType_Skinned)
            {
                SkinnedMeshReader.Read(nativeReader, meshSet);
                return;
            }
            else if (meshSet.Type == MeshType.MeshType_Composite)
            {
                CompositeMeshReader.Read(nativeReader, meshSet);
                return;
            }
            else
            {
                RigidMeshReader.Read(nativeReader, meshSet);
                return;
            }
        }

        public void Read(NativeReader nativeReader, IMeshSet meshSet)
        {
            Read(nativeReader, (MeshSet)meshSet);
        }
    }
}
