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
    public class InputFilterDefaultV1 : MarshalByRefObject, IInputFilter
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
                return "Default";
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
                return "Default Input Filter";
            }
        }

        /// <summary>
        /// Map for Kana to Stroke
        /// </summary>
        internal List<Dictionary<string, List<KeyStroke>>> _strokemap;

        /// <summary>
        /// Convert code to string map
        /// </summary>
        internal List<Dictionary<int, string>> _codeToString;

        /// <summary>
        /// Convert string to code map
        /// </summary>
        internal List<Dictionary<string, int>> _stringToCode;

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
            // Support Official 106 Keyboard
            new InputKeyboard()
            {
                Author = "Denasu System",
                Name = "106 キーボード",
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
                    return Alphabet101;
                }
                else if (keyboard.Name == "106 キーボード")
                {
                    return Alphabet106;
                }
            }

            return -1;
        }

        /// <summary>
        /// Planes
        /// </summary>
        internal const int NoPlane = -1;
        internal const int Alphabet101 = 0;
        internal const int Alphabet106 = 1;
        internal const int MaxPlane = Alphabet106 + 1;

        /// <summary>
        /// Shortcut information
        /// </summary>
        internal class ShortcutInfo
        {
            public InputAutomaton First { set; get; }
            public InputAutomaton Second { set; get; }
            public InputAutomaton Last { set; get; }
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
        public InputFilterDefaultV1()
        {
            // Init KeyMap
            KeyMaps = new List<List<KeyMap>>();

            for (var i = 0; i < MaxPlane; i++)
            {
                KeyMaps.Add(new List<KeyMap>());
            }
        }

        /// <summary>
        /// InputMethod Author
        /// </summary>
        private string _imAuthor;

        /// <summary>
        /// InputMethod Name
        /// </summary>
        private string _imName;

        /// <summary>
        /// InputMethod Version
        /// </summary>
        private int _imVersion;

        /// <summary>
        /// Set InputMethod Info
        /// </summary>
        /// <param name="author">Author</param>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        public void InputMethodInfo(string author, string name, int version)
        {
            _imAuthor = author;
            _imName = name;
            _imVersion = version;
        }

        /// <summary>
        /// Initialize Key Map
        /// </summary>
        public virtual void InitKeyMap()
        {
            // Make Kana map
            _strokemap = new List<Dictionary<string, List<KeyStroke>>>();

            /// Convert code to string map
            _codeToString = new List<Dictionary<int, string>>();

            /// Convert string to code map
            _stringToCode = new List<Dictionary<string, int>>();

            for (var i = 0; i < MaxPlane; i++)
            {
                _strokemap.Add(new Dictionary<string, List<KeyStroke>>());
                _codeToString.Add(new Dictionary<int, string>());
                _stringToCode.Add(new Dictionary<string, int>());
            }

            // Number
            AddStroke(true, "1", new int[] { '1' }, "1");
            AddStroke(true, "2", new int[] { '2' }, "2");
            AddStroke(true, "3", new int[] { '3' }, "3");
            AddStroke(true, "4", new int[] { '4' }, "4");
            AddStroke(true, "5", new int[] { '5' }, "5");
            AddStroke(true, "6", new int[] { '6' }, "6");
            AddStroke(true, "7", new int[] { '7' }, "7");
            AddStroke(true, "8", new int[] { '8' }, "8");
            AddStroke(true, "9", new int[] { '9' }, "9");
            AddStroke(true, "0", new int[] { '0' }, "0");

            // Alphabet
            AddStroke(true, "a", new int[] { 'a' }, "a");
            AddStroke(true, "b", new int[] { 'b' }, "b");
            AddStroke(true, "c", new int[] { 'c' }, "c");
            AddStroke(true, "d", new int[] { 'd' }, "d");
            AddStroke(true, "e", new int[] { 'e' }, "e");
            AddStroke(true, "f", new int[] { 'f' }, "f");
            AddStroke(true, "g", new int[] { 'g' }, "g");
            AddStroke(true, "h", new int[] { 'h' }, "h");
            AddStroke(true, "i", new int[] { 'i' }, "i");
            AddStroke(true, "j", new int[] { 'j' }, "j");
            AddStroke(true, "k", new int[] { 'k' }, "k");
            AddStroke(true, "l", new int[] { 'l' }, "l");
            AddStroke(true, "m", new int[] { 'm' }, "m");
            AddStroke(true, "n", new int[] { 'n' }, "n");
            AddStroke(true, "o", new int[] { 'o' }, "o");
            AddStroke(true, "p", new int[] { 'p' }, "p");
            AddStroke(true, "q", new int[] { 'q' }, "q");
            AddStroke(true, "r", new int[] { 'r' }, "r");
            AddStroke(true, "s", new int[] { 's' }, "s");
            AddStroke(true, "t", new int[] { 't' }, "t");
            AddStroke(true, "u", new int[] { 'u' }, "u");
            AddStroke(true, "v", new int[] { 'v' }, "v");
            AddStroke(true, "w", new int[] { 'w' }, "w");
            AddStroke(true, "x", new int[] { 'x' }, "x");
            AddStroke(true, "y", new int[] { 'y' }, "y");
            AddStroke(true, "z", new int[] { 'z' }, "z");
            AddStroke(true, "A", new int[] { 'A' }, "A");
            AddStroke(true, "B", new int[] { 'B' }, "B");
            AddStroke(true, "C", new int[] { 'C' }, "C");
            AddStroke(true, "D", new int[] { 'D' }, "D");
            AddStroke(true, "E", new int[] { 'E' }, "E");
            AddStroke(true, "F", new int[] { 'F' }, "F");
            AddStroke(true, "G", new int[] { 'G' }, "G");
            AddStroke(true, "H", new int[] { 'H' }, "H");
            AddStroke(true, "I", new int[] { 'I' }, "I");
            AddStroke(true, "J", new int[] { 'J' }, "J");
            AddStroke(true, "K", new int[] { 'K' }, "K");
            AddStroke(true, "L", new int[] { 'L' }, "L");
            AddStroke(true, "M", new int[] { 'M' }, "M");
            AddStroke(true, "N", new int[] { 'N' }, "N");
            AddStroke(true, "O", new int[] { 'O' }, "O");
            AddStroke(true, "P", new int[] { 'P' }, "P");
            AddStroke(true, "Q", new int[] { 'Q' }, "Q");
            AddStroke(true, "R", new int[] { 'R' }, "R");
            AddStroke(true, "S", new int[] { 'S' }, "S");
            AddStroke(true, "T", new int[] { 'T' }, "T");
            AddStroke(true, "U", new int[] { 'U' }, "U");
            AddStroke(true, "V", new int[] { 'V' }, "V");
            AddStroke(true, "W", new int[] { 'W' }, "W");
            AddStroke(true, "X", new int[] { 'X' }, "X");
            AddStroke(true, "Y", new int[] { 'Y' }, "Y");
            AddStroke(true, "Z", new int[] { 'Z' }, "Z");

            // Sign
            AddStroke(true, "!", new int[] { '!' }, "!");
            AddStroke(true, "\"", new int[] { '\"' }, "\"");
            AddStroke(true, "#", new int[] { '#' }, "#");
            AddStroke(true, "$", new int[] { '$' }, "$");
            AddStroke(true, "%", new int[] { '%' }, "%");
            AddStroke(true, "&", new int[] { '&' }, "&");
            AddStroke(true, "\'", new int[] { '\'' }, "\'");
            AddStroke(true, "(", new int[] { '(' }, "(");
            AddStroke(true, ")", new int[] { ')' }, ")");
            AddStroke(true, "*", new int[] { '*' }, "*");
            AddStroke(true, "+", new int[] { '+' }, "+");
            AddStroke(true, ",", new int[] { ',' }, ",");
            AddStroke(true, "-", new int[] { '-' }, "-");
            AddStroke(true, ".", new int[] { '.' }, ".");
            AddStroke(true, "/", new int[] { '/' }, "/");
            AddStroke(true, ":", new int[] { ':' }, ":");
            AddStroke(true, ";", new int[] { ';' }, ";");
            AddStroke(true, "<", new int[] { '<' }, "<");
            AddStroke(true, "=", new int[] { '=' }, "=");
            AddStroke(true, ">", new int[] { '>' }, ">");
            AddStroke(true, "?", new int[] { '?' }, "?");
            AddStroke(true, "@", new int[] { '@' }, "@");
            AddStroke(true, "[", new int[] { '[' }, "[");
            AddStroke(true, "\\", new int[] { '￥' }, "\\");
            AddStroke(true, "]", new int[] { ']' }, "]");
            AddStroke(true, "^", new int[] { '^' }, "^");
            AddStroke(true, "_", new int[] { '_' }, "_");
            AddStroke(true, "`", new int[] { '`' }, "`");
            AddStroke(true, "{", new int[] { '{' }, "{");
            AddStroke(true, "|", new int[] { '|' }, "|");
            AddStroke(true, "}", new int[] { '}' }, "}");
            AddStroke(true, "~", new int[] { '~' }, "~");
            AddStroke(true, " ", new int[] { ' ' }, " ");
        }

        /// <summary>
        /// Helper method to add stroke
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="showKeyArray">Show key in Key Array</param>
        /// <param name="word">Word character</param>
        /// <param name="stroke">Stroke</param>
        /// <param name="display">Input character</param>
        /// <param name="flags">Input customization flags</param>
        internal void AddStroke(bool showKeyArray, string word, int[] stroke, string input)
        {
            // Convert stroke from virtual to physical
            var convertedStrokes101 = ConvertStrokes(Alphabet101, stroke);

            // Add each stroke
            foreach (var convertedStroke in convertedStrokes101)
            {
                AddStrokeInternal(Alphabet101, showKeyArray, word, convertedStroke, input);
            }

            var convertedStrokes106 = ConvertStrokes(Alphabet106, stroke);

            // Add each stroke
            foreach (var convertedStroke in convertedStrokes106)
            {
                AddStrokeInternal(Alphabet106, showKeyArray, word, convertedStroke, input);
            }
        }

        /// <summary>
        /// Convert stroke from virtual to physical and geenrate combination
        /// e.g.
        /// KeyMap has virtual key and physical key mapping.
        ///     [v1 -> p1, p2] [v2 -> p3] [v3 -> p4, p5]
        /// Then create combination of physical keys
        ///     (v1, v2) => (p1, p3, p4) (p1, p3, p5) (p2, p3, p4) (p2, p3, p5)
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="stroke">Stroke in virtual key</param>
        /// <returns>Converted virtual key and physical key</returns>
        internal List<Tuple<int, int>[]> ConvertStrokes(int plane, int[] stroke)
        {
            var result = new List<Tuple<int, int>[]>();

            // Generate combination
            var variation = new List<List<int>>();
            var variationCount = new List<int>();
            foreach (var virtualKey in stroke)
            {
                var physicalKeys = KeyMaps[plane].Where(x => x.VirtualKey == virtualKey).Select(x => x.PhysicalKey);
                variationCount.Add(physicalKeys.Count());
                variation.Add(physicalKeys.ToList());
            }

            // How many combination are there?
            var c = variationCount.Aggregate(1, (x, y) => x * y);
            if (c == 0)
            {
                // Even if mappedStroke is empty, add empty stroke
                // so that InputMethod Editor can add new stroke
                var virtualPhysical = new Tuple<int, int>[variationCount.Count()];
                for (var strokeNo = variationCount.Count() - 1; strokeNo >= 0; strokeNo--)
                {
                    virtualPhysical[strokeNo] = new Tuple<int, int>(
                        // Virtual Key
                        stroke[strokeNo],
                        // Physical Key
                        -1);
                }
                result.Add(virtualPhysical);
            }
            else
            {
                // Create virtual and physical keys
                for (var index = 0; index < c; index++)
                {
                    var tmp = index;
                    var virtualPhysical = new Tuple<int, int>[variationCount.Count()];
                    for (var strokeNo = variationCount.Count() - 1; strokeNo >= 0; strokeNo--)
                    {
                        virtualPhysical[strokeNo] = new Tuple<int, int>(
                            // Virtual Key
                            stroke[strokeNo],
                            // Physical Key
                            variation[strokeNo][tmp % variationCount[strokeNo]]);

                        tmp /= variationCount[strokeNo];
                    }
                    result.Add(virtualPhysical);
                }
            }

            return result;
        }

        /// <summary>
        /// Helper method to add stroke
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="showKeyArray">Show key in Key Array</param>
        /// <param name="word">Word character</param>
        /// <param name="stroke">Stroke</param>
        /// <param name="display">Input character</param>
        /// <param name="flags">Input customization flags</param>
        internal void AddStrokeInternal(int plane, bool showKeyArray, string word, Tuple<int, int>[] stroke, string input)
        {
            // Add to map
            if (showKeyArray)
            {
                _codeToString[plane][stroke[0].Item1] = word;
                _stringToCode[plane][word] = stroke[0].Item1;
            }

            // Add to kana map
            if (!_strokemap[plane].ContainsKey(word))
            {
                _strokemap[plane].Add(word, new List<KeyStroke>());
            }
            else if (_strokemap[plane][word].Where(x => x.Stroke.SequenceEqual(stroke.Select(y => y.Item2))).Count() > 0)
            {
                // The same word and stroke. Ignore
                return;
            }

            _strokemap[plane][word].Add(new KeyStroke()
            {
                Stroke = stroke.Where(y => y.Item2 != -1).Select(y => y.Item2).ToArray(),
                Input = input,
                Custom = new helper.BitFlag(0),
            });
        }

        /// <summary>
        /// Build automaton from word string
        /// </summary>
        /// <param name="word">Word string</param>
        /// <param name="keyboard">Keyboard</param>
        /// <returns>Automaton</returns>
        public virtual InputAutomaton ToAutomaton(string s, InputKeyboard keyboard)
        {
            var plane = GetPlane(keyboard, "Default");

            // Start state
            InputAutomaton start = new InputAutomaton();
            InputAutomaton current = start;
            string c;

            // Make automaton
            for (int i = 0; i < s.Length; i++)
            {
                InputAutomaton next;

                // Get next input character
                c = s.Substring(i, 1);

                next = new InputAutomaton();
                current = ProcessNormal(plane, s, current, ref c, ref i, next);

                current = next;
            }

            return start;
        }

        /// <summary>
        /// Process normal character
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="s">String</param>
        /// <param name="current">Current automaton</param>
        /// <param name="c">Char</param>
        /// <param name="index">Index</param>
        /// <param name="next">Next automaton</param>
        /// <returns></returns>
        internal InputAutomaton ProcessNormal(int plane, string s, InputAutomaton current, ref string c, ref int index, InputAutomaton next)
        {
            BitFlag custom = InputCustomDefaultV1.Ope_NO_CUSTOM;

            // Add automaton
            CreateAutomaton(plane, current, c, next, custom);

            return current;
        }

        /// <summary>
        /// Create new automaton
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="current">Current Automaton</param>
        /// <param name="c">char</param>
        /// <param name="next">Next Automaton</param>
        /// <param name="customAdd">Custom to add</param>
        internal void CreateAutomaton(int plane, InputAutomaton current, string c, InputAutomaton next, BitFlag customAdd)
        {
            // Previous automaton
            InputAutomaton prev = current;

            if (!_strokemap[plane].ContainsKey(c))
            {
                // Input is not supported
                return;
            }

            // For all the stroke for c (e.g. "か"->ka, ca)
            foreach (var stroke in _strokemap[plane][c])
            {
                if (stroke.Stroke.Count() == 0)
                {
                    continue;
                }

                current = prev;

                // Create new automaton for each stroke
                for (var alphabet = 0; alphabet < stroke.Stroke.Count(); alphabet++)
                {
                    if (stroke.Stroke[alphabet] == (int)WTKeyCode.NoKey)
                    {
                        // Unknown
                        continue;
                    }

                    if (alphabet + 1 == stroke.Stroke.Count())
                    {
                        // The last alphabet
                        current.SetConnect(stroke.Stroke[alphabet], new InputAutomaton.Connect()
                        {
                            Automaton = next,
                            Flags = stroke.Custom.Or(customAdd),
                            Character = stroke.Input.Substring(alphabet, 1),
                        });
                        current = next;
                    }
                    else
                    {
                        // Middle alphabet

                        InputAutomaton.Connect connect = current.GetConnect(stroke.Stroke[alphabet]);
                        if (connect == null)
                        {
                            // This automaton doesn't have the connection for this alphabet

                            // Create a new connection for this alphabet
                            connect = new InputAutomaton.Connect()
                            {
                                Automaton = new InputAutomaton(),
                                Flags = stroke.Custom.Or(customAdd),
                                Character = stroke.Input.Substring(alphabet, 1),
                            };

                            // Connect
                            current.SetConnect(stroke.Stroke[alphabet], connect);
                            current = connect.Automaton;
                        }
                        else
                        {
                            // This automaton already have a connection for this alphabet

                            // To make it deterministic, use existing automaton
                            connect.Flags = connect.Flags.Or(stroke.Custom);

                            current = connect.Automaton;
                        }
                    }
                }
            }
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

            // Get key map by physical code
            var mapKey = KeyMaps[plane].Where(x => x.PhysicalKey == physicalKey);
            foreach (var key in mapKey)
            {
                // Convert to virtual code
                var virtualKey = key.VirtualKey;

                // Get string
                if (!_codeToString[plane].ContainsKey(virtualKey))
                {
                    continue;
                }

                result.Add(_codeToString[plane][virtualKey]);
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
            if (!_stringToCode[plane].ContainsKey(label))
            {
                return -1;
            }

            return _stringToCode[plane][label];
        }
    }
}
