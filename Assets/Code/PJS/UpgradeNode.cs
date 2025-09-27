using UnityEngine;

public class UpgradeNode : MonoBehaviour
{
    [SerializeField] private UpgradeNode[] RequestNode;
    [SerializeField] private UpgradeNode[] NextNode;

    [SerializeField] private float coast;

    private bool _activated = false;
    public bool Activated { get { return _activated; } }

    private struct StatValuePair
    {
        public string Key;
        public object Value;
    }
    public void TryActiveNode()
    {
        bool requestNodeActive = true;
        foreach (UpgradeNode node in RequestNode)
        {
            if (node.Activated == false)
            {
                requestNodeActive = false;
                continue;
            }
        }

        if (requestNodeActive)
        {
            _activated = true;
        }
    }
}
