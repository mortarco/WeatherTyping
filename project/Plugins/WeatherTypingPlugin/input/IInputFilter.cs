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
using System;

namespace com.denasu.WeatherTyping.plugin.input
{
    // KeyMap struct
    [Serializable]
    public struct KeyMap
    {
        /// <summary>
        /// Virtual Key (Unicode character code)
        /// </summary>
        public int VirtualKey { set; get; }

        /// <summary>
        /// Physical Key (WTKeyCode)
        /// </summary>
        public int PhysicalKey { set; get; }
    }

    /// <summary>
    /// Plugin for input filter
    /// </summary>
    public interface IInputFilter : IPlugin
    {
        /// <summary>
        /// Set InputMethod Info
        /// </summary>
        /// <param name="author">Author</param>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        void InputMethodInfo(string author, string name, int version);

        /// <summary>
        /// Get keymap corresponds to plane
        /// </summary>
        /// <returns>KeyMap for each plane</returns>
        List<List<KeyMap>> KeyMaps { set; get; }

        /// <summary>
        /// Supported Mode
        /// </summary>
        List<string> SupportedMode { get; }

        /// <summary>
        /// Supported Keyboard
        /// </summary>
        List<InputKeyboard> SupportedKeyboard { get; }

        /// <summary>
        /// Get plane number corresponds to keyboard and mode.
        /// Return -1 if no plane exists.
        /// </summary>
        /// <param name="keyboard">Keyboard Type</param>
        /// <param name="mode">Mode</param>
        /// <returns></returns>
        int GetPlane(InputKeyboard keyboard, string mode);

        /// <summary>
        /// Create InputCustom instance
        /// InputCustom hold how to input a stroke which has multiple input path.
        /// </summary>
        /// <returns>Input Custom instance</returns>
        IInputCustom CreateInputCustom();

        /// <summary>
        /// Initialize
        /// </summary>
        void InitKeyMap();

        /// <summary>
        /// Build automaton from word string
        /// </summary>
        /// <param name="word">Word string</param>
        /// <param name="keyboard">Keyboard</param>
        /// <returns>Automaton</returns>
        InputAutomaton ToAutomaton(string word, InputKeyboard keyboard);

        /// <summary>
        /// Build string from automaton.
        /// </summary>
        /// <param name="root">Root state</param>
        /// <param name="current">Current State</param>
        /// <param name="inputCustom">Input custom (do not modify)</param>
        /// <param name="pos">Return current position</param>
        /// <param name="nextCharacter">Return next valid character.</param>
        /// <returns>If nextCharacter is -1, Return only next character, otherwise, return all the string.</returns>
        string FromAutomaton(InputAutomaton root, InputAutomaton current, IInputCustom inputCustom, ref int pos, ref int nextCharacter);

        /// <summary>
        /// Get key label from key code
        /// This string is used to show KeyArray
        /// </summary>
        /// <param name="plane">plane</param>
        /// <param name="physicalKey">Physical key code</param>
        /// <returns>Key label</returns>
        List<string> GetKeyLabel(int plane, int physicalKey);

        /// <summary>
        /// Get virtual key code
        /// </summary>
        /// <param name="plane">Key plane</param>
        /// <param name="label">Key label</param>
        /// <returns>Virtual key code or -1</returns>
        int GetKeyCode(int plane, string label);
    }
}
