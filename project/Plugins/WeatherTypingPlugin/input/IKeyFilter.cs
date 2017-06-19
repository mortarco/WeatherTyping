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

using System.Collections.Generic;
using com.denasu.Common.Plugin;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Plugin for key filter
    /// </summary>
    public interface IKeyFilter : IPlugin
    {
        /// <summary>
        /// Returns key map names
        /// </summary>
        List<string> GetNames();

        /// <summary>
        /// Set InputMethod Info
        /// </summary>
        /// <param name="author">Author</param>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        void InputMethodInfo(string author, string name, int version);

        /// <summary>
        /// Create KeyCustom instance
        /// The plugin author declares the key which can be assigned by users
        /// WT assins physical keys for the declared keys by special UI.
        /// </summary>
        /// <returns>KeyCustom instance</returns>
        IKeyCustom CreateKeyCustom();

        /// <summary>
        /// Convert physical key to virtual key
        /// WT calls this function before sending input data to other players
        /// </summary>
        /// <param name="physicalKey">Pressed physical key code defined in KeyCode class</param>
        /// <param name="press">Indicate the key is pressed</param>
        /// <param name="currentTime">Pressed system time</param>
        /// <param name="keyCustom">Key custom</param>
        /// <returns>Virtual key codes defined in WTKeyCode class</returns>
        List<int> Convert(int physicalKey, bool pressed, long currentTime, IKeyCustom keyCustom);
    }
}
