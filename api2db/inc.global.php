<?php

/*////////////////////////////
  inc.global.php

  Global functions, constants, and other parts are set up here.

	Benn Eichhorn
	20 July 2006
*/////////////////////////////


// Add runtime settings

// Logging file
define("LOG_FILE", dirname(__FILE__) . "/logs/". $settings["HOSTNAME"] . date(".ym.") ."log");

/// Check for log dir
if(!is_dir(dirname(LOG_FILE)))
{
  if(!mkdir(dirname(LOG_FILE), 0777, TRUE))
  {
    print "Log file dir ".dirname(LOG_FILE)." doesn't exist and it couldn't be created\n".
    exit(1);
  }
  else
    print "Made log dir : ". dirname(LOG_FILE) . "\n";
}
// Check that we can write to log file
if(!is_file(LOG_FILE))
{
  if(!file_put_contents(LOG_FILE, date("c ") . "Log created \r\n" , FILE_APPEND))
  {
    print "Failed making log file ". LOG_FILE ."\n";
    exit(1);
  }
}



// Class autoloader
function __autoload($sClass)
{
    require_once(dirname(__FILE__) . "/classes/class." . substr($sClass,1) . ".php");
}

// Write message to log file
function wLog($class, $messages)
{
  $messages = explode("\n", $messages);
  foreach($messages AS $msg)
  {
	  file_put_contents(LOG_FILE, date("c ")."$class\t$msg\r\n" , FILE_APPEND);
	}
}



/*
// RTD Server IP
$settings["RTD"]["IP"] = "10.10.5.140";
// RTD Server Port
$settings["RTD"]["PORT"] = "1290";

// Database server IP
$settings["DB"]["HOST"] = "10.10.5.141";
// Database server User
$settings["DB"]["USERNAME"] = "rtd2gtrade";
// Database server Password
$settings["DB"]["PASSWORD"] = "rtd2gtrade";
// Database gTrade database
$settings["DB"]["ML"] = "NPC_ML";
// Database RTDdatabase
$settings["DB"]["RTD"] = "NPC_RTD";

// Default stock settlement date
$settings["ML_SETTLEMENT_DEFAULT"] = 3;

// ML GTrade Fields
$settings["BROKERCODE"] = "MEMBXXXX";
*/
?>
