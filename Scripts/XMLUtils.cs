namespace CatRoyale.Scripts;

using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System;
using System.IO;

public static class XMLUtils
{
    //methode trouvé sur internet qui attend un xmlFilePath et un ou plusieurs couples de (namespaceName et un xsdpath associé)
    public static void ValidateXmlFileAsync(string xmlFilePath, params (string namespaceName, string xsdPath)[] schemas)
    {
        var settings = new XmlReaderSettings();
    
        // Charger tous les schémas
        foreach (var (namespaceName, xsdPath) in schemas)
        {
            settings.Schemas.Add(namespaceName, xsdPath);
        }
    
        settings.ValidationType = ValidationType.Schema;
        Console.WriteLine("Nombre de schemas utilisés dans la validation : " + settings.Schemas.Count);
        settings.ValidationEventHandler += ValidationCallback;
    
        using (var readItems = XmlReader.Create(xmlFilePath, settings))
        {
            while (readItems.Read()) { }
        }
    
        Console.WriteLine("Validation terminée.");
    }
    
    private static void ValidationCallback(object? sender, ValidationEventArgs e) 
    {
        if (e.Severity.Equals(XmlSeverityType.Warning)) 
        {
            Console.Write("WARNING: ");
            Console.WriteLine(e.Message);
        } 
        else if (e.Severity.Equals(XmlSeverityType.Error)) 
        {
            Console.Write("ERROR: ");
            Console.WriteLine(e.Message);
        }
    }
    
    
    //methode trouvé sur internet qui appel une feuille de transmformation xslt en C# :
    public static void XslTransform(string xmlFilePath, string xsltFilePath, string htmlFilePath)
    {
        try
        {
            // Validation des chemins
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException($"Le fichier XML n'existe pas : {xmlFilePath}");
    
            if (!File.Exists(xsltFilePath))
                throw new FileNotFoundException($"Le fichier XSLT n'existe pas : {xsltFilePath}");

            // Chargement du document XML
            XPathDocument xpathDoc = new XPathDocument(xmlFilePath);
    
            // Configuration XSLT
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings(enableDocumentFunction: true, enableScript: false);
            XmlUrlResolver resolver = new XmlUrlResolver();
            xslt.Load(xsltFilePath, settings, resolver);

            // Configuration du writer pour HTML
            XmlWriterSettings writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true, // Pas de <?xml?> pour HTML
            };

            // Transformation
            using (XmlWriter htmlWriter = XmlWriter.Create(htmlFilePath, writerSettings))
            {
                xslt.Transform(xpathDoc, null, htmlWriter, resolver);
            }

            Console.WriteLine($"Transformation réussie : {htmlFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la transformation : {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Erreur interne : {ex.InnerException.Message}");
            }
            throw;
        }
    }
    
    //methode trouvé sur internet qui appel une feuille de transmformation xslt en C# et un parametre qui sera l'Id d'un infirmier :
    public static void XslTransformParam(string xmlFilePath, string xsltFilePath, string htmlFilePath, string nomParam, string valeurParam)
    {
        try
        {
            // Validation des chemins
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException($"Le fichier XML n'existe pas : {xmlFilePath}");
    
            if (!File.Exists(xsltFilePath))
                throw new FileNotFoundException($"Le fichier XSLT n'existe pas : {xsltFilePath}");

            // Chargement du document XML
            XPathDocument xpathDoc = new XPathDocument(xmlFilePath);
    
            // Configuration XSLT
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings(enableDocumentFunction: true, enableScript: false);
            XmlUrlResolver resolver = new XmlUrlResolver();
            xslt.Load(xsltFilePath, settings, resolver);
            
            XsltArgumentList parametre = new XsltArgumentList();
            parametre.AddParam(nomParam, "", valeurParam);

            // Configuration du writer pour HTML
            XmlWriterSettings writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true, // Pas de <?xml?> pour HTML
            };

            // Transformation
            using (XmlWriter htmlWriter = XmlWriter.Create(htmlFilePath, writerSettings))
            {
                xslt.Transform(xpathDoc, parametre, htmlWriter, resolver);
            }

            Console.WriteLine($"Transformation réussie : {htmlFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la transformation : {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Erreur interne : {ex.InnerException.Message}");
            }
            throw;
        }
    }
}
