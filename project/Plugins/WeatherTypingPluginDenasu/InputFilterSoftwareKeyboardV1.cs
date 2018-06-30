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

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Default Input Filter
    /// </summary>
    public class InputFilterSoftwareKeyboardV1 : MarshalByRefObject, IInputFilter
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
                return "SoftwareKeyboard";
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
        public virtual string Memo
        {
            get
            {
                return "Software Keyboard Input Filter";
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
            // Support Official Software Keyboard
            new InputKeyboard()
            {
                Author = "Denasu System",
                Name = "Software Keyboard",
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
            return 0;
        }

        /// <summary>
        /// Create InputCustom instance
        /// InputCustom hold how to input a stroke which has multiple input path.
        /// </summary>
        /// <returns>Input Custom instance</returns>
        public IInputCustom CreateInputCustom()
        {
            return new InputCustomDefaultV1();
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="keymap">Key map decrelared in XML</param>
        public InputFilterSoftwareKeyboardV1()
        {
            // Init KeyMap
            KeyMaps = new List<List<KeyMap>>();
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

        /// <summary>
        /// Build automaton from word string
        /// </summary>
        /// <param name="word">Word string</param>
        /// <param name="keyboard">Keyboard</param>
        /// <returns>Automaton</returns>
        public virtual InputAutomaton ToAutomaton(string s, InputKeyboard keyboard)
        {
            // Start state
            InputAutomaton start = new InputAutomaton();
            InputAutomaton current = start;
            string c, c_conv;

            var s_conv = s.Kana2Hira();
            s_conv = s.Zenkaku2Hankaku(true, true);

            // Make automaton
            for (int i = 0; i < s.Length; i++)
            {
                InputAutomaton next;

                // Get next input character
                c = s.Substring(i, 1);
                c_conv = s_conv.Substring(i, 1);

                // Use original 
                next = new InputAutomaton();
                current.SetConnect(c_conv[0], new InputAutomaton.Connect()
                {
                    Automaton = next,
                    Flags = InputCustomDefaultV1.Ope_NO_CUSTOM,
                    Character = c,
                });
                current = next;
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
                int c = '\0';
                string addstring = "";

                // If we have perfect input custom, use that
                foreach (var i in automaton.GetConnect())
                {
                    if (custom.IsSet(i.Value.Flags))
                    {
                        c = i.Key;
                        addstring = i.Value.Character;
                        break;
                    }
                }

                if (c == '\0')
                {
                    // Found no route, use the first route
                    c = automaton.GetConnect().First().Key;
                    addstring = automaton.GetConnect().First().Value.Character;
                }

                // In CPU mode, use only first key
                if (nextCharacter != -1)
                {
                    nextCharacter = c;
                    break;
                }

                // Go to next automaton
                bool valid = false;
                result.Append(addstring);
                automaton = automaton.Input(c, ref valid, null);
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
            return (int)WTKeyCode.NoKey;
        }
    }
}
