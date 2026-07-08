using FMT.Logging;
using FMT.PluginInterfaces;
using FrostySdk.Frostbite.Compilers;

namespace NFSUnboundPlugin
{

    public class AssetCompilerDeadSpace : Frostbite2022AssetCompiler, IAssetCompiler
    {
        /// <summary>
        /// This is run AFTER the compilation of the fbmod into resource files ready for the Actions to TOC/SB/CAS to be taken
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="logger"></param>
        /// <param name="frostyModExecuter">Frosty Mod Executer object</param>
        /// <returns></returns>
        public override bool Compile(ILogger logger, IModExecutor modExecutor)
        {
            return base.Compile(logger, modExecutor);
        }


    }
}