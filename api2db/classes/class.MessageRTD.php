<?php

/*////////////////////////////
	class.rtd2db.php

	This is the base Message class

	Benn Eichhorn
	10 May 2007
*/// //////////////////////////

abstract class CMessageRTD {
   protected $aFieldTypes = Array(1 => "fid_account_id",
      2 => "fid_account_spec1",
      3 => "fid_account_spec2",
      4 => "fid_ask_qty",
      8 => "fid_best_ask",
      9 => "fid_best_bid",
      10 => "fid_bid_qty",
      15 => "fid_context",
      17 => "fid_contract_id",
      19 => "fid_contract_type",
      20 => "fid_contracts_long",
      21 => "fid_contracts_short",
      23 => "fid_count",
      30 => "fid_currency_id",
      33 => "fid_date",
      44 => "fid_error",
      46 => "fid_exchange_country",
      47 => "fid_exchange_id",
      48 => "fid_exchange_symbol",
      50 => "fid_expiration_date",
      51 => "fid_expiration_month",
      65 => "fid_isin",
      66 => "fid_last",
      67 => "fid_last_qty",
      70 => "fid_low",
      71 => "fid_name",
      72 => "fid_open",
      73 => "fid_order_number",
      76 => "fid_price",
      78 => "fid_put_call",
      84 => "fid_settle",
      91 => "fid_symbol",
      95 => "fid_tick_size",
      96 => "fid_time",
      97 => "fid_total_volume",
      98 => "fid_trade_flags",
      99 => "fid_trade_id",
      100 => "fid_trade_number",
      108 => "fid_value",
      165 => "fid_phase",
      167 => "fid_rtd_order_id",
      172 => "fid_user_id",
      199 => "fid_flags",
      200 => "fid_id",
      203 => "fid_text",
      222 => "fid_ctrex_id",
      269 => "fid_exchange_contract_id",
      271 => "fid_sedol", // aka 'fid_local_security_number' in the RTD API
      370 => "fid_close");

   abstract public function GetRequest();

   abstract public function DecodeResponse(&$sMessage);
}

?>
