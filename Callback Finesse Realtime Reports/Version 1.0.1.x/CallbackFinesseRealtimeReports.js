//I like spaces!!!!!!!!
// I don't
String.prototype.includes = function(value)
{
	return this.indexOf(value) > -1;
}

Array.prototype.includes = function(value)
{
	clientLogs.log("Include test : " + value);
	return this.indexOf(value) > -1;
}
	
var finesse = finesse || {};
finesse.gadget = finesse.gadget || {};
finesse.container = finesse.container || {};
clientLogs = finesse.cslogger.ClientLogger || {};  // for logging

var REALTIME_REFRESH_INTERVAL_BASE = 5000;
var REALTIME_REFRESH_INTERVAL_RNDADD = 2500;

var userExtension = "";
var userTeam = "";
var userTeamId = "";
var userId = "";
var bHasASupervisorRole = false;
var Authorization = "";
var Host = "";
var HostPort = "";
var ManagedTeams = [];

var aMonitoredCSQs = null;
var refreshDataInterval = null;

var sCallbackServer = 'http://10.32.9.124:9000/callbackmanagement';
var timeRequestSent; // rjm 4/14/2020

function Initialize()
{
	console.log("CallbackFinesseRealtimeReports.Initialize(): Enter");

	aMonitoredCSQs = null;

	if(bHasASupervisorRole)
	{
		refreshDataInterval = setInterval(RefreshCallbackRealtimeData,(REALTIME_REFRESH_INTERVAL_BASE + Math.floor(Math.random() * REALTIME_REFRESH_INTERVAL_RNDADD)));
	}
	else
	{
		refreshDataInterval = setInterval(RefreshCallbackRealtimeData,(REALTIME_REFRESH_INTERVAL_BASE + Math.floor(Math.random() * REALTIME_REFRESH_INTERVAL_RNDADD)));
	}

	console.log("CallbackFinesseRealtimeReports.Initialize(): Exit");
}

function RefreshCallbackRealtimeData()
{
	console.log("CallbackFinesseRealtimeReports.RefreshCallbackRealtimeData(): Enter");

	//if ( typeof(refreshDataInterval) !== "undefined" && refreshDataInterval !== null ) 
	//{
	//	clearInterval(refreshDataInterval);
	//}

	//var url = sCallbackServer + '?operation=getrecords';				// rjm 4/14/2020
	var url = sCallbackServer + '?operation=getrecordsbycsq';		// rjm 4/14/2020
	timeRequestSent = performance.now(); 										// rjm 4/14/2020
	
	makeRequest(url
		,{
			method: 'GET'
		}
		,{
			success: handleResponseSuccess_GetRecords,
			error: handleResponseError_GetRecords,
		});


	console.log("CallbackFinesseRealtimeReports.RefreshCallbackRealtimeData(): Exit");
}

/** @namespace */
finesse.modules = finesse.modules || {};

/**
 * Make a REST API request and send the response to the success or error
 * handler depending on the HTTP status code
 *
 * @param {String} url
 *     The unencoded URL to which the request is sent (will be encoded)
 * @param {Object} options
 *     An object containing additional options for the request.
 *
 *     {String} options.method
 *        The type of request (e.g. GET, PUT, POST, DELETE)
 *     {String} options.authorization
 *        [Optional] The authorization string for the request
 *     {String} options.contentType
 *        [Optional] The Content-Type of the request (e.g. application/json,
 *                   application/xml)
 *     {String} options.content
 *        A string to send in the content body of the request.
 * @param {Object} handlers
 *     An object containing the success and error handlers.
 *
 *     {Function} handlers.success(response)
 *        A callback function to be invoked for a successful request.
 *     {Function} handlers.error(response)
 *        A callback function to be invoked for an unsuccessful request.
 */
function makeRequest(url, options, handlers) 
{
	var params, uuid;

	clientLogs.log("CallbackFinesseRealtimeReports.makeRequest()");
	
	// Protect against null dereferencing of options & handlers allowing its (nonexistant) keys to be read as undefined
	params = {};
	options = options || {};

	handlers.success = _util.validateHandler(handlers.success);
	handlers.error = _util.validateHandler(handlers.error);

	// Request Headers
	params[gadgets.io.RequestParameters.HEADERS] = {};

	// HTTP method is a passthrough to gadgets.io.makeRequest
	params[gadgets.io.RequestParameters.METHOD] = options.method;

	if (options.method === "GET") 
	{
		// Disable caching for GETs
		if (url.indexOf("?") > -1) 
		{
			url += "&";
		} else 
		{
			url += "?";
		}
		
		url += "nocache=" + _util.currentTimeMillis();
	} 
	else 
	{
		// Generate a requestID and add it to the headers
		uuid = _util.generateUUID();
		params[gadgets.io.RequestParameters.HEADERS].requestId = uuid;
		params[gadgets.io.RequestParameters.GET_FULL_HEADERS] = "true";
	}
	
	// Add authorization to the request header if provided
	if(options.authorization) 
	{
		params[gadgets.io.RequestParameters.HEADERS].Authorization = options.authorization;

		clientLogs.log("CallbackFinesseRealtimeReports.makeRequest(): options.authorization: " + options.authorization);
	}

	// Add content type & body if content body is provided
	if (options.content) 
	{
		// Content Type
		params[gadgets.io.RequestParameters.HEADERS]["Content-Type"] = options.contentType;
		// Content
		params[gadgets.io.RequestParameters.POST_DATA] = options.content;
	}

	// Call the gadgets.io.makereqest function with the encoded url
	clientLogs.log("CallbackFinesseRealtimeReports.makeRequest(): Making a REST API request to: " + url);
	
	gadgets.io.makeRequest(encodeURI(url), handleWebRequestResponse(handlers), params);
}

/**
 * Handler for the response of the REST API request. This function determines if
 * the success or error handler should be called based on HTTP status code.
 *
 * @param {Object} handlers
 *     An object containing the success and error handlers.
 *
 *     {Function} handlers.success(response)
 *        A callback function to be invoked for a successful request.
 *     {Function} handlers.error(response)
 *        A callback function to be invoked for an unsuccessful request.
 */
function handleWebRequestResponse(handlers) 
{
	return function (response) 
	{
		clientLogs.log("CallbackFinesseRealtimeReports.handleWebRequestResponse(): The response status code is: " + response.rc);
		
		// Send the response to the success handler if the http status
		// code is 200 - 299. Send the response to the error handler
		// otherwise.
		if (response.rc >= 200 && response.rc < 300 && handlers.success) 
		{
			clientLogs.log("CallbackFinesseRealtimeReports.handleWebRequestResponse(): Got a successful response.");
			handlers.success(response);
		} 
		else if (handlers.error) 
		{
			clientLogs.log("CallbackFinesseRealtimeReports.handleWebRequestResponse(): Got a failure response.");
			handlers.error(response);
		} 
		else 
		{
			clientLogs.log("CallbackFinesseRealtimeReports.handleWebRequestResponse(): Missing the success and/or error handler.");
		}
	};
}

function handleResponseSuccess_GetRecords(response) 
{
	var timeRequestRx = performance.now(); 							// rjm 4/14/2020
	var timeRoundTrip = timeRequestRx - timeRequestSent;	// rjm 4/14/2020
	timeRoundTrip = timeRoundTrip.toFixed(0);							// rjm 4/14/2020
	clientLogs.log("handleResponseSuccess_GetRecords(): Enter. Call took " + timeRoundTrip + " mS to complete");
	clientLogs.log("getrecordsbycsq response : " + response.text);
	
	var parser = new DOMParser();
	var xml = parser.parseFromString(response.text, 'text/xml');
	
	var records = xml.getElementsByTagName("record");
	
	UpdateUI(records);

	records = null;

	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetRecords(): Exit");
}

function handleResponseError_GetRecords(response) 
{
	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetRecords(): Enter");
	
	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetRecords(): Received code " + response.rc);

	// window.alert("handleResponseError_GetRecords error: Received code " + response.rc);

	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetRecords(): Exit");
}

function handleResponseSuccess_GetCSQsForCurrentTeam(response) 
{
	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): Enter");

	var parser = new DOMParser();
	var xml = parser.parseFromString(response.text, 'text/xml');
	
	var csqs = xml.getElementsByTagName("csq");
	
	if ( typeof(csqs) !== "undefined" && csqs !== null ) 
	{
		clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): # of CSQs:" + csqs.length);

		if(csqs.length != 0)
		{
			aMonitoredCSQs = null;
			aMonitoredCSQs = [];

			var i = 0;
			for(i = 0; i < csqs.length; i++)
			{
				clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): CSQ name:" + csqs[i].getAttribute('name'));
				aMonitoredCSQs.push(csqs[i].getAttribute('name'));
			}

			clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): # of monitored CSQs:" + aMonitoredCSQs.length);
		}
		else
		{
			aMonitoredCSQs = null;
		}
	}
	else
	{
		clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): No csqs in this team.");
		aMonitoredCSQs = null;
	}

	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseSuccess_GetCSQsForCurrentTeam(): Exit");
}

function handleResponseError_GetCSQsForCurrentTeam(response) 
{
	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetCSQsForCurrentTeam(): Enter");

	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetCSQsForCurrentTeam(): Received code " + response.rc);

	// window.alert("handleResponseError_GetCSQsForCurrentTeam error: Received code " + response.rc);

	clientLogs.log("CallbackFinesseRealtimeReports.handleResponseError_GetCSQsForCurrentTeam(): Exit");
}

/* Workers*/
function ResetUI()
{
	clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): Enter");

	document.getElementById("supervisedteamsselector").innerHTML = '';
	
	if(bHasASupervisorRole)
	{
		var bUserTeamIsSupervised = false;

		if(ManagedTeams != null)
		{
			clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): ManagedTeams is not null");

			if(ManagedTeams.length != 0)
			{
				clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): ManagedTeams is not empty");

				var teamsselector = document.getElementById("supervisedteamsselector");
				var option = null;

				var i = 0;

				for(i = 0; i < ManagedTeams.length; i++)
				{
					if(ManagedTeams[i].name == userTeam)
					{
						bUserTeamIsSupervised = true;
					}

					option = document.createElement("option");
					option.text = ManagedTeams[i].name;
					option.value = ManagedTeams[i].id;
					teamsselector.add(option);
					option = null;
				}
			}
			else
			{
				clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): ManagedTeams is empty");

				var teamsselector = document.getElementById("supervisedteamsselector");
				var option = document.createElement("option");
				option.text = userTeam;
				option.value = userTeamId;
				teamsselector.add(option);
				option = null;
				teamsselector = null;
			}
		}
		else
		{
			clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): ManagedTeams is null");
		}

		if(!bUserTeamIsSupervised)
		{
			clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): This user is not the supervisor of its own team!");

			var teamsselector = document.getElementById("supervisedteamsselector");
			var option = document.createElement("option");
			option.text = userTeam;
			option.value = userTeamId;
			teamsselector.add(option);
			option = null;
			teamsselector = null;
		}
		else
		{
			clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): User supervises his team");
		}
	}
	else
	{
		var teamsselector = document.getElementById("supervisedteamsselector");
		var option = document.createElement("option");
		option.text = userTeam;
		option.value = userTeamId;
		teamsselector.add(option);
		option = null;
		teamsselector = null;
	}

	var options = document.getElementById("supervisedteamsselector").options;

	var i = 0;

	for(i = 0; i < options.length; i++)
	{
		if(options[i].text == userTeam)
		{
			options[i].selected = true;
		}
	}

	options = null;

	clientLogs.log("CallbackFinesseRealtimeReports.ResetUI(): Exit");
}

function UpdateUI(records)
{
	clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): Enter");

	if ( typeof(records) !== "undefined" && records !== null ) 
	{
		if(records.length != 0)
		{
			clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): # of records:" + records.length);
		}
		else
		{
			clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): records is empty");
			
			UpdateUIComponents(0,0,0);

			return;
		}
	}
	else
	{
		clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): records is either undefined or null");
		
		UpdateUIComponents(0,0,0);

		return;
	}

	if (aMonitoredCSQs !== null )
	{

	}
	else
	{
		clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): aMonitoredCSQs is null");
		//
		return;
	}

	//{PAULO} I think your changes start here.....
	var i = 0;
	var j = 0;
	var OldestWaitingDuration = 0;
	var AvgWaitingDuration = 0;
	var iTotalWaitingDuration = 0;
	var TotalContactsWaiting = 0;	
//	var aValidatedRecords = [];
	
	// rjm - 4/14/2020 - Start
	for(j = 0; j < records.length; j++)
	{
		var sCallbackCSQName = records[j].getElementsByTagName("csqname")[0].childNodes[0].nodeValue;
		if(aMonitoredCSQs.includes(sCallbackCSQName))
		{
			var sOldestContactWaiting = records[j].getElementsByTagName("oldestcontactwaiting")[0].childNodes[0].nodeValue;
			var iOldestContactWaiting = parseInt(sOldestContactWaiting,10);
			var sContactsWaiting = records[j].getElementsByTagName("contactswaiting")[0].childNodes[0].nodeValue;
			var iContactsWaiting = parseInt(sContactsWaiting,10); // rjm 5/12/2020 - use iOldestContactWaiting instead of sOldestContactWaiting
			var sAvgContactWaitingDuration = records[j].getElementsByTagName("avgcontactwaiting")[0].childNodes[0].nodeValue;
			var iAvgContactWaitingDuration = parseInt(sAvgContactWaitingDuration);
			TotalContactsWaiting = TotalContactsWaiting + iContactsWaiting;
			iTotalWaitingDuration += (iAvgContactWaitingDuration * iContactsWaiting); // rjm 5/12/2020 - use iContactsWaiting instead of sContactsWaiting
			if(iOldestContactWaiting > OldestWaitingDuration)
			{
				OldestWaitingDuration = iOldestContactWaiting; // rjm 5/12/2020 - use iOldestContactWaiting instead of sOldestContactWaiting
			}
		}		
	}//for(j = 0; j < records.length; j++)

	if(TotalContactsWaiting == 0)
	{
		AvgWaitingDuration = 0;
	}
	else
	{
		AvgWaitingDuration = iTotalWaitingDuration/TotalContactsWaiting;
	}
	AvgWaitingDuration = AvgWaitingDuration.toFixed(0);
	clientLogs.log("AvgWait : " + AvgWaitingDuration);
	// rjm - 4/14/2020 - End
	
	/*
	for(i = 0; i < aMonitoredCSQs.length; i++)
	{
		for(j = 0; j < records.length; j++)
		{
			var sCallbackCSQName = records[j].getElementsByTagName("targetcsq")[0].childNodes[0].nodeValue;
			var sCallbackState = records[j].getElementsByTagName("status")[0].childNodes[0].nodeValue;

			//if(sCallbackState == "NEW" )
			//{
				if(aMonitoredCSQs[i] == sCallbackCSQName)
				{
					var msec = Date.parse(records[j].getElementsByTagName("requestdate")[0].childNodes[0].nodeValue);

					aValidatedRecords.push((Date.now() - msec) / 1000);
				}
			//}

		}//for(j = 0; j < records.length; j++)

	}//for(i = 0; i < aMonitoredCSQs.length; i++)

	//Find OldestWaitingDuration and AvgWaitingDuration

	var OldestWaitingDuration = 0;
	var AvgWaitingDuration = 0;

	if(aValidatedRecords.length == 0)
	{
		OldestWaitingDuration = 0;
		AvgWaitingDuration = 0;
	}
	else if(aValidatedRecords.length == 1)
	{
		OldestWaitingDuration = parseInt(aValidatedRecords[0]);
		AvgWaitingDuration = parseInt(aValidatedRecords[0]);
	}
	else
	{
		var tmp = parseInt(aValidatedRecords[0]);

		var i = 0;
		for(i = 1; i < aValidatedRecords.length; i++)
		{
			AvgWaitingDuration = AvgWaitingDuration + aValidatedRecords[i];

			if(parseInt(aValidatedRecords[i]) > tmp)
			{
				tmp = parseInt(aValidatedRecords[i]);
			}
		}

		AvgWaitingDuration = Math.floor(AvgWaitingDuration / aValidatedRecords.length);

		OldestWaitingDuration = tmp;
	}
	*/
	//{PAULO} Until here.....

	//UpdateUIComponents(aValidatedRecords.length,OldestWaitingDuration,AvgWaitingDuration);	// rjm - 4/14/2020 
	UpdateUIComponents(TotalContactsWaiting,OldestWaitingDuration,AvgWaitingDuration); 				// rjm - 4/14/2020 

	clientLogs.log("CallbackFinesseRealtimeReports.UpdateUI(): Exit");
}

function UpdateUIComponents(TotalRecordsWaiting, OldestCallbackWaitingDuration,AvgCallbackWaitingDuration)
{
	clientLogs.log("CallbackFinesseRealtimeReports.UpdateUIComponents(): Enter");

	//TotalRecordsWaiting
	document.getElementById("TotalWaitingValue").innerHTML = TotalRecordsWaiting;

	//OldestCallbackWaitingDuration
	document.getElementById("OldestWaitingDuration").innerHTML = FormatHHMMSS(OldestCallbackWaitingDuration);

	//AvgCallbackWaitingDuration
	document.getElementById("AvgWaitingDuration").innerHTML = FormatHHMMSS(AvgCallbackWaitingDuration);

	clientLogs.log("CallbackFinesseRealtimeReports.UpdateUIComponents(): Exit");
}

function FormatHHMMSS(value)
{
	clientLogs.log("CallbackFinesseRealtimeReports.FormatHHMMSS(): Enter");

	var sminutes = '';
	var sseconds = '';

	if(value <= 120)
	{
		return value + ' secs';
	}
	else if(value > 120 && value < 3600)
	{
		var minutes = Math.floor(value / 60);

		var seconds = value - (minutes * 60);

		if(minutes < 10)
		{
			sminutes = '0' + minutes;
		}
		else
		{
			sminutes = minutes;
		}

		if(seconds < 10)
		{
			sseconds = '0' + seconds;
		}
		else
		{
			sseconds = seconds;
		}

		return '00:' + sminutes + ':' + sseconds;
	}
	else
	{
		var hours = Math.floor(value / 3600);

		value = value - (hours * 3600);

		var minutes = Math.floor(value / 60);

		var seconds = value - (minutes * 60);

		var shours = '';		

		if(hours < 10)
		{
			shours = '0' + hours;
		}
		else
		{
			shours = hours;
		}

		if(minutes < 10)
		{
			sminutes = '0' + minutes;
		}
		else
		{
			sminutes = minutes;
		}

		if(seconds < 10)
		{
			sseconds = '0' + seconds;
		}
		else
		{
			sseconds = seconds;
		}

		return shours + ':' + sminutes + ':' + sseconds;
	}

	clientLogs.log("CallbackFinesseRealtimeReports.FormatHHMMSS(): Exit");
}

function GetCSQsForCurrentTeam()
{
	clientLogs.log("CallbackFinesseRealtimeReports.GetCSQsForCurrentTeam(): Enter");

	var teamsselector = document.getElementById("supervisedteamsselector");

	var selectedTeamId = teamsselector.options[teamsselector.selectedIndex].value;

	clientLogs.log("CallbackFinesseRealtimeReports.GetCSQsForCurrentTeam(): selectedTeamId:" + selectedTeamId);

	var url = 'http://' + Host + ':9080/uccxadminapiproxy?operation=getteaminfo&token=' + Authorization + '&teamid=' + selectedTeamId;

	makeRequest(url
		,{
			method: 'GET'
		}
		,{
			success: handleResponseSuccess_GetCSQsForCurrentTeam,
			error: handleResponseError_GetCSQsForCurrentTeam
		});


	clientLogs.log("CallbackFinesseRealtimeReports.GetCSQsForCurrentTeam(): Exit");
}

/* ONCHANGE events*/
function supervisedteamsselector_onchange()
{
	clientLogs.log("CallbackFinesseRealtimeReports.supervisedteamsselector_onchange(): Enter");

	UpdateUIComponents(0,0,0);

	GetCSQsForCurrentTeam();

	RefreshCallbackRealtimeData();

	clientLogs.log("CallbackFinesseRealtimeReports.supervisedteamsselector_onchange(): Exit");
}

/* ONCLICK events*/
function btnRefreshButton_onClick()
{
	console.log("CallbackFinesseRealtimeReports.btnRefreshButton_onClick(): Enter");

	//var url = sCallbackServer + '?operation=getrecords'; 				// rjm 4/14/2020
	var url = sCallbackServer + '?operation=getrecordsbycsq';		// rjm 4/14/2020
	timeRequestSent = performance.now(); 										// rjm 4/14/2020
	
	makeRequest(url
		,{
			method: 'GET'
		}
		,{
			success: handleResponseSuccess_GetRecords,
			error: handleResponseError_GetRecords,
		});

	console.log("CallbackFinesseRealtimeReports.btnRefreshButton_onClick(): Exit");
}

finesse.modules.CallbackFinesseRealtimeReports = (function ($) {
    var user, dialogs, clientlogs,

    /**
     *  Handler for additions to the Dialogs collection object. This will occur when a new
     *  Dialog is created on the Finesse server for this user.
     */
    handleNewDialog = function(dialog) 
	{
        clientLogs.log("CallbackFinesseRealtimeReports.handleNewDialog(): Enter");
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleNewDialog(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress() + " queueName = " + dialog.getMediaProperties().queueName);
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleNewDialog(): Number of dialogs: " + user.getDialogs().length);

        // add a handler to be called when the dialog object changes
        dialog.addHandler('change', _processCall);
		
		gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleNewDialog(): Exit");
    },
    
	/**
     * Handle when the dialog object is updated/changed
     */
    _processCall = function (dialog) 
	{
        clientLogs.log("CallbackFinesseRealtimeReports._processCall(): Enter");
		
		clientLogs.log("CallbackFinesseRealtimeReports._processCall(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress() + " queueName = " + dialog.getMediaProperties().queueName);
		
		gadgets.window.adjustHeight();

		clientLogs.log("CallbackFinesseRealtimeReports._processCall(): Exit");
    },
	
    /**
     *  Handler for deletions from the Dialogs collection object for this user. This will occur
     *  when a Dialog is removed from this user's collection (example, end call)
     */
    handleEndDialog = function(dialog)
	{
        clientLogs.log("CallbackFinesseRealtimeReports.handleEndDialog(): Enter");
        
		clientLogs.log("CallbackFinesseRealtimeReports.handleEndDialog(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress());

        gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleEndDialog(): Exit");
    },

    /**
     * Handler for the onLoad of a User object. This occurs when the User object is initially read
     * from the Finesse server. Any once only initialization should be done within this function.
     */
    handleUserLoad = function (userevent) 
	{
        clientLogs.log("CallbackFinesseRealtimeReports.handleUserLoad(): Enter");
		
        // Get an instance of the dialogs collection and register handlers for dialog additions and
        // removals
        dialogs = user.getDialogs( {
            onCollectionAdd : handleNewDialog,
            onCollectionDelete : handleEndDialog
        });
        
		userExtension = user.getExtension();
		userTeam = user.getTeamName();
		userTeamId = user.getTeamId();
		userId = user.getId();
		bHasASupervisorRole = user.hasSupervisorRole();
		ManagedTeams = user.getSupervisedTeams();

		clientLogs.log("CallbackFinesseRealtimeReports.handleUserLoad(): Extension:" + userExtension + " Team:" + userTeam + " TeamID:" + user.getTeamId()  + " Id:" + userId + " HasASupervisorRole:" + bHasASupervisorRole);
		
		ResetUI();

		GetCSQsForCurrentTeam();

		Initialize();
		
        gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleUserLoad(): Exit");
    },
	
    /**
     *  Handler for all User updates
     */
    handleUserChange = function(userevent) 
	{
        clientLogs.log("CallbackFinesseRealtimeReports.handleUserChange(): Enter");
		
		userExtension = user.getExtension();
		userTeam = user.getTeamName();
		userTeamId = user.getTeamId();
		userId = user.getId();
		bHasASupervisorRole = user.hasSupervisorRole();
		ManagedTeams = user.getSupervisedTeams();

		clientLogs.log("CallbackFinesseRealtimeReports.handleUserLoad(): Extension:" + userExtension + " Team:" + userTeam + " TeamID:" + user.getTeamId() + " Id:" + userId + " IsUserASupervisor:" + bHasASupervisorRole);
		
		ResetUI();

		GetCSQsForCurrentTeam();

		Initialize();

		gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseRealtimeReports.handleUserChange(): Exit");
    };

    /** @scope finesse.modules.CustomerJourney */
    return {
        /**
         * Performs all initialization for this gadget
         */
        init : function () {
            var cfg = finesse.gadget.Config;
            _util = finesse.utilities.Utilities;

            clientLogs = finesse.cslogger.ClientLogger;  // declare clientLogs

            // Initiate the ClientServices and load the user object. ClientServices are
            // initialized with a reference to the current configuration.
            finesse.clientservices.ClientServices.init(cfg, false);

            // Initiate the ClientLogs. The gadget id will be logged as a part of the message
            clientLogs.init(gadgets.Hub, "Callback Finesse Realtime Reports");

            // Create a user object for this user (Call GET User)
            user = new finesse.restservices.User({
                id: cfg.id, 
                onLoad : handleUserLoad,
                onChange : handleUserChange
            });
            
            // Initiate the ContainerServices and add a handler for when the tab is visible
            // to adjust the height of this gadget in case the tab was not visible
            // when the html was rendered (adjustHeight only works when tab is visible)
			containerServices = finesse.containerservices.ContainerServices.init();
			
            containerServices.addHandler(finesse.containerservices.ContainerServices.Topics.ACTIVE_TAB, function() {
                clientLogs.log("Gadget is now visible");  // log to Finesse logger
                // Automatically adjust the height of the gadget to show the html
                gadgets.window.adjustHeight();
            });
			
            containerServices.makeActiveTabReq();
			
			Authorization = cfg.authorization;
			Host = cfg.host;
			HostPort = cfg.hostPort;

			var sVersion = "";
			
			sVersion = " [DEV Version: 1.0.1.0001]";
			
			// $("#gadgetname").text("Callback Requeue" + sVersion);
			
			clientLogs.log("CallbackFinesseRealtimeReports - " + sVersion);
        }
    };
}(jQuery));