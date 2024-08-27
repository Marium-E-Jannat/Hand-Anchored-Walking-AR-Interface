using UnityEngine;

public class ButtonState:MonoBehaviour{
    public enum State{
        HOVER, 
        STILL,
    }
    public State state = State.STILL;
}
