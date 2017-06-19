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
    /// Roman custom
    /// </summary>
    internal class InputCustomRomanV1 : MarshalByRefObject, IInputCustom
    {
        /// <summary>
        /// Max flags
        /// </summary>
        private const int MaxCustomization = 19;

        /// <summary>
        /// Custom flag
        /// </summary>
        public static BitFlag Custom_NN = new BitFlag(MaxCustomization) { Bits = 1L << 0 };
        public static BitFlag Custom_SH = new BitFlag(MaxCustomization) { Bits = 1L << 1 };
        public static BitFlag Custom_CH = new BitFlag(MaxCustomization) { Bits = 1L << 2 };
        public static BitFlag Custom_TS = new BitFlag(MaxCustomization) { Bits = 1L << 3 };
        public static BitFlag Custom_FU = new BitFlag(MaxCustomization) { Bits = 1L << 4 };
        public static BitFlag Custom_JI = new BitFlag(MaxCustomization) { Bits = 1L << 5 };
        public static BitFlag Custom_X = new BitFlag(MaxCustomization) { Bits = 1L << 6 };
        public static BitFlag Custom_YIE = new BitFlag(MaxCustomization) { Bits = 1L << 7 };
        public static BitFlag Custom_VY = new BitFlag(MaxCustomization) { Bits = 1L << 8 };
        public static BitFlag Custom_QW = new BitFlag(MaxCustomization) { Bits = 1L << 9 };
        public static BitFlag Custom_KW = new BitFlag(MaxCustomization) { Bits = 1L << 10 };
        public static BitFlag Custom_ZY = new BitFlag(MaxCustomization) { Bits = 1L << 11 };
        public static BitFlag Custom_FY = new BitFlag(MaxCustomization) { Bits = 1L << 12 };
        public static BitFlag Custom_CY = new BitFlag(MaxCustomization) { Bits = 1L << 13 };
        public static BitFlag Custom_TY = new BitFlag(MaxCustomization) { Bits = 1L << 14 };
        public static BitFlag Custom_SMALL = new BitFlag(MaxCustomization) { Bits = 1L << 15 };
        public static BitFlag Custom_XN = new BitFlag(MaxCustomization) { Bits = 1L << 16 };
        public static BitFlag Custom_C = new BitFlag(MaxCustomization) { Bits = 1L << 17 };
        public static BitFlag Custom_JY = new BitFlag(MaxCustomization) { Bits = 1L << 18 };

        private const int MaxOperation = 38;

        /// <summary>
        /// Operation flag
        /// </summary>
        public static BitFlag Ope_NO_CUSTOM = new BitFlag(MaxOperation) { Bits = 0 };
        public static BitFlag Ope_NN = new BitFlag(MaxOperation) { Bits = 1L << 0 };
        public static BitFlag Ope_NO_NN = new BitFlag(MaxOperation) { Bits = 1L << 1 };
        public static BitFlag Ope_SH = new BitFlag(MaxOperation) { Bits = 1L << 2 };
        public static BitFlag Ope_NO_SH = new BitFlag(MaxOperation) { Bits = 1L << 3 };
        public static BitFlag Ope_CH = new BitFlag(MaxOperation) { Bits = 1L << 4 };
        public static BitFlag Ope_NO_CH = new BitFlag(MaxOperation) { Bits = 1L << 5 };
        public static BitFlag Ope_TS = new BitFlag(MaxOperation) { Bits = 1L << 6 };
        public static BitFlag Ope_NO_TS = new BitFlag(MaxOperation) { Bits = 1L << 7 };
        public static BitFlag Ope_FU = new BitFlag(MaxOperation) { Bits = 1L << 8 };
        public static BitFlag Ope_HU = new BitFlag(MaxOperation) { Bits = 1L << 9 };
        public static BitFlag Ope_JI = new BitFlag(MaxOperation) { Bits = 1L << 10 };
        public static BitFlag Ope_ZI = new BitFlag(MaxOperation) { Bits = 1L << 11 };
        public static BitFlag Ope_X = new BitFlag(MaxOperation) { Bits = 1L << 12 };
        public static BitFlag Ope_L = new BitFlag(MaxOperation) { Bits = 1L << 13 };
        public static BitFlag Ope_YIE = new BitFlag(MaxOperation) { Bits = 1L << 14 };
        public static BitFlag Ope_NO_YIE = new BitFlag(MaxOperation) { Bits = 1L << 15 };
        public static BitFlag Ope_VY = new BitFlag(MaxOperation) { Bits = 1L << 16 };
        public static BitFlag Ope_NO_VY = new BitFlag(MaxOperation) { Bits = 1L << 17 };
        public static BitFlag Ope_QW = new BitFlag(MaxOperation) { Bits = 1L << 18 };
        public static BitFlag Ope_NO_QW = new BitFlag(MaxOperation) { Bits = 1L << 19 };
        public static BitFlag Ope_KW = new BitFlag(MaxOperation) { Bits = 1L << 20 };
        public static BitFlag Ope_NO_KW = new BitFlag(MaxOperation) { Bits = 1L << 21 };
        public static BitFlag Ope_ZY = new BitFlag(MaxOperation) { Bits = 1L << 22 };
        public static BitFlag Ope_NO_ZY = new BitFlag(MaxOperation) { Bits = 1L << 23 };
        public static BitFlag Ope_FY = new BitFlag(MaxOperation) { Bits = 1L << 24 };
        public static BitFlag Ope_NO_FY = new BitFlag(MaxOperation) { Bits = 1L << 25 };
        public static BitFlag Ope_CY = new BitFlag(MaxOperation) { Bits = 1L << 26 };
        public static BitFlag Ope_NO_CY = new BitFlag(MaxOperation) { Bits = 1L << 27 };
        public static BitFlag Ope_TY = new BitFlag(MaxOperation) { Bits = 1L << 28 };
        public static BitFlag Ope_NO_TY = new BitFlag(MaxOperation) { Bits = 1L << 29 };
        public static BitFlag Ope_SMALL = new BitFlag(MaxOperation) { Bits = 1L << 30 };
        public static BitFlag Ope_NO_SMALL = new BitFlag(MaxOperation) { Bits = 1L << 31 };
        public static BitFlag Ope_XN = new BitFlag(MaxOperation) { Bits = 1L << 32 };
        public static BitFlag Ope_NO_XN = new BitFlag(MaxOperation) { Bits = 1L << 33 };
        public static BitFlag Ope_C = new BitFlag(MaxOperation) { Bits = 1L << 34 };
        public static BitFlag Ope_NO_C = new BitFlag(MaxOperation) { Bits = 1L << 35 };
        public static BitFlag Ope_JY = new BitFlag(MaxOperation) { Bits = 1L << 36 };
        public static BitFlag Ope_NO_JY = new BitFlag(MaxOperation) { Bits = 1L << 37 };

        /// <summary>
        /// Internal customization
        /// </summary>
        public BitFlag Customization { set; get; }

        /// <summary>
        /// Constractor
        /// </summary>
        public InputCustomRomanV1()
		{
            Customization = new BitFlag(MaxCustomization);
        }

        /// <summary>
        /// Set Input customization flags.
        /// Update Customization member depending on these flags
        /// </summary>
        /// <param name="custom">Input customization flags</param>
        public void SetFlags(BitFlag flags)
        {
            if(flags.Equals(Ope_NO_CUSTOM))
            {
                return;
            }
            if (flags.IsSet(Ope_NN))
            {
                Customization.Set(Custom_NN);
                Customization.Reset(Custom_XN);
            }
            if (flags.IsSet(Ope_NO_NN))
            {
                Customization.Reset(Custom_NN);
            }
            if (flags.IsSet(Ope_SH))
            {
                Customization.Set(Custom_SH);
            }
            if (flags.IsSet(Ope_NO_SH))
            {
                Customization.Reset(Custom_SH);
            }
            if (flags.IsSet(Ope_CH))
            {
                Customization.Set(Custom_CH);
            }
            if (flags.IsSet(Ope_NO_CH))
            {
                Customization.Reset(Custom_CH);
            }
            if (flags.IsSet(Ope_TY))
            {
                Customization.Set(Custom_TY);
            }
            if (flags.IsSet(Ope_NO_TY))
            {
                Customization.Reset(Custom_TY);
            }
            if (flags.IsSet(Ope_CY))
            {
                Customization.Set(Custom_CY);
            }
            if (flags.IsSet(Ope_NO_CY))
            {
                Customization.Reset(Custom_CY);
            }
            if (flags.IsSet(Ope_TS))
            {
                Customization.Set(Custom_TS);
            }
            if (flags.IsSet(Ope_NO_TS))
            {
                Customization.Reset(Custom_TS);
            }
            if (flags.IsSet(Ope_FU))
            {
                Customization.Set(Custom_FU);
            }
            if (flags.IsSet(Ope_HU))
            {
                Customization.Reset(Custom_FU);
            }
            if (flags.IsSet(Ope_ZI))
            {
                Customization.Reset(Custom_JI);
            }
            if (flags.IsSet(Ope_JI))
            {
                Customization.Set(Custom_JI);
            }
            if (flags.IsSet(Ope_L))
            {
                Customization.Reset(Custom_X);
            }
            if (flags.IsSet(Ope_X))
            {
                Customization.Set(Custom_X);
            }
            if (flags.IsSet(Ope_YIE))
            {
                Customization.Set(Custom_YIE);
            }
            if (flags.IsSet(Ope_NO_YIE))
            {
                Customization.Reset(Custom_YIE);
            }
            if (flags.IsSet(Ope_VY))
            {
                Customization.Set(Custom_VY);
            }
            if (flags.IsSet(Ope_NO_VY))
            {
                Customization.Reset(Custom_VY);
            }
            if (flags.IsSet(Ope_QW))
            {
                Customization.Set(Custom_QW);
            }
            if (flags.IsSet(Ope_NO_QW))
            {
                Customization.Reset(Custom_QW);
            }
            if (flags.IsSet(Ope_KW))
            {
                Customization.Set(Custom_KW);
            }
            if (flags.IsSet(Ope_NO_KW))
            {
                Customization.Reset(Custom_KW);
            }
            if (flags.IsSet(Ope_ZY))
            {
                Customization.Set(Custom_ZY);
            }
            if (flags.IsSet(Ope_NO_ZY))
            {
                Customization.Reset(Custom_ZY);
            }
            if (flags.IsSet(Ope_FY))
            {
                Customization.Set(Custom_FY);
            }
            if (flags.IsSet(Ope_NO_FY))
            {
                Customization.Reset(Custom_FY);
            }
            if (flags.IsSet(Ope_NO_SMALL))
            {
                Customization.Reset(Custom_SMALL);
            }
            if (flags.IsSet(Ope_SMALL))
            {
                Customization.Set(Custom_SMALL);
            }
            if (flags.IsSet(Ope_XN))
            {
                Customization.Set(Custom_XN);
                Customization.Reset(Custom_NN);
            }
            if (flags.IsSet(Ope_NO_XN))
            {
                Customization.Reset(Custom_XN);
            }
            if (flags.IsSet(Ope_C))
            {
                Customization.Set(Custom_C);
            }
            if (flags.IsSet(Ope_NO_C))
            {
                Customization.Reset(Custom_C);
            }
            if (flags.IsSet(Ope_JY))
            {
                Customization.Set(Custom_JY);
            }
            if (flags.IsSet(Ope_NO_JY))
            {
                Customization.Reset(Custom_JY);
            }
        }

        /// <summary>
        /// Combination of SMALL & NN
        /// </summary>
        private static BitFlag SmallNN = Ope_SMALL.Or(Ope_NO_SMALL).Or(Ope_NN).Or(Ope_NO_NN).Not();

        /// <summary>
        /// Is this flags configured?
        /// </summary>
        /// <param name="custom">Input customization flags</param>
        /// <returns>Is this flags configured?</returns>
        public bool IsSet(BitFlag flags)
        {
            bool result = false;
	        bool result_small = true;
	        bool result_nn = true;

            if(flags.Equals(Ope_NO_CUSTOM))
            {
                return true;
            }

            if (flags.IsSet(Ope_NO_SMALL))
	        {
		        if(!Customization.IsSet(Custom_SMALL))
		        {
			        result_small = true;
		        }
		        else
		        {
			        result_small = false;
		        }
	        }
            if (flags.IsSet(Ope_SMALL))
	        {
		        if(Customization.IsSet(Custom_SMALL))
		        {
			        result_small = true;
		        }
		        else
		        {
			        result_small = false;
		        }
	        }

            if (flags.IsSet(Ope_NN))
	        {
		        if(Customization.IsSet(Custom_NN))
		        {
			        result_nn = true;
		        }
		        else
		        {
			        result_nn = false;
		        }
	        }
            if (flags.IsSet(Ope_NO_NN))
	        {
		        if(!Customization.IsSet(Custom_NN))
		        {
			        result_nn = true;
		        }
		        else
		        {
			        result_nn = false;
		        }
	        }

            if (!flags.And(SmallNN).IsAny())
	        {
		        result = true;
	        }
            if (flags.IsSet(Ope_SH))
	        {
		        if(Customization.IsSet(Custom_SH))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_SH))
	        {
		        if(!Customization.IsSet(Custom_SH))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_CH))
	        {
		        if(Customization.IsSet(Custom_CH))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_CH))
	        {
		        if(!Customization.IsSet(Custom_CH))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_TS))
	        {
		        if(Customization.IsSet(Custom_TS))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_TS))
	        {
		        if(!Customization.IsSet(Custom_TS))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_FU))
	        {
		        if(Customization.IsSet(Custom_FU))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_HU))
	        {
		        if(!Customization.IsSet(Custom_FU))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_ZI))
	        {
		        if(!Customization.IsSet(Custom_JI))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_JI))
	        {
		        if(Customization.IsSet(Custom_JI))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_L))
	        {
		        if(!Customization.IsSet(Custom_X))
		        {
                    if (flags.IsSet(Ope_YIE))
			        {
				        if(Customization.IsSet(Custom_YIE))
				        {
					        result = true;
				        }
			        }
                    if (flags.IsSet(Ope_NO_YIE))
			        {
				        if(!Customization.IsSet(Custom_YIE))
				        {
					        result = true;
				        }
			        }
			        else
			        {
				        result = true;
			        }
		        }
	        }
            if (flags.IsSet(Ope_X))
	        {
		        if(Customization.IsSet(Custom_X))
		        {
                    if (flags.IsSet(Ope_YIE))
			        {
				        if(Customization.IsSet(Custom_YIE))
				        {
					        result = true;
				        }
			        }
                    if (flags.IsSet(Ope_NO_YIE))
			        {
				        if(!Customization.IsSet(Custom_YIE))
				        {
					        result = true;
				        }
			        }
			        else
			        {
				        result = true;
			        }
		        }
	        }
            if (flags.IsSet(Ope_VY))
	        {
		        if(Customization.IsSet(Custom_VY))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_VY))
	        {
		        if(!Customization.IsSet(Custom_VY))
		        {
			        result = true;
		        }
	        }
            if ((flags.IsSet(Ope_NO_QW))
                && (flags.IsSet(Ope_NO_KW)))
	        {
		        if(!Customization.IsSet(Custom_QW )&& !Customization.IsSet(Custom_KW))
		        {
			        result = true;
		        }
	        }
	        else
	        {
                if (flags.IsSet(Ope_QW))
		        {
			        if(Customization.IsSet(Custom_QW))
			        {
				        result = true;
			        }
		        }
                if (flags.IsSet(Ope_NO_QW))
		        {
			        if(!Customization.IsSet(Custom_QW))
			        {
				        result = true;
			        }
		        }
                if (flags.IsSet(Ope_KW))
		        {
			        if(Customization.IsSet(Custom_KW))
			        {
				        result = true;
			        }
		        }
                if (flags.IsSet(Ope_NO_KW))
		        {
			        if(!Customization.IsSet(Custom_KW))
			        {
				        result = true;
			        }
		        }
	        }
            if (flags.IsSet(Ope_ZY))
	        {
		        if(Customization.IsSet(Custom_ZY))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_ZY))
	        {
                if (flags.IsSet(Ope_JY))
		        {
			        if(Customization.IsSet(Custom_JY))
			        {
				        result = true;
			        }
			        else
			        {
				        result = false;
			        }
		        }
                if (flags.IsSet(Ope_NO_JY))
		        {
			        if(!Customization.IsSet(Custom_JY))
			        {
				        result = true;
			        }
			        else
			        {
				        result = false;
			        }
		        }
	        }
            if (flags.IsSet(Ope_FY))
	        {
		        if(Customization.IsSet(Custom_FY))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_FY))
	        {
		        if(!Customization.IsSet(Custom_FY))
		        {
			        result = true;
		        }
	        }
            if ((flags.IsSet(Ope_NO_TY))
                && (flags.IsSet(Ope_NO_CY)))
	        {
		        if(!Customization.IsSet(Custom_TY) && !Customization.IsSet(Custom_CY))
		        {
			        result = true;
		        }
	        }
	        else
	        {
                if (flags.IsSet(Ope_TY))
		        {
			        if(Customization.IsSet(Custom_TY))
			        {
				        result = true;
			        }
			        else
			        {
				        result = false;
			        }
		        }
                if (flags.IsSet(Ope_NO_TY))
		        {
			        if(!Customization.IsSet(Custom_TY))
			        {
				        result = true;
			        }
		        }
                if (flags.IsSet(Ope_CY))
		        {
			        if(Customization.IsSet(Custom_CY))
			        {
				        result = true;
			        }
			        else
			        {
				        result = false;
			        }
		        }
                if (flags.IsSet(Ope_NO_CY))
		        {
			        if(!Customization.IsSet(Custom_CY))
			        {
				        result = true;
			        }
		        }
	        }
            if (flags.IsSet(Ope_XN))
	        {
		        if(Customization.IsSet(Custom_XN))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_XN))
	        {
		        if(!Customization.IsSet(Custom_XN))
		        {
			        result = true;
		        }
	        }

            if (flags.IsSet(Ope_C))
	        {
		        if(Customization.IsSet(Custom_C))
		        {
			        result = true;
		        }
	        }
            if (flags.IsSet(Ope_NO_C))
	        {
		        if(!Customization.IsSet(Custom_C))
		        {
                    if (flags.IsSet(Ope_SH))
			        {
				        if(Customization.IsSet(Custom_SH))
				        {
					        result = true;
				        }
				        else
				        {
					        result = false;
				        }
			        }
                    else if (flags.IsSet(Ope_NO_SH))
			        {
				        if(!Customization.IsSet(Custom_SH))
				        {
					        result = true;
				        }
				        else
				        {
					        result = false;
				        }
			        }
			        else
			        {
				        result = true;
			        }
		        }
	        }

	        return result && result_small && result_nn;
        }
    }
}