
![](https://cloud.githubusercontent.com/assets/5210346/20105222/5ec0b97c-a603-11e6-8bf9-717b9d0163bb.png)

# About this fork

## Important notes
* Various fields and types had been renamed, therefore this fork is incompatible with the original [minhdu/uiman](https://github.com/minhdu/uiman). If you are using the original source, **DO NOT** upgrade to this fork.

## Example

The original example has been upgraded and moved to [grashaar/uiman-examples](https://github.com/grashaar/uiman-examples)

## Installation

1. First, install [OpenUPM-CLI](https://github.com/openupm/openupm-cli#installation), NodeJS 12 is required

    ```
    npm install -g openupm-cli
    ```

2. Then, install this package

    ```
    openupm add com.minhdu.uiman
    ```

3. Copy everything inside [UIMan/Area51/Resources](https://github.com/grashaar/uiman/tree/upm/UIMan/Area51/Resources) folder to your project's `Assets/Resources` folder

## Usage

The original [minhdu/uiman](https://github.com/minhdu/uiman) could only load UIMan objects (`UIManScreen`, `UIManDialog`, textures and sprites) via `Resources.Load` method.

This fork, however, requires you to write a custom loader that implements `IUIManLoader`. Then you must integrate it into UIMan by calling `UIManLoader.Initilize(IUIManLoader)` before showing any screen or dialog.

This is an [example](https://github.com/grashaar/uiman-examples/blob/master/Assets/Scripts/StartGame.cs) that loads objects provided by the Addressable Asset System.

## Changlog

### 1.4.0
#### Breaking Changes
* `NumberBinder`: rename the field `timeChange` to `duration`
* `ImageFillAmountBinder`: rename the binding field from "float" to "Fill Amount", rename the field `timeChange` to `duration`
* `ProgressBarBinder`: rename the binding field of from "float" to "Value", rename the field `changeTime` to `duration`, remove `tweenValueChange`
* Rename `UIActivityIndicator` to `UIActivity`
* Rename `UIMan.Loading` method to `ShowActivity`

#### Improvements
* Add more binding fields to `ImageFillAmountBinder`, `NumberBinder`, `ProgressBarBinder`, `SliderBinder`, `TextBinder`
* Add `IUnuLogger` interface
* `UnuLogger` is now a static class, all of its members are refactored into a private inner `DefaultLogger` class which implements `IUnuLogger` interface
* `UnuLogger` can now take another implementation of `IUnuLogger` interface as a substitute for the `DefaultLogger`
* Improve `UIActivity` and `UIMan.ShowActivity`
* Add `EnumAdapter<T>`

### 1.3.0
* Support two-way data binding via `TwoWayBinding` class
* Implement `ToggleBinder`, `SliderBinder`, `ScrollbarBinder`, `DropdownBinder`
* Implement `DropdownOptionAdapter` and `DropdownOptionConverter`
* Update `InputFieldBinder` with two-way binding

### 1.2.0
* Support data coversion via `Adapter<T>` and `Converter<TValue, TAdapter>`
* Implement `BoolAdapter`, `IntAdapter`, `FloatAdapter`, `StringAdapter`, `ColorAdapter`
* Implement `BoolConverter`, `IntConverter`, `FloatConverter`, `StringConverter`, `ColorConverter`
* All binders are now using converters for their binding values
* The `Convert(object)` method could be overrided by using a custom adapter that inherits from one of any default adapters

### 1.1.0
* `UIManAssetLoader` has been changed to `UIManLoader`.
* All `UIManLoader.Set` methods have been removed. Now, `UIManLoader` must be initialized by the `UIManLoader.Initialize(IUIManLoader)` method.
* `SpriteAtlasImageBinder` will now get sprites from `SpriteAtlasManager` to reduce memory usage.
* To replace the default atlas manager, create a class that implements `ISpriteAtlasManager` interface, then pass it to `SpriteAtlasManager.Initialize(ISpriteAtlasManager)` method.

### 1.0.0
- Renovate UIMan to support UPM
- Various fix and breaking changes
- Add some binders

# UIMan
Fast and flexible solution for UI development and management with MVVM pattern.

![](https://user-images.githubusercontent.com/5210346/43007666-355fbe32-8c63-11e8-8b82-fb883b334747.png)

### Features
* Support Databinding, Observable and MVVM for implementing UI with uGUI.
* Prefab base UI with async loading.
* UI flow and layer management system.
* UI animation (show, idle, hide).
* UI events.
* Customizable activity indicator.
* Unlimited list (scroll rect).
* Component-based binders.
* Easy to extend and customize.
* Auto code generation.

### Structure

![](https://cloud.githubusercontent.com/assets/5210346/20105012/a95b257c-a602-11e6-8ac3-2429ed30a8e9.png)

### Detail Documentation
https://goo.gl/PyXBBU

### Video Tutorial (In Vietnamese)
https://goo.gl/Wn4Dos

### Github Page
https://minhdu.github.io/uiman/
