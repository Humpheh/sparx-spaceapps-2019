using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public string url;
    private GameObject panelChild;
    private GameObject imageChild;
    private Vector3 worldLocation;

    private static bool onboarded = false;
    
    // Start is called before the first frame update
    void Start()
    {
        panelChild = transform.Find("Panel").gameObject;
        imageChild = panelChild.transform.Find("WebImage").gameObject;
        StartCoroutine(DownloadImage(url));
    }

    public void Update()
    {
        var screenPosition = Utils.CanvasPosition(worldLocation);
        GetComponent<RectTransform>().anchoredPosition = screenPosition;
    }

    IEnumerator DownloadImage(string MediaUrl)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            Destroy(gameObject);
        }
        else
        {
            var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            var rect = new Rect(0, 0, texture.width, texture.height);
            imageChild.GetComponent<Image>().sprite = Sprite.Create(texture, rect, Vector2.zero);

            if (!onboarded)
            {
                Modal.OpenModal(
                    "First report received!",
                    "<b>You've received a mosquito report.</b>\\n\\n" +
                    "These reports are useful first indicators of mosquito activity send in by the public. " +
                    "They will appear on your map in bubbles."
                );
                onboarded = true;
            }

            panelChild.SetActive(true);
            yield return new WaitForSeconds(10);
            Destroy(gameObject);
        }
    }

    public static void SpawnPanel(Vector2 worldPosition, string imageURL, string text = "")
    {
        Debug.LogFormat("Loading image: {0}", imageURL);
        GameObject popupPrefab = UnityEngine.Resources.Load("ImagePanel") as GameObject;
               
        var modal = Instantiate(popupPrefab);
        modal.transform.parent = Map.GetSingleton().canvas.transform;
        
        var choice = modal.GetComponent<Popup>();
        choice.url = imageURL;
        choice.worldLocation = worldPosition;
        
        modal.transform.Find("Panel").Find("Text").GetComponent<Text>().text = text;
    }
}
