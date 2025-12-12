<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:cat="http://www.univ-grenoble-alpes.fr/l3miage/CatRoyaleGame"
                exclude-result-prefixes="cat"
>
    <xsl:output method="html" indent="yes"/>
    
    <xsl:template match="/">
        <html>
            <head>
                <title>Cat Royal</title>
                <link rel="stylesheet" href="../css/autoSave.css"/>
            </head>
            
            <body>
                <div class="classJoueur">
                    <h1>Joueur 1 : </h1>
                    <xsl:variable name="joueur1" select="//cat:joueur1"/>
                    <h2>Pseudo : <span><xsl:value-of select="$joueur1/cat:pseudo"/></span></h2>
                    <h2>win streak : <span><xsl:value-of select="$joueur1/cat:winStreak"/></span></h2>
                </div>
                <div class="classJoueur">
                    <h1>Joueur 2 : </h1>
                    <xsl:variable name="joueur2" select="//cat:joueur2"/>
                    <h2>Pseudo : <span><xsl:value-of select="$joueur2/cat:pseudo"/></span></h2>
                    <h2>win streak : <span><xsl:value-of select="$joueur2/cat:winStreak"/></span></h2>
                </div>
            </body>
            
        </html>
    </xsl:template>
    
    <!-- Template pour les nœuds text (pour redefinir le comportement par défaut) -->
    <xsl:template match="text()"/>

    <!-- Template pour les nœuds commentaires (pour redefinir le comportement par défaut) -->
    <xsl:template match="comment()"/>
    
</xsl:stylesheet>
