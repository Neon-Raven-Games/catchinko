namespace UI.MenuStates.ButtonClickActivatedMenu
{
    public class ClickOpeningState : BaseOpeningState
    {
        public ClickOpeningState(AnimatedMenu form) : base(form)
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            animatedMenu.buttonClicked = false;
            animatedMenu.SetButtonText(MenuState.Open);
        }
    }
}