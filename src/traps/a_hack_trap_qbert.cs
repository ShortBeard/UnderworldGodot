using System;
using System.Diagnostics;
namespace Underworld
{
    /// <summary>
    /// Trap which controls the behaviour of qbert pyramid and the moongates in the ethereal void in UW2
    /// </summary>
    public class a_hack_trap_qbert : hack_trap
    {
        public static void Activate(uwObject trapObj, uwObject[] objList)
        {
            HackTrapQbert(trapObj.owner);
        }

        static void HackTrapQbert(int owner)
        {
            InitialiseQbert();
            switch (owner)
            {
                case < 10: // when <10 starting a pyramid color or going to a pyramid
                    StartPyramid(owner);
                    break;
                case >= 10 and < 20:
                    //Teleport to a location
                    TeleportToLocation(owner: owner);
                    break;
                case >= 20 and < 63:
                    //spawns a hacktrap
                    {
                        UseColouredRoomOrb(owner);
                    }
                    break;
                case 63://stepping on tile
                    StepOnPyramidTile();
                    break;
            }
        }

        /// <summary>
        /// When using the orb in a colour room
        /// </summary>
        /// <param name="owner"></param>
        private static void UseColouredRoomOrb(int owner)
        {
            owner -= 30; //? 
            var si = 0;
            while (si < 7)
            {
                if (playerdat.GetGameVariable(100 + si) == owner)
                {
                    //do hacktrap
                    var ax = 6 - si;
                    var dx = 7 - si;
                    ax = ax * dx;
                    ax = ax / 2;
                    var Y = 0x1D - ax;
                    //TODO spawn a camera hack trap in player object tile
                    Debug.Print($"Point a camera at {0x20},{Y}");
                    return;
                }
                si++;
            }
        }

        private static void TeleportToTopOfPyramid()
        {
            DoTeleport(
                teleportX: 49,
                teleportY: 51,
                newLevel: 69,
                heading: 61);
        }



        /// <summary>
        /// Sets game variables at first run of trap execution
        /// </summary>
        private static void InitialiseQbert()
        {
            if (playerdat.GetGameVariable(100) != 6)
            {
                playerdat.SetGameVariable(100, 6);
                for (int i = 1; i < 7; i++)
                {
                    playerdat.SetGameVariable(100 + i, 0xFF);
                }
            }
        }

        private static void StepOnPyramidTile()
        {

        }

        private static void StartPyramid(int owner)
        {
            var si = 0;
            while (si <= 4)
            {
                var gamevarvalue = playerdat.GetGameVariable(100 + si);
                if (gamevarvalue == 0xFF)
                {//freeslot
                    playerdat.SetGameVariable(
                        variableno: 100 + si,
                        value: owner);
                    if (si == 4)
                    {//final colour
                        playerdat.SetGameVariable(
                        variableno: 100 + si + 1,
                        value: 5);
                    }

                    TeleportToTopOfPyramid();
                    return;
                }
                else
                {
                    if (gamevarvalue == owner)
                    {
                        TeleportToTopOfPyramid();
                        return;
                    }
                    si++;
                }
            }
        }

        /// <summary>
        /// Based on the trap owner uses random hidden tiles on the map to determine where the moongate leads to.
        /// </summary>
        /// <param name="owner"></param>
        private static void TeleportToLocation(int owner)
        {
            var counter = 0;
            var tile_a = UWTileMap.current_tilemap.Tiles[owner + 46, 1];
            var newLevel = 69;
            var randomrange = (int)tile_a.wallTexture;

            //ovr110_4480:
            while (counter <= 4)
            {
                var tmpRngRange = (randomrange * 3) >> 1;
                var RngResult_var14 = Rng.r.Next(tmpRngRange);
                if (RngResult_var14 >= randomrange)
                {
                    RngResult_var14 = 1;
                }
                //Debug.Print($"Rng for qbert is {RngResult_var14}");
                var tile_b = UWTileMap.current_tilemap.Tiles[owner + 46, RngResult_var14 + 32];
                var tile_c = UWTileMap.current_tilemap.Tiles[owner + 46, RngResult_var14 + 2];
                var teleportX = tile_b.wallTexture;
                var teleportY = tile_c.wallTexture;

                var heading = ((tile_c.floorTexture + 8) << 2) + ((tile_b.floorTexture & 0x8) >> 3);
                //if ((tile_b.floorTexture & 0x7) != 0)
                //{
                newLevel -= (tile_b.floorTexture & 0x7);
                //}
                counter++;
                if (counter >= 4)
                {//after 4 to find a faraway teleport destination give up and just teleport to the found locaiton.
                    DoTeleport(
                        teleportX: teleportX,
                        teleportY: teleportY,
                        newLevel: newLevel,
                        heading: heading);
                    return;
                }
                else
                {
                    if ((Math.Abs(teleportX - playerdat.tileX) >= 3) || (Math.Abs(teleportY - playerdat.tileY) >= 3))
                    {//only teleport if destination is away from the current location.
                        DoTeleport(
                            teleportX: teleportX,
                            teleportY: teleportY,
                            newLevel: newLevel,
                            heading: heading);
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Performs the teleport for qbert.
        /// </summary>
        /// <param name="teleportX"></param>
        /// <param name="teleportY"></param>
        /// <param name="newLevel"></param>
        /// <param name="heading"></param>
        static void DoTeleport(int teleportX, int teleportY, int newLevel, int heading)
        {
            if (main.JustTeleported)
            {
                main.JustTeleported = false;
                return;
            }
            if (newLevel != playerdat.dungeon_level)
            {
                main.TeleportLevel = newLevel;
            }
            else
            {
                main.TeleportLevel = -1;
            }
            Debug.Print($"{teleportX},{teleportY},{newLevel}");
            main.TeleportTileX = teleportX;
            main.TeleportTileY = teleportY;
            //TODO: include heading after teleport
        }
    } //end class
}//end namespace