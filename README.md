# Hetena Tatakai 3D

Unity 6.x prototype scaffold for the 3D version of **Hetena Tatakai**.

## v0.2 combat core

- Leyla vs Eleni one-click prototype scene
- 3D movement + sidestep + auto-facing
- Collider-based hitboxes and hurtboxes
- State machine for movement, attacks, guard, hit-stun, knockdown and KO
- Light punch / heavy punch / kick / guard
- D6 combat damage constrained to 1-6
- Stat hooks for Raw Power / Technique / Energy
- Guard chip damage
- Critical reaction on a natural 6
- Knockback and hit-stun
- Combo connection tracking
- Best-of-three round manager
- Dynamic fight camera

## Start

1. Create a new **Unity 6.x 3D** project.
2. Copy `Assets/HetenaTatakai` into your project's `Assets` directory.
3. In Unity choose **Hetena Tatakai > Build Prototype Scene**.
4. Open/Play `Assets/HetenaTatakai/PrototypeScene.unity`.

### Player 1
`WASD` move, `J` light punch, `K` heavy punch, `L` kick, hold `I` guard.

### Player 2
Arrow keys move, Numpad `1` light punch, Numpad `2` heavy punch, Numpad `3` kick, hold Numpad `0` guard.

> The starter intentionally uses Unity's classic Input API so it launches without extra packages. If a project is configured for the new Input System only, enable **Both** input backends or swap the input layer later.
