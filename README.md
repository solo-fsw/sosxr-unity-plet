---
media_link: 
MOC:
related:
tags: []
date_created: 2025-02-25
date_modified: 2025-03-31
---

# Plet: A _Highly_ Opinionated Color Palette Manager for Unity

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Fully open source: Feel free to add to, or modify, anything you see fit.

The reason to create something other than the already amazing [swatchr](https://github.com/jknightdoeswork/swatchr) was that I wanted something that was both much simpler in actual use, and much more opinionated on how to use the colors. I needed something that simply forced the user to use three colors, and then use these three colors in a consistent way throughout the scene. This is why I created `plet`.

## Installation

### Via Package Manager (UPM) (recommended)

1. Open the Unity project you want to install this package in.
2. Open the Package Manager window.
3. Click on the `+` button and select `Add package from git URL…`.
4. Paste the URL of this repo into the text field and press `Add`. Make sure it ends with `.git`.

### Via Unity Package

1. Download the latest release from the release page
2. Import the .unitypackage into your Unity project.
3. You're good to go!

### Via Git (to contribute)

1. Fork this repository to your own Git account, and then clone that into your Unity project's `Assets` folder.
2. You're good to go!
3. If you want to contribute, make a pull request to the main repository.

### Dev Release

For the latest (and possibly unstable) version, you can do any of the above on the `dev` branch. This is where the latest changes are made, and where you can find the latest features. For those using UPM, add `#dev` to the end of the URL (after `.git`).

#### Requirements

- TextMeshPro

## Reasoning

Restrictions are sometimes good. A color palette is one such example. By restricting the colors that can be used in a project, you can ensure a consistent look and feel throughout the scene or project. `plet` ("Palette") is even more opinionated in stating that you should only use __three__ colors in each palette. This is 1) done because you sometimes just have to make choices, and 2) because the [60/30/10 rule](https://youtu.be/RdAEf6A7WwQ?si=dyM5K3SIXHOAaYGp).

### The 60/30/10 rule

This rule states that you should use 60% of one color, 30% of another, and 10% of a third color. This is a good rule of thumb to create a balanced and visually appealing scene.

- The first color is called the 'base' or 'dominant' color, which is often advised to be a neutral color. This is often used for the background, or for the main color of the scene.
- The second color is the 'tone' / 'secondary' color. It can be used for slightly more important elements that need to stand out a bit more. It is usually a color that's contrasting a little with the base color, and is often a bit more saturated and/or brighter.
- The final color is an 'accent' color. This is the color that is used for the (visually most) important elements in the scene, or to create dynamic and vivid scenes. It is often the most saturated and/or brightest color of the three.

## Usage

Check out the Samples folder for examples on how to use the different features of this package. plet consists of three main parts: Palettes, PaletteHolders, and ColorProviders.

### Palette(s)

You can create as many or as few Palettes as you want. Right-click in the Project window, and select `Create -> SOSXR -> plet -> Palette`. If you'd like you can add an image to the Palette to give a visual representation of the colors in the palette. This is not necessary, but can be helpful for designers or artists to quickly see what the colors are.

Then, for ease of use: either get the colorpicker to the right of each color to select a color from your image, or use the RGB sliders to set the color manually. However, for most accurate results, click on each color, and type in the exact HEX value. Name each palette something sensible, so you can easily find it back in the PaletteHolder.

Palettes have useful links at the bottom to W3 Schools' [analogous](https://www.w3schools.com/colors/colors_analogous.asp), [compound](https://www.w3schools.com/colors/colors_compound.asp), and [triadic](https://www.w3schools.com/colors/colors_triadic.asp) color wheels. These can be useful to find colors that work well together. As with images: you can take screenshots from the website and add them to the Palette for easy reference, or copy the HEX values from the website and paste them into the Palette.

### PaletteHolder(s)

Create one or more PaletteHolders in a Resources folder of your choosing. Right-click in the Project window, and select `Create -> SOSXR -> plet -> PaletteHolder`. You can create _either_ one PaletteHolder for your entire project, or one per scene.

- If you create just the one PaletteHolder, you can name this whatever you want, and this PaletteHolder will be used throughout your project.
- If you create multiple PaletteHolders, you _need_ to have (exactly) one PaletteHolder per scene in your project. This would allow you to have different palettes in different scenes. Each PaletteHolder _needs_ to have the same name as the scene it governs. Note that you can only edit the PaletteHolder while you're in the scene with the same name.

You cannot have more than one PaletteHolder governing a scene, as this would cause conflicts. __In short: either have 1 PaletteHolder for your entire project, or 1 per scene.__

Add your previously created Palette to the PaletteHolder in the inspector. Right below you'll see the three colors displayed.

Further below you have a few checkboxes. Their use is discussed later in more detail. Safe to say: they should do what they say they do. Once checked, more options appear below.

### Color Providers

Add the ColorProvider component to all GameObjects that you want to be able to change color on. It currently works on:

- SkinnedMeshRenderers
- MeshRenderers
- ParticleSystems
- Light components
- Camera components

Once on the GameObject, it will grab any of these components that it can work with (on the same level, not children, they need their own ColorProvider component). I think it will probably break if you have multiple of these components on the same GameObject, but I haven't tested that.

Next, use the dropdown to select the 'color type': Base, Tone, or Accent color. This will determine which color (or more precisely: which hue) from the palette will be used.

On each instance of the ColorProvider component, you can set a 'value' (brightness) of the color in the palette.

A similar thing can be done with the saturation 1-19 scale (10 being middle), which makes the color more grey, or more vibrant.

### Changing Palettes (a.k.a. the fun part)

You have two options to change the palette that is being used in the scene:

1. Adjust the colors of the currently used palette. This can be done by selecting the PaletteHolder of the scene, and finding the used palette there. Click on it, and adjust the colors as you see fit.
2. Create a new palette, and assign it to the PaletteHolder.

The second option is preferable. This way, you can always go back to the original palette, and you can easily switch between palettes. This is honestly one of the best features of this package. You can easily switch between palettes, and see how the scene changes. It works because each instance of the ColorProvider component is not directly linked to the palette, but to the PaletteHolder. This way, when you change the palette in the PaletteHolder, all ColorProviders will update to use the new palette, while remembering its own value and saturation multipliers, so you can easily switch between palettes, and still keep the look and feel of the scene.

### The checkboxes (PaletteHolder)

- 'Apply Skybox': This will set the skybox in the Lighting tab to the skybox. If you set a skybox in the Lighting tab, it will be overwritten by the skybox in the PaletteHolder. If you uncheck this, the skybox in the Lighting tab will be used. It will look for a material called 'plet_skybox' in the Resources folder. If you don't want to use that one, you can supply your own (drag it forcefully into the 'Skybox' field in the PaletteHolder, you cannot unset the plet skybox without setting a new one). The Shader of the skybox material should be 'Shadergraph/TriColorSkybox' (from the excellent tutorial by [Digvijaysinh Gohil](https://www.youtube.com/watch?v=ZENOA_YFve0)), or at the very least have three public color values called "SkyColor", "HorizonColor", and "GroundColor". You can set which of the three colors (Base, Tone, Accent) should be used for each of these three colors. Multipliers are supplied. Check out the material / shader as well, since it has some more options there.
  Possibly best to have multiple skyboxes for different scenes, if you go down this route.
- 'Apply Ambient Light': if in your Lighting -> Environment settings you have the 'Environment Lighting' set to either 'Color' or 'Gradient', and you check this box, you can supply either one or three colors. 'Color' only needs one color, while 'Gradient' needs three.
- 'Apply Realtime Shadows': You can set the color of the shadows here. You can set one color.
- 'Apply Fog': if you have fog enabled in the Lighting tab, you can set the color of the fog here. You can set one color.

### Further things to note

When working with Materials: it will not update the material directly, but create an instance of it, and modify that. This is usually useful (and kind of required), it can be a bit of a bummer when the same Material is used in multiple places, and you want to set them all to the same color. In short: you can't, and will have to manually set the ColorProvider to each Renderer. When working with either a Renderer or SkinnedMeshRenderer which has multiple materials, it will grab all materials and allow you to change them. On the left of each 'block' / material (in the ColorChanger) the name of each material will be printed.

## Samples and Further Information

The Samples folder holds a couple dozen palette examples, screenshotted from two YouTube videos: [Movie LUTs YouTube](https://www.youtube.com/watch?v=RdAEf6A7WwQ) and [Florent Farges](https://www.youtube.com/watch?v=p0rVUhXnmpY). These are great examples of how you can use palettes to create a consistent look and feel throughout your scene. The videos also give some more information on the how and
why of the 60/30/10 rule.

## Specifics

### TextMeshPro Dropdown (Not Yet Implemented)

Add one to the Dropdown GameObject, and another to the 'Image' directly on the 'Template', and a final one to the 'Toggle' that's way down inside the 'Template' gameobject. Put another on hte Label (for text color), and one on the 'Item Label' (in the 'Item' gameobject).
