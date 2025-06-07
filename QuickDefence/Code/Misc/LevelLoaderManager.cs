using System.IO;


// :D


namespace Misc
{
    internal static class LevelLoaderManager
    {
        public static string levelLoadingPath = (Directory.Exists(@"..\..\..\Assets\SceneData\MainGameScenes\") 
            ? @"..\..\..\Assets\SceneData\MainGameScenes\"
            : @"Assets\SceneData\MainGameScenes\");
    }
}
