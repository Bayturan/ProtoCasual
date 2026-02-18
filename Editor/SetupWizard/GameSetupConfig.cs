namespace ProtoCasual.Editor
{
    // ─────────────────────────────────────────────────
    //  Enums  –– Every question from the wizard maps to one enum
    // ─────────────────────────────────────────────────

    public enum GameType { HyperCasual, Hybrid, Puzzle, Racing, Endless }
    public enum MapType { FixedLevels, Procedural, EndlessGeneration }
    public enum BotOption { None, SimpleAI, AdvancedAI }
    public enum MonetizationType { None, AdsOnly, IAPOnly, AdsPlusIAP }
    public enum StoreOption { Enabled, Disabled }
    public enum InputType { Tap, Swipe, Drag, Steering, Mixed }
    public enum TargetPlatform { Android, iOS, Both }

    /// <summary>
    /// Serializable data object holding every answer the developer gives in the Setup Wizard.
    /// Passed to every generator helper.
    /// </summary>
    [System.Serializable]
    public class GameSetupConfig
    {
        public string gameName = "MyGame";
        public GameType gameType = GameType.HyperCasual;
        public MapType mapType = MapType.FixedLevels;
        public BotOption bots = BotOption.None;
        public MonetizationType monetization = MonetizationType.None;
        public StoreOption store = StoreOption.Disabled;
        public InputType inputType = InputType.Tap;
        public TargetPlatform platform = TargetPlatform.Both;
    }
}
