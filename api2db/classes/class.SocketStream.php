<?php

/*////////////////////////////
	class.socket_stream.php

	Socket stream class for sending and receiving messages

	 - sends and recieves preformated request strings

	Benn Eichhorn
	20 July 2006
*/////////////////////////////

class CSocketStream
{

	// Holds the socket object for this connection
	private $socket;

	// Constructor
	function __construct($ip, $port)
	{
		$this->socket = $this->connectApi($ip, $port);
	}

	// Destructor
	function __destruct()
	{
		$this->closeApi();
	}

	/////// PRIVATE ////////////////////////////////////////////////////////////////

	// Create a socket with the rtd api
	private function connectApi($ip, $port)
	{
		$connectionAttempts = 0;
		$connectionAttemptsMax = 3;
		$connectionTimeWait = 600; // 10 minutes


		wLog(get_class($this), "TCP/IP Connection for RTD API Started");

		/* Create a TCP/IP socket. */
		$socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);
		if ($socket < 0)
		{
		   wLog(get_class($this), "Function socket_create() failed. Reason: " . socket_strerror($socket));
		   exit($socket);
		}
		else
		{
		   wLog(get_class($this), "OK! Socket created");
		}

		wLog(get_class($this), "Attempting to connect to '" . $ip . "' on port '" . $port);

		while(TRUE)
		{
			$result = socket_connect($socket, $ip, $port);
			if (FALSE)
			{
				$connectionAttempts++;
			   wLog(get_class($this), "Function socket_connect() failed $connectionAttempts of $connectionAttemptsMax attempts. Reason: ($result) " . socket_strerror($result));
			   if($connectionAttempts <= $connectionAttemptsMax)
			   	sleep($connectionTimeWait);
			   else
			   	exit($result);
			}
			else
			{
			   wLog(get_class($this), "OK! Connection up");
			   break;
			}
		}

		return $socket;
	}


	/////// PUBLIC ////////////////////////////////////////////////////////////////

	// Send format request string to api
	public function sendRequests(Array $aRequests)
	{
		$requests = implode(chr(10),$aRequests);

		//[Benn] I know it seems this is a little pointless, but it's not
		// This is because some array values may contain multiple requests,
		// doing will let us see them all.
		$aRequests = explode(chr(10),$requests);
		// Write what we are send to log
		foreach($aRequests AS $request)
		{
			wLog(get_class($this), "Sending string : $request");
		}
		$requests .= chr(10);
		$result = socket_write($this->socket, $requests, strlen($requests));

		if ($result === FALSE)
		{
			wLog(get_class($this), "Function socket_write() failed. Reason: " . socket_strerror(socket_last_error()));
			exit(socket_last_error());
		}
		else
		{
			wLog(get_class($this), "OK! Wrote $result bytes");
		}
	}

	// Wait and read on socket for some data from the api
	public function readRequestNormal()
	{
		$reply = socket_read($this->socket, 512, PHP_NORMAL_READ);

		if ($reply === FALSE)
		{
			wLog(get_class($this), "Function socket_read() failed. Reason: " . socket_strerror(socket_last_error()));
			exit(socket_last_error());
		}
		// else return the reply
		return $reply;
	}

	// Wait and read on socket for some data from the api
	public function readRequestBinary()
	{
		$reply = socket_read($this->socket, 2048);

		if ($reply === FALSE)
		{
			wLog("socket_read() failed. Reason: " . socket_strerror(socket_last_error()));
			exit(socket_last_error());
		}
		// else return the reply
		return $reply;
	}

	// Close the socket
	public function closeApi()
	{
		wLog(get_class($this), "Closing connection...");
		socket_close($this->socket);
	}

}


?>
