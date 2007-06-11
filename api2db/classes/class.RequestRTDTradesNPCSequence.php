<?php

/*////////////////////////////
	class.rtd2db.php

	This is the main execution class

	Benn Eichhorn
	30 Jan 2007
*/// //////////////////////////

class CRequestRTDTradesNPCSequence extends CRequestRTD {

   public $oContracts; // holds all data for every symbol. Key = fid_contract_id
   public $oUsers; // holds all data for rts users. Key = fid_user_id
   public $oAccounts; // holds all data for rts users. Key = fid_account_id
   public $oExchanges; // holds all data for exchanges. Key = fid_exchange_id
   public $oTrades;

   public $iLastTradeId;
   public $aAllTradeIds; // Holds an array of all trade ids we are looking for.

   private function GetLastTradeId() { return $this->iLastTradeId; }

   function __construct(Array &$settings)
   {
   	$this->aaSettings = $settings;

      wLog(get_class($this), "I --- Loading ---");
		// Check Sequence settings
		if(!isset($this->aaSettings["SEQUENCE"]["SOURCENAME"], $this->aaSettings["SEQUENCE"]["NEXTID"], $this->aaSettings["SEQUENCE"]["TRADE"]))
		{
			wlog(get_class($this), "E  Arggghhhh Sequence settings not found in INI file!!! Blowing up ungracefully...");
			unset($this);
		}
		// Check RTD settings
		if(!isset($this->aaSettings["RTDHOST"]["IP"],	$this->aaSettings["RTDHOST"]["PORT"]))
		{
			wlog(get_class($this), "E  Arggghhhh RTDHOST settings not found in INI file!!! Blowing up ungracefully...");
			unset($this);
		}
      //$this->oApi = new CSocketStream($this->aaSettings["RTDHOST"]["IP"], $this->aaSettings["RTDHOST"]["PORT"]);
	}

   function __destruct()
   {
      wLog(get_class($this), "I --- Shuting down ---");

      if(isset($this->oApi)) unset($this->api);
      if(isset($this->oDatabase)) unset($this->database);

		wLog(get_class($this), "I --- Stopped ---\n ");
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

      wLog(get_class($this), "I --- Initialised ---");
	}


	// function gets settings from respective ini to load the last trade id
	public function LoadLastTradeId()
	{
		if(isset($this->aaSettings["SEQUENCE"]))
		{
			wlog(get_class($this), "I Processing trades for SEQUENCE");
			wlog(get_class($this), "I Getting sequence from ". $this->aaSettings["SEQUENCE"]["NEXTID"]);
			$ids = file_get_contents($this->aaSettings["SEQUENCE"]["NEXTID"]);
			wlog(get_class($this), "I Response was: ". $ids);
			if($ids !== FALSE && $ids[0] != "FALSE")
			{
				$ids = explode(",", $ids);
				print_r($ids);
				$this->iLastTradeId = $ids[0];
				$this->aAllTradeIds = $ids;
			}
			else
			{
				wlog(get_class($this), "I All trades are in sequence");
				exit(-1);
			}
			wlog(get_class($this), "I Processing trades since rtd_trade_id=". $this->iLastTradeId);
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
		switch($this->aRequestTypes[$rid])
		{
      case "rid_trade_t":
         $this->oTrades->DecodeResponse($message);
         if(in_array($this->oTrades->aTrade["fid_trade_id"], $this->aAllTradeIds))
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
      // Group info
      case "rid_group_t":
         $this->oGroups->DecodeResponse($message);
         break;
      // Response header
      case "rid_answer_t":
        //print  $this->aRequestTypes[$rid] . "\t" . $message . "\n";
		  default:
        break;
		}
   }

} // end class

?>
