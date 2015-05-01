<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="html"  encoding="utf-16"/>
  <xsl:template match="/">
    <head>
      <title>TFS Permissions Report</title>
      <style type="text/css">
        body{ text-align: left; width: 95%;  font-family: Calibri, sans-serif; }

        table{ margin-left:60px; border: none;  border-collapse: separate;  width: 90%; }

        tr.title td{ background: white;font-size: 26px;  font-weight: bold; }

        th{ background: #d0d0d0;  font-weight: bold;  font-size: 16pt;  text-align: left; }
        tr{ background: #eeeeee}
        td, th{
        font-family:'Segoe UI'; font-weight:lighter;
        font-size: 12pt;  padding: 10px;  border: none; }
        h1 {
        margin-top:10px;
        font-size:xx-large;
        font-weight:lighter;
        font-family:'Segoe UI';
        }

        .ProjectsHeader {

        }
        span.tab{
        padding: 0 80px; /* Or desired space*/
        }
        tr.info td{}
        tr.warning td{background-color:yellow;color:black}
        tr.error td{background-color:red;color:black}

        a:hover{text-transform:uppercase;color: #9090F0;}
      </style>
    </head>

    <body>
      <table>
        <tr class="title">
          <td>
            <img style="float:left; margin-right:0px; margin-bottom:0px" src="ALMRangers_Logo.png" alt="" title="Home" />
          </td>
          <td colspan="7">
            <h1 style="margin-left:60px">TFS Permissions Report</h1>
          </td>
        </tr>
        <tr>
          <td colspan="2">Date</td>
          <td colspan="5">
            <xsl:value-of select="//PermissionsReport/Date"/>
          </td>
        </tr>
        <tr>
          <td colspan="2">User</td>
          <td colspan="5">
            <xsl:value-of select="//PermissionsReport/UserName"/>
          </td>
        </tr>
        <tr>
          <td colspan="2">TFS Collection</td>
          <td colspan="5">
            <xsl:value-of select="//PermissionsReport/TfsCollectionUrl"/>
          </td>
        </tr>


      </table>
      <!-- Permissions Table -->
      <table>
        <tr/>
        <tr/>
        <tr>
          <th>Team Projects</th>
          <table>
            <xsl:for-each select="//PermissionsReport/TeamProjects/TeamProject">
              <tr>
                <td colspan="2" style="background-color:#383838; color:white; padding-left:10px">
                  <b>
                    <xsl:value-of select="Name"/>
                  </b>
                </td>
              </tr>
              <!--Project Level Permissions-->
              <tr>

                <td>Project Level Permissions</td>
                <td>
                  <table>
                    <xsl:for-each select="ProjectLevelPermissions/ProjectLevelPermissionsList/Permission">

                      <tr>
                        <td>
                          <xsl:value-of select="DisplayName"/>
                        </td>
                        <td>
                          <xsl:value-of select="Value"/>
                          <span class="tab">
                            <xsl:value-of select="GroupMemberInheritance"/>
                          </span>
                        </td>
                      </tr>
                    </xsl:for-each>
                  </table>
                </td>


              </tr>
              <!--Area-->
              <tr>
                <td>Area Level Permissions</td>
                <td>
                  <table>
                    <xsl:for-each select="AreaPermissions/AreaPermission">
                      <tr>
                        <td>
                          <b>
                            <xsl:value-of select="AreaName"/>
                          </b>
                        </td>
                        <td>
                          <xsl:for-each select="AreaPermissions/Permission">

                            <tr>
                              <td>
                                <xsl:value-of select="DisplayName"/>
                              </td>
                              <td>
                                <xsl:value-of select="Value"/>
                                <span class="tab">
                                  <xsl:value-of select="GroupMemberInheritance"/>
                                </span>
                              </td>
                            </tr>
                          </xsl:for-each>
                        </td>
                      </tr>
                    </xsl:for-each>

                  </table>
                </td>


              </tr>
              <!--Iteration-->
              <tr>
                <td>Iteration Level Permissions</td>
                <td>
                  <table>
                    <xsl:for-each select="IterationPermissions/IterationPermission">
                      <tr>
                        <td>
                          <b>
                            <xsl:value-of select="IterationName"/>
                          </b>
                        </td>
                        <td>
                          <xsl:for-each select="IterationPermissions/Permission">

                            <tr>
                              <td>
                                <xsl:value-of select="DisplayName"/>
                              </td>
                              <td>
                                <xsl:value-of select="Value"/>
                                <span class="tab">
                                  <xsl:value-of select="GroupMemberInheritance"/>
                                </span>
                              </td>
                            </tr>
                          </xsl:for-each>
                        </td>
                      </tr>
                    </xsl:for-each>
                  </table>
                </td>
              </tr>

              <!--Version Control Permissions-->

              <tr>

                <td >Version Control Permissions</td>
                <td>
                  <table>
                    <xsl:for-each select="VersionControlPermissions/VersionControlPermissionsList/Permission">

                      <tr>
                        <td>
                          <xsl:value-of select="DisplayName"/>
                        </td>
                        <td>
                          <xsl:value-of select="Value"/>
                          <span class="tab">
                            <xsl:value-of select="GroupMemberInheritance"/>
                          </span>
                        </td>
                      </tr>
                    </xsl:for-each>
                  </table>
                </td>
              </tr>
              <!-- Git Version Control Permissions-->
              <tr>
                <td >Git Version Control Permissions</td>
                <td>
                  <table>
                    <xsl:for-each select="GitVersionControlPermissions/VersionControlPermissionsList/Permission">
                      <tr>
                        <td>
                          <xsl:value-of select="DisplayName"/>
                        </td>
                        <td>
                          <xsl:value-of select="Value"/>
                          <span class="tab">
                            <xsl:value-of select="GroupMemberInheritance"/>
                          </span>
                        </td>
                      </tr>
                    </xsl:for-each>
                  </table>
                </td>
              </tr>

            </xsl:for-each>
          </table>
        </tr>
      </table>
    </body>
  </xsl:template>
</xsl:stylesheet>