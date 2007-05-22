<?php

/*////////////////////////////
	class.rtd2db.php

	This is the main execution class


TODO put a list of ini settings required for RTD Trade here


	Benn Eichhorn
	30 Jan 2007
*/// //////////////////////////

class CRequestRTDPrices extends CRequestRTD {

   public $oContracts; // holds all data for every symbol. Key = fid_contract_id
   public $oPrices;

   function __construct(Array &$settings)
   {
   	$this->aaSettings = $settings;

      wLog(get_class($this), " \n--- Loading ---");

		// Check DB Clear settings
		if(!isset($this->aaSettings["DB"]["HOST"], $this->aaSettings["DB"]["USERNAME"], $this->aaSettings["DB"]["PASSWORD"]))
		{
			wlog(get_class($this), "E  Arggghhhh DB settings not found in INI file!!! Blowing up ungracefully...");
			exit();
		}
      $this->oDatabase = new CDbMssql($this->aaSettings["DB"]["HOST"], $this->aaSettings["DB"]["USERNAME"], $this->aaSettings["DB"]["PASSWORD"]);

		// Check TRADE (P&L) settings
		if(!isset($this->aaSettings["RTDHOST"]["IP"],	$this->aaSettings["RTDHOST"]["PORT"]))
		{
			wlog(get_class($this), "E  Arggghhhh RTDHOST settings not found in INI file!!! Blowing up ungracefully...");
			exit();
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

      // Pass a refernce of this to the trade object
      $this->oPrices = new CMessageRTDPrices($this);

      wLog(get_class($this), "--- Initialised ---");
	}


   public function SendRequests()
   {
	   // Get the list of queries to send to the API
      $requests[] = $this->oContracts->GetRequest();
      $requests[] = $this->oPrices->GetRequest();

      // Send the queries to the API for processing
      $this->oApi->sendRequests($requests);
   }

   protected function ParseResponse($rid, &$message)
   {
   	//print $this->aRequestTypes[$rid] . "\n";
		switch($this->aRequestTypes[$rid])
		{
      case "rid_price_t":
         $contractType = $this->oPrices->DecodeResponse($message);
         if(array_key_exists($contractType, array_keys($this->aaSettings["CONTRACT_TYPES"])))
         	$this->oPrices->ProcessResponse();
         break;
      // stocks, futures, currencies, options etc
      case "rid_contract_t":
         $this->oContracts->DecodeResponse($message);
         break;
      // Response header
      case "rid_answer_t":
      	break;
		default:
			break;
		}
   }

} // end class

?>