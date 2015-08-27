// <copyright file="SampleGUI.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using UnityEngine;
using Google.SmartLock;

namespace Credentials
{
    /// <summary>
    /// Sample GUI for demonstrating the Credentials API.
    /// </summary>
    public class SampleGUI : MonoBehaviour
    {

        // initialize the current credential to be empty.
        private ICredential currentCredential = new Credential("", "", "", "", "");
        private string msg = "";
        private Texture2D profilePic;
        private WWW www;

        void OnGUI()
        {
            // Adjust the size to look OK on different devices
            GUI.skin.label.fontSize = Screen.height / 32;
            GUI.skin.label.wordWrap = false;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.skin.textField.fontSize = Screen.height / 32;
            GUI.skin.button.fontSize = Screen.height / 24;
            float width = Screen.width * .95f;
            float rowHeight = Screen.height / 6f;

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(Screen.height));

            GUILayout.Label("SmartLock Credentials", GUILayout.Height(rowHeight));
            GUILayout.Label(currentCredential != null ? currentCredential.Name : "");

            // Profile picture
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(ProfilePic);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Email address
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(currentCredential != null ? currentCredential.ID : "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Edit the email (id) of the credential.
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID (Email):", GUILayout.Width(Screen.width / 3f));
            currentCredential.ID = GUILayout.TextField(currentCredential.ID,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(rowHeight * .25f));
            GUILayout.EndHorizontal();

            // Edit the password.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Password:", GUILayout.Width(Screen.width / 3f));
            currentCredential.Password = GUILayout.PasswordField(
                currentCredential.Password != null ? currentCredential.Password : "", "*"[0], 25,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(rowHeight * .25f));
            GUILayout.EndHorizontal();

            //Some space then the status message.
            GUILayout.FlexibleSpace();
            GUILayout.Label(msg);

            // Button bar for commands.
            GUILayout.BeginHorizontal(GUILayout.Height(rowHeight));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load"))
            {
                SmartLockCredentials.Instance.Load(
                    (status, credential) =>
                    {
                        msg = "Got Status: " + status;
                        Debug.Log("Got Status: " + status + ": " + credential);
                        // if the status is success, a real app would use the
                        // credential to sign into the other site.  Here, we
                        // just display them.
                        if (credential != null)
                        {
                            currentCredential = credential;
                        }
                    });
            }
            GUILayout.Space(50);
            if (GUILayout.Button("Save"))
            {
                // A real app would probably call save after the authentication
                // to the other site.  Here is just a button for the demo.
                //
                // Note: you can only save the accountType or the password.
                if (!string.IsNullOrEmpty(currentCredential.Password)) {
                     currentCredential.AccountType = null;
                }
                SmartLockCredentials.Instance.Save(currentCredential,
                    (status, credential) =>
                    {
                        msg = "Got Status: " + status;
                        Debug.Log("Got Status: " + status + ": " + credential);
                    });
            }
            GUILayout.Space(50);
            if (GUILayout.Button("Delete"))
            {
                // Nice to allow the user to delete or "forget me"
                SmartLockCredentials.Instance.Delete(currentCredential,
                    (status, credential) =>
                    {
                        msg = "Got Status: " + status;
                        Debug.Log("Got Status: " + status + ": " + credential);
                        currentCredential = new Credential("", "", "", "", "");
                    });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Gets the profile pic.
        /// </summary>
        /// <value>The profile pic.</value>
        Texture2D ProfilePic
        {
            get
            {
                if (currentCredential != null && profilePic == null &&
                    !string.IsNullOrEmpty(currentCredential.ProfilePictureURL))
                {
                    if (www == null || www.url != currentCredential.ProfilePictureURL)
                    {
                        www = new WWW(currentCredential.ProfilePictureURL);
                    }
                    if (profilePic == null && www.isDone)
                    {
                        profilePic = www.texture;
                    }
                }
                return profilePic;
            }
        }
    }
}
