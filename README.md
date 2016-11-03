# Drive.NET

Easy to use .NET Library for managing multiple Google Drive files.
This library currently uses Google Drive Api v3

## Authentication

Follow these steps(Taken from google guide) to get your credentials from Google.

1. Use [this](https://console.developers.google.com/start/api?id=drive) wizard to create or select a project in the Google Developers Console and automatically turn on the API. Click Continue, then Go to credentials.

2. On the Add credentials to your project page, click the Cancel button.

3. At the top of the page, select the OAuth consent screen tab. Select an Email address, enter a Product name if not already set, and click the Save button.

4. Select the Credentials tab, click the Create credentials button and select OAuth client ID.

5. Select the application type Other, enter the name "Drive API Quickstart", and click the Create button.

6. Click OK to dismiss the resulting dialog.

7. Click the file_download (Download JSON) button to the right of the client ID.

8. Move this file to your working directory and rename it client_secret.json.

## Usage

Reference the compiled class or insert the source class into your project.

### Creating the class

There are **2** constructors for the base class.

**LoggingEnabled** is an optional parameter which enables logging to file.

1) Uses client_secret.json file for credentials

```
NetGDrive drive = new NetGDrive("YourAppName");
```

2) Uses client credentials from string parameter.

```
NetGDrive drive = new NetGDrive("YourAppName","YOUR_CLIENT_ID","YOUR_CLIENT_SECRET");
```

### Searching/Filtering Files

You can use chained methods to build a search parameter.

It supports all the operators and most of the fields from google drive but you can specify your own string so it shouldn't be a problem very much.  

```
NetGSearchBuilder b = new NetGSearchBuilder();
b.AddField(Field.Name).Contains("SomeText").Or().AddField(Field.Name).Equal("SomeMoreText");

NetGDrive drive = new NetGDrive("YourAppName"); //This constructor uses client credentials from client_secret.json file.
var files = drive.GetFiles(b.Search);
    
    foreach (var file in files)
        {
            Console.WriteLine(file.Name);
        }
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Contact Me

_Please dont hesitate to contact for any bugs or requests._

berkaygursoy@gmail.com
