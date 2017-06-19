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

using System.Linq;
using System.Text.RegularExpressions;

namespace com.denasu.WeatherTyping.plugin.helper
{
    /// <summary>
    /// Helper methods for Japanese
    /// </summary>
    public static class JapaneseHelper
    {
        /// <summary>
        /// Hiragana to Kana
        /// </summary>
        /// <param name="s">Hiragana</param>
        /// <returns>Kana</returns>
        public static string Hira2Kana(this string s)
        {
            return new string(s.ToCharArray().Select(c => (c >= 'ぁ' && c <= 'ゖ') ? (char)(c + 'ァ' - 'ぁ') : c).ToArray());
        }

        /// <summary>
        /// Kana to Hiragana
        /// </summary>
        /// <param name="s">Kana</param>
        /// <returns>Hiragana</returns>
        public static string Kana2Hira(this string s)
        {
            return new string(s.ToCharArray().Select(c => (c >= 'ァ' && c <= 'ヶ') ? (char)(c + 'ぁ' - 'ァ') : c).ToArray());
        }

        private static Regex _zentaku = new Regex(@"[０-９Ａ-Ｚａ-ｚ]", RegexOptions.None);
        private static Regex _hankaku = new Regex(@"[0-9A-Za-z]", RegexOptions.None);

        /// <summary>
        /// Zenkaku to Hankaku
        /// </summary>
        /// <param name="s">Zenkaku</param>
        /// <returns>Hankaku</returns>
        public static string Zenkaku2Hankaku(this string s)
        {
            return _zentaku.Replace(s, c => ((char)((int)(c.Value.ToCharArray())[0] - 65248)).ToString());
        }

        /// <summary>
        /// Hankaku to Zenkaku
        /// </summary>
        /// <param name="s">Hankaku</param>
        /// <returns>Zenkaku</returns>
        public static string Hankaku2Zenkaku(this string s)
        {
            return _hankaku.Replace(s, c => ((char)((int)(c.Value.ToCharArray())[0] + 65248)).ToString());
        }

        /// <summary>
        /// Small alphabet to large alphabet
        /// </summary>
        /// <param name="s">Small alphabet</param>
        /// <returns>Large alphabet</returns>
        public static string Small2Large(this string s)
        {
            return new string(s.ToCharArray().Select(c => (c >= 'ａ' && c <= 'ｚ') ? (char)(c + 'a' - 'ａ') : c).ToArray());
        }

        /// <summary>
        /// Large alphabet to small alphabet
        /// </summary>
        /// <param name="s">Large alphabet</param>
        /// <returns>Small alphabet</returns>
        public static string Large2Small(this string s)
        {
            return new string(s.ToCharArray().Select(c => (c >= 'a' && c <= 'z') ? (char)(c + 'ａ' - 'a') : c).ToArray());
        }
    }
}
