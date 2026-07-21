# 3D Game Kit Lite

A complete third-person character controller package for Unity. Includes player movement, enemies, collectibles, interactive objects, and a dual-mode camera system.

---

## What's Included

**Core Systems:**
- Player controller with movement, jumping, and combat
- Dual-mode camera system (Manual and Auto modes)
- Enemy AI with NavMesh pathfinding
- Interactive objects (switches, doors, pressure pads, counters)
- Collectible system (crystals, health crates, keys)
- Checkpoint and respawn system
- Scene transitions

**Assets:**
- Character model and animations (Ellen)
- Enemy models and AI (Chomper)
- 12+ interactive prefabs ready to use
- Complete demo scene
- Sound effects and music

---

## Requirements

**Unity Version:** 2022.3 LTS or newer

**Required Packages:**
- Cinemachine 2.10.x (must be 2.x, NOT 3.x)
- Input System 1.7.0+
- Universal RP 14.0.0+
- AI Navigation 1.1.0+

---

## Quick Start

### Installation

**IMPORTANT: Install dependencies FIRST, before importing the package!**

#### Step 1: Install Required Packages

Open Unity's Package Manager (Window > Package Manager), then install these packages:

1. **Cinemachine 2.10.5**
   - Click "+" (top-left) → Add package by name
   - Type: `com.unity.cinemachine`
   - Version: `2.10.5`

2. **Input System 1.7.0**
   - Click "+" → Add package by name
   - Type: `com.unity.inputsystem`
   - Version: `1.7.0`

3. **Post Processing 3.2.2**
   - Click "+" → Add package by name
   - Type: `com.unity.postprocessing`

4. **AI Navigation 1.1.0**
   - Click "+" → Add package by name
   - Type: `com.unity.ai.navigation`

5. **TextMesh Pro 3.0.6** (usually pre-installed)
   - If not installed: Click "+" → Add package by name
   - Type: `com.unity.textmeshpro`

**Wait for all packages to finish installing before continuing!**

#### Step 2: Import the 3D Game Kit Package

1. Assets → Import Package → Custom Package
2. Select the `.unitypackage` file
3. Import all assets
4. Wait for Unity to import and compile

#### Step 3: Run Setup Wizard

1. Setup Wizard should open automatically after import
2. If not: Tools > 3D Game Kit > Setup Wizard
3. Click through all setup steps
4. Click "Complete Setup"

### Try the Demo

1. Open `3DGamekitLite/Scenes/Demo.unity`
2. Press Play
3. **Controls:**
   - WASD - Move
   - Mouse - Look around
   - Space - Jump
   - Left Click - Attack
   - E - Interact
   - ESC - Pause

---

## Camera Modes

The package includes two camera control modes for different player skill levels.

### Manual Mode (Traditional)
- Full player control with mouse
- Traditional third-person camera
- Best for: Experienced gamers

### Auto Mode (Beginner-Friendly)
- Camera automatically follows character
- Smoothly stays behind player
- Hold right-click for temporary manual control
- Best for: Beginners, non-gamers, accessibility

**Switch modes:** Select camera in scene → Inspector → Camera Mode dropdown

**More details:** See `Camera System` section below

---

## Using Interactive Prefabs

All interactive objects are in `3DGamekitLite/Prefabs/Interactables/`

### Quick Examples

**Simple Door + Pressure Pad:**
1. Place `DoorSmall` prefab
2. Place `PressurePad` prefab
3. On PressurePad, find `Send Game Command` component:
   - Set Interactive Object → Door's `Game Command Receiver`
   - Set Interaction Type → A
4. On Door, find `Play Animation` component:
   - Set Game Command Type → A (must match)
5. Test: Step on pad → Door opens

**Collectible Counter:**
1. Place `Counter` prefab
2. Place 3 `Crystal` prefabs
3. On each Crystal's `Collectable`, add `Send Game Command`:
   - Interactive Object → Counter
   - Interaction Type → A
4. On Counter:
   - Target Count = 3
   - On Target Reached → Door (or whatever you want to trigger)
5. Test: Collect 3 crystals → Counter triggers

### Available Prefabs

**Triggers:**
- PressurePad - Floor plate trigger
- Switch - Manual interact trigger

**Doors:**
- DoorSmall - Standard doorway
- DoorHuge - Large door

**Collectibles:**
- Crystal - Standard collectible
- HealthCrate - Restores health
- DestructibleBox - Breakable crate

**Hazards:**
- Acid - Damage zone
- DeathVolume - Instant kill zone

**Movement:**
- MovingPlatform - Moves between points

**Logic:**
- Counter - Counts events before triggering

**UI:**
- InfoZone - Displays text messages

---

## Camera System Details

### Manual Mode Settings
- Mouse Sensitivity: How fast mouse moves camera
- Controller Sensitivity: How fast gamepad moves camera

### Auto Mode Settings
- Auto Rotation Speed: How fast camera follows (default: 2)
- Rotation Damping: Smoothing amount (default: 1s)
- Align When Idle: Gently align behind character when stopped (default: yes)
- Idle Align Damping: How slow idle alignment is (default: 3s)

**Tuning Tips:**
- For smoother camera: Increase Rotation Damping
- For faster response: Increase Auto Rotation Speed
- For gentler idle alignment: Increase Idle Align Damping

---

## Building Your Scene

### Minimum Setup
1. Drag `Ellen.prefab` (player) into scene
2. Drag `CameraRig.prefab` into scene
3. Create or ensure EventSystem exists (UI > Event System)

### Recommended Setup
1. Add ground/terrain
2. Place player at start position
3. Add Checkpoint01 prefab near start
4. Place a few DestructibleBox prefabs (practice combat)
5. Add HealthCrate for recovery
6. Build level with interactive objects
7. Test frequently

### Level Design Tips
- Place checkpoints every 1/3 of level
- Tutorial before challenge (use InfoZone to explain)
- Scatter collectibles to encourage exploration
- Place health crates after combat sections
- Use Demo scene as reference

---

## Troubleshooting

### Compilation errors about "Cinemachine not found" or "PostProcessing not found"
**Cause:** Dependencies not installed before importing package

**Fix:**
1. Window > Package Manager
2. Install missing packages (see Installation section above)
3. Wait for Unity to compile
4. Errors should disappear
5. Setup Wizard will then appear

### "Cinemachine 3.x detected"
**Fix:** Window > Package Manager > Cinemachine > Select version 2.10.5 > Install > Restart Unity

### "Missing layers" or objects not interacting
**Fix:** Tools > 3D Game Kit > Setup Wizard > Apply Tag & Layer Preset

### NavMesh agents not working
**Fix:**
1. Tools > 3D Game Kit > Setup Wizard > Apply NavMesh Settings
2. Window > AI > Navigation
3. Select ground objects > Mark as Navigation Static
4. Bake tab > Bake

### Camera won't move
**Fix:** Select CameraRig > Check CinemachineInputProvider component is present

### Input not working
**Fix:** Edit > Project Settings > Player > Active Input Handling > Set to "Input System Package (New)"

---

## File Locations

**Important folders:**
- `Prefabs/` - All prefabs (characters, interactables, utilities)
- `Scenes/` - Demo scene
- `Scripts/Game/` - All gameplay scripts
- `InputSystem_Actions.inputactions` - Input configuration

**Documentation:**
- `Readme.md` - This file
- `TechReadme.md` - Code architecture and technical details
- `Context.md` - AI helper context (for asking questions)

---

## Learning Resources

### Study the Demo Scene
The included Demo scene shows:
- Switch-Counter puzzle system
- Enemy placement strategies
- Collectible distribution
- Checkpoint pacing
- Progressive difficulty

Open it and study how objects are connected!

### For Educators

**Teaching Opportunities:**
- Game design principles (player skill levels, accessibility)
- Input systems (traditional vs beginner-friendly)
- Level design (pacing, difficulty curves)
- Interactive systems (command pattern)
- AI navigation

**Student Exercises:**
- Test both camera modes, discuss pros/cons
- Create a level using interactive prefabs
- Design a puzzle using counters and switches
- Place enemies with different strategies

---

## Support

**For Setup Issues:**
Run Setup Wizard: Tools > 3D Game Kit > Setup Wizard

**For Usage Questions:**
- Check `TechReadme.md` for code details
- Study the Demo scene for working examples

**For Students - AI Helper:**
We have a custom AI assistant ready to help you!

**Chat with Gio (Game Dev Teacher):**  
https://chatgpt.com/g/g-688f7adaa6d88191896972ca59a2d7fa-gio-game-dev-teacher

This AI has full knowledge of the 3D Game Kit Lite. Just:
1. Click the link above
2. Ask your questions directly
3. Get help without needing to code!

Example questions:
- "How do I make a door open with a pressure pad?"
- "Why won't my enemy chase the player?"
- "How do I use the auto camera mode?"

---

## Version

**Current Version:** 1.0.0

**Features:**
- Complete Input System migration
- Dual-mode camera system (Manual + Auto)
- Optimized camera for third-person gameplay
- Automatic setup wizard
- NavMesh presets (Humanoid + Chomper)
- Tag/Layer presets
- Comprehensive documentation

---

## Credits

Based on Unity Technologies' 3D Game Kit, modified for:
- New Input System compatibility
- Dual camera modes for accessibility
- Enhanced documentation for education
- Automated setup process

---

**Ready to create your 3D platformer!**

Start with the Demo scene to see everything in action, then build your own levels using the included prefabs.
