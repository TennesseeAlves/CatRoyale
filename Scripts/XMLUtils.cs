namespace CatRoyale.Scripts;

using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;

public static class XMLUtils
{
    public static void XslTransform(string xmlFilePath, string xsltFilePath, string htmlFilePath)
    {
        //validation des chemins
        if (!File.Exists(xmlFilePath))
            throw new FileNotFoundException($"Le fichier XML n'existe pas : {xmlFilePath}");
        if (!File.Exists(xsltFilePath))
            throw new FileNotFoundException($"Le fichier XSLT n'existe pas : {xsltFilePath}");
        //chargement du document XML
        XPathDocument xpath = new XPathDocument(xmlFilePath);
        //configuration XSLT
        XslCompiledTransform xslt = new XslCompiledTransform();
        XsltSettings settings = new XsltSettings(true, false);
        XmlUrlResolver resolver = new XmlUrlResolver();
        xslt.Load(xsltFilePath, settings, resolver);
        //configuration du writer pour HTML
        XmlWriterSettings writerSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "  ",
            OmitXmlDeclaration = true, // Pas de <?xml?> pour HTML
        };
        //transformation
        XmlWriter htmlWriter = XmlWriter.Create(htmlFilePath, writerSettings);
        xslt.Transform(xpath, null, htmlWriter, resolver);
    }
}
