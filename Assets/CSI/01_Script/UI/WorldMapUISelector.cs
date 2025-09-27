using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CSI._01_Script.UI
{
    /// <summary>
    /// UI Image 기반의 월드 맵에서 클릭한 위치의 마스크 색상을 읽어
    /// 지정된 국가 데이터(또는 ID)를 찾아주는 셀렉터.
    /// - nationMask 텍스처는 Read/Write Enabled 여야 합니다.
    /// - worldMapImage에 표시된 스프라이트와 nationMask가 같은 비율/정렬을 가진다고 가정합니다.
    /// - preserveAspect로 인한 레터박스/필러박스 영역을 고려합니다.
    /// - 일치 색상 매핑 실패 시(압축/필터 영향) 색상 허용 오차 탐색을 제공합니다.
    /// </summary>
    public class WorldMapUISelector : MonoBehaviour
    {
        [Header("Map References")]
        [Tooltip("클릭을 받을 UI Image (Sprite 표시)")]
        [SerializeField] private Image worldMapImage;

        [Tooltip("국가 식별용 마스크 텍스처 (색상→국가 매핑). 반드시 Read/Write Enabled")]
        [SerializeField] private Texture2D nationMask;

        [Tooltip("마스크가 이미지의 스프라이트 영역과 1:1 비율/정렬로 대응한다고 가정할지 여부")]
        [SerializeField] private bool maskMatchesSpriteRect = true;

        [Header("Detection Settings")]
        [Range(0f, 1f)]
        [Tooltip("마스크 픽셀의 최소 알파. 이보다 작으면 무시(클릭 미판정)")]
        [SerializeField] private float alphaThreshold = 0.1f;

        [Range(0f, 1f)]
        [Tooltip("정확 매칭 실패 시 색상 허용 오차(0~1). 0이면 오차 탐색 안 함")]
        [SerializeField] private float colorTolerance = 0.02f;

        [Header("Data Mapping (Color → Nation)")]
        [Tooltip("마스크 색상과 국가 ID(또는 페이로드)의 매핑 테이블")]
        public List<NationMaskEntry> nations = new List<NationMaskEntry>();

        private Dictionary<Color32, NationMaskEntry> _lookup;
        private RectTransform _mapRect;
        private Camera _eventCamera;

        [Header("Events")]
        [Tooltip("국가 선택 시 국가 ID를 전달")]
        public UnityEvent<string> onNationSelected;

        [Tooltip("국가 선택 시 임의 오브젝트 페이로드 전달 (선택사항)")]
        public UnityEvent<UnityEngine.Object> onNationPayloadSelected;

        [Tooltip("어떠한 국가도 선택되지 않았을 때 호출")]
        public UnityEvent onNoNation;

        private void Reset()
        {
            // 컴포넌트 자동 할당을 돕습니다.
            if (worldMapImage == null)
                worldMapImage = GetComponent<Image>();
        }

        private void Awake()
        {
            _mapRect = worldMapImage ? worldMapImage.rectTransform : null;
            if (worldMapImage != null && worldMapImage.canvas != null)
            {
                // Overlay면 null, 그 외엔 canvas.worldCamera 사용
                _eventCamera = worldMapImage.canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : worldMapImage.canvas.worldCamera;
            }

            BuildLookup();
            WarnIfMaskNotReadable();
        }

        private void OnEnable()
        {
            if (_mapRect == null && worldMapImage != null)
                _mapRect = worldMapImage.rectTransform;
        }

        private void Update()
        {
            if (!enabled || worldMapImage == null || nationMask == null) return;

#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 pos = mouse.position.ReadValue();
                SelectAtScreenPosition(pos);
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                SelectAtScreenPosition(Input.mousePosition);
            }
#endif
        }

        /// <summary>
        /// 주어진 화면 좌표에서 국가를 선택 시도하고 이벤트를 발생시킵니다.
        /// </summary>
        public bool SelectAtScreenPosition(Vector2 screenPosition)
        {
            if (TryGetNationAt(screenPosition, out var entry))
            {
                Debug.Log(entry.nationId);
                onNationSelected?.Invoke(entry.nationId);
                if (entry.payload != null)
                    onNationPayloadSelected?.Invoke(entry.payload);
                return true;
            }

            Debug.Log("No");
            onNoNation?.Invoke();
            return false;
        }

        /// <summary>
        /// 주어진 화면 좌표에서 국가 데이터를 조회합니다. 성공 시 true 반환.
        /// </summary>
        public bool TryGetNationAt(Vector2 screenPosition, out NationMaskEntry entry)
        {
            entry = null;
            if (_mapRect == null || worldMapImage == null || nationMask == null) return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_mapRect, screenPosition, _eventCamera,
                    out var local))
            {
                return false;
            }

            // 로컬 좌표를 0..1 정규화 좌표(좌하 기준)로 변환
            var rect = _mapRect.rect;
            float nx = (local.x - rect.xMin) / rect.width;
            float ny = (local.y - rect.yMin) / rect.height;

            if (nx < 0f || nx > 1f || ny < 0f || ny > 1f)
                return false; // 이미지 영역 밖

            // preserveAspect로 생기는 레터/필러 박스 보정
            Vector2 uv = new Vector2(nx, ny);
            AdjustForPreserveAspect(worldMapImage, ref uv, out bool inside);
            if (!inside) return false;

            // UV → 마스크 텍스처 픽셀 좌표 변환
            int px, py;
            if (maskMatchesSpriteRect && worldMapImage.sprite != null)
            {
                // 스프라이트 rect를 기준으로 UV를 픽셀로 변환 후, 마스크 해상도로 재매핑
                Rect spriteRect = worldMapImage.sprite.textureRect; // 소스 텍스처상의 영역

                int sx = Mathf.Clamp(Mathf.RoundToInt(uv.x * spriteRect.width), 0, (int)spriteRect.width - 1);
                int sy = Mathf.Clamp(Mathf.RoundToInt(uv.y * spriteRect.height), 0, (int)spriteRect.height - 1);

                px = Mathf.Clamp(Mathf.RoundToInt(sx / spriteRect.width * nationMask.width), 0, nationMask.width - 1);
                py = Mathf.Clamp(Mathf.RoundToInt(sy / spriteRect.height * nationMask.height), 0, nationMask.height - 1);
            }
            else
            {
                // 이미지 Rect 기준 UV를 마스크 전체 해상도로 바로 샘플링
                px = Mathf.Clamp(Mathf.RoundToInt(uv.x * nationMask.width), 0, nationMask.width - 1);
                py = Mathf.Clamp(Mathf.RoundToInt(uv.y * nationMask.height), 0, nationMask.height - 1);
            }

            Color pixel;
            try
            {
                pixel = nationMask.GetPixel(px, py);
            }
            catch (Exception)
            {
                // Read/Write 미설정 등으로 실패할 수 있음
                return false;
            }

            if (pixel.a < alphaThreshold) return false; // 투명 영역 무시

            var c32 = (Color32)pixel;

            // 1) 정확 매칭 시도
            if (_lookup != null && _lookup.TryGetValue(c32, out entry))
                return true;

            // 2) 오차 기반 근사 매칭
            if (_lookup != null && colorTolerance > 0f)
            {
                float tol255 = colorTolerance * 255f;
                foreach (var kv in _lookup)
                {
                    var cc = kv.Key;
                    if (Mathf.Abs(cc.r - c32.r) <= tol255 &&
                        Mathf.Abs(cc.g - c32.g) <= tol255 &&
                        Mathf.Abs(cc.b - c32.b) <= tol255)
                    {
                        entry = kv.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<Color32, NationMaskEntry>();
            for (int i = 0; i < nations.Count; i++)
            {
                var e = nations[i];
                _lookup[(Color32)e.maskColor] = e; // 동일 색상 키는 마지막 항목이 우선
            }
        }

        private void AdjustForPreserveAspect(Image img, ref Vector2 uv, out bool inside)
        {
            inside = true;
            if (!img.preserveAspect || img.sprite == null || _mapRect == null) return;

            Rect rect = _mapRect.rect;
            Vector2 spriteSize = img.sprite.rect.size; // 스프라이트 픽셀 크기

            float rectAspect = rect.width / rect.height;
            float spriteAspect = spriteSize.x / spriteSize.y;

            if (spriteAspect > rectAspect)
            {
                // 가로가 더 긴 스프라이트 → 상하 레터박스
                float shownHeight = rect.width / spriteAspect;
                float pad = (rect.height - shownHeight) / 2f; // 상/하 패딩
                float yMin = pad / rect.height;
                float yMax = 1f - yMin;

                if (uv.y < yMin || uv.y > yMax) { inside = false; return; }
                uv = new Vector2(uv.x, Mathf.InverseLerp(yMin, yMax, uv.y));
            }
            else
            {
                // 세로가 더 긴 스프라이트 → 좌우 필러박스
                float shownWidth = rect.height * spriteAspect;
                float pad = (rect.width - shownWidth) / 2f; // 좌/우 패딩
                float xMin = pad / rect.width;
                float xMax = 1f - xMin;

                if (uv.x < xMin || uv.x > xMax) { inside = false; return; }
                uv = new Vector2(Mathf.InverseLerp(xMin, xMax, uv.x), uv.y);
            }
        }

        private void WarnIfMaskNotReadable()
        {
#if UNITY_EDITOR
            if (nationMask != null)
            {
                try { _ = nationMask.GetPixel(0, 0); }
                catch (UnityException ex)
                {
                    Debug.LogWarning(
                        $"[WorldMapUISelector] nationMask이 Read/Write Enabled가 아닐 수 있습니다. Texture Import Settings에서 Read/Write를 켜주세요. {ex.Message}",
                        this);
                }
            }
#endif
        }

        [Serializable]
        public class NationMaskEntry
        {
            [Tooltip("마스크에서 해당 국가를 나타내는 색상")] public Color maskColor = Color.black;
            [Tooltip("국가 식별용 ID (이벤트로 전달)")] public string nationId;
            [Tooltip("선택 시 이벤트로 함께 전달할 수 있는 임의의 오브젝트(선택사항)")] public UnityEngine.Object payload;
        }
    }
}
