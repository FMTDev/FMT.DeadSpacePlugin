using FMT.Core.Meshes;
using FMT.Ebx;
using FMT.FileTools;
using FMT.PluginInterfaces;
using System;
using System.Collections.Generic;

namespace DeadSpacePlugin.Meshes
{
    public class DeadSpaceMeshSetLodReader
    {
        public MeshSetLod Read(NativeReader reader)
        {
            MeshSetLod meshSetLod = new MeshSetLod();
            var positionAtStart = reader.Position;
            reader.Position = positionAtStart;

            meshSetLod.Type = (MeshType)reader.ReadUInt();
            meshSetLod.maxInstances = reader.ReadUInt();
            uint sectionCount = reader.ReadUInt();
            var sectionOffset = reader.ReadLong();
            long categoryOffset = reader.Position;

            reader.Position = categoryOffset;
            meshSetLod.CategorySubsetIndices.Clear();
            for (int i = 0; i < meshSetLod.MaxCategories; i++)
            {
                int subsetCategoryCount = reader.ReadInt32LittleEndian();
                var subsetCategoryOffset = reader.ReadInt64LittleEndian();

                var currentPosition = reader.Position;

                reader.Position = subsetCategoryOffset;
                meshSetLod.CategorySubsetIndices.Add(new List<byte>());
                for (int j = 0; j < subsetCategoryCount; j++)
                {
                    meshSetLod.CategorySubsetIndices[i].Add(reader.ReadByte());
                }

                reader.Position = currentPosition;
            }
            meshSetLod.Flags = (MeshLayoutFlags)reader.ReadUInt32LittleEndian();
            meshSetLod.indexBufferFormat.format = reader.ReadInt();
            var renderFormatType = TypeLibrary.GetType("RenderFormat");
            var enumNames = Enum.GetNames(renderFormatType);

            meshSetLod.IndexBufferSize = reader.ReadUInt32LittleEndian();
            meshSetLod.VertexBufferSize = reader.ReadUInt32LittleEndian();
            if (meshSetLod.HasAdjacencyInMesh)
            {
                meshSetLod.AdjacencyBufferSize = reader.ReadUInt32LittleEndian();
                meshSetLod.adjacencyData = new byte[meshSetLod.AdjacencyBufferSize];
            }
            meshSetLod.UnknownChunkPad = reader.ReadBytes(12);

            meshSetLod.ChunkId = reader.ReadGuid();
            meshSetLod.inlineDataOffset = reader.ReadUInt32LittleEndian();
            if (meshSetLod.HasAdjacencyInMesh)
            {
                reader.ReadInt64LittleEndian();
            }
            long posShaderDebug = reader.ReadInt64LittleEndian();
            long posFullname = reader.ReadInt64LittleEndian();
            long posShortname = reader.ReadInt64LittleEndian();
            meshSetLod.nameHash = reader.ReadUInt32LittleEndian();
            uint boneCount = 0u;
            long boneOffset = 0L;
            long num7 = 0L;
            long compositepositionofsomething = 0L;
            meshSetLod.UnknownLongAfterNameHash = reader.ReadInt64LittleEndian();
            if (meshSetLod.Type == MeshType.MeshType_Skinned)
            {
                boneCount = reader.ReadUInt32LittleEndian();
                boneOffset = reader.ReadInt64LittleEndian();
            }
            else if (meshSetLod.Type == MeshType.MeshType_Composite)
            {
                compositepositionofsomething = reader.ReadInt64LittleEndian();
            }
            reader.Pad(16);
            long position4 = reader.Position;
            if (meshSetLod.Type == MeshType.MeshType_Skinned)
            {
                reader.Position = boneOffset;
                for (int k = 0; k < boneCount; k++)
                {
                    meshSetLod.BoneIndexArray.Add(reader.ReadUInt32LittleEndian());
                }
                if (num7 != 0L)
                {
                    reader.Position = num7;
                    for (int l = 0; l < boneCount; l++)
                    {
                        meshSetLod.BoneShortNameArray.Add(reader.ReadUInt32LittleEndian());
                    }
                }
            }
            else if (meshSetLod.Type == MeshType.MeshType_Composite)
            {
                if (boneOffset != 0L)
                {
                    reader.Position = boneOffset;
                    for (int m = 0; m < boneCount; m++)
                    {
                        meshSetLod.partBoundingBoxes.Add(reader.ReadAxisAlignedBox());
                    }
                }
                if (num7 != 0L)
                {
                    reader.Position = num7;
                    for (int n = 0; n < boneCount; n++)
                    {
                        meshSetLod.PartTransforms.Add(reader.ReadLinearTransform());
                    }
                }
                if (compositepositionofsomething != 0L)
                {
                    reader.Position = compositepositionofsomething;
                    List<int> list = new();
                    for (int num9 = 0; num9 < 24; num9++)
                    {
                        int num10 = reader.ReadByte();
                        for (int num2 = 0; num2 < 8; num2++)
                        {
                            if (((uint)num10 & (true ? 1u : 0u)) != 0)
                            {
                                list.Add(num9 * 8 + num2);
                            }
                            num10 >>= 1;
                        }
                    }
                }
            }

            // MeshSetSections
            reader.Position = sectionOffset;
            for (uint sectionIndex = 0u; sectionIndex < sectionCount; sectionIndex++)
            {
                meshSetLod.Sections.Add(new MeshSetSection(reader, (int)sectionIndex));
            }

            // Read Shader / Name / ShortName
            reader.Position = posShaderDebug;
            meshSetLod.shaderDebugName = reader.ReadNullTerminatedString();
            reader.Position = posFullname;
            meshSetLod.Name = reader.ReadNullTerminatedString();
            reader.Position = posShortname;
            meshSetLod.shortName = reader.ReadNullTerminatedString();
            reader.Position = position4;
            meshSetLod.hasBoneShortNames = meshSetLod.BoneShortNameArray.Count > 0;
            return meshSetLod;
        }


    }
}
