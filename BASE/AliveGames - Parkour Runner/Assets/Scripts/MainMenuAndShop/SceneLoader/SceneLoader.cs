using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour {


    [SerializeField]  private int _sceneID;
    [SerializeField]  private Image _loadingImg;
    [SerializeField]  private Text _progressText;

    private void Start()
    {
        if (PlayerPrefs.GetInt(EnvironmentController.MULTIPLAYER_KEY) > 0) {
            if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(_sceneID);
            StartCoroutine(PhotonLoadProgress());
        }
        else {
            // StartCoroutine(AsyncLoad());
        }
    }

    private void Update()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().Rotate(new Vector3(0,0,1)*500*Time.deltaTime);
    }
    private IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneID);
        while (!operation.isDone)
        {
            float progress = operation.progress / 0.9f;
            _loadingImg.fillAmount = progress;
            _progressText.text = string.Format("{0:0}%", progress*100);
            yield return null;
        }
    }


    private IEnumerator PhotonLoadProgress() {
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            float progress = PhotonNetwork.LevelLoadingProgress;
            progress = Mathf.Clamp(progress, 0, 1f);
            _loadingImg.fillAmount = progress;
            _progressText.text     = $"{progress * 100:0}%";
            yield return null;
        }
    }
}
