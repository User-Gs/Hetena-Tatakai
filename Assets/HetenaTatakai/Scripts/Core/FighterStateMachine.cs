using System;
using UnityEngine;

namespace HetenaTatakai
{
    public sealed class FighterStateMachine : MonoBehaviour
    {
        [SerializeField] private FighterState currentState = FighterState.Neutral;

        public FighterState CurrentState => currentState;
        public bool CanMove => currentState == FighterState.Neutral || currentState == FighterState.Moving;
        public bool CanAttack => currentState == FighterState.Neutral || currentState == FighterState.Moving;
        public bool CanGuard => currentState == FighterState.Neutral || currentState == FighterState.Moving || currentState == FighterState.Guarding;
        public bool IsLocked => currentState == FighterState.HitStun || currentState == FighterState.Knockdown || currentState == FighterState.KO;

        public event Action<FighterState, FighterState> StateChanged;

        public void SetState(FighterState next)
        {
            if (currentState == FighterState.KO && next != FighterState.Neutral) return;
            if (currentState == next) return;

            FighterState previous = currentState;
            currentState = next;
            StateChanged?.Invoke(previous, next);
        }

        public void ForceReset() => SetState(FighterState.Neutral);
    }
}
