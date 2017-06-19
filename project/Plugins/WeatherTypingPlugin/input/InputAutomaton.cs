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
using System.Collections.Generic;
using System.Text;

namespace com.denasu.WeatherTyping.plugin.input
{
    /// <summary>
    /// Input automaton
    /// </summary>
    [Serializable]
    public sealed class InputAutomaton
	{
        /// <summary>
        /// Connection
        /// </summary>
        [Serializable]
        public class Connect
        {
			/// <summary>
			/// The automaton which this connection is connected to
			/// </summary>
            public InputAutomaton Automaton { set; get; }

			/// <summary>
			/// Input customization flags
            /// Use this to change display string, e.g. "si" or "shi"
			/// </summary>
            public BitFlag Flags { set; get; }

			/// <summary>
			/// Display character of this connection
			/// </summary>
            public string Character { set; get; }
        }

        /// <summary>
        /// Connection
        /// </summary>
        private Dictionary<int, Connect> _connect = new Dictionary<int, Connect>();

        /// <summary>
        /// Instance ID base for debug
        /// </summary>
        private static int _idbase = 0;

        /// <summary>
        /// Instance ID for debug
        /// </summary>
        public int ID { set; get; }

        /// <summary>
        /// Instances for debug
        /// </summary>
        public static Dictionary<int, InputAutomaton> _instances = new Dictionary<int, InputAutomaton>();

		/// <summary>
		/// Constractor
		/// </summary>
		public InputAutomaton()
		{
            _idbase++;
            ID = _idbase;
            _instances.Add(_idbase, this);
		}

        /// <summary>
        /// Set connection
        /// </summary>
        /// <param name="c">Key code</param>
        /// <param name="newconnection">Connection</param>
		public void SetConnect(int c, Connect newconnection)
		{
            if(_connect.ContainsKey(c))
            {
                throw new Exception("Key " + string.Join(",", KeyCodeHelper.SplitKeys(c)) + " is duplicated.");
            }
			_connect.Add(c, newconnection);
        }

        /// <summary>
        /// Get all the connections
        /// </summary>
        /// <returns>Connections</returns>
		public Dictionary<int, Connect> GetConnect()
		{
			return _connect;
		}

        /// <summary>
        /// Get a connection
        /// </summary>
        /// <param name="c">Key code</param>
        /// <returns></returns>
		public Connect GetConnect(int c)
		{
            return _connect.ContainsKey(c) ? _connect[c] : null;
		}

        /// <summary>
        /// Input to automaton
        /// </summary>
        /// <param name="c">Key code</param>
        /// <param name="valid">Returns the character is valid</param>
        /// <param name="custom">Input custom</param>
        /// <returns>Next automaton</returns>
        public InputAutomaton Input(int c, ref bool valid, IInputCustom custom)
		{
			if (!_connect.ContainsKey(c))
			{
				// reject
				valid = false;
				return this;
			}
			else
			{
				valid = true;
                if (custom != null)
                {
                    custom.SetFlags(_connect[c].Flags);
                }
				return _connect[c].Automaton;
			}
		}

		/// <summary>
		/// Is this automaton the accepted state?
		/// </summary>
		/// <returns></returns>
		public bool IsAccept()
		{
			return _connect.Count == 0;
		}

        /// <summary>
        /// Return string representation
        /// </summary>
        /// <param name="automaton"></param>
        /// <returns></returns>
        public static string Dump()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var instance in _instances)
            {
                builder.Append("{ ");
                builder.Append(instance.Key);
                builder.Append(" : {");
                foreach (var connect in instance.Value.GetConnect())
                {
                    builder.Append(" { ");
                    builder.AppendFormat(" ch : {0},", string.Join(",", KeyCodeHelper.SplitKeys(connect.Key)));
                    builder.AppendFormat(" to : {0},", connect.Value.Automaton.ID);
                    builder.AppendFormat(" st : {0} ", connect.Value.Character);
                    builder.Append("} ");
                }
                builder.Append("} ");
                builder.Append("}");
                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Clear debug information
        /// </summary>
        /// <param name="automaton"></param>
        /// <returns></returns>
        public static void Clear()
        {
            _instances.Clear();
            _idbase = 0;
        }
    }
}