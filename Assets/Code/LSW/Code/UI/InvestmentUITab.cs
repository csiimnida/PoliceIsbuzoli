using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.LSW.Code.UI
{
    public class InvestmentUITab : MonoBehaviour
    {
        [Header("Points")]
        // 스킬트리 포인트
        [SerializeField, Min(0)] private int startingPoints = 0;
        [SerializeField, Min(0)] private int currentPoints = 0;

        [Header("Visuals (Optional)")] 
        public string pointsMeaning = "Points";
        [SerializeField] private Color lockedColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        [SerializeField] private Color availableColor = new Color(1f, 0.9f, 0.3f, 1f);
        [SerializeField] private Color unlockedColor = new Color(0.3f, 1f, 0.4f, 1f);
        [SerializeField] private float unavailableButtonAlpha = 0.6f;

        [Header("Optional UI References")] 
        [SerializeField] private TextMeshProUGUI pointsText;

        [Header("Events")]
        [SerializeField] private UnityEvent<int> onPointsChanged = new UnityEvent<int>();
        [SerializeField] private UnityEvent<Node> onNodeUnlocked = new UnityEvent<Node>();

        [Serializable]
        public class Node
        {
            [Header("Identity")]
            public string id;
            public string title;

            [Header("Economy")]
            [Min(0)] public int cost = 1;

            [Header("Dependencies")]
            public List<string> prerequisites = new List<string>();

            [Header("State (Runtime)")]
            [SerializeField] private bool unlocked = false;
            public bool Unlocked => unlocked;

            [Header("UI References")] 
            public Button button;
            public Image background;
            public TextMeshProUGUI costText;
            public List<Image> linkLines;
            
            public void SetUnlocked(bool value)
            {
                unlocked = value;
            }
        }

        [SerializeField] private List<Node> nodes = new List<Node>();

        private readonly Dictionary<string, Node> _nodeById = new Dictionary<string, Node>();

        private void Awake()
        {
            Initialize();
            SetPoints(startingPoints);
            RefreshAll();
        }

        private void OnValidate()
        {
            BuildIndex();
            UpdatePointsLabel();
        }

        private void Initialize()
        {
            BuildIndex();
            foreach (var node in nodes)
            {
                if (node.button == null) continue;
                node.button.onClick.RemoveListener(() => TryUnlockByButton(node));
                node.button.onClick.AddListener(() => TryUnlockByButton(node));
            }
            foreach (var node in nodes)
            {
                if (node.costText)
                    node.costText.text = node.cost > 0 ? node.cost.ToString() : "";
            }
        }

        private void BuildIndex()
        {
            _nodeById.Clear();
            foreach (var n in nodes)
            {
                if (string.IsNullOrWhiteSpace(n.id)) continue;
                _nodeById.TryAdd(n.id, n);
            }
        }
        
        public int GetPoints() => currentPoints;
        public void AddPoints(int delta) => SetPoints(currentPoints + Mathf.Max(0, delta));
        
        public void SetPoints(int value)
        {
            currentPoints = Mathf.Max(0, value);
            UpdatePointsLabel();
            onPointsChanged?.Invoke(currentPoints);
            RefreshAll();
        }

        public bool IsUnlocked(string nodeId)
        {
            return _nodeById.TryGetValue(nodeId, out var n) && n.Unlocked;
        }

        public bool CanUnlock(string nodeId)
        {
            if (!_nodeById.TryGetValue(nodeId, out var n)) 
                return false;
            if (n.Unlocked || n.cost > currentPoints) 
                return false;
            
            foreach (var pre in n.prerequisites)
            {
                if (string.IsNullOrEmpty(pre)) 
                    continue;
                if (!_nodeById.TryGetValue(pre, out var p) || !p.Unlocked) 
                    return false;
            }
            return true;
        }

        public bool TryUnlock(string nodeId)
        {
            if (!CanUnlock(nodeId)) 
                return false;
            var node = _nodeById[nodeId];
            
            // node.effect 효과 적용하는 코드 추가
            currentPoints -= node.cost;
            node.SetUnlocked(true);
            UpdatePointsLabel();
            onPointsChanged?.Invoke(currentPoints);
            onNodeUnlocked?.Invoke(node);
            RefreshAll();
            return true;
        }
        
        private void TryUnlockByButton(Node node)
        {
            if (node == null || string.IsNullOrEmpty(node.id)) 
                return;
            TryUnlock(node.id);
        }

        private void RefreshAll()
        {
            foreach (var node in nodes)
            {
                UpdateNodeVisual(node);
            }
        }

        private void UpdateNodeVisual(Node node)
        {
            bool available = !node.Unlocked && CanUnlock(node.id);
            
            if (node.button)
            {
                node.button.interactable = available;
                var colors = node.button.colors;
                node.button.colors = colors;
                
                var cg = node.button.GetComponent<CanvasGroup>();
                if (cg)
                {
                    cg.alpha = node.Unlocked || available ? 1f : unavailableButtonAlpha;
                }
            }
            
            if (node.background)
            {
                node.background.color = node.Unlocked ? unlockedColor : (available ? availableColor : lockedColor);
            }
            
            if (node.costText)
            {
                node.costText.text = node.Unlocked ? "Unlocked" : node.cost.ToString();
            }
            
            if (node.linkLines is { Count: > 0 })
            {
                bool parentsUnlocked = true;
                foreach (var pre in node.prerequisites)
                {
                    if (string.IsNullOrEmpty(pre)) 
                        continue;
                    if (!_nodeById.TryGetValue(pre, out var p) || !p.Unlocked) { parentsUnlocked = false; break; }
                }
                var lineColor = parentsUnlocked ? unlockedColor : lockedColor;
                foreach (var line in node.linkLines)
                {
                    if (line) line.color = lineColor;
                }
            }
        }

        private void UpdatePointsLabel()
        {
            if (pointsText)
            {
                pointsText.text = pointsMeaning + currentPoints.ToString();
            }
        }

        public void ForceUnlock(string nodeId, bool refresh = true)
        {
            if (!_nodeById.TryGetValue(nodeId, out var n)) 
                return;
            if (!n.Unlocked)
            {
                n.SetUnlocked(true);
                onNodeUnlocked?.Invoke(n);
                if (refresh) RefreshAll();
            }
        }
        
        public void ResetTree(bool keepPoints = true)
        {
            foreach (var n in nodes)
                n.SetUnlocked(false);

            if (!keepPoints)
                SetPoints(startingPoints);
            else
                RefreshAll();
        }
    }
}