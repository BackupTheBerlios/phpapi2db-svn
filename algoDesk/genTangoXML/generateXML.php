<?php
// Generates a strategy to collect tick of the top 300 stocks and futures.
// Getting the data from vdm44algo1 quicktick database
// Benn Eichhorn
// 31 May 2007
set_time_limit(0);
error_reporting(E_ALL);
include_once(dirname($_SERVER['PHP_SELF']) . "/inc.settings.php");
include_once(dirname($_SERVER['PHP_SELF']) . "/class.dbMAS.php");
?>
<?php

$db = new dbMAS();
$query = <<<EOT
SELECT symbol, AVG(lastQty) AS lastQty
FROM (SELECT symbol, CONVERT(varchar, tickTime, 112) AS tickDate, AVG(lastQty) AS lastQty
	FROM ticks_0706
	WHERE (symbol IN
		(SELECT TOP (100) symbol
		FROM ticks_0706 AS ticks_0706_1
		WHERE (exchangeId = 570)
		GROUP BY symbol
		ORDER BY COUNT(*) DESC))
	GROUP BY symbol, CONVERT(varchar, tickTime, 112)) AS DERIVEDTBL
GROUP BY symbol
EOT;
$symbols = $db->fetchArray($query);
$i = 0;
foreach($symbols as $symbol)
{
	$maxQty = 5 * $symbol["lastQty"];
	$strSymbol = str_replace(Array(" ","+","&"),"_",$symbol["symbol"]);
print <<<EOT
  <strategyConfiguration name="BBCv0.4-1m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG006"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">1m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-2m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG007"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">2m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-10m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG008"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">10m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-15m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG009"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">15m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-30m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG010"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">30m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-60m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG011"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">60m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-90m-{$strSymbol}">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG012"</variableDefinition>
    <variableDefinition name="tStock">'LSE {$symbol["symbol"]}'</variableDefinition>
    <variableDefinition name="tdTimeBlock">90m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">{$symbol["lastQty"]}</variableDefinition>
    <variableDefinition name="nMaxQuantity">{$maxQty}</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
EOT;
}
print <<<EOT
  <strategyConfiguration name="BBCv0.4-1m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG006"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">1m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-2m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG007"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">2m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-10m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG008"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">10m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-15m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG009"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">15m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-30m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG010"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">30m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-60m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG011"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">60m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
  <strategyConfiguration name="BBCv0.4-90m-FTSE100">
    <strategyTypeName>BollyBandCross-V0.4</strategyTypeName>
    <simuContext>BollyTest01</simuContext>
    <variableDefinition name="sAccount">"P ALG012"</variableDefinition>
    <variableDefinition name="tStock">'LFF FTSE100 0709'</variableDefinition>
    <variableDefinition name="tdTimeBlock">90m</variableDefinition>
    <variableDefinition name="nWMAOuterWeight">30</variableDefinition>
    <variableDefinition name="nWMAInnerWeight">9</variableDefinition>
    <variableDefinition name="nBollingerOuterPeriods">20</variableDefinition>
    <variableDefinition name="nBollingerOuterSigma">2</variableDefinition>
    <variableDefinition name="nBollingerInnerPeriods">8</variableDefinition>
    <variableDefinition name="nBollingerInnerSigma">1.8</variableDefinition>
    <variableDefinition name="nQuantity">4</variableDefinition>
    <variableDefinition name="nMaxQuantity">20</variableDefinition>
    <variableDefinition name="nSignalTollerance">0</variableDefinition>
  </strategyConfiguration>
EOT;
/*
$query = "SELECT TOP (100) symbol FROM ticks_0706 WHERE (exchangeId = 570) GROUP BY symbol ORDER BY COUNT(*) DESC";
$symbols = $db->fetchArray($query);
$i = 0;
foreach($symbols as $symbol)
{
	print "var tTradable" . str_pad(++$i, 3, "0", STR_PAD_LEFT) . " = 'LSE ". $symbol["symbol"] ."';\n";
}
*/
/*
for($i = 1; $i <= 102; $i++)
{
	print "var tpLastTick" . str_pad($i, 3, "0", STR_PAD_LEFT) . " := tickTimeBefore(tTradable".str_pad($i, 3, "0", STR_PAD_LEFT).", 23:59:59);\n";
}
for($i = 1; $i <= 102; $i++)
{
	print "var nLastClose" . str_pad($i, 3, "0", STR_PAD_LEFT) . " := tickClose(tTradable".str_pad($i, 3, "0", STR_PAD_LEFT).", "
		."tpLastTick" . str_pad($i, 3, "0", STR_PAD_LEFT) . ", tpLastTick" . str_pad($i, 3, "0", STR_PAD_LEFT) . " + 1s);\n";
}*/
/*
for($i = 1; $i <= 102; $i++)
{
	print '<orderAgentDefinition name="Agent'.str_pad($i, 3, "0", STR_PAD_LEFT).'">' ."\n";
	print "<varDefCode></varDefCode>\n";
	print "<cnd>false</cnd>\n";
	print "<trd>tTradable".str_pad($i, 3, "0", STR_PAD_LEFT)."</trd>\n";
	print "<acc>sACCOUNT</acc>\n";
	print "<buy>true</buy>\n";
	print "<qty>nQty".str_pad($i, 3, "0", STR_PAD_LEFT)."</qty>\n";
	print "<lmt>nPrice".str_pad($i, 3, "0", STR_PAD_LEFT)."</lmt>\n";
	print "</orderAgentDefinition>\n";
}
*/
exit;
/*
$query = "SELECT symbol FROM ticks_0706 WHERE (exchangeId = 613) AND (symbol = 'FTSE100 0706') GROUP BY symbol ORDER BY COUNT(*) DESC";
$symbols = $db->fetchArray($query);
foreach($symbols as $symbol)
{
?>
<orderAgentDefinition name="LFF_<?php echo str_replace(Array(" ","+","&"),"_",$symbol["symbol"]); ?>">
<varDefCode></varDefCode>
<cnd>false</cnd>
<trd>'LFF <?php echo $symbol["symbol"]; ?>'</trd>
<acc>sACCOUNT</acc>
<buy>true</buy>
<qty>0</qty>
<lmt>mLastPrice('LFF <?php echo $symbol["symbol"]; ?>')</lmt>
</orderAgentDefinition>
<?php
}
unset($db);
*/
?>
