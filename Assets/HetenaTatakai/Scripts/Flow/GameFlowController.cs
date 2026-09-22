using System.Collections;
using UnityEngine;

namespace HetenaTatakai
{
    public sealed class GameFlowController : MonoBehaviour
    {
        [SerializeField] private FighterCombatController player;
        [SerializeField] private FighterCombatController enemy;
        [SerializeField] private DuelManager duel;
        [SerializeField] private PrototypeCPUController cpu;
        [SerializeField] private FightHUD hud;

        private GameFlowState state = GameFlowState.Title;
        private GameDifficulty difficulty = GameDifficulty.Easy;
        private FighterId selectedFighter = FighterId.Leyla;
        private FighterId selectedEnemy = FighterId.Eleni;
        private int selectedSkin = 1;
        private int selectedArena = 1;
        private float tryAgainHoldStarted = -1f;
        private Coroutine resultRoutine;

        public GameFlowState State => state;

        public void Configure(FighterCombatController playerFighter, FighterCombatController enemyFighter,
            DuelManager duelManager, PrototypeCPUController cpuController, FightHUD fightHud)
        {
            player = playerFighter;
            enemy = enemyFighter;
            duel = duelManager;
            cpu = cpuController;
            hud = fightHud;
        }

        private void Start()
        {
            if (duel != null)
            {
                duel.MatchFinished -= OnMatchFinished;
                duel.MatchFinished += OnMatchFinished;
            }
            EnterFrontend(GameFlowState.Title);
        }

        private void OnDestroy()
        {
            if (duel != null) duel.MatchFinished -= OnMatchFinished;
        }

        private void EnterFrontend(GameFlowState next)
        {
            state = next;
            if (cpu != null) cpu.enabled = false;
            if (hud != null)
            {
                hud.SetVisible(false);
                hud.SetOverlay("");
            }

            if (player != null) player.SetFightActive(false);
            if (enemy != null) enemy.SetFightActive(false);
        }

        private void StartFight()
        {
            FighterProfile playerProfile = FighterCatalog.Get(selectedFighter);
            FighterProfile enemyProfile = FighterCatalog.Get(selectedEnemy);
            FighterStats playerStats = playerProfile.CreateRuntimeStats();
            FighterStats enemyStats = enemyProfile.CreateRuntimeStats();

            FighterMotor3D playerMotor = player.GetComponent<FighterMotor3D>();
            FighterMotor3D enemyMotor = enemy.GetComponent<FighterMotor3D>();
            playerMotor.Configure(playerStats, enemy.transform);
            enemyMotor.Configure(enemyStats, player.transform);
            playerMotor.SetAIControlled(false);
            enemyMotor.SetAIControlled(true);

            Hitbox playerHitbox = player.GetComponentInChildren<Hitbox>(true);
            Hitbox enemyHitbox = enemy.GetComponentInChildren<Hitbox>(true);
            player.Configure(playerStats, enemy, playerHitbox);
            enemy.Configure(enemyStats, player, enemyHitbox);
            player.SetAIControlled(false);
            enemy.SetAIControlled(true);
            enemy.SetDifficulty(difficulty);
            player.SetFightActive(true);
            enemy.SetFightActive(true);

            player.name = $"{playerProfile.DisplayName} [Combination {selectedSkin}]";
            enemy.name = enemyProfile.DisplayName;

            duel.StartNewMatch();
            cpu.Configure(enemyMotor, enemy, player.transform);
            cpu.SetDifficulty(difficulty);
            cpu.enabled = true;

            hud.Configure(player, enemy, duel, playerProfile.DisplayName, enemyProfile.DisplayName, $"Arena {selectedArena}");
            hud.SetVisible(true);
            hud.SetOverlay("");

            playerMotor.SetInputEnabled(true);
            enemyMotor.SetInputEnabled(true);
            state = GameFlowState.Fight;
        }

        private void OnMatchFinished(FighterCombatController winner)
        {
            if (resultRoutine != null) StopCoroutine(resultRoutine);
            resultRoutine = StartCoroutine(ResultSequence(winner == player));
        }

        private IEnumerator ResultSequence(bool playerWon)
        {
            cpu.enabled = false;
            player.SetFightActive(false);
            enemy.SetFightActive(false);

            hud.SetOverlay("K.O.");
            yield return new WaitForSecondsRealtime(2f);
            hud.SetOverlay(playerWon ? "YOU WIN" : "YOU LOSE");
            yield return new WaitForSecondsRealtime(3f);
            hud.SetVisible(false);
            state = GameFlowState.Result;
            tryAgainHoldStarted = -1f;
            resultRoutine = null;
        }

        private void OnGUI()
        {
            if (state == GameFlowState.Fight) return;

            float panelWidth = Mathf.Min(620f, Screen.width - 40f);
            float panelHeight = Mathf.Min(720f, Screen.height - 40f);
            Rect area = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);

            GUI.Box(area, "");
            GUILayout.BeginArea(new Rect(area.x + 28f, area.y + 24f, area.width - 56f, area.height - 48f));

            GUIStyle title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 34,
                fontStyle = FontStyle.Bold
            };

            switch (state)
            {
                case GameFlowState.Title:
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("HETENA TATAKAI 3D", title);
                    GUILayout.Space(30f);
                    if (GUILayout.Button("START", GUILayout.Height(62f))) state = GameFlowState.Difficulty;
                    GUILayout.FlexibleSpace();
                    break;

                case GameFlowState.Difficulty:
                    GUILayout.Label("DIFFICULTY", title);
                    GUILayout.Space(20f);
                    if (GUILayout.Button("EASY", GUILayout.Height(54f)))
                    {
                        difficulty = GameDifficulty.Easy;
                        state = GameFlowState.FighterSelect;
                    }
                    GUILayout.Label("CPU reacts slower. Its D6 has a 10% chance to roll 6.");
                    GUILayout.Space(12f);
                    if (GUILayout.Button("HARD", GUILayout.Height(54f)))
                    {
                        difficulty = GameDifficulty.Hard;
                        state = GameFlowState.FighterSelect;
                    }
                    GUILayout.Label("CPU reacts faster and receives a +15% high-roll opportunity.");
                    DrawBackButton(GameFlowState.Title);
                    break;

                case GameFlowState.FighterSelect:
                    GUILayout.Label("CHOOSE YOUR FIGHTER!", title);
                    GUILayout.Space(12f);
                    DrawFighterButtons(true);
                    DrawBackButton(GameFlowState.Difficulty);
                    break;

                case GameFlowState.SkinSelect:
                    FighterProfile profile = FighterCatalog.Get(selectedFighter);
                    GUILayout.Label($"{profile.DisplayName.ToUpperInvariant()} — SKIN", title);
                    GUILayout.Space(18f);
                    for (int i = 1; i <= profile.SkinCount; i++)
                    {
                        int skin = i;
                        if (GUILayout.Button($"Combination {skin}", GUILayout.Height(48f)))
                        {
                            selectedSkin = skin;
                            state = GameFlowState.ArenaSelect;
                        }
                    }
                    DrawBackButton(GameFlowState.FighterSelect);
                    break;

                case GameFlowState.ArenaSelect:
                    GUILayout.Label("CHOOSE ARENA", title);
                    GUILayout.Space(12f);
                    for (int i = 1; i <= 10; i++)
                    {
                        int arena = i;
                        if (GUILayout.Button($"Arena {arena}", GUILayout.Height(38f)))
                        {
                            selectedArena = arena;
                            state = GameFlowState.EnemySelect;
                        }
                    }
                    DrawBackButton(GameFlowState.SkinSelect);
                    break;

                case GameFlowState.EnemySelect:
                    GUILayout.Label("CHOOSE YOUR ENEMY", title);
                    GUILayout.Space(12f);
                    DrawFighterButtons(false);
                    DrawBackButton(GameFlowState.ArenaSelect);
                    break;

                case GameFlowState.Result:
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("MATCH COMPLETE", title);
                    GUILayout.Space(24f);
                    DrawTryAgain();
                    GUILayout.FlexibleSpace();
                    break;
            }

            GUILayout.EndArea();
        }

        private void DrawFighterButtons(bool selectingPlayer)
        {
            var roster = FighterCatalog.All;
            for (int i = 0; i < roster.Count; i++)
            {
                FighterProfile profile = roster[i];
                if (GUILayout.Button(profile.DisplayName, GUILayout.Height(38f)))
                {
                    if (selectingPlayer)
                    {
                        selectedFighter = profile.Id;
                        state = GameFlowState.SkinSelect;
                    }
                    else
                    {
                        selectedEnemy = profile.Id;
                        StartFight();
                    }
                }
            }
        }

        private void DrawBackButton(GameFlowState target)
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("BACK", GUILayout.Height(42f))) state = target;
        }

        private void DrawTryAgain()
        {
            Rect rect = GUILayoutUtility.GetRect(320f, 72f, GUILayout.ExpandWidth(true));
            float heldFor = tryAgainHoldStarted < 0f ? 0f : Time.unscaledTime - tryAgainHoldStarted;
            float progress = Mathf.Clamp01(heldFor / 2f);
            string label = progress <= 0f ? "HOLD TRY AGAIN — 2s" : $"TRY AGAIN  {Mathf.RoundToInt(progress * 100f)}%";

            bool held = GUI.RepeatButton(rect, label);
            if (held)
            {
                if (tryAgainHoldStarted < 0f) tryAgainHoldStarted = Time.unscaledTime;
                if (Time.unscaledTime - tryAgainHoldStarted >= 2f)
                {
                    tryAgainHoldStarted = -1f;
                    duel.StartNewMatch();
                    EnterFrontend(GameFlowState.Title);
                }
            }
            else
            {
                tryAgainHoldStarted = -1f;
            }
        }
    }
}
