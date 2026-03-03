using UnityEngine;
using UnityEditor;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Copies the selected game-type USS theme to the project for customisation.
    /// The original UXML templates remain in the package (read-only).
    /// Users edit Assets/_Game/UI/Themes/GameTheme.uss to reskin their game.
    /// </summary>
    public static class UIPrefabGenerator
    {
        private const string PKG_USS = "Packages/com.bayturan.protocasual/Runtime/UI/USS";

        public static void Generate(GameSetupConfig cfg)
        {
            CopyThemeToProject(cfg.gameType);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ProtoCasual] UI theme copied to project for customisation.");
        }

        private static void CopyThemeToProject(GameType gameType)
        {
            string themeFile = GetThemeFileName(gameType);
            string srcPath = $"{PKG_USS}/Themes/{themeFile}.uss";
            string dstDir = "Assets/_Game/UI/Themes";
            string dstPath = $"{dstDir}/GameTheme.uss";

            EnsureDir(dstDir);

            // Don't overwrite if the user already has a custom theme
            if (AssetDatabase.LoadAssetAtPath<Object>(dstPath) != null)
            {
                Debug.Log("[ProtoCasual] Project theme already exists — skipping copy.");
                return;
            }

            if (!AssetDatabase.CopyAsset(srcPath, dstPath))
            {
                // Fallback: read from package and create in project
                var srcContent = System.IO.File.ReadAllText(
                    System.IO.Path.GetFullPath(srcPath));
                System.IO.File.WriteAllText(
                    System.IO.Path.GetFullPath(dstPath), srcContent);
                AssetDatabase.ImportAsset(dstPath);
            }

            Debug.Log($"[ProtoCasual] Theme '{themeFile}' → {dstPath}");
        }

        private static string GetThemeFileName(GameType type) => type switch
        {
            GameType.HyperCasual => "HyperCasual",
            GameType.Puzzle      => "Puzzle",
            GameType.Racing      => "Racing",
            GameType.Endless     => "Endless",
            GameType.Hybrid      => "Hybrid",
            _                    => "HyperCasual"
        };

        private static void EnsureDir(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                var folder = System.IO.Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent)) EnsureDir(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
