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

using com.denasu.WeatherTyping.plugin.helper;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Key stroke (Single key or Modified single key)
    /// </summary>
    public sealed class KeyStroke
    {
        /// <summary>
        /// Key stroke for this character
        ///   e.g. か->KeyCode.K + KeyCode.A -> KA
        /// </summary>
        public int[] Stroke { set; get; }

        /// <summary>
        /// Input string for this character
        ///   e.g. か->KA
        /// </summary>
        public string Input { set; get; }

        /// <summary>
        /// Custom bit flag
        ///   e.g. か->USE_KA, not USE_CA
        /// </summary>
        public BitFlag Custom { set; get; }
    }
}
