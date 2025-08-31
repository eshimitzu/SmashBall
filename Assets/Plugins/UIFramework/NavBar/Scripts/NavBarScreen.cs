
using UIFramework.Components.NavBar;

namespace UIFramework.Components.NavBar
{
    // Every screen that wants to have a button on the NavBar should inherit from this class.
    public class NavBarScreen : UIScreen
    {
        // If you want to change notifications you can do it through this screen directly
        public virtual void OnScreenInit(NavButton owner) { }
    }
    
}