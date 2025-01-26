using UnityEngine;

namespace UI.MenuStates
{
    public class BaseOpeningState : BaseMenuAnimationState

    {
        public BaseOpeningState(AnimatedMenu form) : base(form)
        {
        }

        public override MenuState state => MenuState.Opening;
        public override void OnEnter()
        {
            MoveToPosition(TargetPosition);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }
    }
}