# Deep Space Dread - Code Manual

This manual provides a comprehensive overview of the scripts and systems within the Deep Space Dread project, explaining how they work together and how to make adjustments.

## 1. System Architecture Overview

Deep Space Dread is built in Unity using a component-based architecture. It utilizes Unity's **Universal Render Pipeline (URP)** for a 2D/3D hybrid visual style and the **Input System package** for player controls.

### Core Communication Patterns
- **Singletons/Static Managers**: Used for global systems like `GameHandler`, `UIController`, `SoundManager`, and `GameAssets`.
- **Interfaces**: Used to decouple systems.
    - `IInteractable`: For any object that can be interacted with.
    - `IDamage`: For any entity that can take damage (Player, potentially AI).
    - `IController`: General interface for entity controllers.
- **Events and Delegates**: Used for decoupled communication, e.g., `GameHandler.OnNoiseGenerated` for the AI hearing system, and UI button click events.

---

## 2. Core Systems

### 2.1 Player System
The player is managed by several interconnected components:
- **`PlayerController.cs`**: The main hub for player logic. Handles state (Alive, Dead, Sleeping), visibility calculation, flashlight management, and bridges input to actions.
- **`TDPlayerMovement.cs`**: Handles physics-based movement using `Rigidbody2D`. It supports walking, sprinting, and a "Zero-G" mode.
- **`Resource.cs`**: Manages the player's health (and potentially other resources). It triggers death when empty.
- **`InventoryController.cs`**: Manages items the player is carrying (currently focused on throwable objects).
- **`PlayerSounds.cs`**: Handles player-specific audio like footsteps.

### 2.2 AI System
The alien AI uses a state machine to navigate and hunt the player:
- **`AIController.cs`**: The main AI state machine. States include `Hunting`, `Hungry`, `Chasing`, `Attacking`, `Stunned`, `Investigating`, and `Eating`. It uses a `NavMeshAgent` for navigation.
- **`AIPerception.cs`**: Handles how the AI "sees" and "hears" the player. It subscribes to `GameHandler.OnNoiseGenerated` and performs raycasts for visual detection.
- **`AlienSounds.cs`**: Manages AI-specific audio cues (screams, footsteps, eating sounds).

### 2.3 Interaction System
A modular system for environmental interactions:
- **`IInteractable.cs`**: Interface requiring an `Interact()` method.
- **`Interactable.cs`**: Base class for interactable objects. It detects player proximity and displays UI prompts. It can hold multiple `InteractionModule`s.
- **`InteractionModule.cs`**: Abstract base for specific interaction behaviors.
    - **`InteractionMessageModule.cs`**: Displays text from a `MessageDatabase`.
    - **`InteractionSoundModule.cs`**: Plays specific sounds.
    - **`InteractIndicatorModule.cs`**: Shows visual indicators.
- **Specialized Interactables**:
    - `Door.cs`, `Locker.cs` (hiding), `InteractReactor.cs` (complex puzzle), `ThrowableObject.cs`.

### 2.4 UI System
Built with Unity's **UI Toolkit**:
- **`UIController.cs`**: Manages the main game UI, including the HUD, interaction prompts, messages, and game over screens.
- **`UI_Reactor.cs`**: Handles the specific UI for the reactor puzzle.
- **`UI_PauseMenu.cs`**: Manages the pause state and menu.
- **`TypewriterEffect.cs`**: Provides animated text display for messages.

### 2.5 Audio System
- **`SoundManager.cs`**: A static utility for playing sounds globally. It retrieves clips from `GameAssets`.
- **`GameAssets.cs`**: A singleton ScriptableObject-based provider for audio clips, prefabs, and other global assets.

---

## 3. How-To Guide: Making Adjustments

### 3.1 Adding a New Interactable Object
1. Create a new GameObject with a `Collider2D` (set to Is Trigger).
2. Add the `Interactable.cs` component.
3. Set the `Interact Text` (e.g., "Press E to open drawer").
4. Add specific `InteractionModule` components (like `InteractionMessageModule` for lore).
5. Drag these modules into the `Interaction Modules` list on the `Interactable` component.

### 3.2 Adjusting AI Behavior
- **Detection Range**: Modify `View Radius` and `View Angle` in the `AIPerception` component.
- **Movement Speed**: Adjust `Walking Speed` and `NavMeshAgent` settings in the `AIController`.
- **Hunger Logic**: Adjust `Hunger Rate` and `Hunger Threshold` in `AIController` to change how often the alien searches for corpses.

### 3.3 Adding New Sounds
1. Add the new audio clip to the `SoundAudioClipArray` in the `GameAssets` prefab/asset.
2. Add a new entry to the `Sound` enum in `SoundManager.cs`.
3. Call `SoundManager.PlaySound(SoundManager.Sound.YourNewSound)` in your script.

---

## 4. Script Reference

| Script | Path | Purpose |
| :--- | :--- | :--- |
| `GameHandler` | `Assets/Scripts/GameHandler.cs` | Global game state and event dispatcher (e.g., noise generation). |
| `PlayerController` | `Assets/Scripts/Controllers/PlayerController.cs` | Main player logic, input handling, and state management. |
| `AIController` | `Assets/Scripts/Controllers/AIController.cs` | Alien state machine and behavior logic. |
| `AIPerception` | `Assets/Scripts/Controllers/.../AIPerception.cs` | AI sensory logic (sight and hearing). |
| `UIController` | `Assets/Scripts/UI/UIController.cs` | Orchestrates UI Toolkit elements and game menus. |
| `Interactable` | `Assets/Scripts/Interactable/Interactable.cs` | Base component for any player-interactable object. |
| `SoundManager` | `Assets/Scripts/SoundManager.cs` | Static class for playing audio clips. |
| `TDPlayerMovement` | `Assets/Scripts/Controllers/.../TDPlayerMovement.cs` | Top-down movement physics for the player. |
| `RaycastTentacles` | `Assets/Scripts/RaycastTentacles.cs` | Procedural tentacle generation using raycasts. |
| `TentacleMover` | `Assets/Scripts/TentacleMover.cs` | Controls the movement/animation of procedural tentacles. |
| `FieldOfView` | `Assets/Scripts/FieldOfView.cs` | Handles visual FOV mesh generation. |
| `FlickerLight` | `Assets/Scripts/FlickerLight.cs` | Simulates malfunctioning electrical lights. |
| `Resource` | `Assets/Scripts/Resource.cs` | Generic health/resource tracker. |
| `MessageDatabase` | `Assets/Scripts/MessageDatabase.cs` | ScriptableObject for storing and retrieving game text/lore. |
| `Timer` | `Assets/Scripts/Timer.cs` | Manages countdowns and timed game events. |
| `CameraFade` | `Assets/Scripts/CameraFade.cs` | Handles screen transitions (fade in/out). |
| `DirectionIndicator` | `Assets/Scripts/DirectionIndicator.cs` | Points towards targets (e.g., enemy indicator). |
| `RotateObject` | `Assets/Scripts/RotateObject.cs` | Simple utility for rotating objects over time. |
| `AnimatedSpriteModule`| `Assets/Scripts/AnimatedSpriteModule.cs` | Handles sprite-based animations for entities. |
