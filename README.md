# SigmaTestTask

## Image API

Image API requires to specify the following settings:

```json
"AzureBlobSettings": {
  "AzureBlobConnectionString": "Your_Account_Key",
  "AzureBlobContainerName": "Your_Blob_Container_Name"
}
```
You can use the standard `appsettings.json` file for this, or alternatively, pass these settings through environment variables.

## Image UI

### Install dependencies
```
npm install
```

### Run dev server
```
npm run serve

