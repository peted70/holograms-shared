using GLTF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity3dAzure.StorageServices;
using UnityEngine;
using UnityEngine.UI;

public class AzureScript : MonoBehaviour {
    [Header("Azure config")]
    [SerializeField]
    private string azureAccount;
    [SerializeField]
    private string azureAppKey;
    [SerializeField]
    private string container; // NB: blobs need to be public access for this sample

    private BlobService azure;
    private StorageServiceClient client;

    [Header("Unity")]
    [SerializeField]
    private Dropdown menu;
    [SerializeField]
    private GameObject spawn;
    [SerializeField]
    private GameObject gltf;

    public string[] models = new []{ "Monster.glb", "Duck.glb"};
    [SerializeField]
    private Dropdown modelsMenu;

    // Use this for initialization
    void Start () {
        if (string.IsNullOrEmpty(azureAccount) || 
            string.IsNullOrEmpty(azureAppKey) || 
            string.IsNullOrEmpty(container))
        {
            Debug.LogError("Azure account, key and container resource required");
        }
        // setup blob service
        client = new StorageServiceClient(azureAccount, azureAppKey);
        azure = new BlobService(client);

        // populate menus
        ReplaceDropdownOptions(modelsMenu, models);
        ListBlobs();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickRefresh()
    {
        ListBlobs();
    }

    public void ClickLoad()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        if (menu.options.Count > 0)
        {
            string selectedBlob = menu.options[menu.value].text;
            CreateGLTF(resolveBlobURL(selectedBlob), position, rotation, spawn);
        }
        else
        {
            Debug.LogWarning("No blobs available in container: " + container);
        }
    }

    public void ClickUploadBlob()
    {
        UploadBlobFile();
    }

    private void ListBlobs()
    {
        StartCoroutine(azure.ListBlobs(ListBlobsCompleted, container));
    }

    private void ListBlobsCompleted(IRestResponse<BlobResults> response)
    {
        if (response.IsError) {
            Debug.LogError("Failed to get list of blobs: " + response.ErrorMessage);
            return;
        }
        Debug.Log( "Loaded blobs: " + response.Data.Blobs.Length + " Loaded blobs: " + response.Data.Blobs.Length);
        // populate dropdown menu
        List<string> names = new List<string>();
        foreach (Blob blob in response.Data.Blobs)
        {
            names.Add(blob.Name);
        }
        ReplaceDropdownOptions(menu, names.ToArray());
    }

    private void ReplaceDropdownOptions(Dropdown dropdown, string[] options)
    {
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        foreach (string option in options)
        {
            var item = new Dropdown.OptionData(option);
            list.Add(item);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(list);
    }

    private string resolveBlobURL(string filename)
    {
        return string.Format("{0}{1}/{2}", client.PrimaryEndpoint(), container, filename);
    }

    private void CreateGLTF(string url, Vector3 position, Quaternion rotation, GameObject parent)
    {
        Debug.Log("Create blob: " + url);
        GLTFComponent component = gltf.GetComponent<GLTFComponent>();
        component.Url = url;
        var obj = Instantiate(gltf, position, rotation, parent.transform);
        obj.name = url.Substring(url.LastIndexOf('/') + 1);
    }

    private void UploadBlobFile()
    {
        string filename = modelsMenu.options[modelsMenu.value].text;
        //string basename = Path.GetFileNameWithoutExtension(filename);
        string filepath = Application.dataPath + "/Models/" + filename;
        Debug.Log("Upload file: " + filepath);

        if (File.Exists(filepath))
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            PutModel(bytes, filename);
        }
        else
        {
            Debug.LogError("Couldn't resolve file: " +  filepath);
        }
    }

    private void PutModel(byte[] modelBytes, string filename, string contentType = "model/gltf")
    {
        
        Debug.Log("Put " + filename);
        StartCoroutine(azure.PutAssetBundle(PutModelCompleted, modelBytes, container, filename, contentType));
    }

    private void PutModelCompleted(RestResponse response)
    {
        if (response.IsError)
        {
            Debug.LogError("Error putting blob audio:" + response.ErrorMessage);
            return;
        }
        Debug.Log( "Put image blob:" + response.Content);
    }
}
