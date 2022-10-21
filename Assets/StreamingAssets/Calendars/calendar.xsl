<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <body>
                <h2>Calendar:
                    <xsl:value-of select="Calendar/@name"/>
                </h2>
                <h2>Calendar author:
                    <xsl:value-of select="Calendar/@author"/>
                </h2>
                <table border="1">
                    <tr bgcolor="#9acd32">
                        <th>Jumper id</th>
                        <th>Points table</th>
                        <th>Points table team</th>
                    </tr>
                    <xsl:for-each select="Calendar/classifications/individual-place|Calendar/classifications/team-place">
                        <tr>
                            <td>
                                <xsl:value-of select="@name"/>
                            </td>
                            <td>
                                <xsl:value-of select="points-table/@value|points-table-ind/@value"/>
                            </td>
                            <td>
                                <xsl:value-of select="points-table-team/@value"/>
                            </td>
                        </tr>
                    </xsl:for-each>
                </table>
            </body>
        </html>
    </xsl:template>

</xsl:stylesheet>