using System.Runtime.CompilerServices;

namespace Underworld
{
    //Player dat inventory
    public partial class playerdat : Loader
    {

        /// <summary>
        /// Reference to the object the player is using currently in their hand
        /// To be consistanct his object MUST always be a world object. When picking inventory objects add it to world
        /// </summary>
        public static int ObjectInHand = -1;

        public static uwObject[] InventoryObjects = new uwObject[512];
        //public static byte[] InventoryBuffer = new byte[512 * 8];

        /// <summary>
        /// The currently displayed backpack objects.
        /// </summary>
        public static int[] BackPackIndices = new int[8];

        /// <summary>
        /// The last item in the inventory
        /// </summary>
        public static int LastItemIndex;


        /// <summary>
        /// Stores an array listing the currently displayed backpack objects indices
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="obj"></param>
        public static void SetBackPackIndex(int slot, uwObject obj)
        {
            if (obj != null)
            {
                BackPackIndices[slot] = obj.index;
            }
            else
            {
                BackPackIndices[slot] = -1;
            }
        }

        /// <summary>
        /// Get the index of the object at the currently displayed backpack slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static int GetBackPackIndex(int slot)
        {
            return BackPackIndices[slot];
        }

        /// <summary>
        /// Looks up all inventory slots list indices. starting at helm and ending with backpacks
        /// </summary>
        /// <param name="slot"></param>
        public static int GetInventorySlotListHead(int slot)
        {
            int startOffset = 0xF8;
            if (_RES == GAME_UW2)
            {
                startOffset = 0x3A3;
            }
            return GetAt16(startOffset + slot * 2) >> 6;
        }

        public static int GetInventorySlotListHeadOffset(int slot)
        {
            int startOffset = 0xF8;
            if (_RES == GAME_UW2)
            {
                startOffset = 0x3A3;
            }
            return startOffset + slot * 2;
        }

        public static uwObject GetInventorySlotObject(int slot)
        {
            int index = GetInventorySlotListHead(slot);
            return InventoryObjects[index];
        }

        public static void SetInventorySlotListHead(int slot, int value)
        {
            value = value << 6;
            int startOffset = 0xF8;
            if (_RES == GAME_UW2)
            {
                startOffset = 0x3A3;
            }
            var currentValue = GetAt16(startOffset + slot * 2);
            currentValue &= 0x3f; //Preserve existing value
            var valueToSet = value | currentValue;
            SetAt16(startOffset + slot * 2, valueToSet);
        }

        /// <summary>
        /// Object index for the item at the helm slot
        /// </summary>
        public static int Helm
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3A3) >> 6;
                    default:
                        return GetAt16(0xF8) >> 6;
                }
            }
            set
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        SetAt16(0x3A3, value << 6); break;
                    default:
                        SetAt16(0xF8, value << 6); break;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the helm slot
        /// </summary>
        public static uwObject HelmObject
        {
            get
            {
                if (Helm != 0)
                {
                    return InventoryObjects[Helm];
                }
                return null;
            }
        }

        public static int ChestArmour
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3A5) >> 6;
                    default:
                        return GetAt16(0xFA) >> 6;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the chest armour slot
        /// </summary>
        public static uwObject ChestArmourObject
        {
            get
            {
                if (ChestArmour != 0)
                {
                    return InventoryObjects[ChestArmour];
                }
                return null;
            }
        }

        public static int Gloves
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3A7) >> 6;
                    default:
                        return GetAt16(0xFC) >> 6;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the gloves slot
        /// </summary>
        public static uwObject GlovesObject
        {
            get
            {
                if (Gloves != 0)
                {
                    return InventoryObjects[Gloves];
                }
                return null;
            }
        }

        public static int Leggings
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3A9) >> 6;
                    default:
                        return GetAt16(0xFE) >> 6;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the leggings slot
        /// </summary>
        public static uwObject LeggingsObject
        {
            get
            {
                if (Leggings != 0)
                {
                    return InventoryObjects[Leggings];
                }
                return null;
            }
        }

        public static int Boots
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3AB) >> 6;
                    default:
                        return GetAt16(0x100) >> 6;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the boots slot
        /// </summary>
        public static uwObject BootsObject
        {
            get
            {
                if (Boots != 0)
                {
                    return InventoryObjects[Boots];
                }
                return null;
            }
        }


        public static int RightShoulder
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3AD) >> 6;
                    default:
                        return GetAt16(0x102) >> 6;
                }
            }
        }


        /// <summary>
        /// Direct reference to the object at the boots slot
        /// </summary>
        public static uwObject RightShoulderObject
        {
            get
            {
                if (RightShoulder != 0)
                {
                    return InventoryObjects[RightShoulder];
                }
                return null;
            }
        }

        public static int LeftShoulder
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3AF) >> 6;
                    default:
                        return GetAt16(0x104) >> 6;
                }
            }
        }

        /// <summary>
        /// Direct reference to the object at the boots slot
        /// </summary>
        public static uwObject LeftShoulderObject
        {
            get
            {
                if (LeftShoulder != 0)
                {
                    return InventoryObjects[LeftShoulder];
                }
                return null;
            }
        }

        public static int RightHand
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3B1) >> 6;
                    default:
                        return GetAt16(0x106) >> 6;
                }
            }
        }

        /// <summary>
        /// Direct reference to the object at the boots slot
        /// </summary>
        public static uwObject RightHandObject
        {
            get
            {
                if (RightHand != 0)
                {
                    return InventoryObjects[RightHand];
                }
                return null;
            }
        }

        public static int LeftHand
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3B3) >> 6;
                    default:
                        return GetAt16(0x108) >> 6;
                }
            }
        }

        /// <summary>
        /// Direct reference to the object at the boots slot
        /// </summary>
        public static uwObject LeftHandObject
        {
            get
            {
                if (LeftHand != 0)
                {
                    return InventoryObjects[LeftHand];
                }
                return null;
            }
        }


        public static int RightRing
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3B5) >> 6;
                    default:
                        return GetAt16(0x10A) >> 6;
                }
            }
        }

        public static uwObject RightRingObject
        {
            get
            {
                if (RightRing != 0)
                {
                    return InventoryObjects[RightRing];
                }
                return null;
            }
        }
        public static int LeftRing
        {
            get
            {
                switch (_RES)
                {
                    case GAME_UW2:
                        return GetAt16(0x3B7) >> 6;
                    default:
                        return GetAt16(0x10C) >> 6;
                }
            }
        }
        public static uwObject LeftRingObject
        {
            get
            {
                if (LeftRing != 0)
                {
                    return InventoryObjects[LeftRing];
                }
                return null;
            }
        }

        public static int BackPack(int slot)
        {
            switch (_RES)
            {
                case GAME_UW2:
                    return GetAt16(0x3B9 + slot * 2) >> 6;
                default:
                    return GetAt16(0x10E + slot * 2) >> 6;
            }
        }

        public static uwObject BackPackObject(int slot)
        {
            if (BackPack(slot) != 0)
            {
                return InventoryObjects[BackPack(slot)];
            }
            return null;
        }


        /// <summary>
        /// The container currently opened on the paperdoll.
        /// </summary>
        public static int OpenedContainer=-1;


        /// <summary>
        /// The object containing the opened container. Returns -1 parent is at top level. Assumes container is opened.
        /// </summary>
        public static int OpenedContainerParent
        {
            get
            {
                if (OpenedContainer==-1)
                {
                    return -1;
                }
                for (int i=0;i<19;i++)
                {
                    if(GetInventorySlotListHead(i)==OpenedContainer)
                    {
                        return -1;
                    }
                }
                //try and find in the rest of the inventory
                foreach (var objToCheck in InventoryObjects)
                {//if this far down then I need to find the container that the closing container sits in
                    if (objToCheck!=null)
                    {
                        var result = objectsearch.GetContainingObject(
                            ListHead:objToCheck.index, 
                            ToFind: OpenedContainer,
                            objList: InventoryObjects);
                        if (result!=-1)
                        {//container found. Browse into it by using it
                            return result;
                        }
                    }
                }
                return -1; //not found. assume paperdoll
            }
        }


        /// <summary>
        /// Gets a free paperdoll slot
        /// </summary>
        public static int FreePaperDollSlot
        {
            get
            {
            for (int i=11;i<19;i++)
            {
                if (GetInventorySlotListHead(i)==0)
                {
                    return i;
                }
            }
            return -1;
            }
        }


        /// <summary>
        /// Removes the object data at index from the player inventory.
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveFromInventory(int index, bool updateUI = true)
        {
            var obj = InventoryObjects[index];
            var next = obj.next;
            //Find the object chain the object is in
            var LastObj = InventoryObjects[LastItemIndex];
            var LinkOffset = GetItemLinkOffset(index);

            if (LastItemIndex != next)
            {
                //This clears either the next or link to the object and replaces it with the objects next value
                var data = (int)getAt(pdat, (LinkOffset), 16);
                data &= 0x3f; //Clear link/next
                data |= (next << 6); //Or in the obj.next for the object.
                setAt(pdat, LinkOffset, 16, data);
            }
            else
            { //The next object is to be slided down in place of index. In this case link does not change.

            }
            //Slide down the last object.
            if (LastItemIndex == index)
            {//same objects. just wipe the data.
                for (int i = 0; i < 8; i++)
                {
                    pdat[obj.PTR + i] = 0;
                }
                InventoryObjects[index] = null;
                FindAndChangeBackpackIndex(index, -1);
                LastItemIndex--;
            }
            else
            {
                if (LastItemIndex <= 1)
                {
                    //Object is the last item. just clear data.
                    for (int i = 0; i < 8; i++)
                    {
                        pdat[obj.PTR + i] = 0;
                    }
                    InventoryObjects[index] = null;
                    FindAndChangeBackpackIndex(index, -1);
                    LastItemIndex--;
                }
                else
                {
                    //Object is to be replaced with last item. Last item needs to be relinked to the new slot as well as moved
                    var LastObjectLinkOffset = GetItemLinkOffset(LastItemIndex);
                    if (LastObjectLinkOffset != -1)
                    {
                        var data = (int)getAt(pdat, (LastObjectLinkOffset), 16);
                        data &= 0x3f; //Clear link/next
                        data |= (index << 6); //Or in the object next for the object.
                        setAt(pdat, LastObjectLinkOffset, 16, data);
                    }
                    //now move data
                    for (int i = 0; i < 8; i++)
                    {
                        pdat[obj.PTR + i] = pdat[LastObj.PTR + i];
                        pdat[LastObj.PTR + i] = 0;
                    }
                    InventoryObjects[LastItemIndex] = null;
                    FindAndChangeBackpackIndex(LastItemIndex, index);
                    LastItemIndex--;

                }
            }
            if (updateUI)
            {
                if (uimanager.CurrentSlot >= 11)
                {
                    BackPackIndices[uimanager.CurrentSlot - 11] = -1;
                }
                uimanager.UpdateInventoryDisplay();
            }

        }


        /// <summary>
        /// Finds and changes the backpack display array for the specifice object indices
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newValue"></param>
        static void FindAndChangeBackpackIndex(int index, int newValue = -1)
        {
            for (int i = 0; i < BackPackIndices.GetUpperBound(0); i++)
            {
                if (BackPackIndices[i] == index)
                {
                    BackPackIndices[i] = newValue;
                }
            }
        }

        /// <summary>
        /// Finds the byte that links to the object at index. Returns either a next or link that points to the object
        /// </summary>
        /// <param name="IndexToFind"></param>
        /// <returns></returns>
        public static int GetItemLinkOffset(int IndexToFind)
        {
            //first check the paperdolls
            for (int slot = 0; slot <= 18; slot++)
            {
                if (GetInventorySlotListHead(slot) == IndexToFind)
                {
                    //found. 
                    //object is on the paperdoll at that slot. return that slot offset
                    return GetInventorySlotListHeadOffset(slot);
                }
            }

            //otherwise object is on an internal inventory list.
            //Find direct container link first.
            foreach (var objToCheck in InventoryObjects)
            {//if this far down then I need to find the container that the closing container sits in
                if (objToCheck != null)
                {
                    var result = objectsearch.GetContainingObject(
                        ListHead: objToCheck.index,
                        ToFind: IndexToFind,
                        objList: InventoryObjects);
                    if (result != -1)
                    {//container found.
                     //Get either the container link or browse the next chain to match the object
                        if (objToCheck.link == IndexToFind) //check the first object in the container
                        {
                            return objToCheck.PTR + 6;
                        }
                        else
                        {
                            var NextObj = InventoryObjects[objToCheck.link]; //get the first object in the container
                            while (NextObj.next != 0)
                            {
                                if (NextObj.next == IndexToFind)
                                {
                                    return NextObj.PTR + 4;
                                }
                                else
                                {
                                    NextObj = InventoryObjects[NextObj.next];
                                }
                            }
                        }
                    }
                }
            }
            return -1; //not found!
        }



        /// <summary>
        /// Recursively adds object data to player inventory and returns the index it is at.
        /// </summary>
        /// <param name="objIndex"></param>
        /// <param name="ObjectToAddTo"></param>
        /// <returns></returns>
        public static short AddObjectToPlayerInventory(int objIndex)
        {
            var newIndex = MoveObjectToInventoryData(objIndex);
            var newobj = InventoryObjects[newIndex];
            // recursively add linked/next objects
            if (newobj.link != 0 && newobj.is_quant == 0)
            {
                newobj.link = AddObjectToPlayerInventory(newobj.link);
            }
            if (newobj.next != 0)
            {
                newobj.next = AddObjectToPlayerInventory(newobj.next);
            }


            return (short)newIndex;
        }

        private static int MoveObjectToInventoryData(int objIndex)
        {

            var obj = UWTileMap.current_tilemap.LevelObjects[objIndex];
            if (objIndex >= 256)
            {
                //Add to static free list
                UWTileMap.current_tilemap.StaticFreeListPtr++;
                UWTileMap.current_tilemap.StaticFreeListObject = objIndex;
            }
            else
            {
                // TODO mobile              //Add to mobile free list
                // UWTileMap.current_tilemap.MobileFreeListPtr++;
                // UWTileMap.current_tilemap.MobileFreeListObject = objIndex;
            }
            var newIndex = ++LastItemIndex;

            InventoryObjects[newIndex] = new uwObject
            {
                isInventory = true,
                IsStatic = true,
                index = (short)(newIndex),
                PTR = InventoryPtr + newIndex * 8,
                DataBuffer = pdat
            };
            var NewObj = InventoryObjects[newIndex];

            //copy data to this offset.... and wipe the old
            for (int i = 0; i < 8; i++)
            {
                InventoryObjects[newIndex].DataBuffer[NewObj.PTR+i] = obj.DataBuffer[obj.PTR + i];
                obj.DataBuffer[obj.PTR + i] = 0;
            }
            //Destroy the world object.
            obj.instance.uwnode.QueueFree();
            obj.instance = null;
            return newIndex;

        }
    } //end class
} //end namespace