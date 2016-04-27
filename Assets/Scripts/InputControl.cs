using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputControl
{
    static public InputControl _s;
    static public InputControl S {
        get {
            if (_s == null) {
                _s = new InputControl();
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
        Vector2 velocity = new Vector2(Input.GetAxis("HorizontalL" + CtrlForPlayer(playerNum)),
                                       Input.GetAxis("VerticalL" + CtrlForPlayer(playerNum)));

        //allow a player to be controlled by keyboard for testing:
        if (playerNum == keyboardPlayer && velocity.magnitude < 0.2f) {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
                                   Input.GetAxisRaw("Vertical"));
        }
        return velocity;
    }

    public virtual Vector2 TackleVelocity(int playerNum)
    {
        Vector2 tackleForce = new Vector2(Input.GetAxis ("HorizontalR" + CtrlForPlayer(playerNum) + platformString),
                                          Input.GetAxis ("VerticalR" + CtrlForPlayer(playerNum) + platformString));
        //keyboard control for testing:
        if (playerNum == keyboardPlayer && tackleForce.magnitude < 0.2f) {
            tackleForce = new Vector2(Input.GetAxisRaw("HorizontalTackle"),
                                      Input.GetAxisRaw("VerticalTackle"));
        }
        return tackleForce;
    }

    public virtual bool ItemButtonDown(int playerNum, int itemNum)
    {
        return Input.GetButtonDown("Item"+itemNum+CtrlForPlayer(playerNum)) ||
               (playerNum == keyboardPlayer && Input.GetKeyDown(itemNum.ToString()));
    }
}
