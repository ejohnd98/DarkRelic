using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionEvent
{
    public DR_Action action;

    public ActionEvent(DR_Action action){
        this.action = action;
    }

}

public class ActionSystem
{
    public static bool HandleAction(DR_GameManager gm, ActionEvent actionEvent){

        return actionEvent.action.Perform(gm);

        /*
        Requirements:
            create HasRequiredInfo function on DR_Action that can be implemented to say whether all the required variables are filled
            each action could specify an "ActionInput" (create this class as well)
                Has a bool isFilled, alongside an enum specifying the type (entity, position)
                along with variables for those types
        Plan:
            Check if provided action HasRequiredInfo
                If not, set GameManager state + current ActionEvent (take this from existing GameManager code)
                If ready, then continue as actions were handled before.

        motivation is to move all action handling into this class. it's fine if DamageSystem is still used though for now.

        
        */
    }
}
