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

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Key Code
    /// </summary>
    public enum WTKeyCode
    {
        NoKey = 0,
        NumPad0 = 1,
        NumPad1 = 2,
        NumPad2 = 3,
        NumPad3 = 4,
        NumPad4 = 5,
        NumPad5 = 6,
        NumPad6 = 7,
        NumPad7 = 8,
        NumPad8 = 9,
        NumPad9 = 10,
        NumPadMultiply = 11,
        NumPadAdd = 12,
        NumPadSubtract = 13,
        NumPadDivide = 14,
        NumPadPeriod = 15,
        NumPadEqual = 16,
        NumPadEnter = 17,

        Num0 = 100,
        Num1 = 101,
        Num2 = 102,
        Num3 = 103,
        Num4 = 104,
        Num5 = 105,
        Num6 = 106,
        Num7 = 107,
        Num8 = 108,
        Num9 = 109,

        A = 200,
        B = 201,
        C = 202,
        D = 203,
        E = 204,
        F = 205,
        G = 206,
        H = 207,
        I = 208,
        J = 209,
        K = 210,
        L = 211,
        M = 212,
        N = 213,
        O = 214,
        P = 215,
        Q = 216,
        R = 217,
        S = 218,
        T = 219,
        U = 220,
        V = 221,
        W = 222,
        X = 223,
        Y = 224,
        Z = 225,

        F1 = 300,
        F2 = 301,
        F3 = 302,
        F4 = 303,
        F5 = 304,
        F6 = 305,
        F7 = 306,
        F8 = 307,
        F9 = 308,
        F10 = 309,
        F11 = 310,
        F12 = 311,

        Backspace = 400,
        Tab = 401,
        Enter = 402,
        Pause = 403,
        CapsLock = 404,
        Kana = 405,
        Kanji = 406,
        Escape = 407,
        ImeConvert = 408,
        ImeNonConvert = 409,
        Space = 410,
        PageUp = 411,
        PageDown = 412,
        End = 413,
        Home = 414,
        Left = 415,
        Up = 416,
        Right = 417,
        Down = 418,
        PrintScreen = 419,
        Insert = 420,
        Delete = 421,
        Win = 422,
        NumLock = 423,
        ScrollLock = 424,
        App = 425,

        Minus = 500,
        Circumflex = 501,
        Yen = 502,
        At = 503,
        LeftBracket = 504,
        Semicolon = 505,
        Colon = 506,
        RightBracket = 507,
        Comma = 508,
        Period = 509,
        Slash = 510,
        BackSlash = 511,
        Equal = 512,           // 101 Keyboard
        Apostrophe = 513,      // 101 Keyboard
        Grave = 514,           // 101 Keyboard
        NonEng102 = 515,       // 102 Keyboard
        ABNT_C1 = 516,         // Brazilian Keyboard
        ABNT_C2 = 517,         // Brazilian Keyboard
        Ext_101a = 518,        // 101 Extend 1
        Ext_101b = 519,        // 101 Extend 2

        // Modifier bit flags
        Shift = 0x00000400,
        Ctrl = 0x00000800,
        LAlt = 0x00001000,
        Custom1 = 0x00002000,
        Custom2 = 0x00004000,
        Custom3 = 0x00008000,
        Custom4 = 0x00010000,
        RAlt = 0x00020000,
    }
}
