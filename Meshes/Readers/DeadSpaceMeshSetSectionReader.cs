using FMT.Core.Meshes;
using FMT.FileTools;
using FMT.PluginInterfaces;
using FMT.PluginInterfaces.Meshes;

namespace DeadSpacePlugin.Meshes.Readers
{
    internal class DeadSpaceMeshSetSectionReader : IMeshSetSectionReader
    {

        //public void Read(NativeReader nativeReader, MeshSetSectionWithSmoothing section, int index)
        //{
            
        //}

        public void Read(NativeReader nativeReader, MeshSetSection section, int index)
        {
            var startPosition = nativeReader.Position;
            nativeReader.Position = startPosition;

            section.SectionIndex = index;
            section.Offset1 = nativeReader.ReadInt64LittleEndian();
            if (section.Offset1 != 0)
                return;

            section.Name = nativeReader.ReadNullTerminatedString(offset: nativeReader.ReadInt64LittleEndian());
            long bonePositions = nativeReader.ReadInt64();


            section.BoneCount = nativeReader.ReadUInt16LittleEndian();
            section.BonesPerVertex = (byte)nativeReader.ReadUShort();
            section.MaterialId = nativeReader.ReadUInt16LittleEndian();
            section.VertexStride = nativeReader.ReadByte();
            section.PrimitiveType = (PrimitiveType)nativeReader.ReadByte();
            section.PrimitiveCount = (uint)nativeReader.ReadUInt32LittleEndian();
            section.StartIndex = nativeReader.ReadUInt32LittleEndian();
            section.VertexOffset = nativeReader.ReadUInt32LittleEndian();
            section.VertexCount = (uint)nativeReader.ReadUInt32LittleEndian();

            section.UnknownBytes.Add(nativeReader.ReadBytes(20));

            // should be at 452 (on first section)
            section.TextureCoordinateRatios.Clear();
            for (int i = 0; i < 6; i++)
            {
                section.TextureCoordinateRatios.Add(nativeReader.ReadFloat());
            }

            var positionBeforeGeomDecl = nativeReader.Position;
            section.DeclCount = 1;
            section.ReadGeomDecl(nativeReader);
            

            var positionAfterGeomDecl = nativeReader.Position;
            var lengthOfGeomDecl = positionAfterGeomDecl - positionBeforeGeomDecl;

            _ = nativeReader.Position;
            section.UnknownBytes.Add(nativeReader.ReadBytes(64));
            section.ReadBones(nativeReader, bonePositions);
        }

        public void Read(NativeReader nativeReader, IMeshSetSection section, int index)
        {
            this.Read(nativeReader, (MeshSetSection)section, index);
        }
    }
}
