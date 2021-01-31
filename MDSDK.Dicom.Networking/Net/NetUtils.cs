// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MDSDK.Dicom.Networking.Net
{
    internal static class NetUtils
    {
        public const string DicomApplicationContextName = "1.2.840.10008.3.1.1.1";

        public static void WriteAsciiStringTo(string aeTitle, byte[] asciiBuffer)
        {
            for (var i = 0; i < asciiBuffer.Length; i++)
            {
                asciiBuffer[i] = (i < aeTitle.Length) ? (byte)aeTitle[i] : (byte)' ';
            }
        }

        public static string ReadAsciiStringFrom(byte[] asciiBuffer)
        {
            var aeTitle = new char[asciiBuffer.Length];
            for (var i = 0; i < asciiBuffer.Length; i++)
            {
                aeTitle[i] = (char)asciiBuffer[i];
            }
            return new string(aeTitle).Trim();
        }

        public static void ThrowIf(bool condition, [CallerArgumentExpression("condition")] string callerArgumentExpression = null)
        {
            if (condition)
            {
                throw new Exception($"Assertion failed: {callerArgumentExpression}");
            }
        }

        private static XElement ToXml(object obj)
        {
            var type = obj.GetType();
            var objElement = new XElement(type.Name);
            foreach (var property in type.GetProperties())
            {
                var propertyValue = property.GetValue(obj);
                string attributeValue = null;
                if (propertyValue is string stringValue)
                {
                    attributeValue = stringValue;
                }
                else if (propertyValue is IFormattable formattableValue)
                {
                    attributeValue = formattableValue.ToString(null, NumberFormatInfo.InvariantInfo)
                        + $" (0x{formattableValue.ToString("X", NumberFormatInfo.InvariantInfo)})";
                }
                else if (propertyValue is byte[] byteArrayValue)
                {
                    if (byteArrayValue.All(b => (b >= 32) && (b < 128)))
                    {
                        attributeValue = Encoding.ASCII.GetString(byteArrayValue);
                    }
                    else
                    {
                        attributeValue = $"0x{Convert.ToHexString(byteArrayValue)}";
                    }
                }
                else if (propertyValue is IEnumerable enumerableValue)
                {
                    var propertyElement = new XElement(property.Name);
                    foreach (var item in enumerableValue)
                    {
                        var itemElement = ToXml(item);
                        propertyElement.Add(itemElement);
                    }
                    objElement.Add(propertyElement);
                }
                else if (propertyValue != null)
                {
                    attributeValue = propertyValue.ToString();
                }
                if (attributeValue != null) 
                {
                    objElement.SetAttributeValue(property.Name, attributeValue);
                }
            }
            return objElement;
        }

        private static readonly XmlWriterSettings TraceWriterSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            CloseOutput = false,
            Indent = true,
            NewLineOnAttributes = false,
        };

        public static void TraceOutput(TextWriter writer, string prefix, object obj)
        {
            writer.Write(prefix);
            var xml = ToXml(obj);
            using (var xmlWriter = XmlWriter.Create(writer, TraceWriterSettings))
            {
                xml.WriteTo(xmlWriter);
            }
            writer.WriteLine();
            writer.Flush();
        }
    }
}
