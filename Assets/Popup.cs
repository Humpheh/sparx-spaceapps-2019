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
        }
        else
        {
            var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            var rect = new Rect(0, 0, texture.width, texture.height);
            imageChild.GetComponent<Image>().sprite = Sprite.Create(texture, rect, Vector2.zero);
        }
        panelChild.SetActive(true);
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    public static void SpawnPanel(Vector2 worldPosition, string imageURL)
    {
        GameObject popupPrefab = UnityEngine.Resources.Load("ImagePanel") as GameObject;
               
        var modal = Instantiate(popupPrefab);
        modal.transform.parent = Map.GetSingleton().canvas.transform;
        
        var choice = modal.GetComponent<Popup>();
        choice.url = imageURL;
        choice.worldLocation = worldPosition;
    }
}
