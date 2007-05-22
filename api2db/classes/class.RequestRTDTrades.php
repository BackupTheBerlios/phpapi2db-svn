<?php

/*////////////////////////////
	class.rtd2db.php

	This is the main execution class


TODO put a list of ini settings required for RTD Trade here


	Benn Eichhorn
	30 Jan 2007
*/// //////////////////////////

class CRequestRTDTrades extends CRequestRTD {

   public $oContracts; // holds all data for every symbol. Key = fid_contract_id
   public $oUsers; // holds all data for rts users. Key = fid_user_id
   public $oAccounts; // holds all data for rts users. Key = fid_account_id
   public $oExchanges; // holds all data for exchanges. Key = fid_exchange_id
   public $oTrades;

   public $iLastTradeId;

   private function GetLastTradeId() { return $this->iLastTradeId; }

   function __construct(Array &$settings)
   {
   	$this->aaSettings = $settings;

      wLog(get_class($this), " \n--- Loading ---");
		// Check ML Clear settings
		if(!isset($this->aaSettings["DB"]["HOST"], $this->aaSettings["DB"]["USERNAME"], $this->aaSettings["DB"]["PASSWORD"]))
		{
			wlog(get_class($this), "E  Arggghhhh DB settings not found in INI file!!! Blowing up ungracefully...");
			unset($this);
		}
      //$this->oDatabase = new CDbMssql($this->aaSettings["DB"]["HOST"], $this->aaSettings["DB"]["USERNAME"], $this->aaSettings["DB"]["PASSWORD"]);

		// Check TRADE (P&L) settings
		if(!isset($this->aaSettings["RTDHOST"]["IP"],	$this->aaSettings["RTDHOST"]["PORT"]))
		{
			wlog(get_class($this), "E  Arggghhhh RTDHOST settings not found in INI file!!! Blowing up ungracefully...");
			unset($this);
		}
      $this->oApi = new CSocketStream($this->aaSettings["RTDHOST"]["IP"], $this->aaSettings["RTDHOST"]["PORT"]);
	}

   function __destruct()
   {
      wLog(get_class($this), "--- Shuting down ---");

      if(isset($this->oApi)) unset($this->api);
      if(isset($this->oDatabase)) unset($this->database);

		wLog(get_class($this), "--- Stopped ---\n ");
	}

	public function Initialise()
	{
      $this->oContracts = new CMessageRTDContracts($this);
      $this->oUsers = new CMessageRTDUsers();
      $this->oAccounts = new CMessageRTDAccounts();
      $this->oExchanges = new CMessageRTDExchanges();
      $this->oGroups = new CMessageRTDGroups();

      // Pass a refernce of this to the trade object
      $this->oTrades = new CMessageRTDTrades($this);

      // Loads last trade id
      $this->LoadLastTradeId();
      //$this->oTrades->LoadSettlements();

      wLog(get_class($this), "--- Initialised ---");
	}


	// function gets settings from respective ini to load the last trade id
	public function LoadLastTradeId()
	{
	  //$this->iLastTradeId = 1487260;
	  //return;

    print "checking trade id!";

    if(isset($this->aaSettings["WEBPL"]))
    {
      $this->iLastTradeId = file_get_contents($this->aaSettings["WEBPL"]["NEXTID"]);
      print "loading since trade id = ". $this->iLastTradeId . "\n";
    }
		elseif(isset($this->aaSettings["TRADE"]))
		{
			if(!isset($this->aaSettings["TRADE"]["LAST_DB"], $this->aaSettings["TRADE"]["LAST_TABLE"]))
			{
				wlog(get_class($this), "E Arggghhhh last trade settings not found in INI file!!! Can't continue! Blowing up ungracefully...");
				unset($this);
				exit();
			}
  		$this->oDatabase->SelectDatabase($this->aaSettings["TRADE"]["LAST_DB"]);
  		$table = $this->aaSettings["TRADE"]["LAST_TABLE"];
  		$data = "lastId";
  		$where = Array("hostname='". $this->aaSettings["HOSTNAME"]. "'");

  		list($result) = $this->oDatabase->SelectQueryWhere($table, $data, $where);
  		$this->iLastTradeId = $result["lastId"];
  	}
	}

   public function SendRequests()
   {
	   // Get the list of queries to send to the API
      $requests[] = $this->oContracts->GetRequest();
      $requests[] = $this->oExchanges->GetRequest();
      $requests[] = $this->oUsers->GetRequest();
      $requests[] = $this->oAccounts->GetRequest();
      $requests[] = $this->oGroups->GetRequest();
      $requests[] = $this->oTrades->GetRequest();

      // Send the queries to the API for processing
      $this->oApi->sendRequests($requests);
   }

   protected function ParseResponse($rid, &$message)
   {
   	print $this->aRequestTypes[$rid] . "\n";
		switch($this->aRequestTypes[$rid])
		{
      case "rid_trade_t":
         		print "!";
         $this->oTrades->DecodeResponse($message);
         $this->oTrades->ProcessResponse();
         break;
      // stocks, futures, currencies, options etc
      case "rid_contract_t":
         $this->oContracts->DecodeResponse($message);
         break;
      // User, counter and contra parties
      case "rid_account_t":
         $this->oAccounts->DecodeResponse($message);
         break;
      // User, counter and contra parties
      case "rid_usr_t":
         $this->oUsers->DecodeResponse($message);
         break;
      // Exchange info
      case "rid_exchange_t":
         $this->oExchanges->DecodeResponse($message);
         break;
      // Exchange info
      case "rid_group_t":
         $this->oGroups->DecodeResponse($message);
         break;
      // Response header
      case "rid_answer_t":
		  default:
        break;
		}
   }

} // end class

?>
