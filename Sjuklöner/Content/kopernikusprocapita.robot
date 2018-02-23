-Conenct to database.
database connection ‴Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=aspnet-Sjuklöner-20171211025241;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False‴

-Start IVO
ie.open ‴registerplattform.ivo.se‴
ie.detach

-Start final results screen
ie.open ‴http://localhost:52552/Claims/StodSystemLogin‴
ie.detach

task.include boxing.robot

♥boxRect = (rectangle)0,0,1920,1200
♥notFoundPoint = (point)-1,-1
♥y = ♥notFoundPoint⟦y⟧

-Await file produced by the .NET app.
➜scriptstart
♥timeout = 99999999
file.exists filename ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴ errorjump ➜scriptstart
-Read and delete the file created by the .NET app.
text.read ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴ result xmlstring
file.delete ‴♥appdata\Bitoreq AB\KoPerNikus\info.xml‴

delay 10
window title ‴Amount✱‴
ie.attach phrase ‴Amount‴
delay milliseconds 100
ie.click search ‴logoff‴ by ‴id‴
delay 1
ie.detach

-Grab information from the info file.
xml text ♥xmlstring search ‴SSN‴ result SSN
xml text ♥xmlstring search ‴OrgNumber‴ result OrgNumber
xml text ♥xmlstring search ‴ReferenceNumber‴ result ReferenceNumber
xml text ♥xmlstring search ‴ClaimId‴ result ClaimId
xml text ♥xmlstring search ‴UserId‴ result UserId

ie.attach ‴http://registerplattform.ivo.se/‴ by url
window title ‴✱registerplattform✱‴ style ‴maximize‴
ie.seturl ‴registerplattform.ivo.se‴
delay milliseconds 300
-Catch potential jQuery error.
ocr.fromscreen area (rectangle)750,435,852,456 relative false result getError
keyboard ⋘E⋙ if ⊂♥getError.Contains("Fel")⊃
delay milliseconds 300
-Set the search term to the organization number from .NET app.
ie.setattribute name ‴value‴ value ♥OrgNumber search ‴searchText‴ by ‴class‴
delay milliseconds 200
-Set the selected index for the register to search to the third option, Vårdgivarregistret.
ie.runscript ‴document.getElementById("j_id0:j_id8:ivoPageBlock:filter:j_id93:j_id94:j_id95:RecordType").selectedIndex = 2‴
delay milliseconds 200
-Click the search button.
ie.click ‴j_id0:j_id8:ivoPageBlock:filter:search1ID‴
delay 1

-Script for getting the number of hits from the search:
ie.runscript ‴document.getElementsByTagName('strong')[0].innerHTML‴ result hits
delay milliseconds 300
ie.detach
window title ‴✱registerplattform✱‴ style ‴Minimize‴

♥ivovalid = true
jump ➜hitstrue if ⊂Convert.ToInt32(♥hits) > 0⊃
♥ivovalid = false
➜hitstrue

-Update information of the claim to reflect results of the check of IVO.
sql query ‴UPDATE dbo.Claims SET IVOCheck = '♥ivovalid' WHERE ReferenceNumber = '♥ReferenceNumber';‴

window ‴✱Desktop 2016✱‴
jump ➤box searchFor ‴dokumentation‴ rectangle ♥boxRect
mouse.click ♥result
delay milliseconds 100
keyboard ‴♥SSN‴
keyboard ⋘ENTER⋙
keyboard ⋘DOWN⋙
keyboard ⋘SHIFT+F10⋙
keyboard ⋘DOWN 3⋙
keyboard ⋘ENTER⋙
keyboard ⋘TAB 4⋙
keyboard ⋘SPACE⋙
clipboard text ‴‴
keyboard ⋘CTRL+C⋙
♥proxyvalid = false
jump ➜proxy if ⊂string.IsNullOrEmpty(♥clipboard)⊃
♥proxyvalid = true
➜proxy
sql query ‴UPDATE dbo.Claims SET ProxyCheck = '♥proxyvalid' WHERE ReferenceNumber = '♥ReferenceNumber'‴

delay milliseconds 200
keyboard ⋘CTRL+SHIFT+B⋙

jump ➤box searchFor ‴Funktion‴ rectangle ♥boxRect
♥result⟦y⟧ = ♥result⟦y⟧ + 30
♥y = ♥result⟦y⟧
mouse.move ♥result relative false
delay milliseconds 200
mouse.click ♥result
-jump ➤box searchFor ‴Beslut‴ rectangle ♥boxRect
-mouse.click ♥result relative false
keyboard ⋘UP 6⋙
keyboard ⋘ENTER⋙
keyboard ‴♥SSN‴
keyboard ⋘TAB 5⋙
sql query ‴SELECT LastDayOfSicknessDate FROM dbo.Claims WHERE ReferenceNumber = '♥ReferenceNumber';‴ result query
♥queryDate = ‴♥query‴
keyboard ⊂Convert.ToDateTime(♥queryDate.Substring(22)).Year.ToString().Substring(2)⊃
keyboard ‴0‴ if ⊂Convert.ToDateTime(♥queryDate.Substring(22)).Month < 10⊃
keyboard ⊂DateTime.Now.Month⊃
keyboard ‴0‴ if ⊂Convert.ToDateTime(♥queryDate.Substring(22)).Day < 10⊃
keyboard ⊂DateTime.Now.Day⊃
jump ➤box searchFor ‴Endast‴ rectangle ♥boxRect
♥result⟦x⟧ = ♥result⟦x⟧ - 20
♥result⟦y⟧ = ♥y
mouse.click ♥result relative false
-jump ➤box searchFor ‴assistent‴ rectangle ♥boxRect
-mouse.click ♥result
clipboard text ‴‴
keyboard ⋘CTRL+C⋙
keyboard ⋘ENTER⋙ if ⊂string.IsNullOrEmpty(♥clipboard)⊃
♥procapitavalid = false
jump ➜procapita if ⊂string.IsNullOrEmpty(♥clipboard)⊃
♥procapitavalid = true
➜procapita
-Update database to reflect results of ProCapita check
sql query ‴UPDATE dbo.Claims SET ProCapitaCheck = '♥procapitavalid' WHERE ReferenceNumber = '♥ReferenceNumber'‴

window title ‴Inloggning✱‴
ie.attach phrase ‴Inloggning‴

delay milliseconds 400
ie.setattribute search ‴Email‴ by ‴id‴ name ‴value‴ value ‴henrik.signell@helsingborg.se‴
delay milliseconds 400
ie.setattribute search ‴Password‴ by ‴id‴ name ‴value‴ value ♥credentials⟦pass⟧
delay milliseconds 400

ie.click search ‴login‴ by ‴id‴

➜logoff
♥timeout = 99999999
file.exists filename ‴♥appdata\Bitoreq AB\KoPerNikus\stodsystem.xml‴ errorjump ➜logoff
file.delete filename ‴♥appdata\Bitoreq AB\KoPerNikus\stodsystem.xml‴

-sql query ‴INSERT INTO Messages (ClaimId, CommentDate, Comment, applicationUser_Id) VALUES ('♥ClaimId', '⊂DateTime.Now⊃', 'Verksamheten finns i IVO', '♥UserId')‴ if ♥ivovalid
-sql query ‴INSERT INTO Messages (ClaimId, CommentDate, Comment, applicationUser_Id) VALUES ('♥ClaimId', '⊂DateTime.Now⊃', 'Verksamheten saknas i IVO', '♥UserId')‴ if ⊂!♥ivovalid⊃
-sql query ‴INSERT INTO Messages (ClaimId, CommentDate, Comment, applicationUser_Id) VALUES ('♥ClaimId', '⊂DateTime.Now⊃', 'Giltigt beslut om assistans finns', '♥UserId')‴ if ♥procapitavalid
-sql query ‴INSERT INTO Messages (ClaimId, CommentDate, Comment, applicationUser_Id) VALUES ('♥ClaimId', '⊂DateTime.Now⊃', 'Beslut om assistans saknas', '♥UserId')‴ if ⊂!♥procapitavalid⊃

delay 5

ie.click search ‴logoff‴ by ‴id‴
delay milliseconds 600
ie.runscript ‴document.URL‴ result url
♥url = ⊂♥url.Remove(22)⊃
ie.detach
window title ‴✱Inloggning✱‴ style ‴Minimize‴
delay milliseconds 300

ie.attach ‴StodSystem‴
ie.seturl ‴♥url/Claims/StodsystemLogin‴
window title ‴✱Stodsystem✱‴ style ‴Maximize‴
ie.attach ‴StodSystemLogin‴
delay 1
ie.setattribute name ‴value‴ value ♥ReferenceNumber search ‴referencenumber‴ by ‴id‴
ie.setattribute name ‴value‴ value ‴henrik.signell@helsingborg.se‴ search ‴användarnamn‴ by ‴id‴
delay milliseconds 500
ie.setattribute name ‴value‴ value ♥credentials⟦pass⟧ search ‴lösenord‴ by ‴id‴
delay milliseconds 500
ie.click search ‴login‴ by ‴id‴

➜finallogoff
♥timeout = 99999999
file.exists filename ‴♥appdata\Bitoreq AB\KoPerNikus\decided.xml‴ errorjump ➜finallogoff
file.delete filename ‴♥appdata\Bitoreq AB\KoPerNikus\decided.xml‴

delay 1

window title ‴✱Stodsystem✱‴ style ‴Minimize‴

window title ‴✱Inloggning✱‴ style ‴Maximize‴
ie.attach ‴Inloggning‴

delay milliseconds 400
ie.setattribute search ‴Email‴ by ‴id‴ name ‴value‴ value ‴ombud.ombudsson@assistans.se‴
delay milliseconds 400
ie.setattribute search ‴Password‴ by ‴id‴ name ‴value‴ value ♥credentials⟦pass⟧
delay milliseconds 400

ie.click search ‴login‴ by ‴id‴
ie.detach

-Loop the script.
jump ➜scriptstart
