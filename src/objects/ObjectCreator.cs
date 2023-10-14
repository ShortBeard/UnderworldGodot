using System.Collections.Generic;
using System.Runtime.InteropServices;
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


        public static void GenerateObjects(Node3D worldparent, List<uwObject> objects, GRLoader grObjects, TileMap a_tilemap)
        {
            npcs = new();
            foreach (var obj in objects)
            {
                RenderObject(worldparent, grObjects, obj, a_tilemap);
            }
        }

        public static void RenderObject(Node3D worldparent, GRLoader grObjects, uwObject obj, TileMap a_tilemap)
        {
            bool unimplemented = true;

            var newnode = new Node3D();
            newnode.Name = StringLoader.GetObjectNounUW(obj.item_id) + "_" + obj.index.ToString();
            newnode.Position = obj.GetCoordinate(obj.tileX, obj.tileY);
            worldparent.AddChild(newnode);

            switch (obj.majorclass)
            {
                case 1://npcs
                    {
                        npcs.Add(npc.CreateInstance(newnode, obj));
                        unimplemented = false;
                        break;
                    }
                case 5: //doors, 3d models, buttons/switches
                    {
                        unimplemented = MajorClass5(obj, newnode);
                        Label3D obj_lbl = new();
						obj_lbl.Text = $"{StringLoader.GetObjectNounUW(obj.item_id)} {obj.index} {obj.heading} -> {obj.heading_r}";
						obj_lbl.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
						//obj_lbl.Font = font;
						newnode.AddChild(obj_lbl);
                        break;
                    }

                case 0://Weapons
                case 2://misc items incl containers, food, and lights.
                case 3://clutter, runestones, potions
                case 4://keys, usables and readables
                case 6://Traps and Triggers
                case 7://Animos
                default:
                    unimplemented = true; break;

            }

            if (unimplemented)
            {
                //just render a sprite.
                RenderSprite(grObjects, obj, newnode);
            }


            // Label3D obj_lbl = new();
			// 			obj_lbl.Text = $"{StringLoader.GetObjectNounUW(obj.item_id)} {obj.index} {obj.heading} -> {obj.heading_r}";
			// 			obj_lbl.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
			// 			//obj_lbl.Font = font;
			// 			newnode.AddChild(obj_lbl);
        }

        /// <summary>
        /// Doors, 3d models, buttons/switches
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="unimplemented"></param>
        /// <param name="newnode"></param>
        /// <returns></returns>
        private static bool MajorClass5(uwObject obj, Node3D newnode)
        {
            switch (obj.minorclass)
            {
                case 2: //3D models
                    {
                        if (obj.classindex == 0)
                        {//pillar 352
                            pillar.CreateInstance(newnode, obj, newnode.Position);
                            return false; /// unimplemented = false;
                        }
                        if ((_RES == GAME_UW2) && (obj.classindex == 7))
                        {  //or item id 359
                            bed.CreateInstance(newnode, obj);
                            return false; //unimplemented = false;
                        }
                        break;
                    }
            }

            return true;
        }

        private static void RenderSprite(GRLoader grObjects, uwObject obj, Node3D newnode)
        {
            var a_sprite = new MeshInstance3D(); //new Sprite3D();
            a_sprite.Mesh = new QuadMesh();
            Vector2 NewSize;
            a_sprite.Mesh.SurfaceSetMaterial(0, grObjects.GetMaterial(obj.item_id));
            NewSize = new Vector2(
                    ArtLoader.SpriteScale * grObjects.ImageCache[obj.item_id].GetWidth(),
                    ArtLoader.SpriteScale * grObjects.ImageCache[obj.item_id].GetHeight()
                    );
            a_sprite.Mesh.Set("size", NewSize);
            newnode.AddChild(a_sprite);
            a_sprite.Position = new Vector3(0, NewSize.Y / 2, 0);
        }
    }

} //end namesace