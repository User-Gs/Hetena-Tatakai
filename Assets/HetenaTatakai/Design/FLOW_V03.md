# v0.3 — Full 3D Game Flow

Implemented flow:

**Title → Difficulty → Fighter Select → Skin Select → Arena Select → Enemy Select → Fight → K.O. → You Win / You Lose → Hold Try Again (2s) → Title**

## What is wired, not mocked

- Difficulty is stored in the session and changes CPU reaction speed, guard tendency and attack cadence.
- Easy CPU retains a 10% natural-six chance.
- Hard CPU gets a 15% opportunity to upgrade a low roll into the 4–6 band.
- Fighter selection creates the runtime fighter profile used by the match.
- Skin selection is named Combination 1, Combination 2, etc. and is carried into the player object name for asset hookup.
- Arena selection is persisted into the HUD. Arena geometry is still placeholder until arena assets are supplied.
- Enemy selection drives the CPU fighter.
- Best-of-three score remains active.
- KO sequence shows K.O. for 2 seconds and then YOU WIN / YOU LOSE for 3 seconds.
- Try Again must be held for 2 seconds and then returns to Title.
- Health HUD uses an immediate red health bar with a delayed yellow damage-loss bar.

## Asset hookup points

The current fighters are 3D capsule placeholders. Replacing them with real rigged models does not require rewriting the flow. Hook character prefabs/Animator controllers into the fighter objects, and map the selected FighterId + Combination N to the correct model/material set.

## Next

- Real 3D fighter prefabs and Animator state hooks
- Character portraits / face-close selection cards
- Character-specific passives and provoke system
- Real 10 arena prefabs
- Audio/SFX/voice callouts
- Tournament mode
