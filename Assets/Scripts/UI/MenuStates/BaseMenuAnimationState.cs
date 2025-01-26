using System.Collections;
using UnityEngine;

namespace UI.MenuStates
{
    public abstract class BaseMenuAnimationState 
    {
        protected readonly AnimatedMenu animatedMenu;

        protected BaseMenuAnimationState(AnimatedMenu form)
        {
            animatedMenu = form;
        }

        public abstract MenuState state { get; }
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();

        protected bool buttonClicked => animatedMenu.buttonClicked;

        /// <summary>
        /// The desired target position to move the UI element to.
        /// </summary>
        protected Vector2 TargetPosition => animatedMenu.targetPosition;

        protected float SideBarWidth => animatedMenu.sideBarWidth;

        protected bool ButtonPressed => animatedMenu.buttonClicked;

        /// <summary>
        /// The original position of the UI element. We assume that the UI element starts at its original position.
        /// </summary>
        protected Vector2 OriginalPosition => animatedMenu.originalPosition;

        private RectTransform MenuRectTransform => animatedMenu.rectTransform;

        private float InputBoundsPadding => animatedMenu.inputBoundsPadding;

        /// <summary>
        /// Initiates the movement of the UI element to a specified target position.
        /// </summary>
        /// <param name="target">The position to move the UI element to.</param>
        protected void MoveToPosition(Vector2 target) =>
            animatedMenu.StartCoroutine(LerpUIElement(MenuRectTransform.anchoredPosition, target,
                animatedMenu.moveDuration));

        /// <summary>
        /// Moves the UI element to it's original position.
        /// </summary>
        private void MoveToOriginalPosition() => MoveToPosition(OriginalPosition);

        /// <summary>
        /// Moves the UI element to it's target position.
        /// </summary>
        private void MoveToTargetPosition() => MoveToPosition(TargetPosition);

        /// <summary>
        /// Coroutine to smoothly move the UI element from its original position to a target position over a specified duration.
        /// </summary>
        /// <param name="originalPosition">The starting position of the UI element.</param>
        /// <param name="targetPosition">The target position to move the UI element to.</param>
        /// <param name="duration">The duration it takes to complete the move.</param>
        /// <returns>IEnumerator for the coroutine.</returns>
        private IEnumerator LerpUIElement(Vector2 originalPosition, Vector2 targetPosition, float duration)
        {
            var elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var fraction = elapsed / duration;
                MenuRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, fraction);
                yield return null;
            }

            MenuRectTransform.anchoredPosition = targetPosition;
            if (targetPosition == TargetPosition)
            {
                yield return new WaitForSeconds(0.25f);
                ChangeState(MenuState.Open);
            }
            if (targetPosition == OriginalPosition) ChangeState(MenuState.Closed);
        }
        
        

        protected bool MouseOutOfBounds() => MouseOutOfBounds(Screen.height, 0) && MouseOnScreen();

        private bool MouseOutOfBounds(float topBound, float bottomBound) =>
            Input.mousePosition.x > MenuRectTransform.rect.xMax + InputBoundsPadding ||
                   Input.mousePosition.y > topBound || Input.mousePosition.y < bottomBound;

        protected static bool MouseOnScreen() =>
            Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width &&
            Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height;

        public void ChangeState(MenuState state) => animatedMenu.ChangeState(state);

        // todo:
        // these were used for simple elements, and the panel was not registering these properly
        // we need to redo this before the menu can be used again
        protected float GetBottomBounds()
        {
            var halfScreenHeight = Screen.height / 2f;
            var halfSizeDeltaY = MenuRectTransform.sizeDelta.y / 2f;

            return halfScreenHeight - halfSizeDeltaY - InputBoundsPadding;
        }

        protected float GetTopBounds()
        {
            var halfScreenHeight = Screen.height / 2f;
            var halfSizeDeltaY = MenuRectTransform.sizeDelta.y / 2f;

            return halfScreenHeight + halfSizeDeltaY + InputBoundsPadding;
        }
    }
}