using Godot;
using System;
using System.Diagnostics;
namespace Underworld
{

    public class animo : objectInstance
    {

        /// <summary>
        /// global shader for npcs.
        /// </summary>
        public static Shader textureshader;

        public static GRLoader grAnimo;

        /// <summary>
        /// Mesh this sprite is drawn on
        /// </summary>
        public MeshInstance3D sprite;

        /// <summary>
        /// The material for rendering this unique npc
        /// </summary>
        public ShaderMaterial material;


        public int startFrame
        {
            get
            {
                return animationObjectDat.startFrame(uwobject.item_id);
            }
        }

        public int endFrame
        {
            get
            {
                return animationObjectDat.endFrame(uwobject.item_id);
            }
        }

        public static animo CreateInstance(Node3D parent, uwObject obj, string name)
        {
            var a = new animo(obj);
            //a.ApplyAnimoSprite();
            var a_sprite = new MeshInstance3D(); //new Sprite3D();
            a_sprite.Name = name;
            a_sprite.Mesh = new QuadMesh();
            a_sprite.Mesh.SurfaceSetMaterial(0, grAnimo.GetMaterial(obj.owner));

            var img = grAnimo.LoadImageAt(obj.owner);
            var NewSize = new Vector2(
                ArtLoader.SpriteScale * img.GetWidth(),
                ArtLoader.SpriteScale * img.GetHeight()
            );
            a_sprite.Mesh.Set("size", NewSize);
            a.sprite = a_sprite;
            parent.AddChild(a_sprite);
            a_sprite.Position = new Vector3(0, NewSize.Y / 2 + 0.06f, 0);
            a_sprite.CreateConvexCollision();
            return a;
        }

        static animo()
        {
            textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uwsprite.gdshader");
            grAnimo = new GRLoader(GRLoader.ANIMO_GR, GRLoader.GRShaderMode.BillboardSpriteShader);
            grAnimo.UseRedChannel = true;
        }

        public animo(uwObject _uwobject)
        {
            uwobject = _uwobject;
            uwobject.instance = this;
        }

        public void ApplyAnimoSprite()
        {
            // if (material == null)
            // {//create the initial material
            //     var newmaterial = new ShaderMaterial();
            //     newmaterial.Shader = textureshader;
            //     newmaterial.SetShaderParameter("albedo", new Color(1, 1, 1, 1));
            //     newmaterial.SetShaderParameter("uv1_scale", new Vector3(1, 1, 1));
            //     newmaterial.SetShaderParameter("uv2_scale", new Vector3(1, 1, 1));
            //     newmaterial.SetShaderParameter("UseAlpha", true);
            //     material = newmaterial;
            // }
            sprite.Mesh.SurfaceSetMaterial(0, grAnimo.GetMaterial(uwobject.owner));
        }

        public static void AdvanceAnimo(animo obj)
        {
            if (obj != null)
            {
                obj.uwobject.owner++;
                RefreshAnimo(obj);
            }
        }


        public static void ResetAnimo(animo obj)
        {
            if (obj != null)
            {
                obj.uwobject.owner = (short)obj.startFrame;
                RefreshAnimo(obj);
            }
        }

        public static void RefreshAnimo(animo obj)
        {
            if (obj != null)
            {
                obj.ApplyAnimoSprite();
            }
        }
    }
}//end namespace