
using Harmony12;

namespace GuiScroll
{
    public static class ActorMenuItemPackagePatch
    {
        public static int Key, Typ;
        public static NewItemPackage m_itemPackage;

        [HarmonyPatch(typeof(ActorMenu), "UpdateItems")]
        public static class ActorMenu_UpdateItems_Patch
        {
            public static bool Prefix(int key, int typ)
            {

                Key = key; Typ = typ;
                if (!Main.enabled)
                    return true;


                Main.Logger.Log("UpdateItems ������Ʒ key ��" + key + "      typ = " + typ);






                return false;
            }
        }

        [HarmonyPatch(typeof(ActorMenu), "RemoveAllItems")]
        public static class ActorMenu_RemoveAllItems_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled)
                    return true;
                Main.Logger.Log("RemoveAllItems ���������Ʒ");



                return false;
            }
        }

    }

}