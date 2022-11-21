using UnityEngine;
using TMPro;
using Mirror;
using Core.Contracts;
using System.Collections;
using Core.Castle;
using UnityEngine.SceneManagement;
using Effects;

namespace Core.Client
{
    public class BattleUI : MonoBehaviour
    {
        public static BattleUI instanse;

        [Header("Scenes")]
        [Scene]
        public string menuScene;

        [Header("Sounds")]
        public AudioClip damageCastleSound;
        public AudioClip healCastleSound;
        public AudioClip upIncomeSound;
        public AudioClip downIncomeSound;

        [Header("Text objects")]
        public TextMeshProUGUI myNickname;
        public TextMeshProUGUI enemyNickname;
        public TextMeshProUGUI textHealthMyTower;
        public TextMeshProUGUI textHealthEnemyTower;
        public TextMeshProUGUI textHealthMyWall;
        public TextMeshProUGUI textHealthEnemyWall;
        public TextMeshProUGUI myResourceValue_1;
        public TextMeshProUGUI myResourceIncome_1;
        public TextMeshProUGUI myResourceValue_2;
        public TextMeshProUGUI myResourceIncome_2;
        public TextMeshProUGUI myResourceValue_3;
        public TextMeshProUGUI myResourceIncome_3;
        public TextMeshProUGUI enemyResourceValue_1;
        public TextMeshProUGUI enemyResourceIncome_1;
        public TextMeshProUGUI enemyResourceValue_2;
        public TextMeshProUGUI enemyResourceIncome_2;
        public TextMeshProUGUI enemyResourceValue_3;
        public TextMeshProUGUI enemyResourceIncome_3;
        public TextMeshProUGUI timer;
        public TextMeshProUGUI tipsText;
        public TextMeshProUGUI textTurn;
        [SerializeField]
        private GameObject textFatigueNextDamage;
        [SerializeField]
        private GameObject myTurn;
        [SerializeField]
        private GameObject opponentsTurn;

        [Space(10)]
        [Header("Game objects")]
        public GameObject myTower;
        public GameObject enemyTower;
        public GameObject myWall;
        public GameObject enemyWall;
        public GameObject winWindow;
        public GameObject loseWindow;
        public GameObject drawWindow;
        public GameObject tipsWindow;
        public GameObject helpWindow;
        public GameObject waitStartWindow;
        public GameObject skipTurnButton;

        [Space(10)]
        [Header("Height settings")]
        [SerializeField]
        private float maxTowerHeight;
        [SerializeField]
        private float minTowerHeight;
        [SerializeField]
        private float maxWallHeight;
        [SerializeField]
        private float minWallHeight;
        [SerializeField]
        private float numberSize;

        private Vector3 _startTipsPosition;
        private Vector3 _endTipsPosition;

        private Coroutine myTowerCoroutine;
        private Coroutine enemyTowerCoroutine;
        private Coroutine myWallCoroutine;
        private Coroutine enemyWallCoroutine;
        private Coroutine animationTipsCoroutine;

        private AudioSource _audioSource;

        private Canvas _canvas;

        public static void ActivateFatigueDamageText()
        {
            instanse.textFatigueNextDamage.SetActive(true);
        }

        public static void SetTextFatigueDamage(int damage)
        {
            instanse.textFatigueNextDamage.GetComponent<TextMeshProUGUI>().text = $"NEXT NECRO'S HIT DAMAGE: {damage}";
        }

        public static void SetTextMyTurn()
        {
            instanse.opponentsTurn.SetActive(false);
            instanse.myTurn.SetActive(true);
        }

        public static void SetTextEnemyTurn()
        {
            instanse.myTurn.SetActive(false);
            instanse.opponentsTurn.SetActive(true);
        }

        public static void RemoveMyResourceValue(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationRemoveMyResourceValue(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.downIncomeSound);
        }

        public static void AddMyResourceValue(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationAddMyResourceValue(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.upIncomeSound);
        }

        public static void RemoveEnemyResourceValue(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationRemoveEnemyResourceValue(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.downIncomeSound);
        }

        public static void AddEnemyResourceValue(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationAddEnemyResourceValue(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.upIncomeSound);
        }

        public static void RemoveMyResourceIncome(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationRemoveMyResourceIncome(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.downIncomeSound);
        }

        public static void AddMyResourceIncome(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationAddMyResourceIncome(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.upIncomeSound);
        }

        public static void RemoveEnemyResourceIncome(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationRemoveEnemyResourceIncome(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.downIncomeSound);
        }

        public static void AddEnemyResourceIncome(string resourceName, int value)
        {
            instanse.StartCoroutine(instanse.AnimationAddEnemyResourceIncome(resourceName, value));
            instanse._audioSource.PlayOneShot(instanse.upIncomeSound);
        }

        public static void DamageEnemyTower(int damage)
        {
            instanse.StartCoroutine(instanse.AnimationDamageEnemyTower(damage));
            instanse._audioSource.PlayOneShot(instanse.damageCastleSound);
        }

        public static void HealEnemyTower(int heal)
        {
            instanse.StartCoroutine(instanse.AnimationHealEnemyTower(heal));
            instanse._audioSource.PlayOneShot(instanse.healCastleSound);
        }

        public static void DamageEnemyWall(int damage)
        {
            instanse.StartCoroutine(instanse.AnimationDamageEnemyWall(damage));
            instanse._audioSource.PlayOneShot(instanse.damageCastleSound);
        }
        
        public static void StartNextTurn()
        {
            if (BattleClientManager.IsCanPlay())
            {
                BattleClientManager.instance.CheckEndMatch();
                BattleClientManager.SetCanPlay(false);               
                MatchClientController.EndTurn();
            }
        }

        public static void HealEnemyWall(int heal)
        {
            instanse.StartCoroutine(instanse.AnimationHealEnemyWall(heal));
            instanse._audioSource.PlayOneShot(instanse.healCastleSound);
        }

        public static void DamageMyTower(int damage)
        {
            instanse.StartCoroutine(instanse.AnimationDamageMyTower(damage));
            instanse._audioSource.PlayOneShot(instanse.damageCastleSound);
        }

        public static void HealMyTower(int heal)
        {
            instanse.StartCoroutine(instanse.AnimationHealMyTower(heal));
            instanse._audioSource.PlayOneShot(instanse.healCastleSound);
        }

        public static void DamageMyWall(int damage)
        {
            instanse.StartCoroutine(instanse.AnimationDamageMyWall(damage));
            instanse._audioSource.PlayOneShot(instanse.damageCastleSound);
        }

        public static void HealMyWall(int heal)
        {
            instanse.StartCoroutine(instanse.AnimationHealMyWall(heal));
            instanse._audioSource.PlayOneShot(instanse.healCastleSound);
        }

        public static void ShowTipsWindow(string text)
        {
            if (instanse.animationTipsCoroutine != null)
                instanse.StopCoroutine(instanse.animationTipsCoroutine);

            instanse.animationTipsCoroutine = instanse.StartCoroutine(instanse.AnimationShowTips());
            instanse.tipsText.text = text;
        }

        public static void HideTipsWindow()
        {
            if (instanse.animationTipsCoroutine != null)
                instanse.StopCoroutine(instanse.animationTipsCoroutine);

            instanse.animationTipsCoroutine = instanse.StartCoroutine(instanse.AnimationHideTips());
        }

        public static void HideWaitStartWindow()
        {
            instanse.waitStartWindow.SetActive(false);
        }

        public void ShowWinWindow()
        {
            if (!BattleClientManager.IsMatchEnded())
                instanse.winWindow.SetActive(true);
        }

        public void ShowLoseWindow()
        {
            if (!BattleClientManager.IsMatchEnded())
                instanse.loseWindow.SetActive(true);
        }

        public void ShowDrawWindow()
        {
            if (!BattleClientManager.IsMatchEnded())
                instanse.drawWindow.SetActive(true);
        }

        public void ExitToMenu()
        {
            SceneManager.LoadScene(menuScene);
            StopAllCoroutines();
        }

        private void Awake()
        {
            instanse = this;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _canvas = FindObjectOfType<Canvas>();
            _startTipsPosition = tipsWindow.transform.localPosition;
            _endTipsPosition = tipsWindow.transform.localPosition + new Vector3(0, 70f, 0);

            NetworkClient.Send(new RequestBattleInfo
            {
                AccountId = MainClient.GetClientId()
            });

            MatchClientController.matchWin.AddListener(() => Invoke(nameof(ShowWinWindow), 5f));
            MatchClientController.matchLose.AddListener(() => Invoke(nameof(ShowLoseWindow), 5f));
            MatchClientController.matchDraw.AddListener(() => Invoke(nameof(ShowDrawWindow), 5f));
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                helpWindow.SetActive(!helpWindow.activeSelf);
        }

        private void FixedUpdate()
        {
            instanse.timer.text = FormatTime(BattleClientManager.GetTimeLeft());
        }

        private IEnumerator AnimationRemoveMyResourceValue(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetMyData().Castle.GetResource(resourceName);

            resource.RemoveResource(value);

            switch (resourceName)
            {
                case "Resource_1":
                    myResourceValue_1.text = resource.Value.ToString();
                    EffectSpawner.instance.myDownResource_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_1, value, false));
                    break;
                case "Resource_2":
                    myResourceValue_2.text = resource.Value.ToString();
                    EffectSpawner.instance.myDownResource_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_2, value, false));
                    break;
                case "Resource_3":
                    myResourceValue_3.text = resource.Value.ToString();
                    EffectSpawner.instance.myDownResource_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_3, value, false));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationAddMyResourceValue(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetMyData().Castle.GetResource(resourceName);

            resource.AddResource(value);

            switch (resourceName)
            {
                case "Resource_1":
                    myResourceValue_1.text = resource.Value.ToString();
                    EffectSpawner.instance.myUpResource_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_1, value));
                    break;
                case "Resource_2":
                    myResourceValue_2.text = resource.Value.ToString();
                    EffectSpawner.instance.myUpResource_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_2, value));
                    break;
                case "Resource_3":
                    myResourceValue_3.text = resource.Value.ToString();
                    EffectSpawner.instance.myUpResource_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceValue_3, value));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationRemoveEnemyResourceValue(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetEnemyData().Castle.GetResource(resourceName);

            resource.RemoveResource(value);

            switch (resourceName)
            {
                case "Resource_1":
                    enemyResourceValue_1.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyDownResource_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_1, value, false));
                    break;
                case "Resource_2":
                    enemyResourceValue_2.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyDownResource_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_2, value, false));
                    break;
                case "Resource_3":
                    enemyResourceValue_3.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyDownResource_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_3, value, false));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationAddEnemyResourceValue(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetEnemyData().Castle.GetResource(resourceName);

            resource.AddResource(value);

            switch (resourceName)
            {
                case "Resource_1":
                    enemyResourceValue_1.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyUpResource_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_1, value));
                    break;
                case "Resource_2":
                    enemyResourceValue_2.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyUpResource_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_2, value));
                    break;
                case "Resource_3":
                    enemyResourceValue_3.text = resource.Value.ToString();
                    EffectSpawner.instance.enemyUpResource_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceValue_3, value));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationRemoveMyResourceIncome(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetMyData().Castle.GetResource(resourceName);

            resource.RemoveIncome(value);

            switch (resourceName)
            {
                case "Resource_1":
                    myResourceIncome_1.text = resource.Income.ToString();
                    EffectSpawner.instance.myDownIncome_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_1, value, false));
                    break;
                case "Resource_2":
                    myResourceIncome_2.text = resource.Income.ToString();
                    EffectSpawner.instance.myDownIncome_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_2, value, false));
                    break;
                case "Resource_3":
                    myResourceIncome_3.text = resource.Income.ToString();
                    EffectSpawner.instance.myDownIncome_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_3, value, false));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationAddMyResourceIncome(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetMyData().Castle.GetResource(resourceName);

            resource.AddIncome(value);

            switch (resourceName)
            {
                case "Resource_1":
                    myResourceIncome_1.text = resource.Income.ToString();
                    EffectSpawner.instance.myUpIncome_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_1, value));
                    break;
                case "Resource_2":
                    myResourceIncome_2.text = resource.Income.ToString();
                    EffectSpawner.instance.myUpIncome_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_2, value));
                    break;
                case "Resource_3":
                    myResourceIncome_3.text = resource.Income.ToString();
                    EffectSpawner.instance.myUpIncome_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(myResourceIncome_3, value));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationRemoveEnemyResourceIncome(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetEnemyData().Castle.GetResource(resourceName);

            resource.RemoveIncome(value);

            switch (resourceName)
            {
                case "Resource_1":
                    enemyResourceIncome_1.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyDownIncome_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_1, value, false));
                    break;
                case "Resource_2":
                    enemyResourceIncome_2.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyDownIncome_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_2, value, false));
                    break;
                case "Resource_3":
                    enemyResourceIncome_3.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyDownIncome_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_3, value, false));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationAddEnemyResourceIncome(string resourceName, int value)
        {
            Resource resource = BattleClientManager.GetEnemyData().Castle.GetResource(resourceName);

            resource.AddIncome(value);

            switch (resourceName)
            {
                case "Resource_1":
                    enemyResourceIncome_1.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyUpIncome_1.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_1, value));
                    break;
                case "Resource_2":
                    enemyResourceIncome_2.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyUpIncome_2.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_2, value));
                    break;
                case "Resource_3":
                    enemyResourceIncome_3.text = resource.Income.ToString();
                    EffectSpawner.instance.enemyUpIncome_3.Play();
                    instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(enemyResourceIncome_3, value));
                    break;
            }

            yield return null;
        }

        private IEnumerator AnimationDamageEnemyTower(int damage)
        {
            BattleClientManager.GetEnemyData().Castle.Tower.Damage(damage);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthEnemyTower, damage, false));

            float moveForOneHealth = (maxTowerHeight - minTowerHeight) / BattleClientManager.GetEnemyData().Castle.Tower.MaxHealth;

            Vector3 targetPosition = new Vector3(
                enemyTower.transform.localPosition.x,
                minTowerHeight + moveForOneHealth * BattleClientManager.GetEnemyData().Castle.Tower.Health,
                enemyTower.transform.localPosition.z);

            if (enemyTowerCoroutine != null)
                StopCoroutine(enemyTowerCoroutine);

            enemyTowerCoroutine = StartCoroutine(AnimationMoveCastleObject(enemyTower, targetPosition));
            textHealthEnemyTower.text = BattleClientManager.GetEnemyData().Castle.Tower.Health.ToString();

            EffectSpawner.instance.enemyTowerDamage.Play();

            yield return null;
        }

        private IEnumerator AnimationHealEnemyTower(int heal)
        {
            BattleClientManager.GetEnemyData().Castle.Tower.Heal(heal);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthEnemyTower, heal));

            float moveForOneHealth = (maxTowerHeight - minTowerHeight) / BattleClientManager.GetEnemyData().Castle.Tower.MaxHealth;

            Vector3 targetPosition = new Vector3(
                enemyTower.transform.localPosition.x,
                minTowerHeight + moveForOneHealth * BattleClientManager.GetEnemyData().Castle.Tower.Health,
                enemyTower.transform.localPosition.z);

            if (enemyTowerCoroutine != null)
                StopCoroutine(enemyTowerCoroutine);

            enemyTowerCoroutine = StartCoroutine(AnimationMoveCastleObject(enemyTower, targetPosition));
            textHealthEnemyTower.text = BattleClientManager.GetEnemyData().Castle.Tower.Health.ToString();

            EffectSpawner.instance.enemyTowerHeal.Play();

            yield return null;
        }

        private IEnumerator AnimationDamageEnemyWall(int damage)
        {
            int wallHealth = BattleClientManager.GetEnemyData().Castle.Wall.Health;

            BattleClientManager.GetEnemyData().Castle.Wall.Damage(damage);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthEnemyWall,
                (BattleClientManager.GetEnemyData().Castle.Wall.Health > 0) ? damage : wallHealth,
                false));

            float moveForOneHealth = (maxWallHeight - minWallHeight) / BattleClientManager.GetEnemyData().Castle.Wall.MaxHealth;

            Vector3 targetPosition = new Vector3(
                enemyWall.transform.localPosition.x,
                minWallHeight + moveForOneHealth * BattleClientManager.GetEnemyData().Castle.Wall.Health,
                enemyWall.transform.localPosition.z);

            if (enemyWallCoroutine != null)
                StopCoroutine(enemyWallCoroutine);

            enemyWallCoroutine = StartCoroutine(AnimationMoveCastleObject(enemyWall, targetPosition));
            textHealthEnemyWall.text = BattleClientManager.GetEnemyData().Castle.Wall.Health.ToString();

            EffectSpawner.instance.enemyWallDamage.Play();

            yield return null;
        }

        private IEnumerator AnimationHealEnemyWall(int heal)
        {
            BattleClientManager.GetEnemyData().Castle.Wall.Heal(heal);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthEnemyWall, heal));

            float moveForOneHealth = (maxWallHeight - minWallHeight) / BattleClientManager.GetEnemyData().Castle.Wall.MaxHealth;

            Vector3 targetPosition = new Vector3(
                enemyWall.transform.localPosition.x,
                minWallHeight + moveForOneHealth * BattleClientManager.GetEnemyData().Castle.Wall.Health,
                enemyWall.transform.localPosition.z);

            if (enemyWallCoroutine != null)
                StopCoroutine(enemyWallCoroutine);

            enemyWallCoroutine = StartCoroutine(AnimationMoveCastleObject(enemyWall, targetPosition));
            textHealthEnemyWall.text = BattleClientManager.GetEnemyData().Castle.Wall.Health.ToString();

            EffectSpawner.instance.enemyWallHeal.Play();

            yield return null;
        }

        private IEnumerator AnimationDamageMyTower(int damage)
        {
            BattleClientManager.GetMyData().Castle.Tower.Damage(damage);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthMyTower, damage, false));

            float moveForOneHealth = (maxTowerHeight - minTowerHeight) / BattleClientManager.GetMyData().Castle.Tower.MaxHealth;

            Vector3 targetPosition = new Vector3(
                myTower.transform.localPosition.x,
                minTowerHeight + moveForOneHealth * BattleClientManager.GetMyData().Castle.Tower.Health,
                myTower.transform.localPosition.z);

            if (myTowerCoroutine != null)
                StopCoroutine(myTowerCoroutine);

            myTowerCoroutine = StartCoroutine(AnimationMoveCastleObject(myTower, targetPosition));
            textHealthMyTower.text = BattleClientManager.GetMyData().Castle.Tower.Health.ToString();

            EffectSpawner.instance.myTowerDamage.Play();

            yield return null;
        }

        private IEnumerator AnimationHealMyTower(int heal)
        {
            BattleClientManager.GetMyData().Castle.Tower.Heal(heal);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthMyTower, heal));

            float moveForOneHealth = (maxTowerHeight - minTowerHeight) / BattleClientManager.GetMyData().Castle.Tower.MaxHealth;

            Vector3 targetPosition = new Vector3(
                myTower.transform.localPosition.x,
                minTowerHeight + moveForOneHealth * BattleClientManager.GetMyData().Castle.Tower.Health,
                myTower.transform.localPosition.z);

            if (myTowerCoroutine != null)
                StopCoroutine(myTowerCoroutine);

            myTowerCoroutine = StartCoroutine(AnimationMoveCastleObject(myTower, targetPosition));
            textHealthMyTower.text = BattleClientManager.GetMyData().Castle.Tower.Health.ToString();

            EffectSpawner.instance.myTowerHeal.Play();

            yield return null;
        }

        private IEnumerator AnimationDamageMyWall(int damage)
        {
            int wallHealth = BattleClientManager.GetMyData().Castle.Wall.Health;

            BattleClientManager.GetMyData().Castle.Wall.Damage(damage);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthMyWall,
                (BattleClientManager.GetMyData().Castle.Wall.Health > 0) ? damage : wallHealth,
                false));

            float moveForOneHealth = (maxWallHeight - minWallHeight) / BattleClientManager.GetMyData().Castle.Wall.MaxHealth;

            Vector3 targetPosition = new Vector3(
                myWall.transform.localPosition.x,
                minWallHeight + moveForOneHealth * BattleClientManager.GetMyData().Castle.Wall.Health,
                myWall.transform.localPosition.z);

            if (myWallCoroutine != null)
                StopCoroutine(myWallCoroutine);

            myWallCoroutine = StartCoroutine(AnimationMoveCastleObject(myWall, targetPosition));
            textHealthMyWall.text = BattleClientManager.GetMyData().Castle.Wall.Health.ToString();

            EffectSpawner.instance.myWallDamage.Play();

            yield return null;
        }

        private IEnumerator AnimationHealMyWall(int heal)
        {
            BattleClientManager.GetMyData().Castle.Wall.Heal(heal);
            instanse.StartCoroutine(instanse.AnimationChangePlayerCastleValue(textHealthMyWall, heal));

            float moveForOneHealth = (maxWallHeight - minWallHeight) / BattleClientManager.GetMyData().Castle.Wall.MaxHealth;

            Vector3 targetPosition = new Vector3(
                myWall.transform.localPosition.x,
                minWallHeight + moveForOneHealth * BattleClientManager.GetMyData().Castle.Wall.Health,
                myWall.transform.localPosition.z);

            if (myWallCoroutine != null)
                StopCoroutine(myWallCoroutine);

            myWallCoroutine = StartCoroutine(AnimationMoveCastleObject(myWall, targetPosition));
            textHealthMyWall.text = BattleClientManager.GetMyData().Castle.Wall.Health.ToString();

            EffectSpawner.instance.myWallHeal.Play();

            yield return null;
        }

        private IEnumerator AnimationShowTips()
        {
            while (Vector3.Distance(tipsWindow.transform.localPosition, _endTipsPosition) > 0.1f)
            {
                tipsWindow.transform.localPosition = Vector3.Lerp(tipsWindow.transform.localPosition,
                    _endTipsPosition,
                    0.02f);

                yield return null;
            }

            tipsWindow.transform.localPosition = _endTipsPosition;
        }

        private IEnumerator AnimationHideTips()
        {
            while (Vector3.Distance(tipsWindow.transform.localPosition, _startTipsPosition) > 0.1f)
            {
                tipsWindow.transform.localPosition = Vector3.Lerp(tipsWindow.transform.localPosition,
                    _startTipsPosition,
                    0.02f);

                yield return null;
            }

            tipsWindow.transform.localPosition = _startTipsPosition;
        }

        private IEnumerator AnimationMoveCastleObject(GameObject objectMove, Vector3 targetPosition)
        {
            while (Vector3.Distance(objectMove.transform.localPosition, targetPosition) > 0.1f)
            {
                objectMove.transform.localPosition = Vector3.Lerp(objectMove.transform.localPosition,
                    targetPosition,
                    0.02f);

                yield return null;
            }
        }

        private IEnumerator AnimationChangePlayerCastleValue(TextMeshProUGUI resourceText, int value, bool isPositive = true)
        {
            if (value == 0)
                yield break;

            var duplicateText = Instantiate(resourceText, resourceText.transform.position, Quaternion.identity, _canvas.transform);

            duplicateText.transform.localScale *= numberSize;

            var targetPosition = new Vector3(
                duplicateText.transform.localPosition.x,
                duplicateText.transform.localPosition.y + 100f,
                duplicateText.transform.localPosition.z);

            duplicateText.transform.SetAsLastSibling();

            if (isPositive)
            {
                duplicateText.text = $"+{value}";
                duplicateText.color = Color.green;
            }
            else
            {
                duplicateText.text = $"-{value}";
                duplicateText.color = Color.red;
            }

            while (Vector3.Distance(duplicateText.transform.localPosition, targetPosition) > 20f)
            {
                duplicateText.transform.localPosition = Vector3.Lerp(duplicateText.transform.localPosition,
                    targetPosition, 0.01f);

                yield return new WaitForEndOfFrame();
            }

            Destroy(duplicateText.gameObject);
        }

        private string FormatTime(float time)
        {
            var hours = (int)time / 60000;
            var minutes = (int)time / 1000 - 60 * hours;
            var seconds = (int)time - hours * 60000 - 1000 * minutes;

            return string.Format($"{minutes.ToString("00")}:{seconds.ToString("00")}");
        }
    }
}