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
    /// Input Filter for Roman
    /// </summary>
    public class InputFilterRomanV2 : MarshalByRefObject, IInputFilter
    {
        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// The name of this plugin
        /// </summary>
        public virtual string Name
        {
            get
            {
                return "Roman";
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
                return "Roman Input Filter";
            }
        }

        /// <summary>
        /// Map for Kana to Stroke
        /// </summary>
        internal List<Dictionary<string, List<KeyStroke>>> _strokemap;

        /// <summary>
        /// Which Kana makes following "ん" to nn?
        /// </summary>
        internal HashSet<string> _nnmap;

        /// <summary>
        /// Which Kana makes following "っ" to xtu/ltu?
        /// </summary>
        internal HashSet<string> _sokuonmap;

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
            public bool Hatuon { set; get; }
        }

        /// <summary>
        /// Create InputCustom instance
        /// InputCustom hold how to input a stroke which has multiple input path.
        /// </summary>
        /// <returns>Input Custom instance</returns>
        public IInputCustom CreateInputCustom()
        {
            return new InputCustomRomanV1();
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="keymap">Key map decrelared in XML</param>
        public InputFilterRomanV2()
        {
            // A map for the characters which needs NN for previous "ん"
            _nnmap = new HashSet<string>()
            {
                "ア", "イ", "ウ", "エ", "オ", "ヤ", "ユ", "ヨ", "ナ", "ニ",
                "ヌ", "ネ", "ノ", "ン", "a",  "i",  "u",  "e",  "o", "n",
                "y",
            };

            // A map for the characters which can't use e.g. tta for "った"
            _sokuonmap = new HashSet<string>()
            {
                "ア", "イ", "ウ", "エ", "オ", "ナ", "ニ", "ヌ", "ネ", "ノ",
                "ン", "a",  "i",  "u",  "e", "o",  "n",  "1",  "2",  "3",
                "4",  "5",  "6",  "7",  "8", "9",  "0",  "！", "!",  "”",
                "\"", "＃", "#",  "＄", "$",  "％", "%",  "＆", "&",  "’",
                "\'", "（", "(",  "）", ")",  "～", "~",  "＝", "=",  "－",
                "-",  "～", "~",  "＾", "^",  "｜", "|",  "￥", "\\", "｀",
                "`",  "＠", "@",  "「", "［", "[",  "｛", "{",  "」", "］",
                "]",  "｝", "}",  "；", ";",  "＋", "+",  "：", ":",  "＊",
                "*",  "，", "、", ",",  "＜", "<",  "＞", ">",  "。", "．",
                ".",  "・", "…", "／", "/",   "？", "?",  "＿", "_",
            };

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

            // Japanese Character
            AddStroke(false, "ア", new int[] { 'a' }, "a", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "イ", new int[] { 'i' }, "i", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ウ", new int[] { 'u' }, "u", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "エ", new int[] { 'e' }, "e", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "オ", new int[] { 'o' }, "o", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "カ", new int[] { 'k', 'a' }, "ka", InputCustomRomanV1.Ope_NO_C);
            AddStroke(false, "カ", new int[] { 'c', 'a' }, "ca", InputCustomRomanV1.Ope_C);
            AddStroke(false, "キ", new int[] { 'k', 'i' }, "ki", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ク", new int[] { 'k', 'u' }, "ku", InputCustomRomanV1.Ope_NO_C);
            AddStroke(false, "ク", new int[] { 'c', 'u' }, "cu", InputCustomRomanV1.Ope_C);
            AddStroke(false, "ケ", new int[] { 'k', 'e' }, "ke", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "コ", new int[] { 'k', 'o' }, "ko", InputCustomRomanV1.Ope_NO_C);
            AddStroke(false, "コ", new int[] { 'c', 'o' }, "co", InputCustomRomanV1.Ope_C);
            AddStroke(false, "サ", new int[] { 's', 'a' }, "sa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "シ", new int[] { 's', 'i' }, "si", InputCustomRomanV1.Ope_NO_SH.Or(InputCustomRomanV1.Ope_NO_C));
            AddStroke(false, "シ", new int[] { 's', 'h', 'i' }, "shi", InputCustomRomanV1.Ope_SH.Or(InputCustomRomanV1.Ope_NO_C));
            AddStroke(false, "シ", new int[] { 'c', 'i' }, "ci", InputCustomRomanV1.Ope_C);
            AddStroke(false, "ス", new int[] { 's', 'u' }, "su", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "セ", new int[] { 's', 'e' }, "se", InputCustomRomanV1.Ope_NO_C);
            AddStroke(false, "セ", new int[] { 'c', 'e' }, "ce", InputCustomRomanV1.Ope_C);
            AddStroke(false, "ソ", new int[] { 's', 'o' }, "so", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "タ", new int[] { 't', 'a' }, "ta", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "チ", new int[] { 't', 'i' }, "ti", InputCustomRomanV1.Ope_NO_CH);
            AddStroke(false, "チ", new int[] { 'c', 'h', 'i' }, "chi", InputCustomRomanV1.Ope_CH);
            AddStroke(false, "ツ", new int[] { 't', 'u' }, "tu", InputCustomRomanV1.Ope_NO_TS);
            AddStroke(false, "ツ", new int[] { 't', 's', 'u' }, "tsu", InputCustomRomanV1.Ope_TS);
            AddStroke(false, "テ", new int[] { 't', 'e' }, "te", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ト", new int[] { 't', 'o' }, "to", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ナ", new int[] { 'n', 'a' }, "na", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニ", new int[] { 'n', 'i' }, "ni", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヌ", new int[] { 'n', 'u' }, "nu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ネ", new int[] { 'n', 'e' }, "ne", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ノ", new int[] { 'n', 'o' }, "no", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ハ", new int[] { 'h', 'a' }, "ha", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒ", new int[] { 'h', 'i' }, "hi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "フ", new int[] { 'h', 'u' }, "hu", InputCustomRomanV1.Ope_HU);
            AddStroke(false, "フ", new int[] { 'f', 'u' }, "fu", InputCustomRomanV1.Ope_FU);
            AddStroke(false, "ヘ", new int[] { 'h', 'e' }, "he", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ホ", new int[] { 'h', 'o' }, "ho", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "マ", new int[] { 'm', 'a' }, "ma", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミ", new int[] { 'm', 'i' }, "mi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ム", new int[] { 'm', 'u' }, "mu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "メ", new int[] { 'm', 'e' }, "me", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "モ", new int[] { 'm', 'o' }, "mo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヤ", new int[] { 'y', 'a' }, "ya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ユ", new int[] { 'y', 'u' }, "yu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヨ", new int[] { 'y', 'o' }, "yo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ラ", new int[] { 'r', 'a' }, "ra", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リ", new int[] { 'r', 'i' }, "ri", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ル", new int[] { 'r', 'u' }, "ru", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "レ", new int[] { 'r', 'e' }, "re", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ロ", new int[] { 'r', 'o' }, "ro", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ワ", new int[] { 'w', 'a' }, "wa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヲ", new int[] { 'w', 'o' }, "wo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ン", new int[] { 'n' }, "n", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ガ", new int[] { 'g', 'a' }, "ga", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギ", new int[] { 'g', 'i' }, "gi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "グ", new int[] { 'g', 'u' }, "gu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ゲ", new int[] { 'g', 'e' }, "ge", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ゴ", new int[] { 'g', 'o' }, "go", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ザ", new int[] { 'z', 'a' }, "za", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ジ", new int[] { 'j', 'i' }, "ji", InputCustomRomanV1.Ope_JI);
            AddStroke(false, "ジ", new int[] { 'z', 'i' }, "zi", InputCustomRomanV1.Ope_ZI);
            AddStroke(false, "ズ", new int[] { 'z', 'u' }, "zu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ゼ", new int[] { 'z', 'e' }, "ze", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ゾ", new int[] { 'z', 'o' }, "zo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ダ", new int[] { 'd', 'a' }, "da", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヂ", new int[] { 'd', 'i' }, "di", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヅ", new int[] { 'd', 'u' }, "du", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "デ", new int[] { 'd', 'e' }, "de", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ド", new int[] { 'd', 'o' }, "do", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "バ", new int[] { 'b', 'a' }, "ba", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビ", new int[] { 'b', 'i' }, "bi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ブ", new int[] { 'b', 'u' }, "bu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ベ", new int[] { 'b', 'e' }, "be", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ボ", new int[] { 'b', 'o' }, "bo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "パ", new int[] { 'p', 'a' }, "pa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ピ", new int[] { 'p', 'i' }, "pi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "プ", new int[] { 'p', 'u' }, "pu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ペ", new int[] { 'p', 'e' }, "pe", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ポ", new int[] { 'p', 'o' }, "po", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ァ", new int[] { 'x', 'a' }, "xa", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ァ", new int[] { 'l', 'a' }, "la", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ィ", new int[] { 'x', 'i' }, "xi", InputCustomRomanV1.Ope_X.Or(InputCustomRomanV1.Ope_NO_YIE));
            AddStroke(false, "ィ", new int[] { 'l', 'i' }, "li", InputCustomRomanV1.Ope_L.Or(InputCustomRomanV1.Ope_NO_YIE));
            AddStroke(false, "ィ", new int[] { 'x', 'y', 'i' }, "xyi", InputCustomRomanV1.Ope_X.Or(InputCustomRomanV1.Ope_YIE));
            AddStroke(false, "ィ", new int[] { 'l', 'y', 'i' }, "lyi", InputCustomRomanV1.Ope_L.Or(InputCustomRomanV1.Ope_YIE));
            AddStroke(false, "ゥ", new int[] { 'x', 'u' }, "xu", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ゥ", new int[] { 'l', 'u' }, "lu", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ェ", new int[] { 'x', 'e' }, "xe", InputCustomRomanV1.Ope_X.Or(InputCustomRomanV1.Ope_NO_YIE));
            AddStroke(false, "ェ", new int[] { 'l', 'e' }, "le", InputCustomRomanV1.Ope_L.Or(InputCustomRomanV1.Ope_NO_YIE));
            AddStroke(false, "ェ", new int[] { 'x', 'y', 'e' }, "xye", InputCustomRomanV1.Ope_X.Or(InputCustomRomanV1.Ope_YIE));
            AddStroke(false, "ェ", new int[] { 'l', 'y', 'e' }, "lye", InputCustomRomanV1.Ope_L.Or(InputCustomRomanV1.Ope_YIE));
            AddStroke(false, "ォ", new int[] { 'x', 'o' }, "xo", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ォ", new int[] { 'l', 'o' }, "lo", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ヵ", new int[] { 'x', 'k', 'a' }, "xka", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ヵ", new int[] { 'l', 'x', 'a' }, "lxa", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ヶ", new int[] { 'x', 'k', 'e' }, "xke", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ヶ", new int[] { 'l', 'k', 'e' }, "lke", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ッ", new int[] { 'x', 't', 'u' }, "xtu", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ッ", new int[] { 'l', 't', 'u' }, "ltu", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ャ", new int[] { 'x', 'y', 'a' }, "xya", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ャ", new int[] { 'l', 'y', 'a' }, "lya", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ュ", new int[] { 'x', 'y', 'u' }, "xyu", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ュ", new int[] { 'l', 'y', 'u' }, "lyu", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ョ", new int[] { 'x', 'y', 'o' }, "xyo", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ョ", new int[] { 'l', 'y', 'o' }, "lyo", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ヮ", new int[] { 'x', 'w', 'a' }, "xwa", InputCustomRomanV1.Ope_X);
            AddStroke(false, "ヮ", new int[] { 'l', 'w', 'a' }, "lwa", InputCustomRomanV1.Ope_L);
            AddStroke(false, "ヰ", new int[] { 'w', 'y', 'i' }, "wyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヱ", new int[] { 'w', 'y', 'e' }, "wye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴ", new int[] { 'v', 'u' }, "vu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "イェ", new int[] { 'y', 'e' }, "ye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ウァ", new int[] { 'w', 'h', 'a' }, "wha", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ウィ", new int[] { 'w', 'i' }, "wi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ウェ", new int[] { 'w', 'e' }, "we", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ウォ", new int[] { 'w', 'h', 'o' }, "who", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴァ", new int[] { 'v', 'a' }, "va", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴャ", new int[] { 'v', 'y', 'a' }, "vya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴィ", new int[] { 'v', 'i' }, "vi", InputCustomRomanV1.Ope_NO_VY);
            AddStroke(false, "ヴィ", new int[] { 'v', 'y', 'i' }, "vyi", InputCustomRomanV1.Ope_VY);
            AddStroke(false, "ヴュ", new int[] { 'v', 'y', 'u' }, "vyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴェ", new int[] { 'v', 'e' }, "ve", InputCustomRomanV1.Ope_NO_VY);
            AddStroke(false, "ヴェ", new int[] { 'v', 'y', 'e' }, "vye", InputCustomRomanV1.Ope_VY);
            AddStroke(false, "ヴォ", new int[] { 'v', 'o' }, "vo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヴョ", new int[] { 'v', 'y', 'o' }, "vyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "キャ", new int[] { 'k', 'y', 'a' }, "kya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "キィ", new int[] { 'k', 'y', 'i' }, "kyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "キュ", new int[] { 'k', 'y', 'u' }, "kyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "キェ", new int[] { 'k', 'y', 'e' }, "kye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "キョ", new int[] { 'k', 'y', 'o' }, "kyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギャ", new int[] { 'g', 'y', 'a' }, "gya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギィ", new int[] { 'g', 'y', 'i' }, "gyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギュ", new int[] { 'g', 'y', 'u' }, "gyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギェ", new int[] { 'g', 'y', 'e' }, "gye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ギョ", new int[] { 'g', 'y', 'o' }, "gyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "クァ", new int[] { 'q', 'a' }, "qa", InputCustomRomanV1.Ope_NO_QW.Or(InputCustomRomanV1.Ope_NO_KW));
            AddStroke(false, "クァ", new int[] { 'q', 'w', 'a' }, "qwa", InputCustomRomanV1.Ope_QW);
            AddStroke(false, "クァ", new int[] { 'k', 'w', 'a' }, "kwa", InputCustomRomanV1.Ope_KW);
            AddStroke(false, "クィ", new int[] { 'q', 'y', 'i' }, "qyi", InputCustomRomanV1.Ope_NO_QW);
            AddStroke(false, "クィ", new int[] { 'q', 'w', 'i' }, "qwi", InputCustomRomanV1.Ope_QW);
            AddStroke(false, "クェ", new int[] { 'q', 'e' }, "qe", InputCustomRomanV1.Ope_NO_QW);
            AddStroke(false, "クェ", new int[] { 'q', 'w', 'e' }, "qwe", InputCustomRomanV1.Ope_QW);
            AddStroke(false, "クォ", new int[] { 'q', 'o' }, "qo", InputCustomRomanV1.Ope_NO_QW);
            AddStroke(false, "クォ", new int[] { 'q', 'w', 'o' }, "qwo", InputCustomRomanV1.Ope_QW);
            AddStroke(false, "グァ", new int[] { 'g', 'w', 'a' }, "gwa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "グィ", new int[] { 'g', 'w', 'i' }, "gwi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "グゥ", new int[] { 'g', 'w', 'u' }, "gwu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "グェ", new int[] { 'g', 'w', 'e' }, "gwe", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "グォ", new int[] { 'g', 'w', 'o' }, "gwo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "シャ", new int[] { 's', 'y', 'a' }, "sya", InputCustomRomanV1.Ope_NO_SH);
            AddStroke(false, "シャ", new int[] { 's', 'h', 'a' }, "sha", InputCustomRomanV1.Ope_SH);
            AddStroke(false, "シィ", new int[] { 's', 'y', 'i' }, "syi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "シュ", new int[] { 's', 'y', 'u' }, "syu", InputCustomRomanV1.Ope_NO_SH);
            AddStroke(false, "シュ", new int[] { 's', 'h', 'u' }, "shu", InputCustomRomanV1.Ope_SH);
            AddStroke(false, "シェ", new int[] { 's', 'y', 'e' }, "sye", InputCustomRomanV1.Ope_NO_SH);
            AddStroke(false, "シェ", new int[] { 's', 'h', 'e' }, "she", InputCustomRomanV1.Ope_SH);
            AddStroke(false, "ショ", new int[] { 's', 'y', 'o' }, "syo", InputCustomRomanV1.Ope_NO_SH);
            AddStroke(false, "ショ", new int[] { 's', 'h', 'o' }, "sho", InputCustomRomanV1.Ope_SH);
            AddStroke(false, "ジャ", new int[] { 'j', 'a' }, "ja", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_NO_JY));
            AddStroke(false, "ジャ", new int[] { 'j', 'y', 'a' }, "jya", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_JY));
            AddStroke(false, "ジャ", new int[] { 'z', 'y', 'a' }, "zya", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "ジィ", new int[] { 'j', 'y', 'i' }, "jyi", InputCustomRomanV1.Ope_NO_ZY);
            AddStroke(false, "ジィ", new int[] { 'z', 'y', 'i' }, "zyi", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "ジュ", new int[] { 'j', 'u' }, "ju", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_NO_JY));
            AddStroke(false, "ジュ", new int[] { 'j', 'y', 'u' }, "jyu", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_JY));
            AddStroke(false, "ジュ", new int[] { 'z', 'y', 'u' }, "zyu", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "ジェ", new int[] { 'j', 'e' }, "je", InputCustomRomanV1.Ope_NO_ZY);
            AddStroke(false, "ジェ", new int[] { 'z', 'y', 'e' }, "zye", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "ジョ", new int[] { 'j', 'o' }, "jo", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_NO_JY));
            AddStroke(false, "ジョ", new int[] { 'j', 'y', 'o' }, "jyo", InputCustomRomanV1.Ope_NO_ZY.Or(InputCustomRomanV1.Ope_JY));
            AddStroke(false, "ジョ", new int[] { 'z', 'y', 'o' }, "zyo", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "スァ", new int[] { 's', 'w', 'a' }, "swa", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "スィ", new int[] { 's', 'w', 'i' }, "swi", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "スゥ", new int[] { 's', 'w', 'u' }, "swu", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "スェ", new int[] { 's', 'w', 'e' }, "swe", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "スォ", new int[] { 's', 'w', 'o' }, "swo", InputCustomRomanV1.Ope_ZY);
            AddStroke(false, "チャ", new int[] { 't', 'y', 'a' }, "tya", InputCustomRomanV1.Ope_TY);
            AddStroke(false, "チャ", new int[] { 'c', 'h', 'a' }, "cha", InputCustomRomanV1.Ope_CH.Or(InputCustomRomanV1.Ope_NO_CY).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チャ", new int[] { 'c', 'y', 'a' }, "cya", InputCustomRomanV1.Ope_CY.Or(InputCustomRomanV1.Ope_NO_CH).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チィ", new int[] { 't', 'y', 'i' }, "tyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "チュ", new int[] { 't', 'y', 'u' }, "tyu", InputCustomRomanV1.Ope_TY);
            AddStroke(false, "チュ", new int[] { 'c', 'h', 'u' }, "chu", InputCustomRomanV1.Ope_CH.Or(InputCustomRomanV1.Ope_NO_CY).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チュ", new int[] { 'c', 'y', 'u' }, "cyu", InputCustomRomanV1.Ope_CY.Or(InputCustomRomanV1.Ope_NO_CH).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チェ", new int[] { 't', 'y', 'e' }, "tye", InputCustomRomanV1.Ope_TY);
            AddStroke(false, "チェ", new int[] { 'c', 'h', 'e' }, "che", InputCustomRomanV1.Ope_CH.Or(InputCustomRomanV1.Ope_NO_CY).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チェ", new int[] { 'c', 'y', 'e' }, "cye", InputCustomRomanV1.Ope_CY.Or(InputCustomRomanV1.Ope_NO_CH).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チョ", new int[] { 't', 'y', 'o' }, "tyo", InputCustomRomanV1.Ope_TY);
            AddStroke(false, "チョ", new int[] { 'c', 'h', 'o' }, "cho", InputCustomRomanV1.Ope_CH.Or(InputCustomRomanV1.Ope_NO_CY).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "チョ", new int[] { 'c', 'y', 'o' }, "cyo", InputCustomRomanV1.Ope_CY.Or(InputCustomRomanV1.Ope_NO_CH).Or(InputCustomRomanV1.Ope_NO_TY));
            AddStroke(false, "ヂャ", new int[] { 'd', 'y', 'a' }, "dya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヂィ", new int[] { 'd', 'y', 'i' }, "dyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヂュ", new int[] { 'd', 'y', 'u' }, "dyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヂェ", new int[] { 'd', 'y', 'e' }, "dye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヂョ", new int[] { 'd', 'y', 'o' }, "dyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ツァ", new int[] { 't', 's', 'a' }, "tsa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ツィ", new int[] { 't', 's', 'i' }, "tsi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ツェ", new int[] { 't', 's', 'e' }, "tse", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ツォ", new int[] { 't', 's', 'o' }, "tso", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "テャ", new int[] { 't', 'h', 'a' }, "tha", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ティ", new int[] { 't', 'h', 'i' }, "thi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "テュ", new int[] { 't', 'h', 'u' }, "thu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "テェ", new int[] { 't', 'h', 'e' }, "the", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "テョ", new int[] { 't', 'h', 'o' }, "tho", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "デャ", new int[] { 'd', 'h', 'a' }, "dha", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ディ", new int[] { 'd', 'h', 'i' }, "dhi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "デュ", new int[] { 'd', 'h', 'u' }, "dhu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "デェ", new int[] { 'd', 'h', 'e' }, "dhe", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "デョ", new int[] { 'd', 'h', 'o' }, "dho", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "トァ", new int[] { 't', 'w', 'a' }, "twa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "トィ", new int[] { 't', 'w', 'i' }, "twi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "トゥ", new int[] { 't', 'w', 'u' }, "twu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "トェ", new int[] { 't', 'w', 'e' }, "twe", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "トォ", new int[] { 't', 'w', 'o' }, "two", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ドァ", new int[] { 'd', 'w', 'a' }, "dwa", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ドィ", new int[] { 'd', 'w', 'i' }, "dwi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ドゥ", new int[] { 'd', 'w', 'u' }, "dwu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ドェ", new int[] { 'd', 'w', 'e' }, "dwe", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ドォ", new int[] { 'd', 'w', 'o' }, "dwo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニャ", new int[] { 'n', 'y', 'a' }, "nya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニィ", new int[] { 'n', 'y', 'i' }, "nyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニュ", new int[] { 'n', 'y', 'u' }, "nyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニェ", new int[] { 'n', 'y', 'e' }, "nye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ニョ", new int[] { 'n', 'y', 'o' }, "nyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒャ", new int[] { 'h', 'y', 'a' }, "hya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒィ", new int[] { 'h', 'y', 'i' }, "hyi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒュ", new int[] { 'h', 'y', 'u' }, "hyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒェ", new int[] { 'h', 'y', 'e' }, "hye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ヒョ", new int[] { 'h', 'y', 'o' }, "hyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビャ", new int[] { 'b', 'y', 'a' }, "bya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビィ", new int[] { 'b', 'y', 'i' }, "byi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビュ", new int[] { 'b', 'y', 'u' }, "byu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビェ", new int[] { 'b', 'y', 'e' }, "bye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ビョ", new int[] { 'b', 'y', 'o' }, "byo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ピャ", new int[] { 'p', 'y', 'a' }, "pya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ピュ", new int[] { 'p', 'y', 'u' }, "pyu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ピェ", new int[] { 'p', 'y', 'e' }, "pye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ピョ", new int[] { 'p', 'y', 'o' }, "pyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ファ", new int[] { 'f', 'a' }, "fa", InputCustomRomanV1.Ope_NO_FY);
            AddStroke(false, "ファ", new int[] { 'f', 'y', 'a' }, "fya", InputCustomRomanV1.Ope_FY);
            AddStroke(false, "フィ", new int[] { 'f', 'i' }, "fi", InputCustomRomanV1.Ope_NO_FY);
            AddStroke(false, "フィ", new int[] { 'f', 'y', 'i' }, "fyi", InputCustomRomanV1.Ope_FY);
            AddStroke(false, "フュ", new int[] { 'f', 'y', 'u' }, "fyu", InputCustomRomanV1.Ope_FY);
            AddStroke(false, "フェ", new int[] { 'f', 'e' }, "fe", InputCustomRomanV1.Ope_NO_FY);
            AddStroke(false, "フェ", new int[] { 'f', 'y', 'e' }, "fye", InputCustomRomanV1.Ope_FY);
            AddStroke(false, "フォ", new int[] { 'f', 'o' }, "fo", InputCustomRomanV1.Ope_NO_FY);
            AddStroke(false, "フョ", new int[] { 'f', 'y', 'o' }, "fyo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミャ", new int[] { 'm', 'y', 'a' }, "mya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミィ", new int[] { 'm', 'y', 'i' }, "myi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミュ", new int[] { 'm', 'y', 'u' }, "myu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミェ", new int[] { 'm', 'y', 'e' }, "mye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ミョ", new int[] { 'm', 'y', 'o' }, "myo", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リャ", new int[] { 'r', 'y', 'a' }, "rya", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リィ", new int[] { 'r', 'y', 'i' }, "ryi", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リュ", new int[] { 'r', 'y', 'u' }, "ryu", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リェ", new int[] { 'r', 'y', 'e' }, "rye", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "リョ", new int[] { 'r', 'y', 'o' }, "ryo", InputCustomRomanV1.Ope_NO_CUSTOM);

            // Number
            AddStroke(true, "1", new int[] { '1' }, "1", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "2", new int[] { '2' }, "2", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "3", new int[] { '3' }, "3", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "4", new int[] { '4' }, "4", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "5", new int[] { '5' }, "5", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "6", new int[] { '6' }, "6", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "7", new int[] { '7' }, "7", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "8", new int[] { '8' }, "8", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "9", new int[] { '9' }, "9", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "0", new int[] { '0' }, "0", InputCustomRomanV1.Ope_NO_CUSTOM);

            // Alphabet
            AddStroke(true, "a", new int[] { 'a' }, "a", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "b", new int[] { 'b' }, "b", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "c", new int[] { 'c' }, "c", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "d", new int[] { 'd' }, "d", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "e", new int[] { 'e' }, "e", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "f", new int[] { 'f' }, "f", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "g", new int[] { 'g' }, "g", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "h", new int[] { 'h' }, "h", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "i", new int[] { 'i' }, "i", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "j", new int[] { 'j' }, "j", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "k", new int[] { 'k' }, "k", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "l", new int[] { 'l' }, "l", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "m", new int[] { 'm' }, "m", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "n", new int[] { 'n' }, "n", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "o", new int[] { 'o' }, "o", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "p", new int[] { 'p' }, "p", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "q", new int[] { 'q' }, "q", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "r", new int[] { 'r' }, "r", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "s", new int[] { 's' }, "s", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "t", new int[] { 't' }, "t", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "u", new int[] { 'u' }, "u", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "v", new int[] { 'v' }, "v", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "w", new int[] { 'w' }, "w", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "x", new int[] { 'x' }, "x", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "y", new int[] { 'y' }, "y", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "z", new int[] { 'z' }, "z", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "A", new int[] { 'A' }, "A", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "B", new int[] { 'B' }, "B", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "C", new int[] { 'C' }, "C", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "D", new int[] { 'D' }, "D", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "E", new int[] { 'E' }, "E", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "F", new int[] { 'F' }, "F", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "G", new int[] { 'G' }, "G", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "H", new int[] { 'H' }, "H", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "I", new int[] { 'I' }, "I", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "J", new int[] { 'J' }, "J", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "K", new int[] { 'K' }, "K", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "L", new int[] { 'L' }, "L", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "M", new int[] { 'M' }, "M", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "N", new int[] { 'N' }, "N", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "O", new int[] { 'O' }, "O", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "P", new int[] { 'P' }, "P", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "Q", new int[] { 'Q' }, "Q", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "R", new int[] { 'R' }, "R", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "S", new int[] { 'S' }, "S", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "T", new int[] { 'T' }, "T", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "U", new int[] { 'U' }, "U", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "V", new int[] { 'V' }, "V", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "W", new int[] { 'W' }, "W", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "X", new int[] { 'X' }, "X", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "Y", new int[] { 'Y' }, "Y", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "Z", new int[] { 'Z' }, "Z", InputCustomRomanV1.Ope_NO_CUSTOM);

            // Sign
            AddStroke(true, "!", new int[] { '!' }, "!", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "\"", new int[] { '\"' }, "\"", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "#", new int[] { '#' }, "#", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "$", new int[] { '$' }, "$", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "%", new int[] { '%' }, "%", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "&", new int[] { '&' }, "&", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "\'", new int[] { '\'' }, "\'", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "(", new int[] { '(' }, "(", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ")", new int[] { ')' }, ")", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "*", new int[] { '*' }, "*", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "+", new int[] { '+' }, "+", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ",", new int[] { ',' }, ",", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "-", new int[] { '-' }, "-", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ".", new int[] { '.' }, ".", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "/", new int[] { '/' }, "/", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ":", new int[] { ':' }, ":", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ";", new int[] { ';' }, ";", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "<", new int[] { '<' }, "<", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "=", new int[] { '=' }, "=", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, ">", new int[] { '>' }, ">", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "?", new int[] { '?' }, "?", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "@", new int[] { '@' }, "@", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "[", new int[] { '[' }, "[", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "\\", new int[] { '\\' }, "\\", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "]", new int[] { ']' }, "]", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "^", new int[] { '^' }, "^", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "_", new int[] { '_' }, "_", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "`", new int[] { '`' }, "`", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "{", new int[] { '{' }, "{", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "|", new int[] { '|' }, "|", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "}", new int[] { '}' }, "}", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, "~", new int[] { '~' }, "~", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(true, " ", new int[] { ' ' }, " ", InputCustomRomanV1.Ope_NO_CUSTOM);

            // Zenkaku Sign
            AddStroke(false, "！", new int[] { '!' }, "!", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "”", new int[] { '\"' }, "\"", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＃", new int[] { '#' }, "#", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＄", new int[] { '$' }, "$", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "％", new int[] { '%' }, "%", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＆", new int[] { '&' }, "&", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "’", new int[] { '\'' }, "\'", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "‘", new int[] { '`' }, "`", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "（", new int[] { '(' }, "(", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "）", new int[] { ')' }, ")", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＊", new int[] { '*' }, "*", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＋", new int[] { '+' }, "+", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "、", new int[] { ',' }, ",", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "，", new int[] { ',' }, ",", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "－", new int[] { '-' }, "-", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "ー", new int[] { '-' }, "-", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "．", new int[] { '.' }, ".", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "。", new int[] { '.' }, ".", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "／", new int[] { '/' }, "/", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "：", new int[] { ':' }, ":", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "；", new int[] { ';' }, ";", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＜", new int[] { '<' }, "<", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＝", new int[] { '=' }, "=", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＞", new int[] { '>' }, ">", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "？", new int[] { '?' }, "?", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＠", new int[] { '@' }, "@", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "［", new int[] { '[' }, "[", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "「", new int[] { '[' }, "[", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "￥", new int[] { '\\' }, "\\", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "］", new int[] { ']' }, "]", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "」", new int[] { ']' }, "]", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＾", new int[] { '^' }, "^", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "＿", new int[] { '_' }, "_", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "｀", new int[] { '`' }, "`", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "｛", new int[] { '{' }, "{", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "｜", new int[] { '|' }, "|", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "｝", new int[] { '}' }, "}", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "～", new int[] { '~' }, "~", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "　", new int[] { ' ' }, " ", InputCustomRomanV1.Ope_NO_CUSTOM);
            AddStroke(false, "・", new int[] { '/' }, "/", InputCustomRomanV1.Ope_NO_CUSTOM);
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
        internal void AddStroke(bool showKeyArray, string word, int[] stroke, string input, BitFlag flags)
        {
            // Convert stroke from virtual to physical
            var convertedStrokes101 = ConvertStrokes(Alphabet101, stroke);

            // Add each stroke
            foreach (var convertedStroke in convertedStrokes101)
            {
                AddStrokeInternal(Alphabet101, showKeyArray, word, convertedStroke, input, flags);
            }

            var convertedStrokes106 = ConvertStrokes(Alphabet106, stroke);

            // Add each stroke
            foreach (var convertedStroke in convertedStrokes106)
            {
                AddStrokeInternal(Alphabet106, showKeyArray, word, convertedStroke, input, flags);
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
        internal void AddStrokeInternal(int plane, bool showKeyArray, string word, Tuple<int, int>[] stroke, string input, BitFlag flags)
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
                Custom = flags
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

            // Cache for a shortcut which is start from the first N to next automaton
            List<ShortcutInfo> hatuonShortcut = new List<ShortcutInfo>();

            // Cache for a shortcut which is start from Sokuon to next automaton
            List<ShortcutInfo> sokuonShortcut = new List<ShortcutInfo>();

            // Cache physical key code for "n"
            var N = KeyMaps[plane].Where(x => x.VirtualKey == 'n');
            var X = KeyMaps[plane].Where(x => x.VirtualKey == 'x');
            int dikN = N.Count() == 1 ? N.First().PhysicalKey : (int)WTKeyCode.N;
            int dikX = X.Count() == 1 ? X.First().PhysicalKey : (int)WTKeyCode.X;

            if (VersionHelper.WTVersion < 405)
            {
                // There is a bug in the version less than 405
                // To be compatible with that, we set dikX to Virtual key
                dikX = (int)WTKeyCode.X;
            }

            // Make automaton
            int sokuon = 0; // Sokuon counter
            int hatuon = 0;     // Hatuon counter
            for (int i = 0; i < s.Length; i++)
            {
                InputAutomaton next;

                // Get next input character (Hiragana, Alphabet, Number, Sign)
                c = s.Substring(i, 1);

                // Convert the input character to Katakana, Large alphabet, Hankaku.
                // In old version, As SJIS does not have Hiragana of "ヴ", we converted that to Katakana
                c = c.Hira2Kana();
                c = c.Zenkaku2Hankaku();

                if (c.Equals("ッ"))
                {
                    // Process Sokuon
                    if (hatuon != 0)
                    {
                        // "んっ"
                        current = ProcHatuonSokuon(current, hatuonShortcut, hatuon, dikN, dikX);
                        hatuon = 0;
                    }

                    // Increment Sokuon Counter
                    sokuon++;
                    continue;
                }
                else if (c == "ン")
                {
                    if (sokuon != 0)
                    {
                        // "っん"
                        current = ProcSokuonHatuon(plane, current, sokuonShortcut, sokuon);
                        sokuon = 0;
                    }

                    // Increment Hatuon Counter
                    hatuon++;
                    continue;
                }

                next = new InputAutomaton();
                if (sokuon != 0)
                {
                    // "っ"
                    current = ProcessSokuon(plane, s, current, ref c, ref i, next, sokuonShortcut, sokuon);
                    sokuon = 0;
                }
                else if (hatuon != 0)
                {
                    // "ん"
                    current = ProcessHatuon(plane, s, current, ref c, ref i, next, hatuonShortcut, hatuon, dikN, dikX);
                    hatuon = 0;
                }
                else
                {
                    // Normal character
                    current = ProcessNormal(plane, s, current, ref c, ref i, next);
                }

                current = next;
            }

            if (sokuon != 0)
            {
                // Last character is "っ"
                current = ProcessLastSokuon(plane, current, sokuon);
            }

            if (hatuon != 0)
            {
                // Last character is "ん"
                current = ProcessLastHatuon(current, hatuon, dikN, dikX);
            }

            // Sokuon shortcut
            //   っった->xxta
            ProcessSokuonShortcut(sokuonShortcut);

            // Hatuon shortcut
            //   nn->n
            ProcessHatuonShortcut(hatuonShortcut, dikN);

            return start;
        }

        /// <summary>
        // Process "[ん]+っ"
        /// </summary>
        /// <param name="current">Automaton</param>
        /// <param name="hatuonShortcut">Shortcut</param>
        /// <param name="hatuon">Hatuon Counter</param>
        /// <param name="dikN">N key code</param>
        /// <returns>Next automaton</returns>
        internal InputAutomaton ProcHatuonSokuon(InputAutomaton current, List<ShortcutInfo> hatuonShortcuts, int hatuonCount, int dikN, int dikX)
        {
            // Prev-> ([N->N]|[X->N])+ ->Next

            InputAutomaton state_n = new InputAutomaton();
            InputAutomaton state_x = new InputAutomaton();
            InputAutomaton state_nn_or_xn = new InputAutomaton();
            for (int hatuon = 0; hatuon < hatuonCount; hatuon++)
            {
                // prev -> n
                state_n = new InputAutomaton();
                current.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_n,
                    Flags = InputCustomRomanV1.Ope_NO_XN,
                    Character = "n",
                });

                // prev -> n -> nn
                state_nn_or_xn = new InputAutomaton();
                state_n.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_or_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                // prev -> x
                state_x = new InputAutomaton();
                current.SetConnect(dikX, new InputAutomaton.Connect()
                {
                    Automaton = state_x,
                    Flags = InputCustomRomanV1.Ope_XN,
                    Character = "x",
                });

                // prev -> x ->xn
                state_x.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_or_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                current = state_nn_or_xn;
            }

            // Cache shortcut: [prev -> n -> nn -> next] to [prev -> n -> next]
            hatuonShortcuts.Add(new ShortcutInfo() { First = state_n, Second = state_nn_or_xn });

            return current;
        }

        /// <summary>
        /// Process "[っ]+ん"
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="current">Automaton</param>
        /// <param name="sokuonShortcuts">Shortcut</param>
        /// <param name="sokuonCount">Sokuon counter</param>
        /// <returns>Next automaton</returns>
        internal InputAutomaton ProcSokuonHatuon(int plane, InputAutomaton current, List<ShortcutInfo> sokuonShortcuts, int sokuonCount)
        {
            // Next automaton (e.g. っ"ん" -> xtu"n"n)
            var last = new InputAutomaton();

            // Save prev for shourtcut
            InputAutomaton prev = current;

            for (int sokuon = 0; sokuon < sokuonCount; sokuon++)
            {
                InputAutomaton state_xtu_or_ltu;
                if (sokuon == sokuonCount - 1)
                {
                    // Last "っ"

                    // Create shortcut for Prev to Next (e.g. っっった->ttta)
                    state_xtu_or_ltu = last;
                    sokuonShortcuts.Add(new ShortcutInfo()
                    {
                        First = prev,
                        Second = state_xtu_or_ltu,
                        Last = last,
                        Hatuon = true
                    });

                    // Create xtu and ltu
                    CreateAutomaton(plane, prev, "ッ", state_xtu_or_ltu, InputCustomRomanV1.Ope_NO_CUSTOM);

                    // Update prev
                    prev = state_xtu_or_ltu;
                }
                else
                {
                    // Middle "っ"

                    // Create shortcut for Prev to Next (e.g. っっった->ttta)
                    state_xtu_or_ltu = new InputAutomaton();
                    sokuonShortcuts.Add(new ShortcutInfo()
                    {
                        First = prev,
                        Second = state_xtu_or_ltu,
                        Last = last,
                        Hatuon = true
                    });

                    // Create xtu and ltu
                    // The next character is "っ" so select SMALL custom
                    CreateAutomaton(plane, prev, "ッ", state_xtu_or_ltu, InputCustomRomanV1.Ope_SMALL);

                    // Update prev
                    prev = state_xtu_or_ltu;
                }

                current = state_xtu_or_ltu;
            }

            return current;
        }

        /// <summary>
        /// Process "っ"
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="s">String</param>
        /// <param name="current">Current automaton</param>
        /// <param name="c">Char</param>
        /// <param name="index">Index</param>
        /// <param name="next">Next automaton</param>
        /// <param name="sokuonShortcuts">Sokuon shortcut</param>
        /// <param name="sokuonCount">Sokuon counter</param>
        /// <returns></returns>
        internal InputAutomaton ProcessSokuon(int plane, string s, InputAutomaton current, ref string c, ref int index, InputAutomaton next, List<ShortcutInfo> sokuonShortcuts, int sokuonCount)
        {
            // Process Sokuon

            // The next state (e.g. "っっっ"た"" -> ttt"t"a)
            var state_last_xtu_or_ltu = new InputAutomaton();

            var state_prev = current;

            for (int sokuon = 0; sokuon < sokuonCount; sokuon++)
            {
                InputAutomaton state_xtu_or_ltu;
                if (sokuon == sokuonCount - 1)
                {
                    // Last "っ"

                    // Create shortcut for Prev to Next (e.g. っっった->ttta)
                    state_xtu_or_ltu = state_last_xtu_or_ltu;
                    sokuonShortcuts.Add(new ShortcutInfo()
                    {
                        First = state_prev,
                        Second = state_xtu_or_ltu,
                        Last = state_last_xtu_or_ltu,
                        Hatuon = false
                    });

                    if (!_sokuonmap.Contains(c))
                    {
                        // Create xtu and ltu
                        // The next character can use tta so select SMALL custom
                        CreateAutomaton(plane, state_prev, "ッ", state_xtu_or_ltu, InputCustomRomanV1.Ope_SMALL);
                        state_prev = state_xtu_or_ltu;
                    }
                    else
                    {
                        // Create xtu and ltu.
                        // The next character can't use tta so no choice
                        CreateAutomaton(plane, state_prev, "ッ", state_xtu_or_ltu, InputCustomRomanV1.Ope_NO_CUSTOM);
                        state_prev = state_xtu_or_ltu;
                    }
                }
                else
                {
                    // Middle "っ"

                    // Create shortcut for Prev to Next (e.g. っっった->ttta)
                    state_xtu_or_ltu = new InputAutomaton();
                    sokuonShortcuts.Add(new ShortcutInfo()
                    {
                        First = state_prev,
                        Second = state_xtu_or_ltu,
                        Last = state_last_xtu_or_ltu,
                        Hatuon = false
                    });

                    // Create xtu and ltu
                    // The next character is "っ" so select SMALL custom
                    CreateAutomaton(plane, state_prev, "ッ", state_xtu_or_ltu, InputCustomRomanV1.Ope_SMALL);
                    state_prev = state_xtu_or_ltu;
                }
            }

            BitFlag flags = InputCustomRomanV1.Ope_NO_CUSTOM;
            string c1 = c;
            string c2;
            if ((index + 1) != s.Length)
            {
                // e.g. in case "っくぁ",
                //      make xtu ku xa, xtu ku la
                // After this if statement, we make kkwa
                c2 = s.Substring(index + 1, 1);
                c2 = c2.Hira2Kana();
                c2 = c2.Zenkaku2Hankaku();
                if (_strokemap[plane].ContainsKey(c1 + c2))
                {
                    index++;
                    InputAutomaton state_xtu_or_ltu = new InputAutomaton();
                    CreateAutomaton(plane, state_last_xtu_or_ltu, c, state_xtu_or_ltu, InputCustomRomanV1.Ope_SMALL);
                    CreateAutomaton(plane, state_xtu_or_ltu, c2, next, InputCustomRomanV1.Ope_SMALL);
                    c = c1 + c2;
                    flags = InputCustomRomanV1.Ope_NO_SMALL;
                }
            }

            // Succeeding character (e.g. っっ"た")
            CreateAutomaton(plane, state_last_xtu_or_ltu, c, next, flags);

            return current;
        }

        /// <summary>
        /// Process "ん"
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="s">String</param>
        /// <param name="current">Current automaton</param>
        /// <param name="c">Char</param>
        /// <param name="index">Index</param>
        /// <param name="next">Next automaton</param>
        /// <param name="hatuonShortcuts">Hatuon shortcut</param>
        /// <param name="hatuonCount">Hatuon counter</param>
        /// <param name="dikN">Key code for n</param>
        /// <returns></returns>
        internal InputAutomaton ProcessHatuon(int plane, string s, InputAutomaton current, ref string c, ref int index, InputAutomaton next, List<ShortcutInfo> hatuonShortcuts, int hatuonCount, int dikN, int dikX)
        {
            // Process Hatuon

            // Prev-> ([N->N]|[X->N])+ ->Next
            InputAutomaton state_n = new InputAutomaton();
            InputAutomaton state_x = new InputAutomaton();
            InputAutomaton state_nn_xn = new InputAutomaton();
            for (int hatuon = 0; hatuon < hatuonCount; hatuon++)
            {
                state_n = new InputAutomaton();
                current.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_n,
                    Flags = InputCustomRomanV1.Ope_NO_XN,
                    Character = "n",
                });

                state_nn_xn = new InputAutomaton();
                state_n.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                state_x = new InputAutomaton();
                current.SetConnect(dikX, new InputAutomaton.Connect()
                {
                    Automaton = state_x,
                    Flags = InputCustomRomanV1.Ope_XN,
                    Character = "x",
                });

                state_x.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                current = state_nn_xn;
            }

            // Create shortcut for N->N (Can use single N for NN)
            hatuonShortcuts.Add(new ShortcutInfo() { First = state_n, Second = state_nn_xn });

            BitFlag flags = InputCustomRomanV1.Ope_NO_CUSTOM;
            string c1 = c;
            string c2;
            if ((index + 1) != s.Length)
            {
                // e.g. in case "んにゃ",
                //      make nn ni xya, nn ni lxa
                // After this if statement, we make nnnya
                c2 = s.Substring(index + 1, 1);
                c2 = JapaneseHelper.Hira2Kana(c2);
                c2 = JapaneseHelper.Zenkaku2Hankaku(c2);
                if (_strokemap[plane].ContainsKey(c1 + c2))
                {
                    index++;
                    InputAutomaton tmp = new InputAutomaton();
                    CreateAutomaton(plane, state_nn_xn, c, tmp, InputCustomRomanV1.Ope_SMALL);
                    CreateAutomaton(plane, tmp, c2, next, InputCustomRomanV1.Ope_SMALL);
                    c = c1 + c2;
                    flags = InputCustomRomanV1.Ope_NO_SMALL;
                }
            }

            // Succeeding character (e.g. んん"た")
            CreateAutomaton(plane, current, c, next, flags);

            return current;
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
            BitFlag custom = InputCustomRomanV1.Ope_NO_CUSTOM;

            string c1 = c;
            string c2;
            if ((index + 1) != s.Length)
            {
                // Process 2 character case
                c2 = s.Substring(index + 1, 1);
                c2 = JapaneseHelper.Hira2Kana(c2);
                c2 = JapaneseHelper.Zenkaku2Hankaku(c2);
                if (_strokemap[plane].ContainsKey(c1 + c2))
                {
                    index++;
                    InputAutomaton tmp = new InputAutomaton();
                    CreateAutomaton(plane, current, c, tmp, InputCustomRomanV1.Ope_SMALL);
                    CreateAutomaton(plane, tmp, c2, next, InputCustomRomanV1.Ope_SMALL);
                    c = c1 + c2;
                    custom = InputCustomRomanV1.Ope_NO_SMALL;
                }
            }

            // Add automaton
            CreateAutomaton(plane, current, c, next, custom);

            return current;
        }

        /// <summary>
        /// Process last soluon (e.g. "あっ" -> axtu/altu)
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="current">Current automaton</param>
        /// <param name="sokuonCount">Sokuo counter</param>
        /// <returns></returns>
        internal InputAutomaton ProcessLastSokuon(int plane, InputAutomaton current, int sokuonCount)
        {
            // Add jxtu/ltu without shourtcut for xx/ll
            for (int sokuon = 0; sokuon < sokuonCount; sokuon++)
            {
                InputAutomaton state_xtu_ltu = new InputAutomaton();
                CreateAutomaton(plane, current, "ッ", state_xtu_ltu, InputCustomRomanV1.Ope_NO_CUSTOM);
                current = state_xtu_ltu;
            }

            return current;
        }

        /// <summary>
        /// Process last Hatuon (e.g. "たん" -> tann)
        /// </summary>
        /// <param name="current">Current automaton</param>
        /// <param name="hatuonCount">Hatuon counter</param>
        /// <param name="dikN">Key code for n</param>
        /// <returns></returns>
        internal static InputAutomaton ProcessLastHatuon(InputAutomaton current, int hatuonCount, int dikN, int dikX)
        {
            // Add nn/xn without shortcut for nn->n
            for (int hatuon = 0; hatuon < hatuonCount; hatuon++)
            {
                InputAutomaton state_n = new InputAutomaton();
                current.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_n,
                    Flags = InputCustomRomanV1.Ope_NO_XN,
                    Character = "n",
                });

                InputAutomaton state_nn_xn = new InputAutomaton();
                state_n.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                InputAutomaton state_x = new InputAutomaton();
                current.SetConnect(dikX, new InputAutomaton.Connect()
                {
                    Automaton = state_x,
                    Flags = InputCustomRomanV1.Ope_XN,
                    Character = "x",
                });

                state_x.SetConnect(dikN, new InputAutomaton.Connect()
                {
                    Automaton = state_nn_xn,
                    Flags = InputCustomRomanV1.Ope_NO_CUSTOM,
                    Character = "n",
                });

                current = state_nn_xn;
            }

            return current;
        }

        /// <summary>
        /// Process Sokuon shortcut (e.g. った -> tta
        /// </summary>
        /// <param name="sokuonShortcuts">Shortcut</param>
        internal void ProcessSokuonShortcut(List<ShortcutInfo> sokuonShortcuts)
        {
            // Process reversely
            sokuonShortcuts.Reverse();

            foreach (var sokuonShortcut in sokuonShortcuts)
            {
                // For prev -> [x/l -> t -> u]+ -> next,
                //     First : prev state
                //     Second: next state
                //     Last  : last next state

                foreach (var connection_from_second in sokuonShortcut.Second.GetConnect())
                {
                    if (sokuonShortcut.Hatuon)
                    {
                        // っん
                        if (!sokuonShortcut.First.GetConnect().ContainsKey(connection_from_second.Key))
                        {
                            // Not process っん -> nnn
                            // (n is not included in connection)
                            continue;
                        }
                        else
                        {
                            // Process っん -> xxn
                            // (x is included in connection (the first letter in "xtu"))
                        }
                    }
                    else if (_sokuonmap.Contains(connection_from_second.Value.Character))
                    {
                        // Not process っあ、っい... -> aaa, iii...
                        continue;
                    }

                    InputAutomaton intermidiate = null;

                    if (sokuonShortcut.First.GetConnect().ContainsKey(connection_from_second.Key))
                    {
                        // For sequencial of xxx/lll (e.g. "っっぁ" -> xxxa/llla)
                        // The repeat character is x or l so intermidiate automaton is existing
                        // original)   x    t    u    x    t    u    x    a
                        // original) F--->I---> --->S---> ---> --->L---> --->
                        // add     )             x               x
                        // add     )       -------------> ------------->
                        //
                        //            F:First, S:Second, L:Last, I:Intermidiate

                        // Get existing Intermidiate Automaton
                        intermidiate = sokuonShortcut.First.GetConnect()[connection_from_second.Key].Automaton;
                    }
                    else
                    {
                        // For sequencial of next first character (e.g. "っっく" -> kkka, cccu)
                        // The repeat character is not x or l so intermidiate automaton is not existing
                        // original)   x    t    u    x    t    u    t    a
                        // original) F---> ---> --->S---> ---> --->L---> --->
                        // add     )   k              k       k
                        // add     )  --->I--------- ---> ------------->
                        // add     )   c              c       c
                        // add     )  --->I--------- ---> ------------->
                        //
                        //            F:First, S:Second, L:Last, I:Intermidiate

                        // Create intermidiate automaton
                        intermidiate = new InputAutomaton();

                        // Connect new automaton from the First automaton
                        sokuonShortcut.First.SetConnect(connection_from_second.Key, new InputAutomaton.Connect()
                        {
                            Automaton = intermidiate,
                            Character = connection_from_second.Value.Character,
                            Flags = connection_from_second.Value.Flags.Or(InputCustomRomanV1.Ope_NO_SMALL),
                        });

                        // Second's connection is already created
                        // (Last one is succeeding character, the other is created by previous loop)
                    }

                    // Connect state_x to next state with x or l
                    intermidiate.SetConnect(connection_from_second.Key, new InputAutomaton.Connect()
                    {
                        Automaton = connection_from_second.Value.Automaton,
                        Character = connection_from_second.Value.Character,
                        // We can use xn for the case っん->xxn
                        Flags = sokuonShortcut.Hatuon ? InputCustomRomanV1.Ope_XN : InputCustomRomanV1.Ope_NO_CUSTOM,
                    });
                }
            }
        }

        /// <summary>
        /// Process Hatuon shortcut (e.g. んた -> nta
        /// </summary>
        /// <param name="hatuonShortcuts">Shortcut</param>
        /// <param name="dikN">Key code for n</param>
        internal void ProcessHatuonShortcut(List<ShortcutInfo> hatuonShortcuts, int dikN)
        {
            foreach (var hatuonShortcut in hatuonShortcuts)
            {
                // For prev -> n -> nn -> next,
                //     First : n state
                //     Second: nn state
                // Connect first to next (all connection of second)
                foreach (var connection_from_nn in hatuonShortcut.Second.GetConnect())
                {
                    string tmp = string.Empty;
                    tmp += connection_from_nn.Value.Character;
                    if (!_nnmap.Contains(tmp))
                    {
                        // The prev character can make nn to n
                        hatuonShortcut.First.SetConnect(connection_from_nn.Key, new InputAutomaton.Connect()
                        {
                            Automaton = connection_from_nn.Value.Automaton,
                            Flags = connection_from_nn.Value.Flags.Or(InputCustomRomanV1.Ope_NO_NN),
                            Character = connection_from_nn.Value.Character,
                        });

                        hatuonShortcut.First.GetConnect(dikN).Flags = hatuonShortcut.First.GetConnect(dikN).Flags.Or(InputCustomRomanV1.Ope_NN);
                    }
                }
            }
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

                            // Remove flags which can't determine by the first character
                            if (alphabet == 0)
                            {
                                connect.Flags = AdjustFlags(connect.Flags);
                            }

                            // Remove Youon flags
                            connect.Flags = connect.Flags.And(InputCustomRomanV1.Ope_SMALL.Not());
                            connect.Flags = connect.Flags.And(InputCustomRomanV1.Ope_NO_SMALL.Not());

                            current = connect.Automaton;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Conbination of all NOT flags
        /// </summary>
        internal static BitFlag NotAll = InputCustomRomanV1.Ope_SH.Not()
                           .And(InputCustomRomanV1.Ope_NO_SH.Not())
                           .And(InputCustomRomanV1.Ope_TS.Not())
                           .And(InputCustomRomanV1.Ope_NO_TS.Not())
                           .And(InputCustomRomanV1.Ope_YIE.Not())
                           .And(InputCustomRomanV1.Ope_NO_YIE.Not())
                           .And(InputCustomRomanV1.Ope_VY.Not())
                           .And(InputCustomRomanV1.Ope_NO_VY.Not())
                           .And(InputCustomRomanV1.Ope_QW.Not())
                           .And(InputCustomRomanV1.Ope_NO_QW.Not())
                           .And(InputCustomRomanV1.Ope_FY.Not())
                           .And(InputCustomRomanV1.Ope_NO_FY.Not())
                           .And(InputCustomRomanV1.Ope_JY.Not())
                           .And(InputCustomRomanV1.Ope_NO_JY.Not());

        /// <summary>
        /// Adjust input flags to make it deterministic
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        internal BitFlag AdjustFlags(BitFlag flag)
        {
            // Remove all the conflict flags
            flag = flag.And(NotAll);

            if (flag.IsSet(InputCustomRomanV1.Ope_CH)
                && flag.IsSet(InputCustomRomanV1.Ope_NO_CH))
            {
                flag = flag.And(InputCustomRomanV1.Ope_CH.Not())
                               .And(InputCustomRomanV1.Ope_NO_CH.Not());
            }

            if (flag.IsSet(InputCustomRomanV1.Ope_CY)
                && flag.IsSet(InputCustomRomanV1.Ope_NO_CY))
            {
                flag = flag.And(InputCustomRomanV1.Ope_CY.Not())
                               .And(InputCustomRomanV1.Ope_NO_CY.Not());
            }

            return flag;
        }

        internal static BitFlag Small = InputCustomRomanV1.Ope_SMALL.Or(InputCustomRomanV1.Ope_NO_SMALL).Not();

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
                    // Select the route that fits Youon settings
                    //   e.g. Do not use "F" and string is "ファ",
                    //        Do not use HUXA, use FA.
                    foreach (var i in automaton.GetConnect())
                    {
                        if (i.Value.Flags.IsSet(InputCustomRomanV1.Ope_SMALL)
                            && custom.IsSet(InputCustomRomanV1.Ope_SMALL))
                        {
                            c = i.Key;
                            addstring = i.Value.Character;
                            break;
                        }
                        else if (i.Value.Flags.IsSet(InputCustomRomanV1.Ope_NO_SMALL)
                            && custom.IsSet(InputCustomRomanV1.Ope_NO_SMALL))
                        {
                            c = i.Key;
                            addstring = i.Value.Character;
                            break;
                        }
                    }
                }

                if (c == '\0')
                {
                    // Select the route which is not based on Youon
                    foreach (var i in automaton.GetConnect())
                    {
                        if (!(i.Value.Flags.IsSet(InputCustomRomanV1.Ope_SMALL))
                            && !(i.Value.Flags.IsSet(InputCustomRomanV1.Ope_NO_SMALL)))
                        {
                            c = i.Key;
                            addstring = i.Value.Character;
                            break;
                        }
                    }
                }

                if (c == '\0')
                {
                    // Remove Youon and select again
                    foreach (var i in automaton.GetConnect())
                    {
                        if (custom.IsSet(i.Value.Flags.And(Small)))
                        {
                            c = i.Key;
                            addstring = i.Value.Character;
                            break;
                        }
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
            label = label.Hira2Kana();
            label = label.Zenkaku2Hankaku();

            if (!_stringToCode[plane].ContainsKey(label))
            {
                return -1;
            }

            return _stringToCode[plane][label];
        }
    }
}
