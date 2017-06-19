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
using com.denasu.WeatherTyping.plugin.input;

namespace com.denasu.WeatherTyping.plugin.helper
{
    /// <summary>
    /// Helper methods for key code
    /// </summary>
    public sealed class KeyCodeHelper
    {
        /// <summary>
        /// Convert ten keys to main keyboard
        /// </summary>
        public static Dictionary<int, int> TenkeyMap = new Dictionary<int, int>()
        {
            {(int)WTKeyCode.NumPad0, (int)WTKeyCode.Num0},
            {(int)WTKeyCode.NumPad1, (int)WTKeyCode.Num1},
            {(int)WTKeyCode.NumPad2, (int)WTKeyCode.Num2},
            {(int)WTKeyCode.NumPad3, (int)WTKeyCode.Num3},
            {(int)WTKeyCode.NumPad4, (int)WTKeyCode.Num4},
            {(int)WTKeyCode.NumPad5, (int)WTKeyCode.Num5},
            {(int)WTKeyCode.NumPad6, (int)WTKeyCode.Num6},
            {(int)WTKeyCode.NumPad7, (int)WTKeyCode.Num7},
            {(int)WTKeyCode.NumPad8, (int)WTKeyCode.Num8},
            {(int)WTKeyCode.NumPad9, (int)WTKeyCode.Num9},
            {(int)WTKeyCode.NumPadAdd, (int)WTKeyCode.Shift | (int)WTKeyCode.Semicolon},
            {(int)WTKeyCode.NumPadSubtract, (int)WTKeyCode.Minus},
            {(int)WTKeyCode.NumPadMultiply, (int)WTKeyCode.Shift | (int)WTKeyCode.Colon},
            {(int)WTKeyCode.NumPadDivide, (int)WTKeyCode.Slash},
            {(int)WTKeyCode.NumPadPeriod, (int)WTKeyCode.Period},
            {(int)WTKeyCode.NumPadEnter, (int)WTKeyCode.Enter},
        };

        /// <summary>
        /// Conbine a key and modifiers to int
        /// </summary>
        /// <param name="keys">a key and modifiers</param>
        /// <returns>int</returns>
        public static int CombineKeys(IEnumerable<WTKeyCode> keys)
        {
            // Sum all key codes
            return keys.Sum(x => (int)x);
        }

        static WTKeyCode[] _shiftKeys = new WTKeyCode[]
            {
                WTKeyCode.Custom4,
                WTKeyCode.Custom3,
                WTKeyCode.Custom2,
                WTKeyCode.Custom1,
                WTKeyCode.RAlt,
                WTKeyCode.LAlt,
                WTKeyCode.Ctrl,
                WTKeyCode.Shift,
            };

        /// <summary>
        /// Int split int to a key and modifiers
        /// </summary>
        /// <param name="code">int</param>
        /// <returns>a key and modifiers</returns>
        public static IEnumerable<WTKeyCode> SplitKeys(int code)
        {
            // Process modifiers
            List<WTKeyCode> keys = new List<WTKeyCode>();
            foreach (var modifier in _shiftKeys)
            {
                if (code >= (int)modifier)
                {
                    keys.Add(modifier);
                    code -= (int)modifier;
                }
            }

            // Process a key
            keys.Add((WTKeyCode)code);

            return keys;
        }
    }
}
