using GuiBaseUI;
using Harmony12;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static GuiScroll.NewActorListScroll;

namespace GuiScroll
{

    public static class ActorMenuActorListPatch
    {
        public static IEnumeratorActorMenuOpend updateActorMenu;
        public static void Init(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GameObject updateActorMenu = new GameObject();
            GameObject.DontDestroyOnLoad(updateActorMenu);
            ActorMenuActorListPatch.updateActorMenu = updateActorMenu.AddComponent<IEnumeratorActorMenuOpend>();
        }

        public static bool isShowActorMenu
        {
            get
            {
                var f_isShowActorMenu = typeof(ActorMenu).GetField("isShowActorMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                return (bool)f_isShowActorMenu.GetValue(ActorMenu.instance);
            }
            set
            {
                var f_isShowActorMenu = typeof(ActorMenu).GetField("isShowActorMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                f_isShowActorMenu.SetValue(ActorMenu.instance, value);
            }
        }
        public static List<int> showActors = new List<int>();
        //public static List<int> showActors
        //{
        //    get
        //    {
        //        var f_showActors = typeof(ActorMenu).GetField("showActors", BindingFlags.NonPublic | BindingFlags.Instance);
        //        return (List<int>)f_showActors.GetValue(ActorMenu.instance);
        //    }
        //}

        public static GameObject closeButton
        {
            get { return ActorMenu.instance.closeButton; }
        }

        public static bool isEnemy
        {
            get { return ActorMenu.instance.isEnemy; }
            set { ActorMenu.instance.isEnemy = value; }
        }

        public static Toggle actorTeamToggle
        {
            get { return ActorMenu.instance.actorTeamToggle; }
        }

        public static GameObject actorMenu
        {
            get { return ActorMenu.instance.actorMenu; }
        }

        public static int acotrId
        {
            get { return ActorMenu.instance.acotrId; }
            set { ActorMenu.instance.acotrId = value; }
        }

        public static RectTransform listActorsHolder
        {
            get { return ActorMenu.instance.listActorsHolder; }
        }

        public static Toggle actorAttrToggle
        {
            get { return ActorMenu.instance.actorAttrToggle; }
        }

        public static Toggle actorEquipToggle
        {
            get { return ActorMenu.instance.actorEquipToggle; }
        }

        public static Toggle actorGongFaToggle
        {
            get { return ActorMenu.instance.actorGongFaToggle; }
        }

        public static Toggle actorPeopleToggle
        {
            get { return ActorMenu.instance.actorPeopleToggle; }
        }

        public static Toggle actorMassageToggle
        {
            get { return ActorMenu.instance.actorMassageToggle; }
        }

        public static Text familySizeText
        {
            get { return ActorMenu.instance.familySizeText; }
        }
        public static int giveActorId = 0;

        public static NewActorListScroll m_listActorsHolder;
        private static void InitGuiUI()
        {
            ActorMenu.instance.listActorsHolder.gameObject.SetActive(false);
            m_listActorsHolder = ActorMenu.instance.listActorsHolder.parent.parent.gameObject.AddComponent<NewActorListScroll>();
            m_listActorsHolder.Init();
        }



        // ��ʾ��ɫ�˵�
        [HarmonyPatch(typeof(ActorMenu), "ShowActorMenu")]
        public static class ActorMenu_ShowActorMenu_Patch
        {



            public static bool Prefix(bool enemy)
            {
                if (!Main.enabled)
                    return true;

                if (m_listActorsHolder == null)
                {
                    InitGuiUI();
                }

                Main.Logger.Log("��ʾ��ɫ�˵� ShowActorMenu " + enemy);

                if (!isShowActorMenu && !DateFile.instance.playerMoveing)
                {
                    closeButton.SetActive(!DateFile.instance.setNewMianActor);
                    if (DateFile.instance.battleStart && StartBattle.instance.enemyTeamId == 4)
                    {
                        Teaching.instance.RemoveTeachingWindow(0);
                        closeButton.SetActive(value: false);
                    }
                    isShowActorMenu = true;
                    showActors.Clear();
                    if (enemy)
                    {
                        if (DateFile.instance.battleStart)
                        {
                            for (int i = 0; i < DateFile.instance.enemyBattlerIdDate.Count; i++)
                            {
                                showActors.Add(DateFile.instance.enemyBattlerIdDate[i]);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < MassageWindow.instance.massageActors.Count; j++)
                            {
                                showActors.Add(MassageWindow.instance.massageActors[j]);
                            }
                        }
                    }
                    else if (DateFile.instance.battleStart)
                    {
                        for (int k = 0; k < DateFile.instance.actorBattlerIdDate.Count; k++)
                        {
                            int num = DateFile.instance.actorBattlerIdDate[k];
                            if (num > 0)
                            {
                                showActors.Add(DateFile.instance.actorBattlerIdDate[k]);
                            }
                        }
                        if (showActors[0] != DateFile.instance.MianActorID())
                        {
                            enemy = true;
                        }
                    }
                    else
                    {
                        List<int> list = new List<int>(DateFile.instance.GetFamily(getPrisoner: true));
                        for (int l = 0; l < list.Count; l++)
                        {
                            int num2 = list[l];
                            if (num2 > 0 && !DateFile.instance.enemyBattlerIdDate.Contains(num2))
                            {
                                showActors.Add(list[l]);
                            }
                        }
                    }
                    isEnemy = enemy;
                    actorTeamToggle.interactable = !enemy;
                    TipsWindow.instance.showTipsTime = -100;
                    if (StorySystem.instance.itemWindowIsShow)
                    {
                        StorySystem.instance.CloseItemWindow();
                    }
                    if (StorySystem.instance.toStoryIsShow)
                    {
                        StorySystem.instance.ClossToStoryMenu();
                    }
                    if (!DateFile.instance.battleStart)
                    {
                        UIMove.instance.CloseGUI();
                        //StartCoroutine(ActorMenuOpend(0.2f));
                        updateActorMenu.fun_times = 1;
                        updateActorMenu.down_time = 0.2f;
                    }
                    else
                    {
                        //StartCoroutine(ActorMenuOpend(0f));
                        updateActorMenu.fun_times = 1;
                        updateActorMenu.down_time = 0;
                    }
                }


                return false;
            }
        }


        // ��ʾ��ɫ
        [HarmonyPatch(typeof(ActorMenu), "SetActorList")]
        public static class ActorMenu_SetActorList_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled && m_listActorsHolder != null)
                    return true;

                Main.Logger.Log("��ʾ��ɫ SetActorList");

                ActorMenu_RemoveActorList_Patch.Prefix();
                //for (int i = 0; i < showActors.Count; i++)
                //{
                //    int num = showActors[i];
                //    GameObject gameObject = UnityEngine.Object.Instantiate(listActor, Vector3.zero, Quaternion.identity);
                //    gameObject.name = "Actor," + num;
                //    gameObject.transform.SetParent(listActorsHolder, worldPositionStays: false);
                //    gameObject.GetComponent<Toggle>().group = listActorsHolder.GetComponent<ToggleGroup>();
                //    gameObject.GetComponent<SetListActor>().SetActor(num);
                //}
                //m_listActorsHolder.data = showActors.ToArray();

                m_listActorsHolder.scrollRect.verticalNormalizedPosition = 1;

                ActorMenu.instance.SortActorList();

                return false;
            }
        }

        // �����ɫ�б�
        [HarmonyPatch(typeof(ActorMenu), "SortActorList")]
        public static class ActorMenu_SortActorList_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled)
                    return true;


                Main.Logger.Log("�����ɫ�б� SortActorList");

                int idx = 0;
                List<int> list = showActors;
                int[] sort_result = new int[list.Count];
                if (!isEnemy)
                {
                    List<int> list2 = new List<int>();
                    List<int> list3 = new List<int>();
                    List<int> list4 = new List<int>();
                    List<int> list5 = new List<int>();
                    Main.Logger.Log("��ʼ���� SortActorList");
                    for (int j = 0; j < list.Count; j++)
                    {
                        int actorId = list[j];
                        if (actorId == DateFile.instance.MianActorID())
                        {
                            sort_result[idx] = actorId;
                            idx++;
                        }
                        else if (DateFile.instance.acotrTeamDate.Contains(actorId))
                        {
                            list2.Add(actorId);
                        }
                        else if (DateFile.instance.ParseInt(DateFile.instance.GetActorDate(actorId, 27, addValue: false)) == 1)
                        {
                            list4.Add(actorId);
                        }
                        else if (DateFile.instance.ActorIsWorking(actorId) != null)
                        {
                            list3.Add(actorId);
                        }
                        else
                        {
                            list5.Add(actorId);
                        }
                    }
                    Main.Logger.Log("������� SortActorList");
                    for (int k = 0; k < list2.Count; k++)
                    {
                        int num3 = list2[k];
                        sort_result[idx] = num3;
                        idx++;
                    }
                    for (int l = 0; l < list5.Count; l++)
                    {
                        int num4 = list5[l];
                        sort_result[idx] = num4;
                        idx++;
                    }
                    for (int m = 0; m < list4.Count; m++)
                    {
                        int num5 = list4[m];
                        sort_result[idx] = num5;
                        idx++;
                    }
                    for (int n = 0; n < list3.Count; n++)
                    {
                        int num6 = list3[n];
                        sort_result[idx] = num6;
                        idx++;
                    }
                    Main.Logger.Log("������� SortActorList");
                }
                else
                {
                    sort_result = list.ToArray();
                }
                Main.Logger.Log("��ʼ���õ��� SortActorList");
                showActors = new List<int>(sort_result);

                Main.Logger.Log("���õ��˺��� SortActorList");
                int num9 = isEnemy ? showActors.Count : DateFile.instance.GetFamily(getPrisoner: true).Count;
                Main.Logger.Log("num9�������� SortActorList " + num9);
                familySizeText.text = DateFile.instance.SetColoer((num9 > DateFile.instance.GetMaxFamilySize()) ? 20010 : 20003, num9 + " / " + DateFile.instance.GetMaxFamilySize());


                return false;
            }
        }


        [HarmonyPatch(typeof(ActorMenu), "UpdateActorListFavor")]
        public static class ActorMenu_UpdateActorListFavort_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled)
                    return true;

                Main.Logger.Log("���½�ɫ�б�ϲ�� UpdateActorListFavor");
                m_listActorsHolder.data = showActors.ToArray();


                return false;
            }
        }


        [HarmonyPatch(typeof(ActorMenu), "UpdateActorListFace")]
        public static class ActorMenu_UpdateActorListFace_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled)
                    return true;

                Main.Logger.Log("��ʾ��ɫ�б����� UpdateActorListFace");
                m_listActorsHolder.data = showActors.ToArray();


                return false;
            }
        }


        [HarmonyPatch(typeof(ActorMenu), "RemoveActorList")]
        public static class ActorMenu_RemoveActorList_Patch
        {
            public static bool Prefix()
            {
                if (!Main.enabled)
                    return true;

                m_listActorsHolder.data = new int[0];


                Main.Logger.Log("ɾ����ɫ�б� RemoveActorList");


                return false;
            }
        }


        [HarmonyPatch(typeof(ActorMenu), "SetActorAttr")]
        public static class ActorMenu_SetActorAttr_Patch
        {
            public static bool Prefix(int key)
            {
                if (!Main.enabled)
                    return true;

                int key1 = acotrId;
                int key2 = giveActorId;
                bool selGive = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

                Main.Logger.Log(selGive + " ���ý�ɫ���� " + key);

                if (selGive)
                {
                    if (giveActorId == key)
                    {
                        Main.Logger.Log(key+"=========��ͬ���ͣ�" + giveActorId);
                        return false;
                    }
                    else
                        giveActorId = key;
                }
                if (!selGive)
                {
                    if (acotrId == key)
                    {
                        Main.Logger.Log(key + "=========��ͬѡ�У�" + acotrId);
                        return false;
                    }
                    else
                        acotrId = key;

                }
                bool result = false;
                var tf = m_listActorsHolder.rectContent;
                if (tf.childCount > 0)
                {
                    // ����ѡ��
                    string name = "Actor," + key;
                    var child = tf.Find(name);
                    if (child != null)
                    {
                        ActorItem item = child.GetComponentInChildren<ActorItem>();
                        ChildData childData = item.childDatas[0];
                        SelectState selectState;
                        if (selGive)
                        {
                            if (key == acotrId)
                            {
                                selectState = SelectState.All;
                            }
                            else
                            {
                                selectState = SelectState.Give;
                            }
                        }
                        else
                        {
                            if (key == giveActorId)
                            {
                                selectState = SelectState.All;
                            }
                            else
                            {
                                selectState = SelectState.Select;
                            }
                            result = true;
                        }
                        Main.Logger.Log("����ѡ�У�" + selectState);
                        childData.Select(selectState);
                    }
                    // ȡ��ԭ��������
                    if (selGive)
                    {
                        name = "Actor," + key2;
                        child = tf.Find(name);
                        if (child != null)
                        {
                            ActorItem item = child.GetComponentInChildren<ActorItem>();
                            ChildData childData = item.childDatas[0];
                            SelectState selectState;
                            if (key2 == acotrId)
                                selectState = SelectState.Select;
                            else
                                selectState = SelectState.Node;
                            Main.Logger.Log("ȡ��ԭ�������ͣ�" + selectState);
                            childData.Select(selectState);
                        }
                    }
                    else // ȡ��ԭ����ѡ��
                    {
                        name = "Actor," + key1;
                        child = tf.Find(name);
                        if (child != null)
                        {
                            ActorItem item = child.GetComponentInChildren<ActorItem>();
                            ChildData childData = item.childDatas[0];
                            SelectState selectState;
                            if (key1 == giveActorId)
                                selectState = SelectState.Give;
                            else
                                selectState = SelectState.Node;
                            childData.Select(selectState);
                        }
                    }
                }
                Main.Logger.Log("�Ƿ����ѡ�У�" + result);
                Main.Logger.Log("=========ѡ�У�" +acotrId);
                Main.Logger.Log("=========���ͣ�" + giveActorId);
                return result;
            }
        }
    }
    public class IEnumeratorActorMenuOpend : MonoBehaviour
    {
        public float fun_times = 0;
        public float down_time = 0;
        private void Update()
        {
            if (fun_times == 1)
            {
                if (down_time > 0)
                {
                    down_time -= Time.deltaTime;
                }
                else
                {
                    fun_times++;
                    ActorMenuOpend1();
                }
            }
        }

        public void ActorMenuOpend1()
        {
            Main.Logger.Log("��ɫ�˵��� 111");

            TipsWindow.instance.showTipsTime = -100;
            YesOrNoWindow.instance.ShwoWindowMask(ActorMenuActorListPatch.actorMenu.transform, on: true);
            ActorMenuActorListPatch.acotrId = 0;
            ActorMenuActorListPatch.giveActorId = 0;
            ActorMenu.instance.SetActorTeam();
            ActorMenu.instance.SetActorList();
            //if (ActorMenuActorListPatch.listActorsHolder.childCount > 0)
            //{
            //    ActorMenuActorListPatch.listActorsHolder.GetChild(0).GetComponent<Toggle>().isOn = true;
            //}

            if (ActorMenuActorListPatch.showActors.Count > 0)
            {
                ActorMenuActorListPatch.giveActorId = ActorMenuActorListPatch.showActors[0];
                ActorMenu.instance.SetActorAttr(ActorMenuActorListPatch.showActors[0]);
            }
            ActorMenuActorListPatch.m_listActorsHolder.data = ActorMenuActorListPatch.showActors.ToArray();


            ActorMenuActorListPatch.actorMenu.SetActive(true);
            if (ActorMenuActorListPatch.isEnemy)
            {
                ActorMenuActorListPatch.actorAttrToggle.isOn = true;
            }
            ActorMenuActorListPatch.isShowActorMenu = false;
            if (DateFile.instance.teachingOpening > 0)
            {
                switch (DateFile.instance.teachingOpening)
                {
                    case 101:
                        if (!ActorMenuActorListPatch.actorEquipToggle.isOn)
                        {
                            Teaching.instance.SetTeachingWindow(2);
                        }
                        Teaching.instance.SetTeachingWindow(4);
                        break;
                    case 500:
                        if (!ActorMenuActorListPatch.actorGongFaToggle.isOn)
                        {
                            Teaching.instance.SetTeachingWindow(1);
                        }
                        Teaching.instance.SetTeachingWindow(2);
                        break;
                }
            }
            ActorMenuActorListPatch.actorPeopleToggle.interactable = (!ActorMenuActorListPatch.isEnemy || DateFile.instance.ParseInt(DateFile.instance.GetActorDate(ActorMenuActorListPatch.acotrId, 8, addValue: false)) == 1);
            ActorMenuActorListPatch.actorMassageToggle.interactable = (!ActorMenuActorListPatch.isEnemy || DateFile.instance.ParseInt(DateFile.instance.GetActorDate(ActorMenuActorListPatch.acotrId, 8, addValue: false)) == 1);
            if (!DateFile.instance.battleStart && !HomeSystem.instance.homeSystem.activeSelf && !StorySystem.instance.storySystem.activeSelf)
            {
                WorldMapSystem.instance.worldMapSystemHolder.gameObject.SetActive(false);
            }
            Main.Logger.Log("��ɫ�˵��� 444 " + ActorMenuActorListPatch.acotrId);
        }
    }


    public class NewActorListScroll : MonoBehaviour
    {
        public static int lineCount = 1;
        BigDataScroll bigDataScroll;
        public ScrollRect scrollRect;
        public RectTransform rectContent;
        private int[] m_data;
        public int[] data
        {
            set
            {
                m_data = value;
                SetData();
            }
            get
            {
                return m_data;
            }
        }
        public bool isInit = false;


        //�ű�����ActorHolder����
        public void Init()
        {
            InitUI();
        }

        private void InitUI()
        {
            isInit = true;

            Vector2 size = new Vector2(230, 830);
            Vector2 pos = new Vector2(0, 0);
            Vector2 cellSize = new Vector2(230, 97);
            float cellWidth = cellSize.x;
            float cellHeight = cellSize.y;
            // Main.Logger.Log("10");

            GameObject scrollView = CreateUI.NewScrollView(size, BarType.Vertical, ContentType.VerticalLayout); // ��������UI
            scrollRect = scrollView.GetComponent<ScrollRect>(); // �õ��������
            //WorldMapSystem.instance.actorHolder = scrollRect.content; 
            rectContent = scrollRect.content; // ���ݰ� content
            rectContent.GetComponent<ContentSizeFitter>().enabled = false; //�رո߶��Զ���Ӧ
            rectContent.GetComponent<VerticalLayoutGroup>().enabled = false; // �ر��Զ�����
            Main.Logger.Log("��");

            scrollRect.verticalNormalizedPosition = 1; // �������ϵ�λ��
            Image imgScrollView = scrollView.GetComponentInChildren<Image>(); // �õ�����ͼ
            imgScrollView.color = new Color(0.5f, 0.5f, 0.5f, 1f); // ����ͼ��ɫ
            imgScrollView.raycastTarget = false; // ���ñ������ɵ��
            RectTransform rScrollView = ((RectTransform)scrollView.transform); // �õ�����UI
            rScrollView.SetParent(gameObject.transform, false); // ���ø�����
            rScrollView.anchoredPosition = pos; // ����λ��

            //scrollView.GetComponentInChildren<Mask>().enabled = false;
            Main.Logger.Log("��0");

            GameObject gItemCell = new GameObject("line", new System.Type[] { typeof(RectTransform) }); // ����һ��
            RectTransform rItemCell = gItemCell.GetComponent<RectTransform>(); // �õ�transform
            rItemCell.SetParent(transform, false); // ���ø�����
            rItemCell.anchoredPosition = new Vector2(10000, 10000); // ������ңԶ���
            rItemCell.sizeDelta = new Vector2(cellWidth, cellHeight); // ���ô�С
                                                                      //Image imgItemCell = gItemCell.AddComponent<Image>();
                                                                      //imgItemCell.color = new Color(1, 0, 0, 0.5f);
            Main.Logger.Log("���");

            GameObject prefab = ActorMenu.instance.listActor; // �õ�������Ԥ�Ƽ�  ����������������������������������������������������������
            for (int i = 0; i < lineCount; i++) // һ�м���
            {
                GameObject go = UnityEngine.Object.Instantiate(prefab); // ����ÿ��������
                go.transform.SetParent(rItemCell, false); // ���ø�����
                Toggle tog = go.GetComponentInChildren<Toggle>();
                var tar = tog.targetGraphic;
                Main.Logger.Log(tog + "����" + tog.graphic + " " + tog.graphic.transform.parent);
                DestroyImmediate(tog);
                Button btn = go.AddComponent<Button>();
                btn.targetGraphic = tar;
            }
            Main.Logger.Log("���0");


            GridLayoutGroup gridLayoutGroup = gItemCell.AddComponent<GridLayoutGroup>();  // ����������
            gridLayoutGroup.cellSize = prefab.GetComponent<RectTransform>().sizeDelta; // �����������С
            gridLayoutGroup.spacing = new Vector2(0, 0); // ������߾�
            gridLayoutGroup.padding.left = (int)(12); // ��ƫ��
            gridLayoutGroup.padding.top = (int)(0); // ��ƫ��
            Main.Logger.Log("���1");


            ActorItem itemCell = gItemCell.AddComponent<ActorItem>(); // ��Ӵ�����������������
            bigDataScroll = gameObject.AddComponent<BigDataScroll>();  // ��Ӵ����ݹ������
            bigDataScroll.Init(scrollRect, itemCell, SetCell); // ��ʼ�����������
            bigDataScroll.cellHeight = cellHeight; // ����һ�и߶�

            //GuiBaseUI.Main.LogAllChild(transform, true);



            // ���������û�����ͼƬ
            ScrollRect scroll = transform.GetComponent<ScrollRect>();
            // Main.Logger.Log("���v");
            RectTransform otherRect = scroll.verticalScrollbar.GetComponent<RectTransform>();
            Image other = otherRect.GetComponent<Image>();
            // Main.Logger.Log("���a");
            RectTransform myRect = scrollRect.verticalScrollbar.GetComponent<RectTransform>();
            //myRect.sizeDelta = new Vector2(10, 0);
            // Main.Logger.Log("���b");
            Image my = myRect.GetComponent<Image>();
            // Main.Logger.Log("���e");
            //my.color = new Color(0.9490196f, 0.509803951f, 0.503921571f);
            my.sprite = other.sprite;
            my.type = Image.Type.Sliced;
            // Main.Logger.Log("���p");

            // Main.Logger.Log("���V");
            RectTransform otherRect2 = scrollRect.verticalScrollbar.targetGraphic.GetComponent<RectTransform>();
            Image other2 = otherRect2.GetComponent<Image>();
            // Main.Logger.Log("���A");
            RectTransform myRect2 = scrollRect.verticalScrollbar.targetGraphic.GetComponent<RectTransform>();
            // Main.Logger.Log("���B");
            //myRect2.sizeDelta = new Vector2(10, 10);
            Image my2 = myRect2.GetComponent<Image>();
            // Main.Logger.Log("���C");
            //my2.color = new Color(0.5882353f, 0.807843149f, 0.8156863f);
            my2.sprite = other2.sprite;
            my2.type = Image.Type.Sliced;
            // Main.Logger.Log("���D");


            // Main.Logger.Log("���3");
            SetData();

        }

        private void SetData()
        {
            if (bigDataScroll != null && m_data != null && isInit)
            {
                int count = m_data.Length / lineCount + 1;
                //Main.Logger.Log("=======����������=======��������"+count);

                bigDataScroll.cellCount = count;
                //if (!Main.OnChangeList)
                //{
                //    scrollRect.verticalNormalizedPosition = 1;
                //}
            }
        }

        private void SetCell(ItemCell itemCell, int index)
        {
            //Main.Logger.Log(index.ToString() + "���� itemCell������" + itemCell.ToString() + " pos=" + scrollRect.verticalNormalizedPosition.ToString());
            ActorItem item = itemCell as ActorItem;
            if (item == null)
            {
                Main.Logger.Log("WarehouseItem��������");
                return;
            }
            //Main.Logger.Log("���ݳ��ȣ�" + m_data.Length);
            ChildData[] childDatas = item.childDatas;
            for (int i = 0; i < lineCount; i++)
            {
                int idx = (index - 1) * lineCount + i;
                //Main.Logger.Log("ѭ��" + i + "��ȡ�ڡ�" + idx + "����Ԫ�ص�����");
                if (i < childDatas.Length)
                {
                    ChildData childData = childDatas[i];
                   GameObject go = childData.gameObject;
                    if (idx < m_data.Length)
                    {
                        if (!go.activeSelf)
                        {
                            go.SetActive(true);
                        }
                        int num2 = m_data[idx];
                        itemCell.name = "Actor," + num2;
                        //Main.Logger.Log("����A��" + itemCell.name);
                        if (itemCell.transform.childCount > i)
                        {
                            var child = itemCell.transform.GetChild(i);
                            child.name = "Actor," + num2;
                            //Main.Logger.Log("����B��" + itemCell.name);
                            //Toggle tog = child.GetComponent<Toggle>();
                            //tog.group = ActorMenu.instance.listActorsHolder.GetComponent<ToggleGroup>();
                            //Main.Logger.Log(ActorMenu.instance.acotrId +" "+ num2 + "�ж��Ƿ�ѡ�У�" + (ActorMenu.instance.acotrId == num2));
                            //tog.isOn = ActorMenu.instance.acotrId == num2;
                            if(ActorMenu.instance.acotrId == num2&& ActorMenuActorListPatch.giveActorId == num2)
                            {
                                childData.Select(SelectState.All);
                            }
                            else if (ActorMenu.instance.acotrId == num2)
                            {
                                childData.Select(SelectState.Select);
                            }else if(ActorMenuActorListPatch.giveActorId == num2)
                            {
                                childData.Select(SelectState.Give);
                            }
                            else
                            {
                                childData.Select(SelectState.Node);
                            }
                            Button btn = child.GetComponentInChildren<Button>();
                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(delegate () {
                                ActorMenu.instance.SetActorAttr(int.Parse(go.name.Split(',')[1]));
                            });
                        }


                        SetListActor component = childData.setListActor;
                        component.SetActor(num2);

                        //Main.Logger.Log("�˴���һЩ���� ��ͬ��ݵ��˴���ʽ��ͬ");
                        if (!ActorMenu.instance.isEnemy)
                        {
                            if (num2 == DateFile.instance.MianActorID())
                            {
                                //Main.Logger.Log("����");
                                component.SetInTeamIcon(true);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.acotrTeamDate.Contains(num2)) 
                            {
                                //Main.Logger.Log("�ǳ�ս��Ա");
                                component.SetInTeamIcon(true);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.ParseInt(DateFile.instance.GetActorDate(num2, 27, addValue: false)) == 1)
                            {
                                //Main.Logger.Log("��֪����ɶ");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.ActorIsWorking(num2) != null)
                            {
                                //Main.Logger.Log("���ڹ�����");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(true);
                                component.RestMoodFace();
                            }
                            else
                            {
                                //Main.Logger.Log("����");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                        }
                        else
                        {
                            //Main.Logger.Log("����");
                            component.SetInTeamIcon(false);
                            component.SetInBuildingIcon(false);
                            component.RestMoodFace();
                        }
                    }
                    else
                    {
                        if (go.activeSelf)
                        {
                            go.SetActive(false);
                        }
                    }
                    if (i == 0 && !go.transform.parent.gameObject.activeSelf)
                        go.transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    Main.Logger.Log("���ݳ�������");
                }
                //Main.Logger.Log("�����������");
            }
        }

        private void Update()
        {
            if (!gameObject.activeInHierarchy | m_data == null | scrollRect == null)
            {
                return;
            }
            var mousePosition = Input.mousePosition;
            var mouseOnPackage = mousePosition.x < Screen.width / 16 && mousePosition.y > Screen.width / 10 && mousePosition.y < Screen.width / 10 * 9;

            var v = Input.GetAxis("Mouse ScrollWheel");
            if (v != 0)
            {
                if (mouseOnPackage)
                {
                    float count = m_data.Length / lineCount + 1;
                    scrollRect.verticalNormalizedPosition += v / count * Main.settings.scrollSpeed;
                }
            }
        }
        public class ActorItem : ItemCell
        {

            public ChildData[] childDatas;
            public override void Awake()
            {
                base.Awake();
                childDatas = new ChildData[lineCount];
                for (int i = 0; i < lineCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    childDatas[i] = new ChildData(child);
                }
                //Main.Logger.Log("WarehouseItem Awake " + childDatas.Length);
            }
        }
        public struct ChildData
        {
            public GameObject gameObject;
            public SetListActor setListActor;
            public Text des;
            public Image select;
            

            public ChildData(Transform child)
            {
                Main.Logger.Log("����һ��");
                gameObject = child.gameObject;
                Main.Logger.Log("aaa");
                setListActor = gameObject.GetComponent<SetListActor>();

                Main.Logger.Log("��ȡ"+setListActor);
                GameObject obj = GameObject.Instantiate<GameObject>(setListActor.listActorNameText.gameObject);
                Main.Logger.Log("aaa");
                des = obj.GetComponent<Text>();
                Main.Logger.Log("aaa");
                des.color = Color.red;
                Main.Logger.Log("aaa");
                RectTransform tf = obj.GetComponent<RectTransform>();
                Main.Logger.Log("aaa");
                tf.SetParent(gameObject.transform, false);
                Main.Logger.Log("aaa");
                tf.sizeDelta = new Vector2(tf.sizeDelta.y, tf.sizeDelta.y);
                Main.Logger.Log("aaa");
                tf.anchoredPosition = new Vector2(-30, 15);
                Main.Logger.Log("aaa");

                foreach (var item in child)
                {
                    Main.Logger.Log("... " + item);
                }

                select = child.Find("ListActorLabel").GetComponent<Image>();
                des.text = "��";
            }

            public void Select(SelectState value)
            {
                select.enabled = (SelectState.Select & value) == SelectState.Select;
                des.enabled = (SelectState.Give & value) == SelectState.Give;
            }
        }

        public enum SelectState
        {
            Node = 0,
            Select = 1,
            Give = 2,
            All = 3,
        }
    }
}