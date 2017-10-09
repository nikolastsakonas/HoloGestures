using UnityEngine;
using HoloToolkit.Unity;

using System.Text;
using System.Collections.Generic;
using System.Threading;

#if !UNITY_EDITOR
using System;
using Windows.Storage;
using System.Collections.Generic;
using System.Collections.Concurrent;
#endif

/*
 *
 * THANK YOU ADAM AND YUN SUK FOR THIS SCRIPT
 *
 *
 */

public class UserStudyLogger : Singleton<UserStudyLogger>
{

    public float samplesPerSecond = 1.0f;
    public float writeToFilePerSecond = 1.0f;
    public string FileName = "UserData";
	public string FileNameExtraTime = "UserDataExtra";
    public string FolderName = "UserData";

    private bool EnableRecording = false;
    private float samplingDeltaTime = 0;
    private float writeToFileDeltaTime = 0;

#if !UNITY_EDITOR
    private StorageFolder saveFolder;
    public StorageFile saveFile, saveFileExtended;
    private bool haveFolderPath = false, haveFilePath = false;
    private ConcurrentQueue<Tuple<Vector3, Vector3>> transformQueue = new ConcurrentQueue<Tuple<Vector3, Vector3>>();
    private static SemaphoreSlim writeLogLock = new SemaphoreSlim(1, 1);
	private static SemaphoreSlim writeLogExtendedLock = new SemaphoreSlim(1, 1);
#endif

    void Start()
    {
    }

	public void OpenLogFile(string userID)
	{
		#if !UNITY_EDITOR
		CreateStorageFolderAndFile(userID);
		#endif
	}
		
    public void CreateLogFile(string userID)
    {
#if !UNITY_EDITOR
        OpenStorageFolderAndFile(userID);
#endif
    }

#if !UNITY_EDITOR
    private async void OpenStorageFolderAndFile(string userID)
    {
        saveFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        // http://stackoverflow.com/questions/33801760/universal-windows-platform-zipfile-createfromdirectory-creates-empty-zip-file
        saveFolder = await saveFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
        haveFolderPath = true;
        saveFile = await saveFolder.CreateFileAsync(FileName + userID + ".csv", CreationCollisionOption.GenerateUniqueName);
        await Windows.Storage.FileIO.AppendLinesAsync(saveFile, new string[] { TwoHandedUserStudyManager.loggingTitles });

		saveFileExtended = await saveFolder.CreateFileAsync(FileNameExtraTime + userID + ".csv", CreationCollisionOption.GenerateUniqueName);
        await Windows.Storage.FileIO.AppendLinesAsync(saveFileExtended, new string[] { TwoHandedUserStudyManager.loggingTitlesExtra });
        haveFilePath = true;
    }

	private async void CreateStorageFolderAndFile(string userID)
	{
		saveFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

		// http://stackoverflow.com/questions/33801760/universal-windows-platform-zipfile-createfromdirectory-creates-empty-zip-file
		saveFolder = await saveFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
		haveFolderPath = true;
		saveFile = await saveFolder.CreateFileAsync(FileName + userID + ".csv", CreationCollisionOption.OpenIfExists);
		await Windows.Storage.FileIO.AppendLinesAsync(saveFile, new string[] { TwoHandedUserStudyManager.loggingTitles });

		saveFileExtended = await saveFolder.CreateFileAsync(FileNameExtraTime + userID + ".csv", CreationCollisionOption.OpenIfExists);
		await Windows.Storage.FileIO.AppendLinesAsync(saveFileExtended, new string[] { TwoHandedUserStudyManager.loggingTitlesExtra });
		haveFilePath = true;
	}

    private async void Record(string[] content, int saveFileType)
    {
	   
		if (!haveFolderPath || !haveFilePath)
        {
            return;
        }

        if (content.Length > 0)
        {
            string[] copiedContent = new string[content.Length];

            for (int i = 0; i < content.Length; i++)
            {
                copiedContent[i] = content[i][0].ToString();
                copiedContent[i] += content[i].Substring(1, content[i].Length - 1);
            }

            if (saveFileType == 0) { 
				await writeLogLock.WaitAsync();

				try
				{
					await Windows.Storage.FileIO.AppendLinesAsync(saveFile, content);
				}
				finally
				{
					writeLogLock.Release();
				}
			} else {
				await writeLogExtendedLock.WaitAsync();
                
	            try
	            {
                    

                    await Windows.Storage.FileIO.AppendLinesAsync(saveFileExtended, copiedContent);
	            }
	            finally
	            {
					writeLogExtendedLock.Release();
	            }
			}
        }
    }
#endif

	public void Record(string[] content)
	{
		#if !UNITY_EDITOR
		//Record(content, 1);
		#endif
	}

    public void Record(string content, int saveFileType)
    {
#if !UNITY_EDITOR
        //Record(new string[]{content}, saveFileType);
#endif
    }
}
