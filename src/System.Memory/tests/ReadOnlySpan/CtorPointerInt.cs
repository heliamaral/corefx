// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;

#pragma warning disable 0649 //Field 'SpanTests.InnerStruct.J' is never assigned to, and will always have its default value 0

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void CtorPointerInt()
        {
            unsafe
            {
                int[] a = { 90, 91, 92 };
                fixed (int* pa = a)
                {
                    ReadOnlySpan<int> span = new ReadOnlySpan<int>(pa, 3);
                    span.Validate(90, 91, 92);
                    Assert.True(Unsafe.AreSame(ref Unsafe.AsRef<int>(pa), ref span.DangerousGetPinnableReference()));
                }
            }
        }

        [Fact]
        public static void CtorPointerNull()
        {
            unsafe
            {
                ReadOnlySpan<int> span = new ReadOnlySpan<int>((void*)null, 0);
                span.Validate();
                Assert.True(Unsafe.AreSame(ref Unsafe.AsRef<int>((void*)null), ref span.DangerousGetPinnableReference()));
            }
        }

        [Fact]
        public static void CtorPointerRangeChecks()
        {
            unsafe
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    delegate ()
                    {
                        int i = 42;
                        ReadOnlySpan<int> span = new ReadOnlySpan<int>(&i, -1);
                    });
            }
        }

        [Fact]
        public static void CtorPointerNoContainsReferenceEnforcement()
        {
            unsafe
            {
                new ReadOnlySpan<int>((void*)null, 0);
                new ReadOnlySpan<int?>((void*)null, 0);
                AssertExtensions.Throws<ArgumentException>(null, () => new ReadOnlySpan<object>((void*)null, 0).DontBox());
                AssertExtensions.Throws<ArgumentException>(null, () => new ReadOnlySpan<StructWithReferences>((void*)null, 0).DontBox());
            }
        }

        private struct StructWithReferences
        {
            public int I;
            public InnerStruct Inner;
        }

        private struct InnerStruct
        {
            public int J;
            public object O;
        }
    }
}
