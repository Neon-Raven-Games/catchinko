using Screen = UnityEngine.Device.Screen;

namespace UI.MenuStates
{
    public class HoverOpenState : BaseOpenState
    {
        public HoverOpenState(AnimatedMenu form) : base(form)
        {
        }
        
        protected override bool CanClose()
        {
            return MouseOutOfBounds();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}