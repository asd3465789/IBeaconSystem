<?php
$servername = "localhost";
$username = "root";
$password = "zxcrgy4568765432619";
$dbname="test";

// Create connection
$conn = new mysqli($servername, $username, $password,$dbname);

// Check connection
if ($conn->connect_error) 
{
    die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT COUNT(*) FROM `ibeaconusermember`";//計算資料總筆數

$result = $conn->query($sql);

if ($result->num_rows > 0) 
{
    // output data of each row 印出資料
    while($row = $result->fetch_assoc()) 
    {
        echo $row["COUNT(*)"]+1;
        $UserID=$row["COUNT(*)"]+1;//id為總筆數+1
    }
} 
else 
{
    echo "0 results";
}

$InsertSql="INSERT INTO `ibeaconusermember`(ID) VALUES ('".$UserID."')"; //新增id資料

if($conn->query($InsertSql)==true)
{
    //echo "創建成功";

}
else
{
    //echo "創建失敗";
}

$conn->close();




?>