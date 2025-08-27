using System;
using System.IO;
using System.Text;
#if HAVE_ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using Newtonsoft.Json.Utilities;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Tests.Utilities
{
    [TestFixture]
    public class Base64EncoderTests : TestFixtureBase
    {
        [Test]
        public void Constructor_NullWriter_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new Base64Encoder(null);
            });
        }

        [Test]
        public void Constructor_ValidWriter_CreatesInstance()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                Assert.IsNotNull(encoder);
            }
        }

        [Test]
        public void Encode_NullBuffer_ThrowsArgumentNullException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                ExceptionAssert.Throws<ArgumentNullException>(() =>
                {
                    encoder.Encode(null, 0, 0);
                });
            }
        }

        [Test]
        public void Encode_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[10];
                
                ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    encoder.Encode(buffer, -1, 5);
                });
            }
        }

        [Test]
        public void Encode_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[10];
                
                ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    encoder.Encode(buffer, 0, -1);
                });
            }
        }

        [Test]
        public void Encode_CountExceedsBuffer_ThrowsArgumentOutOfRangeException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[10];
                
                ExceptionAssert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    encoder.Encode(buffer, 5, 10);
                });
            }
        }

        [Test]
        public void Encode_EmptyBuffer_WritesNothing()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[0];
                
                encoder.Encode(buffer, 0, 0);
                encoder.Flush();
                
                Assert.AreEqual("", writer.ToString());
            }
        }

        [Test]
        public void Encode_SingleByte_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65 }; // 'A'
                
                encoder.Encode(buffer, 0, 1);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_TwoBytes_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65, 66 }; // 'AB'
                
                encoder.Encode(buffer, 0, 2);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_ThreeBytes_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65, 66, 67 }; // 'ABC'
                
                encoder.Encode(buffer, 0, 3);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_LargeBuffer_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[1000];
                
                // Fill with test data
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(i % 256);
                }
                
                encoder.Encode(buffer, 0, buffer.Length);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_MultipleChunks_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer1 = { 65, 66, 67 }; // 'ABC' - Use multiples of 3 for cleaner Base64
                byte[] buffer2 = { 68, 69, 70 }; // 'DEF'
                
                encoder.Encode(buffer1, 0, buffer1.Length);
                encoder.Encode(buffer2, 0, buffer2.Length);
                encoder.Flush();
                
                byte[] combined = new byte[6];
                Array.Copy(buffer1, 0, combined, 0, 3);
                Array.Copy(buffer2, 0, combined, 3, 3);
                
                string expected = Convert.ToBase64String(combined);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_WithOffset_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 1, 2, 3, 65, 66, 67, 7, 8, 9 }; // Use middle section
                
                encoder.Encode(buffer, 3, 3);
                encoder.Flush();
                
                byte[] expected = { 65, 66, 67 };
                string expectedBase64 = Convert.ToBase64String(expected);
                Assert.AreEqual(expectedBase64, writer.ToString());
            }
        }

        [Test]
        public void Flush_WithoutData_WritesNothing()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                encoder.Flush();
                
                Assert.AreEqual("", writer.ToString());
            }
        }

        [Test]
        public void Flush_WithPartialData_FlushesRemaining()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65 }; // Single byte that needs flushing
                
                encoder.Encode(buffer, 0, 1);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_LongerThanLineSize_HandlesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                // Create buffer longer than line size (57 bytes threshold)
                byte[] buffer = new byte[100];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(i % 256);
                }
                
                encoder.Encode(buffer, 0, buffer.Length);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

#if HAVE_ASYNC

        [Test]
        public async Task EncodeAsync_NullBuffer_ThrowsArgumentNullException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                await ExceptionAssert.ThrowsAsync<ArgumentNullException>(() =>
                {
                    return encoder.EncodeAsync(null, 0, 0, CancellationToken.None);
                });
            }
        }

        [Test]
        public async Task EncodeAsync_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[10];
                
                await ExceptionAssert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                {
                    return encoder.EncodeAsync(buffer, -1, 5, CancellationToken.None);
                });
            }
        }

        [Test]
        public async Task EncodeAsync_SingleByte_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65 }; // 'A'
                
                await encoder.EncodeAsync(buffer, 0, 1, CancellationToken.None);
                await encoder.FlushAsync(CancellationToken.None);
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public async Task EncodeAsync_MultipleChunks_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer1 = { 65, 66 }; // 'AB'
                byte[] buffer2 = { 67, 68 }; // 'CD'
                
                await encoder.EncodeAsync(buffer1, 0, buffer1.Length, CancellationToken.None);
                await encoder.EncodeAsync(buffer2, 0, buffer2.Length, CancellationToken.None);
                await encoder.FlushAsync(CancellationToken.None);
                
                byte[] combined = new byte[4];
                Array.Copy(buffer1, 0, combined, 0, 2);
                Array.Copy(buffer2, 0, combined, 2, 2);
                
                string expected = Convert.ToBase64String(combined);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public async Task FlushAsync_WithoutData_WritesNothing()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                await encoder.FlushAsync(CancellationToken.None);
                
                Assert.AreEqual("", writer.ToString());
            }
        }

        [Test]
        public async Task FlushAsync_WithCancellationToken_RespectsCancellation()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                var cts = new CancellationTokenSource();
                cts.Cancel();
                
                var task = encoder.FlushAsync(cts.Token);
                
                Assert.IsTrue(task.IsCanceled);
            }
        }

        [Test]
        public async Task EncodeAsync_LargeBuffer_EncodesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[1000];
                
                // Fill with test data
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(i % 256);
                }
                
                await encoder.EncodeAsync(buffer, 0, buffer.Length, CancellationToken.None);
                await encoder.FlushAsync(CancellationToken.None);
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

#endif

        [Test]
        public void Encode_EdgeCaseSequences_HandlesCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                
                // Test edge values
                byte[] buffer = { 0, 127, 255, 1, 254, 2 };
                
                encoder.Encode(buffer, 0, buffer.Length);
                encoder.Flush();
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_RepeatedFlush_NoErrors()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = { 65, 66 };
                
                encoder.Encode(buffer, 0, buffer.Length);
                encoder.Flush();
                encoder.Flush(); // Should not cause issues
                
                string expected = Convert.ToBase64String(buffer);
                Assert.AreEqual(expected, writer.ToString());
            }
        }

        [Test]
        public void Encode_ValidateParameterBoundaries_WorksCorrectly()
        {
            using (var writer = new StringWriter())
            {
                var encoder = new Base64Encoder(writer);
                byte[] buffer = new byte[10];
                
                // Edge case: index + count equals buffer length
                encoder.Encode(buffer, 0, buffer.Length);
                encoder.Encode(buffer, 5, 5);
                encoder.Encode(buffer, 9, 1);
                encoder.Flush();
                
                // Should complete without exceptions
                Assert.IsNotNull(writer.ToString());
            }
        }
    }
}