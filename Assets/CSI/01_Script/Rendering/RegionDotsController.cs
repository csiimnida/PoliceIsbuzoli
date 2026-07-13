using UnityEngine;

namespace CSI._01_Script.Rendering
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class RegionDotsController : MonoBehaviour
    {
        [Header("Material Property Names")] 
        [SerializeField] private string densityProperty = "_Density";
        [SerializeField] private string regionColorProperty = "_RegionColor";
        [SerializeField] private string toleranceProperty = "_Tolerance";
        [SerializeField] private string dotSizeProperty = "_DotSize";
        [SerializeField] private string tilingProperty = "_Tiling";
        [SerializeField] private string seedProperty = "_Seed";

        [Header("Parameters")] 
        [Range(0,1)] public float density = 0f; // 0~1, more = more dots
        [ColorUsage(false, true)] public Color regionColor = Color.red; // color in mask texture
        [Range(0f,1f)] public float tolerance = 0.1f; // how close mask color must be
        [Range(0.001f,0.2f)] public float dotSize = 0.05f;
        public Vector2 tiling = new Vector2(10, 10);
        public float seed = 0f;

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
            Apply();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (_renderer == null) _renderer = GetComponent<Renderer>();
            }
            Apply();
        }

        public void SetDensity(float value)
        {
            density = Mathf.Clamp01(value);
            Apply();
        }

        public void Apply()
        {
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_mpb);

            if (!string.IsNullOrEmpty(densityProperty))
                _mpb.SetFloat(densityProperty, Mathf.Clamp01(density));

            if (!string.IsNullOrEmpty(regionColorProperty))
                _mpb.SetColor(regionColorProperty, regionColor);

            if (!string.IsNullOrEmpty(toleranceProperty))
                _mpb.SetFloat(toleranceProperty, Mathf.Clamp01(tolerance));

            if (!string.IsNullOrEmpty(dotSizeProperty))
                _mpb.SetFloat(dotSizeProperty, Mathf.Clamp(dotSize, 0.0001f, 1f));

            if (!string.IsNullOrEmpty(tilingProperty))
                _mpb.SetVector(tilingProperty, new Vector4(Mathf.Max(0.001f, tiling.x), Mathf.Max(0.001f, tiling.y), 0, 0));

            if (!string.IsNullOrEmpty(seedProperty))
                _mpb.SetFloat(seedProperty, seed);

            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
