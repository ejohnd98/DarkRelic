using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: move this to the "new" input system
public class DR_InputHandler : MonoBehaviour
{
    class InputState {
        public KeyCode key;
        public float persistCounter = 0.0f;
        public bool held = false;

        public InputState(KeyCode k){
            key = k;
        }

        public bool KeyPressed(){
            return persistCounter > 0.0f;
        }

        public bool KeyHeld(){
            return held;
        }

        public void ConsumePress(){
            persistCounter = 0.0f;
        }
    }

    public static DR_InputHandler instance;

    public float inputPersistLength = 0.2f;

    KeyCode[] KeysToCheck = {
        KeyCode.UpArrow,
        KeyCode.RightArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.Space
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
            }else{
                InputStates[i].held = false;
            }
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

    public static bool GetKeyHeld(KeyCode key){
        if (!instance.KeyDictionary.ContainsKey(key)){
            return false;
        }
        return instance.KeyDictionary[key].KeyHeld();
    }
}
