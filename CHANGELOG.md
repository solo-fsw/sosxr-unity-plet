# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [0.5.0] - 2026-11-09

### Changed

- Dependency `com.unity.textmeshpro` (deprecated) replaced by `com.unity.ugui` 2.0.0. TextMeshPro ships inside uGUI in Unity 6.
- Samples moved from hidden `Samples~/` to visible `Samples/`. The sample path in `package.json` is now `Samples/Palettes`.
- `ColorProvider` checks components in a fixed priority order: ParticleSystem, TMP_Text, Text, SpriteRenderer, Selectable, Image, Light, Camera, then any other Renderer.
- `ColorProvider` applies colours in `OnEnable` as well as in the editor. OnValidate work is batched with `EditorApplication.delayCall`.
- `ColorProvider` records edit-mode changes with Undo. Objects are marked dirty only when their serialized data changes.
- `ApplyColor` and `GetColorSV` clamp saturation and value to 1–19 instead of throwing `ArgumentOutOfRangeException`.
- `ColorSettings` defaults are now Saturation 10, Value 10, Alpha 1, and FinalColor white.
- `TextureProvider` applies textures through a `MaterialPropertyBlock` per material slot. The shared material asset is no longer modified.
- `TextureProvider` is `[ExecuteAlways]` and reapplies its textures in `OnEnable`.
- Desaturated textures are saved to and loaded from `Assets/_SOSXR/Resources/Plet/Textures/`. `TextureNames` now store Resources paths.
- `Desaturate` no longer calls `AssetDatabase.Refresh` for each step, and it destroys its temporary texture.
- `PletSceneSettings` changes to the palette, buttons, skybox, ambient light, shadows and fog use Undo, and mark the asset, skybox material or scene dirty when data changes.
- The "Create Palette" button records Undo and assigns the new palette through the serialized property.

### Added

- `ColorProvider.ResolveTarget(GameObject)` and the `ColorProvider.Target` property.
- `ColorProvider.SetCameraClearFlagsToSolidColor` (default true). Set it to false to change only `Camera.backgroundColor`.
- `ColorProvider.UseSceneSettings(PletSceneSettings)` to inject settings from code or tests.
- `TextureSettings.MaterialIndex`, so entries map to the correct material slot.
- `TextureProvider.Reinitialize()` (context menu), `ApplyAllTextures()`, and `IsInitialized`.
- A static `TextureLoader` delegate on `TextureProvider` (replaceable for tests) and `FindExistingDesaturatedTextures`.
- `PletSceneSettings.GetSkyboxMaterialName(string)`.
- PletHelpers: `GetSafeSceneName`, `UntitledSceneName`, `TextureResourcesFolder`, `TextureFolderPath`, `TextureResourcePath`, `DesaturatedTextureName`, and `SaturationPercentageForStep`.
- EditMode test assembly `SOSXR.Plet.Tests` in `Tests/Editor/`.

### Fixed

- Colours now show in builds and after runtime `AddComponent`.
- 3D TextMeshPro and ParticleSystem components are coloured. The generic Renderer check used to win first.
- New Selectable and material slots get palette saturation and value. They no longer stay transparent black, which made `ApplyColor` throw.
- Opening a scene no longer dirties it when colour data has not changed.
- Several `PletSceneSettings` with none matching the scene logs a warning instead of throwing. `IsActiveSceneSettings()` returns false.
- Auto-creation of `PletSceneSettings` runs at most once, never overwrites an existing asset, uses "Untitled" for untitled scenes, and is skipped in Play mode.
- "Create Palette" never overwrites an existing `Palette.asset`. It creates a unique name instead.
- Skybox material is created at most once per name and is never overwritten. Untitled scenes no longer produce an empty name.
- `TextureProvider` material slot and texture index mismatches are fixed by `MaterialIndex`. Out-of-range or missing textures log warnings instead of throwing. A failed load keeps the old index so it retries.
- Desaturated PNGs were saved to one folder and searched in another, so 11 PNGs were regenerated on every init. Legacy Resources-root textures are still found.
- A failed or busy desaturation leaves `TextureProvider` uninitialised, so it retries later.
- Removed the orphan `ResourcesSample_Scene_SOSXR_plet_cubes_plet_skybox.mat` (and its `.meta`).

### Upgrade notes

- Plet now depends on `com.unity.ugui` instead of `com.unity.textmeshpro`. Unity installs it with the package.
- Existing `TextureProvider` components have no `MaterialIndex`, so it defaults to 0. On multi-material renderers, choose **Reinitialize** from the component's context menu.
- New textures go to `Resources/Plet/Textures/`. Old PNGs in the Resources root are still found and not regenerated. Remove them by hand if you want a clean folder.
- `ApplyColor` no longer throws for out-of-range values. It clamps them to 1–19.

## [0.4.0]

No changelog entry was recorded for this version.

## [0.3.0]

### Changed

- Unity "6000.0"
- General fixes in ease of use.
- Changing palette now gets the hue and value of that palette for all involved.

## [0.2.1] - 02-04-2025

- Fixing building errors

## [0.2.0] - In Development

> ### Package Numbering Change
> #### Package will now be numbered starting with 0, to better reflect the current status in development (see the official semver information [here](https://semver.org/#spec-item-4)).
>
> If any issues arise when updating from previous (and higher numbered versions), please delete the old version before updating to this version.

### Changed

- Location where desaturated images are stored: /Assets/_SOSXR/Resources/

### Added

### Fixed

- That the InEditorSamples is now no longer pushed to the repo.
- Requires TextMeshPro

## [1.0.1] - 2025-02-20

### Fixed

- Bugfixes, improvements and optimizations

### Changed

- No longer reliant on a saturation / value 'multiplier'. Instead, the saturation and value are now set directly.

## [1.0.0] - 2025-02-19

### Added

- Created plet

