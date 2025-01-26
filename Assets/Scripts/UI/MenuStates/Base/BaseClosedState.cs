
namespace UI.MenuStates
{
    public class BaseClosedState : BaseMenuAnimationState
    {
        public override MenuState state => MenuState.Closed;
        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }

        public BaseClosedState(AnimatedMenu form) : base(form)
        {
        }
    }
}