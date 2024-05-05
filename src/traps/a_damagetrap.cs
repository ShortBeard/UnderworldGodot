using System;

namespace Underworld
{
    public class a_damage_trap:trap
    {

         public static void Activate(uwObject trapObj, uwObject[] objList)
        {
            ApplyDamageTrap(trapObj.owner!=0, trapObj.quality);
        }


        public static void ApplyDamageTrap(bool Poison, int basedamage)
        {
            if (Poison)
            {
                //apply poisoning
                //int test = basedamage;
                //var scale = damage.ScaleDamage(127, ref test, 0x10); //player;
                if (basedamage> playerdat.play_poison)
                {
                    playerdat.play_poison = (byte)Math.Min(basedamage,0xF);
                }
            }
            else
            {
                //appy damage
                playerdat.play_hp  = Math.Max(0, playerdat.play_hp-basedamage);
            }
        }
    }
}
