
using GuiBaseUI;
using UnityEngine;
using UnityEngine.UI;

namespace GuiScroll
{


    public class NewItemPackage : MonoBehaviour
    {
        public static int lineCount = 9;
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
            // Main.Logger.Log("��");

            scrollRect.verticalNormalizedPosition = 1; // �������ϵ�λ��
            Image imgScrollView = scrollView.GetComponentInChildren<Image>(); // �õ�����ͼ
            imgScrollView.color = new Color(0.5f, 0.5f, 0.5f, 1f); // ����ͼ��ɫ
            imgScrollView.raycastTarget = false; // ���ñ������ɵ��
            RectTransform rScrollView = ((RectTransform)scrollView.transform); // �õ�����UI
            rScrollView.SetParent(gameObject.transform, false); // ���ø�����
            rScrollView.anchoredPosition = pos; // ����λ��

            //scrollView.GetComponentInChildren<Mask>().enabled = false;
            // Main.Logger.Log("��0");

            GameObject gItemCell = new GameObject("line", new System.Type[] { typeof(RectTransform) }); // ����һ��
            RectTransform rItemCell = gItemCell.GetComponent<RectTransform>(); // �õ�transform
            rItemCell.SetParent(transform, false); // ���ø�����
            rItemCell.anchoredPosition = new Vector2(10000, 10000); // ������ңԶ���
            rItemCell.sizeDelta = new Vector2(cellWidth, cellHeight); // ���ô�С
                                                                      //Image imgItemCell = gItemCell.AddComponent<Image>();
                                                                      //imgItemCell.color = new Color(1, 0, 0, 0.5f);
                                                                      // Main.Logger.Log("���");

            GameObject prefab = ActorMenu.instance.listActor; // �õ�������Ԥ�Ƽ�  ����������������������������������������������������������
            for (int i = 0; i < lineCount; i++) // һ�м���
            {
                GameObject go = UnityEngine.Object.Instantiate(prefab); // ����ÿ��������
                go.transform.SetParent(rItemCell, false); // ���ø�����
                Toggle tog = go.GetComponentInChildren<Toggle>();
                var tar = tog.targetGraphic;
                // Main.Logger.Log(tog + "����" + tog.graphic + " " + tog.graphic.transform.parent);
                DestroyImmediate(tog);
                Button btn = go.AddComponent<Button>();
                btn.targetGraphic = tar;
            }
            // Main.Logger.Log("���0");


            GridLayoutGroup gridLayoutGroup = gItemCell.AddComponent<GridLayoutGroup>();  // ����������
            gridLayoutGroup.cellSize = prefab.GetComponent<RectTransform>().sizeDelta; // �����������С
            gridLayoutGroup.spacing = new Vector2(0, 0); // ������߾�
            gridLayoutGroup.padding.left = (int)(12); // ��ƫ��
            gridLayoutGroup.padding.top = (int)(0); // ��ƫ��
            // Main.Logger.Log("���1");


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
                // Main.Logger.Log("=======����������=======��������"+count);

                bigDataScroll.cellCount = count;
                //if (!Main.OnChangeList)
                //{
                //    scrollRect.verticalNormalizedPosition = 1;
                //}
            }
        }

        private void SetCell(ItemCell itemCell, int index)
        {
            // Main.Logger.Log(index.ToString() + "���� itemCell������" + itemCell.ToString() + " pos=" + scrollRect.verticalNormalizedPosition.ToString());
            ActorItem item = itemCell as ActorItem;
            if (item == null)
            {
                // Main.Logger.Log("WarehouseItem��������");
                return;
            }
            // Main.Logger.Log("���ݳ��ȣ�" + m_data.Length);
            ChildData[] childDatas = item.childDatas;
            for (int i = 0; i < lineCount; i++)
            {
                int idx = (index - 1) * lineCount + i;
                // Main.Logger.Log("ѭ��" + i + "��ȡ�ڡ�" + idx + "����Ԫ�ص�����");
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
                        // Main.Logger.Log("����A��" + itemCell.name);
                        if (itemCell.transform.childCount > i)
                        {
                            var child = itemCell.transform.GetChild(i);
                            child.name = "Actor," + num2;
                            // Main.Logger.Log("����B��" + itemCell.name);
                            //Toggle tog = child.GetComponent<Toggle>();
                            //tog.group = ActorMenu.instance.listActorsHolder.GetComponent<ToggleGroup>();
                            // Main.Logger.Log(ActorMenu.instance.acotrId +" "+ num2 + "�ж��Ƿ�ѡ�У�" + (ActorMenu.instance.acotrId == num2));
                            //tog.isOn = ActorMenu.instance.acotrId == num2;
                            if (ActorMenu.instance.acotrId == num2 && ActorMenuActorListPatch.giveActorId == num2)
                            {
                                childData.Select(SelectState.All);
                            }
                            else if (ActorMenu.instance.acotrId == num2)
                            {
                                childData.Select(SelectState.Select);
                            }
                            else if (ActorMenuActorListPatch.giveActorId == num2)
                            {
                                childData.Select(SelectState.Give);
                            }
                            else
                            {
                                childData.Select(SelectState.Node);
                            }
                            Button btn = child.GetComponentInChildren<Button>();
                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(delegate ()
                            {
                                ActorMenu.instance.SetActorAttr(int.Parse(go.name.Split(',')[1]));
                            });
                        }


                        SetListActor component = childData.setListActor;
                        component.SetActor(num2);

                        // Main.Logger.Log("�˴���һЩ���� ��ͬ��ݵ��˴���ʽ��ͬ");
                        if (!ActorMenu.instance.isEnemy)
                        {
                            if (num2 == DateFile.instance.MianActorID())
                            {
                                // Main.Logger.Log("����");
                                component.SetInTeamIcon(true);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.acotrTeamDate.Contains(num2))
                            {
                                // Main.Logger.Log("�ǳ�ս��Ա");
                                component.SetInTeamIcon(true);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.ParseInt(DateFile.instance.GetActorDate(num2, 27, addValue: false)) == 1)
                            {
                                // Main.Logger.Log("��֪����ɶ");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                            else if (DateFile.instance.ActorIsWorking(num2) != null)
                            {
                                // Main.Logger.Log("���ڹ�����?");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(true);
                                component.RestMoodFace();
                            }
                            else
                            {
                                // Main.Logger.Log("����");
                                component.SetInTeamIcon(false);
                                component.SetInBuildingIcon(false);
                                component.RestMoodFace();
                            }
                        }
                        else
                        {
                            // Main.Logger.Log("����");
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
                    // Main.Logger.Log("���ݳ�������");
                }
                // Main.Logger.Log("�����������");
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
        //public class ActorItem : ItemCell
        //{

        //    public ChildData[] childDatas;
        //    public override void Awake()
        //    {
        //        base.Awake();
        //        childDatas = new ChildData[lineCount];
        //        for (int i = 0; i < lineCount; i++)
        //        {
        //            Transform child = transform.GetChild(i);
        //            childDatas[i] = new ChildData(child);
        //        }
        //        // Main.Logger.Log("WarehouseItem Awake " + childDatas.Length);
        //    }
        //}
        //public struct ChildData
        //{
        //    public GameObject gameObject;
        //    public SetListActor setListActor;
        //    public Text des;
        //    public Image select;


        //    public ChildData(Transform child)
        //    {
        //        // Main.Logger.Log("����һ��");
        //        gameObject = child.gameObject;
        //        // Main.Logger.Log("aaa");
        //        setListActor = gameObject.GetComponent<SetListActor>();

        //        // Main.Logger.Log("��ȡ" + setListActor);
        //        GameObject obj = GameObject.Instantiate<GameObject>(setListActor.listActorNameText.gameObject);
        //        // Main.Logger.Log("aaa");
        //        des = obj.GetComponent<Text>();
        //        // Main.Logger.Log("aaa");
        //        des.color = Color.red;
        //        // Main.Logger.Log("aaa");
        //        RectTransform tf = obj.GetComponent<RectTransform>();
        //        // Main.Logger.Log("aaa");
        //        tf.SetParent(gameObject.transform, false);
        //        // Main.Logger.Log("aaa");
        //        tf.sizeDelta = new Vector2(tf.sizeDelta.y, tf.sizeDelta.y);
        //        // Main.Logger.Log("aaa");
        //        tf.anchoredPosition = new Vector2(-30, 15);
        //        // Main.Logger.Log("aaa");

        //        foreach (var item in child)
        //        {
        //            // Main.Logger.Log("... " + item);
        //        }

        //        select = child.Find("ListActorLabel").GetComponent<Image>();
        //        des.text = "��";
        //    }

        //    public void Select(SelectState value)
        //    {
        //        select.enabled = (SelectState.Select & value) == SelectState.Select;
        //        des.enabled = (SelectState.Give & value) == SelectState.Give;
        //    }
        //}

        //public enum SelectState
        //{
        //    Node = 0,
        //    Select = 1,
        //    Give = 2,
        //    All = 3,
        //}
    }
}