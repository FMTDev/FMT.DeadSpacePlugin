using FMT.Core.Meshes;
using FMT.FileTools;
using FMT.PluginInterfaces;
using FMT.PluginInterfaces.Meshes;

namespace DeadSpacePlugin.Meshes.Readers
{
    public class DeadSpaceMeshHeaderReader
    {
        public void Read(NativeReader nativeReader, MeshSet meshSet)
        {
            nativeReader.Position = 0;
            meshSet.UnknownBytes.Clear();

            meshSet.BoundingBox = nativeReader.ReadAxisAlignedBox();
            long[] lodOffsets = new long[meshSet.MaxLodCount];
            for (int i2 = 0; i2 < meshSet.MaxLodCount; i2++)
            {
                lodOffsets[i2] = nativeReader.ReadLong();
            }
            meshSet.UnknownPostLODCount = nativeReader.ReadLong();
            long offsetNameLong = nativeReader.ReadLong();
            long offsetNameShort = nativeReader.ReadLong();
            meshSet.nameHash = nativeReader.ReadUInt();
            meshSet.Type = (MeshType)nativeReader.ReadByte();
            meshSet.MeshCount = nativeReader.ReadByte();
            meshSet.UnknownBytes.Add(nativeReader.ReadBytes(14));

            meshSet.LodFade.Clear();
            for (int n = 0; n < meshSet.MaxLodCount * 2; n++)
            {
                meshSet.LodFade.Add(nativeReader.ReadUInt16LittleEndian());
            }
            var meshLayoutFlags = (MeshSetLayoutFlags)nativeReader.ReadULong();
            meshSet.ShaderDrawOrder = (ShaderDrawOrder)nativeReader.ReadByte();
            meshSet.ShaderDrawOrderUserSlot = (ShaderDrawOrderUserSlot)nativeReader.ReadByte();
            meshSet.ShaderDrawOrderSubOrder = (ShaderDrawOrderSubOrder)nativeReader.ReadUShort();
            meshSet.LodCount = nativeReader.ReadUShort();
            meshSet.MeshCount = nativeReader.ReadUShort();
        }
    }
}
