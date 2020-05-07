var finesse = finesse || {};
finesse.gadget = finesse.gadget || {};
finesse.container = finesse.container || {};
clientLogs = finesse.cslogger.ClientLogger || {};  // for logging

var userExtension = "";
var userTeam = "";
var userId = "";
var IsUserASupervisor = false;

var dispositionCodes;

var sCallbackServer = 'http://10.1.10.59:9000/callbackmanagement';
//var sCallbackServer = 'http://10.4.10.153:9020/callbackmanagement';

var _id = '';
var _dnis = '';
var _origincsq = '';
var _targetcsq = '';
var _prompt = '';
var _sessionid = '';
var _implid = '';
var _contactid = '';
var _language = '';
var _queuestarttime = 0;
var _delay = 0;
var _customvar1 = '';
var _customvar2 = '';
var _customvar3 = '';
var _customvar4 = '';
var _customvar5 = '';
var _requeuecounter = '';
var _reqid = '';

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

	clientLogs.log("CallbackFinesseAgent.makeRequest()");
	
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
	clientLogs.log("CallbackFinesseAgent.makeRequest(): Making a REST API request to: " + url);
	
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
		clientLogs.log("CallbackFinesseAgent.handleWebRequestResponse(): The response status code is: " + response.rc);
		
		// Send the response to the success handler if the http status
		// code is 200 - 299. Send the response to the error handler
		// otherwise.
		if (response.rc >= 200 && response.rc < 300 && handlers.success) 
		{
			clientLogs.log("CallbackFinesseAgent.handleWebRequestResponse(): Got a successful response.");
			handlers.success(response);
		} 
		else if (handlers.error) 
		{
			clientLogs.log("CallbackFinesseAgent.handleWebRequestResponse(): Got a failure response.");
			handlers.error(response);
		} 
		else 
		{
			clientLogs.log("CallbackFinesseAgent.handleWebRequestResponse(): Missing the success and/or error handler.");
		}
	};
}

/**
 * Handler for when the REST API response has a HTTP status code >= 200 and < 300.
 *
 * @param {Object} response
 *     An object containing the HTTP response.
 */
function handleResponseSuccess_LoadDispositionCodes(response) 
{
	clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_LoadDispositionCodes(): Enter");
		
	dispositionCodes = JSON.parse(response.text);

	if(dispositionCodes !== null)
	{
		clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_LoadDispositionCodes(): Disposition Codes successfuly parsed.");
	}
	else
	{
		window.alert("CallbackFinesseAgent parsing Disposition Codes error: Received code " + response.rc);
	}
	
	clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_LoadDispositionCodes(): Exit");
}

/**
 * Handler for when the REST API response has a HTTP status code < 200 and >= 300. 
 *
 * @param {Object} response
 *     An object containing the HTTP response.
 */
function handleResponseError_LoadDispositionCodes(response) 
{
	clientLogs.log("CallbackFinesseAgent.handleResponseError_LoadDispositionCodes(): Enter");
	
	window.alert("CallbackFinesseAgent error: Received code " + response.rc);

	clientLogs.log("CallbackFinesseAgent.handleResponseError_LoadDispositionCodes(): Exit");
}

function handleResponseSuccess_AddRecord(response) 
{
	clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_AddRecord(): Enter");
		
	var parser = new DOMParser();
	var xml = parser.parseFromString(response.text, 'text/xml');
	
	var code = xml.getElementsByTagName("Code")[0].childNodes[0].nodeValue;
	
	clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_AddRecord(): code:" + code);

	if(code == 0)
	{
		ResetUI();
	}
	else
	{
		var description = xml.getElementsByTagName("Description")[0].childNodes[0].nodeValue;

		window.alert("CallbackFinesseAgent.handleResponseSuccess_AddRecord(): Failed to requeue record. Reason: " + description);
	}

	clientLogs.log("CallbackFinesseAgent.handleResponseSuccess_AddRecord(): Exit");
}

function handleResponseError_AddRecord(response) 
{
	clientLogs.log("CallbackFinesseAgent.handleResponseError_AddRecord(): Enter");
	
	window.alert("handleResponseError_AddRecord error: Received code " + response.rc);

	clientLogs.log("CallbackFinesseAgent.handleResponseError_AddRecord(): Exit");
}

function ResetUI()
{
	clientLogs.log("CallbackFinesseAgent.ResetUI(): Enter");
	
	document.getElementById("_dnisvalue").value = '';
	document.getElementById("_dnisvalue").disabled = true; 

	document.getElementById("_targetcsqvalue").innerHTML = ''; 

	document.getElementById("_attemptsvalue").innerHTML = '0'; 

	document.getElementById("_requeuecodevalue").innerHTML = ''; 
	document.getElementById("_requeuecodevalue").disabled = true;

	document.getElementById("btnRequeue").disabled = true; 

	clientLogs.log("CallbackFinesseAgent.ResetUI(): Exit");
}

function validate(evt) 
{
	clientLogs.log("CallbackFinesseAgent.validate(): Enter");

	var theEvent = evt || window.event;
  
	// Handle paste
	if (theEvent.type === 'paste') 
	{
		key = event.clipboardData.getData('text/plain');
	} 
	else 
	{
	// Handle key press
		var key = theEvent.keyCode || theEvent.which;
		key = String.fromCharCode(key);
	}
	
	var regex = /[0-9]|\./;

	if( !regex.test(key) ) 
	{
	  theEvent.returnValue = false;

	  if(theEvent.preventDefault)
	  {
		theEvent.preventDefault();
	  }
	}

	clientLogs.log("CallbackFinesseAgent.validate(): Exit");
  }

/* ONCLICK events*/

function btnFakeIncomingCall_onClick()
{
	clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): Enter");

	_id = '349875934875983475';
	_dnis = '2078858968';
	_targetcsq = 'dsb';
	_prompt = '';
	_sessionid = '345345435435';
	_implid = '';
	_contactid = '';
	_language = 'EN_US';
	_customvar1 = '';
	_customvar2 = '';
	_customvar3 = '';
	_customvar4 = '';
	_customvar5 = '';
	_reqid = '';
	_requeuecounter = '2';

	if(_id == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _id is null.");	
	}

	if(_dnis == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _dnis is null.");	
	}

	if(_targetcsq == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _targetcsq is null.");	
	}

	if(_targetcsq == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _targetcsq is null.");	
	}

	if(_prompt == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _prompt is null.");	
	}

	if(_sessionid == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _sessionid is null.");	
	}

	if(_implid == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _implid is null.");	
	}

	if(_contactid == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _contactid is null.");	
	}

	if(_language == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _language is null. Default to EN_US");
		_language = 'EN_US';
	}

	if(_customvar1 == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _customvar1 is null. _customvar1 to empty");
		_customvar1 = '';
	}

	if(_customvar2 == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _customvar2 is null. _customvar2 to empty");
		_customvar2 = '';
	}

	if(_customvar3 == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _customvar3 is null. _customvar3 to empty");
		_customvar3 = '';
	}

	if(_customvar4 == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _customvar4 is null. _customvar4 to empty");
		_customvar4 = '';
	}

	if(_customvar5 == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _customvar5 is null. _customvar5 to empty");
		_customvar5 = '';
	}

	if(_reqid == null)
	{
		clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): _language is null. _reqid to empty");
		_reqid = '';
	}

	clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): " + dispositionCodes.csqs.length);

	for (i = 0; i < dispositionCodes.csqs.length; i++)
	{
		if(dispositionCodes.csqs[i].name === _targetcsq)
		{
			for (j = 0; j < dispositionCodes.csqs[i].dispositioncodes.length; j++)
			{
				clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): Name:" + dispositionCodes.csqs[i].dispositioncodes[j].displayname);

				var option = document.createElement('option');
				option.text = dispositionCodes.csqs[i].dispositioncodes[j].displayname;
				option.value = dispositionCodes.csqs[i].dispositioncodes[j].value;

				document.getElementById("_requeuecodevalue").add(option,0);

				option = null;

			}
		}
	}


	document.getElementById("_dnisvalue").value = _dnis;
	document.getElementById("_targetcsqvalue").innerHTML = _targetcsq; 
	document.getElementById("_attemptsvalue").innerHTML = _requeuecounter; 

	document.getElementById("btnRequeue").disabled = false; 

	clientLogs.log("CallbackFinesseAgent.btnFakeIncomingCall_onClick(): Exit");
}

function btnRequeue_onClick()
{
	clientLogs.log("CallbackFinesseAgent.btnRequeue_onClick(): Enter");

	var RequeueCodes = document.getElementById("_requeuecodevalue");
	var SelectedRequeueCodeValue = RequeueCodes.options[RequeueCodes.selectedIndex].value;
	var SelectedRequeueCode = RequeueCodes.options[RequeueCodes.selectedIndex].text;

	var DNIS = document.getElementById("_dnisvalue").value;

	if(DNIS == null || typeof(DNIS) == "undefined" || DNIS.length != 10)
	{
		clientLogs.log("CallbackFinesseAgent.btnRequeue_onClick(): DNIS value is invalid.");

		window.alert("Callback DNIS value is invalid.");

		document.getElementById("_dnisvalue").focus();

		return;
	}

	url = sCallbackServer + '?operation=addrecord';

	url = url + '&id=' + _id;
	url = url + '&dnis=' + DNIS;
	url = url + '&origincsq=' + '';
	url = url + '&targetcsq=' + _targetcsq;
	url = url + '&prompt=' + _prompt;
	url = url + '&sessionid=' + _sessionid;
	url = url + '&implid=' + _implid;
	url = url + '&contactid=' + _contactid;
	url = url + '&language=' + _language;
	url = url + '&queuestarttime=' + _util.currentTimeMillis();
	url = url + '&delay=' + SelectedRequeueCodeValue;
	url = url + '&customvar1=' + _customvar1;
	url = url + '&customvar2=' + _customvar2;
	url = url + '&customvar3=' + _customvar3;
	url = url + '&customvar4=' + _customvar4;
	url = url + '&customvar5=' + _customvar5;
	url = url + '&requeuecode=' + SelectedRequeueCode;
	url = url + '&requeuecounter=' + (parseInt(_requeuecounter) + 1);

	makeRequest(url, {
					method: 'GET'
				}
				,{
					success: handleResponseSuccess_AddRecord,
					error: handleResponseError_AddRecord,
				}
			);

	clientLogs.log("CallbackFinesseAgent.btnRequeue_onClick(): Exit");
}

finesse.modules.CallbackFinesseAgent = (function ($) {
    var user, dialogs, clientlogs,

    /**
     *  Handler for additions to the Dialogs collection object. This will occur when a new
     *  Dialog is created on the Finesse server for this user.
     */
    handleNewDialog = function(dialog) 
	{
        clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Enter");
		
		clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress() + " queueName = " + dialog.getMediaProperties().queueName);
		
		clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Number of dialogs: " + user.getDialogs().length);
		
		if(user.getDialogs().length != 0)
		{
			clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Ths gadget was not designed to interact with multiple dialogs at the same time.");
			
			return;
		}
		
		if(dialog.getMediaProperties().callType == 'ACD_IN')
		{
			//Get Callback fields from Enterprise Data
			_id = dialog.getMediaProperties()["userid"];
			_dnis = dialog.getMediaProperties()["userdnis"];
			_targetcsq = dialog.getMediaProperties()["usertargetcsq"];
			_prompt = dialog.getMediaProperties()["userprompt"];
			_sessionid = dialog.getMediaProperties()["userid"];
			_implid = dialog.getMediaProperties()["userimplid"];
			_contactid = dialog.getMediaProperties()["usercontactid"];
			_language = dialog.getMediaProperties()["userlanguage"];
			_customvar1 = dialog.getMediaProperties()["usercustomvar1"];
			_customvar2 = dialog.getMediaProperties()["usercustomvar2"];
			_customvar3 = dialog.getMediaProperties()["usercustomvar3"];
			_customvar4 = dialog.getMediaProperties()["usercustomvar4"];
			_customvar5 = dialog.getMediaProperties()["usercustomvar5"];
			_reqid = dialog.getMediaProperties()["userreqid"];
			_requeuecounter = dialog.getMediaProperties()["userrequeuecounter"];

			if(_id == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _id is null.");
			}

			if(_dnis == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _dnis is null.");	
			}

			if(_targetcsq == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _targetcsq is null.");	
			}

			if(_id == null && _dnis == null && _targetcsq == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Current contact does not appera to be a callback contact.");

				return;
			}

			if(_prompt == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _prompt is null.");	
			}

			if(_sessionid == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _sessionid is null.");	
			}

			if(_implid == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _implid is null.");	
			}

			if(_contactid == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _contactid is null.");	
			}

			if(_language == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _language is null. Default to EN_US");
				_language = 'EN_US';
			}

			if(_customvar1 == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _customvar1 is null. _customvar1 to empty");
				_customvar1 = '';
			}

			if(_customvar2 == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _customvar2 is null. _customvar2 to empty");
				_customvar2 = '';
			}

			if(_customvar3 == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _customvar3 is null. _customvar3 to empty");
				_customvar3 = '';
			}

			if(_customvar4 == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _customvar4 is null. _customvar4 to empty");
				_customvar4 = '';
			}

			if(_customvar5 == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _customvar5 is null. _customvar5 to empty");
				_customvar5 = '';
			}

			if(_reqid == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _reqid is null. _reqid to empty");
				_reqid = '';
			}

			if(_requeuecounter == null)
			{
				clientLogs.log("CallbackFinesseAgent.handleNewDialog(): _requeuecounter is null. Default to 0");
				_requeuecounter = '0';
			}

			clientLogs.log("CallbackFinesseAgent.handleNewDialog(): " + dispositionCodes.csqs.length);

			for (i = 0; i < dispositionCodes.csqs.length; i++)
			{
				if(dispositionCodes.csqs[i].name === _targetcsq)
				{
					for (j = 0; j < dispositionCodes.csqs[i].dispositioncodes.length; j++)
					{
						clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Name:" + dispositionCodes.csqs[i].dispositioncodes[j].displayname);

						var option = document.createElement('option');
						option.text = dispositionCodes.csqs[i].dispositioncodes[j].displayname;
						option.value = dispositionCodes.csqs[i].dispositioncodes[j].value;

						document.getElementById("_requeuecodevalue").add(option,0);

						option = null;

					}
				}
			}

			document.getElementById("_requeuecodevalue").disabled = false;

			document.getElementById("_dnisvalue").value = _dnis;
			document.getElementById("_dnisvalue").disabled = false; 

			document.getElementById("_targetcsqvalue").innerHTML = _targetcsq;
			document.getElementById("_attemptsvalue").innerHTML = _requeuecounter;

			document.getElementById("btnRequeue").disabled = false; 
		}

        // add a handler to be called when the dialog object changes
        dialog.addHandler('change', _processCall);
		
		gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseAgent.handleNewDialog(): Exit");
    },
    
	/**
     * Handle when the dialog object is updated/changed
     */
    _processCall = function (dialog) 
	{
        clientLogs.log("CallbackFinesseAgent._processCall(): Enter");
		
		clientLogs.log("CallbackFinesseAgent._processCall(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress() + " queueName = " + dialog.getMediaProperties().queueName);
		
		clientLogs.log("CallbackFinesseAgent._processCall(): Exit");
    },
	
    /**
     *  Handler for deletions from the Dialogs collection object for this user. This will occur
     *  when a Dialog is removed from this user's collection (example, end call)
     */
    handleEndDialog = function(dialog)
	{
        clientLogs.log("CallbackFinesseAgent.handleEndDialog(): Enter");
        
		clientLogs.log("CallbackFinesseAgent.handleEndDialog(): Id = " + dialog.getId() + " calltype = " + dialog.getMediaProperties().callType + " callState = " + dialog.getState() + " toAddress = " + dialog.getToAddress() + " fromAddress = " + dialog.getFromAddress());
		
		ResetUI();

        gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseAgent.handleEndDialog(): Exit");
    },

    /**
     * Handler for the onLoad of a User object. This occurs when the User object is initially read
     * from the Finesse server. Any once only initialization should be done within this function.
     */
    handleUserLoad = function (userevent) 
	{
        clientLogs.log("CallbackFinesseAgent.handleUserLoad(): Enter");
		
        // Get an instance of the dialogs collection and register handlers for dialog additions and
        // removals
        dialogs = user.getDialogs( {
            onCollectionAdd : handleNewDialog,
            onCollectionDelete : handleEndDialog
        });
        
		userExtension = user.getExtension();
		userTeam = user.getTeamName();
		userId = user.getId();
		IsUserASupervisor = user.hasSupervisorRole();
		
		clientLogs.log("CallbackFinesseAgent.handleUserLoad(): Extension:" + userExtension + " Team:" + userTeam + " Id:" + userId + " IsUserASupervisor:" + IsUserASupervisor);
		
		ResetUI();

		var url = 'http://' + finesse.gadget.Config.host + ':9080/CallbackDispositionCodes.json';

		makeRequest(url, {
						method: 'GET'
					}
					,{
						success: handleResponseSuccess_LoadDispositionCodes,
						error: handleResponseError_LoadDispositionCodes,
					}
				);

        gadgets.window.adjustHeight();
		
		clientLogs.log("CallbackFinesseAgent.handleUserLoad(): Exit");
    },
      
    /**
     *  Handler for all User updates
     */
    handleUserChange = function(userevent) 
	{
        clientLogs.log("CallbackFinesseAgent.handleUserChange(): Enter");
		
		clientLogs.log("CallbackFinesseAgent.handleUserChange(): Exit");
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
            clientLogs.init(gadgets.Hub, "CallbackFinesseAgent");

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
			
			var sVersion = "";
			
			sVersion = " [DEV Version: 1.0.0.0006]";
			
			// $("#gadgetname").text("Callback Requeue" + sVersion);
			
			clientLogs.log("CallbackFinesseAgent - " + sVersion);
        }
    };
}(jQuery));