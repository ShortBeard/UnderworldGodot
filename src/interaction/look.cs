namespace Underworld
{
    /// <summary>
    /// Class for interactions involving the look verb
    /// </summary>
    public class look : UWClass
    {
        public static bool LookAt(int index, uwObject[] objList, bool WorldObject = true)
        {
            bool result = false;
            trap.ObjectThatStartedChain = index;
            if (index <= objList.GetUpperBound(0))
            {
                var obj = objList[index];
                switch (obj.majorclass)
                {
                    case 2:
                        {
                            result = LookMajorClass2(obj, objList);
                            break;
                        }
                    case 4:
                        {
                            result = LookMajorClass4(obj, objList);
                            break;
                        }
                    case 5:
                        {
                            result = LookMajorClass5(obj, objList);
                            break;
                        }

                }
                if ((obj.is_quant == 0) && (obj.link != 0))
                {
                    var linkedObj = objList[obj.link];
                    if (linkedObj.item_id == 419)
                    {
                        trigger.LookTrigger(
                            srcObject: obj,
                            triggerIndex: obj.link,
                            objList: objList);
                        return true;
                    }
                }
                if (!result)
                {
                    //default string  when no overriding action has occured           
                    //messageScroll.AddString(GameStrings.GetObjectNounUW(obj.item_id));
                    GeneralLookDescription(obj);
                }
            }

            return false;
        }

        public static bool LookMajorClass2(uwObject obj, uwObject[] objList)
        {
            // switch (obj.minorclass)
            // {
            //     case 3:
            //         return food.LookAt(obj);
            // }
            return false;
        }

        public static bool LookMajorClass4(uwObject obj, uwObject[] objList)
        {
            switch (obj.minorclass)
            {
                case 3: //readables (up to index 8)
                    {
                        if (obj.classindex <= 8)
                        {
                            return Readable.LookAt(obj);
                        }
                        break;
                    }
            }
            return false;
        }

        public static bool LookMajorClass5(uwObject obj, uwObject[] objList)
        {
            switch (obj.minorclass)
            {
                case 2: //misc objects including readables
                    {
                        switch (obj.classindex)
                        {
                            case 6: // a readable sign.
                                return writing.LookAt(obj);
                            case 0xE:
                            case 0xF:
                                return tmap.LookAt(obj);
                            default:
                                return false;
                        }
                    }

            }

            return false;
        }

        /// <summary>
        /// A look description based on the item id
        /// </summary>
        /// <param name="item_id"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public static bool GeneralLookDescription(int item_id, int qty=1)
        {
            string output;
            if (commonObjDat.PrintableLook(item_id))
            {
                output = "You see ";
            }
            else
            {
                System.Diagnostics.Debug.Print("No print description");
                return true;
            }
            string objectname = GameStrings.GetObjectNounUW(item_id, qty);
            var article = GetArticle(objectname);
            output += $"{article}{objectname}";
            messageScroll.AddString($"{output}");
            return true;
        }

        public static bool GeneralLookDescription(uwObject obj)
        {
            string output;
            if (commonObjDat.PrintableLook(obj.item_id))
            {
                output = "You see ";
            }
            else
            {
                System.Diagnostics.Debug.Print("No print description");
                return true;
            }

            //TODO object identifaciton string

            var qualityclass = commonObjDat.qualityclass(obj.item_id);
            int qty = 0;
            if (obj.is_quant == 1)
            {
                if (obj.link < 512)
                {
                    qty = obj.link;
                }
            }

            string objectname = GameStrings.GetObjectNounUW(obj.item_id, qty);

            var finalclass = -1;
            if (obj.quality > 0)
            {
                if (obj.quality == 1)
                {
                    var isLight = (obj.item_id & 0x1F8);
                    if (isLight != 0x90)
                    {
                        finalclass = 1 + (obj.quality >> 4);
                    }
                }
                else
                {
                    if (qualityclass == 3)
                    {
                        finalclass = 5;
                    }
                    else
                    {
                        finalclass = 1 + (obj.quality >> 4);
                    }
                }
            }

            var qualitystringid = commonObjDat.qualitytype(obj.item_id) * 6 + finalclass;
            var qualitystring = GameStrings.GetString(5, qualitystringid);
            if (qualitystring.Length > 0) { qualitystring += " "; }

            var article = "a ";
       
            string qtystring = "";
            if (qty > 1)
            {
                qtystring = $"{qty} ";
            }

            if (qty>=2)
            {
                article = ""; //GetArticle(qtystring);
            }
            else
            {
                if (qualitystring.Length>0)
                {
                    article = GetArticle(qualitystring);
                }
                else
                {
                    article = GetArticle(objectname);
                }
            }

            output += $"{article}{qtystring}{qualitystring}{objectname}";
            messageScroll.AddString($"{output}");
            return true;
        }

        private static string GetArticle(string noun)
        {
            if (
                (noun.ToUpper().StartsWith("A"))
                ||
                (noun.ToUpper().StartsWith("E"))
                ||
                (noun.ToUpper().StartsWith("I"))
                ||
                (noun.ToUpper().StartsWith("O"))
                ||
                (noun.ToUpper().StartsWith("U"))
                )
            {
                return "an ";
            }
            else
            {
                return "a ";
            }
        }
    }//end class
}//end namespace