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
using System;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Key convert for simple key and shift+key
    /// See http://nicola.sunicom.co.jp/spec/kikaku.htm
    /// </summary>
    public class KeyFilterNICOLAV1 : MarshalByRefObject, IKeyFilter
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
                return "NICOLA";
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
                return "NICOLA key converter";
            }
        }

        /// <summary>
        /// Key input state
        /// </summary>
        public abstract class State
        {
            /// <summary>
            /// Constractor
            /// </summary>
            static State()
            {
                _isShift = false;
                _lsKeyCode = 0;
                _rsKeyCode = 0;
                _lsOrRs = 0;
                _timeCharKey = 0;
                _timeLsOrRsKey = 0;
                _keyCode = 0;
            }

            /// <summary>
            /// Key input hander for this state
            /// </summary>
            /// <param name="keyCode">Key code</param>
            /// <param name="press">Key is pressed or not</param>
            /// <param name="result">Output of this state</param>
            /// <param name="currentTime">Current time</param>
            /// <returns>Next state</returns>
            public abstract State Handle(int keyCode, bool press, List<int> result, long currentTime);

            /// <summary>
            /// Threshold of time for pressing keys at the same time (ms) 
            /// </summary>
            public const int _threshold = 100;

            /// <summary>
            /// Left Oyayubi shift key code
            /// </summary>
            public static int _lsKeyCode { set; get; }

            /// <summary>
            /// Right Oyayubi shift key code
            /// </summary>
            public static int _rsKeyCode { set; get; }

            /// <summary>
            /// Is shift pressed?
            /// </summary>
            public static bool _isShift { set; get; }

            /// <summary>
            /// Is Oyayubi shift pressed?
            /// </summary>
            public static int _lsOrRs { set; get; }

            /// <summary>
            /// Time when Char Key pressed
            /// </summary>
            public static long _timeCharKey { set; get; }

            /// <summary>
            /// Time when Oyayubi shift pressed
            /// </summary>
            public static long _timeLsOrRsKey { set; get; }

            /// <summary>
            /// Key code register
            /// </summary>
            public static int _keyCode { set; get; }
        }

        // Nicola Rule
        // Rule 1-1. Char ON -> Timeout
        // Rule 1-2. Char ON -> Char OFF -> Char
        // Rule 1-3. Char1 ON -> Char2 ON -> Char1
        // Rule 2-1. Oyayubi ON + Char ON -> Char with Oyayubi
        // Rule 2-2. Char ON + Oyayubi ON -> Char with Oyayubi
        // Rule 3-1. Char1 ON + Oyayubi ON + Char2 ON ->
        // Rule 3-1a. If (t(Oyayubi) - T(Char1)) <= (t(Char2) - T(Oyayubi)), Char1 with Oyayubi
        // Rule 3-1b. If (t(Oyayubi) - T(Char1)) > (t(Char2) - T(Oyayubi)), Char2 with Oyayubi

        /// <summary>
        /// Initial state
        /// </summary>
        private class StateNone : State
        {
            /// <summary>
            /// Get instance
            /// </summary>
            /// <returns>Instance</returns>
            public static State Instance()
            {
                if (_instance == null)
                {
                    _instance = new StateNone();
                }
                return _instance;
            }

            /// <summary>
            /// Instance
            /// </summary>
            private static StateNone _instance = null;

            /// <summary>
            /// Key input hander for this state
            /// </summary>
            /// <param name="keyCode">Key code</param>
            /// <param name="press">Key is pressed or not</param>
            /// <param name="result">Output of this state</param>
            /// <param name="currentTime">Current time</param>
            /// <returns>Next state</returns>
            public override State Handle(int keyCode, bool press, List<int> result, long currentTime)
            {
                if (press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key pressed. Go to Oyayubi Key state.
                    _lsOrRs = (keyCode == _lsKeyCode) ? _lsKeyCode : _rsKeyCode;
                    _timeLsOrRsKey = currentTime;
                    return StateLsOrRs.Instance();
                }
                else if (!press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key released. Can ignore
                    return Instance();
                }
                else if (press)
                {
                    // Char Key pressed. Go to Char state.
                    _keyCode = keyCode;
                    _timeCharKey = currentTime;
                    return StateChar.Instance();
                }
                else if (!press)
                {
                    // Char Key released. Can ignore.
                    return Instance();
                }

                // Unknown
                return null;
            }
        }

        /// <summary>
        /// Char Key state
        /// </summary>
        private class StateChar : State
        {
            /// <summary>
            /// Get instance
            /// </summary>
            /// <returns>Instance</returns>
            public static State Instance()
            {
                if (_instance == null)
                {
                    _instance = new StateChar();
                }
                return _instance;
            }

            /// <summary>
            /// Instance
            /// </summary>
            private static StateChar _instance = null;

            /// <summary>
            /// Key input hander for this state
            /// </summary>
            /// <param name="keyCode">Key code</param>
            /// <param name="press">Key is pressed or not</param>
            /// <param name="result">Output of this state</param>
            /// <param name="currentTime">Current time</param>
            /// <returns>Next state</returns>
            public override State Handle(int keyCode, bool press, List<int> result, long currentTime)
            {
                if (press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key pressed.
                    _timeLsOrRsKey = currentTime;
                    if (_timeLsOrRsKey - _timeCharKey > _threshold)
                    {
                        // Rule 1-1. Char ON -> Timeout

                        // Process Char Key
                        int oldOfs = keyCode;
                        keyCode = _keyCode;
                        result.Add(keyCode);

                        // Go to Oyayubi Key state
                        _lsOrRs = (oldOfs == _lsKeyCode) ? _lsKeyCode : _rsKeyCode;
                        _timeLsOrRsKey = currentTime;
                        return StateLsOrRs.Instance();
                    }

                    // Rule 2-2. Char ON + Oyayubi ON -> Char with Oyayubi
                    _lsOrRs = (keyCode == _lsKeyCode) ? _lsKeyCode : _rsKeyCode;
                    return StateLsOrRsWithChar.Instance();
                }
                else if (!press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key released. Can ignore
                    return StateChar.Instance();
                }
                else if (press)
                {
                    if (keyCode == _keyCode)
                    {
                        // The same Char Key pressed. Can ignore
                        return StateChar.Instance();
                    }

                    // Rule 1-3. Char1 ON -> Char2 ON -> Char1

                    // Process Char Key1
                    int tmp = keyCode;
                    keyCode = _keyCode;
                    result.Add(keyCode);

                    // Char Key2 pressed. Go to Char state.
                    _timeCharKey = currentTime;
                    _keyCode = tmp;
                    return StateChar.Instance();
                }
                else if (!press)
                {
                    if (keyCode != _keyCode)
                    {
                        // Unrelated Char released. Can ignore
                        return StateChar.Instance();
                    }

                    // Rule 1-2. Char ON -> Char OFF -> Char

                    // Process Char Key
                    keyCode = _keyCode;
                    result.Add(keyCode);

                    // Back to initial state
                    return StateNone.Instance();
                }

                // Unknown
                return null;
            }
        }

        /// <summary>
        /// Oyayubi Key state
        /// </summary>
        private class StateLsOrRs : State
        {
            /// <summary>
            /// Get instance
            /// </summary>
            /// <returns>Instance</returns>
            public static State Instance()
            {
                if (_instance == null)
                {
                    _instance = new StateLsOrRs();
                }
                return _instance;
            }

            /// <summary>
            /// Instance
            /// </summary>
            private static StateLsOrRs _instance = null;

            /// <summary>
            /// Key input hander for this state
            /// </summary>
            /// <param name="keyCode">Key code</param>
            /// <param name="press">Key is pressed or not</param>
            /// <param name="result">Output of this state</param>
            /// <param name="currentTime">Current time</param>
            /// <returns>Next state</returns>
            public override State Handle(int keyCode, bool press, List<int> result, long currentTime)
            {
                if (press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    if (keyCode == _lsOrRs)
                    {
                        // The same Oyayubi Key pressed. Can ignore
                        return StateLsOrRs.Instance();
                    }

                    // Opposite Oyayubi Key pressed
                    // No rule -> Update Oyayubi Key

                    int oldOfs = keyCode;
                    keyCode = _lsOrRs;

                    _lsOrRs = (oldOfs == _lsKeyCode) ? _lsKeyCode : _rsKeyCode;
                    _timeLsOrRsKey = currentTime;
                    return StateLsOrRs.Instance();
                }
                else if (!press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key released

                    // Back to initial state
                    keyCode = _lsOrRs;
                    return StateNone.Instance();
                }
                else if (press)
                {
                    // Char key pressed

                    _timeCharKey = currentTime;
                    _keyCode = keyCode;

                    if (_timeCharKey - _timeLsOrRsKey > _threshold)
                    {
                        // Rule 1-1'. Oyayubi ON -> Timeout

                        keyCode = _lsOrRs;

                        // Go to Char Key state
                        return StateChar.Instance();
                    }

                    // Rule 2-1. Oyayubi ON + Char ON -> Char with Oyayubi
                    return StateLsOrRsWithChar.Instance();
                }
                else if (!press)
                {
                    // Char Key released. Can ignore
                    return StateLsOrRs.Instance();
                }

                // Unknown
                return null;
            }
        }

        /// <summary>
        /// Oyayubi Key with Char Key state
        /// </summary>
        private class StateLsOrRsWithChar : State
        {
            /// <summary>
            /// Get instance
            /// </summary>
            /// <returns>Instance</returns>
            public static State Instance()
            {
                if (_instance == null)
                {
                    _instance = new StateLsOrRsWithChar();
                }
                return _instance;
            }

            /// <summary>
            /// Instance
            /// </summary>
            private static StateLsOrRsWithChar _instance = null;

            /// <summary>
            /// Key input hander for this state
            /// </summary>
            /// <param name="keyCode">Key code</param>
            /// <param name="press">Key is pressed or not</param>
            /// <param name="result">Output of this state</param>
            /// <param name="currentTime">Current time</param>
            /// <returns>Next state</returns>
            public override State Handle(int keyCode, bool press, List<int> result, long currentTime)
            {
                if (press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    if (keyCode == _lsOrRs)
                    {
                        // The same Oyayubi Key pressed. Can ignore
                        return StateLsOrRsWithChar.Instance();
                    }

                    // Opposite Oyayubi Key pressed
                    // (Char Key with Oyayubi Key1) + (Oyayubi Key2)

                    // Either of:
                    // Rule 2-1. Oyayubi ON + Charl ON -> Char with Oyayubi
                    // Rule 2-2. Char ON + Oyayubi ON -> Char with Oyayubi

                    // Set Oyayubi Key flag to Char Key
                    int oldOfs = keyCode;
                    keyCode = _keyCode;
                    keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                    result.Add(keyCode);

                    // Go to Oyayubi Key2 state
                    _lsOrRs = (oldOfs == _lsKeyCode) ? _lsKeyCode : _rsKeyCode;
                    _timeLsOrRsKey = currentTime;
                    return StateLsOrRs.Instance();
                }
                else if (!press && (keyCode == _rsKeyCode || keyCode == _lsKeyCode))
                {
                    // Oyayubi Key released

                    // Either of:
                    // Rule 2-1. Oyayubi ON + Charl ON -> Char with Oyayubi
                    // Rule 2-2. Char ON + Oyayubi ON -> Char with Oyayubi

                    // Set Oyayubi Key flag to Char Key
                    keyCode = _keyCode;
                    keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                    result.Add(keyCode);

                    // Back to initial state
                    return StateNone.Instance();
                }
                else if (press)
                {
                    // Char Key pressed

                    if (_timeLsOrRsKey < _timeCharKey)
                    {
                        // (Oyayubi Key with Char Key1) + (Char Key2)

                        // Rule 2-1. Oyayubi ON + Charl ON -> Char with Oyayubi
                        // Set Oyayubi Key flag to Char Key1
                        int tmp = keyCode;
                        keyCode = _keyCode;
                        keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                        result.Add(keyCode);

                        _timeCharKey = currentTime;
                        _keyCode = tmp;

                        // Go to Char Key2 state
                        return StateChar.Instance();
                    }
                    else
                    {
                        long time = currentTime;
                        if (time - _timeLsOrRsKey > _threshold)
                        {
                            // Rule 1-1'. Oyayubi ON -> Timeout (with Char Key2)

                            // Set Oyayubi Key flag to Char Key1
                            int tmp = keyCode;
                            keyCode = _keyCode;
                            keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                            result.Add(keyCode);

                            _timeCharKey = currentTime;
                            _keyCode = tmp;

                            // Go to Char Key2 state
                            return StateChar.Instance();
                        }

                        // Rule 3-1. Char1 ON + Oyayubi ON + Char2 ON
                        // We have to determine which Char Key the Oyayubi Key is attached to.

                        if (_timeLsOrRsKey - _timeCharKey <= time - _timeLsOrRsKey)
                        {
                            // Rule 3-1a. If (t(Oyayubi) - T(Char1)) <= (t(Char2) - T(Oyayubi)), Char1 with Oyayubi

                            // Set Oyayubi Key flag to Char Key1
                            int tmp = keyCode;
                            keyCode = _keyCode;
                            keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                            result.Add(keyCode);

                            _timeCharKey = currentTime;
                            _keyCode = tmp;

                            // Go to Char Key2 state
                            return StateChar.Instance();
                        }
                        else
                        {
                            // Rule 3-1b. If (t(Oyayubi) - T(Char1)) > (t(Char2) - T(Oyayubi)), Char2 with Oyayubi

                            // Do not set Oyayubi Key flag to Char Key1
                            int tmp = keyCode;
                            keyCode = _keyCode;
                            result.Add(keyCode);

                            // Go to Char Key2 with Oyayubi Key state
                            _timeCharKey = time;
                            _keyCode = tmp;
                            return StateLsOrRsWithChar.Instance();
                        }
                    }
                }
                else if (!press)
                {
                    // Char Key released

                    if (keyCode != _keyCode)
                    {
                        // Unrelated Char released. Can ignore
                        return StateLsOrRsWithChar.Instance();
                    }

                    // Either of:
                    // Rule 2-1. Oyayubi ON + Charl ON -> Char with Oyayubi
                    // Rule 2-2. Char ON + Oyayubi ON -> Char with Oyayubi

                    // Set Oyayubi Key flag to Char Key
                    keyCode = _keyCode;
                    keyCode |= (int)(_lsOrRs == _lsKeyCode ? WTKeyCode.Custom1 : WTKeyCode.Custom2);
                    result.Add(keyCode);

                    // Back to initial state
                    return StateNone.Instance();
                }

                // Unknown
                return null;
            }
        };

        /// <summary>
        /// NICOLA status
        /// </summary>
        State _state = StateNone.Instance();

        /// <summary>
        /// Create KeyCustom instance
        /// The plugin author declares the key which can be assigned by users
        /// WT assins physical keys for the declared keys by special UI.
        /// </summary>
        /// <returns>KeyCustom instance</returns>
        public IKeyCustom CreateKeyCustom()
        {
            return new KeyCustomNICOLAV1();
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

        public const string LSNAME = "左親指シフトキー";
        public const string RSNAME = "右親指シフトキー";

        /// <summary>
        /// Returns key map names
        /// </summary>
        public List<string> GetNames()
        {
            return new List<string>()
            {
                LSNAME, // Left top of key label
                RSNAME, // Right top of key label
            };
        }

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

            State._lsKeyCode = (int)keyCustom.GetMappedKey(LSNAME);
            State._rsKeyCode = (int)keyCustom.GetMappedKey(RSNAME);

            if (pressed && (physicalKey == (int)WTKeyCode.Shift))
            {
                // Normal shift key pressed
                State._isShift = true;
            }
            else if (!pressed && (physicalKey == (int)WTKeyCode.Shift))
            {
                // Normal shift key released
                State._isShift = false;
            }
            else if (pressed)
            {
                if (State._isShift)
                {
                    // Set shift flag
                    physicalKey |= (int)WTKeyCode.Shift;
                    result.Add(physicalKey);
                    return result;
                }

                if (physicalKey == (int)WTKeyCode.Enter || physicalKey == (int)WTKeyCode.Escape)
                {
                    // Special key
                    result.Add(physicalKey);
                    return result;
                }

                // Process press event
                _state = _state.Handle(physicalKey, pressed, result, currentTime);
            }
            else if (!pressed)
            {
                // Process release event
                _state = _state.Handle(physicalKey, pressed, result, currentTime);
            }

            return result;
        }
    }
}
