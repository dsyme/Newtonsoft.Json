using System;
using System.Xml;
using Newtonsoft.Json.Converters;
#if !NET20
using System.Xml.Linq;
#endif
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Tests.Converters
{
    [TestFixture]
    public class XCommentWrapperTests : TestFixtureBase
    {
#if !NET20
        [Test]
        public void Constructor_ValidXComment_CreatesInstance()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper);
            Assert.AreEqual(comment, wrapper.WrappedNode);
        }

        [Test]
        public void Constructor_NullXComment_CreatesInstance()
        {
            // XCommentWrapper constructor accepts null (inherits from XObjectWrapper)
            XCommentWrapper wrapper = new XCommentWrapper(null);
            
            Assert.IsNotNull(wrapper);
            Assert.IsNull(wrapper.WrappedNode);
            Assert.AreEqual(XmlNodeType.None, wrapper.NodeType);
        }

        [Test]
        public void NodeType_ReturnsComment()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(XmlNodeType.Comment, wrapper.NodeType);
        }

        [Test]
        public void Value_Get_ReturnsCommentValue()
        {
            string commentText = "This is a test comment";
            XComment comment = new XComment(commentText);
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(commentText, wrapper.Value);
        }

        [Test]
        public void Value_Set_UpdatesCommentValue()
        {
            XComment comment = new XComment("Original");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            string newValue = "Updated comment";
            
            wrapper.Value = newValue;
            
            Assert.AreEqual(newValue, wrapper.Value);
            Assert.AreEqual(newValue, comment.Value);
        }

        [Test]
        public void Value_SetNull_SetsToEmptyString()
        {
            XComment comment = new XComment("Original");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            wrapper.Value = null;
            
            Assert.AreEqual(string.Empty, wrapper.Value);
            Assert.AreEqual(string.Empty, comment.Value);
        }

        [Test]
        public void Value_SetEmptyString_SetsToEmptyString()
        {
            XComment comment = new XComment("Original");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            wrapper.Value = string.Empty;
            
            Assert.AreEqual(string.Empty, wrapper.Value);
            Assert.AreEqual(string.Empty, comment.Value);
        }

        [Test]
        public void ParentNode_WhenCommentHasParent_ReturnsWrappedParent()
        {
            XElement parent = new XElement("root");
            XComment comment = new XComment("Test comment");
            parent.Add(comment);
            
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper.ParentNode);
            Assert.AreEqual(parent, wrapper.ParentNode.WrappedNode);
        }

        [Test]
        public void ParentNode_WhenCommentHasNoParent_ReturnsNull()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNull(wrapper.ParentNode);
        }

        [Test]
        public void ParentNode_WhenCommentAddedToDocument_ReturnsWrappedDocument()
        {
            XDocument document = new XDocument();
            XComment comment = new XComment("Test comment");
            document.Add(comment);
            
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            // After adding to document, verify the parent relationship
            if (comment.Parent != null)
            {
                Assert.IsNotNull(wrapper.ParentNode);
                Assert.AreEqual(comment.Parent, wrapper.ParentNode.WrappedNode);
            }
            else
            {
                // If comment.Parent is null, wrapper.ParentNode should also be null
                Assert.IsNull(wrapper.ParentNode);
            }
        }

        [Test]
        public void LocalName_ReturnsNull()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNull(wrapper.LocalName);
        }

        [Test]
        public void NamespaceUri_ReturnsNull()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNull(wrapper.NamespaceUri);
        }

        [Test]
        public void ChildNodes_ReturnsEmptyList()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper.ChildNodes);
            Assert.AreEqual(0, wrapper.ChildNodes.Count);
        }

        [Test]
        public void Attributes_ReturnsEmptyList()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper.Attributes);
            Assert.AreEqual(0, wrapper.Attributes.Count);
        }

        [Test]
        public void AppendChild_ThrowsInvalidOperationException()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            XComment childComment = new XComment("Child");
            XCommentWrapper childWrapper = new XCommentWrapper(childComment);
            
            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                wrapper.AppendChild(childWrapper);
            });
        }

        [Test]
        public void WrappedNode_ReturnsOriginalXComment()
        {
            XComment comment = new XComment("Test comment");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(comment, wrapper.WrappedNode);
        }

        [Test]
        public void Value_WithSpecialCharacters_HandledCorrectly()
        {
            string specialText = "Comment with <special> & \"quoted\" text";
            XComment comment = new XComment(specialText);
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(specialText, wrapper.Value);
            
            string newSpecialText = "Updated with & more <special> characters";
            wrapper.Value = newSpecialText;
            
            Assert.AreEqual(newSpecialText, wrapper.Value);
        }

        [Test]
        public void Value_WithUnicodeCharacters_HandledCorrectly()
        {
            string unicodeText = "Unicode: 你好 🌟 Ñoël";
            XComment comment = new XComment(unicodeText);
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(unicodeText, wrapper.Value);
            
            wrapper.Value = "更新的文本";
            Assert.AreEqual("更新的文本", wrapper.Value);
        }

        [Test]
        public void Value_WithLongText_HandledCorrectly()
        {
            string longText = new string('A', 10000);
            XComment comment = new XComment(longText);
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.AreEqual(longText, wrapper.Value);
            
            string newLongText = new string('B', 5000);
            wrapper.Value = newLongText;
            
            Assert.AreEqual(newLongText, wrapper.Value);
        }

        [Test]
        public void ParentNode_AfterRemovingFromParent_ReturnsNull()
        {
            XElement parent = new XElement("root");
            XComment comment = new XComment("Test comment");
            parent.Add(comment);
            
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            Assert.IsNotNull(wrapper.ParentNode);
            
            comment.Remove();
            
            Assert.IsNull(wrapper.ParentNode);
        }

        [Test]
        public void ParentNode_WithNestedElements_ReturnsImmediateParent()
        {
            XElement grandparent = new XElement("grandparent");
            XElement parent = new XElement("parent");
            XComment comment = new XComment("Test comment");
            
            grandparent.Add(parent);
            parent.Add(comment);
            
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper.ParentNode);
            Assert.AreEqual(parent, wrapper.ParentNode.WrappedNode);
            Assert.AreNotEqual(grandparent, wrapper.ParentNode.WrappedNode);
        }

        [Test]
        public void Value_MultipleUpdates_MaintainsConsistency()
        {
            XComment comment = new XComment("Initial");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            string[] values = { "First", "Second", "", "Third", null, "Fourth" };
            string[] expectedValues = { "First", "Second", "", "Third", "", "Fourth" };
            
            for (int i = 0; i < values.Length; i++)
            {
                wrapper.Value = values[i];
                Assert.AreEqual(expectedValues[i], wrapper.Value);
                Assert.AreEqual(expectedValues[i], comment.Value);
            }
        }

        [Test]
        public void Constructor_WithEmptyComment_CreatesValidWrapper()
        {
            XComment comment = new XComment("");
            XCommentWrapper wrapper = new XCommentWrapper(comment);
            
            Assert.IsNotNull(wrapper);
            Assert.AreEqual("", wrapper.Value);
            Assert.AreEqual(XmlNodeType.Comment, wrapper.NodeType);
        }
#endif
    }
}