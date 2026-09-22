using System;
using System.Collections;
using UnityEngine;

namespace HetenaTatakai
{
    public sealed class DuelManager : MonoBehaviour
    {
        [SerializeField] private FighterCombatController fighterA;
        [SerializeField] private FighterCombatController fighterB;
        [SerializeField] private int roundsToWin = 2;
        [SerializeField] private float roundEndDelay = 2f;

        private Vector3 spawnA;
        private Vector3 spawnB;
        private Quaternion rotA;
        private Quaternion rotB;
        private int winsA;
        private int winsB;
        private bool roundEnding;
        private Coroutine roundRoutine;

        public int WinsA => winsA;
        public int WinsB => winsB;
        public event Action<int, int> ScoreChanged;
        public event Action<FighterCombatController> MatchFinished;

        public void Configure(FighterCombatController a, FighterCombatController b)
        {
            fighterA = a;
            fighterB = b;
            spawnA = a.transform.position;
            spawnB = b.transform.position;
            rotA = a.transform.rotation;
            rotB = b.transform.rotation;
            Subscribe();
        }

        private void OnEnable() => Subscribe();
        private void OnDisable() => Unsubscribe();

        private void Subscribe()
        {
            if (fighterA != null)
            {
                fighterA.Health.KnockedOut -= OnKnockout;
                fighterA.Health.KnockedOut += OnKnockout;
            }
            if (fighterB != null)
            {
                fighterB.Health.KnockedOut -= OnKnockout;
                fighterB.Health.KnockedOut += OnKnockout;
            }
        }

        private void Unsubscribe()
        {
            if (fighterA != null) fighterA.Health.KnockedOut -= OnKnockout;
            if (fighterB != null) fighterB.Health.KnockedOut -= OnKnockout;
        }

        public void StartNewMatch()
        {
            if (roundRoutine != null)
            {
                StopCoroutine(roundRoutine);
                roundRoutine = null;
            }

            winsA = 0;
            winsB = 0;
            roundEnding = false;
            ScoreChanged?.Invoke(winsA, winsB);
            ResetTransform(fighterA.transform, spawnA, rotA);
            ResetTransform(fighterB.transform, spawnB, rotB);
            fighterA.ResetForRound();
            fighterB.ResetForRound();
        }

        private void OnKnockout(FighterHealth loser)
        {
            if (roundEnding) return;
            FighterCombatController winner = loser == fighterA.Health ? fighterB : fighterA;
            if (winner == fighterA) winsA++; else winsB++;
            ScoreChanged?.Invoke(winsA, winsB);
            roundRoutine = StartCoroutine(EndRoundRoutine(winner));
        }

        private IEnumerator EndRoundRoutine(FighterCombatController winner)
        {
            roundEnding = true;
            fighterA.GetComponent<FighterMotor3D>().SetInputEnabled(false);
            fighterB.GetComponent<FighterMotor3D>().SetInputEnabled(false);

            if (winsA >= roundsToWin || winsB >= roundsToWin)
            {
                Debug.Log($"MATCH OVER — {winner.name} wins {winsA}-{winsB}");
                MatchFinished?.Invoke(winner);
                yield break;
            }

            Debug.Log($"ROUND OVER — score {winsA}-{winsB}");
            yield return new WaitForSeconds(roundEndDelay);
            ResetTransform(fighterA.transform, spawnA, rotA);
            ResetTransform(fighterB.transform, spawnB, rotB);
            fighterA.ResetForRound();
            fighterB.ResetForRound();
            roundEnding = false;
            roundRoutine = null;
        }

        private static void ResetTransform(Transform target, Vector3 position, Quaternion rotation)
        {
            CharacterController cc = target.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            target.SetPositionAndRotation(position, rotation);
            if (cc != null) cc.enabled = true;
        }
    }
}
