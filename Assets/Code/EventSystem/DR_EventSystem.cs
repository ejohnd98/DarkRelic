using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DR_EventSystem
{
    public static Action<DR_Event> TestEvent;
}

public class DR_Event
{
    public DR_Entity owner;
}

public class TestEvent : DR_Event
{
    public string test = "Test String";
}

public class AttackEvent : DR_Event
{
    public DR_Entity target;
    public int damageDealt = 0;
}