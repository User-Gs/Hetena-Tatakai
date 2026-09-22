#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HetenaTatakai.EditorTools
{
    public static class PrototypeSceneBuilder
    {
        [MenuItem("Hetena Tatakai/Build Prototype Scene")]
        public static void Build()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateLighting();
            CreateArena();

            FighterStats leylaStats = CreateStatsAsset("Leyla_Prototype", FighterId.Leyla, "Leyla", 3, 3, 3, 3, 3, 3);
            FighterStats eleniStats = CreateStatsAsset("Eleni_Prototype", FighterId.Eleni, "Eleni", 3, 2, 4, 3, 3, 2);

            GameObject leyla = CreateFighter("Leyla", new Vector3(-1.7f, 1f, 0f));
            GameObject eleni = CreateFighter("Eleni", new Vector3(1.7f, 1f, 0f));

            FighterMotor3D leylaMotor = leyla.GetComponent<FighterMotor3D>();
            FighterMotor3D eleniMotor = eleni.GetComponent<FighterMotor3D>();
            FighterCombatController leylaCombat = leyla.GetComponent<FighterCombatController>();
            FighterCombatController eleniCombat = eleni.GetComponent<FighterCombatController>();
            Hitbox leylaHitbox = leyla.GetComponentInChildren<Hitbox>();
            Hitbox eleniHitbox = eleni.GetComponentInChildren<Hitbox>();

            leylaMotor.Configure(leylaStats, eleni.transform);
            eleniMotor.Configure(eleniStats, leyla.transform);
            leylaMotor.SetArena(Vector3.zero, 7.5f);
            eleniMotor.SetArena(Vector3.zero, 7.5f);
            eleniMotor.SetKeys(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);

            leylaCombat.Configure(leylaStats, eleniCombat, leylaHitbox);
            eleniCombat.Configure(eleniStats, leylaCombat, eleniHitbox);
            eleniCombat.SetKeys(KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad0);

            GameObject cameraGo = new GameObject("Fight Camera");
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.fieldOfView = 48f;
            cameraGo.tag = "MainCamera";
            FightCamera3D fightCamera = cameraGo.AddComponent<FightCamera3D>();
            fightCamera.Configure(leyla.transform, eleni.transform);

            GameObject managerGo = new GameObject("Duel Manager");
            DuelManager manager = managerGo.AddComponent<DuelManager>();
            manager.Configure(leylaCombat, eleniCombat);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, "Assets/HetenaTatakai/PrototypeScene.unity");
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = leyla;
            Debug.Log("Hetena Tatakai 3D prototype created. Press Play: WASD + J/K/L/I vs Arrows + Numpad 1/2/3/0.");
        }

        private static void CreateLighting()
        {
            GameObject lightGo = new GameObject("Directional Light");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            lightGo.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }

        private static void CreateArena()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floor.name = "Prototype Arena";
            floor.transform.position = new Vector3(0f, -0.25f, 0f);
            floor.transform.localScale = new Vector3(8f, 0.25f, 8f);
        }

        private static GameObject CreateFighter(string fighterName, Vector3 position)
        {
            GameObject fighter = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fighter.name = fighterName;
            fighter.transform.position = position;

            Object.DestroyImmediate(fighter.GetComponent<CapsuleCollider>());
            CharacterController controller = fighter.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.48f;
            controller.center = Vector3.zero;

            fighter.AddComponent<FighterStateMachine>();
            fighter.AddComponent<FighterHealth>();
            fighter.AddComponent<FighterMotor3D>();
            FighterCombatController combat = fighter.AddComponent<FighterCombatController>();

            Hurtbox hurtbox = fighter.AddComponent<Hurtbox>();
            hurtbox.Configure(combat);

            GameObject hitboxGo = new GameObject("Attack Hitbox");
            hitboxGo.transform.SetParent(fighter.transform, false);
            hitboxGo.transform.localPosition = new Vector3(0f, 0.3f, 0.85f);
            BoxCollider box = hitboxGo.AddComponent<BoxCollider>();
            box.size = new Vector3(0.9f, 1.15f, 0.85f);
            Hitbox hitbox = hitboxGo.AddComponent<Hitbox>();
            hitbox.Configure(combat);

            return fighter;
        }

        private static FighterStats CreateStatsAsset(string assetName, FighterId id, string displayName,
            int energy, int rawPower, int speed, int technique, int morale, int durability)
        {
            const string folder = "Assets/HetenaTatakai/Generated";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/HetenaTatakai", "Generated");

            string path = $"{folder}/{assetName}.asset";
            FighterStats stats = AssetDatabase.LoadAssetAtPath<FighterStats>(path);
            if (stats == null)
            {
                stats = ScriptableObject.CreateInstance<FighterStats>();
                AssetDatabase.CreateAsset(stats, path);
            }

            stats.fighterId = id;
            stats.displayName = displayName;
            stats.energy = energy;
            stats.rawPower = rawPower;
            stats.speed = speed;
            stats.technique = technique;
            stats.morale = morale;
            stats.durability = durability;
            EditorUtility.SetDirty(stats);
            return stats;
        }
    }
}
#endif
