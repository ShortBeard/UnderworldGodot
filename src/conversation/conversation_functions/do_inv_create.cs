namespace Underworld
{
    public partial class ConversationVM : UWClass
    {   
        public static void do_inv_create(uwObject talker)
        {
            var arg0 = at(at(stackptr-1));
            var slot = ObjectCreator.PrepareNewObject(arg0);
            var newObj = UWTileMap.current_tilemap.LevelObjects[slot];
            if(newObj!=null)
            {
                newObj.quality = 63;
                newObj.next = talker.link;
                talker.link = newObj.index;
            }
        }
    }//end class
}//end namespace