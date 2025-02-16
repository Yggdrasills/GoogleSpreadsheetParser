Google Spreadsheets Parser for Unity
===
Google spreadsheets parsing utility

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
  - [As Unity Package](#-as-unity-package)
  - [Via UPM](#-via-upm)
- [Google AppScript Publishing](#google-appscript-publishing)
- [Getting Started](#getting-started)
  - [Settings](#settings)
  - [Index.json](#indexjson)
- [Built-in Parsers](#built-in-parsers)
  - [Default](#default)
  - [Complex](#complex)
  - [KeyValue](#key-value)
- [Custom Parser](#custom-parser)
- [Serializer](#serializer)

# Overview
A Unity plugin for managing game data through Google Sheets. Key features:
- Direct integration with Google Sheets API
- Editor window for managing spreadsheets
- Three built-in parser types (Default, Complex, KeyValue)
- Customizable JSON serialization
- Batch processing of multiple sheets
- Automatic type detection for values
- Array support with comma-separated values


### Dependencies
The parser requires following dependency: `Newtonsoft.Json Unity Package (com.unity.nuget.newtonsoft-json: "3.2.1")`
# Installation

### ⭐ As Unity Package

- Download the [latest release](https://github.com/Yggdrasills/GoogleSpreadsheetParser/releases/tag/1.0.0)
- Import to your project
- Add [Newtonsoft Json Unity Package](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html) version 3.2.1
  - [How to install Newtonsoft Json Unity Package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM)

### ⭐ Via UPM
- Open Unity's Package Manager (Window > Package Manager)
- Click the + button in the top-left corner
- Select "Add package from git URL..."
- Enter the following URL:
```
https://github.com/Yggdrasills/GoogleSpreadsheetParser.git?path=Assets/Plugins/GoogleSpreadsheetParser
```
- Click Add to install the package
### ⭐ Manifest.json
- Paste dependency:
```
"com.yggdrasills.googlespreadsheetparser": "https://github.com/Yggdrasills/GoogleSpreadsheetParser.git?path=Assets/Plugins/GoogleSpreadsheetParser"
```
# Google AppScript Publishing
- Make copy of following [app script](https://script.google.com/home/projects/1Wh8-CvTk8GGg-F2tXN27n-WOOOFvN9vc0cmabA2wdgsm8zro4Fh9lPW4/edit) to your disk

![AppScript copy](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/raw/images/app_script_copy.png)

- Deploy

![Deploy](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/raw/images/app_script_deploy.png)

- Copy web app url to your [settings](#settings) file

![Web App url](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/raw/images/app_script_get_web_app_url.png)

# Getting Started
## Settings
❗ When you first open (`Tools > Google Spreadsheets Parser`),  it automatically creates a `GoogleSpreadsheetSettings` asset in your `Resources` folder

Key settings to configure:
- Web Service URL: The URL from your Google AppScript
- Index File Path: Location of your `index.json` file (relative to Assets)
- Downloads Path: Destination folder for generated JSON files (relative to Assets)

![Settings](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/raw/images/settings.png)
![Window](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/raw/images/tool_window.png)

## Index.json
`index.json` file defines your spreadsheet structure and parser configurations. Place this file in the location specified in your settings
### Structure
```
{
  "SpreadsheetName": {
    "spreadsheet_id": "id_from_google_sheets_url",
    "sheets": {
      "SheetName": {
        "type": "parser_type"
      }
    }
  }
}
```
**Fields**
- `SpreadsheetName:` Display name in the editor window
- `spreadsheet_id:` ID from your Google Sheets URL
- `sheets:` Dictionary of sheet configurations
  - `Key:` Sheet name as it appears in Google Sheets
  - `type:` Parser type ("default", "complex", "key_value")
**Example**
```
{
  "Example Spreadsheet 1": {
    "spreadsheet_id": "your_spreadsheet_id",
    "sheets": {
      "Page1": {
        "type": "default"
      },
      "Page2": {
        "type": "default"
      }
    }
  },
  "Example Spreadsheet 2": {
    "spreadsheet_id": "your_spreadsheet_id",
    "sheets": {
      "Page1": {
        "type": "complex"
      },
      "Page2": {
        "type": "key_value"
      }
    }
  }
}
```


# Built-in Parsers
The plugin includes three built-in parsers for different data structures
### Default
The default parser ("type": "default") creates a simple key-value structure using the first column as keys

**Input spreadsheet:**
id      | name    | value
--------|---------|-------
item_1  | Sword   | 100
item_2  | Shield  | 50

**Output JSON:**
```
{
  "item_1": {
    "id": "item_1",
    "name": "Sword",
    "value": 100
  },
  "item_2": {
    "id": "item_2",
    "name": "Shield",
    "value": 50
  }
}
```
### Complex
The complex parser ("type": "complex") handles hierarchical data with categories

**Input spreadsheet:**
category | id      | name    | value
---------|---------|---------|-------
Weapons  | item_1  | Sword   | 100
‎         | item_2  | Axe     | 75
Armor    | item_3  | Shield  | 50
‎         | item_4  | Helmet  | 25

**Output JSON:**
```
{
  "Weapons": {
    "children": [
      {
        "id": "item_1",
        "name": "Sword",
        "value": 100
      },
      {
        "id": "item_2",
        "name": "Axe",
        "value": 75
      }
    ]
  },
  "Armor": {
    "children": [
      {
        "id": "item_3",
        "name": "Shield",
        "value": 50
      },
      {
        "id": "item_4",
        "name": "Helmet",
        "value": 25
      }
    ]
  }
}
```
### Key-Value
The key-value parser ("type": "key_value") expects two columns: key and value

**Input spreadsheet:**
key           | value
--------------|-------
player_speed  | 5.5
max_health    | 100
start_level   | 1

**Output JSON:**
```
{
  "player_speed": 5.5,
  "max_health": 100,
  "start_level": 1
}
```

# Custom Parser
Implement custom parsers by inheriting from `ISpreadsheetParser` or `SpreadsheetParserBase`. See [Custom Parser Documentation](https://github.com/Yggdrasills/GoogleSpreadsheetParser/blob/master/CustomParser.md) for details.

# Serializer
The plugin uses a JSON serializer that can be customized by implementing the `ISheetSerializer` interface:
```
public class CustomSerializer : ISheetSerializer 
{
    string ISheetSerializer.Serialize(SheetData data)
    {
        // Your custom serialization logic
        return JsonConvert.SerializeObject(data);
    }
}
```

To use your custom serializer, override the `GetSheetSerializer` method in the editor window:
```
protected override ISheetSerializer GetSheetSerializer()
{
    return new CustomSerializer();
}
```

By default, the plugin uses `JsonSheetSerializer` that:
- Automatically detects data types (numbers, booleans, strings)
- Supports arrays using comma-separated values and [] suffix in headers
- Preserves number precision
- Handles null values gracefully

**Example with array support:**
id      | tags[]           | values[]
--------|------------------|----------
item_1  | weapon,rare      | 100,150,200
item_2  | armor,common     | 50,75

**Generates:**
```
{
  "item_1": {
    "id": "item_1",
    "tags": ["weapon", "rare"],
    "values": [100, 150, 200]
  },
  "item_2": {
    "id": "item_2",
    "tags": ["armor", "common"],
    "values": [50, 75]
  }
}
```

