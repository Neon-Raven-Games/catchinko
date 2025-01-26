using UnityEngine;

namespace UI.MenuStates.ButtonClickActivatedMenu
{
    public class ClickClosedState : BaseClosedState
    {
        public ClickClosedState(AnimatedMenu form) : base(form)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            animatedMenu.SetButtonText(MenuState.Closed);
            animatedMenu.ToggleButtonClicked();
        }
        
        public override void OnUpdate()
        {
            if (ButtonPressed) ChangeState(MenuState.Opening);
        }
    }
}