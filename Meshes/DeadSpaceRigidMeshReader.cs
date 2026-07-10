using FMT.Core.Meshes;
using FMT.FileTools;
using FMT.PluginInterfaces;
using FMT.PluginInterfaces.Meshes;

namespace DeadSpacePlugin.Meshes
{
    internal class DeadSpaceRigidMeshReader
    {
        public int MaxLodCount => (int)MeshLimits.MaxMeshLodCount;

        public void Read(NativeReader nativeReader, MeshSet meshSet)
        {
            nativeReader.Position = 0;

            meshSet.BoundingBox = nativeReader.ReadAxisAlignedBox();
            long[] lodOffsets = new long[MaxLodCount];
            for (int i2 = 0; i2 < MaxLodCount; i2++)
            {
                lodOffsets[i2] = nativeReader.ReadLong();
            }
            meshSet.UnknownPostLODCount = nativeReader.ReadLong();
            long offsetNameLong = nativeReader.ReadLong();
            long offsetNameShort = nativeReader.ReadLong();
            meshSet.nameHash = nativeReader.ReadUInt();
            meshSet.Type = (MeshType)nativeReader.ReadInt();

            for (int n = 0; n < MaxLodCount * 2; n++)
            {
                meshSet.LodFade.Add(nativeReader.ReadUInt16LittleEndian());
            }
            meshSet.MeshLayout = (EMeshLayout)nativeReader.ReadByte();
            nativeReader.Position -= 1;
            var meshLayoutFlags = (MeshSetLayoutFlags)nativeReader.ReadULong();
            meshSet.ShaderDrawOrder = (ShaderDrawOrder)nativeReader.ReadByte();
            meshSet.ShaderDrawOrderUserSlot = (ShaderDrawOrderUserSlot)nativeReader.ReadByte();
            meshSet.ShaderDrawOrderSubOrder = (ShaderDrawOrderSubOrder)nativeReader.ReadUShort();
            var lodsCount = nativeReader.ReadUShort();
            meshSet.MeshCount = nativeReader.ReadUShort();

            for (var iL = 0; iL < lodsCount; iL++)
            {
                meshSet.PositionsOfLodMeshSet.Add(nativeReader.ReadUShort());
            }

            for (int iL = 0; iL < lodsCount; iL++)
            {
                nativeReader.Position = lodOffsets[iL];
                MeshSetLod lod = new MeshSetLod(nativeReader, meshSet);
                lod.SetParts(meshSet.partTransforms, meshSet.partBoundingBoxes);
                meshSet.Lods.Add(lod);
            }
        }
    }
}
