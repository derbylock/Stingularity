using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;

public class MyInputControl
{
    static public MyInputControl _s;
    static public MyInputControl S {
        get {
            if (_s == null) {
                _s = new MyInputControl();
            }
            return _s;
        }
    }


    public const int keyboardPlayer = 3;

    string platformString {
        get {
            return (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer ||
                    Application.platform == RuntimePlatform.OSXWebPlayer ||
                    Application.platform == RuntimePlatform.OSXDashboardPlayer)
                    ? "OSX" : "WIN";
        }
    }

    public string CtrlForPlayer(int playerNum) {
        return (playerNum+1).ToString();
    }

    public virtual Vector2 RunVelocity(int playerNum)
    {
        Vector2 velocity;
        if (playerNum >= InputManager.Devices.Count)
            velocity = Vector2.zero;
        else {
            InputDevice device = InputManager.Devices[playerNum];
            velocity = device.LeftStick.Vector;
        }
        //allow a player to be controlled by keyboard for testing:
        if (playerNum == keyboardPlayer && velocity.magnitude < 0.2f) {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
                                   Input.GetAxisRaw("Vertical"));
        }
        return velocity;
    }

    public virtual Vector2 TackleVelocity(int playerNum)
    {
        Vector2 tackleForce;
        if (playerNum >= InputManager.Devices.Count)
            tackleForce = Vector2.zero;
        else {
            InputDevice device = InputManager.Devices[playerNum];

            tackleForce = device.RightStick.Vector;
        }
        //keyboard control for testing:
        if (playerNum == keyboardPlayer && tackleForce.magnitude < 0.2f) {
            tackleForce = new Vector2(Input.GetAxisRaw("HorizontalTackle"),
                                      Input.GetAxisRaw("VerticalTackle"));
        }
        return tackleForce;
    }

    public virtual bool ItemButtonDown(int playerNum, int itemNum)
    {
        if (playerNum >= InputManager.Devices.Count)
            return false;

        InputDevice device = InputManager.Devices[playerNum];
        if (itemNum == 1) {
            return device.Action1.WasPressed;
        } if (itemNum == 2) {
            return device.Action2.WasPressed;
        } if (itemNum == 3) {
            return device.Action3.WasPressed;
        } if (itemNum == 4) {
            return device.Action4.WasPressed;
        }

        return Input.GetKeyDown(itemNum.ToString());
    }

    public bool Start() {
        for (int i=0; i<4; ++i) {
            InputDevice device = InputManager.Devices[i];
            if (device.GetControl(InputControlType.Start).WasPressed)
                return true;
        }
        return false;
    }
}
