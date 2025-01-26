using UnityEngine;

namespace UI.MenuStates
{
    public class ClosingState : BaseMenuAnimationState
    {
        public override MenuState state => MenuState.Closing;
        public override void OnEnter()
        {
            MoveToPosition(OriginalPosition);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }

        public ClosingState(AnimatedMenu form) : base(form)
        {
        }
    }
}