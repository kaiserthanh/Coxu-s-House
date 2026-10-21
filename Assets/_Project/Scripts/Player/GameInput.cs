using System;
using UnityEngine.InputSystem;

namespace CoxusHouse.Player
{
    // Mouse and keyboard actions for one local player. Defined in code so there is no
    // input asset to keep in sync; move to an .inputactions asset if rebinding is needed.
    public sealed class GameInput : IDisposable
    {
        public InputAction Move { get; }
        public InputAction Look { get; }
        public InputAction Sprint { get; }
        public InputAction Interact { get; }
        public InputAction Drop { get; }

        public GameInput()
        {
            Move = new InputAction("Move", InputActionType.Value);
            Move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            Look = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
            Sprint = new InputAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");
            Interact = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
            Drop = new InputAction("Drop", InputActionType.Button, "<Keyboard>/q");
        }

        public void Enable()
        {
            Move.Enable();
            Look.Enable();
            Sprint.Enable();
            Interact.Enable();
            Drop.Enable();
        }

        public void Disable()
        {
            Move.Disable();
            Look.Disable();
            Sprint.Disable();
            Interact.Disable();
            Drop.Disable();
        }

        public void Dispose()
        {
            Move.Dispose();
            Look.Dispose();
            Sprint.Dispose();
            Interact.Dispose();
            Drop.Dispose();
        }
    }
}
