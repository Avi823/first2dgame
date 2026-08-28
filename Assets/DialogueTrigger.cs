using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    [SerializeField] private string message = "*Default message.*";
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        string formattedMessage = message.Replace("\\n", "\n");
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Dialogue.instance.SetText(formattedMessage);
            if (triggerOnce)
            {
                hasTriggered = true;
            }
        }
    }
}
