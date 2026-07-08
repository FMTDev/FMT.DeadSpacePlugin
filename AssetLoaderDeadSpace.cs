using FMT.Core;
using FMT.PluginInterfaces;
using FMT.PluginInterfaces.Assets;
using FMT.ServicesManagers;
using FMT.ServicesManagers.Interfaces;
using FrostySdk;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NFSUnboundPlugin
{
    public class AssetLoaderDeadSpace : IAssetLoader
    {
        public void LoadData(IEnumerable<string> superBundles, string folder = "native_data/")
        {
            Parallel.ForEach(superBundles, (sbName) =>
            {
                var tocFileRAW = $"{folder}{sbName}.toc";
                string tocFileLocation = SingletonService.GetInstance<IFileSystemService>().ResolvePath(tocFileRAW);
                if (!string.IsNullOrEmpty(tocFileLocation) && File.Exists(tocFileLocation))
                {
                    TOCFile tocFile = new(tocFileRAW, true, true, false, -1, false);
                    tocFile.Dispose();
                }
            });
        }

        public IEnumerable<IAssetEntry> Load(IEnumerable<string> superBundles)
        {
            SingletonService.GetInstance<IFileSystemService>().TOCFileType = typeof(TOCFile);
            LoadData(superBundles, "native_patch/");
            LoadData(superBundles);
            return null;
        }
    }
}
