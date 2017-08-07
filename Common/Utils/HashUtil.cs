// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils
{
    public class HashUtil
    {
        public static long GetLongHashCode(string stringInput)
        {
            var byteContents = Encoding.Unicode.GetBytes(stringInput);
            var hash = new MD5CryptoServiceProvider();
            var hashText = hash.ComputeHash(byteContents);
            return BitConverter.ToInt64(hashText, 0) ^ BitConverter.ToInt64(hashText, 7);
        }

        public static int GetIntHashCode(string stringInput)
        {
            return (int) GetLongHashCode(stringInput);
        }
    }
}