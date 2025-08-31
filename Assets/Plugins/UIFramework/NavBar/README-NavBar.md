# NavBar
A quick way to get a Navbar into your game.

## How to use:

The general idea is the NavBar has NavButtons that point to ScreenControllers when you click them it will animate the screen into view.  The NavBar Select and Unselect are now driven by animations (called `Selected` and `Unselected`) simply change these to make it your look'n feel.


The width of the button is controlled through script not through the animation.  This is because all the buttons need to respond to the screen aspect ratio for a given device and scale properly, and there doesn't seem to be a way to do that nicely in the animator.

## Steps:
1. In the _Game/UISettings/UISettings create a layer named NavBar and add the NavBar controller.
2. In your NavBar prefab fill out the buttons you want (in Nav Buttons), each button has a name, an icon and a screenController.  You probably want 3 or 5 buttons.
3. Replace the art as you see fit.
4. The rest should be automatic, it will call the OnStart of your screenControllers as needed.
