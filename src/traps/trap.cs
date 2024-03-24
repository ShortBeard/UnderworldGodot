using System.Diagnostics;

namespace Underworld
{
    public class trap : UWClass
    {
        public static int ObjectThatStartedChain = 0;

        public static void ActivateTrap(uwObject triggerObj, int trapIndex, uwObject[] objList)
        {
            var trapObj = objList[trapIndex];
            if (trapObj == null)
            {
                Debug.Print($"Null trap at {trapIndex}");
                return;
            }
            else
            {
                Debug.Print($"Running trap {trapObj.a_name}");
            }
            var triggerNextIndex = trapObj.link; //default object to trigger next. This may change due to the results of a check_variable_trap
            bool implemented = false;
            if (trapObj.majorclass == 6)
            {
                switch (trapObj.minorclass)
                {
                    case 0: // class 6-0 traps
                        {
                            switch (trapObj.classindex)
                            {
                                case 0://damage traps
                                    {
                                        implemented = true;
                                        a_damage_trap.Activate(
                                                trapObj: trapObj,
                                                triggerObj: triggerObj,
                                                objList: objList);
                                        break;
                                    }
                                case 1: // a teleport trap
                                    {
                                        implemented = true;
                                        a_teleport_trap.Activate(
                                                trapObj: trapObj,                                              
                                                objList: objList);
                                        break;
                                    }
                                case 3:// Do and hack traps
                                    {
                                        implemented = true;
                                        hack_trap.ActivateHackTrap(
                                                trapObj: trapObj,
                                                triggerObj: triggerObj,
                                                objList: objList);
                                        break;
                                    }
                                case 4: // pit trap 6-0-4 in uw1, special effects in uw2
                                    {
                                        if (_RES!=GAME_UW2)
                                        {//uw1 pit trap
                                            implemented = true;//to continue the chain
                                        }
                                        break;
                                    }
                                case 5: //change terrain trap
                                    {
                                        implemented = true;
                                        a_change_terrain_trap.Activate(
                                            triggerObj: triggerObj, 
                                            trapObj: trapObj);
                                        break;
                                    }
                                case 7: //create object 6-0-7
                                    {
                                        implemented = true;
                                        a_create_object_trap.Activate(
                                            triggerObj: triggerObj, 
                                            trapObj: trapObj, 
                                            objList: objList);
                                        triggerNextIndex = 0;//always stop on create object trap
                                        break;
                                    }
                                case 8://door trap
                                    {
                                        implemented = true;
                                        a_door_trap.Activate(
                                            triggerObj: triggerObj, 
                                            trapObj: trapObj, 
                                            objList: objList);
                                        break;
                                    }
                                case 0xD://set variable trap
                                    {
                                        implemented = true;
                                        a_set_variable_trap.Activate(
                                            triggerObj: triggerObj, 
                                            trapObj: trapObj);
                                        break;
                                    }
                                case 0xE://check variable trap
                                    {
                                        implemented = true;
                                        triggerNextIndex = a_check_variable_trap.Activate(
                                            triggerObj: triggerObj, 
                                            trapObj: trapObj, 
                                            objList: objList);
                                        break;
                                    }
                                case 0xF://nulltrap/combination trap
                                    {   
                                        implemented = true;//null trap does nothing.
                                        break;
                                    }
                            }
                            break;
                        }
                    case 1://class 6-1
                        {
                            switch (trapObj.classindex)
                            {
                                case 0: //6-1-0 Text String Trap
                                    {
                                        implemented = true;
                                        a_text_string_trap.activate(
                                            trapObj: trapObj, 
                                            objList: objList);
                                        break;
                                    }

                            }
                            break;
                        }
                }
            }

            if (!implemented)
            {
                Debug.Print($"Unknown/unimplemented Trap Class {trapObj.majorclass} {trapObj.minorclass} {trapObj.classindex} {trapObj.a_name} i:{trapObj.index}");
            }
            else
            {
                if (triggerNextIndex!=0)
                {
                    var triggerNextObject = objList[triggerNextIndex];
                    if (triggerNextObject!=null)
                    {
                        if (triggerNextObject.majorclass == 6)//only trigger next a trap/trigger.
                        {
                            switch (triggerNextObject.minorclass)
                            {
                                case 0:
                                case 1://traps
                                    ActivateTrap(
                                        triggerObj: triggerObj, 
                                        trapIndex: triggerNextIndex, 
                                        objList: objList); //am i right re-using the original trigger?
                                    break;
                                case 2:
                                case 3://triggers
                                    TriggerNext(
                                        trapObj: trapObj, 
                                        objList: objList, 
                                        triggerNextIndex: triggerNextIndex);
                                    break;

                            }                            
                        }
                    }                    
                }                
            }

            if (triggerObj.flags1 == 0)
            {
                //remove trigger chain
                Debug.Print("TEST ME, THIS TRIGGER SHOULD ONLY FIRE ONCE and clear the trigger chain");
            }
        } //end activate trap


        public static void TriggerNext(uwObject trapObj, uwObject[] objList, int triggerNextIndex)
        {
            //Continue the trigger-trap chain if possible.
            if ((trapObj.link != 0) && (trapObj.is_quant == 0) && (triggerNextIndex !=0 ))
            {
                trigger.Trigger(
                    srcObject: trapObj,
                    triggerIndex: triggerNextIndex,
                    objList: objList);
            }
        }

    }//end class
}//end namespace