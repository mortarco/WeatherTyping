/*
 * Copyright 2017 Denasu System
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using System.Collections.Generic;
using com.denasu.WeatherTyping.plugin.helper;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Key convert for Software Keyboard
    /// </summary>
    public class KeyFilterSoftwareKeyboardV1 : MarshalByRefObject, IKeyFilter
    {
        /// <summary>
        /// Version
        /// </summary>
        public int Version
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// The name of this plugin
        /// </summary>
        public string Name
        {
            get
            {
                return "Software Keyboard";
            }
        }

        /// <summary>
        /// The author of this plugin
        /// </summary>
        public string Author
        {
            get
            {
                return "Denasu System";
            }
        }

        /// <summary>
        /// Memo
        /// </summary>
        public string Memo
        {
            get
            {
                return "Software Keyboard key converter";
            }
        }

        /// <summary>
        /// Language
        /// </summary>
        public List<string> Language
        {
            get
            {
                // Neutral
                return new List<string>() { };
            }
        }


        /// <summary>
        /// Returns key map names
        /// </summary>
        public List<string> GetNames()
        {
            // No customization
            return new List<string>();
        }

        /// <summary>
        /// Set InputMethod Info
        /// </summary>
        /// <param name="author">Author</param>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        public void InputMethodInfo(string author, string name, int version)
        {
        }

        /// <summary>
        /// Create KeyCustom instance
        /// </summary>
        /// <returns>KeyCustom instance</returns>
        public IKeyCustom CreateKeyCustom()
        {
            // No customization
            return new KeyCustomDefaultV1();
        }

        /// <summary>
        /// Convert physical key to virtual key
        /// WT calls this function before sending input data to other players
        /// </summary>
        /// <param name="version">Key Filter version</param>
        /// <param name="physicalKey">Pressed physical key code defined in KeyCode class</param>
        /// <param name="press">Indicate the key is pressed</param>
        /// <param name="currentTime">Pressed system time</param>
        /// <param name="keyCustom">Key custom</param>
        /// <returns>Virtual key codes defined in WTKeyCode class</returns>
        public List<int> Convert(int physicalKey, bool pressed, long currentTime, IKeyCustom keyCustom)
        {
            List<int> result = new List<int>();

            if (pressed)
            {
                // In Software Keyboard filter, physicalKey is actual character code.
                // We convert Zenkaku to Hankaku here to enable input Zenkaku character in screen keyboard
                string s = ((char)physicalKey).ToString().Zenkaku2Hankaku(true, true);
                result.Add(s[0]);
            }

            return result;
        }
    }
}
