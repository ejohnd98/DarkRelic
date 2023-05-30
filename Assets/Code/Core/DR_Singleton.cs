using UnityEngine;

public abstract class DR_Singleton<T> where T : class
{
    public static DR_Singleton<T> instance;

    public DR_Singleton(){
        if (instance != null && instance != this){
            Debug.LogError("The following singleton already exists!: " + typeof(DR_Singleton<T>).Name);
        }else{
            instance = this;
        }
    }
}
