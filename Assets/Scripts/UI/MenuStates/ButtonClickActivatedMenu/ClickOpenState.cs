using UnityEngine.Device;

namespace UI.MenuStates.ButtonClickActivatedMenu
{
    public class ClickOpenState : BaseOpenState
    {
        public ClickOpenState(AnimatedMenu form) : base(form)
        {
        }

        protected override bool CanClose() =>
            ButtonPressed;
    }
}