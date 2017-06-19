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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// VersionHelper
/// </summary>
namespace com.denasu.WeatherTyping.plugin.helper
{
    /// <summary>
    /// VersionHelper
    /// </summary>
    public class VersionHelper
    {
        /// <summary>
        /// Weather Typing version
        /// In battle mode, this will be the lowst version of all players.
        /// In replay mode, this will be the WT version which the replay file was created.
        /// </summary>
        public static int WTVersion { set; get; }
    }
}
