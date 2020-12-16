using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadGamePanel : MonoBehaviour
{
    [SerializeField] MainMenu m_mainMenu = default;
    [SerializeField] Transform m_fileListRoot = default;
    [SerializeField] GameObject m_fileListItemPrefab = default;

    [SerializeField] TextMeshProUGUI m_currentFileText = default;

    List<GameObject> m_fileListItems;
    string selectedFileName;

    // Start is called before the first frame update
    void Awake()
    {
        m_fileListItems = new List<GameObject>();
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        RefreshPanel();
    }

    public void HidePanel()
    {
        ClearFileList();
        gameObject.SetActive(false);
        m_mainMenu.RefreshPanel();
    }

    void RefreshPanel()
    {
        ClearFileList();
        List<string> files = SaveLoadUtility.GetSaveFileNames();

        if(files == null || files.Count == 0)
        {
            return;
        }

        for(int i = 0; i < files.Count; i++)
        {
            GameObject item = Instantiate(m_fileListItemPrefab, m_fileListRoot);
            item.GetComponentInChildren<TextMeshProUGUI>().text = files[i];
            item.GetComponent<Button>().onClick.AddListener(() => SelectFile(item));
            m_fileListItems.Add(item);
        }
    }

    void ClearFileList()
    {
        selectedFileName = null;
        if (m_fileListItems == null)
        {
            m_fileListItems = new List<GameObject>();
            return;
        }

        foreach (GameObject go in m_fileListItems)
        {
            Destroy(go);
        }

        m_fileListItems = new List<GameObject>();

    }

    void SelectFile(GameObject fileListItem)
    {
        selectedFileName = fileListItem.GetComponentInChildren<TextMeshProUGUI>().text;
        m_currentFileText.text = selectedFileName;
    }

    public void LoadSelectedFile()
    {
        if(selectedFileName == null)
        {
            Debug.LogWarning("No file selected");
            return;
        }

        m_mainMenu.StartGame(selectedFileName);
        //SaveLoadUtility.LoadGame(selectedFileName);
    }

    public void DeleteSelectedFile()
    {
        if (selectedFileName == null)
        {
            Debug.LogWarning("No file selected");
            return;
        }

        SaveLoadUtility.DeleteGame(selectedFileName);
        RefreshPanel();
    }
}
