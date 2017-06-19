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

using com.denasu.WeatherTyping.plugin.input;
using System;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;

namespace com.denasu.WeatherTyping.plugin
{
    /// <summary>
    /// Plugin Utility
    /// </summary>
    public class PluginUtil : MarshalByRefObject
    {
        /// <summary>
        /// Set expiration time to infinite
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        public void Initialize()
        {
            new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration).Assert();
            LifetimeServices.LeaseTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Dump InputAutomaton in Plugin domain
        /// </summary>
        public string DumpAutomaton()
        {
            return InputAutomaton.Dump();
        }
    }
}
