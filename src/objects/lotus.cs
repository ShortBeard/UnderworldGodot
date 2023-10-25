using System;
using Godot;
namespace Underworld
{
    public class lotus : model3D
    {
        public static lotus CreateInstance(Node3D parent, uwObject obj, string name)
        {
            var l = new lotus(obj);
            var modelNode = l.Generate3DModel(parent, name);
            // modelNode.Rotate(Vector3.Up, (float)Math.PI);
            SetModelRotation(parent, l);
            DisplayModelPoints(l, parent, 200);
            return l;
        }

        public lotus(uwObject _uwobject)
        {
            uwobject = _uwobject;
        }


        public override Vector3[] ModelVertices()
        {
            var v = new Vector3[194];
            v[0] = new Vector3(-0.34375f, 0.1640625f, 0.3671875f);
            v[1] = new Vector3(-0.24609375f, 0.37109375f, -0.00390625f);
            v[2] = new Vector3(0.3359375f, 0.26953125f, 0.1796875f);
            v[3] = new Vector3(0.26953125f, 0.34375f, -0.2734375f);
            v[4] = new Vector3(0.31640625f, 0.2890625f, -0.30078125f);
            v[5] = new Vector3(-0.34375f, 0.16015625f, 0.33984375f);
            v[6] = new Vector3(-0.34375f, 0.14453125f, 0.3203125f);
            v[7] = new Vector3(-0.34375f, 0.12109375f, 0.3046875f);
            v[8] = new Vector3(-0.34375f, 0.09765625f, 0.296875f);
            v[9] = new Vector3(-0.34375f, 0.0703125f, 0.3046875f);
            v[10] = new Vector3(-0.34375f, 0.046875f, 0.3203125f);
            v[11] = new Vector3(-0.34375f, 0.03515625f, 0.33984375f);
            v[12] = new Vector3(-0.34375f, 0.02734375f, 0.3671875f);
            v[13] = new Vector3(-0.34375f, 0.03515625f, 0.390625f);
            v[14] = new Vector3(-0.34375f, 0.046875f, 0.4140625f);
            v[15] = new Vector3(-0.34375f, 0.0703125f, 0.4296875f);
            v[16] = new Vector3(-0.34375f, 0.09765625f, 0.4375f);
            v[17] = new Vector3(-0.34375f, 0.12109375f, 0.4296875f);
            v[18] = new Vector3(-0.34375f, 0.14453125f, 0.4140625f);
            v[19] = new Vector3(-0.34375f, 0.16015625f, 0.390625f);
            v[20] = new Vector3(-0.34375f, 0.1875f, 0.3671875f);
            v[21] = new Vector3(-0.34375f, 0.1796875f, 0.33203125f);
            v[22] = new Vector3(-0.34375f, 0.16015625f, 0.30078125f);
            v[23] = new Vector3(-0.34375f, 0.12890625f, 0.28125f);
            v[24] = new Vector3(-0.34375f, 0.09375f, 0.2734375f);
            v[25] = new Vector3(-0.34375f, 0.05859375f, 0.28125f);
            v[26] = new Vector3(-0.34375f, 0.02734375f, 0.30078125f);
            v[27] = new Vector3(-0.34375f, 0.0078125f, 0.33203125f);
            v[28] = new Vector3(-0.34375f, 0f, 0.3671875f);
            v[29] = new Vector3(-0.34375f, 0.0078125f, 0.40234375f);
            v[30] = new Vector3(-0.34375f, 0.02734375f, 0.43359375f);
            v[31] = new Vector3(-0.34375f, 0.05859375f, 0.453125f);
            v[32] = new Vector3(-0.34375f, 0.09375f, 0.4609375f);
            v[33] = new Vector3(-0.34375f, 0.12890625f, 0.453125f);
            v[34] = new Vector3(-0.34375f, 0.16015625f, 0.43359375f);
            v[35] = new Vector3(-0.34375f, 0.1796875f, 0.40234375f);
            v[36] = new Vector3(-0.34375f, 0.19140625f, -0.40234375f);
            v[37] = new Vector3(-0.34375f, 0.18359375f, -0.4375f);
            v[38] = new Vector3(-0.34375f, 0.1640625f, -0.46875f);
            v[39] = new Vector3(-0.34375f, 0.1328125f, -0.48828125f);
            v[40] = new Vector3(-0.34375f, 0.09765625f, -0.49609375f);
            v[41] = new Vector3(-0.34375f, 0.0625f, -0.48828125f);
            v[42] = new Vector3(-0.34375f, 0.03125f, -0.46875f);
            v[43] = new Vector3(-0.34375f, 0.01171875f, -0.4375f);
            v[44] = new Vector3(-0.34375f, 0.00390625f, -0.40234375f);
            v[45] = new Vector3(-0.34375f, 0.01171875f, -0.3671875f);
            v[46] = new Vector3(-0.34375f, 0.03125f, -0.3359375f);
            v[47] = new Vector3(-0.34375f, 0.0625f, -0.31640625f);
            v[48] = new Vector3(-0.34375f, 0.09765625f, -0.30859375f);
            v[49] = new Vector3(-0.34375f, 0.1328125f, -0.31640625f);
            v[50] = new Vector3(-0.34375f, 0.1640625f, -0.3359375f);
            v[51] = new Vector3(-0.34375f, 0.18359375f, -0.3671875f);
            v[52] = new Vector3(-0.34375f, 0.16796875f, -0.40234375f);
            v[53] = new Vector3(-0.34375f, 0.1640625f, -0.4296875f);
            v[54] = new Vector3(-0.34375f, 0.1484375f, -0.44921875f);
            v[55] = new Vector3(-0.34375f, 0.125f, -0.46484375f);
            v[56] = new Vector3(-0.34375f, 0.1015625f, -0.47265625f);
            v[57] = new Vector3(-0.34375f, 0.07421875f, -0.46484375f);
            v[58] = new Vector3(-0.34375f, 0.05078125f, -0.44921875f);
            v[59] = new Vector3(-0.34375f, 0.0390625f, -0.4296875f);
            v[60] = new Vector3(-0.34375f, 0.03125f, -0.40234375f);
            v[61] = new Vector3(-0.34375f, 0.0390625f, -0.37890625f);
            v[62] = new Vector3(-0.34375f, 0.05078125f, -0.35546875f);
            v[63] = new Vector3(-0.34375f, 0.07421875f, -0.33984375f);
            v[64] = new Vector3(-0.34375f, 0.1015625f, -0.33203125f);
            v[65] = new Vector3(-0.34375f, 0.125f, -0.33984375f);
            v[66] = new Vector3(-0.34375f, 0.1484375f, -0.35546875f);
            v[67] = new Vector3(-0.34375f, 0.1640625f, -0.37890625f);
            v[68] = new Vector3(0.34375f, 0.16796875f, -0.40234375f);
            v[69] = new Vector3(0.34375f, 0.1640625f, -0.4296875f);
            v[70] = new Vector3(0.34375f, 0.1484375f, -0.44921875f);
            v[71] = new Vector3(0.34375f, 0.125f, -0.46484375f);
            v[72] = new Vector3(0.34375f, 0.1015625f, -0.47265625f);
            v[73] = new Vector3(0.34375f, 0.07421875f, -0.46484375f);
            v[74] = new Vector3(0.34375f, 0.05078125f, -0.44921875f);
            v[75] = new Vector3(0.34375f, 0.0390625f, -0.4296875f);
            v[76] = new Vector3(0.34375f, 0.03125f, -0.40234375f);
            v[77] = new Vector3(0.34375f, 0.0390625f, -0.37890625f);
            v[78] = new Vector3(0.34375f, 0.05078125f, -0.35546875f);
            v[79] = new Vector3(0.34375f, 0.07421875f, -0.33984375f);
            v[80] = new Vector3(0.34375f, 0.1015625f, -0.33203125f);
            v[81] = new Vector3(0.34375f, 0.125f, -0.33984375f);
            v[82] = new Vector3(0.34375f, 0.1484375f, -0.35546875f);
            v[83] = new Vector3(0.34375f, 0.1640625f, -0.37890625f);
            v[84] = new Vector3(0.34375f, 0.1875f, 0.3671875f);
            v[85] = new Vector3(0.34375f, 0.1796875f, 0.33203125f);
            v[86] = new Vector3(0.34375f, 0.16015625f, 0.30078125f);
            v[87] = new Vector3(0.34375f, 0.12890625f, 0.28125f);
            v[88] = new Vector3(0.34375f, 0.09375f, 0.2734375f);
            v[89] = new Vector3(0.34375f, 0.05859375f, 0.28125f);
            v[90] = new Vector3(0.34375f, 0.02734375f, 0.30078125f);
            v[91] = new Vector3(0.34375f, 0.0078125f, 0.33203125f);
            v[92] = new Vector3(0.34375f, 0f, 0.3671875f);
            v[93] = new Vector3(0.34375f, 0.0078125f, 0.40234375f);
            v[94] = new Vector3(0.34375f, 0.02734375f, 0.43359375f);
            v[95] = new Vector3(0.34375f, 0.05859375f, 0.453125f);
            v[96] = new Vector3(0.34375f, 0.09375f, 0.4609375f);
            v[97] = new Vector3(0.34375f, 0.12890625f, 0.453125f);
            v[98] = new Vector3(0.34375f, 0.16015625f, 0.43359375f);
            v[99] = new Vector3(0.34375f, 0.1796875f, 0.40234375f);
            v[100] = new Vector3(0.34375f, 0.1640625f, 0.3671875f);
            v[101] = new Vector3(0.34375f, 0.16015625f, 0.33984375f);
            v[102] = new Vector3(0.34375f, 0.14453125f, 0.3203125f);
            v[103] = new Vector3(0.34375f, 0.12109375f, 0.3046875f);
            v[104] = new Vector3(0.34375f, 0.09765625f, 0.296875f);
            v[105] = new Vector3(0.34375f, 0.0703125f, 0.3046875f);
            v[106] = new Vector3(0.34375f, 0.046875f, 0.3203125f);
            v[107] = new Vector3(0.34375f, 0.03515625f, 0.33984375f);
            v[108] = new Vector3(0.34375f, 0.02734375f, 0.3671875f);
            v[109] = new Vector3(0.34375f, 0.03515625f, 0.390625f);
            v[110] = new Vector3(0.34375f, 0.046875f, 0.4140625f);
            v[111] = new Vector3(0.34375f, 0.0703125f, 0.4296875f);
            v[112] = new Vector3(0.34375f, 0.09765625f, 0.4375f);
            v[113] = new Vector3(0.34375f, 0.12109375f, 0.4296875f);
            v[114] = new Vector3(0.34375f, 0.14453125f, 0.4140625f);
            v[115] = new Vector3(0.34375f, 0.16015625f, 0.390625f);
            v[116] = new Vector3(0.34375f, 0.19140625f, -0.40234375f);
            v[117] = new Vector3(0.34375f, 0.18359375f, -0.4375f);
            v[118] = new Vector3(0.34375f, 0.1640625f, -0.46875f);
            v[119] = new Vector3(0.34375f, 0.1328125f, -0.48828125f);
            v[120] = new Vector3(0.34375f, 0.09765625f, -0.49609375f);
            v[121] = new Vector3(0.34375f, 0.0625f, -0.48828125f);
            v[122] = new Vector3(0.34375f, 0.03125f, -0.46875f);
            v[123] = new Vector3(0.34375f, 0.01171875f, -0.4375f);
            v[124] = new Vector3(0.34375f, 0.00390625f, -0.40234375f);
            v[125] = new Vector3(0.34375f, 0.01171875f, -0.3671875f);
            v[126] = new Vector3(0.34375f, 0.03125f, -0.3359375f);
            v[127] = new Vector3(0.34375f, 0.0625f, -0.31640625f);
            v[128] = new Vector3(0.34375f, 0.09765625f, -0.30859375f);
            v[129] = new Vector3(0.34375f, 0.1328125f, -0.31640625f);
            v[130] = new Vector3(0.34375f, 0.1640625f, -0.3359375f);
            v[131] = new Vector3(0.34375f, 0.18359375f, -0.3671875f);
            v[132] = new Vector3(-0.34375f, 0.046875f, 0.48828125f);
            v[133] = new Vector3(-0.34375f, 0.234375f, 0.484375f);
            v[134] = new Vector3(-0.34375f, 0.26171875f, 0.3125f);
            v[135] = new Vector3(-0.34375f, 0.26171875f, -0.65625f);
            v[136] = new Vector3(-0.34375f, 0.17578125f, -0.66015625f);
            v[137] = new Vector3(-0.34375f, 0.0859375f, -0.65625f);
            v[138] = new Vector3(-0.34375f, 0.046875f, -0.6484375f);
            v[139] = new Vector3(-0.34375f, 0.15625f, -0.70703125f);
            v[140] = new Vector3(-0.34375f, 0.09765625f, -0.703125f);
            v[141] = new Vector3(-0.34375f, 0.1796875f, 0.44921875f);
            v[142] = new Vector3(-0.34375f, 0.21484375f, 0.3671875f);
            v[143] = new Vector3(-0.34375f, 0.17578125f, 0.28125f);
            v[144] = new Vector3(-0.34375f, 0.046875f, 0.25f);
            v[145] = new Vector3(-0.34375f, 0.046875f, 0.64453125f);
            v[146] = new Vector3(-0.34375f, 0.1484375f, 0.70703125f);
            v[147] = new Vector3(-0.34375f, 0.046875f, -0.28125f);
            v[148] = new Vector3(-0.34375f, 0.1796875f, -0.3203125f);
            v[149] = new Vector3(-0.34375f, 0.21484375f, -0.40234375f);
            v[150] = new Vector3(-0.34375f, 0.17578125f, -0.48828125f);
            v[151] = new Vector3(-0.34375f, 0.046875f, -0.51953125f);
            v[152] = new Vector3(0.34375f, 0.17578125f, -0.66015625f);
            v[153] = new Vector3(0.34375f, 0.15625f, -0.70703125f);
            v[154] = new Vector3(0.34375f, 0.09765625f, -0.703125f);
            v[155] = new Vector3(0.34375f, 0.0859375f, -0.65625f);
            v[156] = new Vector3(0.34375f, 0.046875f, 0.48828125f);
            v[157] = new Vector3(0.34375f, 0.046875f, 0.64453125f);
            v[158] = new Vector3(0.34375f, 0.1484375f, 0.70703125f);
            v[159] = new Vector3(0.34375f, 0.234375f, 0.484375f);
            v[160] = new Vector3(0.34375f, 0.26171875f, 0.3125f);
            v[161] = new Vector3(0.34375f, 0.26171875f, -0.65625f);
            v[162] = new Vector3(0.34375f, 0.046875f, -0.6484375f);
            v[163] = new Vector3(-0.24609375f, 0.37109375f, -0.25f);
            v[164] = new Vector3(0.24609375f, 0.37109375f, -0.00390625f);
            v[165] = new Vector3(0.24609375f, 0.37109375f, -0.25f);
            v[166] = new Vector3(0.30859375f, 0.296875f, 0.1796875f);
            v[167] = new Vector3(0.25390625f, 0.359375f, -0.00390625f);
            v[168] = new Vector3(0.25390625f, 0.359375f, -0.23828125f);
            v[169] = new Vector3(0.3359375f, 0.26953125f, -0.28125f);
            v[170] = new Vector3(0.24609375f, 0.27734375f, -0.5859375f);
            v[171] = new Vector3(-0.24609375f, 0.27734375f, -0.5859375f);
            v[172] = new Vector3(0.34375f, 0.046875f, -0.28125f);
            v[173] = new Vector3(0.34375f, 0.1796875f, -0.3203125f);
            v[174] = new Vector3(0.34375f, 0.21484375f, -0.40234375f);
            v[175] = new Vector3(0.34375f, 0.17578125f, -0.48828125f);
            v[176] = new Vector3(0.34375f, 0.046875f, -0.51953125f);
            v[177] = new Vector3(0.34375f, 0.1796875f, 0.44921875f);
            v[178] = new Vector3(0.34375f, 0.21484375f, 0.3671875f);
            v[179] = new Vector3(0.34375f, 0.17578125f, 0.28125f);
            v[180] = new Vector3(0.34375f, 0.046875f, 0.25f);
            v[181] = new Vector3(-0.3359375f, 0.26953125f, -0.28125f);
            v[182] = new Vector3(-0.31640625f, 0.2890625f, -0.30078125f);
            v[183] = new Vector3(-0.26953125f, 0.34375f, -0.2734375f);
            v[184] = new Vector3(-0.25390625f, 0.359375f, -0.23828125f);
            v[185] = new Vector3(-0.25390625f, 0.359375f, -0.00390625f);
            v[186] = new Vector3(-0.30859375f, 0.296875f, 0.1796875f);
            v[187] = new Vector3(-0.3359375f, 0.26953125f, 0.1796875f);
            v[188] = new Vector3(0.16015625f, 0.26171875f, -0.65625f);
            v[189] = new Vector3(0.16015625f, 0.20703125f, -0.65625f);
            v[190] = new Vector3(0.34375f, 0.20703125f, -0.65625f);
            v[191] = new Vector3(-0.34375f, 0.20703125f, -0.65625f);
            v[192] = new Vector3(-0.16015625f, 0.20703125f, -0.65625f);
            v[193] = new Vector3(-0.16015625f, 0.26171875f, -0.65625f);

            for (int i = 0; i <= v.GetUpperBound(0); i++)
            {
                v[i] = v[i] * new Vector3(8f, 8f, 8f);
            }

            return v;
        }

        public override int ModelColour(int meshNo)
        {
            switch (meshNo)
            {
                case 0://wheel hubs
                case 1:
                case 2:
                case 3:
                    return 81;//blue
                case 4: //tyres
                case 5:
                case 6:
                case 7:
                    return 92; // pinkish
            }
            return base.ModelColour(meshNo);
        }

        public override int NoOfMeshes()
        {
            return 8;
        }
        public override int[] ModelTriangles(int meshNo)
        {
            switch (meshNo)
            {
                case 0:
                    return Wheel1Hub();
                case 1:
                    return Wheel2Hub();
                case 2:
                    return Wheel3Hub();
                case 3:
                    return Wheel4Hub();
                case 4:
                    return Tyre1();
                case 5:
                    return Tyre2();
                case 6:
                    return Tyre3();
                case 7:
                    return Tyre4();
            }
            return base.ModelTriangles(meshNo);
        }

        public int[] Wheel1Hub()
        {
            var tris = new int[42];
            tris[0] = 68;
            tris[1] = 82;
            tris[2] = 83;
            tris[3] = 68;
            tris[4] = 81;
            tris[5] = 82;
            tris[6] = 68;
            tris[7] = 80;
            tris[8] = 81;
            tris[9] = 68;
            tris[10] = 79;
            tris[11] = 80;
            tris[12] = 68;
            tris[13] = 78;
            tris[14] = 79;
            tris[15] = 68;
            tris[16] = 77;
            tris[17] = 78;
            tris[18] = 68;
            tris[19] = 76;
            tris[20] = 77;
            tris[21] = 68;
            tris[22] = 75;
            tris[23] = 76;
            tris[24] = 68;
            tris[25] = 74;
            tris[26] = 75;
            tris[27] = 68;
            tris[28] = 73;
            tris[29] = 74;
            tris[30] = 68;
            tris[31] = 72;
            tris[32] = 73;
            tris[33] = 68;
            tris[34] = 71;
            tris[35] = 72;
            tris[36] = 68;
            tris[37] = 70;
            tris[38] = 71;
            tris[39] = 68;
            tris[40] = 69;
            tris[41] = 70;


            return tris;

        }

        public int[] Wheel2Hub()
        {
            var tris = new int[42];
            tris[0] = 100;
            tris[1] = 114;
            tris[2] = 115;
            tris[3] = 100;
            tris[4] = 113;
            tris[5] = 114;
            tris[6] = 100;
            tris[7] = 112;
            tris[8] = 114;
            tris[9] = 100;
            tris[10] = 111;
            tris[11] = 112;
            tris[12] = 100;
            tris[13] = 110;
            tris[14] = 111;
            tris[15] = 100;
            tris[16] = 109;
            tris[17] = 110;
            tris[18] = 100;
            tris[19] = 108;
            tris[20] = 109;
            tris[21] = 100;
            tris[22] = 107;
            tris[23] = 108;
            tris[24] = 100;
            tris[25] = 106;
            tris[26] = 107;
            tris[27] = 100;
            tris[28] = 105;
            tris[29] = 106;
            tris[30] = 100;
            tris[31] = 104;
            tris[32] = 105;
            tris[33] = 100;
            tris[34] = 103;
            tris[35] = 104;
            tris[36] = 100;
            tris[37] = 102;
            tris[38] = 103;
            tris[39] = 100;
            tris[40] = 101;
            tris[41] = 102;

            return tris;
        }

        public int[] Wheel3Hub()
        {
            var tris = new int[42];
            tris[0] = 52;
            tris[1] = 54;
            tris[2] = 53;
            tris[3] = 52;
            tris[4] = 55;
            tris[5] = 54;
            tris[6] = 52;
            tris[7] = 56;
            tris[8] = 55;
            tris[9] = 52;
            tris[10] = 57;
            tris[11] = 56;
            tris[12] = 52;
            tris[13] = 58;
            tris[14] = 57;
            tris[15] = 52;
            tris[16] = 59;
            tris[17] = 58;
            tris[18] = 52;
            tris[19] = 60;
            tris[20] = 59;
            tris[21] = 52;
            tris[22] = 61;
            tris[23] = 60;
            tris[24] = 52;
            tris[25] = 62;
            tris[26] = 61;
            tris[27] = 52;
            tris[28] = 63;
            tris[29] = 62;
            tris[30] = 52;
            tris[31] = 64;
            tris[32] = 63;
            tris[33] = 52;
            tris[34] = 65;
            tris[35] = 64;
            tris[36] = 52;
            tris[37] = 66;
            tris[38] = 65;
            tris[39] = 52;
            tris[40] = 67;
            tris[41] = 66;

            return tris;
        }

        public int[] Wheel4Hub()
        {
            var tris = new int[42];
            tris[0] = 0;
            tris[1] = 6;
            tris[2] = 5;
            tris[3] = 0;
            tris[4] = 7;
            tris[5] = 6;
            tris[6] = 0;
            tris[7] = 8;
            tris[8] = 7;
            tris[9] = 0;
            tris[10] = 9;
            tris[11] = 8;
            tris[12] = 0;
            tris[13] = 10;
            tris[14] = 9;
            tris[15] = 0;
            tris[16] = 11;
            tris[17] = 10;
            tris[18] = 0;
            tris[19] = 12;
            tris[20] = 11;
            tris[21] = 0;
            tris[22] = 13;
            tris[23] = 12;
            tris[24] = 0;
            tris[25] = 14;
            tris[26] = 13;
            tris[27] = 0;
            tris[28] = 15;
            tris[29] = 14;
            tris[30] = 0;
            tris[31] = 17;
            tris[32] = 16;
            tris[33] = 0;
            tris[34] = 16;
            tris[35] = 15;
            tris[36] = 0;
            tris[37] = 18;
            tris[38] = 17;
            tris[39] = 0;
            tris[40] = 19;
            tris[41] = 18;

            return tris;
        }

        public int[] Tyre1()
        {
            var tris = new int[96];
            tris[0] = 68;
            tris[1] = 116;
            tris[2] = 117;
            tris[3] = 117;
            tris[4] = 69;
            tris[5] = 68;

            tris[6] = 69;
            tris[7] = 117;
            tris[8] = 118;
            tris[9] = 118;
            tris[10] = 70;
            tris[11] = 69;

            tris[12] = 70;
            tris[13] = 118;
            tris[14] = 119;
            tris[15] = 119;
            tris[16] = 71;
            tris[17] = 70;

            tris[18] = 71;
            tris[19] = 119;
            tris[20] = 120;
            tris[21] = 120;
            tris[22] = 72;
            tris[23] = 71;

            tris[24] = 72;
            tris[25] = 120;
            tris[26] = 121;
            tris[27] = 121;
            tris[28] = 73;
            tris[29] = 72;

            tris[30] = 73;
            tris[31] = 121;
            tris[32] = 122;
            tris[33] = 122;
            tris[34] = 74;
            tris[35] = 73;

            tris[36] = 74;
            tris[37] = 122;
            tris[38] = 123;
            tris[39] = 123;
            tris[40] = 75;
            tris[41] = 74;

            tris[42] = 75;
            tris[43] = 123;
            tris[44] = 124;
            tris[45] = 124;
            tris[46] = 76;
            tris[47] = 75;

            tris[48] = 76;
            tris[49] = 124;
            tris[50] = 125;
            tris[51] = 125;
            tris[52] = 77;
            tris[53] = 76;

            tris[54] = 77;
            tris[55] = 125;
            tris[56] = 126;
            tris[57] = 126;
            tris[58] = 78;
            tris[59] = 77;

            tris[60] = 78;
            tris[61] = 126;
            tris[62] = 127;
            tris[63] = 127;
            tris[64] = 79;
            tris[65] = 78;

            tris[66] = 79;
            tris[67] = 127;
            tris[68] = 128;
            tris[69] = 128;
            tris[70] = 80;
            tris[71] = 79;

            tris[72] = 80;
            tris[73] = 128;
            tris[74] = 129;
            tris[75] = 129;
            tris[76] = 81;
            tris[77] = 80;

            tris[78] = 81;
            tris[79] = 129;
            tris[80] = 130;
            tris[81] = 130;
            tris[82] = 82;
            tris[83] = 81;

            tris[84] = 82;
            tris[85] = 130;
            tris[86] = 131;
            tris[87] = 131;
            tris[88] = 83;
            tris[89] = 82;

            tris[90] = 83;
            tris[91] = 131;
            tris[92] = 116;
            tris[93] = 116;
            tris[94] = 68;
            tris[95] = 83;

            return tris;
        }

        public int[] Tyre2()
        {
            var tris = new int[96];
            tris[0] = 100;
            tris[1] = 84;
            tris[2] = 85;
            tris[3] = 85;
            tris[4] = 101;
            tris[5] = 100;

            tris[6] = 101;
            tris[7] = 85;
            tris[8] = 86;
            tris[9] = 86;
            tris[10] = 102;
            tris[11] = 101;

            tris[12] = 102;
            tris[13] = 86;
            tris[14] = 87;
            tris[15] = 87;
            tris[16] = 103;
            tris[17] = 102;

            tris[18] = 103;
            tris[19] = 87;
            tris[20] = 88;
            tris[21] = 88;
            tris[22] = 104;
            tris[23] = 103;

            tris[24] = 104;
            tris[25] = 88;
            tris[26] = 89;
            tris[27] = 89;
            tris[28] = 105;
            tris[29] = 104;

            tris[30] = 105;
            tris[31] = 89;
            tris[32] = 90;
            tris[33] = 90;
            tris[34] = 106;
            tris[35] = 105;

            tris[36] = 106;
            tris[37] = 90;
            tris[38] = 91;
            tris[39] = 91;
            tris[40] = 107;
            tris[41] = 106;

            tris[42] = 107;
            tris[43] = 91;
            tris[44] = 92;
            tris[45] = 92;
            tris[46] = 108;
            tris[47] = 107;

            tris[48] = 108;
            tris[49] = 92;
            tris[50] = 93;
            tris[51] = 93;
            tris[52] = 109;
            tris[53] = 108;

            tris[54] = 109;
            tris[55] = 93;
            tris[56] = 94;
            tris[57] = 94;
            tris[58] = 110;
            tris[59] = 109;

            tris[60] = 110;
            tris[61] = 94;
            tris[62] = 95;
            tris[63] = 95;
            tris[64] = 111;
            tris[65] = 110;

            tris[66] = 111;
            tris[67] = 95;
            tris[68] = 96;
            tris[69] = 96;
            tris[70] = 112;
            tris[71] = 111;

            tris[72] = 112;
            tris[73] = 96;
            tris[74] = 97;
            tris[75] = 97;
            tris[76] = 113;
            tris[77] = 112;

            tris[78] = 113;
            tris[79] = 97;
            tris[80] = 98;
            tris[81] = 98;
            tris[82] = 114;
            tris[83] = 113;

            tris[84] = 114;
            tris[85] = 98;
            tris[86] = 99;
            tris[87] = 99;
            tris[88] = 115;
            tris[89] = 114;

            tris[90] = 115;
            tris[91] = 99;
            tris[92] = 84;
            tris[93] = 84;
            tris[94] = 100;
            tris[95] = 115;

            return tris;
        }

        public int[] Tyre3()
        {
            var tris = new int[96];
            tris[0] = 52;
            tris[1] = 36;
            tris[2] = 51;
            tris[3] = 51;
            tris[4] = 67;
            tris[5] = 52;

            tris[6] = 67;
            tris[7] = 51;
            tris[8] = 50;
            tris[9] = 50;
            tris[10] = 66;
            tris[11] = 67;

            tris[12] = 66;
            tris[13] = 50;
            tris[14] = 49;
            tris[15] = 49;
            tris[16] = 65;
            tris[17] = 66;

            tris[18] = 65;
            tris[19] = 49;
            tris[20] = 48;
            tris[21] = 48;
            tris[22] = 64;
            tris[23] = 65;

            tris[24] = 64;
            tris[25] = 48;
            tris[26] = 47;
            tris[27] = 47;
            tris[28] = 63;
            tris[29] = 64;

            tris[30] = 63;
            tris[31] = 47;
            tris[32] = 46;
            tris[33] = 46;
            tris[34] = 62;
            tris[35] = 63;

            tris[36] = 62;
            tris[37] = 46;
            tris[38] = 45;
            tris[39] = 45;
            tris[40] = 61;
            tris[41] = 62;

            tris[42] = 61;
            tris[43] = 45;
            tris[44] = 44;
            tris[45] = 44;
            tris[46] = 60;
            tris[47] = 61;

            tris[48] = 60;
            tris[49] = 44;
            tris[50] = 43;
            tris[51] = 43;
            tris[52] = 59;
            tris[53] = 60;

            tris[54] = 59;
            tris[55] = 43;
            tris[56] = 42;
            tris[57] = 42;
            tris[58] = 58;
            tris[59] = 59;

            tris[60] = 58;
            tris[61] = 42;
            tris[62] = 41;
            tris[63] = 41;
            tris[64] = 57;
            tris[65] = 58;

            tris[66] = 57;
            tris[67] = 41;
            tris[68] = 40;
            tris[69] = 40;
            tris[70] = 56;
            tris[71] = 57;

            tris[72] = 56;
            tris[73] = 40;
            tris[74] = 39;
            tris[75] = 39;
            tris[76] = 55;
            tris[77] = 56;

            tris[78] = 55;
            tris[79] = 39;
            tris[80] = 38;
            tris[81] = 38;
            tris[82] = 54;
            tris[83] = 55;

            tris[84] = 54;
            tris[85] = 38;
            tris[86] = 37;
            tris[87] = 37;
            tris[88] = 53;
            tris[89] = 54;

            tris[90] = 53;
            tris[91] = 37;
            tris[92] = 36;
            tris[93] = 36;
            tris[94] = 52;
            tris[95] = 53;

            return tris;
        }

        public int[] Tyre4()
        {
            var tris = new int[96];
            tris[0] = 0;
            tris[1] = 20;
            tris[2] = 35;
            tris[3] = 35;
            tris[4] = 19;
            tris[5] = 0;

            tris[6] = 19;
            tris[7] = 35;
            tris[8] = 34;
            tris[9] = 34;
            tris[10] = 18;
            tris[11] = 19;

            tris[12] = 18;
            tris[13] = 34;
            tris[14] = 33;
            tris[15] = 33;
            tris[16] = 17;
            tris[17] = 18;

            tris[18] = 17;
            tris[19] = 33;
            tris[20] = 32;
            tris[21] = 32;
            tris[22] = 16;
            tris[23] = 17;

            tris[24] = 16;
            tris[25] = 32;
            tris[26] = 31;
            tris[27] = 31;
            tris[28] = 15;
            tris[29] = 16;

            tris[30] = 15;
            tris[31] = 31;
            tris[32] = 30;
            tris[33] = 30;
            tris[34] = 14;
            tris[35] = 15;

            tris[36] = 14;
            tris[37] = 30;
            tris[38] = 29;
            tris[39] = 29;
            tris[40] = 13;
            tris[41] = 14;

            tris[42] = 13;
            tris[43] = 29;
            tris[44] = 28;
            tris[45] = 28;
            tris[46] = 12;
            tris[47] = 13;

            tris[48] = 12;
            tris[49] = 28;
            tris[50] = 27;
            tris[51] = 27;
            tris[52] = 11;
            tris[53] = 12;

            tris[54] = 11;
            tris[55] = 27;
            tris[56] = 26;
            tris[57] = 26;
            tris[58] = 10;
            tris[59] = 11;

            tris[60] = 10;
            tris[61] = 26;
            tris[62] = 25;
            tris[63] = 25;
            tris[64] = 9;
            tris[65] = 10;

            tris[66] = 9;
            tris[67] = 25;
            tris[68] = 24;
            tris[69] = 24;
            tris[70] = 8;
            tris[71] = 9;

            tris[72] = 8;
            tris[73] = 24;
            tris[74] = 23;
            tris[75] = 23;
            tris[76] = 7;
            tris[77] = 8;

            tris[78] = 7;
            tris[79] = 23;
            tris[80] = 22;
            tris[81] = 22;
            tris[82] = 6;
            tris[83] = 7;

            tris[84] = 6;
            tris[85] = 22;
            tris[86] = 21;
            tris[87] = 21;
            tris[88] = 5;
            tris[89] = 6;

            tris[90] = 5;
            tris[91] = 21;
            tris[92] = 20;
            tris[93] = 20;
            tris[94] = 0;
            tris[95] = 5;

            return tris;
        }

    }
}//end namespace