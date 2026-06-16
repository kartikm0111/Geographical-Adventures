# Planetary Explorer

This project transforms the original Geographical Adventures airplane simulator into a 3rd-person planetary explorer with spherical locomotion, custom interactable artifacts using ScriptableObjects, and a brand new UI system.

## Setup Instructions
1. Open the project in Unity 2021.3.2f1 or later.
2. From the top menu bar, click **Geographical Adventures > 1. Setup UI and Mechanics**.
3. This will automatically strip the old UI, generate the new Futuristic Surveyor UI Canvas, instantiate the ScriptableObject artifacts (Ancient Sword and Laser Blaster), drop them onto the spherical terrain, and configure the Player character.
4. Open the `Assets/Scenes/Game.unity` scene if it isn't open already.
5. Click Play!

## Controls
- **W, A, S, D / Arrow Keys**: Move the character along the spherical surface.
- **Mouse Drag (Left or Right Click)**: Orbit the planetary camera around the player.
- **E / Space**: Pick up the nearest artifact, or drop the currently held artifact.

## Architecture Overview
- **Spherical Locomotion**: The `Player.cs` controller aligns its `up` vector perfectly with the surface normal (`transform.position.normalized`). All WASD inputs are projected onto the spherical tangent plane so movement feels completely natural anywhere on the globe.
- **Planetary Orbit Camera**: `GameCamera.cs` uses `Quaternion.Slerp` and `Vector3.SmoothDamp` to orbit the player relative to their dynamic Up vector, perfectly avoiding gimbal lock at the poles.
- **Artifact System**: Data for the artifacts are driven by `ArtifactData` ScriptableObjects (Name, Weight, Icon). `InteractableArtifact.cs` attaches to physical objects on the globe, and `PlayerInteraction.cs` handles the pickup logic and emits events.
- **Event-Driven UI**: `UIManager.cs` listens for static events from `PlayerInteraction.cs` to show/hide the `CarryingHUD` using smooth, programmatic CanvasGroup fade and slide transitions, decoupled entirely from the `Update` loop physics logic.

## Build Instructions (WebGL / Android)
1. Ensure you have the WebGL or Android Build Support modules installed via Unity Hub.
2. Go to **File > Build Settings**.
3. Select **WebGL** or **Android** and click **Switch Platform**.
4. Click **Build and Run**.
