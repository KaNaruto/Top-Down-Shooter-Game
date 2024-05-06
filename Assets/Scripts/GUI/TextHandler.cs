using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI credits;
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (FindObjectOfType<Menu>().inCreditsMenu && eventData.button == PointerEventData.InputButton.Left)
        {
            int index = TMP_TextUtilities.FindIntersectingLink(credits,  eventData.position, null);
            Debug.Log(index);
            if (index > -1)
            {
                Application.OpenURL(credits.textInfo.linkInfo[index].GetLinkID());
            }
        }
    }
}
