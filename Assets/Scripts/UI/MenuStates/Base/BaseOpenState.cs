using UnityEngine.Device;

namespace UI.MenuStates
{
    public abstract class BaseOpenState : BaseMenuAnimationState
    {
        public override MenuState state => MenuState.Open;

        protected abstract bool CanClose();


        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            if (CanClose()) ChangeState(MenuState.Closing);
        }

        public BaseOpenState(AnimatedMenu form) : base(form)
        {
        } 
    }
}