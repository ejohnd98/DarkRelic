using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//TODO: move this to the "new" input system
public class DR_InputHandler : MonoBehaviour
{
    public Vector2Int mouseWorldPosition;
    public bool mouseIsInWorld = false;
    class InputState {
        public KeyCode key;
        public float persistCounter = 0.0f;
        public float heldCounter = 0.0f;
        public bool held = false;

        public InputState(KeyCode k){
            key = k;
        }

        public bool KeyPressed(){
            return persistCounter > 0.0f;
        }

        public bool KeyHeld(float threshold = 0.0f){
            return held && heldCounter >= threshold;
        }

        public void ConsumePress(){
            persistCounter = 0.0f;
        }
    }

    public static DR_InputHandler instance;

    public Camera cameraObj;

    public float inputPersistLength = 0.5f;

    KeyCode[] KeysToCheck = {
        KeyCode.UpArrow,
        KeyCode.RightArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.Space,
        KeyCode.G,
        KeyCode.LeftControl,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
        };

    List<InputState> InputStates;
    Dictionary<KeyCode, InputState> KeyDictionary;

    private void Awake() {
        if (instance != null){
            Debug.LogError("InputHandler already exists!");
        }
        instance = this;

        InputStates = new List<InputState>();
        KeyDictionary = new Dictionary<KeyCode, InputState>();
        
        foreach(KeyCode keyCode in KeysToCheck){
            InputState inputState = new InputState(keyCode);
            InputStates.Add(inputState);
            KeyDictionary[keyCode] = inputState;
        }

        cameraObj = DR_GameManager.instance.MainCamera;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < InputStates.Count; i++){
            if (Input.GetKeyDown(InputStates[i].key)){
                InputStates[i].persistCounter = inputPersistLength;
            }else if (InputStates[i].persistCounter > 0.0f){
                InputStates[i].persistCounter -= Time.deltaTime;
            }

            if (Input.GetKey(InputStates[i].key)){
                InputStates[i].held = true;
                InputStates[i].heldCounter += Time.deltaTime;
            }else{
                InputStates[i].held = false;
                InputStates[i].heldCounter = 0.0f;
            }
        }

        mouseIsInWorld = !EventSystem.current.IsPointerOverGameObject();
        if (mouseIsInWorld){
            mouseWorldPosition = GetMouseCellPosition();
        }
    }

    public static bool GetKeyPressed(KeyCode key){
        if (!instance.KeyDictionary.ContainsKey(key)){
            return false;
        }
        if (instance.KeyDictionary[key].KeyPressed()){
            instance.KeyDictionary[key].ConsumePress();
            return true;
        }
        return false;
    }

    public static bool GetKeyHeld(KeyCode key, float threshold = 0.0f){
        if (!instance.KeyDictionary.ContainsKey(key)){
            return false;
        }
        return instance.KeyDictionary[key].KeyHeld(threshold);
    }

    private Vector2Int GetMouseCellPosition()
    {
        Vector3 Position = cameraObj.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(Position.x);
        int y = Mathf.RoundToInt(Position.y);

        return new Vector2Int(x,y);
    }
}
