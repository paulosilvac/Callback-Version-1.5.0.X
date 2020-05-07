var gCallbackServerIP = "@@CALLBACKSERVERENDPOINT@@";
var gCSQListCCX = [];
var gCSQListCB = {};
var gXml = "";
var gIP = "";
var gToken = "";

// 10/4/2017 	- Hid 'Offered Algorithm' (display:none)
//					- Hid overflow csqs

function authenticate()
{
	//This works fine in IE
	var ahttp = new XMLHttpRequest();
	var isIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);
	var username = document.getElementById("txtUsername").value;
	var pass = document.getElementById("txtPassword").value;

	var url;
	try
	{	
		url = "http://" + gCallbackServerIP + "/callbackmanagement?operation=authenticateuser&token=" + window.btoa(username + ':' + pass);
	}
	catch(err)
	{
		showError("Please select 'Tools->Compatibility View' and REMOVE this site from the list. Also, be sure you are using IE 11 or higher. FireFox and Chrome will work as well.")
	}
	var settingsLoaded = false;
	
	ahttp.open("GET", url, false);

	try {ahttp.responseType = "msxml-document"} catch(err) {showError(err + " in Auth ");} // Helping IE11
	try
	{
		ahttp.send("");
	}
	catch(err)
	{
		showError("Error sending authentication to server: " + err);
	}

	var xmlIn = ahttp.responseXML;
	var txtIn = ahttp.responseText;

	var txtAuth = "";
	var xmlResp;

	if(isIE)
	{
		var parser = new DOMParser();
		xmlIn = parser.parseFromString(txtIn, 'text/xml');
		xmlResp = xmlIn.getElementsByTagName("Authenticated");
	}		
	else
	{
		xmlResp = xmlIn.getElementsByTagName("Authenticated");
	}
	try
	{
		var result = xmlIn.getElementsByTagName("Code");
		var code = result[0].childNodes[0].nodeValue;
		if(code == "-1")
		{
			// There was a problem loading the settings
			var errResult = xmlIn.getElementsByTagName("Description");
			var desc = errResult[0].childNodes[0].nodeValue;
			showError("Problem when attempting to authenticate. Error returned by the server: " + desc);
			return false;
		}
		if(code == "-2")
		{
			// UCCX communication not established
			var errResult = xmlIn.getElementsByTagName("Description");
			var desc = errResult[0].childNodes[0].nodeValue;
			showError("UCCX communications not yet established. Please try again in 30 seconds. Message returned by the server: " + desc);
			return false;
		}
	}
	catch(err)
	{
		showError("Communication with the server may have been lost. Error description: " + err);
		return false;
	}
	
	txtAuth = xmlResp[0].childNodes[0].nodeValue;

	if(txtAuth.toLowerCase() == "true")
	{
		// Hide/show as needed
		document.getElementById("loginPage").style.display = "none";
		document.getElementById("csqPage").style.display = "inherit";
		document.getElementById("logoutDiv").style.display = "inherit";
		settingsLoaded = loadSettings();
	}
	else
	{
		showError('<b>Authentication failure</b>');
	}
}

function loadCSQs()
{
	var uccxXML = loadXMLDoc("http://" + gCallbackServerIP + "/callbackmanagement?operation=getcsqlist", ""); 
	
	try
	{
		var result = uccxXML.getElementsByTagName("Code");
		var code = result[0].childNodes[0].nodeValue;

		if(code != "0")
		{
			// There was a problem loading the settings
			var errResult = uccxXML.getElementsByTagName("Description");
			var desc = errResult[0].childNodes[0].nodeValue;
			showError("Problem loading/reloading settings. Error returned by the server: " + desc);
			return false;
		}
	}
	catch(err)
	{
		showError("Communication with the server may have been lost. Error description: " + err);
		return false;
	}
	try
	{
		var aCSQs = uccxXML.getElementsByTagName("name");

		var i;
		
		// Make Default a legitamate csq for display purposes
		gCSQListCCX.push("Default");
		for (i = 0; i < aCSQs.length; i++) 
		{ 
			gCSQListCCX.push(aCSQs[i].childNodes[0].textContent);
		}

		document.getElementById("loginPage").style.display = "none";
		document.getElementById("csqPage").style.display = "inherit";
		document.getElementById("p_status").innerHTML = gCSQListCCX.length + " records found";
	}
	catch(err)
	{
		showError("There was an issue loading the CSQ list. Error details: " + err);
	}
}

function loadSettings() // Loads settings info from the callback server
{	
	if (gCallbackServerIP == "UNKNOWN")
	{
		showError("IP for callback server is not set");
		return false;
	}
	gXml = loadXMLDoc("http://" + gCallbackServerIP + "/callbackmanagement?operation=getsettings&GUID=" + generateUUID(), ""); 

	try
	{
		var result = gXml.getElementsByTagName("Code");
		var code = result[0].childNodes[0].nodeValue;

		if(code != "0")
		{
			// There was a problem loading the settings
			var errResult = gXml.getElementsByTagName("Description");
			var desc = errResult[0].childNodes[0].nodeValue;
			showError("Problem loading/reloading settings. Error returned by the server: " + desc);
			return false;
		}
	}
	catch(err)
	{
		showError("Communication with the server may have been lost. Error description: " + err);
		return false;
	}

	var x, i;

	//var aCallbackEnabled = gXml.getElementsByTagName("CallbackEnabled");
	// if using loadCSQs from UCCX, CSQ needs to be upper case. Otherwise, it needs to be lower case 03/30/2017
	var aCSQsettings = gXml.getElementsByTagName("csq");
	
	// Load the csq list from uccx
	loadCSQs();

	// Iterate through the csq list from the settings
	for (i = 0; i < aCSQsettings.length; i++) 
	{ 
		var csqName = aCSQsettings[i].getAttribute("name");

		// Only load settings if the csq is in UCCX
		if (gCSQListCCX.indexOf(csqName) > -1 || csqName == "Default") 
		{
			var enabled = aCSQsettings[i].getElementsByTagName("CallbackEnabled")[0].childNodes[0].nodeValue;		
			enabled = enabled.toLowerCase();
			if (csqName == "Default")
				enabled = "true";
			
			gCSQListCB[csqName] = {cbEnabled:enabled,useDefaultSettings:false,callerRecording:"",retPeriod:"",appServerURL:"",eMailAlerts:"",adminEmail:"",
										cIdVerify:"",abCallback:"",abMinCbTime:"",abMinIcTime:"",cbQ1name:"",cbQ1OFT:"",cbQ2name:"",cbQ2OFT:"",
										acceptTfBegin:"",acceptTfEnd:"",offAgLogInEN:"",offAgLogInOP:"",offAgLogInValue:"",
										offCWaitEN:"",offCWaitOP:"",offCWaitValue:"",offLongQEN:"",offLongQOP:"",offLongQValue:"",
										offCbReqEN:"",offCbReqOP:"",offCbReqValue:"",algTotIQEN:"", algTotIQOP:"",algTotIQValue:"",
										algAgReadyEN:"", algAgReadyOP:"",algAgReadyValue:"",algCallsWaitingEN:"", algCallsWaitingOP:"",algCallsWaitingValue:"",
										cbProcTimeBegin:"",cbProcTimeEnd:"",cbEndOfDayPurge:""};
			
			var testDefaultUsed = false;
		
			try
			{  // CSQ may use default profile, in which case all settings other than CallbackEnabled are missing
				gCSQListCB[csqName].callerRecording = aCSQsettings[i].getElementsByTagName("CallerRecording")[0].childNodes[0].nodeValue;
			}
			catch(err)
			{
				if(enabled.toLowerCase() == "true")
				{
					testDefaultUsed = true;
					gCSQListCB[csqName].useDefaultSettings = true;
				}
			}
			if(enabled.toLowerCase() == "true" && testDefaultUsed == false) // Get the settings for the CSQ
			{
				gCSQListCB[csqName].callerRecording = aCSQsettings[i].getElementsByTagName("CallerRecording")[0].childNodes[0].nodeValue;
				gCSQListCB[csqName].retPeriod = aCSQsettings[i].getElementsByTagName("RetentionPeriod")[0].childNodes[0].nodeValue;
				if(csqName == "Development")
					console.log("Loading ret period: " + gCSQListCB[csqName].retPeriod);
				try{gCSQListCB[csqName].appServerURL = aCSQsettings[i].getElementsByTagName("AppServerURLPrefix")[0].childNodes[0].nodeValue;} catch(err){}
				gCSQListCB[csqName].eMailAlerts = aCSQsettings[i].getElementsByTagName("EmailAlerts")[0].childNodes[0].nodeValue;
				try{gCSQListCB[csqName].adminEmail = aCSQsettings[i].getElementsByTagName("AdminEmail")[0].childNodes[0].nodeValue;} catch(err){}
				gCSQListCB[csqName].cIdVerify = aCSQsettings[i].getElementsByTagName("CallerIDVerify")[0].childNodes[0].nodeValue;
				gCSQListCB[csqName].abCallback = aCSQsettings[i].getElementsByTagName("AbandonCallback")[0].childNodes[0].nodeValue;
				gCSQListCB[csqName].abMinCbTime = aCSQsettings[i].getElementsByTagName("AbandonCBMinQTime")[0].childNodes[0].nodeValue;
				gCSQListCB[csqName].abMinIcTime = aCSQsettings[i].getElementsByTagName("AbandonCBMinInterCallTime")[0].childNodes[0].nodeValue;

				gCSQListCB[csqName].cbQ1name = aCSQsettings[i].getElementsByTagName("CBQueue")[0].getAttribute("csq");
				gCSQListCB[csqName].cbQ1OFT = aCSQsettings[i].getElementsByTagName("CBQueue")[0].getAttribute("overflowtime");
				
				gCSQListCB[csqName].cbQ2name = aCSQsettings[i].getElementsByTagName("CBQueue")[1].getAttribute("csq");
				gCSQListCB[csqName].cbQ2OFT = aCSQsettings[i].getElementsByTagName("CBQueue")[1].getAttribute("overflowtime");
				
				var aCbTimeframe = aCSQsettings[i].getElementsByTagName("AcceptCallbacksTimeframe");	
				try{gCSQListCB[csqName].acceptTfBegin = aCbTimeframe[0].getElementsByTagName("Begin")[0].childNodes[0].nodeValue;} catch(err){}
				try{gCSQListCB[csqName].acceptTfEnd = aCbTimeframe[0].getElementsByTagName("End")[0].childNodes[0].nodeValue;} catch(err){}
				
// Callback Offered
				gCSQListCB[csqName].offAgLogInEN =  aCSQsettings[i].getElementsByTagName("AgentsLoggedIn")[0].getAttribute("Enabled");
				gCSQListCB[csqName].offAgLogInOP = 'BIGGERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("AgentsLoggedIn")[0].getAttribute("Operation");
				gCSQListCB[csqName].offAgLogInValue = aCSQsettings[i].getElementsByTagName("AgentsLoggedIn")[0].getAttribute("Value");
				
				gCSQListCB[csqName].offCWaitEN = aCSQsettings[i].getElementsByTagName("CallsWaiting")[0].getAttribute("Enabled");
				gCSQListCB[csqName].offCWaitOP = 'SMALLERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("CallsWaiting")[0].getAttribute("Operation");
				gCSQListCB[csqName].offCWaitValue = aCSQsettings[i].getElementsByTagName("CallsWaiting")[0].getAttribute("Value");
				
				gCSQListCB[csqName].offLongQEN = aCSQsettings[i].getElementsByTagName("LongestQueueTime")[0].getAttribute("Enabled");
				gCSQListCB[csqName].offLongQOP = 'SMALLERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("LongestQueueTime")[0].getAttribute("Operation");
				gCSQListCB[csqName].offLongQValue = aCSQsettings[i].getElementsByTagName("LongestQueueTime")[0].getAttribute("Value");				

				gCSQListCB[csqName].offCbReqEN = aCSQsettings[i].getElementsByTagName("CallbackRequests")[0].getAttribute("Enabled");
				gCSQListCB[csqName].offCbReqOP = 'SMALLERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("CallbackRequests")[0].getAttribute("Operation");
				gCSQListCB[csqName].offCbReqValue = aCSQsettings[i].getElementsByTagName("CallbackRequests")[0].getAttribute("Value");					
				 
// Callback Reentry				
				gCSQListCB[csqName].algTotIQEN = aCSQsettings[i].getElementsByTagName("TotalInQueue")[0].getAttribute("Enabled");
				gCSQListCB[csqName].algTotIQOP = 'SMALLERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("TotalInQueue")[0].getAttribute("Operation");
				gCSQListCB[csqName].algTotIQValue = aCSQsettings[i].getElementsByTagName("TotalInQueue")[0].getAttribute("Value");
				
				gCSQListCB[csqName].algAgReadyEN = aCSQsettings[i].getElementsByTagName("CSQAgentsReady")[0].getAttribute("Enabled");
				gCSQListCB[csqName].algAgReadyOP = 'BIGGERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("CSQAgentsReady")[0].getAttribute("Operation");
				gCSQListCB[csqName].algAgReadyValue = aCSQsettings[i].getElementsByTagName("CSQAgentsReady")[0].getAttribute("Value");
				
				gCSQListCB[csqName].algCallsWaitingEN = aCSQsettings[i].getElementsByTagName("CSQCallsWaiting")[0].getAttribute("Enabled");
				gCSQListCB[csqName].algCallsWaitingOP = 'SMALLERTHANOREQUALTO'; //aCSQsettings[i].getElementsByTagName("CSQCallsWaiting")[0].getAttribute("Operation");
				gCSQListCB[csqName].algCallsWaitingValue = aCSQsettings[i].getElementsByTagName("CSQCallsWaiting")[0].getAttribute("Value");
				
				var aCbProcTimeframe = aCSQsettings[i].getElementsByTagName("CallbackProcessingTimeframe");	
				try{gCSQListCB[csqName].cbProcTimeBegin = aCbProcTimeframe[0].getElementsByTagName("Begin")[0].childNodes[0].nodeValue;} catch(err){}
				try{gCSQListCB[csqName].cbProcTimeEnd = aCbProcTimeframe[0].getElementsByTagName("End")[0].childNodes[0].nodeValue;} catch(err){}
				
				gCSQListCB[csqName].cbEndOfDayPurge = aCSQsettings[i].getElementsByTagName("EndOfDayPurgeCallbackRequests")[0].childNodes[0].nodeValue;
			}
		}
	}
	for (k = 0; k < gCSQListCCX.length; k++)
	{
		try
		{
			var test = gCSQListCB[gCSQListCCX[k]];
			if (test == undefined)
			{
				gCSQListCB[gCSQListCCX[k]] = {cbEnabled:"false"};
			}
		}
		catch(err) // This should not happen...
		{
			gCSQListCB[gCSQListCCX[k]] = {cbEnabled:"false"};
		}
	}
	
	var txtCSQTable = "<caption style='border:1px solid #fdb913'>Contact Service Queues</caption>" +
			"<tr><td class='csqTableHeadings'>Name</td><td class='csqTableHeadings'>Callback Enabled</td></tr>";

	for (j = 0; j < gCSQListCCX.length; j++)
	{
		if(j%2 != 0)
		{
			var enabled = "false";
			try
			{
				enabled = gCSQListCB[gCSQListCCX[j]].cbEnabled;
				if (enabled == "")
					enabled = "false";
			}
			catch(err){}//alert(gCSQListCCX[j] + " doesn't exist in settings.");}
			if(gCSQListCCX[j].toLowerCase() == "default")
				txtCSQTable += "<tr style='background-color:#FFF0CB'><td class='tdCSQ'><a onclick=settingsPage('" + gCSQListCCX[j] + "')>" + gCSQListCCX[j] + "*</a></td><td class='tdCSQ'>" + enabled + "</td></tr>";
			else
			{
				var name = gCSQListCCX[j];
				name = name.replace(/\s/g, "-");
				txtCSQTable += "<tr style='background-color:#FFF0CB'><td class='tdCSQ'><a onclick=settingsPage('" + gCSQListCCX[j] + "')>" + gCSQListCCX[j] + "</a></td><td class='tdCSQ'>" + enabled + "</td></tr>";
			}
		}	
		else
		{
			var enabled = "false";
			try
			{
				enabled = gCSQListCB[gCSQListCCX[j]].cbEnabled;
				if (enabled == "")
					enabled = "false";
			}
			catch(err){}//alert(gCSQListCCX[j] + " doesn't exist in settings.");}
			if(gCSQListCCX[j].toLowerCase() == "default")
				txtCSQTable += "<tr style='background-color:#FFF7E5'><td class='tdCSQ'><a onclick=settingsPage('" + gCSQListCCX[j] + "')>" + gCSQListCCX[j] + "*</a></td><td class='tdCSQ'>" + enabled + "</td></tr>";
			else
			{
				var name = gCSQListCCX[j];
				name = name.replace(/\s/g, "-");
				txtCSQTable += "<tr style='background-color:#FFF7E5'><td class='tdCSQ'><a onclick=settingsPage('" + gCSQListCCX[j] + "')>" + gCSQListCCX[j] + "</a></td><td class='tdCSQ'>" + enabled + "</td></tr>";
			}
		}
	}
	document.getElementById("csqTable").innerHTML = txtCSQTable;
	return true;
}

function loadXMLDoc(filename,authString)
{
	var isIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);
	var xmlResp;

	if (isIE)//(window.ActiveXObject)
	{
		xhttp = new XMLHttpRequest();//xhttp = new ActiveXObject("MSXML2.XMLHTTP.3.0");//
	}
	else
	{
		xhttp = new XMLHttpRequest();
	}

	xhttp.open("GET", filename, false);

	if(authString != "")
	{
		xhttp.setRequestHeader('Authorization', 'Basic ' + authString);
	}
	
	try
	{
		xhttp.send("");
	}
	catch(err)
	{
		showError("There was a problem communicating with the server. Error: " + err);
	}

	if(isIE)
	{
		var parser = new DOMParser();
		xmlResp = parser.parseFromString(xhttp.responseText, 'text/xml');
	}		
	else
		xmlResp = xhttp.responseXML;
//alert(xhttp.responseText);
	return xmlResp;
}

function clearLogin()
{
	document.getElementById("csqSettingsTable").innerHTML = "";
	gCSQListCCX = [];
	gCSQListCB = {};
	document.getElementById("loginPage").style.display = "inherit";
	document.getElementById("csqPage").style.display = "none";
	document.getElementById("settingsPage").style.display = "none";
	document.getElementById("txtUsername").value="";
	document.getElementById("txtPassword").value="";
	document.getElementById("logoutDiv").style.display = "none";
}

function settingsPage(csqname)
{
	csqname = csqname.replace(/-/g, " ");
	var txtSettingsTable;
	var callbackEnabled = gCSQListCB[csqname].cbEnabled;
	var defaultEnabled = gCSQListCB[csqname].callerRecording;
	var btnSave = document.getElementById("btnSaveSettings");
	var useDefaultSettings = gCSQListCB[csqname].useDefaultSettings;
	
	if (csqname == "Default")
		callbackEnabled = "true";
	
	if (useDefaultSettings == undefined) // Undefined if CSQ is in UCCX and not in settings file
		useDefaultSettings = true;
	// Save csq name with button so I can call save on correct csq
	btnSave.setAttribute("name",csqname);
	
	txtSettingsTable = "<tr><td class='tdCSQ'>Contact Service Queue Name</td><td class='tdCSQ'><input style='background-color:#dddddd' size='40' type='text' readonly value='" + csqname + "'></input></td></tr>";
	if(csqname.toLowerCase() == "default")
	{
		txtSettingsTable += "<tr><td class='tdCSQ'>Callback</td><td class='tdCSQ'><input type='radio' id='cbEnabledT' name='cbEnabled' value='Enabled' checked>Enabled<input type='radio' onclick=ignoreClick() name='cbEnabled' value='Disabled'>Disabled</td></tr>";
	}
	else
	{
		if(callbackEnabled.toLowerCase() == "true")
			txtSettingsTable += "<tr><td class='tdCSQ'>Callback</td><td class='tdCSQ'><input type='radio' id='cbEnabledT' name='cbEnabled' value='Enabled' checked>Enabled<input type='radio' name='cbEnabled' value='Disabled'>Disabled</td></tr>";
		else
			txtSettingsTable += "<tr><td class='tdCSQ'>Callback</td><td class='tdCSQ'><input type='radio' id='cbEnabledT' name='cbEnabled' value='Enabled'>Enabled<input type='radio' name='cbEnabled' value='Disabled' checked>Disabled</td></tr>";
		// Don't show this for 'Default' 
		if(useDefaultSettings == true)
			txtSettingsTable += "<tr><td class='tdCSQ'>Use Default Settings</td><td class='tdCSQ'><input onclick=setBgDisabled(true) type='radio' id='defaultSettingsT' name='defEnabled' value='Enabled' checked>Enabled<input onclick=setBgDisabled(false) type='radio' name='defEnabled' value='Disabled'>Disabled</td></tr>";
		else
			txtSettingsTable += "<tr><td class='tdCSQ'>Use Default Settings</td><td class='tdCSQ'><input onclick=setBgDisabled(true) type='radio' id='defaultSettingsT' name='defEnabled' value='Enabled'>Enabled<input onclick=setBgDisabled(false) type='radio' name='defEnabled' value='Disabled' checked>Disabled</td></tr>";
	}
//***********************************************
	// Fill in default values if CB is enabled and nothing else in the XML is
	if (useDefaultSettings)
	{
		csqname = "Default";
		txtSettingsTable += "<tbody id='bgDefault' style='pointer-events:none;background-color:lightgray'>";
	}
	else
		txtSettingsTable += "<tbody id='bgDefault' style='pointer-events:inherit;background-color:white'>";
	
	if(gCSQListCB[csqname].callerRecording.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Caller Recording</td><td class='tdCSQ'><input type='radio' id='callerRecordingT' name='recEnabled' value='Enabled' checked>Enabled<input type='radio' id='callerRecordingF' name='recEnabled' value='Disabled'>Disabled</td><td></td><td></td></tr>";
	else
		txtSettingsTable += "<tr><td class='tdCSQ'>Caller Recording</td><td class='tdCSQ'><input type='radio' id='callerRecordingT' name='recEnabled' value='Enabled'>Enabled<input type='radio' id='callerRecordingF' name='recEnabled' value='Disabled' checked>Disabled</td><td></td><td></td></tr>";
	
	txtSettingsTable += "<tr><td class='tdCSQ'>Caller Recording Retention Days</td><td class='tdCSQ'><input size='40' type='text' id='retPeriod' value='" + gCSQListCB[csqname].retPeriod + "'></input></td><td></td><td></td></tr>";
	console.log("Retention Period: " + gCSQListCB[csqname].retPeriod);
	//txtSettingsTable += "<tr><td class='tdCSQ'>Application Server URL</td><td class='tdCSQ'><input size='40' type='text' id='appServerURL' value='" + gCSQListCB[csqname].appServerURL + "'></input></td></tr>";

	if(gCSQListCB[csqname].eMailAlerts.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Email Alerts</td><td class='tdCSQ'><input type='radio' id='eMailAlertsT' name='emailAlEnabled' value='Enabled' checked>Enabled<input type='radio' id='eMailAlertsF' name='emailAlEnabled' value='Disabled'>Disabled</td><td></td><td></td></tr>";
	else
		txtSettingsTable += "<tr><td class='tdCSQ'>Email Alerts</td><td class='tdCSQ'><input type='radio' id='eMailAlertsT' name='emailAlEnabled' value='Enabled'>Enabled<input type='radio' id='eMailAlertsF' name='emailAlEnabled' value='Disabled' checked>Disabled</td><td></td><td></td></tr>";
	
	txtSettingsTable += "<tr><td class='tdCSQ'>Administrator Email</td><td class='tdCSQ'><input size='40' type='text' id='adminEmail' value='" + gCSQListCB[csqname].adminEmail + "'></input></td><td></td><td></td></tr>";

	if(gCSQListCB[csqname].cIdVerify.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Caller ID Verify</td><td class='tdCSQ'><input type='radio' id='cidVerifyT' name='cidVerify' value='Enabled' checked>Enabled<input type='radio' id='cidVerifyF' name='cidVerify' value='Disabled'>Disabled</td><td></td><td></td></tr>";
	else
		txtSettingsTable += "<tr><td class='tdCSQ'>Caller ID Verify</td><td class='tdCSQ'><input type='radio' id='cidVerifyT' name='cidVerify' value='Enabled'>Enabled<input type='radio' id='cidVerifyF' name='cidVerify' value='Disabled' checked>Disabled</td><td></td><td></td></tr>";

	if(gCSQListCB[csqname].abCallback.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Abandon Callback</td><td class='tdCSQ'><input type='radio' id='abCallbackT' name='abCallback' value='Enabled' checked>Enabled<input type='radio' id='abCallbackF' name='abCallback' value='Disabled'>Disabled</td>";
	else
		txtSettingsTable += "<tr><td class='tdCSQ'>Abandon Callback</td><td class='tdCSQ'><input type='radio' id='abCallbackT' name='abCallback' value='Enabled'>Enabled<input type='radio' id='abCallbackF' name='abCallback' value='Disabled' checked>Disabled</td>";
	txtSettingsTable += "<td class='tdCSQ'>Min Queue Time&nbsp;<input size='6' type='text' id='abMinCbTime' value='" + gCSQListCB[csqname].abMinCbTime + "'></input></td>";
	txtSettingsTable += "<td class='tdCSQ'>Min Intercall Time&nbsp;<input size='6' type='text' id='abMinIcTime' value='" + gCSQListCB[csqname].abMinIcTime + "'></input></td></tr>";
	
	txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Overflow Queue 1</td><td class='tdCSQ'><input size='40' type='text' id='cbQ1name' value='" + gCSQListCB[csqname].cbQ1name + "'></input></td>" +
								"<td class='tdCSQ'>Overflow Time&nbsp;<input size='6' type='text' id='cbQ1OFT' value='" + gCSQListCB[csqname].cbQ1OFT + "'></input></td><td></td></tr>";
	txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Overflow Queue 2</td><td class='tdCSQ'><input size='40' type='text' id='cbQ2name' value='" + gCSQListCB[csqname].cbQ2name + "'></input></td>" +
								"<td class='tdCSQ'>Overflow Time&nbsp;<input size='6' type='text' id='cbQ2OFT' value='" + gCSQListCB[csqname].cbQ2OFT + "'></input></td><td></td></tr>";
	
	txtSettingsTable += "<tr><td class='tdCSQ'>Accept Callback Period</td><td class='tdCSQ'>Begin&nbsp;<input size='6' type='text' id='acceptTfBegin' value='" + gCSQListCB[csqname].acceptTfBegin + "'></input>" +
								"&nbsp;&nbsp;End&nbsp;<input size='6' type='text' id='acceptTfEnd' value='" + gCSQListCB[csqname].acceptTfEnd + "'></input></td><td></td><td></td></tr>";
// Offered Algorithm
 	txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'><b>Callback Offered Algorithm</b></td><td></td><td></td><td></td></tr>";	
	if(gCSQListCB[csqname].offAgLogInEN.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Agents Logged In</td><td class='tdCSQ'><input type='radio' id='offAgLogInENT' name='offAgLogInEN' value='Enabled' checked>Enabled<input type='radio' id='offAgLogInENF' name='offAgLogInEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Agents Logged In</td><td class='tdCSQ'><input type='radio' id='offAgLogInENT' name='offAgLogInEN' value='Enabled'>Enabled<input type='radio' id='offAgLogInENF' name='offAgLogInEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='offAgLogInOP' value='" + gCSQListCB[csqname].offAgLogInOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='offAgLogInValue' value='" + gCSQListCB[csqname].offAgLogInValue + "'></input></td><td></td></tr>";
	
	if(gCSQListCB[csqname].offCWaitEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Calls Waiting</td><td class='tdCSQ'><input type='radio' id='offCWaitENT' name='offCWaitEN' value='Enabled' checked>Enabled<input type='radio' id='offCWaitENF' name='offCWaitEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Calls Waiting</td><td class='tdCSQ'><input type='radio' id='offCWaitENT' name='offCWaitEN' value='Enabled'>Enabled<input type='radio' id='offCWaitENF' name='offCWaitEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='offCWaitOP' value='" + gCSQListCB[csqname].offCWaitOP + "'></input></td>" +
		txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='offCWaitValue' value='" + gCSQListCB[csqname].offCWaitValue + "'></input></td><td></td></tr>";	
	
	if(gCSQListCB[csqname].offLongQEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Longest Queue Time</td><td class='tdCSQ'><input type='radio' id='offLongQENT' name='offLongQEN' value='Enabled' checked>Enabled<input type='radio' id='offLongQENF' name='offLongQEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Longest Queue Time</td><td class='tdCSQ'><input type='radio' id='offLongQENT' name='offLongQEN' value='Enabled'>Enabled<input type='radio' id='offLongQENF' name='offLongQEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='offLongQOP' value='" + gCSQListCB[csqname].offLongQOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='offLongQValue' value='" + gCSQListCB[csqname].offLongQValue + "'></input></td><td></td></tr>";		

	if(gCSQListCB[csqname].offCbReqEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Callback Requests</td><td class='tdCSQ'><input type='radio' id='offCbReqENT' name='offCbReqEN' value='Enabled' checked>Enabled<input type='radio' id='offCbReqENF' name='offCbReqEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Callback Requests</td><td class='tdCSQ'><input type='radio' id='offCbReqENT' name='offCbReqEN' value='Enabled'>Enabled<input type='radio' id='offCbReqENF' name='offCbReqEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='offCbReqOP' value='" + gCSQListCB[csqname].offCbReqOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='offCbReqValue' value='" + gCSQListCB[csqname].offCbReqValue + "'></input></td><td></td></tr>";									
// Reentry Algorithm
	txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'><b>Callback Reentry Algorithm</b></td><td></td><td></td><td></td></tr>";
	
	if(gCSQListCB[csqname].algTotIQEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Total Calls in CSQ</td><td class='tdCSQ'><input type='radio' id='algTotIQENT' name='algTotIQEN' value='Enabled' checked>Enabled<input type='radio' id='algTotIQENF' name='algTotIQEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Total Calls in CSQ</td><td class='tdCSQ'><input type='radio' id='algTotIQENT' name='algTotIQEN' value='Enabled'>Enabled<input type='radio' id='algTotIQENF' name='algTotIQEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='algTotIQOP' value='" + gCSQListCB[csqname].algTotIQOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='algTotIQValue' value='" + gCSQListCB[csqname].algTotIQValue + "'></input></td><td></td></tr>";
	if(gCSQListCB[csqname].algAgReadyEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Agents Ready</td><td class='tdCSQ'><input type='radio' id='algAgReadyENT' name='algAgReadyEN' value='Enabled' checked>Enabled<input type='radio' id='algAgReadyENF' name='algAgReadyEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Agents Ready</td><td class='tdCSQ'><input type='radio' id='algAgReadyENT' name='algAgReadyEN' value='Enabled'>Enabled<input type='radio' id='algAgReadyENF' name='algAgReadyEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='algAgReadyOP' value='" + gCSQListCB[csqname].algAgReadyOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='algAgReadyValue' value='" + gCSQListCB[csqname].algAgReadyValue + "'></input></td><td></td></tr>";	
	if(gCSQListCB[csqname].algCallsWaitingEN.toLowerCase() == "true")
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Calls Waiting</td><td class='tdCSQ'><input type='radio' id='algCallsWaitingENT' name='algCallsWaitingEN' value='Enabled' checked>Enabled<input type='radio' id='algCallsWaitingENF' name='algCallsWaitingEN' value='Disabled'>Disabled</td>";								
	else
		txtSettingsTable += "<tr style='display:none'><td class='tdCSQ'>Number of Calls Waiting</td><td class='tdCSQ'><input type='radio' id='algCallsWaitingENT' name='algCallsWaitingEN' value='Enabled'>Enabled<input type='radio' id='algCallsWaitingENF' name='algCallsWaitingEN' value='Disabled' checked>Disabled</td>";
	//txtSettingsTable += "<td class='tdCSQ'>Operation&nbsp;<input style='background-color:#dddddd' readonly size='20' type='text' id='algCallsWaitingOP' value='" + gCSQListCB[csqname].algCallsWaitingOP + "'></input></td>" +
	txtSettingsTable += "<td class='tdCSQ'>Value&nbsp;<input size='6' type='text' id='algCallsWaitingValue' value='" + gCSQListCB[csqname].algCallsWaitingValue + "'></input></td><td></td></tr>";		

	txtSettingsTable += "<tr><td class='tdCSQ'>Callback Processing Period</td><td class='tdCSQ'>Begin&nbsp;<input size='6' type='text' id='cbProcTimeBegin' value='" + gCSQListCB[csqname].cbProcTimeBegin + "'></input>" +
								"&nbsp;&nbsp;End&nbsp;<input size='6' type='text' id='cbProcTimeEnd' value='" + gCSQListCB[csqname].cbProcTimeEnd + "'></input></td><td></td><td></td></tr>";
		
	if(gCSQListCB[csqname].cbEndOfDayPurge.toLowerCase() == "true")
		txtSettingsTable += "<tr><td class='tdCSQ'>Callback End of Day Purge</td><td class='tdCSQ'><input type='radio' id='cbEndOfDayPurgeT' name='eodPurge' value='Enabled' checked>Enabled<input type='radio' id='cbEndOfDayPurgeF' name='eodPurge' value='Disabled'>Disabled</td><td></td><td></td></tr>";
	else
		txtSettingsTable += "<tr><td class='tdCSQ'>Callback End of Day Purge</td><td class='tdCSQ'><input type='radio' id='cbEndOfDayPurgeT' name='eodPurge' value='Enabled'>Enabled<input type='radio' id='cbEndOfDayPurgeF' name='eodPurge' value='Disabled' checked>Disabled</td><td></td><td></td></tr>";
		
	if (useDefaultSettings)
	{
		csqname = "Default";
		txtSettingsTable += "</tbody>";
	}
	
	document.getElementById("csqSettingsTable").innerHTML = txtSettingsTable;
	document.getElementById("loginPage").style.display = "none";
	document.getElementById("csqPage").style.display = "none";
	document.getElementById("settingsPage").style.display = "inherit";
}

function saveSettings()
{
	var btnSave = document.getElementById("btnSaveSettings");
	var csq = btnSave.getAttribute("name");
	var i = 0;

	// First, validate the data
	if(validateSettings(csq))
	{
		// Next, save the changed CSQ to gCSQListCB
		saveToCsqObject(csq);
		// Then, iterate over gCSQListCB and convert it to XML message to send back to the server
		var xml = "<callback><csqs>";
		for (var csq in gCSQListCB) 
		{
			 xml += formatCSQtoXML(csq);
		}
		xml += "</csqs></callback>";
console.log("xml to save: " + xml);
		// Finally,	send the xml to the server // TODO: Make sure this works before clearing the table and reloading the settings below
		if(saveToServer(xml))
		{
			// Clear the table, settings list and csq list
			document.getElementById("csqSettingsTable").innerHTML = "";
			gCSQListCCX = [];
			gCSQListCB = {};
			// Reload the csq page
			loadSettings();
			
			// Display the csq page
			document.getElementById("loginPage").style.display = "none";
			document.getElementById("csqPage").style.display = "inherit";
			document.getElementById("settingsPage").style.display = "none";
		}
	}	
}

function saveToServer(xmlMessage)
{ // TODO: Check that this works, return true/flase
	var isIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);
	var xmlResp;

	if (isIE)//(window.ActiveXObject)
	{
		xhttp = new XMLHttpRequest();
		//xhttp = new ActiveXObject("MSXML2.XMLHTTP.3.0");//ActiveXObject("Msxml2.XMLHTTP");
	}
	else
	{
		xhttp = new XMLHttpRequest();
	}

	xhttp.open("POST", "http://" + gCallbackServerIP + "/callbackmanagement?operation=setsettings", false);
	
	xhttp.setRequestHeader('Content-Type', 'text/xml ');

	xhttp.send(xmlMessage);

	if(isIE)
	{
		var parser = new DOMParser();
		xmlResp = parser.parseFromString(xhttp.responseText, 'text/xml');
	}		
	else
		xmlResp = xhttp.responseXML;
	
	try
	{
		var result = xmlResp.getElementsByTagName("Code");
		var code = result[0].childNodes[0].nodeValue;
		
		if(code != "0")
		{
			// There was a problem saving the settings
			var errResult = xmlResp.getElementsByTagName("Description");
			var desc = result[0].childNodes[0].nodeValue;
			showError("Problem saving settings. Error returned by the server: " + desc);
			return false;
		}
	}
	catch(err)
	{
		showError("Communication with the server may have been lost. Error description: " + err);
		return false;
	}
	return true;
}

function cancelSettings()
{
	// Clear the table, settings list and csq list
	document.getElementById("csqSettingsTable").innerHTML = "";
	gCSQListCCX = [];
	gCSQListCB = {};
	// Reload the csq page
	loadSettings();
	document.getElementById("loginPage").style.display = "none";
	document.getElementById("csqPage").style.display = "inherit";
	document.getElementById("settingsPage").style.display = "none";		
}

function validateSettings(csqName)
{
	var errorString = "";
	
	//alert("Between: " + num.between(10,20));
	var timePattern = /^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$/;
	var urlPattern = /(http|ftp|https):\/\/[\w-]*(\.[\w-]+)+([\w.,@?^=%&amp;:\/~+#-]*[\w@?^=%&amp;\/~+#-])?/;
	var emailPattern = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

	// If callback is not enabled, don't bother validating
	if(!document.getElementById("cbEnabledT").checked)
		return true;
	
//	var testURL = document.getElementById("appServerURL").value;
//	if(!(urlPattern.test(testURL)))
//		errorString += "Application Server URL is invalid<br/>";

	// If callback is enabled, check the retention period
	if(document.getElementById("cbEnabledT").checked)
	{
		var retPeriod = document.getElementById("retPeriod").value;
		console.log("ret period read from page: " + retPeriod);
		if(Number(retPeriod))
		{
			if((retPeriod < 1) || (retPeriod > 30))
				errorString += "Retention Period must be between 1 and 30 days";
		}
		else
			errorString += "Retention Period must be an integer between 1 and 30";
	}
	
	var testEmail = document.getElementById("adminEmail").value;
	if(document.getElementById("eMailAlertsT").checked)
	{
		if(!emailPattern.test(testEmail))
			errorString += "Administrator email address is invalid<br/>";
	}
	
	if(document.getElementById("abCallbackT").checked)
	{
		if(!Number.posInteger(Number(document.getElementById("abMinCbTime").value)) || document.getElementById("abMinCbTime").value == "")
			errorString += "Abandon callback minimum queue time is not a positive integer<br/>";

		if(!Number.posInteger(Number(document.getElementById("abMinIcTime").value)) || document.getElementById("abMinIcTime").value == "")
			errorString += "Abandon callback minimum intercall time is not a positive integer<br/>";	
	}
/* rjm - removed overflow csqs 10/4/2017
	var csq1 = document.getElementById("cbQ1name").value;	
	if(gCSQListCCX.indexOf(csq1) == -1)
		errorString += "'" + csq1 + "' is not a valid CSQ name<br/>";
	
	if(!Number.posInteger(Number(document.getElementById("cbQ1OFT").value)))
		errorString += csq1 + " overflow time is not a positive integer";
		
	var csq2 = document.getElementById("cbQ2name").value;	
	if(gCSQListCCX.indexOf(csq2) == -1)
		errorString += "'" + csq2 + "' is not a valid CSQ name<br/>";

	if(!Number.posInteger(Number(document.getElementById("cbQ2OFT").value)))
		errorString += csq2 + " overflow time is not a positive integer";
*/
// Accept Time check
	var cbBeginTime = document.getElementById("acceptTfBegin").value;
	var beginSum = -1;
	if(!timePattern.test(cbBeginTime))
		errorString += "Accept Callback Period begin time invalid or is not of the form 'hh:mm:ss'<br/>";
	else
	{
		var beginParts = cbBeginTime.split(":");
		beginSum = Number(beginParts[0])*60*60 + Number(beginParts[1])*60 + Number(beginParts[2]);
	}
	
	var cbEndTime = document.getElementById("acceptTfEnd").value;
	var endSum = -1;
	if(!timePattern.test(document.getElementById("acceptTfEnd").value))
		errorString += "Accept Callback Period end time invalid or is not of the form 'hh:mm:ss'<br/>";
	else
	{
		var endParts = cbEndTime.split(":");
		endSum = Number(endParts[0])*60*60 + Number(endParts[1])*60 + Number(endParts[2]);
	}
	
	if(beginSum > -1 && endSum > -1)
	{
		if(beginSum >= endSum)
			errorString += "Accept Callback Period end time must be later than begin time<br/>"
	}

	if(document.getElementById("offAgLogInENT").checked)
	{
		if(!Number.posInteger(Number(document.getElementById("offAgLogInValue").value)) || document.getElementById("offAgLogInValue").value == "")
			errorString += "Agents Logged In value is not a positive integer<br/>";
	}

	if(document.getElementById("offCWaitENT").checked)
	{	
		if(!Number.posInteger(Number(document.getElementById("offCWaitValue").value)) || document.getElementById("offCWaitValue").value == "")
			errorString += "Offered Algorithm Calls Waiting value is not a positive integer<br/>";
	}
	
	if(document.getElementById("offLongQENT").checked)
	{	
		if(!Number.posInteger(Number(document.getElementById("offLongQValue").value)) || document.getElementById("offLongQValue").value == "")
			errorString += "Longest Queue Time value is not a positive integer<br/>";
	}
	
	if(document.getElementById("offCbReqENT").checked)
	{		
		if(!Number.posInteger(Number(document.getElementById("offCbReqValue").value)) || document.getElementById("offCbReqValue").value == "")
			errorString += "Callback Requests value is not a positive integer<br/>";
	}
	
	if(document.getElementById("algTotIQENT").checked)
	{	
		if(!Number.posInteger(Number(document.getElementById("algTotIQValue").value)) || document.getElementById("algTotIQValue").value == "")
			errorString += "Total In Queue value is not a positive integer<br/>";
	}
	
	if(document.getElementById("algAgReadyENT").checked)
	{
		if(!Number.posInteger(Number(document.getElementById("algAgReadyValue").value)) || document.getElementById("algAgReadyValue").value == "")
			errorString += "Agents Ready value is not a positive integer<br/>";
	}
	
	if(document.getElementById("algCallsWaitingENT").checked)
	{
		if(!Number.posInteger(Number(document.getElementById("algCallsWaitingValue").value)) || document.getElementById("algCallsWaitingValue").value == "")
			errorString += "Reentry Algorithm Calls Waiting value is not a positive integer<br/>";
	}

// Processing time check	
	var ptBeginTime = document.getElementById("cbProcTimeBegin").value;
	var beginSum1 = -1;
	if(!timePattern.test(document.getElementById("cbProcTimeBegin").value))
		errorString += "Callback Processing Period begin time invalid or is not of the form 'hh:mm:ss'<br/>";
	else
	{
		var beginParts = ptBeginTime.split(":");
		beginSum1 = Number(beginParts[0])*60*60 + Number(beginParts[1])*60 + Number(beginParts[2]);
	}
	
	var ptEndTime = document.getElementById("cbProcTimeEnd").value;
	var endSum1 = -1;	
	if(!timePattern.test(document.getElementById("cbProcTimeEnd").value))
		errorString += "Callback Processing Period end time invalid or is not of the form 'hh:mm:ss'<br/>";
	else
	{
		var endParts = ptEndTime.split(":");
		endSum1 = Number(endParts[0])*60*60 + Number(endParts[1])*60 + Number(endParts[2]);
	}
	
	if(beginSum1 > -1 && endSum1 > -1)
	{
		if(beginSum1 >= endSum1)
			errorString += "Callback Processing Period end time must be later than begin time<br/>"
	}

	
	if(errorString != "")
	{
		if(csqName.toLowerCase() != "default")
		{
			if(document.getElementById("defaultSettingsT").checked)
			{
				showError("Check Default settings and correct the following error(s):<br/><br/>" + errorString);
				return false;
			}
			showError("The following error(s) need to be addressed:<br/><br/>" + errorString);
			return false;
		}
		else
		{
			showError("The following error(s) need to be addressed:<br/><br/>" + errorString);
		}
		return false;
	}
	
	return true;
}

function saveToCsqObject(csqName)
{
	var val = "";
	var useDefault = false;
	
	// 'Default' doesn't have this element
	try
	{
		useDefault = document.getElementById("defaultSettingsT").checked;
	}
	catch(err)
	{
		useDefault = false;
	}

	gCSQListCB[csqName].cbEnabled = (document.getElementById("cbEnabledT").checked ? "true" : "false");
	if(!useDefault)
	{
		gCSQListCB[csqName].callerRecording = (document.getElementById("callerRecordingT").checked ? "true" : "false");

		gCSQListCB[csqName].retPeriod = document.getElementById("retPeriod").value;
		console.log("save value to obj: " + document.getElementById("retPeriod").value);
	//	gCSQListCB[csqName].appServerURL = document.getElementById("appServerURL").value;
		gCSQListCB[csqName].eMailAlerts = (document.getElementById("eMailAlertsT").checked ? "true" : "false");
		gCSQListCB[csqName].adminEmail = document.getElementById("adminEmail").value;
		gCSQListCB[csqName].cIdVerify = (document.getElementById("cidVerifyT").checked ? "true" : "false");
		gCSQListCB[csqName].abCallback = (document.getElementById("abCallbackT").checked ? "true" : "false");
		gCSQListCB[csqName].abMinCbTime = document.getElementById("abMinCbTime").value;	
		gCSQListCB[csqName].abMinIcTime = document.getElementById("abMinIcTime").value;	
		gCSQListCB[csqName].cbQ1name = document.getElementById("cbQ1name").value;
		gCSQListCB[csqName].cbQ1OFT = document.getElementById("cbQ1OFT").value;
		gCSQListCB[csqName].cbQ2name = document.getElementById("cbQ2name").value;
		gCSQListCB[csqName].cbQ2OFT = document.getElementById("cbQ2OFT").value;
		gCSQListCB[csqName].acceptTfBegin = document.getElementById("acceptTfBegin").value;
		gCSQListCB[csqName].acceptTfEnd = document.getElementById("acceptTfEnd").value;	
/* 		
		gCSQListCB[csqName].offAgLogInEN = (document.getElementById("offAgLogInENT").checked ? "true" : "false");
		gCSQListCB[csqName].offAgLogInOP = 'BIGGERTHANOREQUALTO'; //gCSQListCB["Default"].offAgLogInOP;//document.getElementById("offAgLogInOP").value;
		gCSQListCB[csqName].offAgLogInValue = document.getElementById("offAgLogInValue").value;
		
		gCSQListCB[csqName].offCWaitEN = (document.getElementById("offCWaitENT").checked ? "true" : "false");
		gCSQListCB[csqName].offCWaitOP = 'BIGGERTHANOREQUALTO'; //gCSQListCB["Default"].offCWaitOP;//document.getElementById("offCWaitOP").value;
		gCSQListCB[csqName].offCWaitValue = document.getElementById("offCWaitValue").value;	
			
		gCSQListCB[csqName].offLongQEN = (document.getElementById("offLongQENT").checked ? "true" : "false");
		gCSQListCB[csqName].offLongQOP = 'SMALLERTHANOREQUALTO'; //gCSQListCB["Default"].offLongQOP;//document.getElementById("offLongQOP").value;
		gCSQListCB[csqName].offLongQValue = document.getElementById("offLongQValue").value;	
				
		gCSQListCB[csqName].offCbReqEN = (document.getElementById("offCbReqENT").checked ? "true" : "false");
		gCSQListCB[csqName].offCbReqOP = 'SMALLERTHANOREQUALTO'; //gCSQListCB["Default"].offCbReqOP;//document.getElementById("offCbReqOP").value;
		gCSQListCB[csqName].offCbReqValue = document.getElementById("offCbReqValue").value;	
			 */
		gCSQListCB[csqName].algTotIQEN = (document.getElementById("algTotIQENT").checked ? "true" : "false");
		gCSQListCB[csqName].algTotIQOP = 'SMALLERTHANOREQUALTO'; //gCSQListCB["Default"].algTotIQOP;//document.getElementById("algTotIQOP").value;
		gCSQListCB[csqName].algTotIQValue = document.getElementById("algTotIQValue").value;	
			
		gCSQListCB[csqName].algAgReadyEN = (document.getElementById("algAgReadyENT").checked ? "true" : "false");
		gCSQListCB[csqName].algAgReadyOP = 'BIGGERTHANOREQUALTO'; //gCSQListCB["Default"].algAgReadyOP;//document.getElementById("algAgReadyOP").value;
		gCSQListCB[csqName].algAgReadyValue = document.getElementById("algAgReadyValue").value;	
				
		gCSQListCB[csqName].algCallsWaitingEN = (document.getElementById("algCallsWaitingENT").checked ? "true" : "false");
		gCSQListCB[csqName].algCallsWaitingOP = 'SMALLERTHANOREQUALTO'; //gCSQListCB["Default"].algCallsWaitingOP;//document.getElementById("algCallsWaitingOP").value;
		gCSQListCB[csqName].algCallsWaitingValue = document.getElementById("algCallsWaitingValue").value;	
		
		gCSQListCB[csqName].cbProcTimeBegin = document.getElementById("cbProcTimeBegin").value;
		gCSQListCB[csqName].cbProcTimeEnd = document.getElementById("cbProcTimeEnd").value;	
		gCSQListCB[csqName].cbEndOfDayPurge = (document.getElementById("cbEndOfDayPurgeT").checked ? "true" : "false");
	}
	else
	{
		gCSQListCB[csqName].useDefaultSettings = true;
	}
}

function formatCSQtoXML(csqName)
{
	var xml = "<csq name='"+ csqName + "'>" +
					"<CallbackEnabled>" + gCSQListCB[csqName].cbEnabled + "</CallbackEnabled>";
	if((gCSQListCB[csqName].cbEnabled.toLowerCase() == "true") && (!gCSQListCB[csqName].useDefaultSettings))
	{	
		if(csqName == "Development")
			console.log("Format to save: " +  gCSQListCB[csqName].retPeriod);
		
		xml += "<CallerRecording>" + gCSQListCB[csqName].callerRecording + "</CallerRecording>" + 
				"<RetentionPeriod>" + gCSQListCB[csqName].retPeriod + "</RetentionPeriod>" +
				"<AppServerURLPrefix>" + gCSQListCB[csqName].appServerURL + "</AppServerURLPrefix>" +
				"<EmailAlerts>" + gCSQListCB[csqName].eMailAlerts + "</EmailAlerts>" +
				"<AdminEmail>" + gCSQListCB[csqName].adminEmail + "</AdminEmail>" +
				"<CallerIDVerify>" + gCSQListCB[csqName].cIdVerify + "</CallerIDVerify>" +
				"<AbandonCallback>" + gCSQListCB[csqName].abCallback + "</AbandonCallback>" +
				"<AbandonCBMinQTime>" + gCSQListCB[csqName].abMinCbTime + "</AbandonCBMinQTime>" +
				"<AbandonCBMinInterCallTime>" + gCSQListCB[csqName].abMinIcTime + "</AbandonCBMinInterCallTime>" +
				"<CBQueue csq='" + gCSQListCB[csqName].cbQ1name + "' overflowtime='" + gCSQListCB[csqName].cbQ1OFT + "'></CBQueue>" +
				"<CBQueue csq='" + gCSQListCB[csqName].cbQ2name + "' overflowtime='" + gCSQListCB[csqName].cbQ2OFT + "'></CBQueue>" +
				"<AcceptCallbacksTimeframe>" +
				"<Begin>" + gCSQListCB[csqName].acceptTfBegin + "</Begin>" +
				"<End>" + gCSQListCB[csqName].acceptTfEnd + "</End>" +
				"</AcceptCallbacksTimeframe>" +	
 				"<CallbackOfferedAlgorithm>" +
				"<AgentsLoggedIn Enabled='" + gCSQListCB[csqName].offAgLogInEN + "' Operation='" + gCSQListCB[csqName].offAgLogInOP + "' Value='" + gCSQListCB[csqName].offAgLogInValue + "'></AgentsLoggedIn>" +
				"<CallsWaiting Enabled='" + gCSQListCB[csqName].offCWaitEN + "' Operation='" + gCSQListCB[csqName].offCWaitOP + "' Value='" + gCSQListCB[csqName].offCWaitValue + "'></CallsWaiting>" +
				"<LongestQueueTime Enabled='" + gCSQListCB[csqName].offLongQEN + "' Operation='" + gCSQListCB[csqName].offLongQOP + "' Value='" + gCSQListCB[csqName].offLongQValue + "'></LongestQueueTime>" +
				"<CallbackRequests Enabled='" + gCSQListCB[csqName].offCbReqEN + "' Operation='" + gCSQListCB[csqName].offCbReqOP + "' Value='" + gCSQListCB[csqName].offCbReqValue + "'></CallbackRequests>" +
				"</CallbackOfferedAlgorithm>" +	 			
				"<CallbackReentryAlgorithm>" +
				"<TotalInQueue Enabled='" + gCSQListCB[csqName].algTotIQEN + "' Operation='" + gCSQListCB[csqName].algTotIQOP + "' Value='" + gCSQListCB[csqName].algTotIQValue + "'></TotalInQueue>" +
				"<CSQAgentsReady Enabled='" + gCSQListCB[csqName].algAgReadyEN + "' Operation='" + gCSQListCB[csqName].algAgReadyOP + "' Value='" + gCSQListCB[csqName].algAgReadyValue + "'></CSQAgentsReady>" +
				"<CSQCallsWaiting Enabled='" + gCSQListCB[csqName].algCallsWaitingEN + "' Operation='" + gCSQListCB[csqName].algCallsWaitingOP + "'  Value='" + gCSQListCB[csqName].algCallsWaitingValue + "'></CSQCallsWaiting>" +
				"<CallbackProcessingTimeframe>" +
				"<Begin>" + gCSQListCB[csqName].cbProcTimeBegin + "</Begin>" +
				"<End>" + gCSQListCB[csqName].cbProcTimeEnd + "</End>" +
				"</CallbackProcessingTimeframe>" +
				"<EndOfDayPurgeCallbackRequests>" + gCSQListCB[csqName].cbEndOfDayPurge + "</EndOfDayPurgeCallbackRequests>" +
				"</CallbackReentryAlgorithm>";
	}

	xml += "</csq>";
	
	return xml;
}

function ignoreClick() 
{
  document.getElementById("cbEnabledT").checked = true;
}

 // Called when 'Use Default Settings' is selected to fill in the default values
function setBgDisabled(disable)
{
	var btnSave = document.getElementById("btnSaveSettings");
	var csqName = btnSave.getAttribute("name");

	if(!disable)
	{
		document.getElementById("bgDefault").setAttribute('style','pointer-events:inherit;background-color:white');
		gCSQListCB[csqName].useDefaultSettings = false;
	}
	else
	{	
		gCSQListCB[csqName].useDefaultSettings = true;
		
		if(gCSQListCB["Default"].callerRecording.toLowerCase() == "true")
			document.getElementById("callerRecordingT").checked = true;
		else
			document.getElementById("callerRecordingF").checked = true;
		
		document.getElementById("retPeriod").value = gCSQListCB["Default"].retPeriod;
		
		gCSQListCB[csqName].appServerURL = gCSQListCB["Default"].appServerURL;
		
		if(gCSQListCB["Default"].eMailAlerts.toLowerCase() == "true")
			document.getElementById("eMailAlertsT").checked = true;
		else
			document.getElementById("eMailAlertsF").checked = true;		
		
		document.getElementById("adminEmail").value = gCSQListCB["Default"].adminEmail;	

		if(gCSQListCB["Default"].cIdVerify.toLowerCase() == "true")
			document.getElementById("cidVerifyT").checked = true;
		else
			document.getElementById("cidVerifyF").checked = true;			
		
		if(gCSQListCB["Default"].abCallback.toLowerCase() == "true")
			document.getElementById("abCallbackT").checked = true;
		else
			document.getElementById("abCallbackF").checked = true;	

		document.getElementById("abMinCbTime").value = gCSQListCB["Default"].abMinCbTime;
		document.getElementById("abMinIcTime").value = gCSQListCB["Default"].abMinIcTime;
		document.getElementById("cbQ1name").value = gCSQListCB["Default"].cbQ1name;
		document.getElementById("cbQ1OFT").value = gCSQListCB["Default"].cbQ1OFT;
		document.getElementById("cbQ2name").value = gCSQListCB["Default"].cbQ2name;
		document.getElementById("cbQ2OFT").value = gCSQListCB["Default"].cbQ2OFT;
		document.getElementById("acceptTfBegin").value = gCSQListCB["Default"].acceptTfBegin;
		document.getElementById("acceptTfEnd").value = gCSQListCB["Default"].acceptTfEnd;	

		
		if(gCSQListCB["Default"].offAgLogInEN.toLowerCase() == "true")
			document.getElementById("offAgLogInENT").checked = true;
		else
			document.getElementById("offAgLogInENF").checked = true;

		document.getElementById("offAgLogInValue").value = gCSQListCB["Default"].offAgLogInValue;
		
		if(gCSQListCB["Default"].offCWaitEN.toLowerCase() == "true")
			document.getElementById("offCWaitENT").checked = true;
		else
			document.getElementById("offCWaitENF").checked = true;
		
		document.getElementById("offCWaitValue").value = gCSQListCB["Default"].offCWaitValue;
		
		if(gCSQListCB["Default"].offLongQEN.toLowerCase() == "true")
			document.getElementById("offLongQENT").checked = true;
		else
			document.getElementById("offLongQENF").checked = true;

		document.getElementById("offLongQValue").value = gCSQListCB["Default"].offLongQValue;
				
		if(gCSQListCB["Default"].offCbReqEN.toLowerCase() == "true")
			document.getElementById("offCbReqENT").checked = true;
		else
			document.getElementById("offCbReqENF").checked = true;
		
		document.getElementById("offCbReqValue").value = gCSQListCB["Default"].offCbReqValue;
		
		if(gCSQListCB["Default"].algTotIQEN.toLowerCase() == "true")
			document.getElementById("algTotIQENT").checked = true;
		else
			document.getElementById("algTotIQENF").checked = true;
		
		document.getElementById("algTotIQValue").value = gCSQListCB["Default"].algTotIQValue;
			
		if(gCSQListCB["Default"].algAgReadyEN.toLowerCase() == "true")
			document.getElementById("algAgReadyENT").checked = true;
		else
			document.getElementById("algAgReadyENF").checked = true;		
		
		document.getElementById("algAgReadyValue").value = gCSQListCB["Default"].algAgReadyValue;

		if(gCSQListCB["Default"].algCallsWaitingEN.toLowerCase() == "true")
			document.getElementById("algCallsWaitingENT").checked = true;
		else
			document.getElementById("algCallsWaitingENF").checked = true;	

		document.getElementById("algCallsWaitingValue").value = gCSQListCB["Default"].algCallsWaitingValue;		
		
		document.getElementById("cbProcTimeBegin").value = gCSQListCB["Default"].cbProcTimeBegin;
		document.getElementById("cbProcTimeEnd").value = gCSQListCB["Default"].cbProcTimeEnd;
		
		if(gCSQListCB["Default"].cbEndOfDayPurge.toLowerCase() == "true")
			document.getElementById("cbEndOfDayPurgeT").checked = true;
		else
			document.getElementById("cbEndOfDayPurgeF").checked = true;	
		
		document.getElementById("bgDefault").setAttribute('style','pointer-events:none;background-color:lightgray');
	}
}

// Usage: var myNum = 150; if(myNum.between(100,200)){}
Number.prototype.between = function(first,last)
{
    return (first < last ? this >= first && this <= last : this >= last && this <= first);
}

Number.posInteger = function(value)
{
	if(Number.isInteger(value))
		if(value > -1)
			return true;
	return false;
		
}

// Polyfill for IE11 
Number.isInteger = Number.isInteger || function(value) 
{
    return typeof value === "number" && 
           isFinite(value) && 
           Math.floor(value) === value;
}

function generateUUID()
{
    var d = new Date().getTime();
    if(window.performance && typeof window.performance.now === "function"){
        d += performance.now(); //use high-precision timer if available
    }
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = (d + Math.random()*16)%16 | 0;
        d = Math.floor(d/16);
        return (c=='x' ? r : (r&0x3|0x8)).toString(16);
    });
    return uuid;
}