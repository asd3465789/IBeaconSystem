<?php
$servername = "localhost";
$username = "root";
$password = "12345678";
$dbname="test";

//跟unity那方連結的參數
$userID=$_POST["UserID"];
$ibeaconMinor = $_POST["ibeaconMinor"];
$stayTime = $_POST["stayTime"];
$ibeaconSort = $_POST["ibeaconSort"];
$nowDate = $_POST["nowDate"];

// Create connection
$conn = new mysqli($servername, $username, $password,$dbname);

// Check connection
if ($conn->connect_error) 
{
    die("Connection failed: " . $conn->connect_error);
}

//尋找是否有一樣的資料
$sql = "SELECT `ID` FROM `ibeacon` WHERE `ID`= '".$userID."' && `Date` = '".$nowDate."'";
$result = $conn->query($sql);

//如果資料有一樣就更新，沒有的話就新增
if ($result->num_rows > 0) 
{
    $updateData= "UPDATE `ibeacon` SET `stayTime`='".$stayTime."' WHERE `ID`='".$userID."' && `Date`='".$nowDate."'";
    if($conn->query($updateData)==true)
    {
        echo "更新成功";
    }
    else
    {
        echo "更新失敗";
    }
} 
else 
{
    $InsertSql="INSERT INTO `ibeacon`(`ID`, `Date`, `ibeaconSort`, `ibeaconMinor`, `stayTime`) VALUES ('".$userID."','".$nowDate."','".$ibeaconSort."','".$ibeaconMinor."','".$stayTime."')"; //新增id資料

    if($conn->query($InsertSql)==true)
    {
        //echo "新增成功";

    }
    else
    {
        //echo "新增失敗";
    }
}
 
$conn->close();

?>