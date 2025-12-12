using var game = new CatRoyale.Scripts.CatRoyal();
game.Run();
CatRoyale.Scripts.XMLUtils.XslTransform("../../../data/xml/autoSave.xml", "../../../data/xslt/autoSave_to_html.xsl", "../../../data/html/autoSave.html"); 
