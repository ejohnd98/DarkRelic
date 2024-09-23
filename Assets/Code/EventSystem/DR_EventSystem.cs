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

public class AttackTransactionEvent : DR_Event
{
    public AttackTransaction attackTransaction;
}

public class MoveEvent : DR_Event
{
    public Vector2Int startPos, endPos;
}

public class BloodChangeEvent : DR_Event
{
    public DR_Entity target;
    public int bloodDelta;
    public int oldBlood;
    public int newBlood;
}

public class HealthChangeEvent : DR_Event
{
    public DR_Entity target;
    public HealthComponent healthComp;
    public int delta = 0;
}

public class RoomChangeEvent : DR_Event
{
    public MapGenRoom room;
}

public class DoorEvent : DR_Event
{
    public DoorComponent door;
}

public class ActionEvent : DR_Event
{
    public DR_Action action;
}