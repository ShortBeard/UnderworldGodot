using Godot;

namespace Underworld
{

/// <summary>
/// Palette loader.
/// </summary>
    public class PaletteLoader : ArtLoader
    {
        
        /// <summary>
        /// Palettes in pals.dat
        /// </summary>
        public static Palette[] Palettes = new Palette[22];
        public static int NoOfPals = 22;

        /// <summary>
        /// A crappy greyscale palette
        /// </summary>
        public static Palette GreyScale = null;

        /// <summary>
        /// Palettes loaded by lights.dat
        /// </summary>
        public static lightmap[] light = null;

        /// <summary>
        /// Palettes loaded by mono.dat
        /// </summary>
        public static lightmap[] mono  = null;

        static PaletteLoader()  //void LoadPalettes(string filePath)
        {
            var path_pals = System.IO.Path.Combine(BasePath,"DATA","PALS.DAT");
            var path_light = System.IO.Path.Combine(BasePath,"DATA","LIGHT.DAT");
            var path_mono = System.IO.Path.Combine(BasePath,"DATA","MONO.DAT");
            GreyScale = new Palette();
            for (int i = 0; i <= GreyScale.blue.GetUpperBound(0); i++)
            {
                GreyScale.red[i] = (byte)i;
                GreyScale.blue[i] = 0;// (byte)i;
                GreyScale.green[i] = 0;// (byte)i;                
            }
            switch (_RES)
            {
                default:
                    {
                        Palettes = new Palette[NoOfPals];
                        if (ReadStreamFile(path_pals, out byte[] pals_dat))
                        {
                            for (int palNo = 0; palNo <= Palettes.GetUpperBound(0); palNo++)
                            {
                                Palettes[palNo] = new Palette();
                                for (int pixel = 0; pixel < 256; pixel++)
                                {
                                    Palettes[palNo].red[pixel] = (byte)(getValAtAddress(pals_dat, palNo * 256 + (pixel * 3) + 0, 8) << 2);
                                    Palettes[palNo].green[pixel] = (byte)(getValAtAddress(pals_dat, palNo * 256 + (pixel * 3) + 1, 8) << 2);
                                    Palettes[palNo].blue[pixel] = (byte)(getValAtAddress(pals_dat, palNo * 256 + (pixel * 3) + 2, 8) << 2);                                }
                            }
                        }

                        light = new lightmap[16];
                        if (ReadStreamFile(path_light, out byte[] light_dat))
                        {
                            for (int palNo = 0; palNo <= light.GetUpperBound(0); palNo++)
                            {
                                light[palNo] = new lightmap();
                                for (int pixel = 0; pixel < 256; pixel++)
                                { //just store the index values.
                                   light[palNo].red[pixel] = (byte)getValAtAddress(light_dat, palNo * 256 + pixel + 0, 8);
                                   light[palNo].blue[pixel] = (byte)getValAtAddress(light_dat, palNo * 256 + pixel + 0, 8);
                                   light[palNo].green[pixel] = (byte)getValAtAddress(light_dat, palNo * 256 + pixel + 0, 8);
                                }
                            }
                        }


                        mono = new lightmap[16];
                        if (ReadStreamFile(path_mono, out byte[] mono_dat))
                        {
                            for (int palNo = 0; palNo <= mono.GetUpperBound(0); palNo++)
                            {
                                mono[palNo] = new lightmap();
                                for (int pixel = 0; pixel < 256; pixel++)
                                { //just store the index values.
                                   mono[palNo].red[pixel] = (byte)getValAtAddress(mono_dat, palNo * 256 + pixel + 0, 8);
                                   mono[palNo].blue[pixel] = (byte)getValAtAddress(mono_dat, palNo * 256 + pixel + 0, 8);
                                   mono[palNo].green[pixel] = (byte)getValAtAddress(mono_dat, palNo * 256 + pixel + 0, 8);
                                }
                            }
                        }

                    }
                    break;
            }
        }

        public static int[] LoadAuxilaryPalIndices(string auxPalPath, int auxPalIndex)
        {
            int[] auxpal = new int[16];

            if (ReadStreamFile(auxPalPath, out byte[] palf))
            {
                for (int j = 0; j < 16; j++)
                {
                    auxpal[j] = (int)getValAtAddress(palf, auxPalIndex * 16 + j, 8);
                }
            }
            return auxpal;
        }

        public static Palette LoadAuxilaryPal(string auxPalPath, Palette gamepal, int auxPalIndex)
        {
            Palette auxpal = new Palette
            {
                red = new byte[16],
                green = new byte[16],
                blue = new byte[16]
            };
            if (ReadStreamFile(auxPalPath, out byte[] palf))
            {
                for (int j = 0; j < 16; j++)
                {
                    int value = (int)getValAtAddress(palf, auxPalIndex * 16 + j, 8);
                    auxpal.green[j] = gamepal.green[value];
                    auxpal.blue[j] = gamepal.blue[value];
                    auxpal.red[j] = gamepal.red[value];
                }
            }
            return auxpal;
        }


        /// <summary>
        /// Loads all the lightmaps as a single image to use as a global lookup in the shader.
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        public static ImageTexture AllLightMaps(lightmap[] maps)
        {
            byte[] imgdata =new byte[maps.GetUpperBound(0)*256];
            for (int l = 0; l< maps.GetUpperBound(0);l++)
            {
                for (int b=0;b<256;b++)
                {
                    imgdata[(l*256) + b] = maps[l].red[b];
                }
            }
            var output = ArtLoader.Image(imgdata, 0, 256, maps.GetUpperBound(0), "name here", GreyScale, true, true);
            return output;
        }

    }//end class


}//end namespace