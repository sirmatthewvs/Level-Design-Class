# Setup Tools

This folder contains the automated setup wizard and configuration presets for the 3D Game Kit Lite.

---

## Contents

### GameKitSetupWizard.cs
**Automatic Setup Wizard** that runs when the package is imported.

**Features:**
- Checks Cinemachine version (ensures 2.x, not 3.x)
- Applies Tag & Layer configuration
- Applies NavMesh settings (Humanoid + Chomper agents)
- Verifies Input System setup

**Menu Access:**
- Tools > 3D Game Kit > Setup Wizard (manual run)
- Tools > 3D Game Kit > Reset Setup (force re-run)

**How It Works:**
- `[InitializeOnLoad]` attribute triggers on editor startup
- Checks `EditorPrefs` to see if setup completed
- Shows wizard window if first import or reset
- Stores completion state to avoid repeated prompts

---

### TagManager.preset
**Layer and Tag Configuration** preset file.

**Includes:**
- **Custom Tags:** FlythroughCam
- **Custom Layers:**
  - Layer 9: Player
  - Layer 16: Environment
  - Layer 23: Enemy
  - Layer 24: Cameras
  - Layer 29: Checkpoint
  - And many more for the complete kit

**Applied by:** Setup Wizard "Apply Tag & Layer Preset" button

**Why preset format?**
Presets allow modifying ProjectSettings programmatically without manual Inspector editing.

---

### NavMeshSettings.preset
**NavMesh Agent Types** preset file.

**Includes:**
- **Humanoid Agent** (ID: 0)
  - Radius: 0.5
  - Height: 2.0
  - Climb: 0.75
  
- **Chomper Agent** (ID: -1372625422)
  - Radius: 0.5
  - Height: 1.0
  - Climb: 0.6

- **Areas:**
  - Walkable (cost: 1)
  - Not Walkable (cost: 1)
  - Jump (cost: 2)

**Applied by:** Setup Wizard "Apply NavMesh Settings Preset" button

---

## Technical Details

### Why Presets?

Unity's ProjectSettings files (Tags, Layers, NavMesh) can't be directly exported with packages. They're project-global settings stored in `ProjectSettings/` directory.

**Solution:** Preset files
- Can be included in packages
- Programmatically applied via `SerializedObject`
- Non-destructive (only modify specified properties)

### Code Structure

```csharp
// Loading preset
Preset preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);

// Applying to ProjectSettings
SerializedObject settings = new SerializedObject(
    AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
);

preset.ApplyTo(settings.targetObject);
settings.ApplyModifiedProperties();
```

### Namespace Note

This folder uses namespace `Gamekit3D.SetupTools` instead of `Gamekit3D.Editor` to avoid conflicts with Unity's built-in `UnityEditor.Editor` class.

---

## For Educators

**When distributing the package:**
1. These files MUST be included in the export
2. Students should see the Setup Wizard automatically on first import
3. If wizard doesn't appear, they can manually run: Tools > 3D Game Kit > Setup Wizard

**Troubleshooting:**
- Wizard shows even after completion → Check EditorPrefs key `GameKit3D_SetupComplete_v1`
- Presets not found → Verify files exist in SetupTools folder
- Settings not applying → Check Unity console for errors

---

## Version History

**v1.0:**
- Initial setup wizard implementation
- TagManager preset with all custom layers
- NavMeshSettings preset with Humanoid + Chomper
- Automatic Cinemachine version checking
- Integration with custom GPT for student support

---

**This folder is essential for package distribution. Do not delete or modify without testing!**
