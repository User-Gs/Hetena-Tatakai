# Hetena Tatakai 3D

Unity 6.x prototype scaffold for the 3D version of **Hetena Tatakai**.

## v0.3 — playable flow

The project now wires the complete prototype loop:

**Title → Difficulty → Fighter Select → Skin Select → Arena Select → Enemy Select → Fight → K.O. → You Win / You Lose → Hold Try Again → Title**

### Included

- 12-fighter roster hook
- Combination 1 / 2 / 3... skin-selection hook
- 10 arena-selection slots
- Easy / Hard CPU
- Easy CPU: 10% natural-six chance
- Hard CPU: faster reactions + 15% high-roll opportunity
- 3D movement and sidestep
- Auto-facing
- Collider hitbox / hurtbox combat
- Light punch / heavy punch / kick / guard
- D6 damage constrained to 1–6
- Hit-stun / critical knockdown / knockback
- Energy fatigue model
- Best-of-three rounds
- Dynamic fight camera
- Health HUD with delayed yellow damage bar
- K.O. → result sequence
- 2-second hold-to-retry behavior

## Start

1. Create/open a **Unity 6.x 3D** project.
2. Copy this repository's Assets/HetenaTatakai into the Unity project's Assets directory.
3. In Unity choose **Hetena Tatakai > Build Full 3D Flow**.
4. Open/Play Assets/HetenaTatakai/HetenaTatakai3D.unity.

### Player controls

WASD move, J light punch, K heavy punch, L kick, hold I guard.

The opponent is CPU-controlled after Enemy Select.

> Real fighter models, animations, portraits, audio and arena art are asset hookup work. The gameplay flow is deliberately separated from those assets so they can be replaced without rebuilding the combat architecture.
