# Package Export Guide

Complete guide for exporting the 3D Game Kit Lite as a Unity package.

---

## Understanding the Warnings vs Errors

### ✅ Warnings (SAFE - Ignore These)

When exporting, you'll see warnings like:

```
Dependency asset Packages/com.unity.postprocessing/... is in package Post Processing.
Dependency asset Packages/com.unity.render-pipelines.universal/... is in package Universal Render Pipeline.
```

**These are COMPLETELY NORMAL!**

**What they mean:** Your assets reference files from Unity packages (URP, Post Processing, Cinemachine, etc).

**Why it's OK:** Your `package.json` already declares these dependencies. When students import your package, Unity automatically installs all required packages.

**Action:** Ignore these warnings. They're informational only.

---

### ❌ Errors (MUST FIX)

Errors like:

```
Shader error in 'TextMeshPro/...: Couldn't open include file '...'
```

**These are PROBLEMS!**

**What they mean:** You're trying to export Unity packages (like TextMesh Pro) that should be installed separately.

**Solution:** Remove those folders and add as dependencies in `package.json` instead.

---

## About SRP / URP Setup

### Your Project IS Already Using URP!

**Check your `package.json`:**
```json
"dependencies": {
    "com.unity.render-pipelines.universal": "14.0.0",
    ...
}
```

This project uses **Universal Render Pipeline (URP)**, which is Unity's Scriptable Render Pipeline (SRP).

**What this means:**
- ✅ The project is properly configured for URP
- ✅ Materials use URP shaders
- ✅ Render pipeline asset is configured
- ✅ Students who import this will get URP automatically

**You don't need to "convert to SRP" - you're already using it!**

---

## Pre-Export Checklist

Before exporting, verify these steps:

### 1. ✅ Convert Materials to URP (CRITICAL!)

**Problem:** If materials use Built-in RP shaders, they'll be purple in URP projects!

**Check if conversion needed:**
1. Select any material: `Assets/3DGamekitLite/Art/Materials/Characters/Ellen/Ellen_Body_Mat`
2. Look at Inspector → **Shader** field
3. If it says **"Standard"** or **"Legacy"** → MUST CONVERT
4. If it says **"Universal Render Pipeline/Lit"** → Already converted ✅

**How to convert:**

**Option A: Automatic Converter (Recommended)**
1. Window → Rendering → **Render Pipeline Converter**
2. Select: **Built-in to URP**
3. Check: Materials, Scenes, Prefabs
4. Click **Convert Assets**
5. Wait for completion
6. Verify materials now use URP shaders

**Option B: Manual Conversion**
1. Navigate to `Assets/3DGamekitLite/Art/Materials/`
2. Select all materials in each subfolder
3. Inspector → Shader dropdown
4. Change **Standard** to **Universal Render Pipeline → Lit**
5. Repeat for all material subfolders

**After conversion:**
- Open Demo scene
- Character should NOT be purple
- All materials should look correct

### 2. ✅ Remove Built-in Unity Packages

**Already done (if you deleted TextMesh Pro folder):**
- ❌ Deleted `TextMesh Pro` folder
- ✅ Added `com.unity.textmeshpro` to dependencies

**Check for others:**
Look in `Assets/3DGamekitLite/Packages/` and ensure no Unity packages are there:
- ❌ ProBuilder (if present)
- ❌ Cinemachine (if present)
- ❌ Any other Unity packages

**Keep these (custom code):**
- ✅ DefaultPlayables
- ✅ Interactive
- ✅ SceneManagement
- ✅ SimpleSFX
- ✅ Skybox3D
- ✅ WorldBuilding

### 3. ✅ Verify package.json

Your `package.json` should list ALL Unity packages your project needs:

```json
{
  "name": "com.gamekit.3dgamekitlite",
  "version": "1.0.0",
  "dependencies": {
    "com.unity.cinemachine": "2.10.5",
    "com.unity.inputsystem": "1.7.0",
    "com.unity.postprocessing": "3.2.2",
    "com.unity.render-pipelines.universal": "14.0.0",
    "com.unity.timeline": "1.7.0",
    "com.unity.ai.navigation": "1.1.0",
    "com.unity.textmeshpro": "3.0.6"
  }
}
```

### 4. ✅ Test in Current Project

Before exporting:
1. Open Demo scene
2. Press Play
3. Verify everything works:
   - Player moves
   - Camera works (both modes)
   - Enemies chase
   - Interactables work
   - No console errors

### 5. ✅ Clean Unity Cache

Sometimes Unity holds references to deleted files:

**In Unity Editor:**
1. Close Unity
2. Delete these folders in your project root:
   - `Library/`
   - `Temp/`
3. Reopen Unity (will rebuild cache)
4. Verify no errors

---

## Export Instructions

### Step 1: Select What to Export

**In Unity:**
1. Project window → Find `3DGamekitLite` folder
2. Right-click → **Export Package...**

### Step 2: Review Export Dialog

**IMPORTANT - Deselect these if present:**
- [ ] `.git` files
- [ ] `.vs` folder
- [ ] `Library` folder
- [ ] Any `.meta` files for deleted folders

**MUST include:**
- [x] All scripts (.cs files)
- [x] All prefabs
- [x] All scenes
- [x] All materials and shaders
- [x] All models and textures
- [x] `package.json`
- [x] All documentation (.md files)
- [x] `SetupTools/` folder with presets

### Step 3: Export Settings

**Filename:** `3DGameKitLite_v1.0.unitypackage`

**Options:**
- [x] Include dependencies (already handled by package.json)
- [ ] Include project settings (already handled by presets)

### Step 4: Click Export

Unity will create the `.unitypackage` file.

**Expected warnings:** Dependency warnings (ignore these!)

**Unexpected errors:** Shader errors, missing file errors (fix these!)

---

## Testing the Exported Package

### Critical: Test in Fresh Project

**Never skip this step!** Bugs only appear in fresh projects.

1. **Create NEW Unity project:**
   - Unity Hub → New Project
   - Template: **3D (URP)**
   - Unity 2022.3 LTS

2. **Import your package:**
   - Assets → Import Package → Custom Package
   - Select your `.unitypackage`
   - Import all

3. **Setup Wizard should appear:**
   - If not: Tools → 3D Game Kit → Setup Wizard
   - Click through all setup steps
   - Click "Complete Setup"

4. **Test everything:**
   - Open Demo scene
   - Press Play
   - Test player movement
   - Test camera modes (Manual and Auto)
   - Test enemies
   - Test interactables
   - Check console for errors

5. **Test in new scene:**
   - Create new scene
   - Drag Ellen prefab
   - Drag CameraRig prefab
   - Press Play
   - Should work immediately

---

## Common Export Issues & Solutions

### Issue: "Missing script references"

**Cause:** Scripts moved or deleted

**Fix:**
1. Open Prefabs folder
2. Find prefabs with warnings
3. Reassign missing scripts
4. Re-export

### Issue: "Pink/magenta/purple materials"

**Cause:** Materials using Built-in RP shaders instead of URP shaders

**Fix:**
1. In SOURCE project (before exporting):
   - Window → Rendering → Render Pipeline Converter
   - Convert Built-in to URP
   - OR manually change all materials to URP shaders
2. Verify materials look correct in Demo scene
3. Re-export package
4. Test in fresh URP project - should work now

### Issue: "Package.json not found after import"

**Cause:** Didn't include in export

**Fix:**
1. Re-export
2. Verify `package.json` is checked in export dialog

### Issue: "Setup Wizard doesn't run" or "Compilation errors about Cinemachine not found"

**Cause:** Dependencies not installed before importing package

**Root cause:** When using `.unitypackage` files, Unity doesn't automatically install dependencies from `package.json`. This only works with UPM packages.

**Fix:**
1. Window > Package Manager
2. Install required packages manually:
   - com.unity.cinemachine@2.10.5
   - com.unity.inputsystem@1.7.0
   - com.unity.postprocessing@3.2.2
   - com.unity.ai.navigation@1.1.0
   - com.unity.textmeshpro@3.0.6
3. Wait for Unity to install and compile
4. Console errors should disappear
5. Setup Wizard should appear automatically

**Prevention:** Always install dependencies BEFORE importing the package!

### Issue: "Students get different Cinemachine version"

**Cause:** Package Manager installs latest unless specified

**Fix:**
- Already handled! Your `package.json` specifies exact version: `"com.unity.cinemachine": "2.10.5"`

---

## Distribution Checklist

Before giving to students:

- [ ] Tested in fresh Unity 2022.3 project
- [ ] Manually installed all dependencies FIRST (Cinemachine, Input System, Post Processing, AI Navigation)
- [ ] Then imported package
- [ ] Demo scene works perfectly
- [ ] No console errors
- [ ] Setup Wizard runs automatically after import
- [ ] Both camera modes work
- [ ] All interactables work
- [ ] Created test scene from scratch - works
- [ ] Documentation is clear (especially installation steps)
- [ ] AI helper link works

---

## File Size Expectations

**Typical package size:** 100-300 MB

**If larger than 500 MB:**
- Check for unnecessary files
- Look for large textures (can compress?)
- Check for duplicate assets

**If smaller than 50 MB:**
- Missing assets! Check export dialog carefully

---

## Version Control Recommendations

**Tag your export:**
```bash
git tag v1.0.0-release
git push origin v1.0.0-release
```

**Keep export record:**
Create `CHANGELOG.md`:
```markdown
# Version 1.0.0 (2026-01-18)

## Features
- Dual-mode camera system (Manual + Auto)
- Complete Input System integration
- Automatic setup wizard
- Custom GPT integration for student support

## Assets Included
- Player character (Ellen)
- Enemy (Chomper)
- 12+ interactive prefabs
- Complete demo scene
- Full documentation

## Dependencies
- Unity 2022.3 LTS or newer
- Cinemachine 2.10.5
- Input System 1.7.0
- Universal RP 14.0.0
- TextMesh Pro 3.0.6
```

---

## Student Distribution

**CRITICAL: Students must install dependencies BEFORE importing the package!**

### What To Give Students:

1. **The `.unitypackage` file**
   - Upload to Google Drive / OneDrive / Dropbox
   - Create shareable link

2. **Installation instructions** (from Readme.md):
   - Step 1: Install dependencies via Package Manager
   - Step 2: Import the package
   - Step 3: Run Setup Wizard

3. **Link to AI helper:**
   - https://chatgpt.com/g/g-688f7adaa6d88191896972ca59a2d7fa-gio-game-dev-teacher

### Why This Order Matters:

**If students import BEFORE installing dependencies:**
- ❌ Compilation errors (Cinemachine not found, etc)
- ❌ Setup Wizard can't run (scripts won't compile)
- ❌ Confusing experience for beginners

**If students install dependencies FIRST:**
- ✅ Package imports cleanly
- ✅ Scripts compile immediately
- ✅ Setup Wizard runs automatically
- ✅ Everything works perfectly

### Alternative: Unity Asset Store
- More visibility
- Automatic dependency management (handles this automatically!)
- Automatic updates
- But requires approval process

---

## Summary

**What to expect when exporting:**

✅ **Normal (ignore):**
- Dependency warnings for URP, Post Processing, etc.
- "Asset is in package X" warnings
- Hundreds of these warnings

❌ **Problems (must fix):**
- Shader compilation errors
- Missing script references
- File not found errors

**Your project status:**
- ✅ Already using URP (SRP)
- ✅ TextMesh Pro removed and added as dependency
- ✅ Package.json configured correctly
- ✅ Ready to export!

**Next steps:**
1. Follow "Export Instructions" above
2. Test in fresh project (CRITICAL!)
3. Fix any issues found
4. Re-export if needed
5. Distribute to students

---

**You're ready to export! The warnings you saw are normal. Just make sure to test in a fresh project before distributing to students.**
