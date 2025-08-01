using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] private WeaponData drillData;  // 무기 구매 시 데이터 넣기
    [SerializeField] private WeaponData pickaxeData;

    public static Shop instance;

    public Inventory Inventory;
    public Player player;
    public PlayerController playerController;

    public int shopView_idx;
    [SerializeField] private GameObject[] shopList;

    public TextMeshProUGUI shop_pickLvText;
    public TextMeshProUGUI shop_pickUpdateText;
    public TextMeshProUGUI shop_lightLvText;
    public TextMeshProUGUI shop_lightUpdateText;

    public GameObject playerlight;

    public float pick_damage;
    public float lightRadius;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded2;
        }
        else
        {
            Destroy(this.gameObject); // 중복 방지
        }
        pick_damage = playerController.pickdamage;
        lightRadius = playerlight.GetComponent<Light2D>().pointLightOuterRadius;

        Debug.Log(pick_damage);
        Debug.Log(lightRadius);
    }

    private void Start()
    {
        shopView_idx = 0;
        Switch_ShopView();
        
    }

    private void OnDestroy()
    {
        // 이벤트 제거 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded2;
    }

    private void OnSceneLoaded2(Scene scene, LoadSceneMode mode)
    {
        //if(scene.buildIndex != 2 || scene.buildIndex != 3) return;

        // 씬이 로드된 후 Player 다시 찾기
        player = FindObjectOfType<Player>();
        playerController = FindObjectOfType<PlayerController>();
        playerlight = GameObject.Find("Spot Light 2D");
        //Debug.Log(playerlight.gameObject.name);

        

        if (player != null)
        {
            Debug.Log("씬 전환 후 Player 연결 완료: " + player.name);
            playerController.pickdamage = pick_damage;
            playerlight.GetComponent<Light2D>().pointLightOuterRadius = lightRadius;
        }
        else
        {
            Debug.Log("씬 전환 후 Player를 찾지 못했습니다.");
        }
    }

    // 상점 이동 왼쪽
    public void Button_Left()
    {
        shopView_idx -= 1;
        if (shopView_idx < 0 )
        {
            shopView_idx = 2;
        }
        Switch_ShopView();
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[30]);

    }

    // 상점 이동 오른쪽
    public void Button_Right()
    {
        shopView_idx += 1;
        if (shopView_idx > 2)
        {
            shopView_idx = 0;
        }
        Switch_ShopView();
        SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[30]);

    }

    // 상점 구분 0 : 판매 / 1 : 구매 / 2 : 업그레이드
    private void Switch_ShopView()
    {
        switch (shopView_idx)
        {
            case 0:
                shopList[0].SetActive(true);
                shopList[1].SetActive(false);
                shopList[2].SetActive(false);
                break;
            case 1:
                shopList[0].SetActive(false);
                shopList[1].SetActive(true);
                shopList[2].SetActive(false);
                break;
            case 2:
                shopList[0].SetActive(false);
                shopList[1].SetActive(false);
                shopList[2].SetActive(true);
                break;
        }
    }


    // 광물 판매 버튼
    public void Button_Sell_Mineral_Coal()
    {
        Inventory.SellAllItem(player.minerals[0]);
    }
    public void Button_Sell_Mineral_Copper()
    {
        Inventory.SellAllItem(player.minerals[1]);
    }
    public void Button_Sell_Mineral_Iron()
    {
        Inventory.SellAllItem(player.minerals[2]);
    }
    public void Button_Sell_Mineral_Gold()
    {
        Inventory.SellAllItem(player.minerals[3]);
    }
    public void Button_Sell_Mineral_Ruby()
    {
        Inventory.SellAllItem(player.minerals[4]);
    }
    public void Button_Sell_Mineral_Diamond()
    {
        Inventory.SellAllItem(player.minerals[5]);
    }

    public void Button_All_Sell_Mineral_Coal()
    {
        Inventory.SellAllItem(player.minerals[0]);
    }


    //아이템 구매 버튼
    public void Button_Buy_Item_Bomb()
    {
        if(Inventory.money_item.count >= player.UseItems[0].value)
        {
            Inventory.money_item.count -= player.UseItems[0].value;
            Inventory.AddItem(player.UseItems[0], 1);
            SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[33]);

        }
        else
        {
            Inventory.LogMessage("돈이 부족합니다");
        }

    }
    public void Button_Buy_Item_Torch()
    {
        if (Inventory.money_item.count >= player.UseItems[1].value)
        {
            Inventory.money_item.count -= player.UseItems[1].value;
            Inventory.AddItem(player.UseItems[1], 1);
            SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[33]);

        }
        else
        {
            Inventory.LogMessage("돈이 부족합니다");
        }

    }

    // 업그레이드 버튼
    public void Button_Upgrade_Pick()
    {
        if(Inventory.money_item.count >= player.UpgradeItems[0].value)
        {
            Inventory.money_item.count -= player.UpgradeItems[0].value;
            pick_damage += 0.4f;
            WeaponBase weapon = player.GetComponentInChildren<WeaponBase>();
            
            if(weapon != null && weapon is Pickaxe)
            {
                weapon.DigSpeed = pick_damage; 
            }

            //playerController.pickdamage = pick_damage;
            player.UpgradeItems[0].count++;
            shop_pickLvText.text = "레벨 : " + player.UpgradeItems[0].count;
            player.UpgradeItems[0].value += 10;
            shop_pickUpdateText.text = "-" + player.UpgradeItems[0].value.ToString();

            Debug.Log("해당 무기의 데미지: " + weapon.DigSpeed);
            Inventory.FreshSlot();

            SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[31]);
        }
        else
        {
            Inventory.LogMessage("돈이 부족합니다");
        }
    }
    public void Button_Upgrade_EyeLight()
    {
        if (Inventory.money_item.count >= player.UpgradeItems[1].value)
        {
            Inventory.money_item.count -= player.UpgradeItems[1].value;
            lightRadius += 0.1f;
            playerlight.GetComponent<Light2D>().pointLightOuterRadius = lightRadius;
            player.UpgradeItems[1].count++;
            shop_lightLvText.text = "레벨 : " + player.UpgradeItems[1].count;
            player.UpgradeItems[1].value += 50;
            shop_lightUpdateText.text = "-" + player.UpgradeItems[1].value.ToString();

            Debug.Log(playerlight.GetComponent<Light2D>().pointLightOuterRadius);
            Inventory.FreshSlot();

            SoundManager.Instance.SFXPlay(SoundManager.Instance.SFXSounds[31]);

        }
        else
        {
            Inventory.LogMessage("돈이 부족합니다");
        }
    }

}
