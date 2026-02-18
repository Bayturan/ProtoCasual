using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Generates UI screen prefabs (Menu, Gameplay, Win, Lose, Pause, Store, Settings)
    /// and saves them to Assets/_Game/UI/.
    /// </summary>
    public static class UIPrefabGenerator
    {
        private const string UI_DIR = "Assets/_Game/UI";

        public static void Generate(GameSetupConfig cfg)
        {
            EnsureDir(UI_DIR);

            CreateScreenPrefab("MenuScreen", BuildMenuScreen);
            CreateScreenPrefab("GameplayScreen", BuildGameplayScreen);
            CreateScreenPrefab("WinScreen", BuildWinScreen);
            CreateScreenPrefab("LoseScreen", BuildLoseScreen);
            CreateScreenPrefab("PauseScreen", BuildPauseScreen);

            if (cfg.store == StoreOption.Enabled)
                CreateScreenPrefab("StoreScreen", BuildStoreScreen);

            CreateScreenPrefab("SettingsScreen", BuildSettingsScreen);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ProtoCasual] UI prefabs created.");
        }

        // ─── Screen builders ────────────────────────────────────────────

        private static void BuildMenuScreen(GameObject root)
        {
            AddBackground(root, new Color(0.1f, 0.1f, 0.15f, 1f));
            AddText(root, "Title", "GAME TITLE", 64, TextAlignmentOptions.Center,
                new Vector2(0, 100), new Vector2(600, 80));
            AddButton(root, "PlayButton", "PLAY",
                new Vector2(0, -20), new Vector2(300, 60));
            AddButton(root, "SettingsButton", "SETTINGS",
                new Vector2(0, -100), new Vector2(300, 60));
        }

        private static void BuildGameplayScreen(GameObject root)
        {
            AddText(root, "ScoreText", "Score: 0", 32, TextAlignmentOptions.TopLeft,
                new Vector2(-300, 400), new Vector2(300, 50));
            AddText(root, "LevelText", "Level 1", 28, TextAlignmentOptions.Top,
                new Vector2(0, 400), new Vector2(200, 50));
            AddButton(root, "PauseButton", "||",
                new Vector2(350, 400), new Vector2(80, 80));
        }

        private static void BuildWinScreen(GameObject root)
        {
            AddBackground(root, new Color(0f, 0.2f, 0f, 0.85f));
            AddText(root, "WinTitle", "YOU WIN!", 72, TextAlignmentOptions.Center,
                new Vector2(0, 100), new Vector2(600, 100));
            AddButton(root, "NextLevelButton", "NEXT LEVEL",
                new Vector2(0, -30), new Vector2(300, 60));
            AddButton(root, "MenuButton", "MENU",
                new Vector2(0, -110), new Vector2(300, 60));
        }

        private static void BuildLoseScreen(GameObject root)
        {
            AddBackground(root, new Color(0.3f, 0f, 0f, 0.85f));
            AddText(root, "LoseTitle", "GAME OVER", 72, TextAlignmentOptions.Center,
                new Vector2(0, 100), new Vector2(600, 100));
            AddButton(root, "RetryButton", "RETRY",
                new Vector2(0, -30), new Vector2(300, 60));
            AddButton(root, "MenuButton", "MENU",
                new Vector2(0, -110), new Vector2(300, 60));
        }

        private static void BuildPauseScreen(GameObject root)
        {
            AddBackground(root, new Color(0f, 0f, 0f, 0.7f));
            AddText(root, "PauseTitle", "PAUSED", 64, TextAlignmentOptions.Center,
                new Vector2(0, 120), new Vector2(400, 80));
            AddButton(root, "ResumeButton", "RESUME",
                new Vector2(0, 0), new Vector2(300, 60));
            AddButton(root, "RestartButton", "RESTART",
                new Vector2(0, -80), new Vector2(300, 60));
            AddButton(root, "QuitButton", "QUIT",
                new Vector2(0, -160), new Vector2(300, 60));
        }

        private static void BuildStoreScreen(GameObject root)
        {
            AddBackground(root, new Color(0.05f, 0.05f, 0.1f, 1f));
            AddText(root, "StoreTitle", "STORE", 54, TextAlignmentOptions.Center,
                new Vector2(0, 350), new Vector2(400, 70));
            AddButton(root, "CloseButton", "X",
                new Vector2(380, 400), new Vector2(60, 60));

            // Content area placeholder
            var content = new GameObject("Content");
            content.transform.SetParent(root.transform, false);
            var rt = content.AddComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(700, 600);
        }

        private static void BuildSettingsScreen(GameObject root)
        {
            AddBackground(root, new Color(0.08f, 0.08f, 0.12f, 1f));
            AddText(root, "SettingsTitle", "SETTINGS", 54, TextAlignmentOptions.Center,
                new Vector2(0, 300), new Vector2(400, 70));
            AddButton(root, "CloseButton", "X",
                new Vector2(380, 400), new Vector2(60, 60));

            // Toggle placeholders
            AddText(root, "SoundLabel", "Sound", 28, TextAlignmentOptions.Left,
                new Vector2(-100, 100), new Vector2(200, 40));
            AddText(root, "VibrationLabel", "Vibration", 28, TextAlignmentOptions.Left,
                new Vector2(-100, 40), new Vector2(200, 40));
        }

        // ─── Shared builder helpers ─────────────────────────────────────

        private static void CreateScreenPrefab(string name, System.Action<GameObject> build)
        {
            string assetPath = $"{UI_DIR}/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null) return;

            var go = new GameObject(name);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            build(go);

            PrefabUtility.SaveAsPrefabAsset(go, assetPath);
            Object.DestroyImmediate(go);
        }

        private static void AddBackground(GameObject parent, Color color)
        {
            var bg = new GameObject("Background");
            bg.transform.SetParent(parent.transform, false);
            var rt = bg.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = bg.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = true;
        }

        private static TextMeshProUGUI AddText(GameObject parent, string name, string text,
            int fontSize, TextAlignmentOptions align, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            return tmp;
        }

        private static Button AddButton(GameObject parent, string name, string label,
            Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f, 1f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            // Button label
            AddText(go, "Label", label, 24, TextAlignmentOptions.Center, Vector2.zero, size);
            return btn;
        }

        private static void EnsureDir(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                var folder = System.IO.Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent))
                    EnsureDir(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
