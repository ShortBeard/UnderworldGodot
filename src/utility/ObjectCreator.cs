using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Underworld
{
    /// <summary>
    /// Class for creating instances of objects and calling updates
    /// </summary>
    public class ObjectCreator : UWClass
    {
        //List of active NPCs
        public static List<npc> npcs;
        public static bool printlabels = true;

        public enum ObjectListType
        {
            StaticList=0,
            MobileList=1
        };

        /// <summary>
        /// Allocates data for a new object
        /// </summary>
        /// <param name="item_id"></param>
        /// <returns></returns>
        public static int PrepareNewObject(int item_id, ObjectListType WhichList = ObjectListType.StaticList)
        {
            int slot = GetAvailableObjectSlot(WhichList);
            if (slot!=0)
            {
                var obj = TileMap.current_tilemap.LevelObjects[slot];
                obj.quality = 0x28;
                obj.item_id = item_id;
                obj.zpos = 0;
                obj.doordir = 0;
                obj.invis =0;
                obj.enchantment =0;
                obj.flags = 0; //DBLCHK (0x11)
                obj.xpos = 3;
                obj.ypos = 3;
                obj.heading = 0;
                obj.next = 0;
                obj.owner = 0;
                //allocate static props
                var stackable = commonObjDat.stackable(item_id);

                switch (stackable)
                {
                    case 0:
                    case 2:
                        obj.link = 1;
                        obj.is_quant = 1;
                        break;
                    case 1:
                    case 3:
                    default:
                        obj.is_quant=0;
                        obj.link = 0;
                        break;
                }

                if (obj.majorclass == 1) //NPC
                {
                    //TODO INITIALISE CRITTER
                }
            }
            return slot;
        }


        public static int GetAvailableObjectSlot(ObjectListType WhichList = ObjectListType.StaticList)
        {
            //look up object free list
            switch (WhichList)
            {
                case ObjectListType.StaticList:
                    //Move PTR down, get object at that point.
                    TileMap.current_tilemap.StaticFreeListPtr--;
                    Debug.Print ($"Allocating {TileMap.current_tilemap.StaticFreeListObject}");
                    return TileMap.current_tilemap.StaticFreeListObject;
                case ObjectListType.MobileList:
                    return 0; //TODO
            }
            return 0;
        }

        /// <summary>
        /// Process object list
        /// </summary>
        /// <param name="worldparent"></param>
        /// <param name="objects"></param>
        /// <param name="grObjects"></param>
        /// <param name="a_tilemap"></param>
        public static void GenerateObjects(Node3D worldparent, uwObject[] objects, GRLoader grObjects, TileMap a_tilemap)
        {
            npcs = new();
            foreach (var obj in objects)
            {
                if (obj.item_id <= 463)
                {
                    RenderObject(worldparent, grObjects, obj, a_tilemap);
                }
            }
        }


        /// <summary>
        /// Adds an object instance to the tilemap
        /// </summary>
        /// <param name="worldparent"></param>
        /// <param name="grObjects"></param>
        /// <param name="obj"></param>
        /// <param name="a_tilemap"></param>
        public static void RenderObject(Node3D worldparent, GRLoader grObjects, uwObject obj, TileMap a_tilemap)
        {
            bool unimplemented = true;
            var name = $"{obj.index}_{GameStrings.GetObjectNounUW(obj.item_id)}";
            var newparent = new Node3D();
            newparent.Name = name;
            newparent.Position = obj.GetCoordinate(obj.tileX, obj.tileY);
            worldparent.AddChild(newparent);

            switch (obj.majorclass)
            {
                case 1://npcs
                    {
                        if (obj.item_id<124)
                        {
                        npcs.Add(npc.CreateInstance(newparent, obj, name));
                        unimplemented = false;
                        }
                        break;
                    }
                case 3: // misc objects
                {
                    unimplemented = MajorClass3(obj, newparent, grObjects, name);
                    break;
                }
                case 5: //doors, 3d models, buttons/switches
                    {
                        unimplemented = MajorClass5(obj, newparent, a_tilemap, name);
                        break;
                    }

                case 0://Weapons
                case 2://misc items incl containers, food, and lights.                
                case 4://keys, usables and readables
                case 6://Traps and Triggers
                    break;
                case 7://Animos
                    {
                        unimplemented = MajorClass7(obj, newparent, a_tilemap, name);
                        break;
                    }
                default:
                    unimplemented = true; break;

            }           
            if (unimplemented)
            {
                //just render a sprite.
                CreateSpriteInstance(grObjects, obj, newparent, $"{name}");
                if (printlabels)
                {
                    Label3D obj_lbl = new();
                    obj_lbl.Text = $"{name}";
                    obj_lbl.Font = uimanager.instance.Font4X5P;
                    obj_lbl.FontSize=16;
                    obj_lbl.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
                    obj_lbl.Position = new Vector3(0f,0.4f,0f);
                    newparent.AddChild(obj_lbl);
                }
            }
        }


        /// <summary>
        /// Runestones and some misc objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <param name="grObjects"></param>
        /// <returns></returns>
        private static bool MajorClass3(uwObject obj, Node3D parent, GRLoader grObjects, string name)
        {
            if ((obj.minorclass == 2) && (obj.classindex == 0))
            {
                runestone.CreateInstance(parent, obj, grObjects, name);
                return false;
            }
            if (((obj.minorclass == 2) && (obj.classindex >= 8))
                || (obj.minorclass == 3)
                || ((obj.minorclass == 2) && (obj.classindex == 0)))
            {//runestones
                runestone.CreateInstance(parent, obj, grObjects, name);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Doors, 3d models, buttons/switches
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="unimplemented"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static bool MajorClass5(uwObject obj, Node3D parent, TileMap a_tilemap, string name)
        {
            if (obj.tileX >= 65) { return true; }//don't render offmap models.
            switch (obj.minorclass)
            {
                case 0: //doors
                    {
                        door.CreateInstance(parent, obj, a_tilemap, name);
                        doorway.CreateInstance(parent, obj, a_tilemap, name);
                        return false;
                    }
                case 1: //3D Models
                    {
                        if (obj.classindex == 0)
                        {//bench
                            bench.CreateInstance(parent, obj, name);
                            return false;
                        }
                        if ((obj.classindex>=3) && (obj.classindex<=6))
                        {//boulders
                            boulder.CreateInstance(parent,obj, name);
                            return false;
                        }
                        if (obj.classindex == 7)
                        {//shrine
                            shrine.CreateInstance(parent, obj, name);
                            return false;
                        }
                        if (obj.classindex == 8)
                        {
                            table.CreateInstance(parent, obj, name);
                            return false;
                        }   
                        if (obj.classindex == 9)
                        {
                            beam.CreateInstance(parent, obj, name);
                            return false;
                        }   
                        if (obj.classindex == 0xA)
                        {
                            moongate.CreateInstance(parent, obj, name);
                            return false;
                        }  
                        if (obj.classindex == 0xB)
                        {
                            barrel.CreateInstance(parent, obj, name);
                            return false;
                        } 
                        if (obj.classindex == 0xC)
                        {
                            chair.CreateInstance(parent, obj, name);
                            return false;
                        }  
                        if (obj.classindex == 0xD)
                        {
                            chest.CreateInstance(parent, obj, name);
                            return false;
                        }  
                        if (obj.classindex == 0xE)
                        {
                            nightstand.CreateInstance(parent, obj, name);
                            return false;
                        }    
                        if (obj.classindex == 0xF)
                        {
                            lotus.CreateInstance(parent, obj, name);
                            return false;
                        }   
                        break;
                    }
                case 2: //3D models
                    {
                        if (obj.classindex == 0)
                        {//pillar 352
                            pillar.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if ((obj.classindex == 1) || (obj.classindex==2))
                        {//353 and 354, rotary switches
                            buttonrotary.CreateInstance(parent, obj, name);
                            return false;
                        }
                        if ((_RES == GAME_UW2) && (obj.classindex == 3))
                        {  //or item id 163
                            painting.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if (obj.classindex == 4)
                        {//bridge 356
                            bridge.CreateInstance(parent, obj, name, a_tilemap);
                            return false; 
                        }
                        if (obj.classindex == 5)
                        {//gravestone
                            gravestone.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if (obj.classindex == 6)
                        {//some_writing 358
                            writing.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if ((_RES == GAME_UW2) && (obj.classindex == 7))
                        {  //or item id 359
                            bed.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if ((_RES == GAME_UW2) && (obj.classindex == 8))
                        {  //or item id 360
                            largeblackrockgem.CreateInstance(parent, obj, name);
                            return false;
                        }
                        if ((_RES == GAME_UW2) && (obj.classindex == 9))
                        {  //or item id 361
                            shelf.CreateInstance(parent, obj, name);
                            return false; 
                        }
                        if ((obj.classindex == 0xE) || (obj.classindex == 0xF))
                        {//tmaps
                            tmap.CreateInstance(parent, obj, a_tilemap, name);
                            return false;
                        }
                        break;
                    }
                case 3:
                    {   //buttons
                        button.CreateInstance(parent, obj, name);
                        return false;
                    }
            }

            return true;
        }


         private static bool MajorClass7(uwObject obj, Node3D parent, TileMap a_tilemap, string name)
         {//animos and the moving door
            //class 7 has only a single minor class. jump straight to the class index
            switch (obj.classindex)
            {
                case 0xF://moving door special case
                        door.CreateInstance(parent, obj, a_tilemap, name);
                        doorway.CreateInstance(parent, obj, a_tilemap, name);
                        return false;                    
                default:
                    animo.CreateInstance(parent, obj, name);
                    return false;
            }
         }

        public static void CreateSpriteInstance(GRLoader grObjects, uwObject obj, Node3D parent, string name)
        {
            if (obj.invis ==0)
            {
                CreateSprite(grObjects, obj.item_id, parent, name);
            }           
        }

        public static void CreateSprite(GRLoader gr, int spriteNo, Node3D parent, string name, bool EnableCollision = true)
        {
            // var a_sprite = new Sprite3D();
            // a_sprite.Name = name;
            // var img = gr.LoadImageAt(spriteNo);
            // var NewSize = new Vector2(
            //             ArtLoader.SpriteScale * img.GetWidth(),
            //             ArtLoader.SpriteScale * img.GetHeight()
            //             );
            // a_sprite.Texture = gr.LoadImageAt(spriteNo);
            // a_sprite.MaterialOverride =  gr.GetMaterial(spriteNo);
            // parent.AddChild(a_sprite);
            // a_sprite.Position = new Vector3(0, NewSize.Y / 2, 0);            
        
            var a_sprite = new MeshInstance3D(); //new Sprite3D();
            a_sprite.Name = name;
            a_sprite.Mesh = new QuadMesh();
            Vector2 NewSize;
            var img = gr.LoadImageAt(spriteNo);
            if (img != null)
            {
                a_sprite.Mesh.SurfaceSetMaterial(0, gr.GetMaterial(spriteNo));
                NewSize = new Vector2(
                        ArtLoader.SpriteScale * img.GetWidth(),
                        ArtLoader.SpriteScale * img.GetHeight()
                        );
                a_sprite.Mesh.Set("size", NewSize);
                parent.AddChild(a_sprite);
                a_sprite.Position = new Vector3(0, NewSize.Y / 2, 0);
                if (EnableCollision)
                {
                    a_sprite.CreateConvexCollision();
                }                
            }
        }
    }

} //end namesace