<?php

$imagefile = 'http://maps.ljmindmap.com/live/'.$_GET['f'] ;
$image=imagecreatefromgif($imagefile); 

$f=fopen("allviews.txt","a");
fwrite($f, $_GET['f']  . " " . $_GET['x'] . "\r\n");
fwrite($f, "# the above is from " . $_SERVER['HTTP_REFERER'] . " " . $_SERVER['REMOTE_ADDR'] . "\r\n");
fclose($f); 

if( !empty($_GET['t']) )
{
$newimage = imagecreate(500,80);
list($width, $height) = getimagesize($imagefile);
imagecopy($newimage, $image, 0, 0, $width/2-250, $height/2-40, 500, 80);
$image = $newimage;

if(isset($_SERVER['HTTP_REFERER']))
    if( strrchr($_SERVER['HTTP_REFERER'], ".html"))
{
	$image = imagecreate(1,1);
}
}

// Output the image to browser
header('Content-type: image/gif');

imagegif($image);
imagedestroy($image);
?>
