using UnityEngine;

namespace UI.MenuStates
{
    public class HoverClosedState : BaseClosedState
    {


        public HoverClosedState(AnimatedMenu form) : base(form)
        {
        }

        public override void OnUpdate()
        {
            if (Input.mousePosition.x <= SideBarWidth && Input.mousePosition.x >= 0 && !MouseOutOfBounds() && MouseOnScreen())
                ChangeState(MenuState.Opening);
        }
    }
}