// Copyright (C) 2017 The Regents of the University of California (Regents).
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//
//     * Neither the name of The Regents or University of California nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Please contact the author of this library if you have any questions.
// Author: Nikolas Chaconas (nikolas.chaconas@gmail.com)

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
