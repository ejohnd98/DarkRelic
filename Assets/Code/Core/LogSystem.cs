using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogEntry{
    public GameObject obj;
    public string content;
    public float visibleTime;
    public float alpha = 1.0f;

    public LogEntry(string content, float time, GameObject obj){
        visibleTime = time;
        this.content = content;
        this.obj = obj;
    }

    public void UpdateAlpha(){
        Color col = obj.GetComponent<TextMeshProUGUI>().color;
        col.a = alpha;
        obj.GetComponent<TextMeshProUGUI>().color = col;
    }
}

public class LogSystem : MonoBehaviour
{
    public static LogSystem instance;
    public GameObject LogObj;
    public Transform LogParent;
    public int maxVisibleLogs = 50;

    public List<LogEntry> LogObjs;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        LogObjs = new List<LogEntry>();
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
        StartCoroutine(FadeLogText(log));
    }

    public void AddDamageLog(DamageEvent damageEvent){

        if (LogObjs.Count == maxVisibleLogs){
            Destroy(LogObjs[0].obj);
            LogObjs.RemoveAt(0);
        }

        LogEntry log = new LogEntry(damageEvent.GetLogText(), 1.5f,  GameObject.Instantiate(LogObj, LogParent));
        log.obj.GetComponent<TextMeshProUGUI>().text = log.content;

        LogObjs.Add(log);
        StartCoroutine(FadeLogText(log));
    }

    public void AddTextLog(string text){
        if (LogObjs.Count == maxVisibleLogs){
            Destroy(LogObjs[0].obj);
            LogObjs.RemoveAt(0);
        }

        LogEntry log = new LogEntry(text, 1.5f,  GameObject.Instantiate(LogObj, LogParent));
        log.obj.GetComponent<TextMeshProUGUI>().text = log.content;

        LogObjs.Add(log);
        StartCoroutine(FadeLogText(log));
    }

    public IEnumerator FadeLogText(LogEntry logEntry){
        yield return new WaitForSecondsRealtime(logEntry.visibleTime);

        while (logEntry.alpha > 0.5f)
        {
            logEntry.alpha -= 0.5f * Time.deltaTime;
            logEntry.UpdateAlpha();
            yield return null;
        }
    }
}
