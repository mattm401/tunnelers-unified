//
//  SmartLocalizationWindow.cs
//
// Copyright (c) 2013 Niklas Borglund. All rights reserved.
// @NiklasBorglund

using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class SmartLocalizationWindow : EditorWindow
{
#region Members
	/// <summary>A list of all the available languages </summary>
	List<CultureInfo> availableLanguages = new List<CultureInfo>();
	/// <summary>A list of all the languages not available</summary>
	List<CultureInfo> notAvailableLanguages = new List<CultureInfo>();
	/// <summary>For the create new languages popup</summary>
	List<string> notAvailableLanguagesEnglishNames = new List<string>();
	/// <summary>Popup index for create new language</summary>
	int chosenCreateNewCultureValue;
	
	//References to the possible GUI-windows that can be created
	EditRootLanguageFileWindow editRootWindow;
	TranslateLanguageWindow translateLanguageWindow;
	
	///<summary>Popup index for create new language</summary>
	Vector2 scrollPosition = Vector2.zero;

	///<summary>The undo manager</summary>
	HOEditorUndoManager undoManager;
	
	//Microsoft Translator Variables
	///<summary>The translator manager</summary>
	[SerializeField]
	MicrosoftTranslatorManager microsoftTranslator;
	[SerializeField]
	string mtCliendID = "";
	[SerializeField]
	string mtCliendSecret = "";
	[SerializeField]
	bool keepTranslatorAuthenticated;
#endregion

#region Properties
	/// <summary>
	/// Gets the microsoft translator manager
	/// </summary>
	/// <value>
	/// The microsoft translator.
	/// </value>
	public MicrosoftTranslatorManager MicrosoftTranslator
	{
		get
		{
			return microsoftTranslator;	
		}
	}
	/// <summary>A list of all the available(created) languages </summary>
	public List<CultureInfo> AvailableLanguages
	{
		get
		{
			return availableLanguages;	
		}
	}
	public string MicrosoftTranslator_ClientID
	{
		get
		{
			return mtCliendID;	
		}
	}
	public string MicrosoftTranslator_ClientSecret
	{
		get
		{
			return mtCliendSecret;	
		}
	}
#endregion

	void OnEnable()
	{
		Initialize();
	}

	void OnFocus()
	{
		Initialize();
	}

	void OnProjectChange()
	{
		Initialize();
	}

	public void Initialize()
	{
		// Instantiate Undo Manager
		if(undoManager == null)
		{
			undoManager = new HOEditorUndoManager( this, "Smart Localization - Main Window" );
		}
		if(microsoftTranslator == null)
		{
			microsoftTranslator = new MicrosoftTranslatorManager();

			//cws == Cry Wolf Studios
			//mt == Microsoft Translator
			if(EditorPrefs.HasKey("cws_mtClientID") && EditorPrefs.HasKey("cws_mtClientSecret") && EditorPrefs.HasKey("cws_mtKeepAuthenticated"))
			{
				mtCliendID = EditorPrefs.GetString("cws_mtClientID");
				mtCliendSecret = EditorPrefs.GetString("cws_mtClientSecret");
				keepTranslatorAuthenticated = EditorPrefs.GetBool("cws_mtKeepAuthenticated");
			}

			//Authenticate on enable
			if(keepTranslatorAuthenticated)
			{
				microsoftTranslator.GetAccessToken(mtCliendID, mtCliendSecret);
			}
		}

		if(availableLanguages == null || availableLanguages.Count < 1)
		{
			LocFileUtility.CheckAvailableLanguages(availableLanguages,notAvailableLanguages,notAvailableLanguagesEnglishNames);
		}
	}
	
	void OnGUI()
	{
		if(EditorWindowUtility.ShowWindow())
		{
			GUILayout.Label ("Settings", EditorStyles.boldLabel);
			if(!LocFileUtility.CheckIfRootLanguageFileExists())
			{
        		if (GUILayout.Button("Create New Localization System"))
        		{
					LocFileUtility.CreateRootResourceFile();
				}
			}
			else
			{
				undoManager.CheckUndo();
				if (GUILayout.Button("Refresh"))
        		{
					LocFileUtility.CheckAvailableLanguages(availableLanguages,notAvailableLanguages,notAvailableLanguagesEnglishNames);
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label ("Microsoft Translator Settings", EditorStyles.boldLabel, GUILayout.Width(200));
				if(microsoftTranslator.IsInitialized)
				{
					GUILayout.Label (" - Authenticated!", EditorStyles.miniLabel);
				}
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Client ID:", GUILayout.Width(70));
				mtCliendID = EditorGUILayout.TextField(mtCliendID);
				GUILayout.Label("Client Secret:", GUILayout.Width(100));
				mtCliendSecret = EditorGUILayout.TextField(mtCliendSecret);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Save", GUILayout.Width(50)))
				{
					SaveMicrosoftTranslatorSettings();
					if(!microsoftTranslator.IsInitialized)
					{
						microsoftTranslator.GetAccessToken(mtCliendID, mtCliendSecret);
					}
				}
				if(!microsoftTranslator.IsInitialized)
				{
					if(GUILayout.Button("Authenticate!", GUILayout.Width(150)))
					{
						microsoftTranslator.GetAccessToken(mtCliendID, mtCliendSecret);
					}
				}
				keepTranslatorAuthenticated = EditorGUILayout.Toggle("Keep Authenticated", keepTranslatorAuthenticated);
				EditorGUILayout.EndHorizontal();
			
				GUILayout.Label ("Edit Root Language File", EditorStyles.boldLabel);
				if (GUILayout.Button("Edit"))
        		{
					ShowRootEditWindow(LocFileUtility.LoadParsedLanguageFile(null));
				}
			
				GUILayout.Label ("Create new language", EditorStyles.boldLabel);
				chosenCreateNewCultureValue = EditorGUILayout.Popup(chosenCreateNewCultureValue, notAvailableLanguagesEnglishNames.ToArray());
				if (GUILayout.Button("Create Language"))
        		{
					CreateNewLanguage(notAvailableLanguagesEnglishNames[chosenCreateNewCultureValue]);
				}
		
				GUILayout.Label ("Translate Languages", EditorStyles.boldLabel);
				//Start the scroll view
				scrollPosition = GUILayout.BeginScrollView(scrollPosition);
				bool languageDeleted = false;
				foreach(CultureInfo info in availableLanguages)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(info.EnglishName + " - " + info.Name))
        			{
						//Open language edit window
						ShowTranslateWindow(info);
					}
					if(GUILayout.Button("Delete", GUILayout.Width(60)))
					{
						LocFileUtility.DeleteLanguage(info);
						languageDeleted = true;
						break;
					}
					EditorGUILayout.EndHorizontal();
				}

				if(languageDeleted) //Refresh
				{
					LocFileUtility.CheckAvailableLanguages(availableLanguages,notAvailableLanguages,notAvailableLanguagesEnglishNames);
				}

				//End the scroll view
				GUILayout.EndScrollView();
				undoManager.CheckDirty();
			}
		}
	}
	
#region Saving and Loading ResX Files
	/// <summary>
	/// Creates a new language. Gets the languagename and passes it over to LocFileUtility.CreateNewLanguage
	/// </summary>
	void CreateNewLanguage(string englishName)
	{
		string languageName = "ERROR";
		//Find the chosen culture name
		foreach(CultureInfo info in notAvailableLanguages)
		{
			if(info.EnglishName == englishName)
			{
				languageName = info.Name;
				break;
			}
		}
		
		if(languageName == "ERROR")
		{
			Debug.Log("ERROR:Couldn't create the language: " + englishName);
			return;
		}	
		LocFileUtility.CreateNewLanguage(languageName);
		
		//Update the available languages
		LocFileUtility.CheckAvailableLanguages(availableLanguages,notAvailableLanguages,notAvailableLanguagesEnglishNames);
	}
#endregion	
#region Translator Settings (Microsoft Translator)
	/// <summary>
	/// Saves the microsoft translator settings in EditorPrefs
	/// Keys = cws_mtCliendID, cws_mtCliendSecret
	/// </summary>
	void SaveMicrosoftTranslatorSettings()
	{
		EditorPrefs.SetString("cws_mtClientID", mtCliendID);
		EditorPrefs.SetString("cws_mtClientSecret", mtCliendSecret);
		EditorPrefs.SetBool("cws_mtKeepAuthenticated", keepTranslatorAuthenticated);
	}
#endregion
		
#region Get/Set Windows
	void ShowRootEditWindow(Dictionary<string,LocalizedObject> rootValues)
	{
		if(editRootWindow == null)
		{
			editRootWindow = EditRootLanguageFileWindow.ShowWindow();
			editRootWindow.ShowTab();
		}
		else
		{
			editRootWindow.SetRootValues(rootValues);
			editRootWindow.ShowTab();
		}
	}
	void ShowTranslateWindow(CultureInfo info)
	{
		if(translateLanguageWindow == null)
		{
			translateLanguageWindow = TranslateLanguageWindow.ShowWindow(info,this);
			translateLanguageWindow.ShowTab();
		}
		else
		{
			translateLanguageWindow.Initialize (info, false);
			translateLanguageWindow.ShowTab();
		}
	}
	//Show window
    [MenuItem("Window/Smart Localization")]
    public static void ShowWindow()
    {
        SmartLocalizationWindow thisWindow = (SmartLocalizationWindow)EditorWindow.GetWindow(typeof(SmartLocalizationWindow));
        thisWindow.title = "Smart Localization";
		thisWindow.Initialize();
    }
#endregion
}
