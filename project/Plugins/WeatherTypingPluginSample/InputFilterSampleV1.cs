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
using System.Linq;
using System.Text;
using com.denasu.WeatherTyping.plugin.helper;
using System;
using com.denasu.WeatherTyping.plugin.input;

namespace WeatherTypingSample
{
    /// <summary>
    /// Default Input Filter
    /// </summary>
    public class InputFilterSampleV1 : MarshalByRefObject, IInputFilter
    {
        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// The name of this plugin
        /// </summary>
        public virtual string Name
        {
            get
            {
                return "Sample";
            }
        }

        /// <summary>
        /// The author of this plugin
        /// </summary>
        public string Author
        {
            get
            {
                return "Sample";
            }
        }

        /// <summary>
        /// Memo
        /// </summary>
        public virtual string Memo
        {
            get
            {
                return "Sample Input Filter";
            }
        }

        /// <summary>
        /// Key map
        /// </summary>
        public List<List<KeyMap>> KeyMaps { set; get; }

        /// <summary>
        /// Supported Mode
        /// </summary>
        private static List<string> _supportedMode = new List<string>()
        {
            // Default Mode
            "Default",
        };

        /// <summary>
        /// Supported Mode
        /// </summary>
        public List<string> SupportedMode
        {
            get
            {
                return _supportedMode;
            }
        }

        /// <summary>
        /// Supported Keyboard
        /// </summary>
        private static List<InputKeyboard> _supportedKeyboard = new List<InputKeyboard>()
        {
            // Support Official 101 Keyboard
            new InputKeyboard()
            {
                Author = "Denasu System",
                Name = "101 Keyboard",
                Version = 1
            },
        };

        /// <summary>
        /// Supported Keyboard
        /// </summary>
        public List<InputKeyboard> SupportedKeyboard
        {
            get
            {
                return _supportedKeyboard;
            }
        }

        /// <summary>
        /// Get plane number corresponds to keyboard and mode.
        /// Return -1 if no plane exists.
        /// </summary>
        /// <param name="keyboard">Keyboard</param>
        /// <param name="mode">Mode</param>
        /// <returns></returns>
        public int GetPlane(InputKeyboard keyboard, string mode)
        {
            if (keyboard.Author == "Denasu System")
            {
                if (keyboard.Name == "101 Keyboard")
                {
                    return 0;
                }
            }

            return -1;
        }

        /// <summary>
        /// Create InputCustom instance
        /// InputCustom hold how to input a stroke which has multiple input path.
        /// </summary>
        /// <returns>Input Custom instance</returns>
        public IInputCustom CreateInputCustom()
        {
            return new InputCustomSampleV1();
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="keymap">Key map decrelared in XML</param>
        public InputFilterSampleV1()
        {
            // Init KeyMap
            KeyMaps = new List<List<KeyMap>>();
            KeyMaps.Add(new List<KeyMap>());
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
        /// Initialize Key Map
        /// </summary>
        public virtual void InitKeyMap()
        {
        }

        public static BitFlag Ope_NO_XN = new BitFlag(1) { Bits = 1L << 33 };

        /// <summary>
        /// Build automaton from word string
        /// </summary>
        /// <param name="word">Word string</param>
        /// <param name="keyboard">Keyboard</param>
        /// <returns>Automaton</returns>
        public virtual InputAutomaton ToAutomaton(string s, InputKeyboard keyboard)
        {
            InputAutomaton start = new InputAutomaton();
            InputAutomaton current = start;

            // You can input any character with "a" key!
            for (int i = 0; i < s.Length; i++)
            {
                var newAutomaton = new InputAutomaton();
                current.SetConnect((int)WTKeyCode.A, new InputAutomaton.Connect()
                {
                    Automaton = newAutomaton,
                    Character = "a",
                    Flags = Ope_NO_XN,
                });
                current = newAutomaton;
            }

            return start;
        }

        /// <summary>
        /// Build string from automaton.
        /// </summary>
        /// <param name="root">Root state</param>
        /// <param name="current">Current State</param>
        /// <param name="inputCustom">Input custom (do not modify)</param>
        /// <param name="pos">Return current position</param>
        /// <param name="nextCharacter">Return next valid character.</param>
        /// <returns>If nextCharacter is -1, Return only next character, otherwise, return all the string.</returns>
        public virtual string FromAutomaton(InputAutomaton root, InputAutomaton current, IInputCustom custom, ref int pos, ref int nextCharacter)
        {
            var result = new StringBuilder();
            pos = 0;

            InputAutomaton automaton = root;

            while (!(automaton.IsAccept()))
            {
                // Select first connection
                var select = automaton.GetConnect().First();

                int c2 = select.Key;
                string addstring = select.Value.Character;

                // In CPU mode, use only first key
                if (nextCharacter != -1)
                {
                    nextCharacter = c2;
                    break;
                }

                // Go to next automaton
                bool valid2 = false;
                result.Append(addstring);
                automaton = automaton.Input(c2, ref valid2, null);
                if (current == automaton)
                {
                    pos = result.Length;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Get key label from key code
        /// This string is used to show KeyArray
        /// </summary>
        /// <param name="plane">plane</param>
        /// <param name="physicalKey">Physical key code</param>
        /// <returns>Key label</returns>
        public virtual List<string> GetKeyLabel(int plane, int physicalKey)
        {
            var result = new List<string>();

            // Get key map by physical code
            var mapKey = KeyMaps[plane].Where(x => x.PhysicalKey == physicalKey);
            foreach (var key in mapKey)
            {
                // Convert to virtual code
                var virtualKey = key.VirtualKey;

                // Get string
                if (virtualKey != (int)WTKeyCode.A)
                {
                    continue;
                }

                // Always return A!
                result.Add("A");
            }

            return result;
        }

        /// <summary>
        /// Get virtual key code
        /// </summary>
        /// <param name="plane">Key plane</param>
        /// <param name="label">Key label</param>
        /// <returns>Virtual key code or -1</returns>
        public virtual int GetKeyCode(int plane, string label)
        {
            if (label != "A")
            {
                return -1;
            }

            // Always return A!
            return (int)WTKeyCode.A;
        }
    }
}
