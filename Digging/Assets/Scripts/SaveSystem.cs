using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    public Inventory inventory;

    public Collection collection;

    public Shop shop;

    public Player player;

    public PlayerController playerController;

    public LoadScene loadSceneScript;

    public LevelManager levelManager;

    public BlocksDictionary blocksDictionary;

    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject enemyPrefab1;

    public static SaveSystem Instance;
    private string savePath => Application.persistentDataPath + "/SaveData.json";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Save()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                print("인벤토리를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (collection == null)
        {
            collection = FindObjectOfType<Collection>();
            if (collection == null)
            {
                print("도감을 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (shop == null)
        {
            shop = FindObjectOfType<Shop>();
            if (shop == null)
            {
                print("상점을 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player == null)
            {
                print("플레이어를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (playerController == null)
        {
            playerController= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (playerController == null)
            {
                print("플레이어 컨트롤러를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (loadSceneScript == null)
        {
            loadSceneScript = FindObjectOfType<LoadScene>();
            if (loadSceneScript == null)
            {
                print("로드씬을 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if(levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if(levelManager == null)
            {
                print("레벨매니저를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if(blocksDictionary == null)
        {
            blocksDictionary = FindObjectOfType<BlocksDictionary>();
            if(blocksDictionary == null)
            {
                print("블록딕셔너리를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }


        var saveData = new SaveData();


        //인벤토리 저장
        saveData.inventoryData = new InventoryData
        {
            items = ConvertItemList(inventory.items),
            money_item = ConvertItem(inventory.money_item)
        };

        //도감 저장
        saveData.collectionData = new CollectionData
        {
            collect_sum = collection.collect_sum,
            player_lv = collection.player_lv,
            li_isCollect = collection.li_isCollect,
            li_isRelicOnTable = collection.li_isRelicOnTable
        };

        //상점 저장
        saveData.shopData = new ShopData
        {
            pick_damage = shop.pick_damage,
            lightRadius = shop.lightRadius
        };

        //플레이어 저장
        saveData.playerData = new PlayerData
        {
            items = ConvertItemList(player.items),
            minerals = ConvertItemList(player.minerals),
            UseItems = ConvertItemList(player.UseItems),
            UpgradeItems = ConvertItemList(player.UpgradeItems)
        };

        //플레이어컨트롤러 저장
        saveData.playerControllerData = new PlayerControllerData
        {
            pickDamage = playerController.pickdamage
        };

        //로드씬데이터 저장
        saveData.loadSceneData = new LoadSceneData
        {
            isAlreadyWatchStory = loadSceneScript.isAlreadyWatchStory,
            stage_Level = loadSceneScript.stage_Level
};

        //레벨매니저 저장
        saveData.levelManageData = new LevelManageData
        {
            remainingTime = levelManager.remainingTime
        };
        

        //블록들 저장
        saveData.blocksData = new BlocksData
        {
            blockDatas = ConvertBlockList(blocksDictionary)
        };

        //몬스터정보 저장
        Enemy[] enemys = FindObjectsOfType<Enemy>();
        if (enemys.Length != 0)
        {
            List<Vector2> enemyPosition = new List<Vector2>();
            foreach (Enemy enemy in enemys)
            {
                GameObject obj = enemy.gameObject;
                enemyPosition.Add(obj.transform.position);
            }

            saveData.enemyData = new EnemyData
            {
                EnemyDatas = enemyPosition,
            };
        }

        //저장할 파일을 json형태로 변경 및 위에서 선언한 savePath경로상에 저장
        string jsonForSave = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, jsonForSave);
        print("저장완료");

    }

    public void Load()
    {
        if(!File.Exists(savePath))
        {
            print("저장된 파일이 없습니다");
            return;
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                print("인벤토리를 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (collection == null)
        {
            collection = FindObjectOfType<Collection>();
            if (collection == null)
            {
                print("도감을 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (shop == null)
        {
            shop = FindObjectOfType<Shop>();
            if (shop == null)
            {
                print("상점을 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player == null)
            {
                print("플레이어를 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (playerController == null)
            {
                print("플레이어 컨트롤러를 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (loadSceneScript == null)
        {
            loadSceneScript = FindObjectOfType<LoadScene>();
            if (loadSceneScript == null)
            {
                print("로드씬을 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }

        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                print("레벨매니저를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        if (blocksDictionary == null)
        {
            blocksDictionary = FindObjectOfType<BlocksDictionary>();
            if (blocksDictionary == null)
            {
                print("블록딕셔너리를 참조할 수 없어 세이브에 실패하였습니다");
                return;
            }
        }

        //로드 시작
        string jsonForLoad = File.ReadAllText(savePath);
        SaveData loaded = JsonUtility.FromJson<SaveData>(jsonForLoad);

        //인벤토리 로드
        inventory.items = CreateItemList(loaded.inventoryData.items);
        ApplyItem(inventory.money_item, loaded.inventoryData.money_item);

        //플레이어 로드
        ApplyItemList(player.items, loaded.playerData.items);
        ApplyItemList(player.minerals, loaded.playerData.minerals);
        ApplyItemList(player.UseItems, loaded.playerData.UseItems);
        ApplyItemList(player.UpgradeItems, loaded.playerData.UpgradeItems);

        //도감 로드
        collection.collect_sum = loaded.collectionData.collect_sum;
        collection.player_lv = loaded.collectionData.player_lv;
        collection.li_isCollect = loaded.collectionData.li_isCollect;
        collection.li_isRelicOnTable = loaded.collectionData.li_isRelicOnTable;

        //상점 로드
        shop.pick_damage = loaded.shopData.pick_damage;
        shop.lightRadius = loaded.shopData.lightRadius;

        //플레이어컨트롤러 로드
        playerController.pickdamage = loaded.playerControllerData.pickDamage;

        //로드씬 로드
        loadSceneScript.isAlreadyWatchStory = loaded.loadSceneData.isAlreadyWatchStory;
        loadSceneScript.stage_Level = loaded.loadSceneData.stage_Level;

        //레벨매니저 로드
        levelManager.remainingTime = loaded.levelManageData.remainingTime;

        //블록들 로드
        HashSet<Vector2> loadedBlockPos = new HashSet<Vector2>(loaded.blocksData.blockDatas.Select(data => new Vector2(data.blockPosition.x, data.blockPosition.y)));

        List<GameObject> blocksToRemove = new List<GameObject>();
        foreach(var pair in blocksDictionary.blockPosition)
        {
            Vector2 pos = pair.Key;
            GameObject obj = pair.Value;
            Block blockScript = obj.GetComponent<Block>();
            if(blockScript != null)
            {
                if (blockScript.blockType != -2 && !loadedBlockPos.Contains(pos))
                {
                    blocksToRemove.Add(obj);
                }
            }
        }
        foreach (GameObject obj in blocksToRemove)
        {
            Block blockScript = obj.GetComponent<Block>();
            if(blockScript != null)
            {
                blockScript.BlockRemove();
            }    
        }

        foreach (BlockData blockData in loaded.blocksData.blockDatas)
        {
            GameObject obj;
            if (blocksDictionary.blockPosition.ContainsKey(blockData.blockPosition))
            {
                obj = blocksDictionary.blockPosition[blockData.blockPosition];
                if (obj != null)
                {
                    Block blockScript = obj.GetComponent<Block>();

                    if (blockScript != null)
                    {
                        blockScript.nowBlockType = blockData.nowBlockType;
                        blockScript.stageNum = blockData.stageNum;
                        blockScript.ChangeBlock(blockData.blockType);
                        blockScript.blockHealth = blockData.blockHealth;
                    }
                }
            }
            else
                print($"잘못된 키 ({blockData.blockPosition})");
        }

        //foreach (BlockData blockData in loaded.blocksData.blockDatas)
        //{
        //    GameObject newBlock = Instantiate(blockPrefab);
        //    Block blockScript = newBlock.GetComponent<Block>();

        //    newBlock.transform.position = blockData.blockPosition;
        //    blocksDictionary.blockPosition.Add(blockData.blockPosition, newBlock);
        //    blockScript.blocksDictionary = blocksDictionary;
        //    BlockBreakingEffectManager effectManager = FindObjectOfType<BlockBreakingEffectManager>();
        //    if (effectManager != null)
        //        blockScript.effectManager = effectManager;
        //    blockScript.nowBlockType = blockData.nowBlockType;
        //    blockScript.stageNum = blockData.stageNum;
        //    blockScript.ChangeBlock(blockData.blockType);
        //    blockScript.blockHealth = blockData.blockHealth;

        //}

        //몬스터들 로드
        Enemy[] enemys = FindObjectsOfType<Enemy>();
        for (int i = enemys.Length - 1; i >= 0; i--)
            enemys[i].EnemyDie();

        if (loaded.enemyData.EnemyDatas.Count > 0)
        {
            foreach(Vector2 enemyPos in loaded.enemyData.EnemyDatas)
            {
                Instantiate(enemyPrefab1, enemyPos, Quaternion.identity);
            }
        }
        

        //UI재갱신(아마도 도감도 갱신 넣어야할 예정)
        inventory.FreshSlot();
        print("로드완료");



    }

    public void LoadForLoadScene()
    {
        if (!File.Exists(savePath))
        {
            print("저장된 파일이 없습니다");
            return;
        }


        if (loadSceneScript == null)
        {
            loadSceneScript = FindObjectOfType<LoadScene>();
            if (loadSceneScript == null)
            {
                print("로드씬을 참조할 수 없어 로드에 실패하였습니다");
                return;
            }
        }


        //로드 시작
        string jsonForLoad = File.ReadAllText(savePath);
        SaveData loaded = JsonUtility.FromJson<SaveData>(jsonForLoad);

        loadSceneScript.isAlreadyWatchStory = loaded.loadSceneData.isAlreadyWatchStory;
        loadSceneScript.stage_Level = loaded.loadSceneData.stage_Level;
    }



    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            print("세이브 파일이 삭제되었습니다.");
        }
        else
        {
            print("세이브 파일이 존재하지 않습니다.");
        }
    }


    private List<ItemData> ConvertItemList(List<Item> itemList)
    {
        var result = new List<ItemData>();

        foreach(var item in itemList)
        {
            result.Add(ConvertItem(item));
        }

        return result;
    }
    
    private ItemData ConvertItem(Item item)
    {
        var result = new ItemData();

        result.itemName = item.itemName;
        result.count = item.count;
        result.accumulation_count = item.accumulation_count;
        result.isMineral = item.isMineral;
        result.isRelic = item.isRelic;
        result.value = item.value;
        result.ishaveitem = item.ishaveitem;
        result.isalreadySell = item.isalreadySell;

        return result;
    }

    private void ApplyItemList(List<Item> itemList, List<ItemData> dataList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            ApplyItem(itemList[i], dataList[i]);
        }
    }

    private void ApplyItem(Item item, ItemData data)
    {
        item.count = data.count;
        item.accumulation_count = data.accumulation_count;
        item.value = data.value;
        item.ishaveitem = data.ishaveitem;
        item.isalreadySell = data.isalreadySell;
    }

    private List<Item> CreateItemList(List<ItemData> dataList)
    {
        var result = new List<Item>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        if(player == null)
        {
            print("플레이어가 없어 아이템 정보를 불러올 수 없습니다.");
        }
        else
        {
            foreach (var data in dataList)
            {
                if (data.isMineral)
                {
                    for(int i = 0; i < player.minerals.Count; i++)
                    {
                        if (data.itemName == player.minerals[i].itemName)
                        {
                            result.Add(player.minerals[i]);
                            break;
                        }
                    }
                }
                else if(data.isRelic)
                {
                    for (int i = 0; i < player.items.Count; i++)
                    {
                        if (data.itemName == player.items[i].itemName)
                        {
                            result.Add(player.items[i]);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < player.UseItems.Count; i++)
                    {
                        if (data.itemName == player.UseItems[i].itemName)
                        {
                            result.Add(player.UseItems[i]);
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }

    private List<BlockData> ConvertBlockList(BlocksDictionary blocksDictionary)
    {
        var result = new List<BlockData>();

        foreach(GameObject obj in blocksDictionary.blockPosition.Values)
        {
            Block blockScript = obj.GetComponent<Block>();
            if (blockScript != null)
            {
                if(blockScript.blockType != -2)
                {
                    result.Add(ConvertBlock(obj, blockScript));
                }
            }
        }

        return result;
    }

    private BlockData ConvertBlock(GameObject obj, Block blockScript)
    {
        var result = new BlockData();

        result.blockPosition = obj.transform.position;
        result.nowBlockType = blockScript.nowBlockType;
        result.blockType = blockScript.blockType;
        result.stageNum = blockScript.stageNum;
        result.blockHealth = blockScript.blockHealth;

        return result;
    }
}
