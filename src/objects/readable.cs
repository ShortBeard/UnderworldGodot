namespace Underworld
{
    public class Readable: objectInstance
    {
        public static bool LookAt(uwObject obj)
        {
            if (obj.is_quant == 1)
                {
                look.GeneralLookDescription(obj.item_id);
                uimanager.AddToMessageScroll(GameStrings.GetString(3, obj.link-0x200));
                }
            return true;
        }

    }//end class
}//end namespace