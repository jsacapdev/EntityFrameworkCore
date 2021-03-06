﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.Converters;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public class BytesToStringConverterTest
    {
        private static readonly BytesToStringConverter _bytesToStringConverter
            = new BytesToStringConverter();

        [Fact]
        public void Cacluates_base64_size()
        {
            var sizeFunc = BytesToStringConverter.DefaultInfo.MappingHints.SizeFunction;

            var inputs = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            Assert.Equal(
                new[] { 4, 4, 4, 4, 8, 8, 8, 12, 12, 12, 16, 16, 16 },
                inputs.Select(i => sizeFunc(i)).ToArray());
        }

        [Fact]
        public void Can_convert_strings_to_bytes()
        {
            var converter = _bytesToStringConverter.ConvertToStoreExpression.Compile();

            Assert.Equal("U3DEsW7MiGFsIFRhcA==", converter(new byte[] { 83, 112, 196, 177, 110, 204, 136, 97, 108, 32, 84, 97, 112 }));
            Assert.Equal("", converter(new byte[0]));
            Assert.Null(converter(null));
        }

        [Fact]
        public void Can_convert_bytes_to_strings()
        {
            var converter = _bytesToStringConverter.ConvertFromStoreExpression.Compile();

            Assert.Equal(new byte[] { 83, 112, 196, 177, 110, 204, 136, 97, 108, 32, 84, 97, 112 }, converter("U3DEsW7MiGFsIFRhcA=="));
            Assert.Equal(new byte[0], converter(""));
            Assert.Null(converter(null));
        }

        [Fact]
        public void Can_convert_strings_to_long_non_char_bytes()
        {
            var converter = _bytesToStringConverter.ConvertToStoreExpression.Compile();

            Assert.Equal(CreateLongBytesString(), converter(CreateLongBytes()));
        }

        [Fact]
        public void Can_convert_long_non_char_bytes_to_strings()
        {
            var converter = _bytesToStringConverter.ConvertFromStoreExpression.Compile();

            Assert.Equal(CreateLongBytes(), converter(CreateLongBytesString()));
        }

        private static byte[] CreateLongBytes()
        {
            var longBinary = new byte[1000];
            for (var i = 0; i < longBinary.Length; i++)
            {
                longBinary[i] = (byte)i;
            }

            return longBinary;
        }

        private static string CreateLongBytesString()
        {
            var longBinary = new byte[1000];
            for (var i = 0; i < longBinary.Length; i++)
            {
                longBinary[i] = (byte)i;
            }

            return Convert.ToBase64String(longBinary);
        }
    }
}
