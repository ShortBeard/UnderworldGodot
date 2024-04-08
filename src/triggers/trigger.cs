using System.Collections;
using System.Diagnostics;
using Peaky.Coroutines;

namespace Underworld
{
    public partial class trigger:UWClass
    {

        /// <summary>
        /// To prevent teleporting again when the teleport destination in inside a teleport trap
        /// </summary>
        public static bool JustTeleported;
        public static int TeleportLevel = -1;
        public static int TeleportTileX = -1;
        public static int TeleportTileY = -1;


        /// <summary>
        /// General trigger function for the execution of triggers generically (call this from the traps only when continuing a chain
        /// Specialised triggers like look and use triggers should be called directly by the interaction modes.
        /// IE this is use to continue chains, not start them so it should not call the start and end trigger chain events
        /// </summary>
        /// <param name="srcObject"></param>
        /// <param name="triggerIndex"></param>
        /// <param name="objList"></param>
        public static void Trigger(uwObject srcObject, int triggerIndex, uwObject[] objList)
        {
            if (triggerIndex!=0)
            {
                var Trig = objList[triggerIndex];
                if (Trig!=null)
                {
                    if (Trig.link!=0)
                    {         
                        trap.ActivateTrap(Trig, Trig.link, objList);
                    }
                }
            }
        }  

        /// <summary>
        /// Handles common start events at start of chain.
        /// </summary>
        public static void StartTriggerChainEvents()
        {
            TeleportLevel = -1;
            TeleportTileX = -1;
            TeleportTileY = -1;
        }

        /// <summary>
        /// Handles the end of chain events.
        /// </summary>
        public static void EndTriggerChainEvents()
        {
            //Handle level transitions now since it's possible for further traps to be called after the teleport trap
            if (TeleportLevel!=-1)
            {
                playerdat.dungeon_level = TeleportLevel;
                //switch level
                UWTileMap.LoadTileMap(
                        newLevelNo: playerdat.dungeon_level - 1,
                        datafolder: playerdat.currentfolder,
                        newGameSession: false);
            }
            if ((TeleportTileX != -1) && (TeleportTileY !=-1))
            {
                //move to new tile
                var targetTile = UWTileMap.current_tilemap.Tiles[TeleportTileX, TeleportTileY];
                playerdat.zpos = targetTile.floorHeight << 2;
                playerdat.xpos = 3; playerdat.ypos = 3;
                playerdat.tileX = TeleportTileX; playerdat.tileY = TeleportTileY;
                main.gamecam.Position = uwObject.GetCoordinate(
                    tileX: playerdat.tileX, 
                    tileY: playerdat.tileY, 
                    _xpos: playerdat.xpos, 
                    _ypos: playerdat.ypos, 
                    _zpos: playerdat.camerazpos);
            }

            if ((TeleportTileX!=-1) || (TeleportTileY!=-1) || (TeleportLevel!=-1))
            {
                    if (playerdat.ObjectInHand!=0)
                    {
                        Debug.Print("Player is holding a world object while changing levels. This scenario needs to fixed!");
                    }
                JustTeleported = true;
                    _ = Peaky.Coroutines.Coroutine.Run(
                    PauseTeleport(),
                    main.instance
                    );
            }
            TeleportLevel = -1;
            TeleportTileX = -1;
            TeleportTileY = -1;
        }

        /// <summary>
        /// Puts a block on sucessive level transitions due to teleport placing player in a new move trigger
        /// </summary>
        /// <returns></returns>
        static IEnumerator PauseTeleport()
        {
            JustTeleported = true;
            yield return new WaitForSeconds(1);
            JustTeleported = false;
            yield return 0;
        }

    }//end class
}//end namespace