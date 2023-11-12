using System.Diagnostics;
using Godot;
namespace Underworld
{
    public class writing:model3D
    {
        public static writing CreateInstance(Node3D parent, uwObject obj, string name)
        {
            var b = new writing(obj);
            var modelNode = b.Generate3DModel(parent, name);
            SetModelRotation(parent, b);            
            //DisplayModelPoints(b,modelNode);
            return b;
        }

        public writing(uwObject _uwobject)
        {            
            uwobject = _uwobject;   
            uwobject.instance = this;         
        }


        public static bool LookAt(uwObject obj)
        {
            if (obj.is_quant == 1)
                {
                messageScroll.AddString(GameStrings.GetString(8, obj.link-0x200));
                }
            return true;
        }

        public override Vector3[] ModelVertices()
        {
            var v = new Vector3[4];
            v[0] = new Vector3(-0.0625f, 0f, 0.0625f);
            v[1] = new Vector3(0.1875f, 0f, 0.0625f);
            v[2] = new Vector3(0.1875f, 0.25f, 0.0625f);
            v[3] = new Vector3(-0.0625f, 0.25f, 0.0625f);
            return v;
        }

        public override int[] ModelTriangles(int meshNo)
        {
           var tris = new int[6];
           tris[0] = 0;
           tris[1] = 3;
           tris[2] = 2;
           tris[3] = 2;
           tris[4] = 1;
           tris[5] = 0;
           return tris;
        }

        public override Vector2[] ModelUVs(Vector3[] verts)
        {
            var uv = new Vector2[4];
            uv[0] = new Vector2(0,1);
            uv[1] = new Vector2(1,1);
            uv[2] = new Vector2(1,0);
            uv[3] = new Vector2(0,0); 
            return uv;
        }

        public override ShaderMaterial GetMaterial(int textureno, int surface)
        {
            //(20 + (flags & 0x07)           
            return GetTmObj.GetMaterial(20 + (uwobject.flags & 0x07));
        }

    }//end class
}//end namespace