# Smash Ball Prototype

## ⏱ Time Spent
- **Total:** ~5–6 days  
- Breakdown (approx.):  
  - Project setup & DI architecture: 1 day  
  - Core movement & mobile controls: 1 day  
  - Ball mechanics, collision & scoring loop: 1 day  
  - Ability design & implementation (*SuperPowerHit*): 1 day  
  - Game feel (feedback, trails, screenshake, sound variation): 1–1.5 days  
  - Polishing & bug fixing: ~0.5 day  

---

## 🎮 Ability Design
- **Implemented Ability:** *SuperPowerHit*  
- **Intent:** Powerful strike with auto-aim assist, increased damage and speed boost.  
- **Tuning Parameters:** Damage multiplier, ball speed.

---

## 🏗 Architecture Overview
- **Dependency Injection:** Built on **VContainer**, ensuring modular and testable code.  
- **Config-driven design:**  
  - `GameplayConfig` – core rules and match settings.  
  - `HeroesConfig` – hero-specific parameters.  
  - `SuperPowerHit` – ability parameters (cooldown, force, visuals, homing).  
  - `CameraNormalSettings`, `CameraServeSettings` – camera tuning for different gameplay states.  
- **FSM (Finite State Machine):** Used to manage game flow. Provides clear separation of responsibilities and extensibility for new states.  
- **Scenes:**  
  - `InitialScene` – bootstrap (DI scope, managers, UI initialization).  
  - `BattleScene` – main gameplay loop (ball, players, scoring).  
- **Managers:**  
  - `SoundManager` – sfx and music.  
  - `GameFlow` – state transitions, round start, scoring, and victory handling.  
- **UI:** Current implementation is prefab-based with tweens and procedural UI components.  

**Extensibility:** New heroes/abilities/configs can be added without changing core logic. FSM + DI make the project maintainable and expandable.

---

## 🎨 Assets Used
- **Graphics & Audio:** Decompiled assets from the original Smash Ball! (used strictly for prototype purposes).  
- **Plugins:**  
  - Demigiant DOTween (tweens, feedback)  
  - UniRx (reactive programming)  
  - VContainer (dependency injection)  
  - GameFlow, UIEffect, UIFramework  

---

## 🐞 Known Issues / Next Steps
- **Architecture:**  
  - Move gameplay logic out of `MonoBehaviour` into pure domain classes.  
  - Add **Presenters** for UI layer (clean MVP/MVVM separation).  
  - Implement **object pooling** for ball trails, particles, and UI effects.  
- **Content/Assets:**  
  - Clean up unused materials and textures.  
- **Gameplay polish:**  
  - Fine-tune physics for extreme strikes.  
  - Improve mobile joystick handling (deadzone & sensitivity tuning).  
- **Future expansion:**  
  - Improve AI/bot opponent.  
  - More abilities & hero variety.  
  - Extra juice: impact animations, camera zooms, ability-specific VFX.
  - Add vibrations for better game feedback

---

## 📹 Deliverables
- **Unity Project (2022.3.25f1)**  
- **Android APK Build**  
- **Gameplay Video Showcase**
