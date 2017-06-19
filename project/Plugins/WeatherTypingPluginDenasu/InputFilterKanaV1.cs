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
    /// Input Filter for Kana
    /// </summary>
    public class InputFilterKanaV1 : MarshalByRefObject, IInputFilter
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
                return "かな";
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
                return "かな入力 Input Filter";
            }
        }

        /// <summary>
        /// Map for Kana to Stroke
        /// </summary>
        internal Dictionary<string, List<KeyStroke>> _kanamap;

        /// <summary>
        /// Convert code to string map
        /// </summary>
        internal List<Dictionary<int, string>> _codeToString;

        /// <summary>
        /// Convert string to code map
        /// </summary>
        internal List<Dictionary<string, int>> _stringToCode;

        /// <summary>
        /// Planes
        /// </summary>
        internal const int NoPlane = -1;
        internal const int Alphabet = 0;
        internal const int Kana = 1;
        internal const int MaxPlane = Kana + 1;

        /// <summary>
        /// Key map
        /// </summary>
        public List<List<KeyMap>> KeyMaps { set; get; }

        /// <summary>
        /// Supported Mode
        /// </summary>
        private static List<string> _supportedMode = new List<string>()
        {
            // かな mode
            "かな",
            // アルファベット mode
            "アルファベット",
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
                if (keyboard.Name == "106 キーボード")
                {
                    if (mode == SupportedMode[0])
                    {
                        return Kana;
                    }
                    else if (mode == SupportedMode[1])
                    {
                        return Alphabet;
                    }
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
            return new InputCustomDefaultV1();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InputFilterKanaV1()
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
            _kanamap = new Dictionary<string, List<input.KeyStroke>>();

            /// Convert code to string map
            _codeToString = new List<Dictionary<int, string>>();

            /// Convert string to code map
            _stringToCode = new List<Dictionary<string, int>>();

            for (var i = 0; i < MaxPlane; i++)
            {
                _codeToString.Add(new Dictionary<int, string>());
                _stringToCode.Add(new Dictionary<string, int>());
            }

            //
            // Kana Plane
            //

            // Kana
            AddStroke(Kana, true, "ア", new int[] { 'あ' }, "あ");
            AddStroke(Kana, true, "イ", new int[] { 'い' }, "い");
            AddStroke(Kana, true, "ウ", new int[] { 'う' }, "う");
            AddStroke(Kana, true, "エ", new int[] { 'え' }, "え");
            AddStroke(Kana, true, "オ", new int[] { 'お' }, "お");
            AddStroke(Kana, true, "カ", new int[] { 'か' }, "か");
            AddStroke(Kana, true, "キ", new int[] { 'き' }, "き");
            AddStroke(Kana, true, "ク", new int[] { 'く' }, "く");
            AddStroke(Kana, true, "ケ", new int[] { 'け' }, "け");
            AddStroke(Kana, true, "コ", new int[] { 'こ' }, "こ");
            AddStroke(Kana, true, "サ", new int[] { 'さ' }, "さ");
            AddStroke(Kana, true, "シ", new int[] { 'し' }, "し");
            AddStroke(Kana, true, "ス", new int[] { 'す' }, "す");
            AddStroke(Kana, true, "セ", new int[] { 'せ' }, "せ");
            AddStroke(Kana, true, "ソ", new int[] { 'そ' }, "そ");
            AddStroke(Kana, true, "タ", new int[] { 'た' }, "た");
            AddStroke(Kana, true, "チ", new int[] { 'ち' }, "ち");
            AddStroke(Kana, true, "ツ", new int[] { 'つ' }, "つ");
            AddStroke(Kana, true, "テ", new int[] { 'て' }, "て");
            AddStroke(Kana, true, "ト", new int[] { 'と' }, "と");
            AddStroke(Kana, true, "ナ", new int[] { 'な' }, "な");
            AddStroke(Kana, true, "ニ", new int[] { 'に' }, "に");
            AddStroke(Kana, true, "ヌ", new int[] { 'ぬ' }, "ぬ");
            AddStroke(Kana, true, "ネ", new int[] { 'ね' }, "ね");
            AddStroke(Kana, true, "ノ", new int[] { 'の' }, "の");
            AddStroke(Kana, true, "ハ", new int[] { 'は' }, "は");
            AddStroke(Kana, true, "ヒ", new int[] { 'ひ' }, "ひ");
            AddStroke(Kana, true, "フ", new int[] { 'ふ' }, "ふ");
            AddStroke(Kana, true, "ヘ", new int[] { 'へ' }, "へ");
            AddStroke(Kana, true, "ホ", new int[] { 'ほ' }, "ほ");
            AddStroke(Kana, true, "マ", new int[] { 'ま' }, "ま");
            AddStroke(Kana, true, "ミ", new int[] { 'み' }, "み");
            AddStroke(Kana, true, "ム", new int[] { 'む' }, "む");
            AddStroke(Kana, true, "メ", new int[] { 'め' }, "め");
            AddStroke(Kana, true, "モ", new int[] { 'も' }, "も");
            AddStroke(Kana, true, "ヤ", new int[] { 'や' }, "や");
            AddStroke(Kana, true, "ユ", new int[] { 'ゆ' }, "ゆ");
            AddStroke(Kana, true, "ヨ", new int[] { 'よ' }, "よ");
            AddStroke(Kana, true, "ラ", new int[] { 'ら' }, "ら");
            AddStroke(Kana, true, "リ", new int[] { 'り' }, "り");
            AddStroke(Kana, true, "ル", new int[] { 'る' }, "る");
            AddStroke(Kana, true, "レ", new int[] { 'れ' }, "れ");
            AddStroke(Kana, true, "ロ", new int[] { 'ろ' }, "ろ");
            AddStroke(Kana, true, "ワ", new int[] { 'わ' }, "わ");
            AddStroke(Kana, true, "ヲ", new int[] { 'を' }, "を");
            AddStroke(Kana, true, "ン", new int[] { 'ん' }, "ん");

            // Kana + Dakuten
            AddStroke(Kana, false, "ガ", new int[] { 'か', '゛' }, "が");
            AddStroke(Kana, false, "ギ", new int[] { 'き', '゛' }, "ぎ");
            AddStroke(Kana, false, "グ", new int[] { 'く', '゛' }, "ぐ");
            AddStroke(Kana, false, "ゲ", new int[] { 'け', '゛' }, "げ");
            AddStroke(Kana, false, "ゴ", new int[] { 'こ', '゛' }, "ご");
            AddStroke(Kana, false, "ザ", new int[] { 'さ', '゛' }, "ざ");
            AddStroke(Kana, false, "ジ", new int[] { 'し', '゛' }, "じ");
            AddStroke(Kana, false, "ズ", new int[] { 'す', '゛' }, "ず");
            AddStroke(Kana, false, "ゼ", new int[] { 'せ', '゛' }, "ぜ");
            AddStroke(Kana, false, "ゾ", new int[] { 'そ', '゛' }, "ぞ");
            AddStroke(Kana, false, "ダ", new int[] { 'た', '゛' }, "だ");
            AddStroke(Kana, false, "ヂ", new int[] { 'ち', '゛' }, "ぢ");
            AddStroke(Kana, false, "ヅ", new int[] { 'つ', '゛' }, "づ");
            AddStroke(Kana, false, "デ", new int[] { 'て', '゛' }, "で");
            AddStroke(Kana, false, "ド", new int[] { 'と', '゛' }, "ど");
            AddStroke(Kana, false, "バ", new int[] { 'は', '゛' }, "ば");
            AddStroke(Kana, false, "ビ", new int[] { 'ひ', '゛' }, "び");
            AddStroke(Kana, false, "ブ", new int[] { 'ふ', '゛' }, "ぶ");
            AddStroke(Kana, false, "ベ", new int[] { 'へ', '゛' }, "べ");
            AddStroke(Kana, false, "ボ", new int[] { 'ほ', '゛' }, "ぼ");
            AddStroke(Kana, false, "パ", new int[] { 'は', '゜' }, "ぱ");
            AddStroke(Kana, false, "ピ", new int[] { 'ひ', '゜' }, "ぴ");
            AddStroke(Kana, false, "プ", new int[] { 'ふ', '゜' }, "ぷ");
            AddStroke(Kana, false, "ペ", new int[] { 'へ', '゜' }, "ぺ");
            AddStroke(Kana, false, "ポ", new int[] { 'ほ', '゜' }, "ぽ");
            AddStroke(Kana, false, "ヴ", new int[] { 'う', '゛' }, "ヴ");

            // Youon+Other
            AddStroke(Kana, true, "ァ", new int[] { 'ぁ' }, "ぁ");
            AddStroke(Kana, true, "ィ", new int[] { 'ぃ' }, "ぃ");
            AddStroke(Kana, true, "ゥ", new int[] { 'ぅ' }, "ぅ");
            AddStroke(Kana, true, "ェ", new int[] { 'ぇ' }, "ぇ");
            AddStroke(Kana, true, "ォ", new int[] { 'ぉ' }, "ぉ");
            AddStroke(Kana, true, "ッ", new int[] { 'っ' }, "っ");
            AddStroke(Kana, true, "ャ", new int[] { 'ゃ' }, "ゃ");
            AddStroke(Kana, true, "ュ", new int[] { 'ゅ' }, "ゅ");
            AddStroke(Kana, true, "ョ", new int[] { 'ょ' }, "ょ");
            AddStroke(Kana, true, "ヵ", new int[] { 'ヵ' }, "ヵ");
            AddStroke(Kana, true, "ヶ", new int[] { 'ヶ' }, "ヶ");
            AddStroke(Kana, true, "ヮ", new int[] { 'ヮ' }, "ゎ");
            AddStroke(Kana, true, "ヰ", new int[] { 'ヰ' }, "ゐ");
            AddStroke(Kana, true, "ヱ", new int[] { 'ヱ' }, "ゑ");

            // Single Dakuon for some key arrays
            AddStroke(Kana, true, "ガ", new int[] { 'が' }, "が");
            AddStroke(Kana, true, "ギ", new int[] { 'ぎ' }, "ぎ");
            AddStroke(Kana, true, "グ", new int[] { 'ぐ' }, "ぐ");
            AddStroke(Kana, true, "ゲ", new int[] { 'げ' }, "げ");
            AddStroke(Kana, true, "ゴ", new int[] { 'ご' }, "ご");
            AddStroke(Kana, true, "ザ", new int[] { 'ざ' }, "ざ");
            AddStroke(Kana, true, "ジ", new int[] { 'じ' }, "じ");
            AddStroke(Kana, true, "ズ", new int[] { 'ず' }, "ず");
            AddStroke(Kana, true, "ゼ", new int[] { 'ぜ' }, "ぜ");
            AddStroke(Kana, true, "ゾ", new int[] { 'ぞ' }, "ぞ");
            AddStroke(Kana, true, "ダ", new int[] { 'だ' }, "だ");
            AddStroke(Kana, true, "ヂ", new int[] { 'ぢ' }, "ぢ");
            AddStroke(Kana, true, "ヅ", new int[] { 'づ' }, "づ");
            AddStroke(Kana, true, "デ", new int[] { 'で' }, "で");
            AddStroke(Kana, true, "ド", new int[] { 'ど' }, "ど");
            AddStroke(Kana, true, "バ", new int[] { 'ば' }, "ば");
            AddStroke(Kana, true, "ビ", new int[] { 'び' }, "び");
            AddStroke(Kana, true, "ブ", new int[] { 'ぶ' }, "ぶ");
            AddStroke(Kana, true, "ベ", new int[] { 'べ' }, "べ");
            AddStroke(Kana, true, "ボ", new int[] { 'ぼ' }, "ぼ");
            AddStroke(Kana, true, "パ", new int[] { 'ぱ' }, "ぱ");
            AddStroke(Kana, true, "ピ", new int[] { 'ぴ' }, "ぴ");
            AddStroke(Kana, true, "プ", new int[] { 'ぷ' }, "ぷ");
            AddStroke(Kana, true, "ペ", new int[] { 'ぺ' }, "ぺ");
            AddStroke(Kana, true, "ポ", new int[] { 'ぽ' }, "ぽ");
            AddStroke(Kana, true, "ヴ", new int[] { 'ヴ' }, "ヴ");

            // Zenkaku Sign
            AddStroke(Kana, true, "！", new int[] { '！' }, "！");
            AddStroke(Kana, true, "”", new int[] { '”' }, "”");
            AddStroke(Kana, true, "＃", new int[] { '＃' }, "＃");
            AddStroke(Kana, true, "＄", new int[] { '＄' }, "＄");
            AddStroke(Kana, true, "％", new int[] { '％' }, "％");
            AddStroke(Kana, true, "＆", new int[] { '＆' }, "＆");
            AddStroke(Kana, true, "’", new int[] { '’' }, "’");
            AddStroke(Kana, true, "‘", new int[] { '‘' }, "‘");
            AddStroke(Kana, true, "（", new int[] { '（' }, "（");
            AddStroke(Kana, true, "）", new int[] { '）' }, "）");
            AddStroke(Kana, true, "＊", new int[] { '＊' }, "＊");
            AddStroke(Kana, true, "＋", new int[] { '＋' }, "＋");
            AddStroke(Kana, true, "、", new int[] { '、' }, "、");
            AddStroke(Kana, true, "，", new int[] { '，' }, "，");
            AddStroke(Kana, true, "－", new int[] { '－' }, "－");
            AddStroke(Kana, true, "ー", new int[] { 'ー' }, "ー");
            AddStroke(Kana, true, "．", new int[] { '．' }, "．");
            AddStroke(Kana, true, "。", new int[] { '。' }, "。");
            AddStroke(Kana, true, "／", new int[] { '／' }, "／");
            AddStroke(Kana, true, "：", new int[] { '：' }, "：");
            AddStroke(Kana, true, "；", new int[] { '；' }, "；");
            AddStroke(Kana, true, "＜", new int[] { '＜' }, "＜");
            AddStroke(Kana, true, "＝", new int[] { '＝' }, "＝");
            AddStroke(Kana, true, "＞", new int[] { '＞' }, "＞");
            AddStroke(Kana, true, "？", new int[] { '？' }, "？");
            AddStroke(Kana, true, "＠", new int[] { '＠' }, "＠");
            AddStroke(Kana, true, "［", new int[] { '［' }, "[");
            AddStroke(Kana, true, "「", new int[] { '「' }, "[");
            AddStroke(Kana, true, "￥", new int[] { '￥' }, "￥");
            AddStroke(Kana, true, "］", new int[] { '］' }, "]");
            AddStroke(Kana, true, "」", new int[] { '」' }, "]");
            AddStroke(Kana, true, "＾", new int[] { '＾' }, "＾");
            AddStroke(Kana, true, "＿", new int[] { '＿' }, "＿");
            AddStroke(Kana, true, "｀", new int[] { '｀' }, "｀");
            AddStroke(Kana, true, "｛", new int[] { '｛' }, "｛");
            AddStroke(Kana, true, "｜", new int[] { '｜' }, "｜");
            AddStroke(Kana, true, "｝", new int[] { '｝' }, "｝");
            AddStroke(Kana, true, "～", new int[] { '～' }, "～");
            AddStroke(Kana, true, "　", new int[] { '　' }, "　");
            AddStroke(Kana, true, "・", new int[] { '・' }, "・");

            // Zenkaku Additional Sign
            AddStroke(Kana, true, "゛", new int[] { '゛' }, null);
            AddStroke(Kana, true, "゜", new int[] { '゜' }, null);

            //
            // Alphabet Plane
            //
            
            // Number
            AddStroke(Alphabet, true, "0", new int[] { '0' }, "0");
            AddStroke(Alphabet, true, "1", new int[] { '1' }, "1");
            AddStroke(Alphabet, true, "2", new int[] { '2' }, "2");
            AddStroke(Alphabet, true, "3", new int[] { '3' }, "3");
            AddStroke(Alphabet, true, "4", new int[] { '4' }, "4");
            AddStroke(Alphabet, true, "5", new int[] { '5' }, "5");
            AddStroke(Alphabet, true, "6", new int[] { '6' }, "6");
            AddStroke(Alphabet, true, "7", new int[] { '7' }, "7");
            AddStroke(Alphabet, true, "8", new int[] { '8' }, "8");
            AddStroke(Alphabet, true, "9", new int[] { '9' }, "9");

            // Alphabet
            AddStroke(Alphabet, true, "a", new int[] { 'a' }, "a");
            AddStroke(Alphabet, true, "b", new int[] { 'b' }, "b");
            AddStroke(Alphabet, true, "c", new int[] { 'c' }, "c");
            AddStroke(Alphabet, true, "d", new int[] { 'd' }, "d");
            AddStroke(Alphabet, true, "e", new int[] { 'e' }, "e");
            AddStroke(Alphabet, true, "f", new int[] { 'f' }, "f");
            AddStroke(Alphabet, true, "g", new int[] { 'g' }, "g");
            AddStroke(Alphabet, true, "h", new int[] { 'h' }, "h");
            AddStroke(Alphabet, true, "i", new int[] { 'i' }, "i");
            AddStroke(Alphabet, true, "j", new int[] { 'j' }, "j");
            AddStroke(Alphabet, true, "k", new int[] { 'k' }, "k");
            AddStroke(Alphabet, true, "l", new int[] { 'l' }, "l");
            AddStroke(Alphabet, true, "m", new int[] { 'm' }, "m");
            AddStroke(Alphabet, true, "n", new int[] { 'n' }, "n");
            AddStroke(Alphabet, true, "o", new int[] { 'o' }, "o");
            AddStroke(Alphabet, true, "p", new int[] { 'p' }, "p");
            AddStroke(Alphabet, true, "q", new int[] { 'q' }, "q");
            AddStroke(Alphabet, true, "r", new int[] { 'r' }, "r");
            AddStroke(Alphabet, true, "s", new int[] { 's' }, "s");
            AddStroke(Alphabet, true, "t", new int[] { 't' }, "t");
            AddStroke(Alphabet, true, "u", new int[] { 'u' }, "u");
            AddStroke(Alphabet, true, "v", new int[] { 'v' }, "v");
            AddStroke(Alphabet, true, "w", new int[] { 'w' }, "w");
            AddStroke(Alphabet, true, "x", new int[] { 'x' }, "x");
            AddStroke(Alphabet, true, "y", new int[] { 'y' }, "y");
            AddStroke(Alphabet, true, "z", new int[] { 'z' }, "z");
            AddStroke(Alphabet, true, "A", new int[] { 'A' }, "A");
            AddStroke(Alphabet, true, "B", new int[] { 'B' }, "B");
            AddStroke(Alphabet, true, "C", new int[] { 'C' }, "C");
            AddStroke(Alphabet, true, "D", new int[] { 'D' }, "D");
            AddStroke(Alphabet, true, "E", new int[] { 'E' }, "E");
            AddStroke(Alphabet, true, "F", new int[] { 'F' }, "F");
            AddStroke(Alphabet, true, "G", new int[] { 'G' }, "G");
            AddStroke(Alphabet, true, "H", new int[] { 'H' }, "H");
            AddStroke(Alphabet, true, "I", new int[] { 'I' }, "I");
            AddStroke(Alphabet, true, "J", new int[] { 'J' }, "J");
            AddStroke(Alphabet, true, "K", new int[] { 'K' }, "K");
            AddStroke(Alphabet, true, "L", new int[] { 'L' }, "L");
            AddStroke(Alphabet, true, "M", new int[] { 'M' }, "M");
            AddStroke(Alphabet, true, "N", new int[] { 'N' }, "N");
            AddStroke(Alphabet, true, "O", new int[] { 'O' }, "O");
            AddStroke(Alphabet, true, "P", new int[] { 'P' }, "P");
            AddStroke(Alphabet, true, "Q", new int[] { 'Q' }, "Q");
            AddStroke(Alphabet, true, "R", new int[] { 'R' }, "R");
            AddStroke(Alphabet, true, "S", new int[] { 'S' }, "S");
            AddStroke(Alphabet, true, "T", new int[] { 'T' }, "T");
            AddStroke(Alphabet, true, "U", new int[] { 'U' }, "U");
            AddStroke(Alphabet, true, "V", new int[] { 'V' }, "V");
            AddStroke(Alphabet, true, "W", new int[] { 'W' }, "W");
            AddStroke(Alphabet, true, "X", new int[] { 'X' }, "X");
            AddStroke(Alphabet, true, "Y", new int[] { 'Y' }, "Y");
            AddStroke(Alphabet, true, "Z", new int[] { 'Z' }, "Z");

            // Sign
            AddStroke(Alphabet, true, "!", new int[] { '!' }, "!");
            AddStroke(Alphabet, true, "\"", new int[] { '\"' }, "\"");
            AddStroke(Alphabet, true, "#", new int[] { '#' }, "#");
            AddStroke(Alphabet, true, "$", new int[] { '$' }, "$");
            AddStroke(Alphabet, true, "%", new int[] { '%' }, "%");
            AddStroke(Alphabet, true, "&", new int[] { '&' }, "&");
            AddStroke(Alphabet, true, "\'", new int[] { '\'' }, "\'");
            AddStroke(Alphabet, true, "(", new int[] { '(' }, "(");
            AddStroke(Alphabet, true, ")", new int[] { ')' }, ")");
            AddStroke(Alphabet, true, "*", new int[] { '*' }, "*");
            AddStroke(Alphabet, true, "+", new int[] { '+' }, "+");
            AddStroke(Alphabet, true, ",", new int[] { ',' }, ",");
            AddStroke(Alphabet, true, "-", new int[] { '-' }, "-");
            AddStroke(Alphabet, true, ".", new int[] { '.' }, ".");
            AddStroke(Alphabet, true, "/", new int[] { '/' }, "/");
            AddStroke(Alphabet, true, ":", new int[] { ':' }, ":");
            AddStroke(Alphabet, true, ";", new int[] { ';' }, ";");
            AddStroke(Alphabet, true, "<", new int[] { '<' }, "<");
            AddStroke(Alphabet, true, "=", new int[] { '=' }, "=");
            AddStroke(Alphabet, true, ">", new int[] { '>' }, ">");
            AddStroke(Alphabet, true, "?", new int[] { '?' }, "?");
            AddStroke(Alphabet, true, "@", new int[] { '@' }, "@");
            AddStroke(Alphabet, true, "[", new int[] { '[' }, "[");
            AddStroke(Alphabet, true, "\\", new int[] { '\\' }, "\\");
            AddStroke(Alphabet, true, "]", new int[] { ']' }, "]");
            AddStroke(Alphabet, true, "^", new int[] { '^' }, "^");
            AddStroke(Alphabet, true, "_", new int[] { '_' }, "_");
            AddStroke(Alphabet, true, "`", new int[] { '`' }, "`");
            AddStroke(Alphabet, true, "{", new int[] { '{' }, "{");
            AddStroke(Alphabet, true, "|", new int[] { '|' }, "|");
            AddStroke(Alphabet, true, "}", new int[] { '}' }, "}");
            AddStroke(Alphabet, true, "~", new int[] { '~' }, "~");
            AddStroke(Alphabet, true, " ", new int[] { ' ' }, " ");
        }

        /// <summary>
        /// Helper method to add stroke
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="showKeyArray">Show key in Key Array</param>
        /// <param name="word">Word character</param>
        /// <param name="stroke">Stroke</param>
        /// <param name="input">Input character</param>
        internal void AddStroke(int plane, bool showKeyArray, string word, int[] stroke, string input)
        {
            // Convert stroke from virtual to physical
            var convertedStrokes = ConvertStrokes(plane, stroke);

            // Add each stroke
            foreach (var convertedStroke in convertedStrokes)
            {
                AddStrokeInternal(plane, showKeyArray, word, convertedStroke, input);
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
        /// <param name="input">Input character</param>
        internal void AddStrokeInternal(int plane, bool showKeyArray, string word, Tuple<int, int>[] stroke, string input)
        {
            // Add to map
            if (showKeyArray)
            {
                _codeToString[plane][stroke[0].Item1] = word;
                _stringToCode[plane][word] = stroke[0].Item1;
            }

            // Add to kana map
            if (!_kanamap.ContainsKey(word))
            {
                _kanamap.Add(word, new List<KeyStroke>());
            }
            else if (_kanamap[word].Where(x => x.Stroke.SequenceEqual(stroke.Select(y => y.Item2))).Count() > 0)
            {
                // The same word and stroke. Ignore
                return;
            }

            _kanamap[word].Add(new KeyStroke()
            {
                Stroke = stroke.Where(y => y.Item2 != -1).Select(y => y.Item2).ToArray(),
                Input = input,
                Custom = InputCustomDefaultV1.Ope_NO_CUSTOM
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
            // Start state
            InputAutomaton start = new InputAutomaton();
            InputAutomaton current = start;
            string c;

            for (int i = 0; i < s.Length; i++)
            {
                InputAutomaton next;

                // Get next input character (Hiragana, Alphabet, Number, Sign)
                c = s.Substring(i, 1);

                // Convert the input character to Katakana, Large alphabet, Hankaku.
                // In old version, As SJIS does not have Hiragana of "ヴ", we converted that to Katakana
                c = c.Hira2Kana();
                c = c.Zenkaku2Hankaku();

                next = new InputAutomaton();

                if (CreateAutomaton(current, c, next, InputCustomDefaultV1.Ope_NO_CUSTOM))
                {
                    current = next;
                }
            }

            return start;
        }

        /// <summary>
        /// Create new automaton
        /// </summary>
        /// <param name="current"></param>
        /// <param name="c"></param>
        /// <param name="next"></param>
        /// <param name="customAdd"></param>
        internal bool CreateAutomaton(InputAutomaton current, string c, InputAutomaton next, BitFlag customAdd)
        {
            // Previous automaton
            InputAutomaton prev = current;
            if (!_kanamap.ContainsKey(c))
            {
                // Input is not supported
                return false;
            }

            var kanamap = _kanamap[c];

            if (VersionHelper.WTVersion == 404)
            {
                // There is a bug in the version 405
                // To be compatible with that, we add ぱ/ぴ/ぺ with Shift Handakuten
                if ((_imAuthor == "Denasu System")
                    && (_imName == "JISかな")
                    && (c == "パ" || c == "ピ" || c == "ペ"))
                {
                    kanamap = new List<KeyStroke>();
                    foreach(var j in _kanamap[c])
                    {
                        kanamap.Add(j);

                        if (j.Stroke.Count() == 2
                            && ((j.Stroke[0] & (int)WTKeyCode.Shift) != (int)WTKeyCode.Shift)
                            && (j.Stroke[1] & (int)WTKeyCode.Shift) != (int)WTKeyCode.Shift)
                        {
                            kanamap.Add(new KeyStroke()
                            {
                                Custom = j.Custom,
                                Input = j.Input,
                                Stroke = new int[2]
                                {
                                    j.Stroke[0] | (int)WTKeyCode.Shift,
                                    j.Stroke[1] | (int)WTKeyCode.Shift,
                                }
                            });
                        }
                    }
                }
            }

            foreach (var j in kanamap)
            {
                if (j.Stroke.Count() == 0)
                {
                    continue;
                }

                current = prev;

                // Create new automaton for each stroke
                for (var k = 0; k < j.Stroke.Count(); k++)
                {
                    if(j.Stroke[k] == (int)WTKeyCode.NoKey)
                    {
                        continue;
                    }

                    if (k + 1 == j.Stroke.Count())
                    {
                        // The last alphabet
                        InputAutomaton.Connect connect = new InputAutomaton.Connect()
                        {
                            Automaton = next,
                            Flags = j.Custom.Or(customAdd),
                            Character = j.Input,
                        };
                        current.SetConnect(j.Stroke[k], connect);
                        current = next;
                    }
                    else
                    {
                        InputAutomaton.Connect connect = current.GetConnect(j.Stroke[k]);
                        if (connect == null)
                        {
                            // Create a automaton with Flags
                            connect = new InputAutomaton.Connect()
                            {
                                Automaton = new InputAutomaton(),
                                Flags = j.Custom.Or(customAdd),
                                Character = "",
                            };
                            current.SetConnect(j.Stroke[k], connect);
                            current = connect.Automaton;
                        }
                        else
                        {
                            // To make it deterministic, use existing automaton
                            connect.Flags = connect.Flags.Or(j.Custom);

                            current = connect.Automaton;
                        }
                    }
                }
            }

            return true;
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
                bool found = false;
                KeyValuePair<int, InputAutomaton.Connect> select = new KeyValuePair<int, InputAutomaton.Connect>();
                foreach (var i in automaton.GetConnect())
                {
                    // Use first connection because no need to consider customization
                    int c = i.Key;
                    bool valid = false;
                    InputAutomaton automaton_tmp = automaton.Input(c, ref valid, null);
                    if (current == automaton_tmp)
                    {
                        select = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    // Found no route, use the first route
                    select = automaton.GetConnect().First();
                }

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
                if (!_codeToString[plane].ContainsKey(virtualKey))
                {
                    continue;
                }

                result.Add(JapaneseHelper.Kana2Hira(_codeToString[plane][virtualKey]));
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
