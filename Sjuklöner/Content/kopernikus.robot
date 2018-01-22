-Conenct to database.
database connection ‴Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=aspnet-Sjuklöner-20171211025241;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False‴

➜scriptstart
timeout.reset value 99999999
file.exists filename ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴ errorjump ➜scriptstart

text.read ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴ result xmlstring
file.delete ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴

xml text ♥xmlstring search ‴SSN‴ result SSN
xml text ♥xmlstring search ‴OrgNumber‴ result OrgNumber
xml text ♥xmlstring search ‴ReferenceNumber‴ result ReferenceNumber

ie.open ‴registerplattform.ivo.se‴
window title ‴✱registerplattform✱‴ style ‴maximize‴

-Set the search term to the organization number from .NET app.
ie.setattribute name ‴value‴ value ♥OrgNumber search ‴searchText‴ by ‴class‴
-Set the selected index for the register to search to the third option, Vårdgivarregistret.
ie.runscript ‴document.getElementById("j_id0:j_id8:ivoPageBlock:filter:j_id93:j_id94:j_id95:RecordType").selectedIndex = 2‴
-Click the search button.
ie.click ‴j_id0:j_id8:ivoPageBlock:filter:search1ID‴

delay milliseconds 100
-Script for getting the number of hits from the search:
ie.runscript ‴document.getElementsByClassName("pbBody")[2].childNodes[0].childNodes[1].innerHTML‴ result hits

♥ivovalid = true
jump ➜hitstrue if ⊂Convert.ToInt32(♥hits) > 0⊃
♥ivovalid = false
➜hitstrue

-Update information of the claim to reflect results of the check of IVO.
sql query ‴UPDATE dbo.Claims SET IVOCheck = ♥ivovalid WHERE ReferenceNumber = '♥ReferenceNumber';‴

-Start ProCapita
program name ‴♥appdata\Bitoreq AB\KoPerNikus\Procapita.exe‴
window title ‴✱ProCapita✱‴

-Click Mina Brukare.
mouse.click position (point)110,1091 relative true
-Click the search field.
mouse.click position (point)154,115 relative true
-Type the SSN to search for into search field.
keyboard ♥SSN keydelay 10
-Click the search button
mouse.click position (point)249,124 relative true

-Update database to reflect results of ProCapita check ? czy to bedzie dzialac w nowym robocie?
-sql query ‴UPDATE dbo.Claims SET ProCapitaCheck = ♥ProCapitaValid WHERE ReferenceNumber = '♥ReferenceNumber'‴

-Loop the script.
jump ➜scriptstart
