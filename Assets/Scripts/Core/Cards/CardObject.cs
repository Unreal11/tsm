using Core.Cards.Effects;
using Core.Castle;
using Core.Client;
using Core.Contracts;
using Effects;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Cards
{
    public class CardObject : MonoBehaviour, IPointerEnterHandler, 
        IPointerExitHandler, IBeginDragHandler, 
        IEndDragHandler, IDragHandler, IPointerClickHandler
    {
        public static bool IsCardGrab;
        public static bool IsDiscardMode;

        #region variables
        [Header("Card data")]
        public CardData card;

        [Space(10)]
        [Header("Sounds setting")]
        public AudioClip playSound;
        public AudioClip discardSound;

        [Space(10)]
        [Header("Size settings")]
        [SerializeField]
        private float deltaSize = 0.05f;
        [SerializeField]
        private float defaultSize = 1.25f;
        [SerializeField]
        private float handleSize = 1.4f;

        [Space(10)]
        [Header("Rotate handle settings")]
        [SerializeField]
        private float rotateModificator = 0.1f;
        [SerializeField]
        private float limitAngle = 0.4f;
        [SerializeField]
        private float smoothRotate = 0.08f;

        [Space(10)]
        [Header("Move settings")]
        [SerializeField]
        private float smoothMove = 0.08f;
        [SerializeField]
        private RectTransform targetTransformPosition;

        private Canvas _canvas;
        private RectTransform _rectTransform;
        private Image _tableTopImage;
        private Quaternion _targetAngle;
        private Coroutine _sizeCoroutine;
        private bool _isDragging;
        private bool _canPlayed;
        private GameObject _mainAnimationPosition;
        private GameObject _deck;
        private GameObject _spawnEnemyCardPosition;
        private CardPosition _cardPosition;
        private AudioSource _audioSource;
        #endregion

        #region Methods
        public CardPosition GetCardPosition() => _cardPosition;

        public void Discard()
        {
            StartCoroutine(CardDiscardPlayAnimation());

            CardClientController.SendRequestCardAction(new RequestCardDto
            {
                AccountId = MainClient.GetClientId(),
                ActionType = CardActionType.RequestDiscard,
                CardId = Guid.Parse(card.Id)
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right 
                && BattleClientManager.IsMyTurn()
                && BattleClientManager.IsCanPlay()
                && !card.NonDiscard)
            {
                Discard();

                if (IsDiscardMode)
                {
                    IsDiscardMode = false;
                }
                else
                {
                    BattleClientManager.SetCanPlay(false);
                    MatchClientController.EndTurn();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsCardGrab)
            {
                if (_sizeCoroutine != null)
                    StopCoroutine(_sizeCoroutine);

                _sizeCoroutine = StartCoroutine(SizeUp(handleSize));
                transform.parent.SetAsLastSibling();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsCardGrab)
            {
                if (_sizeCoroutine != null)
                    StopCoroutine(_sizeCoroutine);

                _sizeCoroutine = StartCoroutine(SizeDown(defaultSize));

                if (_cardPosition != null)
                    _cardPosition.transform.SetSiblingIndex(_cardPosition.startPointerIndex);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_canPlayed && !IsDiscardMode)
            {
                if (_sizeCoroutine != null)
                    StopCoroutine(_sizeCoroutine);

                _sizeCoroutine = StartCoroutine(SizeUp(handleSize));
                _rectTransform.position -= Vector3.forward;
                _tableTopImage.raycastTarget = false;
                _isDragging = true;
                IsCardGrab = true;
                _cardPosition.transform.SetAsLastSibling();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_canPlayed && !IsDiscardMode)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
                _rectTransform.position = new Vector3(transform.position.x, transform.position.y, _rectTransform.position.z);
                _targetAngle = new Quaternion(
                    Mathf.Clamp(eventData.delta.y * rotateModificator, -limitAngle, limitAngle),
                    Mathf.Clamp(-eventData.delta.x * rotateModificator, -limitAngle, limitAngle),
                    _rectTransform.rotation.z,
                    _rectTransform.rotation.w);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_canPlayed && !IsDiscardMode)
            {
                if (_sizeCoroutine != null)
                    StopCoroutine(_sizeCoroutine);

                _sizeCoroutine = StartCoroutine(SizeDown(defaultSize));
                _tableTopImage.raycastTarget = true;
                _isDragging = false;
                IsCardGrab = false;

                _cardPosition.transform.SetSiblingIndex(_cardPosition.startIndex);

                if (DropZone.isPlayable && _canPlayed)
                    StartCoroutine(CardYouPlayAnimation());
            }
        }

        public void SetCanPlayed(bool canPlayed)
        {
            if (_isDragging && !BattleClientManager.IsMyTurn())
                OnEndDrag(null);

            if (_cardPosition != null)
            {
                _canPlayed = canPlayed;
                _tableTopImage.color = canPlayed
                    ? Color.white
                    : new Color(0.3f, 0.3f, 0.3f, 1f);
            }
            else
            {
                _canPlayed = false;
                _tableTopImage.color = Color.white;
            }

            if (IsDiscardMode)
                _tableTopImage.color = Color.white;
        }

        public bool HasResources()
        {
            foreach (Resource resource in card.Cost)
            {
                int countResource = BattleClientManager.GetMyData().Castle.Resources
                    .FirstOrDefault(r => r.Name == resource.Name).Value;

                if (countResource < resource.Value)
                    return false;
            }

            return true;
        }

        public IEnumerator SizeUp(float handleSize)
        {
            while (transform.localScale.x < handleSize)
            {
                transform.localScale += new Vector3(deltaSize, deltaSize, deltaSize);

                yield return new WaitForFixedUpdate();
            }

            transform.localScale = new Vector3(handleSize, handleSize, handleSize);
        }

        public IEnumerator SizeDown(float size)
        {
            while (transform.localScale.x > size)
            {
                transform.localScale -= new Vector3(deltaSize, deltaSize, deltaSize);

                yield return new WaitForFixedUpdate();
            }

            transform.localScale = new Vector3(size, size, size);
        }

        public IEnumerator CardYouPlayAnimation()
        {
            CardClientController.SendRequestCardAction(new RequestCardDto
            {
                AccountId = MainClient.GetClientId(),
                ActionType = CardActionType.RequestPlay,
                CardId = Guid.Parse(card.Id)
            });

            BattleClientManager.SetCanPlay(false);

            yield return new WaitForEndOfFrame();

            foreach (Resource resource in card.Cost)
                BattleUI.RemoveMyResourceValue(resource.Name, resource.Value);

            _cardPosition.card = null;
            _cardPosition = null;
            _tableTopImage.raycastTarget = false;
            targetTransformPosition = _mainAnimationPosition.GetComponent<RectTransform>();

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeUp(1.8f));

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeDown(defaultSize));
            EffectSpawner.instance.effectDropCard.Play();
            _audioSource.PlayOneShot(playSound);

            yield return new WaitForSeconds(0.8f);

            foreach (Effect effect in card.Effects)
                yield return StartCoroutine(effect.Animation(this, true));

            yield return new WaitForSeconds(0.8f);

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeDown(0.01f));
            targetTransformPosition = _deck.GetComponent<RectTransform>();

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();

            if (card.SaveTurn)
            {
                BattleClientManager.SetCanPlay(true);
                BattleUI.ShowTipsWindow("Please, play another card");
            }
            else
            {
                BattleUI.HideTipsWindow();
                MatchClientController.EndTurn();
            }

            BattleClientManager.instance.CheckEndMatch();
            Destroy(gameObject);
        }

        public IEnumerator CardEnemyPlayAnimation()
        {
            yield return new WaitForEndOfFrame();

            foreach (Resource resource in card.Cost)
                BattleUI.RemoveEnemyResourceValue(resource.Name, resource.Value);

            _tableTopImage.sprite = card.CardImage;
            _tableTopImage.raycastTarget = false;
            targetTransformPosition = _mainAnimationPosition.GetComponent<RectTransform>();

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeUp(1.8f));

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeDown(defaultSize));
            EffectSpawner.instance.effectDropCard.Play();
            _audioSource.PlayOneShot(playSound);

            yield return new WaitForSeconds(0.8f);

            foreach (Effect effect in card.Effects)
                yield return StartCoroutine(effect.Animation(this, false));

            yield return new WaitForSeconds(0.8f);

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeDown(0.4f));
            targetTransformPosition = _spawnEnemyCardPosition.GetComponent<RectTransform>();

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();

            BattleClientManager.instance.CheckEndMatch();
            Destroy(gameObject); 
        }

        public IEnumerator CardDraftPlayAnimation()
        {
            yield return new WaitForEndOfFrame();

            _tableTopImage.sprite = card.CardImage;
            _cardPosition = FindObjectsOfType<CardPosition>()
                .Where(p => p.card == null)
                .FirstOrDefault();

            transform.SetParent(_cardPosition.transform);
            _cardPosition.card = gameObject;
            targetTransformPosition = _cardPosition.rectTransform;

            _cardPosition.transform.SetSiblingIndex(_cardPosition.startPointerIndex);

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();
        }

        public IEnumerator CardDiscardPlayAnimation()
        {         
            _audioSource.PlayOneShot(discardSound);

            yield return new WaitForEndOfFrame();

            _cardPosition.card = null;
            _cardPosition = null;
            _tableTopImage.raycastTarget = false;
            targetTransformPosition = _deck.GetComponent<RectTransform>();

            if (_sizeCoroutine != null)
                StopCoroutine(_sizeCoroutine);

            _sizeCoroutine = StartCoroutine(SizeDown(0.01f));

            while (Vector3.Distance(_rectTransform.position, targetTransformPosition.position) > 0.1f)
                yield return new WaitForFixedUpdate();

            BattleClientManager.instance.CheckEndMatch();
            BattleUI.HideTipsWindow();
            Destroy(gameObject);
        }

        private void Start()
        {
            //TODO: Срочный рефактор, полная помойка

            _rectTransform = GetComponentInParent<RectTransform>();
            _canvas = FindObjectOfType<Canvas>();
            _tableTopImage = GetComponent<Image>();
            _mainAnimationPosition = GameObject.Find("SpawnAnimationPlaceHolder");
            _deck = GameObject.Find("Deck");
            _spawnEnemyCardPosition = GameObject.Find("SpawnPointEnemyCards");
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _rectTransform.rotation = Quaternion.Slerp(_rectTransform.rotation, _targetAngle, smoothRotate);
            _targetAngle = new Quaternion(
               0, 0,
               _rectTransform.rotation.z,
               _rectTransform.rotation.w);

            if (!_isDragging && targetTransformPosition != null)
                _rectTransform.position = Vector3.Slerp(_rectTransform.position, targetTransformPosition.position, smoothMove);

            SetCanPlayed(BattleClientManager.IsMyTurn() 
                && BattleClientManager.IsCanPlay() 
                && HasResources());
        }
        #endregion
    }
}