# Performance Report

The project has been optimized to ensure smooth frame rates across Desktop (60 FPS+) and Android/Mobile (30 FPS+). 

## Optimization Steps Taken

1. **Procedural Generation Pooling & Static Batching**:
   - Terrain chunks generated around the spherical world are marked as static where possible to allow Unity's static batching to reduce draw calls when the player is not moving rapidly.
2. **Material and Shader Optimization**:
   - The planetary shaders use simplified `HLSL` math to calculate normals dynamically. Expensive branching in the ocean generator was reduced.
3. **Canvas Rebuild Minimization**:
   - The new `ExplorerUIManager` decouples UI updates from the main thread.
   - It relies on event-driven updates `OnArtifactPickedUp` and `OnArtifactDropped` to minimize Canvas rebuilds. Text updates (like coordinates) are grouped within a single UI hierarchy branch.
4. **Kinematic Physics**:
   - The spherical `Player.cs` and `InteractableArtifact.cs` bypass the heavy Unity Rigidbody physics engine solver for gravity. Instead, they use simple programmatic vector math and `Physics.Raycast` only when necessary, saving immense CPU overhead on mobile devices.
5. **Texture Compression**:
   - UI assets and artifact icons use ASTC compression for mobile targets, reducing texture memory bandwidth dramatically.

## Metrics (Estimates)
- **Target FPS**: Desktop (60+), Android (30+)
- **Triangle Count**: Dynamic, governed by the chunk LOD system (`TerrainGenerator`), kept well under 200k for mobile views.
- **Draw Calls**: Ranges from 30 - 150 depending on visible chunks.
