
using System.Diagnostics;

namespace Underworld
{
    public class an_arrow_trap : trap
    {
        public static void Activate(uwObject trapObj, int triggerX, int triggerY, uwObject[] objList)
        {
            var itemid = (int)trapObj.owner | ((int)trapObj.quality<<5);
            ObjectCreator.spawnObjectInTile(
                itemid: itemid, 
                tileX: triggerX, 
                tileY: triggerY, 
                xpos: (short)trapObj.tileX, 
                ypos: (short)trapObj.tileY, 
                zpos: trapObj.zpos, 
                WhichList: ObjectCreator.ObjectListType.StaticList);
            Debug.Print("TODO: spawn this arrow trap object as a projectile");
        }
    }//end class
        
}//end namespace
        //owner | (quality<<5)