# Hetena Tatakai 3D Prototype v0.2

## Goal
A mechanically honest 3D conversion of Hetena Tatakai, not a generic button-masher.

## Current playable loop
- Leyla vs Eleni
- Circular arena
- Forward/back movement and sidestep
- Auto-facing opponent
- Light punch, heavy punch, kick and guard
- Collider-based hitbox/hurtbox detection
- Fighter state machine: Neutral / Moving / Attacking / Guarding / HitStun / Knockdown / KO
- D6 damage remains clamped to 1-6
- Stats can influence the roll without escaping the 1-6 band
- Heavy attacks have longer startup/recovery and stronger knockback
- Energy affects fatigue by increasing recovery after the character passes their attack threshold
- Guard reduces damage to chip damage and softens knockback
- Natural 6 creates a stronger critical reaction
- 3-hit combo counter hook
- Best-of-three rounds (first to 2)
- Dynamic side-on fight camera with stable orientation and distance-based FOV

## Controls
Player 1: WASD, J light, K heavy, L kick, I guard
Player 2: Arrows, Numpad 1 light, Numpad 2 heavy, Numpad 3 kick, Numpad 0 guard

## Next production slice
1. Animator hooks for startup/active/recovery states
2. Health bar + delayed yellow damage bar
3. KO / Win / Lose sequence
4. Character-specific passives (Leyla and Eleni first)
5. CPU opponent and difficulty model
6. Character select -> skin select -> arena -> enemy -> fight flow
