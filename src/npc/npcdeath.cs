using System.Diagnostics;
using System.Linq;
namespace Underworld
{
    /// <summary>
    /// Class for managing the ai death special cases
    /// </summary>
    public partial class npc : objectInstance
    {

        /// <summary>
        /// Handles special death cases for npcs.
        /// </summary>
        /// <param name="critter"></param>
        /// <param name="mode">0 when initial killing blow, 1 when at end of death animation</param>
        /// <returns>true if NPC should die, otherwise false to stay alive</returns>
        public static bool SpecialDeathCases(uwObject critter, int mode = 0)
        {
            switch(_RES)
            {
                case GAME_UW2:
                    return SpecialDeathCasesUW2(critter, mode);
                default:
                    return SpecialDeathCasesUW1(critter, mode);
            }
        }

        public static bool SpecialDeathCasesUW1(uwObject critter, int mode = 0)
        {
            switch(critter.npc_whoami)
            {
                case 0xB://Thorlson (cut content npc)
                    {
                        if (mode==0)
                        {
                            talk.Talk(critter.index, UWTileMap.current_tilemap.LevelObjects, true);
                            critter.npc_hp= 0x3C;//thorlson is unstoppable, once enraged nothing can kill him!
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case 0x16://golem
                    {
                        if (mode==0)
                        {
                            if (critter.UnkBit_0XA_Bit456==0)
                            {
                                critter.UnkBit_0XA_Bit456 = 1;
                                playerdat.ChangeExperience(500);
                            }
                            combat.EndCombatLoop();
                            critter.ProjectileSourceID=0; //forget that the player has hit them.
                            critter.npc_animation=32;//?
                            critter.AnimationFrame=0;
                            talk.Talk(critter.index, UWTileMap.current_tilemap.LevelObjects, true);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case 0x18://prisoner (murgo)
                    {
                        if (mode==1)
                        {
                            playerdat.SetQuest(6,1);
                        }
                        return true;
                    }
                case 0x1B://garamon
                    {
                        if (mode==1)
                        {
                            var tmp = playerdat.GetQuest(37);
                            tmp &=0xFB;
                            playerdat.SetQuest(37,tmp);//can talismans be thrown == false
                        }
                        return true;
                    }
                case 0x6E://gazer
                    {
                        if (mode==1)
                        {
                            playerdat.SetQuest(4, 1);
                        }
                        return true;
                    }

                case 0x8E://Rodrick
                    {
                        if (mode==1)
                        {
                            playerdat.SetQuest(11, 1);
                        }
                        return true;
                    }
                case 0xE7://tybal
                    {
                        if (mode==1)
                        {
                            TybalDeath(critter);
                        }
                        return true;
                    }


                case 0://generic npcs
                default:
                    return true;//to die
            }
        }


        
        public static bool SpecialDeathCasesUW2(uwObject critter, int mode = 0)
        {
            return true;//todo
        }



        /// <summary>
        /// Handles tybals death.
        /// </summary>
        /// <param name="tybal"></param>
        public static void TybalDeath(uwObject tybal)
        {
            Debug.Print("Play cutscene 2");
            //set quest variables
            var tmp = playerdat.GetQuest(37);
            tmp|=4;
            playerdat.SetQuest(37, tmp);
            //remove a move trigger from the prison
            var tile = UWTileMap.current_tilemap.Tiles[0x17,0x38];//trap in the nw prison area
            var next = tile.indexObjectList;
            var previous = 0;
            while (next!=0)
            {
                var nextObj = UWTileMap.current_tilemap.LevelObjects[next];                
                if (nextObj.item_id==0x1A0)
                {
                    next = nextObj.next;
                    if (previous==0)
                    {//head of tile
                        tile.indexObjectList = nextObj.next;
                        nextObj.next=0;                        
                    }
                    else
                    {
                        var previousObject = UWTileMap.current_tilemap.LevelObjects[previous];
                        previousObject.next = nextObj.next;                        
                        nextObj.next = 0;
                    }   
                    ObjectCreator.RemoveObject(nextObj);   
                }
                else
                {//not a match. try next
                    previous = nextObj.index;
                    next = nextObj.next;
                }               
            }

            int[] WhotoRemove = new int[]{0xDE,0xD1,0xD8,0xD2,0xDC,0xD5,0xD8,0xD4,0xD3,0xDD};

            for(int i = 1; i<=255;i++)
            {
                var n = UWTileMap.current_tilemap.LevelObjects[i];
                if (n.majorclass==1)
                {                    
                    if (UWTileMap.ValidTile(n.tileX, n.tileY))
                    {
                        if (WhotoRemove.Contains(n.npc_whoami))
                        {
                            ObjectCreator.DeleteObjectFromTile(n.tileX,n.tileY, n.index, true);
                        }
                    }
                }
            }
        }
    }//end class
}//end namespace