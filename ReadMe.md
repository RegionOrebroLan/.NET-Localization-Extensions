# .NET-Localization-Extensions

Localization additions and extensions for .NET.

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.Localization.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.Localization)

## 1 Path-based localization
Path-based localization implemented with JSON- and XML-localization.

Built on ASP.NET Core localization:
- [Globalization and localization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization/)
- [GitHub](https://github.com/aspnet/Extensions/tree/master/src/Localization/)

The functional idea behind it:
- [EPiServer: Localizing the user interface](https://world.episerver.com/documentation/developer-guides/CMS/globalization/localizing-the-user-interface/)

### 1.1 Features

- **Localizable strings (translations) in json- and/or xml-files** - Manage your localizable strings in [json-](/Source/Embedded-resources/Colors/Colors.en.json) and/or [xml-files](/Source/Embedded-resources/Animals/Animals.en.xml)
- **Localizable strings in embedded- and/or file-resources** - Both embedded- and file-resources are supported.
- **Caching and changable at runtime** - Localizable strings can be changed, added and removed at runtime. Resources and resource-entries are cached and if changes occurs the cache is cleared.
- **Caching and NOT changable at runtime** - If we don't want a dynamic cache we can use a static cache. To get our changed resources, we need an application restart. That is, we need an application restart to clear the cache. Regarding configuration, see below.
- **Previously declared localizable strings can be overrided** - A value for a path can be overridden with the same path in another or the same resource. The last path will be the one that applies. Embedded-resources are read in the order they are added and file-resources are read in normal file-system order. Embedded-resources are read before file-resources. The order can be changed by setting a [priority-attribute](/Source/Embedded-resources/Prioritized-words/Prioritized-words.json#L6). The resource with the highest priority will be read last.
- **Lookup** - A value for a path can be looked up on another path. If an entry have both a lookup-value and a value, the value will apply.
- **Rooted path for typed string-localizer** - If you have a typed string-localizer, eg. IStringLocalizer&lt;HomeController&gt;, you can still get localized strings by the full path with a leading colon, eg localizer[":the.full.path"].
- **Case-insensitive path declaration** - Path declaration is case-insensitive and path-separators are ".", "/" or "\\".
- **Resource-names** - The name of a resource does not affect the culture. The culture is set in the resource.
- **Root-namespace-attribute can be used** - [Example](/Source/Embedded-resources/Root-namespaced-resources/Properties/Assembly-Information.cs#L3)

### 1.2 Configuration & settings

#### 1.2.1 [Localization-options](/Source/Project/Configuration/LocalizationOptions.cs#L5)

- [AlphabeticalSorting](/Source/Project/Configuration/LocalizationOptions.cs#L17) - Defaults to true. If true, sorts all localized strings alphabetically when calling IStringLocalizer.GetAllStrings(). If false, localized strings will be ordered as they are read from resources when calling IStringLocalizer.GetAllStrings().
- [EmbeddedResourceAssemblies](/Source/Project/Configuration/LocalizationOptions.cs#L22) - Items can be an assembly-name, assembly-name-pattern (MyAssembly*) or an assembly-fullname.
- [FileResourcesDirectoryPath](/Source/Project/Configuration/LocalizationOptions.cs#L27) - Defaults to null. Absolute or application-relative path to the directory to scan for file-resources. If empty, the application-root will be used. If null, file-resources are disabled.
- [IncludeParentCultures](/Source/Project/Configuration/LocalizationOptions.cs#L29) - Defaults to false. If fallback to parent-cultures should be included when asking for localized strings.
- [ThrowErrors](/Source/Project/Configuration/LocalizationOptions.cs#L30) - Defaults to false. If set to true and a resource can not be parsed, the application will throw an exception. Whatever value is set, exceptions are always logged.

##### 1.2.1.1 Remarks
To get the **FileResourcesDirectoryPath** to be null it have to be absent from the configuration-file". This want work:

	{
		"Localization": {
			"FileResourcesDirectoryPath": null
		}
	}

#### 1.2.2 Configuration

AppSettings.json example:

    {
	    "Localization": {
		    "AlphabeticalSorting": true,
		    "EmbeddedResourceAssemblies": [
			    "Animals",
			    "Colors",
			    "Numbers",
			    "Prioritized-words",
			    "Root-namespaced-resources",
			    "Words"
		    ],
		    "FileResourcesDirectoryPath": "Resources",
		    "IncludeParentCultures": false,
		    "ThrowErrors": false
	    }
    }

### 1.3 Setup

Example: [Web-application startup](/Samples/Path-Based-Localization/Program.cs#L22)

#### 1.3.1 With AppSettings.json

	...

	builder.Services.AddPathBasedLocalization(this.Configuration);

	...

##### 1.3.1.1 If we want to use a static cache - AppSettings.json

    {
	    "LocalizationDependencyInjection": {
		    "StaticCache": true
	    }
    }

#### 1.3.2 Action

	...

	builder.Services.AddPathBasedLocalization(localizationOptions =>
    {
		localizationOptions.AlphabeticalSorting = true;
		localizationOptions.EmbeddedResourceAssemblies.Add("Assembly-name"); // Can be an assembly-name, assembly-name-pattern (MyAssembly*) or an assembly-fullname. 
		localizationOptions.FileResourcesDirectoryPath = "Resources";
    });

	...

##### 1.3.2.1 If we want to use a static cache

	...

	builder.Services.AddPathBasedLocalization(localizationOptions =>
    {
		localizationOptions.AlphabeticalSorting = true;
		localizationOptions.EmbeddedResourceAssemblies.Add("Assembly-name"); // Can be an assembly-name, assembly-name-pattern (MyAssembly*) or an assembly-fullname. 
		localizationOptions.FileResourcesDirectoryPath = "Resources";
    }, true);

	...

#### 1.3.3 Default

    ...

	builder.Services.AddPathBasedLocalization();

	...

##### 1.3.3.1 If we want to use a static cache

    ...

	builder.Services.AddPathBasedLocalization(true);

	...

### 1.4 Examples

#### 1.4.1 Code

IStringLocalizer:
- localizer["root.firstLevel.secondLevel.value"] => "Translated value"
- localizer["root/firstLevel/secondLevel/value"] => "Translated value"
- localizer["root\firstLevel\secondLevel\value"] => "Translated value"
- localizer["common.yes"] => "Yes"
- localizer["common/yes"] => "Yes"
- localizer["common\yes"] => "Yes"

IStringLocalizer&lt;T&gt;, example where T is Company.WebApplication.Controllers.HomeController:
- localizer["Label"] => "Translated value for label" (the path behind is company.webapplication.controllers.homecontroller.label, alternatively controllers.homecontroller.label)
- localizer[":Common.Yes"] => "Yes" (rooted path to common.yes)

#### 1.4.2 Lookup
A localization-entry can be a lookup to another entry:

##### 1.4.2.1 JSON

    {
	    "Cultures": [
		    {
			    "Culture": {
				    "Name": "en",
				    "Nodes": [
					    {
						    "Node": {
							    "Name": "Common",
							    "Entries": {
								    "Yes": {
									    "Value": "Yes"
								    }
							    }
						    }
					    },
					    {
						    "Node": {
							    "Name": "Company.WebApplication.Controllers.HomeController",
							    "Entries": {
								    "Yes": {
									    "Lookup": "Common.Yes"
								    }
							    }
						    }

					    }
				    ]
			    }
		    }
	    ]
    }

##### 1.4.2.2 XML

    <cultures>
	    <culture name="en">
		    <common>
			    <yes>Yes</yes>
		    </common>
		    <company>
			    <webApplication>
				    <controllers>
						<homeController>
							<yes lookup="common.yes" />
						</homeController>
				    </controllers>
			    </webApplication>
		    </company>
	    </culture>
    </cultures>

#### 1.4.3 Embedded resources

- [Animals](/Embedded-resources/Animals/)
- [Colors](/Embedded-resources/Colors/)
- [Numbers](/Embedded-resources/Numbers/)
- [Prioritized-words](/Embedded-resources/Prioritized-words/)
- [Root-namespaced-resources](/Embedded-resources/Root-namespaced-resources/)
- [Words](/Embedded-resources/Words/)

#### 1.4.4 Sample-applications

- [Path-Based-Localization-Web-Application](/Samples/Path-Based-Localization/): Web-application with path-based localization
- [Resx-Localization-Web-Application](/Samples/Resx-Localization/): Web-application with RESX-localization

## 2 Development

### 2.1 Signing

Drop the "StrongName.snk" file in the repository-root. The file should not be included in source control.