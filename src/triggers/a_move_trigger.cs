using Godot;
namespace Underworld
{
    public partial class trigger:UWClass
    {
        public static bool CreateMoveTrigger(uwObject obj, Node3D parent)
        {
            var h = ((float)commonObjDat.height(obj.item_id)/128f)  * 0.15f;
            h=h*10; //increase size a bit. not sure what height it should be, but a bigger box makes it easier to test.
            var r = ((float)commonObjDat.radius(obj.item_id)/8f) * 1.2f;
            var ar = new a_movetriggercollision();
            ar.uwObjectIndex = obj.index;
            var col = new CollisionShape3D();
            var box = new BoxShape3D();
            box.Set("extents", new Vector3(r,h,r));
            col.Shape = box;
            ar.AddChild(col);
            parent.AddChild(ar);
            ar.BodyEntered += ar.movetrigger_entered;

            //Now mark the tile type if linked to a teleport
            if (obj.link!=0)
            {
                var linked = UWTileMap.current_tilemap.LevelObjects[obj.link];
                if (linked.item_id == 385)
                {
                    if (linked.zpos!=0)
                    {
                        //linked to a teleport trap to another level
                        if (UWTileMap.ValidTile(obj.tileX, obj.tileY))
                        {                        
                            UWTileMap.current_tilemap.Tiles[obj.tileX,obj.tileY].isStair = true;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// The Move trigger. A collision box that the player bumps into
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objList"></param>
        /// <param name="WorldObject"></param>
        /// <returns></returns>
        // public static bool Move(uwObject srcObject, int triggerIndex, uwObject[] objList)
        // {
        //     var triggerObj = objList[triggerIndex];
        //     if (triggerObj == null)
        //     {
        //         Debug.Print($"Null trigger at {triggerIndex}");
        //         return false;
        //     }
        //     //a_use trigger	4	4
        //     //if ((triggerObj.majorclass == 6) && ((triggerObj.minorclass == 2) || (triggerObj.minorclass == 3)) && (triggerObj.classindex == 2))
        //     //{//use trigger class , 6-2-2 or 6-3-2
        //         //activate trap
        //         trigger.StartTriggerChainEvents();
        //         Debug.Print($"Activating trap {triggerObj.link}");
        //         trap.ActivateTrap(
        //             triggerObj: triggerObj,
        //             trapIndex: triggerObj.link,
        //             objList: objList);
        //         trigger.EndTriggerChainEvents();
        //         return true;
        //     //}
        //     //return false;
        // }


    }//end class    
}//end namespace
