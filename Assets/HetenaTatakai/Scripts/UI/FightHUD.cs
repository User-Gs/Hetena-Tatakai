using UnityEngine;

namespace HetenaTatakai
{
    public sealed class FightHUD : MonoBehaviour
    {
        private FighterHealth playerHealth;
        private FighterHealth enemyHealth;
        private DuelManager duel;
        private string playerName = "PLAYER";
        private string enemyName = "ENEMY";
        private string arenaName = "";
        private string overlay = "";
        private bool visible;

        private float playerDelayed = 1f;
        private float enemyDelayed = 1f;
        private float playerDelayTimer;
        private float enemyDelayTimer;

        public void Configure(FighterCombatController player, FighterCombatController enemy, DuelManager duelManager,
            string selectedPlayerName, string selectedEnemyName, string selectedArenaName)
        {
            playerHealth = player.Health;
            enemyHealth = enemy.Health;
            duel = duelManager;
            playerName = selectedPlayerName;
            enemyName = selectedEnemyName;
            arenaName = selectedArenaName;
            playerDelayed = playerHealth.Normalized;
            enemyDelayed = enemyHealth.Normalized;
            playerDelayTimer = enemyDelayTimer = 0f;
        }

        public void SetVisible(bool value) => visible = value;
        public void SetOverlay(string value) => overlay = value ?? "";

        private void Update()
        {
            if (!visible || playerHealth == null || enemyHealth == null) return;
            UpdateDelayed(ref playerDelayed, playerHealth.Normalized, ref playerDelayTimer);
            UpdateDelayed(ref enemyDelayed, enemyHealth.Normalized, ref enemyDelayTimer);
        }

        private static void UpdateDelayed(ref float delayed, float current, ref float timer)
        {
            if (current < delayed)
            {
                timer += Time.unscaledDeltaTime;
                if (timer > 0.35f)
                    delayed = Mathf.MoveTowards(delayed, current, Time.unscaledDeltaTime * 0.55f);
            }
            else
            {
                delayed = current;
                timer = 0f;
            }
        }

        private void OnGUI()
        {
            if (!visible || playerHealth == null || enemyHealth == null) return;

            float w = Mathf.Min(430f, Screen.width * 0.36f);
            float h = 28f;
            float top = 28f;

            DrawHealthBar(new Rect(35f, top, w, h), playerHealth.Normalized, playerDelayed, false);
            DrawHealthBar(new Rect(Screen.width - 35f - w, top, w, h), enemyHealth.Normalized, enemyDelayed, true);

            GUIStyle nameStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            GUI.Label(new Rect(35f, top + 32f, w, 28f), playerName, nameStyle);
            GUI.Label(new Rect(Screen.width - 35f - w, top + 32f, w, 28f), enemyName, nameStyle);

            GUIStyle center = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            GUI.Label(new Rect(Screen.width * 0.5f - 130f, top, 260f, 30f),
                duel != null ? $"{duel.WinsA}  -  {duel.WinsB}" : "", center);
            GUI.Label(new Rect(Screen.width * 0.5f - 150f, top + 32f, 300f, 26f), arenaName, center);

            if (!string.IsNullOrEmpty(overlay))
            {
                GUIStyle overlayStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = Mathf.Clamp(Screen.height / 10, 42, 92),
                    fontStyle = FontStyle.Bold
                };
                GUI.Label(new Rect(0f, Screen.height * 0.28f, Screen.width, Screen.height * 0.3f), overlay, overlayStyle);
            }
        }

        private static void DrawHealthBar(Rect rect, float current, float delayed, bool rightToLeft)
        {
            Color old = GUI.color;
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            Rect delayedRect = rightToLeft
                ? new Rect(rect.xMax - rect.width * delayed, rect.y, rect.width * delayed, rect.height)
                : new Rect(rect.x, rect.y, rect.width * delayed, rect.height);
            GUI.color = new Color(0.95f, 0.72f, 0.12f, 1f);
            GUI.DrawTexture(delayedRect, Texture2D.whiteTexture);

            Rect currentRect = rightToLeft
                ? new Rect(rect.xMax - rect.width * current, rect.y, rect.width * current, rect.height)
                : new Rect(rect.x, rect.y, rect.width * current, rect.height);
            GUI.color = new Color(0.82f, 0.12f, 0.12f, 1f);
            GUI.DrawTexture(currentRect, Texture2D.whiteTexture);
            GUI.color = old;
        }
    }
}
