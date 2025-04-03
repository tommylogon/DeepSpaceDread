# DeepSpaceDread Technical Context

## Core Technologies
- **Unity 2022.3.x**
- **Universal Render Pipeline (URP)**
  - Lighting_Renderer.asset
  - URP GlobalSettings.asset
- **Input System**
  - New Input System package
  - PlayerControls.inputactions
- **UI Toolkit**
  - GameUI.uxml
  - UXML Schema files present

## Development Environment
- **Project Structure**
  - Assets organized by type (Scripts, Prefabs, Scenes)
  - Animation controllers grouped together
  - Audio files categorized
- **Dependencies**
  - NavMeshPlus (navigation system)
  - Unity's Input System
  - URP packages

## Build Pipeline
- Target platforms: Windows, possibly others
- URP configured for 2D/3D hybrid rendering
- Asset bundles planned for optimization

## Coding Standards
- C# scripts follow Unity conventions
- ScriptableObjects for data
- Event system for communication
- Editor scripts for custom tooling (RaycastTentaclesEditor.cs)
