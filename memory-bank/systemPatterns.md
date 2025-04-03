# DeepSpaceDread System Patterns

## Core Architecture
- Unity ECS-inspired organization
- Component-based game objects
- Event-driven systems

## Key Subsystems
1. **Input System**
   - Uses Unity's Input System package
   - PlayerControls.inputactions defines bindings
   - PlayerControls.cs handles input processing

2. **Animation System**
   - State machine based (Animator controllers)
   - Separate controllers for player, aliens, environment
   - Blend trees for movement animations

3. **Audio System**
   - Uses SoundManager.cs for playing sounds
   - Environmental sounds (spaceship ambience)
   - Creature sounds (alien movements)
   - Interactive object sounds (doors, terminals)

4. **Environmental Interactions**
   - RaycastTentacles.cs creates tentacles using raycasts
   - TentacleMover.cs controls tentacle movement and wall attachment
   - Tentacle.cs controls the visual representation of the tentacle using a LineRenderer

5. **UI System**
   - Built with UI Toolkit
   - GameUI.uxml defines layout
   - Style sheets for theming
   - Uses TypewriterEffect.cs for animated text

6. **Game State**
   - ScriptableObjects for persistent data
   - Event system for state changes
   - Uses Timer.cs for managing game time and triggering events
   - Save/load system planned

7. **AI System**
   - Uses NavMeshAgent for movement
   - AIPerception component for target detection
   - Implements the IController interface
   - Uses a State enum for controlling behavior (Hunting, Chasing, etc.)
   - AlienSounds component for audio cues

8. **Effects System**
   - Uses Particle Systems for visual effects
   - Uses Audio Sources for sound effects
   - SimpleEffectController script for managing effects

## Component Relationships
```mermaid
flowchart TD
    Input[Input System] --> Player[Player Controller]
    Player[Player Controller] : Handles movement, interactions, inventory, and visibility
    Player[Player Controller] -- Inventory[InventoryController] : Manages throwable objects
    Player --> Animation[Animation System]
    Player --> Physics[Physics System]
    Physics --> Audio[Audio System]
    GameState --> UI[UI System]
    UI --> GameState
    AI[AI System] --> Physics
    AI --> Audio
    Physics --> Effects[Effects System]
    AI --> Effects
    Player --> Effects
