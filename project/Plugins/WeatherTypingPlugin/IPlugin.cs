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

namespace com.denasu.Common.Plugin
{
    /// <summary>
    /// All plugins must implement this interface
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Version
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The name of this plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The author of this plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Memo
        /// </summary>
        string Memo { get; }
    }
}
