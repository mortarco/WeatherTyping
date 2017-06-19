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
using System.Collections.Generic;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Key custom
    /// </summary>
    internal class KeyCustomNICOLAV1 : MarshalByRefObject, IKeyCustom
    {
        Dictionary<string, WTKeyCode> _keyMap = new Dictionary<string, WTKeyCode>()
            {
                { KeyFilterNICOLAV1.LSNAME, WTKeyCode.ImeNonConvert },
                { KeyFilterNICOLAV1.RSNAME, WTKeyCode.ImeConvert },
            };

        /// <summary>
        /// Add key map
        /// </summary>
        /// <param name="name">Key map name</param>
        /// <param name="keyCode">Key code</param>
        public void SetMappedKey(string name, WTKeyCode keyCode)
        {
            _keyMap[name] = keyCode;
        }

        /// <summary>
        /// Get key map
        /// </summary>
        /// <param name="name">Key map name</param>
        /// <returns>Key code</returns>
        public WTKeyCode GetMappedKey(string name)
        {
            return _keyMap[name];
        }
    }
}
