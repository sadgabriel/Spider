using System.Collections.Generic;
using UnityEngine;


namespace SpecialAction
{
    abstract class SpecialAction
    {
        public bool IsActive { get; private set; } = false;
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
            InputManager.Instance.OnKeyDown += HandleKeyDown;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            InputManager.Instance.OnMouseButtonDown -= HandleMouseButtonDown;
            InputManager.Instance.OnKeyDown -= HandleKeyDown;
        }

        protected abstract void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO);
        protected abstract void HandleKeyDown(HashSet<KeyCode> pressedKeys);
    }
}
