# Unity GLTF project

1. Setup Azure blob storage.
2. Add containter (public blobs).
3. Upload *.glb models into container.
4. Add blob storage account, key and container name to Unity project.

# Loading GLTF models from Blob storage

1. Refresh button to update list of blobs in container.
2. Click 'Load' to add GLTF prefab to load model from blob storage.

# Uploading GLTF models from Blob storage

The easiest way is to upload models using the Azure Storage Explorer app. 

To upload within the Unity Editor:  
1. Move blobs into app data (Assets) folder.
2. Populate Dropdown menu with list of model filenames.
3. Run in Editor to upload the selected file.

# Unity build player settings for UWP app

## Configuration:
- Scripting backend: **IL2CCP**
- API compatibility level: **.NET 2.0**

## Capabilities: 
- InternetClient
