<?php

// api2db
//
// Feed handler to database script
// written by Benn Eichhorn
//
// 14 May 2007


// Jan 2007



set_time_limit(0);

error_reporting(E_ALL & ~E_NOTICE);



if($argv[1] == "-t" && $argc == 3 && is_file($argv[2]))
{

	$settings = parse_ini_file($argv[2], TRUE);

	$settings["HOSTNAME"] = basename($argv[2], ".ini");

	include_once(dirname(__FILE__) . "/inc.global.php");

	$oRequestRTDTrades = new CRequestRTDTrades($settings);

	// Start main program loop
	$oRequestRTDTrades->Initialise();
	$oRequestRTDTrades->SendRequests();
	$oRequestRTDTrades->GetResponse();

	unset($oRequestRTDTrades);

}
elseif($argv[1] == "-p" && $argc == 3 && is_file($argv[2]))
{

	$settings = parse_ini_file($argv[2], TRUE);

	$settings["HOSTNAME"] = basename($argv[2], ".ini");

	include_once(dirname(__FILE__) . "/inc.global.php");

	$oRequestRTDPrices = new CRequestRTDPrices($settings);

	// Start main program loop
	$oRequestRTDPrices->Initialise();
	$oRequestRTDPrices->SendRequests();
	$oRequestRTDPrices->GetResponse();

	unset($oRequestRTDTrades);

}
elseif($argv[1] == "-s" && $argc == 3 && is_file($argv[2]))
{

	$settings = parse_ini_file($argv[2], TRUE);

	$settings["HOSTNAME"] = basename($argv[2], ".ini");

	include_once(dirname(__FILE__) . "/inc.global.php");

	$oRequestRTDTrades = new CRequestRTDTradesNPCSequence($settings);

	// Start main program loop
	$oRequestRTDTrades->Initialise();
  if($oRequestRTDTrades->GetLastTradeId() > -1)
  {
	 $oRequestRTDTrades->SendRequests();
	 $oRequestRTDTrades->GetResponse();
	}

	unset($oRequestRTDTrades);

}

else
{
?>

  Usage: <?php echo $argv[0]; ?> [option] <params>...

  Options

    RTD Trade Analysis
    -t <settingsfile.ini>
		Name of settings ini file must exist with relevent settings

    RTD Pricing Translation
    -p <settingsfile.ini>
		Name of settings ini file must exist with relevent settings
		Used for handling RTD prices to database

    RTD Trade ID Sequence Restore
    -s <settingsfile.ini>
		Name of settings ini file must exist with relevent settings
		Used for handling RTD trades that are mising from the database

    Other
    -h      Displays this help

<?php

}


exit;
?>
