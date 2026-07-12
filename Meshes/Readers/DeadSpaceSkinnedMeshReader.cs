using FMT.Core.Meshes;
using FMT.FileTools;

namespace DeadSpacePlugin.Meshes.Readers
{
    public class DeadSpaceSkinnedMeshReader
    {
        public int MaxLodCount => (int)MeshLimits.MaxMeshLodCount;

        public void Read(NativeReader nativeReader, MeshSet meshSet)
        {
            nativeReader.Position = 0;

            new DeadSpaceMeshHeaderReader().Read(nativeReader, meshSet);

            // useful for resetting when live debugging
            var positionBeforeMeshTypeRead = nativeReader.Position;
            nativeReader.Position = positionBeforeMeshTypeRead;

            nativeReader.ReadBytes(12);

            meshSet.BoneCount = nativeReader.ReadUInt16LittleEndian();
            meshSet.CullBoxCount = nativeReader.ReadUInt16LittleEndian();
            if (meshSet.CullBoxCount != 0)
            {
                long cullBoxBoneIndicesOffset = nativeReader.ReadInt64LittleEndian();
                long cullBoxBoundingBoxOffset = nativeReader.ReadInt64LittleEndian();
                long position = nativeReader.Position;
                if (cullBoxBoneIndicesOffset != 0L)
                {
                    nativeReader.Position = cullBoxBoneIndicesOffset;
                    for (int m = 0; m < meshSet.CullBoxCount; m++)
                    {
                        meshSet.boneIndices.Add(nativeReader.ReadUInt16LittleEndian());
                    }
                }
                if (cullBoxBoundingBoxOffset != 0L)
                {
                    nativeReader.Position = cullBoxBoundingBoxOffset;
                    for (int l = 0; l < meshSet.CullBoxCount; l++)
                    {
                        meshSet.boneBoundingBoxes.Add(nativeReader.ReadAxisAlignedBox());
                    }
                }
                nativeReader.Position = position;
            }

            for (int iL = 0; iL < meshSet.LodCount; iL++)
            {
                if (meshSet.LodOffsets[iL] != 0)
                {
                    nativeReader.Position = meshSet.LodOffsets[iL];
                    MeshSetLod lod = new DeadSpaceMeshSetLodReader().Read(nativeReader);
                    lod.SetParts(meshSet.partTransforms, meshSet.partBoundingBoxes);
                    meshSet.Lods.Add(lod);
                }
            }
        }
    }
}
