using System.Diagnostics.Contracts;
using Microsoft.VisualBasic;

namespace Underworld
{
    public class automap:Loader
    {
        public static automap currentautomap;
        public static automap[] automaps;

        public byte[] buffer;

        public automaptileinfo[,] tiles = new automaptileinfo[64,64];

        public automap (int LevelNo)
        {
            //load buffer. then init tiles with their offsets
           int blockno;
            switch(_RES)
            {
                case GAME_UW2:
                    blockno = LevelNo * 4;
                    break;
                default:
                    blockno = LevelNo + 27;
                    break;
            }
            if (DataLoader.LoadUWBlock(LevArkLoader.lev_ark_file_data, blockno, 64 * 64, out UWBlock block))
            {
                buffer = block.Data;
                for (int y=0; y<64; y++)
                {
                    for (int x=0; x<64; x++)
                    {
                        tiles[x,y] = new automaptileinfo(y*64+x, ref buffer);
                    }
                }
            }
        }
    }
}