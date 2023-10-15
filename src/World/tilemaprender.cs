using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Underworld
{

    public class tileMapRender : UWClass
    {

        //public static Shader textureshader;

        

        //static tileMapRender()
        //{
            //MaterialMasterList = new();
        //}

        const int TILE_SOLID = 0;
        const int TILE_OPEN = 1;

        //Note the order of these 4 tiles are actually different in SHOCK. I swap them around in BuildTileMapShock for consistancy

        const int TILE_DIAG_SE = 2;
        const int TILE_DIAG_SW = 3;
        const int TILE_DIAG_NE = 4;
        const int TILE_DIAG_NW = 5;

        const int TILE_SLOPE_N = 6;
        const int TILE_SLOPE_S = 7;
        const int TILE_SLOPE_E = 8;
        const int TILE_SLOPE_W = 9;
        // const int TILE_VALLEY_NW = 10;
        // const int TILE_VALLEY_NE = 11;
        // const int TILE_VALLEY_SE = 12;
        // const int TILE_VALLEY_SW = 13;
        // const int TILE_RIDGE_SE = 14;
        // const int TILE_RIDGE_SW = 15;
        // const int TILE_RIDGE_NW = 16;
        // const int TILE_RIDGE_NE = 17;

        const int SLOPE_BOTH_PARALLEL = 0;
        const int SLOPE_BOTH_OPPOSITE = 1;
        const int SLOPE_FLOOR_ONLY = 2;
        const int SLOPE_CEILING_ONLY = 3;

        //Visible faces indices
        const int vTOP = 0;
        const int vEAST = 1;
        const int vBOTTOM = 2;
        const int vWEST = 3;
        const int vNORTH = 4;
        const int vSOUTH = 5;


        //BrushFaces
        const int fSELF = 128;
        const int fCEIL = 64;
        const int fNORTH = 32;
        const int fSOUTH = 16;
        const int fEAST = 8;
        const int fWEST = 4;
        const int fTOP = 2;
        const int fBOTTOM = 1;

        //Door headings
        public const int heading4 = 180;
        public const int heading0 = 0;
        public const int Heading6 = 270;
        public const int heading2 = 90;

        public static bool EnableCollision = true;

        //static int UW_CEILING_HEIGHT;
        static int CEILING_HEIGHT;

        const int CEIL_ADJ = 0;
        const int FLOOR_ADJ = 0; //-2;

        const float doorwidth = 0.8f;
        const float doorframewidth = 1.2f;
        const float doorSideWidth = (doorframewidth - doorwidth) / 2f;
        const float doorheight = 7f * 0.15f;

        public static TextureLoader mapTextures;

        static tileMapRender()
        {
            if (mapTextures==null)
            {
                mapTextures = new();
            }
        }

        public static void GenerateLevelFromTileMap(Node3D parent, Node3D sceneryParent, string game, TileMap Level, List<uwObject> objList, bool UpdateOnly)
        {
            bool skipCeil = true;
            CEILING_HEIGHT = Level.CEILING_HEIGHT;
            // if (game == GAME_SHOCK)
            // {
            //     skipCeil = false;
            // }
            
            if (!UpdateOnly)
            {
                //Clear out the children in the transform
                foreach (var child in parent.GetChildren())
                {
                    child.QueueFree();
                    //Object.Destroy(child.gameObject);
                }
                // foreach (var child in sceneryParent.GetChildren())
                // {
                //     child.QueueFree();
                //     Object.Destroy(child.gameObject);
                // }
            }

            for (int y = 0; y <= TileMap.TileMapSizeY; y++)
            {
                for (int x = 0; x <= TileMap.TileMapSizeX; x++)
                {
                    if (
                            (
                            (UpdateOnly) && (Level.Tiles[x, y].NeedsReRender)
                            )
                            ||
                            (
                                    !UpdateOnly
                            )
                    )
                    {
                        RenderTile(parent, x, y, Level.Tiles[x, y], false, false, false, skipCeil);

                        //if (game != GAME_SHOCK)
                        // {//Water
                        RenderTile(parent, x, y, Level.Tiles[x, y], true, false, false, skipCeil);
                        Level.Tiles[x, y].NeedsReRender = false;
                        // }
                    }
                    Level.Tiles[x, y].NeedsReRender = false;
                }
            }

            //Do a ceiling

            // if (game != GAME_SHOCK)
            //{
            if (!UpdateOnly)
            {
                var output = RenderCeiling(parent, 0, 0, CEILING_HEIGHT, CEILING_HEIGHT + CEIL_ADJ, Level.UWCeilingTexture, "CEILING", Level);
            }
            //}
            if (!UpdateOnly)
            {
                //Render bridges, pillars and door ways
                // switch (_RES)
                // {
                //     case GAME_SHOCK:
                //         break;
                //     default:
                //          RenderPillars(sceneryParent, Level, objList);
                //         RenderDoorways(sceneryParent, Level, objList);
                //         break;
                // }

            }
            // if ((UWEBase.EditorMode) && (UpdateOnly))
            // {
            //     UWHUD.instance.editor.RefreshTileMap();
            // }
        }



        /// <summary>
        /// Renders the a tile
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        /// <param name="skipFloor">If set to <c>true</c> skip floor.</param>
        /// <param name="skipCeil">If set to <c>true</c> skip ceil.</param>
        public static Node3D RenderTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert, bool skipFloor, bool skipCeil)
        {           
            //Picks the tile to render based on tile type/flags.
            switch (t.tileType)
            {
                case TILE_SOLID:    //0
                    {   //solid                       
                        return RenderSolidTile(parent, x, y, t, Water);
                    }
                case TILE_OPEN:     //1
                    {//open
                        if (skipFloor != true) { return RenderOpenTile(parent, x, y, t, Water, false); }    //floor
                        if ((skipCeil != true)) { return RenderOpenTile(parent, x, y, t, Water, true); }    //ceiling	
                        break;
                    }
                case TILE_DIAG_SE:
                    {//diag se
                        if (skipFloor != true) { RenderDiagSETile(parent, x, y, t, Water, false); }//floor
                        if ((skipCeil != true)) { RenderDiagSETile(parent, x, y, t, Water, true); }
                        return null;
                    }

                case TILE_DIAG_SW:
                    {   //diag sw
                        if (skipFloor != true) { RenderDiagSWTile(parent, x, y, t, Water, false); }//floor
                        if ((skipCeil != true)) { RenderDiagSWTile(parent, x, y, t, Water, true); }
                        return null;
                    }

                case TILE_DIAG_NE:
                    {   //diag ne
                        if (skipFloor != true) { RenderDiagNETile(parent, x, y, t, Water, invert); }//floor
                        if ((skipCeil != true)) { RenderDiagNETile(parent, x, y, t, Water, true); }
                        return null;
                    }

                case TILE_DIAG_NW:
                    {//diag nw
                        if (skipFloor != true) { RenderDiagNWTile(parent, x, y, t, Water, invert); }//floor
                        if ((skipCeil != true)) { RenderDiagNWTile(parent, x, y, t, Water, true); }
                        return null;
                    }

                case TILE_SLOPE_N:  //6
                    {//slope n
                        switch (t.shockSlopeFlag)
                        {
                            case SLOPE_BOTH_PARALLEL:
                                {
                                    if (skipFloor != true) { RenderSlopeNTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderSlopeNTile(parent, x, y, t, Water, true); }
                                    break;
                                }
                            case SLOPE_BOTH_OPPOSITE:
                                {
                                    if (skipFloor != true) { RenderSlopeNTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderSlopeSTile(parent, x, y, t, Water, true); }
                                    break;
                                }
                            case SLOPE_FLOOR_ONLY:
                                {
                                    if (skipFloor != true) { RenderSlopeNTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                                    break;
                                }
                            case SLOPE_CEILING_ONLY:
                                {
                                    if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                                    RenderSlopeNTile(parent, x, y, t, Water, true);
                                    break;
                                }
                        }
                        return null;
                    }
                case TILE_SLOPE_S: //slope s	7
                    {
                        switch (t.shockSlopeFlag)
                        {
                            case SLOPE_BOTH_PARALLEL:
                                {
                                    if (skipFloor != true) { RenderSlopeSTile(parent, x, y, t, Water, false); } //floor
                                    RenderSlopeSTile(parent, x, y, t, Water, true);
                                    break;
                                }
                            case SLOPE_BOTH_OPPOSITE:
                                {
                                    if (skipFloor != true) { RenderSlopeSTile(parent, x, y, t, Water, false); } //floor
                                    RenderSlopeNTile(parent, x, y, t, Water, true);
                                    break;
                                }
                            case SLOPE_FLOOR_ONLY:
                                {
                                    if (skipFloor != true) { RenderSlopeSTile(parent, x, y, t, Water, false); } //floor
                                    if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                                    break;
                                }
                            case SLOPE_CEILING_ONLY:
                                {
                                    if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                                    if ((skipCeil != true)) { RenderSlopeSTile(parent, x, y, t, Water, true); }
                                    break;
                                }
                        }
                        return null;
                    }
                case TILE_SLOPE_E:      //slope e 8	
                    {
                        switch (t.shockSlopeFlag)
                        {
                            case SLOPE_BOTH_PARALLEL:
                                {
                                    if (skipFloor != true) { RenderSlopeETile(parent, x, y, t, Water, false); }//floor
                                    RenderSlopeETile(parent, x, y, t, Water, true);
                                    break;
                                }
                            case SLOPE_BOTH_OPPOSITE:
                                {
                                    if (skipFloor != true) { RenderSlopeETile(parent, x, y, t, Water, false); }//floor
                                    RenderSlopeWTile(parent, x, y, t, Water, true);
                                    break;
                                }
                            case SLOPE_FLOOR_ONLY:
                                {
                                    if (skipFloor != true) { RenderSlopeETile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                                    break;
                                }
                            case SLOPE_CEILING_ONLY:
                                {
                                    if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                                    if ((skipCeil != true)) { RenderSlopeETile(parent, x, y, t, Water, true); }
                                    break;
                                }
                        }
                        return null;
                    }
                case TILE_SLOPE_W:  //9
                    { //slope w
                        switch (t.shockSlopeFlag)
                        {
                            case SLOPE_BOTH_PARALLEL:
                                {
                                    if (skipFloor != true) { RenderSlopeWTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderSlopeWTile(parent, x, y, t, Water, true); }
                                    break;
                                }
                            case SLOPE_BOTH_OPPOSITE:
                                {
                                    if (skipFloor != true) { RenderSlopeWTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderSlopeETile(parent, x, y, t, Water, true); }
                                    break;
                                }
                            case SLOPE_FLOOR_ONLY:
                                {
                                    if (skipFloor != true) { RenderSlopeWTile(parent, x, y, t, Water, false); }//floor
                                    if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                                    break;
                                }
                            case SLOPE_CEILING_ONLY:
                                {
                                    if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                                    if ((skipCeil != true)) { RenderSlopeWTile(parent, x, y, t, Water, true); }
                                    break;
                                }
                        }
                        return null;
                    }
                    // case TILE_VALLEY_NW:
                    //     {   //valleyNw(a)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleySETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderRidgeNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_VALLEY_NE:
                    //     {   //valleyne(b)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleySWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleyNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_VALLEY_SE:
                    //     {   //valleyse(c)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeSETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleyNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderRidgeSETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_VALLEY_SW:
                    //     {   //valleysw(d)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeSWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleyNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderValleySWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderRidgeSWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_RIDGE_SE:
                    //     {   //ridge se(f)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleySETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleySETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_RIDGE_SW:
                    //     {   //ridgesw(g)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleySWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeSWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderValleySWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_RIDGE_NW:
                    //     {   //ridgenw(h)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleyNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeSETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNWTile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderValleyNWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
                    // case TILE_RIDGE_NE:
                    //     {   //ridgene(i)
                    //         switch (t.shockSlopeFlag)
                    //         {
                    //             case SLOPE_BOTH_PARALLEL:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderValleyNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_BOTH_OPPOSITE:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderRidgeSWTile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //             case SLOPE_FLOOR_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderRidgeNETile(parent, x, y, t, Water, false); }//floor
                    //                     if ((skipCeil != true)) { RenderOpenTile(parent, x, y, t, Water, true); }   //ceiling
                    //                     break;
                    //                 }
                    //             case SLOPE_CEILING_ONLY:
                    //                 {
                    //                     if (skipFloor != true) { RenderOpenTile(parent, x, y, t, Water, false); }   //floor
                    //                     if ((skipCeil != true)) { RenderValleyNETile(parent, x, y, t, Water, true); }
                    //                     break;
                    //                 }
                    //         }
                    //         return null;
                    //     }
            }
            return null;
        }


        static Node3D RenderSolidTile(Node3D parent, int x, int y, TileInfo t, bool Water)
        {
            if (t.Render == true)
            {
                if (t.isWater == Water)
                {
                    string TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                    t.VisibleFaces[vTOP] = false;
                    t.VisibleFaces[vBOTTOM] = false;
                    return RenderCuboid(parent, x, y, t, Water, FLOOR_ADJ, CEILING_HEIGHT + CEIL_ADJ, TileName);
                }
            }
            return null;
        }


        /// <summary>
        /// Renders an open tile with no slopes
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static Node3D RenderOpenTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                if (t.isWater == Water)
                {
                    string TileName;
                    if (invert == false)
                    {
                        //Bottom face 
                        if (t.TerrainChange)
                        {
                            TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                            return RenderCuboid(parent, x, y, t, Water, -16, t.floorHeight, TileName);
                        }
                        else
                        {
                            TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                            return RenderCuboid(parent, x, y, t, Water, 0, t.floorHeight, TileName);
                        }
                    }
                    else
                    {
                        //Ceiling version of the tile
                        bool visB = t.VisibleFaces[vBOTTOM];
                        bool visT = t.VisibleFaces[vTOP];
                        t.VisibleFaces[vBOTTOM] = true;
                        t.VisibleFaces[vTOP] = false;
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        var output = RenderCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TileName);
                        t.VisibleFaces[vBOTTOM] = visB;
                        t.VisibleFaces[vTOP] = visT;
                        return output;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Renders the a cuboid with no slopes
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderCeiling(Node3D parent, int x, int y, int Bottom, int Top, int CeilingTexture, string TileName, Underworld.TileMap map)
        {
            //return null;

            //Draw a cube with no slopes.
            int NumberOfVisibleFaces = 1;

            //Allocate enough verticea and UVs for the faces
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);

            //Now create the mesh
            // Node3DTile = new GameObject(TileName)
            // {
            //     layer = LayerMask.NameToLayer("MapMesh")
            // };
            // Tile.transform.parent = parent.transform;
            // Tile.transform.position = new Vector3(x * 1.2f, 0.0f, y * 1.2f);

            // Tile.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // MeshFilter mf = Tile.AddComponent<MeshFilter>();
            // MeshRenderer mr = Tile.AddComponent<MeshRenderer>();

            // Mesh mesh = new Mesh
            // {
            //     subMeshCount = NumberOfVisibleFaces//Should be no of visible faces
            // };

            var a_mesh = new ArrayMesh();

            int[] MatsToUse = new int[NumberOfVisibleFaces];
            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            // float offset = 0f;

            //bottom wall vertices
            MatsToUse[FaceCounter] = map.texture_map[CeilingTexture];
            verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * 64);
            verts[1 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
            verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * 64, baseHeight, 0f);
            verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * 64, baseHeight, 1.2f * 64);
            //Change default UVs
            uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
            uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * 64);
            uvs[2 + (4 * FaceCounter)] = new Vector2(64, 1.0f * 64);
            uvs[3 + (4 * FaceCounter)] = new Vector2(64, 0.0f);

            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            //Apply the uvs and create my tris
            // mesh.vertices = verts;
            // mesh.uv = uvs;
            FaceCounter = 0;
            int[] indices = new int[6];
            indices[0] = 0 + (4 * FaceCounter);
            indices[1] = 1 + (4 * FaceCounter);
            indices[2] = 2 + (4 * FaceCounter);
            indices[3] = 0 + (4 * FaceCounter);
            indices[4] = 2 + (4 * FaceCounter);
            indices[5] = 3 + (4 * FaceCounter);
            //mesh.SetTriangles(indices, FaceCounter);
            AddSurfaceToMesh(verts, uvs, MatsToUse, 0, a_mesh, normals, indices);

            return CreateMeshInstance(parent, x, y, TileName, a_mesh);

            // mr.materials = MatsToUse;//mats;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // mf.mesh = mesh;
            // if (EnableCollision)
            // {
            //     MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //     mc.sharedMesh = null;
            //     mc.sharedMesh = mesh;
            // }
            // return Tile;
        }

        private static Node3D CreateMeshInstance(Node3D parent, int x, int y, string TileName, ArrayMesh a_mesh, bool EnableCollision = false)
        {
            var final_mesh = new MeshInstance3D();
            parent.AddChild(final_mesh);
            final_mesh.Position = new Vector3(x * -1.2f, 0.0f, y * 1.2f);
            final_mesh.Name = TileName;
            final_mesh.Mesh = a_mesh;
            if (EnableCollision)
            {
                final_mesh.CreateConvexCollision();
            }
            return final_mesh;
        }


        static void RenderDiagSETile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            //int BLeftX; int BLeftY; int BLeftZ; int TLeftX; int TLeftY; int TLeftZ; int TRightX; int TRightY; int TRightZ;

            if (t.Render == true)
            {
                if (invert == false)
                {

                    if (Water != true)
                    {
                        //the wall part
                        string TileName = "Wall_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderDiagSEPortion(parent, FLOOR_ADJ, CEILING_HEIGHT + CEIL_ADJ, t, TileName);
                    }
                    if (t.isWater == Water)
                    {
                        //it's floor
                        //RenderDiagNWPortion( FLOOR_ADJ, t.floorHeight, t,"DiagNW1");
                        bool PreviousNorth = t.VisibleFaces[vNORTH];
                        bool PreviousWest = t.VisibleFaces[vWEST];
                        t.VisibleFaces[vNORTH] = false;
                        t.VisibleFaces[vWEST] = false;
                        RenderDiagOpenTile(parent, x, y, t, Water, false);
                        t.VisibleFaces[vNORTH] = PreviousNorth;
                        t.VisibleFaces[vWEST] = PreviousWest;
                    }
                }
                else
                {//it's ceiling
                 //RenderDiagNWPortion( CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, t, "DiagNW2a");
                    bool vis = t.VisibleFaces[vBOTTOM];
                    t.VisibleFaces[vBOTTOM] = true;
                    RenderOpenTile(parent, x, y, t, Water, true);
                    t.VisibleFaces[vBOTTOM] = vis;
                }
            }
            return;
        }

        /// <summary>
        /// Renders the diag SW tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderDiagSWTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                if (invert == false)
                {
                    if (Water != true)
                    {
                        //Its wall
                        string TileName = "Wall_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderDiagSWPortion(parent, FLOOR_ADJ, CEILING_HEIGHT + CEIL_ADJ, t, TileName);
                    }
                    if (t.isWater == Water)
                    {
                        //it's floor
                        //RenderDiagNEPortion( FLOOR_ADJ, t.floorHeight, t,"TileNe1");
                        bool PreviousNorth = t.VisibleFaces[vNORTH];
                        bool PreviousEast = t.VisibleFaces[vEAST];
                        t.VisibleFaces[vNORTH] = false;
                        t.VisibleFaces[vEAST] = false;
                        //RenderOpenTile( parent , x, y, t, Water, false);
                        RenderDiagOpenTile(parent, x, y, t, Water, false);
                        t.VisibleFaces[vNORTH] = PreviousNorth;
                        t.VisibleFaces[vEAST] = PreviousEast;
                    }
                }
                else
                {
                    //its' ceiling.
                    //RenderDiagNEPortion( CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, t, "TileNe2");
                    bool vis = t.VisibleFaces[vBOTTOM];
                    t.VisibleFaces[vBOTTOM] = true;
                    RenderOpenTile(parent, x, y, t, Water, true);
                    t.VisibleFaces[vBOTTOM] = vis;
                }
            }
            return;
        }



        /// <summary>
        /// Renders the diag NE tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderDiagNETile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                if (invert == false)
                {
                    if (Water != true)
                    {
                        string TileName = "Wall_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderDiagNEPortion(parent, FLOOR_ADJ, CEILING_HEIGHT + CEIL_ADJ, t, TileName);
                    }
                    if (t.isWater == Water)
                    {
                        //it's floor
                        //RenderDiagSWPortion( FLOOR_ADJ, t.floorHeight, t, "DiagSW2");
                        bool PreviousSouth = t.VisibleFaces[vSOUTH];
                        bool PreviousWest = t.VisibleFaces[vWEST];
                        t.VisibleFaces[vSOUTH] = false;
                        t.VisibleFaces[vWEST] = false;
                        //RenderOpenTile( parent , x, y, t, Water, false);
                        RenderDiagOpenTile(parent, x, y, t, Water, false);
                        t.VisibleFaces[vSOUTH] = PreviousSouth;
                        t.VisibleFaces[vWEST] = PreviousWest;
                    }
                }
                else
                {//it's ceiling
                 //RenderDiagSWPortion( CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, t, "DiagSE3");
                    bool vis = t.VisibleFaces[vBOTTOM];
                    t.VisibleFaces[vBOTTOM] = true;
                    RenderOpenTile(parent, x, y, t, Water, true);
                    t.VisibleFaces[vBOTTOM] = vis;
                }
            }
            return;
        }


        /// <summary>
        /// Renders the diag NW tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderDiagNWTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                if (invert == false)
                {
                    if (Water != true)
                    {
                        //It's wall.
                        string TileName = "Wall_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderDiagNWPortion(parent, FLOOR_ADJ, CEILING_HEIGHT + CEIL_ADJ, t, TileName);
                    }

                    if (t.isWater == Water)
                    {//TODO:Update these floors to only show the top surface.
                     //it's floor
                     //RenderDiagSEPortion( FLOOR_ADJ, t.floorHeight, t, "DiagSE2");
                        bool PreviousSouth = t.VisibleFaces[vSOUTH];
                        bool PreviousEast = t.VisibleFaces[vEAST];
                        t.VisibleFaces[vSOUTH] = false;
                        t.VisibleFaces[vEAST] = false;
                        RenderDiagOpenTile(parent, x, y, t, Water, false);
                        t.VisibleFaces[vSOUTH] = PreviousSouth;
                        t.VisibleFaces[vEAST] = PreviousEast;
                    }
                }
                else
                {//it's ceiling
                 //RenderDiagSEPortion( CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, t, "DiagSE3");
                    bool vis = t.VisibleFaces[vBOTTOM];
                    t.VisibleFaces[vBOTTOM] = true;
                    RenderOpenTile(parent, x, y, t, Water, true);
                    t.VisibleFaces[vBOTTOM] = vis;
                }
            }
            return;
        }

        public static Node3D RenderCuboid(Node3D parent, int x, int y, TileInfo t, bool Water, int Bottom, int Top, string TileName)
        {
            //Draw a cube with no slopes.
            int NumberOfVisibleFaces = 0;
            //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    NumberOfVisibleFaces++;
                }
            }
            //Allocate enough verticea and UVs for the faces
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            float dimY = t.DimY;

            // SetTileLayer(t, a_tile);
            //a_tile.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //MeshFilter mf = a_tile.AddComponent<MeshFilter>();
            //MeshRenderer mr = a_tile.AddComponent<MeshRenderer>();

            //Mesh mesh = new Mesh
            //{
            //    subMeshCount = NumberOfVisibleFaces//Should be no of visible faces
            //};

            int[] MatsToUse = new int[NumberOfVisibleFaces];    //was material
            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);  //this was positive in unity. I had to flip it to get it to work in godot. Possibly a issue from unity that I never sp
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    float offset;
                    switch (i)
                    {
                        case vTOP:
                            {
                                //Set the verts	
                                MatsToUse[FaceCounter] = FloorTexture(fSELF, t);
                                // if (_RES == GAME_UW1)
                                //     if (GameWorldController.instance.dungeon_level == 6)
                                //     {
                                //         if (t.floorTexture == 4)
                                //         {//Special case for tybals floor
                                //          //MatsToUse[FaceCounter]= (Material)Resources.Load(_RES+ "\\Materials\\Textures\\uw1_224_maze");
                                //             MatsToUse[FaceCounter] = GameWorldController.instance.SpecialMaterials[0];
                                //         }
                                //     }
                                // if ((t.tileType == TILE_SOLID) && (UWEBase.EditorMode))
                                // {
                                //     MatsToUse[FaceCounter] = GameWorldController.instance.Jorge;
                                // }

                                verts[0 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight, 0.0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0.0f);

                                //Allocate UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 0.0f);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 1.0f * dimY);

                                break;
                            }

                        case vNORTH:
                            {
                                //north wall vertices
                                offset = CalcCeilOffset(fNORTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fNORTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);

                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);


                                break;
                            }

                        case vWEST:
                            {
                                //west wall vertices
                                offset = CalcCeilOffset(fWEST, t);
                                MatsToUse[FaceCounter] = WallTexture(fWEST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                break;
                            }

                        case vEAST:
                            {
                                //east wall vertices
                                offset = CalcCeilOffset(fEAST, t);
                                MatsToUse[FaceCounter] = WallTexture(fEAST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                break;
                            }

                        case vSOUTH:
                            {
                                offset = CalcCeilOffset(fSOUTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fSOUTH, t);
                                //south wall vertices
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);

                                break;
                            }
                        case vBOTTOM:
                            {
                                //bottom wall vertices
                                MatsToUse[FaceCounter] = FloorTexture(fCEIL, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                //Change default UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, 1.0f * dimY);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, 0.0f);
                                break;
                            }
                    }
                    FaceCounter++;
                }
            }

            //Generate normals
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            // mesh.vertices = verts;
            // mesh.uv = uvs;

            //Create the overall mesh
            var a_mesh = new ArrayMesh(); //= Mesh as ArrayMesh;

            //add indiviual surfaces to the mesh.

            FaceCounter = 0;
            int[] indices = new int[6];
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    indices[0] = 0 + (4 * FaceCounter);
                    indices[1] = 1 + (4 * FaceCounter);
                    indices[2] = 2 + (4 * FaceCounter);
                    indices[3] = 0 + (4 * FaceCounter);
                    indices[4] = 2 + (4 * FaceCounter);
                    indices[5] = 3 + (4 * FaceCounter);
                    //mesh.SetTriangles(indices, FaceCounter);

                    //Create the surface.
                    AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                    FaceCounter++;
                }
            }


            return CreateMeshInstance(parent, x, y, TileName, a_mesh);
            //GetTree().Root.CallDeferred("add_child", final_mesh);



            // mr.materials = MatsToUse;//mats;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // // mf.mesh = mesh;
            // if (EnableCollision)
            // {
            //     // MeshCollider mc = a_tile.AddComponent<MeshCollider>();
            //     // mc.sharedMesh = null;
            //     // mc.sharedMesh = mesh;
            // }
            // return final_mesh;
        }

        /// <summary>
        /// Renders the slope N tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderSlopeNTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                string TileName;
                if (invert == false)
                {
                    if (t.isWater == Water)
                    {
                        //A floor
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderSlopedCuboid(parent, x, y, t, Water, FLOOR_ADJ, t.floorHeight, TILE_SLOPE_N, t.TileSlopeSteepness, 1, TileName);
                    }
                }
                else
                {
                    //It's invert
                    TileName = "N_Ceiling_" + x.ToString("D2") + "_" + y.ToString("D2");
                    bool visB = t.VisibleFaces[vBOTTOM];
                    bool visT = t.VisibleFaces[vTOP];
                    t.VisibleFaces[vBOTTOM] = true;
                    t.VisibleFaces[vTOP] = false;
                    RenderSlopedCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TILE_SLOPE_N, t.TileSlopeSteepness, 0, TileName);
                    t.VisibleFaces[vBOTTOM] = visB;
                    t.VisibleFaces[vTOP] = visT;
                }
            }
            return;
        }

        /// <summary>
        /// Renders the slope S tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderSlopeSTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                string TileName;
                if (invert == false)
                {
                    if (t.isWater == Water)
                    {
                        //A floor
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderSlopedCuboid(parent, x, y, t, Water, FLOOR_ADJ, t.floorHeight, TILE_SLOPE_S, t.TileSlopeSteepness, 1, TileName);
                    }
                }
                else
                {
                    //It's invert
                    TileName = "S_Ceiling_" + x.ToString("D2") + "_" + y.ToString("D2");
                    bool visB = t.VisibleFaces[vBOTTOM];
                    bool visT = t.VisibleFaces[vTOP];
                    t.VisibleFaces[vBOTTOM] = true;
                    t.VisibleFaces[vTOP] = false;
                    RenderSlopedCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TILE_SLOPE_S, t.TileSlopeSteepness, 0, TileName);
                    t.VisibleFaces[vBOTTOM] = visB;
                    t.VisibleFaces[vTOP] = visT;
                }
            }
            return;
        }

        /// <summary>
        /// Renders the slope W tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderSlopeWTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                string TileName;
                if (invert == false)
                {
                    if (t.isWater == Water)
                    {
                        //A floor
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderSlopedCuboid(parent, x, y, t, Water, FLOOR_ADJ, t.floorHeight, TILE_SLOPE_W, t.TileSlopeSteepness, 1, TileName);
                    }
                }
                else
                {
                    //It's invert
                    TileName = "W_Ceiling_" + x.ToString("D2") + "_" + y.ToString("D2");
                    bool visB = t.VisibleFaces[vBOTTOM];
                    bool visT = t.VisibleFaces[vTOP];
                    t.VisibleFaces[vBOTTOM] = true;
                    t.VisibleFaces[vTOP] = false;
                    RenderSlopedCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TILE_SLOPE_W, t.TileSlopeSteepness, 0, TileName);
                    t.VisibleFaces[vBOTTOM] = visB;
                    t.VisibleFaces[vTOP] = visT;
                }
            }
            return;
        }

        /// <summary>
        /// Renders the slope E tile.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static void RenderSlopeETile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                string TileName;
                if (invert == false)
                {
                    if (t.isWater == Water)
                    {
                        //A floor
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        RenderSlopedCuboid(parent, x, y, t, Water, FLOOR_ADJ, t.floorHeight, TILE_SLOPE_E, t.TileSlopeSteepness, 1, TileName);
                    }
                }
                else
                {
                    //It's invert
                    TileName = "E_Ceiling_" + x.ToString("D2") + "_" + y.ToString("D2");
                    bool visB = t.VisibleFaces[vBOTTOM];
                    bool visT = t.VisibleFaces[vTOP];
                    t.VisibleFaces[vBOTTOM] = true;
                    t.VisibleFaces[vTOP] = false;
                    RenderSlopedCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TILE_SLOPE_E, t.TileSlopeSteepness, 0, TileName);
                    t.VisibleFaces[vBOTTOM] = visB;
                    t.VisibleFaces[vTOP] = visT;
                }
            }
            return;
        }



        /// <summary>
        /// Use to calculate texture offsets.
        /// </summary>
        /// <param name="face"></param>
        /// <param name="t"></param>
        /// <returns>0 always since this is UW and this is only needed for shock tiles</returns>
        static float CalcCeilOffset(int face, TileInfo t)
        {
            // int ceilOffset = t.ceilingHeight;

            // if (_RES != GAME_SHOCK)
            // {
            return 0;
            // }
            // else
            // {
            //     switch (face)
            //     {
            //         case fEAST:
            //             ceilOffset = t.shockEastCeilHeight; break;
            //         case fWEST:
            //             ceilOffset = t.shockWestCeilHeight; break;
            //         case fSOUTH:
            //             ceilOffset = t.shockSouthCeilHeight; break;
            //         case fNORTH:
            //             ceilOffset = t.shockNorthCeilHeight; break;
            //     }
            //     float shock_ceil = CurrentTileMap().SHOCK_CEILING_HEIGHT;
            //     float floorOffset = shock_ceil - ceilOffset - 8;  //The floor of the tile if it is 1 texture tall.
            //     while (floorOffset >= 8)  //Reduce the offset to 0 to 7 since textures go up in steps of 1/8ths
            //     {
            //         floorOffset -= 8;
            //     }
            //     return floorOffset * 0.125f;
            // }
        }

        /// <summary>
        /// Gets the wall texture for the specified face
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="face">Face.</param>
        /// <param name="t">T.</param>
        public static int WallTexture(int face, TileInfo t)
        {
            int wallTexture;
            //int ceilOffset = 0;
            wallTexture = t.wallTexture;
            switch (face)
            {
                case fSOUTH:
                    wallTexture = t.South;
                    break;
                case fNORTH:
                    wallTexture = t.North;
                    break;
                case fEAST:
                    wallTexture = t.East;
                    break;
                case fWEST:
                    wallTexture = t.West;
                    break;
            }
            if ((wallTexture < 0) || (wallTexture > 512))
            {
                wallTexture = 0;
            }
            // if (debugtextures)
            // {
            //     return wallTexture;
            // }
            return t.map.texture_map[wallTexture];
        }

        /// <summary>
        /// Returns the floor texture from the texture map.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="face">Face.</param>
        /// <param name="t">T.</param>
        public static int FloorTexture(int face, TileInfo t)
        {
            int floorTexture;
            // if (debugtextures)
            // {
            //     return t.floorTexture;
            // }
            if (face == fCEIL)
            {
                floorTexture = t.map.texture_map[t.shockCeilingTexture];
            }
            else
            {
                //floorTexture = t.floorTexture;
                switch (_RES)
                {
                    //case GAME_SHOCK:
                    case GAME_UW2:
                        floorTexture = t.map.texture_map[t.floorTexture];
                        //floorTexture = t.floorTexture;
                        break;
                    default:
                        floorTexture = t.map.texture_map[t.floorTexture + 48];
                        break;
                }

            }

            if ((floorTexture < 0) || (floorTexture > 512))
            {
                floorTexture = 0;
            }
            return floorTexture;
        }


        /// <summary>
        /// Renders the diag NE portion.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="t">T.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderDiagNEPortion(Node3D parent, int Bottom, int Top, TileInfo t, string TileName)
        {
            //Does a thing.
            //Does a thing.
            //Draws 3 meshes. Outward diagonal wall. Back and side if visible.

            int NumberOfVisibleFaces = 1;//Will always have the diag.
                                         //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if ((i == vSOUTH) || (i == vWEST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        NumberOfVisibleFaces++;
                    }
                }
            }
            //Allocate enough verticea and UVs for the faces
            int[] MatsToUse = new int[NumberOfVisibleFaces];
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];

            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            //    //Now create the mesh
            //    GameObject Tile = new GameObject(TileName)
            //    {
            //        layer = LayerMask.NameToLayer("MapMesh")
            //    };
            //    Tile.transform.parent = parent.transform;
            //    Tile.transform.position = new Vector3(t.tileX * 1.2f, 0.0f, t.tileY * 1.2f);

            //    Tile.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //    MeshFilter mf = Tile.AddComponent<MeshFilter>();
            //    MeshRenderer mr = Tile.AddComponent<MeshRenderer>();
            //MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //mc.sharedMesh=null;

            //    Mesh mesh = new Mesh
            //    {
            //        subMeshCount = NumberOfVisibleFaces//Should be no of visible faces
            //    };

            var a_mesh = new ArrayMesh();

            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            //Set the diagonal face first
            MatsToUse[FaceCounter] = WallTexture(fSELF, t);
            verts[0] = new Vector3(-1.2f, baseHeight, 0f);
            verts[1] = new Vector3(-1.2f, floorHeight, 0f);
            verts[2] = new Vector3(0f, floorHeight, 1.2f);
            verts[3] = new Vector3(0f, baseHeight, 1.2f);

            uvs[0] = new Vector2(0.0f, uv0);
            uvs[1] = new Vector2(0.0f, uv1);
            uvs[2] = new Vector2(1, uv1);
            uvs[3] = new Vector2(1, uv0);
            FaceCounter++;

            for (int i = 0; i < 6; i++)
            {
                if ((t.VisibleFaces[i] == true) && ((i == vSOUTH) || (i == vWEST)))
                {//Will only render north or west if needed.
                 //float dimY = t.DimY;
                    float offset;
                    switch (i)
                    {
                        case vSOUTH:
                            {
                                //south wall vertices
                                offset = CalcCeilOffset(fSOUTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fSOUTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);

                                break;
                            }

                        case vWEST:
                            {
                                //west wall vertices
                                offset = CalcCeilOffset(fWEST, t);
                                MatsToUse[FaceCounter] = WallTexture(fWEST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1, uv0 - offset);

                                break;
                            }
                    }
                    FaceCounter++;
                }
            }

            FaceCounter = 0;
            //Apply the uvs and create my tris
            //    mesh.vertices = verts;
            //    mesh.uv = uvs;
            int[] indices = new int[6];
            //Tris for diagonal.
            //Create normals
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            // mesh.SetTriangles(indices, 0);
            AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);

            FaceCounter = 1;

            for (int i = 0; i < 6; i++)
            {
                if ((i == vSOUTH) || (i == vWEST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        indices[0] = 0 + (4 * FaceCounter);
                        indices[1] = 1 + (4 * FaceCounter);
                        indices[2] = 2 + (4 * FaceCounter);
                        indices[3] = 0 + (4 * FaceCounter);
                        indices[4] = 2 + (4 * FaceCounter);
                        indices[5] = 3 + (4 * FaceCounter);
                        //mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                        FaceCounter++;
                    }
                }
            }

            return CreateMeshInstance(parent, t.tileX, t.tileY, TileName, a_mesh);
            //    mr.materials = MatsToUse;
            //    mesh.RecalculateNormals();
            //    mesh.RecalculateBounds();
            //    mf.mesh = mesh;
            //    if (EnableCollision)
            //    {
            //        MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //        mc.sharedMesh = null;
            //        mc.sharedMesh = mesh;
            //    }
            //mc.sharedMesh=mesh;
            // return;
        }


        /// <summary>
        /// Renders the diag SE portion.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="t">T.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderDiagSEPortion(Node3D parent, int Bottom, int Top, TileInfo t, string TileName)
        {
            //Does a thing.
            //Draws 3 meshes. Outward diagonal wall. Back and side if visible.

            int NumberOfVisibleFaces = 1;//Will always have the diag.
                                         //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if ((i == vNORTH) || (i == vWEST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        NumberOfVisibleFaces++;
                    }
                }
            }
            //Allocate enough vertice and UVs for the faces
            int[] MatsToUse = new int[NumberOfVisibleFaces];
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);

            //    //Now create the mesh
            //    GameObject Tile = new GameObject(TileName)
            //    {
            //        layer = LayerMask.NameToLayer("MapMesh")
            //    };
            //    Tile.transform.parent = parent.transform;
            //    Tile.transform.position = new Vector3(t.tileX * 1.2f, 0.0f, t.tileY * 1.2f);

            //    Tile.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //    MeshFilter mf = Tile.AddComponent<MeshFilter>();
            //    MeshRenderer mr = Tile.AddComponent<MeshRenderer>();
            //MeshCollider mc = Tile.AddComponent<MeshCollider>();
            ///mc.sharedMesh=null;

            var a_mesh = new ArrayMesh();


            //    Mesh mesh = new Mesh
            //    {
            //        subMeshCount = NumberOfVisibleFaces//Should be no of visible faces
            //    };

            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            MatsToUse[FaceCounter] = WallTexture(fSELF, t);
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            //Set the diagonal face first

            verts[0] = new Vector3(0f, baseHeight, 0f);
            verts[1] = new Vector3(0f, floorHeight, 0f);
            verts[2] = new Vector3(-1.2f, floorHeight, 1.2f);
            verts[3] = new Vector3(-1.2f, baseHeight, 1.2f);

            uvs[0] = new Vector2(0.0f, uv0);
            uvs[1] = new Vector2(0.0f, uv1);
            uvs[2] = new Vector2(1, uv1);
            uvs[3] = new Vector2(1, uv0);
            FaceCounter++;

            for (int i = 0; i < 6; i++)
            {
                if ((t.VisibleFaces[i] == true) && ((i == vNORTH) || (i == vWEST)))
                {//Will only render north or west if needed.
                    float offset;
                    switch (i)
                    {
                        case vNORTH:
                            {
                                //north wall vertices
                                offset = CalcCeilOffset(fNORTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fNORTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f, baseHeight, 1.2f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f, floorHeight, 1.2f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f);

                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1, uv0 - offset);

                                break;
                            }
                        case vWEST:
                            {
                                //west wall vertices
                                offset = CalcCeilOffset(fWEST, t);
                                MatsToUse[FaceCounter] = WallTexture(fWEST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1, uv0 - offset);

                                break;
                            }
                    }
                    FaceCounter++;
                }
            }

            FaceCounter = 0;
            //Create normals
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            //Apply the uvs and create my tris
            //    mesh.vertices = verts;
            //    mesh.uv = uvs;
            int[] indices = new int[6];
            //Tris for diagonal.

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);

            // mesh.SetTriangles(indices, 0);
            FaceCounter = 1;

            for (int i = 0; i < 6; i++)
            {
                if ((i == vNORTH) || (i == vWEST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        indices[0] = 0 + (4 * FaceCounter);
                        indices[1] = 1 + (4 * FaceCounter);
                        indices[2] = 2 + (4 * FaceCounter);
                        indices[3] = 0 + (4 * FaceCounter);
                        indices[4] = 2 + (4 * FaceCounter);
                        indices[5] = 3 + (4 * FaceCounter);
                        //mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                        FaceCounter++;
                    }
                }
            }
            return CreateMeshInstance(parent, t.tileX, t.tileY, TileName, a_mesh);

            //    mr.materials = MatsToUse;
            //    mesh.RecalculateNormals();
            //    mesh.RecalculateBounds();
            //    mf.mesh = mesh;
            //    if (EnableCollision)
            //    {
            //        MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //        mc.sharedMesh = null;
            //        mc.sharedMesh = mesh;
            //    }
            //mc.sharedMesh=mesh;
            //return;
        }


        /// <summary>
        /// Renders an open tile with no slopes
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="invert">If set to <c>true</c> invert.</param>
        static Node3D RenderDiagOpenTile(Node3D parent, int x, int y, TileInfo t, bool Water, bool invert)
        {
            if (t.Render == true)
            {
                if (t.isWater == Water)
                {
                    string TileName;
                    if (invert == false)
                    {
                        //Bottom face 
                        if (t.TerrainChange)
                        {
                            TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                            return RenderPrism(parent, x, y, t, Water, -16, t.floorHeight, TileName);
                        }
                        else
                        {
                            TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                            return RenderPrism(parent, x, y, t, Water, 0, t.floorHeight, TileName);
                        }
                    }
                    else
                    {
                        //Ceiling version of the tile
                        bool visB = t.VisibleFaces[vBOTTOM];
                        bool visT = t.VisibleFaces[vTOP];
                        t.VisibleFaces[vBOTTOM] = true;
                        t.VisibleFaces[vTOP] = false;
                        TileName = "Tile_" + x.ToString("D2") + "_" + y.ToString("D2");
                        var output = RenderCuboid(parent, x, y, t, Water, CEILING_HEIGHT - t.ceilingHeight, CEILING_HEIGHT + CEIL_ADJ, TileName);
                        t.VisibleFaces[vBOTTOM] = visB;
                        t.VisibleFaces[vTOP] = visT;
                        return output;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Renders the diag SW portion.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="t">T.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderDiagSWPortion(Node3D parent, int Bottom, int Top, TileInfo t, string TileName)
        {

            //Does a thing.
            //Does a thing.
            //Draws 3 meshes. Outward diagonal wall. Back and side if visible.
            int NumberOfVisibleFaces = 1;//Will always have the diag.
                                         //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if ((i == vNORTH) || (i == vEAST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        NumberOfVisibleFaces++;
                    }
                }
            }
            //Allocate enough verticea and UVs for the faces
            int[] MatsToUse = new int[NumberOfVisibleFaces];
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            float dimY = t.DimY;
            //Now create the mesh
            // GameObject Tile = new GameObject(TileName)
            // {
            //     layer = LayerMask.NameToLayer("MapMesh")
            // };
            // Tile.transform.parent = parent.transform;
            // Tile.transform.position = new Vector3(t.tileX * 1.2f, 0.0f, t.tileY * 1.2f);

            // Tile.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // MeshFilter mf = Tile.AddComponent<MeshFilter>();
            // MeshRenderer mr = Tile.AddComponent<MeshRenderer>();
            //MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //mc.sharedMesh=null;

            // Mesh mesh = new Mesh
            // {
            //     subMeshCount = NumberOfVisibleFaces//Should be no of visible faces
            // };

            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.

            MatsToUse[FaceCounter] = WallTexture(fSELF, t);

            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            //Set the diagonal face first
            verts[0] = new Vector3(0f, baseHeight, 1.2f);
            verts[1] = new Vector3(0f, floorHeight, 1.2f);
            verts[2] = new Vector3(-1.2f, floorHeight, 0f);
            verts[3] = new Vector3(-1.2f, baseHeight, 0f);

            uvs[0] = new Vector2(0.0f, uv0);
            uvs[1] = new Vector2(0.0f, uv1);
            uvs[2] = new Vector2(1, uv1);
            uvs[3] = new Vector2(1, uv0);
            FaceCounter++;

            for (int i = 0; i < 6; i++)
            {
                if ((t.VisibleFaces[i] == true) && ((i == vNORTH) || (i == vEAST)))
                {//Will only render north or west if needed.
                    float offset;
                    switch (i)
                    {
                        case vNORTH:
                            {
                                //north wall vertices
                                offset = CalcCeilOffset(fNORTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fNORTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f, baseHeight, 1.2f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f, floorHeight, 1.2f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f);

                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1, uv0 - offset);

                                break;
                            }

                        case vEAST:
                            {
                                //east wall vertices
                                offset = CalcCeilOffset(fEAST, t);
                                MatsToUse[FaceCounter] = WallTexture(fEAST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                break;
                            }
                    }
                    FaceCounter++;
                }
            }

            var a_mesh = new ArrayMesh(); //= Mesh as ArrayMesh;
            //create normals
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            //Apply the uvs and create my tris
            // mesh.vertices = verts;
            // mesh.uv = uvs;
            int[] indices = new int[6];
            //Tris for diagonal.
            FaceCounter = 0;
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            //mesh.SetTriangles(indices, 0);
            AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);

            FaceCounter = 1;

            for (int i = 0; i < 6; i++)
            {
                if ((i == vNORTH) || (i == vEAST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        indices[0] = 0 + (4 * FaceCounter);
                        indices[1] = 1 + (4 * FaceCounter);
                        indices[2] = 2 + (4 * FaceCounter);
                        indices[3] = 0 + (4 * FaceCounter);
                        indices[4] = 2 + (4 * FaceCounter);
                        indices[5] = 3 + (4 * FaceCounter);
                        //mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                        FaceCounter++;
                    }
                }
            }

            // mr.materials = MatsToUse;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // mf.mesh = mesh;
            // if (EnableCollision)
            // {
            //     MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //     mc.sharedMesh = null;
            //     mc.sharedMesh = mesh;
            // }

            return CreateMeshInstance(parent, t.tileX, t.tileY, TileName, a_mesh);

            //mc.sharedMesh=mesh;

        }
        /// <summary>
        /// Renders the diag NW portion.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="t">T.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderDiagNWPortion(Node3D parent, int Bottom, int Top, TileInfo t, string TileName)
        {
            //Does a thing.
            //Does a thing.
            //Draws 3 meshes. Outward diagonal wall. Back and side if visible.


            int NumberOfVisibleFaces = 1;//Will always have the diag.
                                         //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if ((i == vSOUTH) || (i == vEAST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        NumberOfVisibleFaces++;
                    }
                }
            }
            //Allocate enough verticea and UVs for the faces
            int[] MatsToUse = new int[NumberOfVisibleFaces];
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            float dimY = t.DimY;

            var a_mesh = new ArrayMesh();


            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            //Set the diagonal face first
            MatsToUse[FaceCounter] = WallTexture(fSELF, t);

            verts[0] = new Vector3(-1.2f, baseHeight, 1.2f);
            verts[1] = new Vector3(-1.2f, floorHeight, 1.2f);
            verts[2] = new Vector3(0f, floorHeight, 0f);
            verts[3] = new Vector3(0f, baseHeight, 0f);

            uvs[0] = new Vector2(0.0f, uv0);
            uvs[1] = new Vector2(0.0f, uv1);
            uvs[2] = new Vector2(1, uv1);
            uvs[3] = new Vector2(1, uv0);
            FaceCounter++;

            for (int i = 0; i < 6; i++)
            {
                if ((t.VisibleFaces[i] == true) && ((i == vSOUTH) || (i == vEAST)))
                {//Will only render north or west if needed.
                    float offset;
                    switch (i)
                    {
                        case vEAST:
                            {
                                //east wall vertices
                                offset = CalcCeilOffset(fEAST, t);
                                MatsToUse[FaceCounter] = WallTexture(fEAST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);
                                break;
                            }

                        case vSOUTH:
                            {
                                //south wall vertices
                                offset = CalcCeilOffset(fSOUTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fSOUTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);

                                break;
                            }
                    }
                    FaceCounter++;
                }
            }
            FaceCounter = 0;
            //create normals from verts
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }
            //Apply the uvs and create my tris
            //    mesh.vertices = verts;
            //    mesh.uv = uvs;
            int[] indices = new int[6];
            //Tris for diagonal.

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            //mesh.SetTriangles(indices, 0);

            AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);

            FaceCounter = 1;

            for (int i = 0; i < 6; i++)
            {
                if ((i == vSOUTH) || (i == vEAST))
                {
                    if (t.VisibleFaces[i] == true)
                    {
                        indices[0] = 0 + (4 * FaceCounter);
                        indices[1] = 1 + (4 * FaceCounter);
                        indices[2] = 2 + (4 * FaceCounter);
                        indices[3] = 0 + (4 * FaceCounter);
                        indices[4] = 2 + (4 * FaceCounter);
                        indices[5] = 3 + (4 * FaceCounter);
                        // mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                        FaceCounter++;
                    }
                }
            }
            return CreateMeshInstance(parent, t.tileX, t.tileY, TileName, a_mesh);
        }




        /// <summary>
        /// Renders a cuboid with sloped tops
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="SlopeDir">Slope dir.</param>
        /// <param name="Steepness">Steepness.</param>
        /// <param name="Floor">Floor.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderSlopedCuboid(Node3D parent, int x, int y, TileInfo t, bool Water, int Bottom, int Top, int SlopeDir, int Steepness, int Floor, string TileName)
        {

            //Draws a cube with sloped tops

            float AdjustUpperNorth = 0f;
            float AdjustUpperSouth = 0f;
            float AdjustUpperEast = 0f;
            float AdjustUpperWest = 0f;

            float AdjustLowerNorth = 0f;
            float AdjustLowerSouth = 0f;
            float AdjustLowerEast = 0f;
            float AdjustLowerWest = 0f;

            float AdjustUpperNorthEast = 0f;
            float AdjustUpperNorthWest = 0f;
            float AdjustUpperSouthEast = 0f;
            float AdjustUpperSouthWest = 0f;

            float AdjustLowerNorthEast = 0f;
            float AdjustLowerNorthWest = 0f;
            float AdjustLowerSouthEast = 0f;
            float AdjustLowerSouthWest = 0f;

            if (Floor == 1)
            {
                switch (SlopeDir)
                {
                    case TILE_SLOPE_N:
                        AdjustUpperNorth = Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_S:
                        AdjustUpperSouth = Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_E:
                        AdjustUpperEast = Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_W:
                        AdjustUpperWest = Steepness * 0.15f;
                        break;
                }
            }
            if (Floor == 0)
            {
                switch (SlopeDir)
                {
                    case TILE_SLOPE_N:
                        AdjustLowerNorth = -Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_S:
                        AdjustLowerSouth = -Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_E:
                        AdjustLowerEast = -Steepness * 0.15f;
                        break;
                    case TILE_SLOPE_W:
                        AdjustLowerWest = -Steepness * 0.15f;
                        break;
                }
            }

            int NumberOfVisibleFaces = 0;
            int NumberOfSlopedFaces = 0;
            int SlopesAdded = 0;
            //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    NumberOfVisibleFaces++;
                    if (
                            (((SlopeDir == TILE_SLOPE_N) || (SlopeDir == TILE_SLOPE_S)) && ((i == vWEST) || (i == vEAST)))
                            ||
                            (((SlopeDir == TILE_SLOPE_E) || (SlopeDir == TILE_SLOPE_W)) && ((i == vNORTH) || (i == vSOUTH)))
                        )
                    {
                        NumberOfSlopedFaces++;  //SHould only be to a max of two
                    }
                }
            }
            //Allocate enough verticea and UVs for the faces
            int[] MatsToUse = new int[NumberOfVisibleFaces + NumberOfSlopedFaces];
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4 + +NumberOfSlopedFaces * 3];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4 + NumberOfSlopedFaces * 3];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            float dimY = t.DimY;


            //Now create the mesh
            var a_mesh = new ArrayMesh();

            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
                                //float PolySize= Top-Bottom;
                                //float uv0= (float)(Bottom*0.125f);
                                //float uv1=(PolySize / 8.0f) + (uv0);
            CalcUVForSlopedCuboid(Top, Bottom, out float uv0, out float uv1);
            float slopeHeight;
            float uv0Slope;
            float uv1Slope;
            if (Floor == 1)
            {
                CalcUVForSlopedCuboid(Top + Steepness, Bottom, out uv0Slope, out uv1Slope);
                slopeHeight = floorHeight;
            }
            else
            {
                CalcUVForSlopedCuboid(Top, Bottom - Steepness, out uv0Slope, out uv1Slope);
                slopeHeight = baseHeight;
            }


            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    float offset;
                    switch (i)
                    {
                        case vTOP:
                            {

                                //Set the verts	
                                MatsToUse[FaceCounter] = FloorTexture(fSELF, t);

                                verts[0 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight + AdjustUpperWest + AdjustUpperSouth + AdjustUpperSouthWest, 0.0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight + AdjustUpperWest + AdjustUpperNorth + AdjustUpperNorthWest, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperNorth + AdjustUpperEast + AdjustUpperNorthEast, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperSouth + AdjustUpperEast + AdjustUpperSouthEast, 0.0f);
                                //Allocate UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 0.0f);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 1.0f * dimY);
                                break;
                            }

                        case vNORTH:
                            {
                                //north wall vertices
                                offset = CalcCeilOffset(fNORTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fNORTH, t);
                                int oldSlopeDir = SlopeDir;
                                if ((Floor == 0) && (SlopeDir == TILE_SLOPE_S))
                                {
                                    SlopeDir = TILE_SLOPE_N;
                                }
                                switch (SlopeDir)
                                {
                                    case TILE_SLOPE_N:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerSouth + AdjustLowerWest, 1.2f * dimY);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperNorth + AdjustUpperEast, 1.2f * dimY);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperNorth + AdjustUpperWest, 1.2f * dimY);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerSouth + AdjustLowerEast, 1.2f * dimY);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0Slope - offset);//bottom uv
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1Slope - offset);//top uv
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1Slope - offset);//top uv
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0Slope - offset);//bottom uv
                                        break;

                                    default:

                                        verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperNorthEast, 1.2f * dimY);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperNorthWest, 1.2f * dimY);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);//bottom uv
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);//top uv
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);//top uv
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);//bottom uv
                                        break;
                                }
                                if ((SlopeDir == TILE_SLOPE_E) || (SlopeDir == TILE_SLOPE_W))
                                {//Insert my verts for this slope														
                                    int index = uvs.GetUpperBound(0) - ((NumberOfSlopedFaces - SlopesAdded) * 3) + 1;
                                    MatsToUse[MatsToUse.GetUpperBound(0) - NumberOfSlopedFaces + SlopesAdded + 1] = MatsToUse[FaceCounter];
                                    int origSlopeDir = SlopeDir;
                                    if (Floor == 0)
                                    {//flip my tile types when doing ceilings
                                        if (SlopeDir == TILE_SLOPE_E)
                                        {
                                            SlopeDir = TILE_SLOPE_W;
                                        }
                                        else
                                        {
                                            SlopeDir = TILE_SLOPE_E;
                                        }
                                    }

                                    switch (SlopeDir)
                                    {
                                        case TILE_SLOPE_E:
                                            {
                                                verts[index + 0] = new Vector3(-1.2f * dimX, slopeHeight + AdjustLowerSouth + AdjustLowerWest, 1.2f * dimY);
                                                verts[index + 1] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperNorth + AdjustUpperEast, 1.2f * dimY);
                                                verts[index + 2] = new Vector3(0f, slopeHeight + AdjustUpperNorth + AdjustUpperWest, 1.2f * dimY);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse + Steepness * 0.125f);   //1, vertical alignment	
                                                }
                                                break;
                                            }

                                        case TILE_SLOPE_W:
                                            {

                                                verts[index + 0] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperNorth + AdjustUpperEast, 1.2f * dimY);
                                                verts[index + 1] = new Vector3(0f, slopeHeight + AdjustUpperNorth + AdjustUpperWest, 1.2f * dimY);
                                                verts[index + 2] = new Vector3(0f, slopeHeight + AdjustLowerSouth + AdjustLowerEast, 1.2f * dimY);
                                                //uvs[index+0]= new Vector2(0,0);
                                                //uvs[index+1]= new Vector2(1,1);
                                                //uvs[index+2]= new Vector2(1,0);
                                                float uv0edge = 0;
                                                float uvToUse;
                                                if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                float uv1edge;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                                                                                                 //uvs[index+0]= new Vector2(0,0);//0, vertical alignment
                                                                                                                                 //uvs[index+1]= new Vector2(1,  (float)Steepness*0.125f); //vertical + scale
                                                                                                                                 //uvs[index+2]= new Vector2(1,  (float)Steepness*0.125f);	//1, vertical alignment	
                                                    uvs[index + 0] = new Vector2(0, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 2] = new Vector2(1, uvToUse);
                                                }
                                                break;
                                            }

                                    }

                                    SlopesAdded++;
                                }

                                SlopeDir = oldSlopeDir;
                                break;
                            }//end north


                        case vSOUTH:
                            {
                                //south wall vertices
                                offset = CalcCeilOffset(fSOUTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fSOUTH, t);
                                int oldSlopeDir = SlopeDir;
                                if ((Floor == 0) && (SlopeDir == TILE_SLOPE_N))
                                {
                                    SlopeDir = TILE_SLOPE_S;
                                }
                                switch (SlopeDir)
                                {
                                    case TILE_SLOPE_S:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerNorth + AdjustLowerEast, 0f);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperSouth + AdjustUpperWest, 0f);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperSouth + AdjustUpperEast, 0f);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerNorth + AdjustLowerWest, 0f);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0Slope - offset);//bottom uv
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1Slope - offset);//top uv
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1Slope - offset);//top uv
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0Slope - offset);//bottom uv
                                        break;
                                    default:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperSouthWest, 0f);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperSouthEast, 0f);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);
                                        break;
                                }

                                if ((SlopeDir == TILE_SLOPE_E) || (SlopeDir == TILE_SLOPE_W))
                                {//Insert my verts for this slope

                                    int origSlopeDir = SlopeDir;
                                    if (Floor == 0)
                                    {//flip my tile types when doing ceilings
                                        if (SlopeDir == TILE_SLOPE_E)
                                        {
                                            SlopeDir = TILE_SLOPE_W;
                                        }
                                        else
                                        {
                                            SlopeDir = TILE_SLOPE_E;
                                        }
                                    }


                                    int index = uvs.GetUpperBound(0) - ((NumberOfSlopedFaces - SlopesAdded) * 3) + 1;
                                    MatsToUse[MatsToUse.GetUpperBound(0) - NumberOfSlopedFaces + SlopesAdded + 1] = MatsToUse[FaceCounter];
                                    switch (SlopeDir)
                                    {
                                        case TILE_SLOPE_W:
                                            {

                                                verts[index + 0] = new Vector3(0f, slopeHeight + AdjustLowerNorth + AdjustLowerEast, 0f);
                                                verts[index + 1] = new Vector3(0f, slopeHeight + AdjustUpperSouth + AdjustUpperWest, 0f);
                                                verts[index + 2] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperSouth + AdjustUpperEast, 0f);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse + Steepness * 0.125f);   //1, vertical alignment	
                                                }
                                                break;
                                            }

                                        case TILE_SLOPE_E:
                                            {

                                                verts[index + 0] = new Vector3(0f, slopeHeight + AdjustLowerNorth + AdjustLowerEast, 0f);
                                                verts[index + 1] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperSouth + AdjustUpperEast, 0f);
                                                verts[index + 2] = new Vector3(-1.2f * dimX, slopeHeight + AdjustLowerNorth + AdjustLowerWest, 0f);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    //uvs[index+0]= new Vector2(0,0);//0, vertical alignment
                                                    //uvs[index+1]= new Vector2(1,  (float)Steepness*0.125f); //vertical + scale
                                                    //uvs[index+2]= new Vector2(1,  (float)Steepness*0.125f);	//1, vertical alignment	
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 2] = new Vector2(1, uvToUse);
                                                }
                                                break;
                                            }

                                    }

                                    SlopesAdded++;
                                }

                                SlopeDir = oldSlopeDir;
                                break;
                            }//end south

                        case vWEST:
                            {
                                offset = CalcCeilOffset(fWEST, t);

                                MatsToUse[FaceCounter] = WallTexture(fWEST, t);

                                int oldSlopeDir = SlopeDir;
                                if ((Floor == 0) && (SlopeDir == TILE_SLOPE_E))
                                {
                                    SlopeDir = TILE_SLOPE_W;
                                }

                                switch (SlopeDir)
                                {
                                    case TILE_SLOPE_W:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerEast + AdjustLowerSouth, 1.2f * dimY);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperWest + AdjustUpperNorth, 1.2f * dimY);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperWest + AdjustUpperSouth, 0f);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerEast + AdjustLowerNorth, 0f);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0Slope - offset);//bottom uv
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1Slope - offset);//top uv
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1Slope - offset);//top uv
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0Slope - offset);//bottom uv
                                        break;
                                    default:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperNorthWest, 1.2f * dimY);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight + AdjustUpperSouthWest, 0f);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                        break;
                                }

                                if ((SlopeDir == TILE_SLOPE_N) || (SlopeDir == TILE_SLOPE_S))
                                {//Insert my verts for this slope
                                    MatsToUse[MatsToUse.GetUpperBound(0) - NumberOfSlopedFaces + SlopesAdded + 1] = MatsToUse[FaceCounter];
                                    int index = uvs.GetUpperBound(0) - ((NumberOfSlopedFaces - SlopesAdded) * 3) + 1;
                                    int origSlopeDir = SlopeDir;
                                    if (Floor == 0)
                                    {//flip my tile types when doing ceilings
                                        if (SlopeDir == TILE_SLOPE_N)
                                        {
                                            SlopeDir = TILE_SLOPE_S;
                                        }
                                        else
                                        {
                                            SlopeDir = TILE_SLOPE_N;
                                        }
                                    }
                                    switch (SlopeDir)
                                    {
                                        case TILE_SLOPE_N:
                                            {

                                                verts[index + 0] = new Vector3(0f, slopeHeight + AdjustLowerEast + AdjustLowerSouth, 1.2f * dimY);
                                                verts[index + 1] = new Vector3(0f, slopeHeight + AdjustUpperWest + AdjustUpperNorth, 1.2f * dimY);
                                                verts[index + 2] = new Vector3(0f, slopeHeight + AdjustUpperWest + AdjustUpperSouth, 0f);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment		
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse + Steepness * 0.125f);   //1, vertical alignment	
                                                }


                                                break;
                                            }

                                        case TILE_SLOPE_S:
                                            {
                                                //ceil n west
                                                verts[index + 0] = new Vector3(0f, slopeHeight + AdjustUpperWest + AdjustUpperNorth, 1.2f * dimY);
                                                verts[index + 1] = new Vector3(0f, slopeHeight + AdjustUpperWest + AdjustUpperSouth, 0f);
                                                verts[index + 2] = new Vector3(0f, slopeHeight + AdjustLowerEast + AdjustLowerNorth, 0f);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    //uvs[index+0]= new Vector2(0,0);//0, vertical alignment
                                                    //uvs[index+1]= new Vector2(1, (float)Steepness*0.125f); //vertical + scale
                                                    //uvs[index+2]= new Vector2(1, (float)Steepness*0.125f);	//1, vertical alignment	
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 2] = new Vector2(1, uvToUse);
                                                }
                                                break;
                                            }

                                    }

                                    SlopesAdded++;
                                }


                                SlopeDir = oldSlopeDir;
                                break;

                            }//end west

                        case vEAST:
                            {
                                //east wall vertices
                                offset = CalcCeilOffset(fEAST, t);

                                MatsToUse[FaceCounter] = WallTexture(fEAST, t);

                                int oldSlopeDir = SlopeDir;
                                if ((Floor == 0) && (SlopeDir == TILE_SLOPE_W))
                                {
                                    SlopeDir = TILE_SLOPE_E;
                                }
                                switch (SlopeDir)
                                {
                                    case TILE_SLOPE_E:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerWest + AdjustLowerNorth, 0f);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperEast + AdjustUpperSouth, 0f);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperEast + AdjustUpperNorth, 1.2f * dimY);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerWest + AdjustLowerSouth, 1.2f * dimY);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0Slope - offset);//bottom uv
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1Slope - offset);//top uv
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1Slope - offset);//top uv
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0Slope - offset);//bottom uv
                                        break;
                                    default:
                                        verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                        verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperSouthEast, 0f);
                                        verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight + AdjustUpperNorthEast, 1.2f * dimY);
                                        verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                        uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);//0
                                        uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);//1
                                        uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);//1
                                        uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);//0
                                        break;
                                }
                                if ((SlopeDir == TILE_SLOPE_N) || (SlopeDir == TILE_SLOPE_S))
                                {//Insert my verts for this slope

                                    MatsToUse[MatsToUse.GetUpperBound(0) - NumberOfSlopedFaces + SlopesAdded + 1] = MatsToUse[FaceCounter];
                                    int index = uvs.GetUpperBound(0) - ((NumberOfSlopedFaces - SlopesAdded) * 3) + 1;
                                    int origSlopeDir = SlopeDir;
                                    if (Floor == 0)
                                    {//flip my tile types when doing ceilings
                                        if (SlopeDir == TILE_SLOPE_N)
                                        {
                                            SlopeDir = TILE_SLOPE_S;
                                        }
                                        else
                                        {
                                            SlopeDir = TILE_SLOPE_N;
                                        }
                                    }
                                    switch (SlopeDir)
                                    {
                                        case TILE_SLOPE_S:
                                            {
                                                //ceil_n east		
                                                verts[index + 0] = new Vector3(-1.2f * dimX, slopeHeight + AdjustLowerWest + AdjustLowerNorth, 0f);
                                                verts[index + 1] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperEast + AdjustUpperSouth, 0f);
                                                verts[index + 2] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperEast + AdjustUpperNorth, 1.2f * dimY);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(0, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse + Steepness * 0.125f);   //1, vertical alignment	
                                                }
                                                break;
                                            }

                                        case TILE_SLOPE_N:
                                            {
                                                //hey east on tile s ceil
                                                verts[index + 0] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperEast + AdjustUpperSouth, 0f);
                                                verts[index + 1] = new Vector3(-1.2f * dimX, slopeHeight + AdjustUpperEast + AdjustUpperNorth, 1.2f * dimY);
                                                verts[index + 2] = new Vector3(-1.2f * dimX, slopeHeight + AdjustLowerWest + AdjustLowerSouth, 1.2f * dimY);
                                                float uv0edge;
                                                float uv1edge;
                                                float uvToUse;
                                                //if (t.shockEastOffset==0){uvToUse=+uv1edge;}else{uvToUse=-uv0edge;}
                                                //uvToUse=uv0edge;
                                                if (Floor == 1)
                                                {
                                                    CalcUVForSlopedCuboid(Top + Steepness, Top, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }
                                                    uvs[index + 0] = new Vector2(0, uvToUse);//0, vertical alignment
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f); //vertical + scale
                                                    uvs[index + 2] = new Vector2(1, uvToUse);   //1, vertical alignment	
                                                }
                                                else
                                                {
                                                    CalcUVForSlopedCuboid(Bottom, Bottom - Steepness, out uv0edge, out uv1edge);
                                                    if (offset == 0) { uvToUse = +uv0edge; } else { uvToUse = uv0edge - offset; }//Ceil
                                                    uvs[index + 0] = new Vector2(0, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 1] = new Vector2(1, uvToUse + Steepness * 0.125f);
                                                    uvs[index + 2] = new Vector2(1, uvToUse);
                                                }
                                                break;
                                            }


                                    }

                                    SlopesAdded++;
                                }

                                SlopeDir = oldSlopeDir;
                                break;
                            }//end east


                        case vBOTTOM:
                            {
                                //bottom wall vertices
                                MatsToUse[FaceCounter] = FloorTexture(fCEIL, t);

                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerSouth + AdjustLowerEast + AdjustLowerSouthEast, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, baseHeight + AdjustLowerEast + AdjustLowerNorth + AdjustLowerNorthEast, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerNorth + AdjustLowerWest + AdjustLowerNorthWest, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight + AdjustLowerSouth + AdjustLowerWest + AdjustLowerSouthWest, 1.2f * dimY);

                                //Change default UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, 1.0f * dimY);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, 0.0f);
                                break;
                            }
                    }
                    FaceCounter++;
                }
            }

            //Apply the uvs and create my tris
            // mesh.vertices = verts;
            // mesh.uv = uvs;
            FaceCounter = 0;

            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            int[] indices = new int[6];
            int LastIndex = 0;
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    indices[0] = 0 + (4 * FaceCounter);
                    indices[1] = 1 + (4 * FaceCounter);
                    indices[2] = 2 + (4 * FaceCounter);
                    indices[3] = 0 + (4 * FaceCounter);
                    indices[4] = 2 + (4 * FaceCounter);
                    indices[5] = 3 + (4 * FaceCounter);
                    LastIndex = 3 + (4 * FaceCounter);
                    //mesh.SetTriangles(indices, FaceCounter);
                    AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                    FaceCounter++;
                }
            }
            //Insert any sloped tris at the end
            indices = new int[3];
            //FaceCounter=0;
            SlopesAdded = 0;
            LastIndex++;
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    if (
                            (((SlopeDir == TILE_SLOPE_N) || (SlopeDir == TILE_SLOPE_S)) && ((i == vWEST) || (i == vEAST)))
                            ||
                            (((SlopeDir == TILE_SLOPE_E) || (SlopeDir == TILE_SLOPE_W)) && ((i == vNORTH) || (i == vSOUTH)))
                    )
                    {
                        indices[0] = 0 + LastIndex + (3 * SlopesAdded);
                        indices[1] = 1 + LastIndex + (3 * SlopesAdded);
                        indices[2] = 2 + LastIndex + (3 * SlopesAdded);
                        //mesh.SetTriangles(indices, FaceCounter + SlopesAdded);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter + SlopesAdded, a_mesh, normals, indices);
                        SlopesAdded++;
                    }
                }
            }

            return CreateMeshInstance(parent, x, y, TileName, a_mesh);
            // mr.materials = MatsToUse;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // mf.mesh = mesh;
            // if (EnableCollision)
            // {
            //     MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //     mc.sharedMesh = null;
            //     mc.sharedMesh = mesh;
            // }
            //mc.sharedMesh=mesh;

        }







        /// <summary>
        /// Renders the floor of a diag tile
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="t">T.</param>
        /// <param name="Water">If set to <c>true</c> water.</param>
        /// <param name="Bottom">Bottom.</param>
        /// <param name="Top">Top.</param>
        /// <param name="TileName">Tile name.</param>
        static Node3D RenderPrism(Node3D parent, int x, int y, TileInfo t, bool Water, int Bottom, int Top, string TileName)
        {

            //Draw a cube with no slopes.
            int NumberOfVisibleFaces = 0;
            //Get the number of faces
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    NumberOfVisibleFaces++;
                }
            }
            //Allocate enough verticea and UVs for the faces
            Vector3[] verts = new Vector3[NumberOfVisibleFaces * 4];
            Vector2[] uvs = new Vector2[NumberOfVisibleFaces * 4];
            float floorHeight = (float)(Top * 0.15f);
            float baseHeight = (float)(Bottom * 0.15f);
            float dimX = t.DimX;
            float dimY = t.DimY;


            int[] MatsToUse = new int[NumberOfVisibleFaces];
            //Now allocate the visible faces to triangles.
            int FaceCounter = 0;//Tracks which number face we are now on.
            float PolySize = Top - Bottom;
            float uv0 = (float)(Bottom * 0.125f);
            float uv1 = -(PolySize / 8.0f) + (uv0);
            //int vertCountOffset=0;
            for (int i = 0; i < 6; i++)
            {
                if (t.VisibleFaces[i] == true)
                {
                    float offset;
                    switch (i)
                    {
                        case vTOP:
                            {
                                //Set the verts	
                                MatsToUse[FaceCounter] = FloorTexture(fSELF, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight, 0.0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0.0f, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0.0f);
                                //Allocate UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 0.0f);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(1.0f * dimX, 1.0f * dimY);
                                break;
                            }

                        case vNORTH:
                            {
                                //north wall vertices
                                offset = CalcCeilOffset(fNORTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fNORTH, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);

                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);
                                break;
                            }

                        case vWEST:
                            {
                                //west wall vertices
                                offset = CalcCeilOffset(fWEST, t);
                                MatsToUse[FaceCounter] = WallTexture(fWEST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 1.2f * dimY);
                                verts[2 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                break;
                            }

                        case vEAST:
                            {
                                //east wall vertices
                                offset = CalcCeilOffset(fEAST, t);
                                MatsToUse[FaceCounter] = WallTexture(fEAST, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 1.2f * dimY);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimY, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimY, uv0 - offset);

                                break;
                            }

                        case vSOUTH:
                            {
                                offset = CalcCeilOffset(fSOUTH, t);
                                MatsToUse[FaceCounter] = WallTexture(fSOUTH, t);
                                //south wall vertices
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, floorHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, floorHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, uv0 - offset);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, uv1 - offset);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, uv1 - offset);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, uv0 - offset);

                                break;
                            }
                        case vBOTTOM:
                            {
                                //bottom wall vertices
                                MatsToUse[FaceCounter] = FloorTexture(fCEIL, t);
                                verts[0 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 1.2f * dimY);
                                verts[1 + (4 * FaceCounter)] = new Vector3(0f, baseHeight, 0f);
                                verts[2 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 0f);
                                verts[3 + (4 * FaceCounter)] = new Vector3(-1.2f * dimX, baseHeight, 1.2f * dimY);
                                //Change default UVs
                                uvs[0 + (4 * FaceCounter)] = new Vector2(0.0f, 0.0f);
                                uvs[1 + (4 * FaceCounter)] = new Vector2(0.0f, 1.0f * dimY);
                                uvs[2 + (4 * FaceCounter)] = new Vector2(dimX, 1.0f * dimY);
                                uvs[3 + (4 * FaceCounter)] = new Vector2(dimX, 0.0f);
                                break;
                            }
                    }
                    FaceCounter++;
                }
            }


            var a_mesh = new ArrayMesh(); //= Mesh as ArrayMesh;
                                          //create normals from verts
            var normals = new List<Vector3>();
            foreach (var vert in verts)
            {
                normals.Add(vert.Normalized());
            }

            //Apply the uvs and create my tris
            // mesh.vertices = verts;
            // mesh.uv = uvs;
            FaceCounter = 0;
            int curFace = 0;
            int[] indices;// = new int[6];

            for (int i = 0; i < 6; i++)
            {
                if (curFace == vTOP)
                {
                    indices = new int[3];
                }
                else
                {
                    indices = new int[6];
                }

                if (t.VisibleFaces[i] == true)
                {
                    if (i == vTOP)
                    {
                        switch (t.tileType)
                        {
                            case TILE_DIAG_NE:
                                indices[0] = 1 + (4 * FaceCounter);
                                indices[1] = 2 + (4 * FaceCounter);
                                indices[2] = 3 + (4 * FaceCounter);
                                break;
                            case TILE_DIAG_SE:
                                indices[0] = 0 + (4 * FaceCounter);
                                indices[1] = 2 + (4 * FaceCounter);
                                indices[2] = 3 + (4 * FaceCounter);
                                break;
                            case TILE_DIAG_SW:
                                indices[0] = 0 + (4 * FaceCounter);
                                indices[1] = 1 + (4 * FaceCounter);
                                indices[2] = 3 + (4 * FaceCounter);
                                break;
                            case TILE_DIAG_NW:
                            default:
                                indices[0] = 0 + (4 * FaceCounter);
                                indices[1] = 1 + (4 * FaceCounter);
                                indices[2] = 2 + (4 * FaceCounter);
                                break;
                        }

                        //tris[3]=0+(4*FaceCounter);
                        //tris[4]=2+(4*FaceCounter);
                        //tris[5]=3+(4*FaceCounter);
                        //mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                    }
                    else
                    {
                        indices[0] = 0 + (4 * FaceCounter);
                        indices[1] = 1 + (4 * FaceCounter);
                        indices[2] = 2 + (4 * FaceCounter);
                        indices[3] = 0 + (4 * FaceCounter);
                        indices[4] = 2 + (4 * FaceCounter);
                        indices[5] = 3 + (4 * FaceCounter);
                        // mesh.SetTriangles(indices, FaceCounter);
                        AddSurfaceToMesh(verts, uvs, MatsToUse, FaceCounter, a_mesh, normals, indices);
                    }
                    FaceCounter++;
                    curFace++;
                }
            }

            return CreateMeshInstance(parent, x, y, TileName, a_mesh);

            // mr.materials = MatsToUse;//mats;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // mf.mesh = mesh;
            // if (EnableCollision)
            // {
            //     MeshCollider mc = Tile.AddComponent<MeshCollider>();
            //     mc.sharedMesh = null;
            //     mc.sharedMesh = mesh;
            // }
            // return Tile;
        }


        /// <summary>
        /// Adds a surface built from the various uv, vertices and materials arrays to a mesh
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="uvs"></param>
        /// <param name="MatsToUse"></param>
        /// <param name="FaceCounter"></param>
        /// <param name="a_mesh"></param>
        /// <param name="normals"></param>
        /// <param name="indices"></param>
        private static void AddSurfaceToMesh(Vector3[] verts, Vector2[] uvs, int[] MatsToUse, int FaceCounter, ArrayMesh a_mesh, List<Vector3> normals, int[] indices, int faceCounterAdj = 0)
        {
            var surfaceArray = new Godot.Collections.Array();
            surfaceArray.Resize((int)Mesh.ArrayType.Max);

            surfaceArray[(int)Mesh.ArrayType.Vertex] = verts; //.ToArray();
            surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs; //.ToArray();
            surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
            surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

            //Add the new surface to the mesh
            a_mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

            //bool useCustomShader=true;
           // if (useCustomShader)
           // {
            a_mesh.SurfaceSetMaterial(FaceCounter + faceCounterAdj, mapTextures.GetMaterial(MatsToUse[FaceCounter])); //  surfacematerial.Get(MatsToUse[FaceCounter]));
           // }
            // else
            // { //standard material shader. this works but does not cycle the textures.
            //     var material = new StandardMaterial3D(); // or shader 
            //     material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            //     material.AlbedoTexture = MatsToUse[FaceCounter];  //textureForMesh; // shader parameter, etc.
            //     material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;            
            //     a_mesh.SurfaceSetMaterial(FaceCounter + faceCounterAdj, material);
            // }

        }


        static void CalcUVForSlopedCuboid(int Top, int Bottom, out float uv0, out float uv1)
        {
            float PolySize = Top - Bottom;
            uv0 = (float)(Bottom * 0.125f);
            uv1 = -(PolySize / 8.0f) + (uv0);
        }         



    } //end class

} //end namespace