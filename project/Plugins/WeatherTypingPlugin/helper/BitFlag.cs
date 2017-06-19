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

 using System;

namespace com.denasu.WeatherTyping.plugin.helper
{
    /// <summary>
    /// Bit flag helper class
    /// </summary>
    [Serializable]
    public sealed class BitFlag
    {
        private int _maxbit = 0;

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="max">Max flags</param>
        public BitFlag(int maxbit)
        {
            _maxbit = maxbit;
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="max">Max flags</param>
        public BitFlag(BitFlag other)
        {
            _maxbit = other._maxbit;
            Bits = other.Bits;
        }

        /// <summary>
        /// Count flags
        /// </summary>
        /// <returns>Count</returns>
        public int Count()
        {
            return _maxbit;
        }

        /// <summary>
        /// Count flags
        /// </summary>
        /// <returns>Count</returns>
        public bool Equals(BitFlag other)
        {
            return Bits == other.Bits;
        }

        /// <summary>
        /// Bits
        /// </summary>
        public long Bits { set; get; }

        /// <summary>
        /// Returns the other flag is set?
        /// </summary>
        /// <param name="other">Other flag</param>
        /// <returns>The other flag is set?</returns>
        public bool IsSet(BitFlag other)
        {
            return (Bits & other.Bits) != 0;
        }

        /// <summary>
        /// Returns any flag is set?
        /// </summary>
        /// <param name="other">Other flag</param>
        /// <returns>Any flag is set?</returns>
        public bool IsAny()
        {
            return (Bits != 0);
        }

        /// <summary>
        /// Set flag
        /// </summary>
        /// <param name="other">Other flag</param>
        public void Set(BitFlag other)
        {
            Bits |= other.Bits;
        }

        /// <summary>
        /// Reset flag
        /// </summary>
        /// <param name="other">Other flag</param>
        public void Reset(BitFlag other)
        {
            Bits &= ~other.Bits;
        }

        /// <summary>
        /// And
        /// </summary>
        /// <param name="other">Other flag</param>
        /// <returns></returns>
        public BitFlag And(BitFlag other)
        {
            return new BitFlag(Count()) { Bits = Bits & other.Bits };
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="other">Other flag</param>
        /// <returns></returns>
        public BitFlag Or(BitFlag other)
        {
            return new BitFlag(Count()) { Bits = Bits | other.Bits };
        }

        /// <summary>
        /// Not
        /// </summary>
        /// <param name="other">Other flag</param>
        /// <returns></returns>
        public BitFlag Not()
        {
            return new BitFlag(Count()) { Bits = (~Bits) & ((1L << Count()) - 1) };
        }
    }
}
