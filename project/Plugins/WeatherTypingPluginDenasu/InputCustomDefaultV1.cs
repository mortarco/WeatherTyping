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
using System;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Default custom
    /// </summary>
    internal class InputCustomDefaultV1 : MarshalByRefObject, IInputCustom
    {
        /// <summary>
        /// Internal customization
        /// </summary>
        public BitFlag Customization { set; get; }

        /// <summary>
        /// No custom
        /// </summary>
        public static BitFlag Ope_NO_CUSTOM = new BitFlag(1) { Bits = 0 };

        /// <summary>
        /// Constractor
        /// </summary>
        public InputCustomDefaultV1()
		{
            Customization = new BitFlag(0);
        }

        /// <summary>
        /// Set Input customization flags.
        /// Update Customization member depending on these flags
        /// </summary>
        /// <param name="custom">Input customization flags</param>
        public void SetFlags(BitFlag flags)
        {
        }

        /// <summary>
        /// Is this flags configured?
        /// </summary>
        /// <param name="custom">Input customization flags</param>
        /// <returns>Is this flags configured?</returns>
        public bool IsSet(BitFlag flags)
        {
            return true;
        }
    }
}