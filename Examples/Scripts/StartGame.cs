using UnityEngine;
using UnuGames;

public class StartGame : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        UIMan.Instance.ShowScreen<UIMainMenu>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}