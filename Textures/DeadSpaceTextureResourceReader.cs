using FMT.FileTools;
using FMT.Logging;
using FMT.PluginInterfaces;
using FMT.ServicesManagers;
using FMT.ServicesManagers.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeadSpacePlugin.Textures
{
    public class DeadSpaceTextureResourceReader : ITextureResourceReader
    {
        private IAssetManagementService assetManagementService => SingletonService.GetInstance<IAssetManagementService>();

        public void ReadInStream(NativeReader nativeReader, ITexture texture)
        {
#if DEBUG
            nativeReader.Position = 0;
            var msCopy = new MemoryStream();
            nativeReader.BaseStream.CopyTo(msCopy);
            nativeReader.Position = 0;
            DebugBytesToFileLogger.Instance.WriteAllBytes("Texture.bin", msCopy.ToArray(), "Texture/DeadSpace/Read", false);
#endif

            texture.MipOffsets[0] = nativeReader.ReadUInt();
            texture.MipOffsets[1] = nativeReader.ReadUInt();
            texture.Type = (TextureType)nativeReader.ReadUInt();
            texture.PixelFormatNumber = nativeReader.ReadInt();
            texture.PoolId = nativeReader.ReadUInt();
            texture.Flags = (TextureFlags)nativeReader.ReadUShort();
            texture.Width = nativeReader.ReadUShort();
            texture.Height = nativeReader.ReadUShort();
            texture.Depth = nativeReader.ReadUShort();
            texture.SliceCount = nativeReader.ReadUShort();
            texture.MipCount = (byte)nativeReader.ReadUShort();
            texture.FirstMip = (byte)nativeReader.ReadInt();
            texture.ChunkId = nativeReader.ReadGuid();
            texture.MipSizes = (from _ in Enumerable.Range(0, 15)
                                select nativeReader.ReadUInt()).ToArray();

            texture.ChunkSize = nativeReader.ReadUInt();
            texture.AssetNameHash = nativeReader.ReadUInt();
            texture.TextureGroup = nativeReader.ReadNullTerminatedString();
            if (nativeReader.Position != nativeReader.Length)
            {
                List<byte> lastBytes = new();
                while (nativeReader.Position != nativeReader.Length)
                {
                    lastBytes.Add(nativeReader.ReadByte());
                }
                texture.UnknownBytes.Add(lastBytes.ToArray());
            }

            if (assetManagementService.Logger != null)
                assetManagementService.Logger.Log($"Texture: Loading ChunkId: {texture.ChunkId}");

            texture.ChunkEntry = assetManagementService.GetChunkEntry(texture.ChunkId);
            texture.Data = assetManagementService.GetChunk(texture.ChunkEntry).ToArray();
        }
    }
}
