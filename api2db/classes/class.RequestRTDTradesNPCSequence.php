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

   public $iLastTradeId = -1;
   public $aAllTradeIds; // Holds an array of all trade ids we are looking for.

   public function GetLastTradeId() { return $this->iLastTradeId; }

   function __construct(Array &$settings)
   {
   	$this->aaSettings = $settings;

      wLog(get_class($this), "I --- Loading ---");
		// Check Sequence settings
		if(!isset($this->aaSettings["SEQUENCE"]["NEXTID"]))
		{
			wlog(get_class($this), "E  Arggghhhh Sequence settings not found in INI file!!! Blowing up ungracefully...");
			exit(-1);
		}
    // Check Sequence settings
		if(!isset($this->aaSettings["WEBPL"]["SOURCENAME"], $this->aaSettings["WEBPL"]["TRADE"]))
		{
			wlog(get_class($this), "E  Arggghhhh WebPL settings not found in INI file!!! Blowing up ungracefully...");
			exit(-1);
		}
		// Check RTD settings
		if(!isset($this->aaSettings["RTDHOST"]["IP"],	$this->aaSettings["RTDHOST"]["PORT"]))
		{
			wlog(get_class($this), "E  Arggghhhh RTDHOST settings not found in INI file!!! Blowing up ungracefully...");
			exit(-1);
		}
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
      // Loads last trade id
      $this->LoadLastTradeId();

      $this->oContracts = new CMessageRTDContracts($this);
      $this->oUsers = new CMessageRTDUsers();
      $this->oAccounts = new CMessageRTDAccounts();
      $this->oExchanges = new CMessageRTDExchanges();
      $this->oGroups = new CMessageRTDGroups();

      // Pass a refernce of this to the trade object
      $this->oTrades = new CMessageRTDTrades($this);

      wLog(get_class($this), "I --- Initialised ---");
	}


	// function gets settings from respective ini to load the last trade id
	public function LoadLastTradeId()
	{
		if(isset($this->aaSettings["SEQUENCE"]))
		{
			wlog(get_class($this), "I Processing trades for SEQUENCE");
			wlog(get_class($this), "I Getting sequence from ". $this->aaSettings["SEQUENCE"]["NEXTID"]);
			$ids = @file_get_contents($this->aaSettings["SEQUENCE"]["NEXTID"]);
			wlog(get_class($this), "I Response was: ". $ids);
			if($ids !== FALSE && $ids[0] != "FALSE")
			{
				$ids = explode(",", $ids);
				$this->iLastTradeId = $ids[0] - 1; // Had to do it!
				$this->aAllTradeIds = $ids;
				wlog(get_class($this), "I Processing trades since rtd_trade_id=". $this->iLastTradeId);
			}
			elseif($ids === FALSE)
			{
        wlog(get_class($this), "W Couldn't fetch sequence check url");
      }
			else
			{
				wlog(get_class($this), "I All trades are in sequence");
			}
		}
	}

   public function SendRequests()
   {
      $this->oApi = new CSocketStream($this->aaSettings["RTDHOST"]["IP"], $this->aaSettings["RTDHOST"]["PORT"]);
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
         if(in_array($this->oTrades->GetTradeID(), $this->aAllTradeIds))
         {
	         $this->oTrades->ProcessResponse();
	         unset($this->aAllTradeIds[array_search($this->oTrades->GetTradeID(),$this->aAllTradeIds)]);
	       }
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
        $oAnswer = new CMessageRTDAnswer();
        $oAnswer->DecodeResponse($message);
        unset($oAnswer);
		  default:
        break;
		}
   }


   public function GetResponse()
   {
      wlog(get_class($this), "I Requests sent. Reading responses...");

      $reply = "";
      // MAIN PROGRAM LOOP
      while (count($this->aAllTradeIds) > 0)
      { // append to data left from previous request
        $reply .= $this->oApi->readRequestBinary();
        // if the last character isn't the EOT character, then this contains a part response
        //print ".";
        if ($reply[strlen($reply)-1] != chr(10))
  			{
          $nlpos = strrpos($reply, chr(10));
          if ($nlpos !== false)
				  {
            // has a full response plus a part response
            // process the full response sub string
            $aResponses = explode(chr(10), trim(substr($reply, 0, $nlpos + 1)));
            $reply = substr($reply, $nlpos + 1); // keep the part response
          }
            // else it is only part of another response so leave $reply set
        }
        else
        {
          // is simply a full response, so process it
          $aResponses = explode(chr(10), trim($reply));
          $reply = "";
        }
         // now parse each message one at a time
	      foreach($aResponses AS $response)
  			{
  				$rid = substr($response, 0, strpos($response, chr(31)));
  				$response = substr($response, strpos($response, chr(31))+1);
  				$this->ParseResponse($rid, $response);
  			}
      } // while
   }

} // end class

?>
