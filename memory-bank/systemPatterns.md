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
   - Environmental sounds (spaceship ambience)
   - Creature sounds (alien movements)
   - Interactive object sounds (doors, terminals)

4. **UI System**
   - Built with UI Toolkit
   - GameUI.uxml defines layout
   - Style sheets for theming

5. **Game State**
   - ScriptableObjects for persistent data
   - Event system for state changes
   - Save/load system planned

## Component Relationships
```mermaid
flowchart TD
    Input[Input System] --> Player[Player Controller]
    Player --> Animation[Animation System]
    Player --> Physics[Physics System]
    Physics --> Audio[Audio System]
    GameState --> UI[UI System]
    UI --> GameState
