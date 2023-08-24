using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogEntry{
    public GameObject obj;
    public string content;
    public float visibleTime;
    float counter;

    public LogEntry(string content, float time, GameObject obj){
        visibleTime = time;
        this.content = content;
        counter = 0.0f;
        this.obj = obj;
    }

    public bool IsExpired(){
        return counter >= visibleTime;
    }

    public bool UpdateTime(float deltaTime){
        counter += deltaTime;
        return IsExpired();
    }

    public float GetOpacity(){
        float temp = (counter / visibleTime);
        return 1.0f - Mathf.Clamp01(temp * temp);
    }
}

public class LogSystem : MonoBehaviour
{
    public static LogSystem instance;
    public GameObject LogObj;
    public Transform LogParent;
    public int maxVisibleLogs = 4;

    public List<LogEntry> LogObjs;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        LogObjs = new List<LogEntry>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < LogObjs.Count; i++)
        {
            LogObjs[i].UpdateTime(Time.deltaTime);
            if (LogObjs[i].IsExpired()){
                Destroy(LogObjs[i].obj);
                LogObjs.RemoveAt(i);
                i--;
            }else{
                Color col = LogObjs[i].obj.GetComponent<TextMeshProUGUI>().color;
                col.a = LogObjs[i].GetOpacity();
                LogObjs[i].obj.GetComponent<TextMeshProUGUI>().color = col;
            }
        }
    }

    public void AddLog(DR_Action action){
        if (!action.loggable){
            return;
        }

        if (LogObjs.Count == maxVisibleLogs){
            Destroy(LogObjs[0].obj);
            LogObjs.RemoveAt(0);
        }

        LogEntry log = new LogEntry(action.GetLogText(), 1.5f,  GameObject.Instantiate(LogObj, LogParent));
        log.obj.GetComponent<TextMeshProUGUI>().text = log.content;

        LogObjs.Add(log);
    }

    public void AddTextLog(string text){
        if (LogObjs.Count == maxVisibleLogs){
            Destroy(LogObjs[0].obj);
            LogObjs.RemoveAt(0);
        }

        LogEntry log = new LogEntry(text, 1.5f,  GameObject.Instantiate(LogObj, LogParent));
        log.obj.GetComponent<TextMeshProUGUI>().text = log.content;

        LogObjs.Add(log);
    }
}
