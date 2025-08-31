# UI Framework

This framework provides screen/popup/panel/window management.

## Basic usage

Create `UISettings` asset, set canvas options and create a layer.

Create a screen script with a base class of `UIScreen`, create a prefab and attach the script
```c#
public class LevelUpPopup : UIScreen
{
    ...
}
```
Assign the prefab to a layer in the `UISettings`

Create UI frame and initialize it
```c#
_uiFrame = UISettings.BuildUIFrame();

_uiFrame.Initialize();
```

Open/Close the screen
```c#
_uiFrame.Open<LevelUpPopup>();
...
_uiFrame.Close<LevelUpPopup>();
```

## UISettings

`UISettings` is a scriptable object with a collection of settings, prefabs and screen options. 
It is used for creating layers, adjusting sorting orders and customizing the screen behaviours.


To create `UISettings.asset`, right click on the project view and select `Create -> Voodoo -> UI Settings`.

### Canvas Settings

Main canvas used by the ui framework is created at runtime. 
You can customize the settings for the created canvas here.

Render Mode, Sorting Layer Name and Order in Layer are the required settings.

Recommended settings are:
```
Render Mode : Screen Space Overlay
Sorting Layer Name: UI
Order in Layer: 0
```

### CanvasScaler Settings

CanvasScaler is used for adjusting canvas scale for different aspect ratios.

Screen Match Mode, Reference Resolution and Reference PPU are the required settings.

Recommended settings are:
```
Screen Match Mode : Expand
Reference Resolution: 1080x1920(for portrait games) 1920x1080(for landscape games)
Reference PPU: 100
```

### Layers

The main canvas contains layers. Layers are used for grouping screens. 
You can create as many layers as you wish. However, keep in mind that layers are independent. 
Layer order in the list is the sorting order for the layers. If a layer is in the bottom, it will be drawn over other layers.

#### Panel Layer Type

Panels are a generic screens like widgets, hud or fullscreen windows. 

There is no specific behaviour attached to panel layers. 

Sorting orders have to be adjusted manually by simply dragging the screens in the list. 
The order in the list defines the sorting order. If a screen is in the bottom, it will be drawn over others.


#### Popup Layer Type

Popups have a custom logic for sorting and background blocking. 

Order in the screen list is ignored for the popups. 

Last opened popup is always shown in front of others. (It will be set as last sibling)

There is a background blocker in the popup layers. It is used for making a dark background and detecting outside clicks.

### Screen settings

#### Load on Demand
By default, all registered screens are created when the `UIFrame` is initialized. 
If `LoadOnDemand` is enabled, the screen will be created when it is opened for the first time.

#### Destroy on Close
By default, when a screen is closed, it will remain in the canvas.
If `DestroyOnClose` is enabled, the screen will be destroyed when closed.
If it is going to open again, it will instantiated. 

#### Close with Escape
If `CloseWithEscape` is enabled, pressing the back button on an Android will close the popup. 
It is ignored for panels.
You can also use Escape button in the editor to get the same behaviour.

#### Close with Bg Click
If `CloseWithBgClick` is enabled, pressing the background blocker(the dark bg) will trigger closing the popup.
It is ignored for panels.

----------------------------

## UIFrame

`UIFrame` is the main controller for all the layers.
To create `UIFrame`, call `BuildUIFrame()` on `UISettings`:

```c#
var _uiFrame = UISettings.BuildUIFrame();
```

Building the `UIFrame` will instantiate the canvas in the scene with the layers. 

**IMPORTANT**: Screens are not created yet at this point.

You should call `Initialize()` method on `UIFrame` to create initial screens.

```c#
_uiFrame.Initialize();
```

After initializing, you can start opening/closing screens through `UIFrame`:

```c#
_uiFrame.Open<LevelUpPopup>();
...
_uiFrame.Close<LevelUpPopup>();
```

### UIFrame.Open

```c#
_uiFrame.Open<LevelUpPopup>();

_uiFrame.Open(typeof(LevelUpPopup));

_uiFrame.Open<ChestPopup>(new ChestPopupProperties(_rewards));

_uiFrame.Open(typeof(ChestPopup), new ChestPopupProperties(_rewards));
```

### UIFrame.Close

```c#
_uiFrame.Close<LevelUpPopup>();

_uiFrame.Close(typeof(LevelUpPopup));
```

### UIFrame.GetScreen

```c#
var levelUpPopup = _uiFrame.GetScreen<LevelUpPopup>();
```

### UIFrame.GetLayer

```c#
var popupLayer = _uiFrame.GetLayer<LevelUpPopup>(); // Layer of the screen

OR

var popupLayer = _uiFrame.GetLayer("PopupLayer");
```
```c#
var visibleScreens = popupLayer.GetVisibleScreens(); // Only instantiated ones

var screensInLayer = popupLayer.GetAllScreens(); // Only instantiated ones

bool isAnyScreenVisible = popupLayer.IsAnyScreenVisible();
```

### UIFrame.GetAllLayers
```c#
var layers = _uiFrame.GetAllLayers();
```

### UIFrame.IsVisible
A screen is visible when it is opening, opened or closing
```c#
bool isVisible = _uiFrame.IsVisible<LevelUpPopup>();
```

### UIFrame.IsOpen
A screen is open when it has completed opening transition
```c#
bool isVisible = _uiFrame.IsOpen<LevelUpPopup>();
```

### UIScreen.RequestUIInteractionBlock / UIScreen.RequestUIInteractionUnblock
You can use them to manually disable interactions of the whole UIFrame.
Make sure that block/unblock called equally, since it used a counter internally.
```c#
_uiFrame.RequestUIInteractionBlock();
...
_uiFrame.RequestUIInteractionUnblock();
```

### UIScreen.IsInteractable
You can use this to check if the UIFrame is interactable or not.
```c#
var isInteractable = _uiFrame.IsInteractable();
```

### UIScreen.AddScreenInfo / UIScreen.RemoveScreenInfo
If you want to dynamically add or remove screen infos, use these methods.
Adding already existing prefabs and removing a visible screen are not allowed.
```c#
_uiFrame.AddScreenInfo("PopupLayer", new ScreenInfo
{
    Prefab = _settingsPopupPrefab,
    LoadOnDemand = false,
    DestroyOnClose = false,
    CloseWithEscape = true,
    CloseWithBgClick = true
});
```
```c#
_uiFrame.RemoveScreenInfo<SettingsPopup>();
```

## UIScreen

`UIScreen` is base definition of all the UI panels/popups/widgets/windows etc. 
It is a monoBehaviour component. It is used for customizing UI scripts and receiving events like OnOpening/OnClosing.

```c#
public class ConfirmationPopup : UIScreen
{
    protected override void OnCreated()
    {
        ...
    }

    protected override void OnOpening()
    {
        ...
    }
    
    protected override void OnOpened()
    {
        ...
    }

    protected override void OnClosing()
    {
        ...
    }
    
    protected override void OnClosed()
    {
        ...
    }
    
    protected override void OnDestroyed()
    {
        ...
    }
}
```

### UIScreen.OnCreated

`OnCreated` is called when the UIScreen is instantiated.
If DestroyOnClose is enabled for the screen, `OnCreated` is called for every time the screen is instantiated.
Otherwise it will be called for one time on the first instantiation.

### UIScreen.OnOpening

`OnOpening` is called when the UIScreen is about to start transition for opening. 
Transition is not started at this point. Ideal for setting up the screen before showing.

During transition, interactions are disabled.

### UIScreen.OnOpened

`OnOpened` is called when the opening transition is complete.
Player can interact with the screen after this.

If no transition is defined, it will be called right after `OnOpening`

### UIScreen.OnClosing

`OnClosing` is called when the UIScreen is about to start transition for closing.
Transition is not started at this point.

During transition, interactions are disabled.

### UIScreen.OnClosed

`OnClosed` is called when the closing transition is complete.
Player can interact with the other screens after this.

If no transition is defined, it will be called right after `OnClosing`

### UIScreen.OnDestroyed

`OnDestroyed` is called when the UIScreen is destroyed.
If DestroyOnClose is enabled for the screen, `OnDestroyed` is called for every time the screen is destroyed.
Otherwise it will be called for one time on the `UIFrame` destroy.

### UI_Close

It is a shortcut method to close the screen itself. For example, it can be connected to a close button callback:
```c#
public class ConfirmationPopup : UIScreen
{
    public Button _closeButton;
    
    protected override void OnCreated()
    {
        _closeButton.onClick.AddListener(() => {
            UI_Close();
        });
    }
}
```

## Screen Properties

Most of the time, we want to open a screen with supplying custom data. 
This is done by implementing `IScreenProperties` to a custom class and adding it to `UIScreen` definition as a Generic Type.
We can pass the properties to `UIFrame.Open` methods.
Any screen that implements `UIScreen` base, can access to supplied properties.

```c#
public class ThemePopup : UIScreen<ThemePopupProperties>
{
    public TextMeshProUGUI Title;
    
    protected override void OnOpening()
    {
        Title.text = Properties.ThemeMeta.name;
    }
}

public class ThemePopupProperties : IScreenProperties
{
    public ThemeMeta ThemeMeta;

    public ThemePopupProperties(ThemeMeta themeMeta)
    {
        ThemeMeta = themeMeta;
    }
}

...

_uiFrame.Open<ThemePopup>(new ThemePopupProperties(_themeMeta));

```

## Events

There are couple of events you can subscribe to get notified about created, destroyed, opened or closed screens.
You can register to events on `UIFrame` level and `UILayer` level.

All events are called before screen override method calls. For example `OnScreenOpening` event is called before `protected override void OnOpening()` 

```c#
_uiFrame = UISettings.BuildUIFrame();

_uiFrame.OnScreenCreated += (screen) => { ... };
_uiFrame.OnScreenOpening += (screen) => { ... };
_uiFrame.OnScreenOpened += (screen) => { ... };
_uiFrame.OnScreenClosing += (screen) => { ... };
_uiFrame.OnScreenClosed += (screen) => { ... };

_uiFrame.Initialize();
```

```c#
var popupLayer = _uiFrame.GetLayer("PopupLayer");

popupLayer.onScreenCreated += (screen) => { ... };
popupLayer.onScreenOpening += (screen) => { ... };
popupLayer.onScreenOpened += (screen) => { ... };
popupLayer.onScreenClosing += (screen) => { ... };
popupLayer.onScreenClosed += (screen) => { ... };
```

## Transitions

Transitions are used for animating opening and closing of a screen.

It is done by assigning transition components to a screen in the inspector (animIn and animOut)

All transitions expected to inherit from `UITransition` and override `Animate` method.
given callback needed to be called when a transition finished.

During transitions, canvas interactions are disabled.

```c#
public class ExampleTransition : UITransition
{
    public override void Animate(Transform target, Action onTransitionCompleteCallback)
    {
        // Animate
        ...
        onTransitionCompleteCallback();
    }
}
```

## Safe Area

`SafeArea` is a helper script for dealing notched screens.

Simply attach to a fully stretched RectTransform and it will adjust the RectTransform to the safe area on runtime.
Using the Device Simulator in the editor is advised to preview before shipping the game.

If a panel uses a full screen background image, create an immediate child and put the component on that instead, with all other elements childed below it.
This will allow the background image to stretch to the full extents of the screen behind the notch and children to stay in safe area.

For other cases that use a mixture of full horizontal and vertical background stripes, use the X & Y controls on inspector to separate elements as needed.

## Non Drawing Graphic

`NonDrawingGraphic` is a helper component for having invisible UI raycasters without rendering.
Simply attach it a to a game object without other Graphic components like Image.

For example, it can be used to make an invisible button. 

## UI Particle System

If you want to have particles in the UI Canvas, you need to use `UIParticleSystem`. 
It scales the particle system to make it visible in the canvas.
Simply attach it a to a game object with a particle system.