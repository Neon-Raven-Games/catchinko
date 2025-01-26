using System.Collections.Generic;
using UI.MenuStates;
using UI.MenuStates.ButtonClickActivatedMenu;

public class StateController
{
    private Dictionary<MenuState, BaseMenuAnimationState> _states = new();
    private readonly AnimatedMenu animatedMenu;

    public StateController(AnimatedMenu form) : base()
    {
        animatedMenu = form;
        if (animatedMenu.menuType == MenuType.HoverActivated) InitHoverActivatedMenu();
        else InitButtonClickActivatedMenu();

        InitSharedStates();

        animatedMenu.currentState = _states[MenuState.Closed];
    }

    private void InitSharedStates() =>
        _states.Add(MenuState.Closing, new ClosingState(animatedMenu));

    private void InitButtonClickActivatedMenu()
    {
        _states.Add(MenuState.Opening, new ClickOpeningState(animatedMenu));
        _states.Add(MenuState.Open, new ClickOpenState(animatedMenu));
        _states.Add(MenuState.Closed, new ClickClosedState(animatedMenu));
    }

    private void InitHoverActivatedMenu()
    {
        _states.Add(MenuState.Opening, new BaseOpeningState(animatedMenu));
        _states.Add(MenuState.Open, new HoverOpenState(animatedMenu));
        _states.Add(MenuState.Closed, new HoverClosedState(animatedMenu));
    }

    public void ChangeState(MenuState state)
    {
        if (animatedMenu.State == state) return;
        animatedMenu.currentState.OnExit();
        animatedMenu.currentState = _states[state];
        animatedMenu.currentState.OnEnter();
    }
}